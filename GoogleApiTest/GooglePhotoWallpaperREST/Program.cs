using Google.Apis.Auth.OAuth2;
using Google.Apis.Services;
using Google.Apis.Util.Store;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Net;
using System.Threading;
using System.Threading.Tasks;

//installation steps
// host file "C:\Windows\System32\drivers\etc\hosts" needs to have "	127.0.0.1       localhost" line. To edit host file, elevated permissions required

namespace GooglePhotoWallpaperREST
{
    /// <summary>
    /// Sample which demonstrates how to use the Books API.
    /// https://developers.google.com/books/docs/v1/getting_started
    /// <summary>
    internal class Program
    {
        static string[] Scopes = new[] { @"https://www.googleapis.com/auth/photoslibrary.readonly" };

        [STAThread]
        static void Main(string[] args)
        {
            Console.WriteLine("Books API Sample: List MyLibrary");
            Console.WriteLine("================================");
            try
            {
                new Program().Run().Wait();
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

        private async Task Run()
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
            var service = new MyGooglePhotosRESTClientService(new BaseClientService.Initializer()
            {
                HttpClientInitializer = credential,
                ApplicationName = "Google Photo Wallpaper - REST",
                GZipEnabled = false,
            });

            //GooglePhotosMediaItemsCollection mediaItems = await service.ListMediaItems(1);
            //foreach (var mediaItem in mediaItems.mediaItems)
            //{
            //    Console.WriteLine(mediaItem.Filename + " -- " + mediaItem.Id);
            //}
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
            Parallel.ForEach(slideshowSettings.selectedAlbumIds, (anAlbumId) =>
            {
                photosToSlideshow.AddRange((service.FetchAllPhotosOfAlbum(anAlbumId)).Result.mediaItems);
            });

            //foreach (var anAlbumId in slideshowSettings.selectedAlbumIds)
            //{
            //    photosToSlideshow.AddRange((await service.FetchAllPhotosOfAlbum(anAlbumId)).mediaItems);
            //}
            sw.Stop();
            Console.WriteLine(sw.ElapsedMilliseconds / 1000);

            photosToSlideshow.AddRange((await service.FetchAllFavoredPhotos()).mediaItems);

            foreach (var aPhoto in photosToSlideshow)
            {
                //if Album ID is set, filters can't be
                //https://developers.google.com/photos/library/reference/rest/v1/mediaItems/search
                Wallpaper.Wallpaper.Set(new Uri(aPhoto.BaseUrl.AbsoluteUri + "=w1600-h900"), slideshowSettings.WallpaperStyle);
                Thread.Sleep(1000);
            }

            //slideshowSettings.SaveSettings();
        }
    }
}

