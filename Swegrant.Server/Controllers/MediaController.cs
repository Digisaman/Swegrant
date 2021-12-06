using Microsoft.AspNetCore.Mvc;
using System;


namespace Swegrant.Server.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class MediaController 
    {
        [HttpGet]
        public DateTime GetCurrentTime()
        {
            return DateTime.Now;
        }

    }
}
