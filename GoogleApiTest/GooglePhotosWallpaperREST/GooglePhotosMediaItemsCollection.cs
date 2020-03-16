using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace GooglePhotoWallpaperREST
{
    class GooglePhotosMediaItemsCollection : GooglePhotosCollectionBase
    {
        public List<GooglePhotosMediaItem> mediaItems { get; set; }
    }

    public partial class GooglePhotosMediaItem
    {
        [JsonProperty("id")]
        public string Id { get; set; }

        [JsonProperty("productUrl")]
        public Uri ProductUrl { get; set; }

        [JsonProperty("baseUrl")]
        public Uri BaseUrl { get; set; }

        [JsonProperty("mimeType")]
        public string MimeType { get; set; }

        [JsonProperty("mediaMetadata")]
        public MediaMetadata MediaMetadata { get; set; }

        [JsonProperty("filename")]
        public string Filename { get; set; }
    }

    public partial class MediaMetadata
    {
        [JsonProperty("creationTime")]
        public DateTimeOffset CreationTime { get; set; }

        [JsonProperty("width")]
        public string Width { get; set; }

        [JsonProperty("height")]
        public string Height { get; set; }

        [JsonProperty("photo")]
        public Photo Photo { get; set; }
    }

    public partial class Photo
    {
        [JsonProperty("cameraMake")]
        public string CameraMake { get; set; }

        [JsonProperty("cameraModel")]
        public string CameraModel { get; set; }

        [JsonProperty("focalLength")]
        public double FocalLength { get; set; }

        [JsonProperty("apertureFNumber", NullValueHandling = NullValueHandling.Ignore)]
        public double? ApertureFNumber { get; set; }

        [JsonProperty("isoEquivalent", NullValueHandling = NullValueHandling.Ignore)]
        public long? IsoEquivalent { get; set; }
    }
}
