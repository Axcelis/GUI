using Xunit;
using BasicStatusGUI;

namespace BasicGUITests
{
    public class MainFormTests
    {
        [Fact]
        public void UpdateTemperature_SetsLabelCorrectly()
        {
        var form = new MainForm(false);
            form.UpdateTemperature("42");
            Assert.Equal("Temperature: 42°C", form.lblTemperature.Text);
        }

        [Fact]
        public void UpdatePumpSpeed_SetsLabelCorrectly()
        {
        var form = new MainForm(false);
            form.UpdatePumpSpeed("1500");
            Assert.Equal("Pump Speed: 1500 RPM", form.lblPumpSpeed.Text);
        }

        [Fact]
        public void UpdateValvePosition_SetsLabelCorrectly()
        {
        var form = new MainForm(false);
            form.UpdateValvePosition("Closed");
            Assert.Equal("Valve Position: Closed", form.lblValvePosition.Text);
        }

        [Fact]
        public void UpdateControllerState_SetsLabelCorrectly()
        {
        var form = new MainForm(false);
            form.UpdateControllerState("Running");
            Assert.Equal("State: Running", form.lblControllerState.Text);
        }
    }
}
