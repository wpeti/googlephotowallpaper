using System;
using System.Collections.Generic;
using System.Text;

namespace GooglePhotoWallpaperREST
{
    abstract class GooglePhotosCollectionBase
    {
        public string nextPageToken { get; set; }
    }
}
