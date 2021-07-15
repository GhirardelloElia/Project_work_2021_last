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
            MQTTManager manager = new MQTTManager();
            manager.Subscribe();
            Console.WriteLine("Commesso al MQTT");
            PlcManager plcMan = new PlcManager();
            plcMan.Start();
            Console.WriteLine("Commesso al PLC");
        }
    }
}
