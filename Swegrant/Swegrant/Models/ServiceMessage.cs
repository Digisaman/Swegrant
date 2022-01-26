using System;
using System.Collections.Generic;
using System.Text;

namespace Swegrant.Models
{
    public class ServiceMessage
    {

        public Command Command { get; set; }
        public Mode Mode { get; set; }
        public int Scene { get; set; }
        public string Text { get; set; }
    }
}
