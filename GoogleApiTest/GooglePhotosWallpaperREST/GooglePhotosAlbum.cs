using System;
using System.Collections.Generic;
using System.Text;

namespace GooglePhotoWallpaperREST
{
    class GooglePhotosAlbum
    {
        public string id { get; set; }
        public string title { get; set; }
        public string productUrl { get; set; }
        public int mediaItemsCount { get; set; }
        public string coverPhotoBaseUrl { get; set; }
        public string coverPhotoMediaItemId { get; set; }
    }
}
