using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Progetto_Main
{
    class Messaggio
    {
        public string Text { get; set; }
        public bool IsSent { get; set; } // se 1 = Lo ha mandato il PLC, se 0 = mandato ufficio
        public string TimeStamp { get; set; }
    }
}
