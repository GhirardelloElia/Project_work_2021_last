using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Timers;

namespace Progetto_Main
{
    class PlcManager
    {
        PlcCommunicationService plc = new PlcCommunicationService();
        ServerManager serverManager = new ServerManager();

        public void Start()
        {
            StartNodeValues();
            Timer timer = new Timer(3000);
            timer.Elapsed += Timer_Elapsed;
        }

        private void Timer_Elapsed(object sender, ElapsedEventArgs e)
        {
            AggiornaStatoPLC();
        }

        public void AggiornaStatoPLC()
        {
            plc.StartAsync("opc.tcp://192.168.1.91:4840").GetAwaiter().GetResult();
            string codiceCommessa = (string)plc.ReadNodeValue(PLCGlobals.Codice_commessa_in_corso);
            int velocitaMacchina = (int)plc.ReadNodeValue(PLCGlobals.Velocita_attuale);
            bool inWarning = (bool)plc.ReadNodeValue(PLCGlobals.Warning_in_corso);
            bool inEmergenza = (bool)plc.ReadNodeValue(PLCGlobals.Emergenza_in_corso);

            serverManager.AggiornaStatoPLC(codiceCommessa, velocitaMacchina, inEmergenza || inWarning ? 1 : 0, 1);
        }

        // TODO: Il ricevimessaggio è un evento onchange della variabile. Ricorda di specificare
        // come deve essere cambiato

        public void UpdateCommessaInCorso()
        {
            plc.StartAsync("opc.tcp://192.168.1.91:4840").GetAwaiter().GetResult();
            int pezziBuoni = (int)plc.ReadNodeValue(PLCGlobals.PezziProdotti);
            int pezziScarti = (int)plc.ReadNodeValue(PLCGlobals.PezziScarti);
            int pezziDaprodurre = (int)plc.ReadNodeValue(PLCGlobals.PezziDaProdurre);
            int pezziTotCommessa = pezziBuoni + pezziScarti;
            string codiceCommessa = (string)plc.ReadNodeValue(PLCGlobals.Codice_commessa_in_corso);
            string codiceCliente = (string)plc.ReadNodeValue(PLCGlobals.Codice_cliente);
            serverManager.UpdateCommessaInCorso(pezziBuoni, pezziScarti, pezziTotCommessa,
                pezziDaprodurre, codiceCommessa, codiceCliente);
        }

        public void ScriviCommessaInCoda(Commessa commessa)
        {
            plc.StartAsync("opc.tcp://192.168.1.91:4840").GetAwaiter().GetResult();
            plc.WriteToNode(new NodeWritingRequest<string>(PLCGlobals.Codice_commessa_coda,
                commessa.CodiceCommessa));
            plc.WriteToNode(new NodeWritingRequest<string>(PLCGlobals.Codice_prodotto_coda,
                commessa.NomeProdotto));
            plc.WriteToNode(new NodeWritingRequest<string>(PLCGlobals.Codice_cliente_coda,
                commessa.NomeCliente));
            plc.WriteToNode(new NodeWritingRequest<Int16>(PLCGlobals.PezziDaProdurre_coda,
                commessa.Quantita));
            plc.WriteToNode(new NodeWritingRequest<float>(PLCGlobals.Target_velocita_coda,
                commessa.VelocitaMacchina));
        }

        public Allarme[] GetAllarmi()
        {
            plc.StartAsync("opc.tcp://192.168.1.91:4840").GetAwaiter().GetResult();
            // PER LEGGERE ARRAY LEGGI UNO AD UNO
            Allarme[] allarmi = new Allarme[32];
            int nodeIdArray = 58;
            for (int i = 0; i < 31; i++)
            {
                allarmi[i] = new Allarme()
                {
                    Message = (string)plc.ReadNodeValue($"ns={PLCGlobals.ns};i={nodeIdArray + 2}"),
                    IsActive = (bool)plc.ReadNodeValue($"ns={PLCGlobals.ns};i={nodeIdArray + 3}"),
                    IsWarning = (bool)plc.ReadNodeValue($"ns={PLCGlobals.ns};i={nodeIdArray + 4}")
                };
                nodeIdArray += 4;
            }
            // Allarmi[] allarmi = (Allarmi[])plc.ReadNodeValue(PLCGlobals.Array_allarmi);
            return allarmi;
        }

        public void GetStatoMacchina()
        {
            plc.StartAsync("opc.tcp://192.168.1.91:4840").GetAwaiter().GetResult();
            var stato = plc.ReadNodeValue(PLCGlobals.StatoMacchina);
        }

        public void ScriviMessaggiAPlc()
        {
            plc.StartAsync("opc.tcp://192.168.1.91:4840").GetAwaiter().GetResult();
            Messaggio[] messaggi = serverManager.GetMessaggiDB();

            /*
            Messaggio[] messaggi = new Messaggio[8]
            {
                new Messaggio() { Text = "uno", IsSent = false, TimeStamp = "14/07/2021"},
                new Messaggio() { Text = "due", IsSent = true, TimeStamp = "14/07/2021"},
                new Messaggio() { Text = "tre", IsSent = false, TimeStamp = "14/07/2021"},
                new Messaggio() { Text = "quattro", IsSent = true, TimeStamp = "14/07/2021"},
                new Messaggio() { Text = "cinqe", IsSent = false, TimeStamp = "14/07/2021"},
                new Messaggio() { Text = "sei", IsSent = false, TimeStamp = "14/07/2021"},
                new Messaggio() { Text = "sette", IsSent = false, TimeStamp = "14/07/2021"},
                new Messaggio() { Text = "otto", IsSent = false, TimeStamp = "14/07/2021"},
            };
            */

            int nodeIdArray = 199;

            foreach (var messaggio in messaggi)
            {
                plc.WriteToNode(new NodeWritingRequest<string>($"ns={PLCGlobals.ns};i={nodeIdArray + 2}",
                messaggio.Text));
                plc.WriteToNode(new NodeWritingRequest<bool>($"ns={PLCGlobals.ns};i={nodeIdArray + 3}",
                messaggio.IsSent));
                plc.WriteToNode(new NodeWritingRequest<string>($"ns={PLCGlobals.ns};i={nodeIdArray + 4}",
                messaggio.TimeStamp));
                nodeIdArray += 4;
            }
        }

        public void StartNodeValues()
        {
            plc.StartAsync("opc.tcp://192.168.1.91:4840").GetAwaiter().GetResult();
            var items = new List<string>();
            items.Add(PLCGlobals.WatchDog);
            items.Add(PLCGlobals.MessaggioDaPLC);
            items.Add(PLCGlobals.Warning_in_corso);
            items.Add(PLCGlobals.Emergenza_in_corso);

            plc.SubscribeToNodeChanges(items, 1000);

            plc.NodeValueChanged += plc_nodevalueChanged;
        }

        private void plc_nodevalueChanged(object sender, NodeValueChangedNotification e)
        {
            plc.StartAsync("opc.tcp://192.168.1.91:4840").GetAwaiter().GetResult();

            if (e.NodeId == PLCGlobals.WatchDog)
            {
                if ((bool)e.Value)
                    plc.WriteToNode(new NodeWritingRequest<bool>(PLCGlobals.WatchDog, false));
                return;
            }
            else if (e.NodeId == PLCGlobals.Emergenza_in_corso || e.NodeId == PLCGlobals.Warning_in_corso)
            {
                AggiornaStatoPLC();
            }
            else if (e.NodeId == PLCGlobals.MessaggioDaPLC)
            {
                if (string.IsNullOrEmpty((string)e.Value))
                    return;

                serverManager.RegistraMessaggio((string)e.Value, true);
                plc.WriteToNode(new NodeWritingRequest<string>(PLCGlobals.MessaggioDaPLC, ""));
            }
        }
    }
}
