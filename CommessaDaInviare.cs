using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Progetto_Main
{
    public class CommessaDaInviare
    {
        public string CodiceCommessa { get; set; }
        public string NomeProdotto { get; set; } 
        public Int16 Quantita { get; set; }
        public string NomeCliente { get; set; }
        public float VelocitaMacchina { get; set; }
    }
}