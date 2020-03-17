using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Text;

namespace GooglePhotoWallpaperREST
{
    public class GooglePhotosAlbumsCollection
    {
        public ObservableCollection<GooglePhotosAlbum> albums { get; set; }
        public string nextPageToken { get; set; }

    }
}
