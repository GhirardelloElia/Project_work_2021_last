using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Progetto_Main
{
    class PlcManager
    {
        PlcCommunicationService plc = new PlcCommunicationService();
        ServerManager serverManager = new ServerManager();

        public int InviaMessaggioAdOperatore(string messaggio)
        {
            try
            {
                plc.WriteToNode(new NodeWritingRequest<string>(PLCGlobals.MessaggioDaUfficio, messaggio));
            }
            catch
            {
                // TODO: Enum oppure custom error class custom error class sembra il meglio 
                return 4;
            }

            serverManager.RegistraMessaggio(messaggio, false);

            return 0;
        }

        // TODO: Il ricevimessaggio è un evento onchange della variabile. Ricorda di specificare
        // come deve essere cambiato

        public void UpdateCommessaInCorso()
        {
            int pezziBuoni = (int)plc.ReadNodeValue(PLCGlobals.PezziProdotti);
            int pezziScarti = (int)plc.ReadNodeValue(PLCGlobals.PezziScarti);
            int pezziDaprodurre = (int)plc.ReadNodeValue(PLCGlobals.PezziDaProdurre);
            int pezziTotCommessa = pezziBuoni + pezziScarti;
            string codiceCommessa = (string)plc.ReadNodeValue(PLCGlobals.Codice_commessa_in_corso);
            serverManager.UpdateCommessaInCorso(pezziBuoni, pezziScarti, pezziTotCommessa,
                pezziDaprodurre, codiceCommessa);        
        }

        public void ScriviCommessaAttuale(Commessa commessa)
        {
            plc.WriteToNode(new NodeWritingRequest<string>(PLCGlobals.Codice_commessa_in_corso, 
                commessa.CodiceCommessa));
            plc.WriteToNode(new NodeWritingRequest<string>(PLCGlobals.Codice_prodotto,
                commessa.CodiceProdotto));
            plc.WriteToNode(new NodeWritingRequest<string>(PLCGlobals.Codice_cliente,
                commessa.CodiceCliente));
            plc.WriteToNode(new NodeWritingRequest<Int16>(PLCGlobals.PezziDaProdurre,
                commessa.PezziDaFare));
            plc.WriteToNode(new NodeWritingRequest<float>(PLCGlobals.Target_velocita,
                commessa.TargetVelocita));
        }

        public void ScriviCommessaInCoda(Commessa commessa)
        {
            plc.StartAsync("opc.tcp://192.168.1.91:4840").GetAwaiter().GetResult();
            plc.WriteToNode(new NodeWritingRequest<string>(PLCGlobals.Codice_commessa_coda,
                commessa.CodiceCommessa));
            plc.WriteToNode(new NodeWritingRequest<string>(PLCGlobals.Codice_prodotto_coda,
                commessa.CodiceProdotto));
            plc.WriteToNode(new NodeWritingRequest<string>(PLCGlobals.Codice_cliente_coda,
                commessa.CodiceCliente));
            plc.WriteToNode(new NodeWritingRequest<Int16>(PLCGlobals.PezziDaProdurre_coda,
                commessa.PezziDaFare));
            plc.WriteToNode(new NodeWritingRequest<float>(PLCGlobals.Target_velocita_coda,
                commessa.TargetVelocita));
        }

        public void GetAllarmi()
        {
            var allarmi = new Allarmi[32];
            allarmi = (Allarmi[])plc.ReadNodeValue(PLCGlobals.PezziProdotti);
        }

        public void GetStatoMacchina()
        {
            var stato = plc.ReadNodeValue(PLCGlobals.StatoMacchina);
        }
    }
}
