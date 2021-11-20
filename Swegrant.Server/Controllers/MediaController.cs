using System;
using System.Web.Http;

namespace Swegrant.Server.Controllers
{
    public class MediaController : ApiController
    {
        [HttpGet]
        public DateTime GetCurrentTime()
        {
            return DateTime.Now;
        }

    }
}
