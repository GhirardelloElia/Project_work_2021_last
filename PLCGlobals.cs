using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Progetto_Main
{
    public class PLCGlobals
    {
        public static readonly string ns = "4";

        // Dati per la commessa corrente 

        public static readonly string Codice_commessa_in_corso = $"ns={ns};i=8";
        public static readonly string PezziDaProdurre = $"ns={ns}i=9";
        public static readonly string Codice_prodotto = $"ns={ns}i=10";
        public static readonly string Target_velocita = $"ns={ns}i=11";
        public static readonly string Codice_cliente = $"ns={ns}i=12";
        public static readonly string PezziProdotti = $"ns={ns};i=13";
        public static readonly string PezziScarti = $"ns={ns};i=14";
        public static readonly string PezziMancanti = $"ns={ns};i=15";

        // Dati per il management della coda

        public static readonly string Codice_commessa_coda = $"ns={ns};i=20";
        public static readonly string PezziDaProdurre_coda = $"ns={ns};i=21";
        public static readonly string Codice_prodotto_coda = $"ns={ns};i=22";
        public static readonly string Target_velocita_coda = $"ns={ns};i=23";
        public static readonly string Codice_cliente_coda = $"ns={ns};i=24";
        public static readonly string Sovrascrittura = $"ns={ns};i=25";

        // Dati comunicazione ufficio

        public static readonly string Array_Messaggi = $"ns={ns};i=197"; // Array di Messaggo[8]
        public static readonly string AbilitazioneDaUfficio = $"ns={ns};i=190";
        public static readonly string WatchDog = $"ns={ns};i=191";

        // Info generali

        public static readonly string PezziTOT = $"ns={ns};i=38";
        public static readonly string OreTOT = $"ns={ns};i=39"; // Prob è un time
        public static readonly string Velocita_attuale = $"ns={ns};i=40"; // Float

        // Stato macchina

        public static readonly string StatoMacchina = $"ns={ns};i=31"; // Array di bool[5]
        public static readonly string Emergenza_in_corso = $"ns={ns};i=32";
        public static readonly string Macchina_in_stop = $"ns={ns};i=33";
        public static readonly string Stop_in_fase = $"ns={ns};i=34";
        public static readonly string Ciclo_auto_in_corso = $"ns={ns};i=35";
        public static readonly string Comandi_manuali_inseriti = $"ns={ns};i=36";
        public static readonly string Produzione_libera = $"ns={ns};i=37";

        // Allarmi

        public static readonly string Array_allarmi = $"ns={ns};i=58"; // Array di Allarmi[31] ex58
    }
}
