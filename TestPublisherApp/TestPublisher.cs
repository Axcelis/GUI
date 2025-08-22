// Simple NetMQ publisher to test the C# GUI
// Compile with: dotnet run (after adding NetMQ package)
using System;
using NetMQ;
using NetMQ.Sockets;

class TestPublisher
{
    static void Main(string[] args)
    {
        using (var pubSocket = new PublisherSocket())
        {
            pubSocket.Bind("tcp://127.0.0.1:5556");
            Console.WriteLine("TestPublisher started. Press Ctrl+C to exit.");
            int speed = 1000;
            int temp = 25;
            string[] valveStates = { "Open", "Closed" };
            int valveIdx = 0;
            string[] controllerStates = { "Idle", "Running", "Error" };
            int controllerIdx = 0;
            while (true)
            {
                pubSocket.SendMoreFrame("pump_speed").SendFrame(speed.ToString());
                pubSocket.SendMoreFrame("temperature").SendFrame(temp.ToString());
                pubSocket.SendMoreFrame("valve_position").SendFrame(valveStates[valveIdx]);
                pubSocket.SendMoreFrame("controller_state").SendFrame(controllerStates[controllerIdx]);
                Console.WriteLine($"Sent pump_speed: {speed}");
                Console.WriteLine($"Sent temperature: {temp}");
                Console.WriteLine($"Sent valve_position: {valveStates[valveIdx]}");
                Console.WriteLine($"Sent controller_state: {controllerStates[controllerIdx]}");
                speed += 100;
                if (speed > 2000) speed = 1000;
                temp += 1;
                if (temp > 35) temp = 25;
                valveIdx = (valveIdx + 1) % valveStates.Length;
                controllerIdx = (controllerIdx + 1) % controllerStates.Length;
                System.Threading.Thread.Sleep(1000);
            }
        }
    }
}
