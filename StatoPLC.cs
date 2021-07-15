using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Progetto_Main
{
    class StatoPLC
    {
        public bool IsInWarning { get; set; }
        public bool IsInErrore { get; set; } 
        public bool IsOnline { get; set; } // se risponde al ping
        public bool IsWorking { get; set; } // se sta lavorando
        public bool AbilitazioneDaUfficio { get; set; }
        public string commessaInCorso { get; set; }
        public string clienteInCorso { get; set; }
        public int pezziTotali { get; set; }
        public int pezziBuoni { get; set; }
        public int pezziScarti { get; set; }
    }
}
