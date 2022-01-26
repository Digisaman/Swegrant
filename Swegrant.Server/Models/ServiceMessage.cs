using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Swegrant.Server.Models
{
    public class ServiceMessage
    {

        public Command Command { get; set; }
        public Mode Mode { get; set; }
        public int Scene { get; set; }
        public string Text { get; set; }
    }
}
