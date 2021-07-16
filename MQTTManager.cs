using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Newtonsoft.Json;
using uPLibrary.Networking.M2Mqtt;
using uPLibrary.Networking.M2Mqtt.Messages;

namespace Progetto_Main
{
    class MQTTManager
    {
        MqttClient client;
        string clientId;

        public MQTTManager()
        {

        }

        public void Dispose()
        {
            client.Disconnect();
        }

        public void Subscribe()
        {
            string BrokerAddress = "test.mosquitto.org";

            client = new MqttClient(BrokerAddress);

            client.MqttMsgPublishReceived += client_MqttMsgPublishReceived;

            clientId = Guid.NewGuid().ToString();
            client.Connect(clientId);

            string messaggistica = "/THIENEPLC/sage/virus/IN/Message";
            string commesse = "/THIENEPLC/sage/virus/IN/IOT";
            string abilitazioneDaUfficio = "/THIENEPLC/sage/virus/IN/AbMacchina";

            client.Subscribe(new string[] { messaggistica }, new byte[] { 2 });
            client.Subscribe(new string[] { commesse }, new byte[] { 2 });
            client.Subscribe(new string[] { abilitazioneDaUfficio }, new byte[] { 2 });
        }

        public void Publish (string message)
        {
            string Topic = "/THIENEPLC/sage/virus/OUT/IOT";

            client.Publish(Topic, Encoding.UTF8.GetBytes(message), MqttMsgBase.QOS_LEVEL_EXACTLY_ONCE, true);
        }

        public void client_MqttMsgPublishReceived(object sender, MqttMsgPublishEventArgs e)
        {
            string ReceivedMessage = Encoding.UTF8.GetString(e.Message);
            
            Console.WriteLine(ReceivedMessage);
            PlcManager plc = new PlcManager();

            if (e.Topic == "/THIENEPLC/sage/virus/IN/Message")
            {
                plc.ScriviMessaggiAPlc(0, true);
            }
            else if (e.Topic == "/THIENEPLC/sage/virus/IN/IOT")
            {
                var commessa = JsonConvert.DeserializeObject<Commessa>(ReceivedMessage);
                plc.ScriviCommessaInCoda(commessa);
            }
            else if (e.Topic == "/THIENEPLC/sage/virus/IN/AbMacchina")
            {
                new PlcManager().ScriviAbilitazioneDaUfficio(new ServerManager().LeggiAbilitazioneDaUfficio());
            }
        }
    }
}
