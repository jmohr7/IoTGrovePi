﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
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
using Microsoft.Azure.Devices.Client;
using Newtonsoft.Json;
using GrovePi;
using GrovePi.Sensors;
using GrovePi.I2CDevices;

namespace IoTGrovePi
{
    /// <summary>
    /// UWP Page that sends sound sensor readings to Azure
    /// </summary>
    public sealed partial class MainPage : Page
    {
        private DispatcherTimer timer;
        private static IRgbLcdDisplay lcd;
        private static ISoundSensor soundsensor;
        static DeviceClient deviceClient;
        static string iotHubUri = ConnectionStrings.iotHubUri;
        static string deviceKey = ConnectionStrings.deviceKey;

        public MainPage()
        {
            this.InitializeComponent();

            deviceClient = DeviceClient.Create(iotHubUri, new DeviceAuthenticationWithRegistrySymmetricKey("GrovePiSoundSensor", deviceKey), TransportType.Http1);
            soundsensor = DeviceFactory.Build.SoundSensor(Pin.AnalogPin0);
            lcd = DeviceFactory.Build.RgbLcdDisplay().SetText("Hello").SetBacklightRgb(0,0,255);
            timer = new DispatcherTimer();
            timer.Interval = TimeSpan.FromMilliseconds(5000);
            timer.Tick += Timer_Tick;
            timer.Start();
        }

        /*
        // Method called every time the app timer interval occurs.  It read the sound
        // level from the sound sensor; converts it to JSON; sends it asynchronously 
        // to the Azure IoT hub; and finally updates the UI and GrovePI LCD with the 
        // new sound values
        */
        private async void Timer_Tick(object sender, object e)
        {
            try
            {
                int soundValue = soundsensor.SensorValue();
                var soundDataPoint = new
                {
                    deviceId = "GrovePi Sensor",
                    soundLevel = soundValue,
                };
                var messageString = JsonConvert.SerializeObject(soundDataPoint);
                var message = new Message(Encoding.ASCII.GetBytes(messageString));
                lcd.SetText("S: " + soundValue.ToString());
                await deviceClient.SendEventAsync(message);
                textBlockSound.Text = soundValue.ToString();
                textBlockMessage.Text = "Sensors read; Message sent";
            }
            catch (Exception)
            {
                // Bad sensor read
                lcd.SetText("Read or send failed");
            }
        }
    }
}
