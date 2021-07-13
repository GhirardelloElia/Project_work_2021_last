using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Progetto_Main
{
    public class PLCGlobals
    {
        // Dati per la commessa corrente 

        public static readonly string Codice_commessa_in_corso = "ns=5;i=8";
        public static readonly string PezziProdotti = "ns=5;i=13";
        public static readonly string PezziScarti = "ns=5;i=14";
        public static readonly string PezziMancanti = "ns=5;i=15";
        public static readonly string PezziDaProdurre = "ns=5;i=9";
        public static readonly string Codice_prodotto = "ns=5;i=10";
        public static readonly string Target_velocita = "ns=5;i=11";
        public static readonly string Codice_cliente = "ns=5;i=12";

        // Dati per il management della coda

        public static readonly string Sovrascrittura = "ns=5;i=25";
        public static readonly string Codice_commessa_coda = "ns=5;i=20";
        public static readonly string PezziDaProdurre_coda = "ns=5;i=21";
        public static readonly string Codice_prodotto_coda = "ns=5;i=22";
        public static readonly string Target_velocita_coda = "ns=5;i=23";
        public static readonly string Codice_cliente_coda = "ns=5;i=24";

        // Dati comunicazione ufficio

        public static readonly string MessaggioPerUfficio = "";
        public static readonly string MessaggioDaUfficio = "ns=5;i=59";
        public static readonly string AbilitazioneDaUfficio = "ns=5;i=59";

        // Stato macchina

        public static readonly string StatoMacchina = "ns=5;i=31";

        // Info generali

        public static readonly string PezziTOT = "ns=5;i=38";
        public static readonly string OreTOT = "ns=5;i=39";
        public static readonly string Velocita_attuale = "ns=5;i=40";
    }
}
