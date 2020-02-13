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
            //Console.WriteLine("Apikey: ", ApiKey);
            //Console.WriteLine("ApplicationName: ", ApplicationName);
            //Console.WriteLine("HttpClient base addr: ", base.HttpClient.BaseAddress.ToString());
        }

        //public async Task<string> HttpGet(string requestUri)
        //{
        //    return await HttpClient.GetStringAsync("");
        //}
        public override string Name => throw new NotImplementedException();

        public override string BaseUri => throw new NotImplementedException();

        public override string BasePath => throw new NotImplementedException();

        public override IList<string> Features => throw new NotImplementedException();
    }
}
