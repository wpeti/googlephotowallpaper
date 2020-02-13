using Google.Apis.Auth.OAuth2;
using Google.Apis.Discovery.v1;
using Google.Apis.Discovery.v1.Data;
using Google.Apis.Drive.v3;
using Google.Apis.Services;
using Google.Apis.Util.Store;
using System;
using System.Collections.Generic;
using System.IO;
using System.Threading;
using System.Threading.Tasks;

namespace GoogleApiTest
{
    class Program
    {
        // If modifying these scopes, delete your previously saved credentials
        // at ~/.credentials/drive-dotnet-quickstart.json
        static string[] Scopes = { DriveService.Scope.DrivePhotosReadonly };
        static string ApplicationName = "Drive API .NET Quickstart";

        static void Main(string[] args)
        {
            UserCredential credential;

            using (var stream =
                new FileStream("creds.json", FileMode.Open, FileAccess.Read))
            {
                // The file token.json stores the user's access and refresh tokens, and is created
                // automatically when the authorization flow completes for the first time.
                string credPath = "token.json";
                credential = GoogleWebAuthorizationBroker.AuthorizeAsync(
                    GoogleClientSecrets.Load(stream).Secrets,
                    Scopes,
                    "user",
                    CancellationToken.None,
                    new FileDataStore(credPath, true)).Result;
                Console.WriteLine("Credential file saved to: " + credPath);
            }

            // Create Drive API service.
            var service = new DriveService(new BaseClientService.Initializer()
            {
                HttpClientInitializer = credential,
                ApplicationName = ApplicationName,
            });

            // Define parameters of request.
            FilesResource.ListRequest listRequest = service.Files.List();
            listRequest.PageSize = 10;
            //listRequest.Fields = "nextPageToken, files(id, name)";
            listRequest.Fields = "nextPageToken, files(*)";
            //listRequest.Q = "'1SCrpFATrqiw6V59tE4CSlBX_KMwxypG3' in parents";
            //listRequest.Spaces = "photos";
            //listRequest.Q = "mimeType='application/vnd.google-apps.folder' and '1SCrpFATrqiw6V59tE4CSlBX_KMwxypG3' in parents";

            Console.WriteLine("Files:");

            do
            {
                // List files.
                IList<Google.Apis.Drive.v3.Data.File> files = listRequest.Execute()
                    .Files;
                if (files != null && files.Count > 0)
                {
                    foreach (var file in files)
                    {
                        //if (file.MimeType.Equals("application/vnd.google-apps.folder"))
                        {
                            Console.WriteLine("{0} -- ({1})", file.Name, file.Id);
                        }
                    }
                }
                else
                {
                    Console.WriteLine("No files found.");
                }

                listRequest.PageToken = listRequest.Execute().NextPageToken;
            }
            while (!string.IsNullOrEmpty(listRequest.PageToken));


            Console.WriteLine("Any key to exit...");
            Console.Read();
        }
    }
}
