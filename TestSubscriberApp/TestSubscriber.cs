using System;
using NetMQ;
using NetMQ.Sockets;

class TestSubscriber
{
    static void Main(string[] args)
    {
        using (var subSocket = new SubscriberSocket())
        {
            subSocket.Connect("tcp://127.0.0.1:5557");
            subSocket.Subscribe("temp_cmd");
            subSocket.Subscribe("pump_cmd");
            subSocket.Subscribe("valve_cmd");

            Console.WriteLine("TestSubscriber listening for commands. Press Ctrl+C to exit.");

            NetMQMessage msg = null;
            while (true)
            {
                if (subSocket.TryReceiveMultipartMessage(TimeSpan.FromMilliseconds(100), ref msg) && msg != null && msg.FrameCount >= 2)
                {
                    string topic = msg[0].ConvertToString();
                    string command = msg[1].ConvertToString();
                    Console.WriteLine($"Received {topic}: {command}");
                }
            }
        }
    }
}