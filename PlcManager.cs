using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.NetworkInformation;
using System.Text;
using System.Timers;

namespace Progetto_Main
{
    class PlcManager
    {
        PlcCommunicationService plc = new PlcCommunicationService();
        ServerManager serverManager = new ServerManager();
        StatoPLC statoPLC = new StatoPLC();

        public void Start()
        {
            StartMessaggistica();
            StartWatchDog();
            StartEmergenza();
            StartWarning();
            // StartIndexMessaggio();
            Timer timer = new Timer(3000);
            timer.Elapsed += Timer_Elapsed;
            timer.Start();
        }

        private void Timer_Elapsed(object sender, ElapsedEventArgs e)
        {
            AggiornaStatoPLC();
            UpdateCommessaInCorso();
        }

        public void AggiornaStatoPLC()
        {
            try
            {
                plc.StartAsync("opc.tcp://192.168.1.91:4840").GetAwaiter().GetResult();
                string codiceCommessa = (string)plc.ReadNodeValue(PLCGlobals.Codice_commessa_in_corso);
                string codiceCliente = (string)plc.ReadNodeValue(PLCGlobals.Codice_cliente);
                float velocitaMacchina = (float)plc.ReadNodeValue(PLCGlobals.Velocita_attuale);
                int pezziBuoni = (int)plc.ReadNodeValue(PLCGlobals.PezziProdotti);
                int pezziScarti = (int)plc.ReadNodeValue(PLCGlobals.PezziScarti);
                int pezziTot = pezziBuoni + pezziScarti;
                bool inWarning = (bool)plc.ReadNodeValue(PLCGlobals.Warning_in_corso);
                bool inEmergenza = (bool)plc.ReadNodeValue(PLCGlobals.Emergenza_in_corso);
                bool AbilitazioneDaUfficio = (bool)plc.ReadNodeValue(PLCGlobals.AbilitazioneDaUfficio);
                bool isAlive = false;
                bool isWorking = (bool)plc.ReadNodeValue(PLCGlobals.Ciclo_auto_in_corso);


                Ping myPing = new Ping();
                PingReply reply = myPing.Send("192.168.1.91", 50);
                if (reply != null)
                    isAlive = true;

                statoPLC.IsInErrore = inEmergenza;
                statoPLC.IsInWarning = inWarning;
                statoPLC.IsOnline = isAlive;
                statoPLC.pezziScarti = pezziScarti;
                statoPLC.pezziBuoni = pezziBuoni;
                statoPLC.pezziTotali = pezziTot;
                statoPLC.commessaInCorso = codiceCommessa;
                statoPLC.clienteInCorso = codiceCliente;
                statoPLC.AbilitazioneDaUfficio = AbilitazioneDaUfficio;
                statoPLC.IsWorking = isWorking;

                Console.WriteLine("Ricevuti i dati per aggiornamento stato PLC...");

                serverManager.AggiornaStatoPLC(statoPLC);
            }
            catch (Exception)
            {
                Console.WriteLine("Errore nell'aggiornamento dati stato PLC...");
            }
        }

        // TODO: Il ricevimessaggio è un evento onchange della variabile. Ricorda di specificare
        // come deve essere cambiato

        public void UpdateCommessaInCorso()
        {
            try
            {
                plc.StartAsync("opc.tcp://192.168.1.91:4840").GetAwaiter().GetResult();
                int pezziBuoni = (int)plc.ReadNodeValue(PLCGlobals.PezziProdotti);
                int pezziScarti = (int)plc.ReadNodeValue(PLCGlobals.PezziScarti);
                int pezziDaprodurre = (int)plc.ReadNodeValue(PLCGlobals.PezziDaProdurre);
                int pezziTotCommessa = pezziBuoni + pezziScarti;
                string codiceCommessa = (string)plc.ReadNodeValue(PLCGlobals.Codice_commessa_in_corso);
                string codiceCliente = (string)plc.ReadNodeValue(PLCGlobals.Codice_cliente);

                Console.WriteLine("Ricevuti i dati per aggiornamento commessa in corso...");

                serverManager.UpdateCommessaInCorso(pezziBuoni, pezziScarti, pezziTotCommessa,
                    pezziDaprodurre, codiceCommessa, codiceCliente);
            }
            catch (Exception)
            {
                Console.WriteLine("Errore nell'update della commessa in corso");
            }
        }

        public void ScriviCommessaInCoda(Commessa commessa)
        {
            try
            {
                plc.StartAsync("opc.tcp://192.168.1.91:4840").GetAwaiter().GetResult();

                bool sovrascrittura = (bool)plc.ReadNodeValue(PLCGlobals.Sovrascrittura);

                //if (!sovrascrittura)
                 //   return;

                plc.WriteToNode(new NodeWritingRequest<string>(PLCGlobals.Codice_commessa_coda,
                    commessa.CodiceCommessa));
                plc.WriteToNode(new NodeWritingRequest<string>(PLCGlobals.Codice_prodotto_coda,
                    commessa.NomeProdotto));
                plc.WriteToNode(new NodeWritingRequest<string>(PLCGlobals.Codice_cliente_coda,
                    commessa.NomeCliente));
                plc.WriteToNode(new NodeWritingRequest<Int32>(PLCGlobals.PezziDaProdurre_coda,
                    commessa.Quantita));
                plc.WriteToNode(new NodeWritingRequest<float>(PLCGlobals.Target_velocita_coda,
                    commessa.VelocitaMacchina));
                plc.WriteToNode(new NodeWritingRequest<bool>(PLCGlobals.Sovrascrittura,
                    false));

                Console.WriteLine("Scritta commessa in coda...");
            }
            catch (Exception)
            {
                Console.WriteLine("Errore nella scrittura della commessa in coda...");
            }
        }

        public Allarme[] GetAllarmi()
        {
            try
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

                Console.WriteLine("Letti allarmi da PLC...");

                return allarmi;
            }
            catch (Exception)
            {
                Console.WriteLine("Errore nella lettura degli allarmi");

                return new Allarme[1];
            }
        }

        public void GetStatoMacchina()
        {
            plc.StartAsync("opc.tcp://192.168.1.91:4840").GetAwaiter().GetResult();
            var stato = plc.ReadNodeValue(PLCGlobals.StatoMacchina);
        }

        public void ScriviAbilitazioneDaUfficio(bool abilitato)
        {
            try
            {
                plc.StartAsync("opc.tcp://192.168.1.91:4840").GetAwaiter().GetResult();

                plc.WriteToNode(new NodeWritingRequest<bool>(PLCGlobals.AbilitazioneDaUfficio,
                    abilitato));

                Console.WriteLine($"Scritto abilitazione da ufficio: {abilitato}");
            }
            catch (Exception)
            {
                Console.WriteLine("Errore nella scrittura abilitazione da ufficio...");
            }
        }

        public void ScriviMessaggiAPlc(UInt32 index = 0)
        {
            plc.StartAsync("opc.tcp://192.168.1.91:4840").GetAwaiter().GetResult();
            Messaggio[] messaggi;
            if (index != 0)
                messaggi = serverManager.GetMessaggiDB(index);
            else
                messaggi = serverManager.GetMessaggiDB();

            int nodeIdArray = 199;

            foreach (var messaggio in messaggi)
            {
                if (messaggio is null)
                    continue;

                try
                {
                    plc.WriteToNode(new NodeWritingRequest<string>($"ns={PLCGlobals.ns};i={nodeIdArray + 2}",
                    messaggio.Text));
                    plc.WriteToNode(new NodeWritingRequest<bool>($"ns={PLCGlobals.ns};i={nodeIdArray + 3}",
                    messaggio.IsSent));
                    plc.WriteToNode(new NodeWritingRequest<string>($"ns={PLCGlobals.ns};i={nodeIdArray + 4}",
                    messaggio.TimeStamp));
                    nodeIdArray += 4;
                }
                catch
                {
                    Console.WriteLine("Errore nella scrittura dei messaggi a PLC...");
                    return;
                }
            }

            Console.WriteLine("Scritti i messaggi a PLC...");
        }

        public void StartMessaggistica()
        {
            plc.StartAsync("opc.tcp://192.168.1.91:4840").GetAwaiter().GetResult();
            var items = new List<string>();
            items.Add(PLCGlobals.MessaggioDaPLC);

            plc.SubscribeToNodeChanges(items, 1000);

            plc.NodeValueChanged += plc_nodevalueChanged;
        }

        private void plc_nodevalueChanged(object sender, NodeValueChangedNotification e)
        {
            plc.StartAsync("opc.tcp://192.168.1.91:4840").GetAwaiter().GetResult();

            if (e.NodeId == PLCGlobals.MessaggioDaPLC)
            {
                Console.WriteLine("Messaggio cambiato...");
                if (string.IsNullOrEmpty((string)e.Value))
                    return;

                serverManager.RegistraMessaggio((string)e.Value, true);
                plc.WriteToNode(new NodeWritingRequest<string>(PLCGlobals.MessaggioDaPLC, ""));
                ScriviMessaggiAPlc();
            }
        }

        public void StartWatchDog()
        {
            plc.StartAsync("opc.tcp://192.168.1.91:4840").GetAwaiter().GetResult();
            var items = new List<string>();
            items.Add(PLCGlobals.WatchDog);

            plc.SubscribeToNodeChanges(items, 1000);

            plc.NodeValueChanged += WatchDog_nodevaluechanged;
        }

        private void WatchDog_nodevaluechanged(object sender, NodeValueChangedNotification e)
        {
            if (e.NodeId == PLCGlobals.WatchDog)
            {
                Console.WriteLine("watchdog cambiato...");
                if ((bool)e.Value)
                    plc.WriteToNode(new NodeWritingRequest<bool>(PLCGlobals.WatchDog, false));
                return;
            }
        }

        public void StartWarning()
        {
            plc.StartAsync("opc.tcp://192.168.1.91:4840").GetAwaiter().GetResult();
            var items = new List<string>();
            items.Add(PLCGlobals.Warning_in_corso);
            items.Add(PLCGlobals.IndexVisuale);

            plc.SubscribeToNodeChanges(items, 1000);

            plc.NodeValueChanged += Warning_nodevaluechanged;
        }

        private void Warning_nodevaluechanged(object sender, NodeValueChangedNotification e)
        {
            if (e.NodeId == PLCGlobals.Warning_in_corso)
            {
                AggiornaStatoPLC();
                string codiceCommessa = (string)plc.ReadNodeValue(PLCGlobals.Codice_commessa_in_corso);
                string messaggioAllarme = "Errore in codice c#";
                Console.WriteLine("bit warning cambiato...");

                foreach (var item in GetAllarmi())
                {
                    if (item is null)
                        continue;

                    if (item.IsActive)
                    {
                        messaggioAllarme = item.Message;
                        break;
                    }
                }

                serverManager.ScriviAllarme(messaggioAllarme, codiceCommessa, true);
            }
            else if (e.NodeId == PLCGlobals.IndexVisuale)
            {
                Console.WriteLine("bit indexvis cambiato");
                    ScriviMessaggiAPlc((UInt32)e.Value);
            }
        }

        public void StartEmergenza()
        {
            plc.StartAsync("opc.tcp://192.168.1.91:4840").GetAwaiter().GetResult();
            var items = new List<string>();
            items.Add(PLCGlobals.Emergenza_in_corso);

            plc.SubscribeToNodeChanges(items, 1000);

            plc.NodeValueChanged += Errore_nodevaluechanged;
        }

        private void Errore_nodevaluechanged(object sender, NodeValueChangedNotification e)
        {
            plc.StartAsync("opc.tcp://192.168.1.91:4840").GetAwaiter().GetResult();
            Console.WriteLine("Bit emergenza cambiato...");
            if (e.NodeId == PLCGlobals.Emergenza_in_corso)
            {
                AggiornaStatoPLC();
                string codiceCommessa = (string)plc.ReadNodeValue(PLCGlobals.Codice_commessa_in_corso);
                string messaggioAllarme = "Errore in codice c#";

                foreach (var item in GetAllarmi())
                {
                    if (item is null)
                        continue;

                    if (item.IsActive)
                    {
                        messaggioAllarme = item.Message;
                        break;
                    }
                }

                serverManager.ScriviAllarme(messaggioAllarme, codiceCommessa, true);
            }
        }

        public void StartIndexMessaggio()
        {
            plc.StartAsync("opc.tcp://192.168.1.91:4840").GetAwaiter().GetResult();
            var items = new List<string>();
            items.Add(PLCGlobals.IndexVisuale);

            plc.SubscribeToNodeChanges(items, 1000);

            plc.NodeValueChanged += IndexMessaggio_nodevaluechanged;
        }

        private void IndexMessaggio_nodevaluechanged(object sender, NodeValueChangedNotification e)
        {

        }
    }
}
