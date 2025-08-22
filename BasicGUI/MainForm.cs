using System;
using System.Drawing;
using System.Windows.Forms;
using NetMQ;

namespace BasicStatusGUI
{
public class MainForm : Form
{
    public NetMQ.NetMQSocket subscriber;
    public System.Threading.Thread zmqThread;
    private NetMQ.Sockets.PublisherSocket pubSocket;
    private Label lblTemperature;
    private Label lblPumpSpeed;
    private Label lblValvePosition;
    private Panel pnlTempStatus;
    private Panel pnlPumpStatus;
    private Panel pnlValveStatus;
    private Button btnTempOn;
    private Button btnTempOff;
    private Button btnPumpOn;
    private Button btnPumpOff;
    private Button btnValveOn;
    private Button btnValveOff;
    private GroupBox grpControllerState;
    private Label lblControllerState;

    public MainForm()
    {
        // Start ZeroMQ subscriber in a background thread
        zmqThread = new System.Threading.Thread(StartZmqSubscriber);
        zmqThread.IsBackground = true;
        zmqThread.Start();

        // Create publisher socket for sending commands
        pubSocket = new NetMQ.Sockets.PublisherSocket();
        pubSocket.Bind("tcp://127.0.0.1:5557");

        this.Text = "System Status";
        this.Size = new Size(650, 400);
        this.StartPosition = FormStartPosition.CenterScreen;
        grpControllerState = new GroupBox { Text = "Controller State Machine", Location = new Point(500, 25), Size = new Size(130, 100) };
        lblControllerState = new Label { Text = "State: Idle", Location = new Point(10, 30), AutoSize = true };
        grpControllerState.Controls.Add(lblControllerState);

        lblTemperature = new Label { Text = "Temperature: 25°C", Location = new Point(30, 30), AutoSize = true };
        pnlTempStatus = new Panel { Location = new Point(250, 25), Size = new Size(30, 30), BackColor = Color.Green };

        lblPumpSpeed = new Label { Text = "Pump Speed: 1200 RPM", Location = new Point(30, 80), AutoSize = true };
        pnlPumpStatus = new Panel { Location = new Point(250, 75), Size = new Size(30, 30), BackColor = Color.Green };

        lblValvePosition = new Label { Text = "Valve Position: Open", Location = new Point(30, 130), AutoSize = true };
        pnlValveStatus = new Panel { Location = new Point(250, 125), Size = new Size(30, 30), BackColor = Color.Green };

        btnTempOn = new Button { Text = "Temp ON", Location = new Point(350, 25), Size = new Size(120, 40) };
        btnTempOff = new Button { Text = "Temp OFF", Location = new Point(350, 75), Size = new Size(120, 40) };
        btnPumpOn = new Button { Text = "Pump ON", Location = new Point(350, 125), Size = new Size(120, 40) };
        btnPumpOff = new Button { Text = "Pump OFF", Location = new Point(350, 175), Size = new Size(120, 40) };
        btnValveOn = new Button { Text = "Valve ON", Location = new Point(350, 225), Size = new Size(120, 40) };
        btnValveOff = new Button { Text = "Valve OFF", Location = new Point(350, 275), Size = new Size(120, 40) };

        btnTempOn.Click += (s, e) => { SendCommand("temp_cmd", "on"); };
        btnTempOff.Click += (s, e) => { SendCommand("temp_cmd", "off"); };
        btnPumpOn.Click += (s, e) => { SendCommand("pump_cmd", "on"); };
        btnPumpOff.Click += (s, e) => { SendCommand("pump_cmd", "off"); };
        btnValveOn.Click += (s, e) => { SendCommand("valve_cmd", "on"); };
        btnValveOff.Click += (s, e) => { SendCommand("valve_cmd", "off"); };

        this.Controls.Add(lblTemperature);
        this.Controls.Add(pnlTempStatus);
        this.Controls.Add(lblPumpSpeed);
        this.Controls.Add(pnlPumpStatus);
        this.Controls.Add(lblValvePosition);
        this.Controls.Add(pnlValveStatus);
        this.Controls.Add(btnTempOn);
        this.Controls.Add(btnTempOff);
        this.Controls.Add(btnPumpOn);
        this.Controls.Add(btnPumpOff);
        this.Controls.Add(btnValveOn);
        this.Controls.Add(btnValveOff);
        this.Controls.Add(grpControllerState);
    }
    private void SendCommand(string topic, string command)
    {
        if (pubSocket != null)
        {
            pubSocket.SendMoreFrame(topic).SendFrame(command);
        }
    }
    private void StartZmqSubscriber()
    {
        using (var subSocket = new NetMQ.Sockets.SubscriberSocket())
        {
            subSocket.Connect("tcp://127.0.0.1:5556");
            subSocket.Subscribe("pump_speed");
            subSocket.Subscribe("temperature");
            subSocket.Subscribe("valve_position");
            subSocket.Subscribe("controller_state");
            NetMQMessage msg = null;
            while (true)
            {
                if (subSocket.TryReceiveMultipartMessage(TimeSpan.FromMilliseconds(100), ref msg) && msg != null && msg.FrameCount >= 2)
                {
                    string topic = msg[0].ConvertToString();
                    string message = msg[1].ConvertToString();
                    if (topic == "pump_speed")
                    {
                        UpdatePumpSpeed(message);
                    }
                    else if (topic == "temperature")
                    {
                        UpdateTemperature(message);
                    }
                    else if (topic == "valve_position")
                    {
                        UpdateValvePosition(message);
                    }
                    else if (topic == "controller_state")
                    {
                        UpdateControllerState(message);
                    }
                }
            }
        }
    }
    private void UpdatePumpSpeed(string speed)
    {
        if (lblPumpSpeed.InvokeRequired)
        {
            lblPumpSpeed.Invoke(new Action<string>(UpdatePumpSpeed), speed);
        }
        else
        {
            lblPumpSpeed.Text = $"Pump Speed: {speed} RPM";
        }
    }

    private void UpdateTemperature(string temp)
    {
        if (lblTemperature.InvokeRequired)
        {
            lblTemperature.Invoke(new Action<string>(UpdateTemperature), temp);
        }
        else
        {
            lblTemperature.Text = $"Temperature: {temp}°C";
        }
    }

    private void UpdateValvePosition(string position)
    {
        if (lblValvePosition.InvokeRequired)
        {
            lblValvePosition.Invoke(new Action<string>(UpdateValvePosition), position);
        }
        else
        {
            lblValvePosition.Text = $"Valve Position: {position}";
        }
    }

    private void UpdateControllerState(string state)
    {
        if (lblControllerState.InvokeRequired)
        {
            lblControllerState.Invoke(new Action<string>(UpdateControllerState), state);
        }
        else
        {
            lblControllerState.Text = $"State: {state}";
        }
    }
    [STAThread]
    protected override void OnFormClosed(FormClosedEventArgs e)
    {
        pubSocket?.Dispose();
        base.OnFormClosed(e);
    }
    public static void Main()
    {
        Application.EnableVisualStyles();
        Application.SetCompatibleTextRenderingDefault(false);
        Application.Run(new MainForm());
    }
}
}
