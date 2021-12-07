using Microsoft.AspNetCore.Mvc;
using System;


namespace Swegrant.Server.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class MediaController : ControllerBase
    {
        [HttpGet]
        public DateTime GetCurrentTime()
        {
            return DateTime.Now;
        }

    }
}
