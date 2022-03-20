using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using Swegrant.Shared.Models;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;

namespace Swegrant.Server.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class SubtitleController : ControllerBase
    {
        [HttpGet]
        [Route(nameof(HideSubtitle))]
        public bool HideSubtitle()
        {
            try
            {
                MainWindow.Singleton.HideSubtitle();
            }
            catch(Exception ex)
            {
                return false;
            }
            return true;
        }

        [HttpGet]
        [Route(nameof(ShowSubtitle))]
        public bool ShowSubtitle()
        {
            try
            {
                MainWindow.Singleton.ShowSubtitle();
            }
            catch (Exception ex)
            {
                return false;
            }
            return true;
        }

        [HttpGet]
        [Route(nameof(ResumeAutoSub))]
        public bool ResumeAutoSub()
        {
            try
            {
                MainWindow.Singleton.ResumeAutoSub();
            }
            catch (Exception ex)
            {
                return false;
            }
            return true;
        }

        [HttpGet]
        [Route(nameof(PauseAutoSub))]
        public bool PauseAutoSub()
        {
            try
            {
                MainWindow.Singleton.PauseAutoSub();
            }
            catch (Exception ex)
            {
                return false;
            }
            return true;
        }

        [HttpGet]
        [Route(nameof(NextMaunualSub))]
        public bool NextMaunualSub()
        {
            try
            {
                MainWindow.Singleton.NextMaunualSub();
            }
            catch (Exception ex)
            {
                return false;
            }
            return true;
        }



    }
}
