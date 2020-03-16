using Google.Apis.Services;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Net;
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

        internal async Task<GooglePhotosMediaItemsCollection> FetchAllFavoredPhotos()
        {
            GooglePhotosMediaItemsCollection mediasCache = new GooglePhotosMediaItemsCollection();
            GooglePhotosMediaItemsCollection allFavoredPhotos = new GooglePhotosMediaItemsCollection() { mediaItems = new List<GooglePhotosMediaItem>()};

            do
            {
                mediasCache = await SearchFavoredPhotos(100, mediasCache.nextPageToken);

                allFavoredPhotos.mediaItems.AddRange(mediasCache.mediaItems);
                allFavoredPhotos.nextPageToken = mediasCache.nextPageToken;
            }
            while (!string.IsNullOrEmpty(mediasCache.nextPageToken));

            return mediasCache;
        }

        public async Task<GooglePhotosMediaItemsCollection> FetchAllPhotosOfAlbum(string albumID)
        {
            GooglePhotosMediaItemsCollection googlePhotosMediaItemsCollection = new GooglePhotosMediaItemsCollection() { mediaItems = new List<GooglePhotosMediaItem>()};
            GooglePhotosMediaItemsCollection cacheGooglePhotosMediaItemsCollection = new GooglePhotosMediaItemsCollection();

            do
            {
                cacheGooglePhotosMediaItemsCollection = await FetchPhotosOfAlbum(albumID, 100, cacheGooglePhotosMediaItemsCollection.nextPageToken);

                if (cacheGooglePhotosMediaItemsCollection.mediaItems.Count != 0)
                    googlePhotosMediaItemsCollection.mediaItems.AddRange(cacheGooglePhotosMediaItemsCollection.mediaItems);

                googlePhotosMediaItemsCollection.nextPageToken = cacheGooglePhotosMediaItemsCollection.nextPageToken;

            } while (!string.IsNullOrEmpty(cacheGooglePhotosMediaItemsCollection.nextPageToken)) ;


            return googlePhotosMediaItemsCollection;
        }

        public async Task<GooglePhotosMediaItemsCollection> FetchPhotosOfAlbum(string albumID, int pageSize = 0, string pageToken = "")
        {
            //if Album ID is set, filters can't be
            //https://developers.google.com/photos/library/reference/rest/v1/mediaItems/search
            string searchCriteria = JsonConvert.SerializeObject(new
            {
                pageSize = pageSize.ToString(),
                pageToken = pageToken,
                albumId = albumID
            });

            GooglePhotosMediaItemsCollection cacheCollection = await SearchMediaItems(searchCriteria, pageSize, pageToken);

            cacheCollection.mediaItems.RemoveAll(m => !m.MimeType.Equals("image/jpeg"));

            return cacheCollection;
        }
        public async Task<GooglePhotosMediaItemsCollection> SearchFavoredPhotos(int pageSize = 0, string pageToken = "")
        {
            string searchCriteria = JsonConvert.SerializeObject(new
            {
                pageSize = pageSize.ToString(),
                pageToken = pageToken,
                filters = new
                {
                    mediaTypeFilter = new
                    {
                        mediaTypes = new[] { "PHOTO" }
                    },
                    featureFilter = new
                    {
                        includedFeatures = new[] { "FAVORITES" }
                    }
                }
            });

            return await SearchMediaItems(searchCriteria, pageSize, pageToken);
        }
        public async Task<GooglePhotosMediaItemsCollection> SearchMediaItems(string searchCriteria, int pageSize = 0, string pageToken = "")
        {
            string url = @"https://photoslibrary.googleapis.com/v1/mediaItems:search";

            StringContent content = new StringContent(searchCriteria);
            
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

        internal async Task<GooglePhotosAlbumsCollection> FetchAllAlbums()
        {
            GooglePhotosAlbumsCollection albums = new GooglePhotosAlbumsCollection() { albums = new List<GooglePhotosAlbum>() };

            GooglePhotosAlbumsCollection albumsCache = new GooglePhotosAlbumsCollection();

            do
            {
                albumsCache = await FetchAlbums(albumsCache.nextPageToken);

                albums.albums.AddRange(albumsCache.albums);
                albums.nextPageToken = albumsCache.nextPageToken;
            }
            while (!string.IsNullOrEmpty(albumsCache.nextPageToken));

            return albums;
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
