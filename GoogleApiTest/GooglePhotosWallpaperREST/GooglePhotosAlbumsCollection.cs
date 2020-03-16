using System;
using System.Collections.Generic;
using System.Text;

namespace GooglePhotoWallpaperREST
{
    class GooglePhotosAlbumsCollection
    {
        public List<GooglePhotosAlbum> albums { get; set; }
        public string nextPageToken { get; set; }

    }
}
