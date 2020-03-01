using System;
using System.Collections.Generic;
using System.Text;

namespace GooglePhotoWallpaperREST
{
    class SlideshowSettings
    {
        SortBy sorting;
        DisplayOrder order;

        List<string> selectedAlbumIds;
        bool favorites;
    }

    enum SortBy
    {
        DateTime,
        FileName
    }

    enum DisplayOrder
    {
        Ascending,
        Descending,
        Random
    }
}
