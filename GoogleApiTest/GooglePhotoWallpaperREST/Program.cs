using Google.Apis.Auth.OAuth2;
using Google.Apis.Services;
using Google.Apis.Util.Store;
using Newtonsoft;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Threading;
using System.Threading.Tasks;

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

            var response = await service.HttpClient.GetAsync(@"https://photoslibrary.googleapis.com/v1/albums");
            response.EnsureSuccessStatusCode();

            string result = await response.Content.ReadAsStringAsync();
            Console.WriteLine(result);

            var albums = JsonConvert.DeserializeObject<GooglePhotosAlbumsCollection>(result);

            Console.WriteLine("deserialized");

            //var bookshelves = await service.Mylibrary.Bookshelves.List().ExecuteAsync();

        }
    }
}

