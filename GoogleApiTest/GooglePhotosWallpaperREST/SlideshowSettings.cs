using System;
using System.Collections.Generic;
using System.IO;
using Newtonsoft.Json;

namespace GooglePhotoWallpaperREST
{
    class SlideshowSettings
    {
        private readonly string settingsFile = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "SlideshowSettings");

        public OrderBy orderBy;
        public Order order;
        public Wallpaper.Wallpaper.Style WallpaperStyle = Wallpaper.Wallpaper.Style.Centered;

        public List<string> selectedAlbumIds = new List<string>();
        public bool displayFavorites;

        public SlideshowSettings()
        {
            //LoadSettings();
        }

        private void LoadSettings()
        {
            var tmp = JsonConvert.DeserializeObject<SlideshowSettings>(settingsFile);
            selectedAlbumIds = tmp.selectedAlbumIds;
            displayFavorites = tmp.displayFavorites;
            order = tmp.order;
            orderBy = tmp.orderBy;
        }

        public void SaveSettings()
        {
            File.WriteAllText(settingsFile, JsonConvert.SerializeObject(this));
        }

        public void AddSelectedAlbumId(string anAlbumId)
        {
            if (!selectedAlbumIds.Contains(anAlbumId))
            {
                selectedAlbumIds.Add(anAlbumId);
            }
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
