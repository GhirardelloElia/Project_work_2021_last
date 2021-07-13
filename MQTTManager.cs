using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Threading;
using uPLibrary.Networking.M2Mqtt;
using uPLibrary.Networking.M2Mqtt.Messages;

namespace Progetto_Main
{
    class MQTTManager
    {
        MqttClient client;
        string clientId;


        // this code runs when the main window opens (start of the app)
        public MQTTManager()
        {
            string BrokerAddress = "test.mosquitto.org";

            client = new MqttClient(BrokerAddress);

            // register a callback-function (we have to implement, see below) which is called by the library when a message was received
            client.MqttMsgPublishReceived += client_MqttMsgPublishReceived;

            // use a unique id as client id, each time we start the application
            clientId = Guid.NewGuid().ToString();

            client.Connect(clientId);
        }


        // this code runs when the main window closes (end of the app)
        public void Dispose()
        {
            client.Disconnect();
        }


        // this code runs when the button "Subscribe" is clicked
        public void Subscribe()
        {
                // whole topic
                string Topic = "/THIENEPLC/sage/virus/IN/IOT";

                // subscribe to the topic with QoS 2
                client.Subscribe(new string[] { Topic }, new byte[] { 2 });
        }

        public void SubscribeMessaggi()
        {
            // /THIENEPLC/sage/virus/IN/Message

        }

        // this code runs when the button "Publish" is clicked
        public void Publish (string message)
        {
                // whole topic
                string Topic = "/THIENEPLC/sage/virus/OUT/IOT";

                // publish a message with QoS 2
                client.Publish(Topic, Encoding.UTF8.GetBytes(message), MqttMsgBase.QOS_LEVEL_EXACTLY_ONCE, true);
        }

        // this code runs when a message was received
        public void client_MqttMsgPublishReceived(object sender, MqttMsgPublishEventArgs e)
        {
            string ReceivedMessage = Encoding.UTF8.GetString(e.Message);

            Console.WriteLine(ReceivedMessage);
        }
    }
}
