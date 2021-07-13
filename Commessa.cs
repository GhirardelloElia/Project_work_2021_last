using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Progetto_Main
{
    class Commessa
    {
        public string CodiceCommessa { get; set; }
        public string CodiceProdotto { get; set; }
        public string CodiceCliente { get; set; }
        public Int16 PezziDaFare { get; set; }
        public float TargetVelocita { get; set; }

        public Commessa(string codiceCommessa, string codiceProdotto, string codiceCliente, Int16 pezziDaFare, float targetVelocita)
        {
            CodiceCommessa = codiceCommessa ?? throw new ArgumentNullException(nameof(codiceCommessa));
            CodiceProdotto = codiceProdotto ?? throw new ArgumentNullException(nameof(codiceProdotto));
            CodiceCliente = codiceCliente ?? throw new ArgumentNullException(nameof(codiceCliente));
            PezziDaFare = pezziDaFare;
            TargetVelocita = targetVelocita;
        }
    }
}
