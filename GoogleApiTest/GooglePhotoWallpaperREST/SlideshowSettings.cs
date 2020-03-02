using System;
using System.Collections.Generic;
using System.IO;
using Newtonsoft.Json;

namespace GooglePhotoWallpaperREST
{
    class SlideshowSettings
    {
        public OrderBy orderBy;
        public Order order;

        public List<string> selectedAlbumIds = new List<string>();
        public bool displayFavorites;

        public void SaveSettings()
        {
            File.WriteAllText(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "SlideshowSettings"), JsonConvert.SerializeObject(this));
        }
    }

    enum OrderBy
    {
        DateTime,
        FileName
    }

    enum Order
    {
        Ascending,
        Descending,
        Random
    }
}
