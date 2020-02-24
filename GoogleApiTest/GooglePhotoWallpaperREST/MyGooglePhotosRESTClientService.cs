using Google.Apis.Services;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace GooglePhotoWallpaperREST
{
    class MyGooglePhotosRESTClientService : BaseClientService
    {
        public MyGooglePhotosRESTClientService(Initializer initializer) : base(initializer)
        {
        }
        public override string Name => throw new NotImplementedException();

        public override string BaseUri => throw new NotImplementedException();

        public override string BasePath => throw new NotImplementedException();

        public override IList<string> Features => throw new NotImplementedException();

        public async Task<GooglePhotosMediaItemsCollection> SearchMediaItems(int pageSize = 0, string pageToken = "")
        {
            string url = @"https://photoslibrary.googleapis.com/v1/mediaItems:search";

            var values = new Dictionary<string, string>
            {
                { "pageToken", pageToken },
                { "pageSize", pageSize.ToString() },
                { "filters",  "{featureFilter: { includedFeatures: [FAVORITES]}}"}
            };

            FormUrlEncodedContent content = new FormUrlEncodedContent(values);

            var response = await base.HttpClient.PostAsync(url, content);

            response.EnsureSuccessStatusCode();

            return JsonConvert.DeserializeObject<GooglePhotosMediaItemsCollection>(await response.Content.ReadAsStringAsync());
        }


        public async Task<GooglePhotosMediaItemsCollection> ListMediaItems(int pageSize = 0, string pageToken = "")
        {
            string url = @"https://photoslibrary.googleapis.com/v1/mediaItems";

            if (pageSize > 0)
            {
                url = ExtendUrlWithPageSize(url, pageSize);
            }
            if (!string.IsNullOrEmpty(pageToken))
            {
                url = ExtendUrlWithPageToken(url, pageToken);
            }

            var response = await base.HttpClient.GetAsync(url);

            response.EnsureSuccessStatusCode();

            return JsonConvert.DeserializeObject<GooglePhotosMediaItemsCollection>(await response.Content.ReadAsStringAsync());
        }

        internal async Task<GooglePhotosAlbumsCollection> FetchAlbums(string nextPageToken)
        {
            return await FetchAlbums(20, nextPageToken);
        }

        public async Task<GooglePhotosAlbumsCollection> FetchAlbums(int pageSize = 20, string pageToken = "")
        {
            string url = @"https://photoslibrary.googleapis.com/v1/albums";

            if (pageSize > 0)
            {
                url = ExtendUrlWithPageSize(url, pageSize);
            }
            if (!string.IsNullOrEmpty(pageToken))
            {
                url = ExtendUrlWithPageToken(url, pageToken);
            }
                
            var response = await base.HttpClient.GetAsync(url);

            response.EnsureSuccessStatusCode();

            string content = await response.Content.ReadAsStringAsync();

            return JsonConvert.DeserializeObject<GooglePhotosAlbumsCollection>(content);
        }

        private string ExtendUrlWithPageToken(string url, string pageToken)
        {
            return ExtendUrlWithParameter(url, "pageToken", pageToken);
        }

        private string ExtendUrlWithPageSize(string url, int pageSize)
        {
            return ExtendUrlWithParameter(url, "pageSize", pageSize.ToString());
        }

        private string ExtendUrlWithParameter(string url, string parameterName, string parameterValue)
        {
            url = AppendOperandSign(url);

            url += parameterName + "=" + parameterValue;

            return url;
        }

        private string AppendOperandSign(string url)
        {
            if (!url.Contains("?"))
            {
                url += "?";
            }
            else
            {
                url += "&";
            }

            return url;
        }
    }
}
