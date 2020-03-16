using Google.Apis.Auth.OAuth2;
using Google.Apis.Services;
using Google.Apis.Util.Store;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
using System.Threading;
using System.Threading.Tasks;

//installation steps
// host file "C:\Windows\System32\drivers\etc\hosts" needs to have "	127.0.0.1       localhost" line. To edit host file, elevated permissions required
// download credential of OAuth 2.0 client  https://console.developers.google.com/apis/credentials?folder=&organizationId=&project=sonorous-treat-268116

namespace GooglePhotoWallpaperREST
{
    /// <summary>
    /// Sample which demonstrates how to use the Books API.
    /// https://developers.google.com/books/docs/v1/getting_started
    /// <summary>
    public class Program
    {
        static string[] Scopes = new[] { @"https://www.googleapis.com/auth/photoslibrary.readonly" };
        public MyGooglePhotosRESTClientService service;

        [STAThread]
        static void Main(string[] args)
        {
            Console.WriteLine("Books API Sample: List MyLibrary");
            Console.WriteLine("================================");
            try
            {
                Program prog = new Program();
                prog.SignInAndInitiateService().Wait();
                prog.Run().Wait();
            }
            catch (AggregateException ex)
            {
                Console.WriteLine(ex.ToString());
                foreach (var e in ex.InnerExceptions)
                {
                    Console.WriteLine("ERROR: " + e.Message);
                }
            }
            Console.WriteLine("Press any key to continue...");
            Console.ReadKey();
        }

        public async Task SignInAndInitiateService()
        {
            UserCredential credential;
            using (var stream = new FileStream("creds.json", FileMode.Open, FileAccess.Read))
            {
                string credPath = "token.json";
                credential = await GoogleWebAuthorizationBroker.AuthorizeAsync(
                    GoogleClientSecrets.Load(stream).Secrets,
                    Scopes,
                    "user",
                    CancellationToken.None,
                    new FileDataStore(credPath, true));
            }

            // Create the service.
            service = new MyGooglePhotosRESTClientService(new BaseClientService.Initializer()
            {
                HttpClientInitializer = credential,
                ApplicationName = "My Google Photos Wallpaper",
                GZipEnabled = false,
            });
        }

        private async Task Run()
        {
            SlideshowSettings slideshowSettings = new SlideshowSettings();
            slideshowSettings.displayFavorites = true;
            
            int albumCount = 0;
            GooglePhotosAlbumsCollection albums = await service.FetchAllAlbums();

            foreach (var anAlbum in albums.albums)
            {
                Console.WriteLine("{0}) {1}", albumCount++, anAlbum.title);
                slideshowSettings.AddSelectedAlbumId(anAlbum.id);
            }

            int mediaItemCount = 0;
            GooglePhotosMediaItemsCollection medias = await service.FetchAllFavoredPhotos();
            foreach (var media in medias.mediaItems)
            {
                Console.WriteLine("{0}) {1}", mediaItemCount++, media.Filename);
            }

            List<GooglePhotosMediaItem> photosToSlideshow = new List<GooglePhotosMediaItem>();

            var sw = Stopwatch.StartNew();
            //parallel fetching of photos
            Parallel.ForEach(slideshowSettings.selectedAlbumIds, (anAlbumId) =>
            {
                photosToSlideshow.AddRange((service.FetchAllPhotosOfAlbum(anAlbumId)).Result.mediaItems);
            });

            //serial fetching of photos
            //foreach (var anAlbumId in slideshowSettings.selectedAlbumIds)
            //{
            //    photosToSlideshow.AddRange((await service.FetchAllPhotosOfAlbum(anAlbumId)).mediaItems);
            //}

            sw.Stop();
            Console.WriteLine("Milliseconds elapsed while fetching photos: {0} [ms]", sw.ElapsedMilliseconds / 1000);

            photosToSlideshow.AddRange((await service.FetchAllFavoredPhotos()).mediaItems);

            switch (slideshowSettings.orderBy)
            {
                case OrderBy.DateTime:
                    switch (slideshowSettings.order)
                    {
                        case Order.Ascending:
                            photosToSlideshow = photosToSlideshow.OrderBy(m => m.MediaMetadata.CreationTime).ToList();
                            break;
                        case Order.Descending:
                            photosToSlideshow = photosToSlideshow.OrderByDescending(m => m.MediaMetadata.CreationTime).ToList();
                            break;
                        case Order.Random:
                            photosToSlideshow = photosToSlideshow.OrderBy(m => m.Id).ToList();
                            break;
                        default:
                            break;
                    }
                    break;
                case OrderBy.FileName:
                    switch (slideshowSettings.order)
                    {
                        case Order.Ascending:
                            photosToSlideshow = photosToSlideshow.OrderBy(m => m.Filename).ToList();
                            break;
                        case Order.Descending:
                            photosToSlideshow = photosToSlideshow.OrderByDescending(m => m.Filename).ToList();
                            break;
                        case Order.Random:
                            photosToSlideshow = photosToSlideshow.OrderBy(m => m.Id).ToList();
                            break;
                        default:
                            break;
                    }
                    break;
                default:
                    break;
            }

            foreach (var aPhoto in photosToSlideshow)
            {
                Wallpaper.Wallpaper.Set(new Uri(aPhoto.BaseUrl.AbsoluteUri + "=w1600-h900"), slideshowSettings.WallpaperStyle);
                Thread.Sleep(1000);
            }

            //slideshowSettings.SaveSettings();
        }
    }
}

