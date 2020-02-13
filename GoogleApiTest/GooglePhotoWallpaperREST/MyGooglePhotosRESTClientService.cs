using Google.Apis.Services;
using System;
using System.Collections.Generic;
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
    }
}
