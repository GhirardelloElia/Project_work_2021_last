using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Progetto_Main
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Faccio partire il processo...");
            //MQTTManager manager = new MQTTManager();
            //manager.Subscribe();
            //manager.Publish("Dio porco");
            PlcManager plcMan = new PlcManager();
            //plcMan.ScriviCommessaInCoda(new Commessa("Ciccio", "Boia", "io", 6, 1));
            plcMan.StartWatchDog();

            foreach (var item in plcMan.GetAllarmi())
            {
                if (item != null)
                    Console.WriteLine(item.Message);
            }

            plcMan.ScriviMessaggiAPlc();
        }
    }
}
