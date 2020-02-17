using Google.Apis.Auth.OAuth2;
using Google.Apis.Services;
using Google.Apis.Util.Store;
using Newtonsoft;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
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

            int albumCount = 1;
            string tmpFile = string.Empty;
            GooglePhotosAlbumsCollection albums = new GooglePhotosAlbumsCollection();

            using (WebClient downloader = new WebClient())
            {
                do
                {
                    albums = await service.FetchAlbums(albums.nextPageToken);

                    foreach (var anAlbum in albums.albums)
                    {
                        tmpFile = @"C:\tmp\" + /*anAlbum.title +*/ "_" + albumCount + ".png";
                        Console.WriteLine(albumCount++ + ". " + anAlbum.title + " -- " + tmpFile + "--" + anAlbum.id);
                        downloader.DownloadFile(new Uri(anAlbum.coverPhotoBaseUrl), tmpFile);

                        Wallpaper.Wallpaper.Set(new Uri(anAlbum.coverPhotoBaseUrl), Wallpaper.Wallpaper.Style.Centered);
                    }
                }
                while (!string.IsNullOrEmpty(albums.nextPageToken));
            }
        }
    }
}

