using DevUpdater.Server.Services;
using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using System.Web.Http;
using System.Web.Http.ModelBinding;

namespace DevUpdater.Server.Controllers
{
    [Authorize]
    public class SettingsController :  ApiController
    {
        public SettingsService SettingsService { get; set; }

        public IHttpActionResult Get()
        {
            Models.SettingsModel result = SettingsService.GetSettings();
            return Ok(result);
        }
    }
}
