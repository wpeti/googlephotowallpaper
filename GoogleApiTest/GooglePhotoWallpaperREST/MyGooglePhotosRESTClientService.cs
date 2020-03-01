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

                //foreach (var aMediaItem in mediasCache.mediaItems)
                //{
                //    tmpFile = @"C:\tmp\" + /*anAlbum.title +*/ "_" + mediaItemCount + ".png";
                //    Console.WriteLine(mediaItemCount++ + ". " + aMediaItem.Filename + " -- " + tmpFile + " -- " + aMediaItem.ProductUrl.ToString());
                //    downloader.DownloadFile(aMediaItem.BaseUrl + "=w1600-900", tmpFile);
                //    Console.WriteLine(Wallpaper.Wallpaper.Get());
                //    Wallpaper.Wallpaper.Set(new Uri(aMediaItem.BaseUrl.AbsoluteUri + "=w1600-h900"), Wallpaper.Wallpaper.Style.Tiled);
                //    Thread.Sleep(1000);
                //}
            }
            while (!string.IsNullOrEmpty(mediasCache.nextPageToken));

            return mediasCache;
        }

        public async Task<GooglePhotosMediaItemsCollection> FetchPhotosOfAlbum(string albumID, int pageSize = 0, string pageToken = "")
        {
            object filterCriteria = new
            {
                albumId = albumID,
                mediaTypeFilter = new
                {
                    mediaTypes = new[] { "PHOTO" }
                }
            };

            return await SearchMediaItems(filterCriteria, pageSize, pageToken);
        }
        public async Task<GooglePhotosMediaItemsCollection> SearchFavoredPhotos(int pageSize = 0, string pageToken = "")
        {
            object filterCriteria = new
            {
                mediaTypeFilter = new
                {
                    mediaTypes = new[] { "PHOTO" }
                },
                featureFilter = new
                {
                    includedFeatures = new[] { "FAVORITES" }
                }
            };

            return await SearchMediaItems(filterCriteria, pageSize, pageToken);
        }
        public async Task<GooglePhotosMediaItemsCollection> SearchMediaItems(object filterCriteria, int pageSize = 0, string pageToken = "")
        {
            string url = @"https://photoslibrary.googleapis.com/v1/mediaItems:search";

            string searchCriteria = JsonConvert.SerializeObject(
                new
                {
                    pageSize = pageSize.ToString(),
                    pageToken = pageToken,
                    filters = filterCriteria
                });

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

                //foreach (var anAlbum in albums.albums)
                //{
                //    //tmpFile = @"C:\tmp\" + /*anAlbum.title +*/ "_" + albumCount + ".png";
                //    //Console.WriteLine(albumCount++ + ". " + anAlbum.title + " -- " + tmpFile + "--" + anAlbum.id);
                //    //downloader.DownloadFile(new Uri(anAlbum.coverPhotoBaseUrl), tmpFile);
                //    //Wallpaper.Wallpaper.Set(new Uri(anAlbum.coverPhotoBaseUrl), Wallpaper.Wallpaper.Style.Centered);
                //}
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
