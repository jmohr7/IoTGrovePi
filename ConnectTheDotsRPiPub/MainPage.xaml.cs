using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;
using GrovePi;
using GrovePi.Sensors;
using GrovePi.I2CDevices;

// The Blank Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=402352&clcid=0x409

namespace ConnectTheDotsRPiPub
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class MainPage : Page
    {
        private DispatcherTimer timer;
        private static IRgbLcdDisplay lcd;
        private static ISoundSensor soundsensor;

        ConnectTheDotsHelper ctdHelper;

        /// <summary>
        /// Main page constructor
        /// </summary>
        public MainPage()
        {
            this.InitializeComponent();

            // Hard coding guid for sensors. Not an issue for this particular application which is meant for testing and demos
            List<ConnectTheDotsSensor> sensors = new List<ConnectTheDotsSensor> {
                new ConnectTheDotsSensor("2298a348-e2f9-4438-ab23-82a3930662ab", "Sound", "Amp"),
            };

            ctdHelper = new ConnectTheDotsHelper(serviceBusNamespace: ConnectionStrings.ServicebusNamespaceDefault,
                eventHubName: ConnectionStrings.EventHubNameDefault,
                keyName: ConnectionStrings.KeyNameDefault,
                key: ConnectionStrings.KeyDefault,
                displayName: ConnectionStrings.DisplayNameDefault,
                organization: ConnectionStrings.OrganizationDefault,
                location: ConnectionStrings.LocationDefault,
                sensorList: sensors);
            soundsensor = DeviceFactory.Build.SoundSensor(Pin.AnalogPin0);
            lcd = DeviceFactory.Build.RgbLcdDisplay().SetText("Hello").SetBacklightRgb(0, 0, 255);
            timer = new DispatcherTimer();
            timer.Interval = TimeSpan.FromMilliseconds(5000);
            timer.Tick += Timer_Tick;
            timer.Start();
        }

        private void Timer_Tick(object sender, object e)
        {
            ConnectTheDotsSensor sensor = ctdHelper.sensors.Find(item => item.measurename == "Sound");

            try
            {
                int soundValue = soundsensor.SensorValue();
                sensor.value = soundValue;
                lcd.SetText("S: " + soundValue.ToString());
                ctdHelper.SendSensorData(sensor);
            }
            catch (Exception)
            {
                // Bad sensor read
                lcd.SetText("Read or send failed");
            }           
        }
    }
}
