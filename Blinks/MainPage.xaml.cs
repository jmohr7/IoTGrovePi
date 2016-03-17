using System;
using Windows.Devices.Gpio;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Media;
using GrovePi;
using GrovePi.Sensors;

// The Blank Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=402352&clcid=0x409

namespace Blinks
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class MainPage : Page
    {
        private DispatcherTimer timer;
        private SolidColorBrush redBrush = new SolidColorBrush(Windows.UI.Colors.Red);
        private SolidColorBrush grayBrush = new SolidColorBrush(Windows.UI.Colors.LightGray);
        private readonly IBuildGroveDevices _deviceFactory = DeviceFactory.Build;
        private ILed led;
        private bool ledOff;

        public MainPage()
        {
            InitializeComponent();
            ledOff = true;
            timer = new DispatcherTimer();
            timer.Interval = TimeSpan.FromMilliseconds(500);
            timer.Tick += Timer_Tick;
            led = _deviceFactory.Led(Pin.DigitalPin4);
            if (LED != null) {
                GpioStatus.Text = "GPIO pin initialized correctly.";
                timer.Start();
            }                      
        }

        private void Timer_Tick(object sender, object e)
        {
            if (ledOff)
            {                
                try {
                    led.ChangeState(SensorStatus.On);
                    LED.Fill = redBrush;
                    ledOff = false;
                }
                catch (Exception)
                {
                    throw;
                }
            }
            else
            {               
                try {
                    led.ChangeState(SensorStatus.Off);
                    LED.Fill = grayBrush;
                    ledOff = true;
                }
                catch (Exception)
                {
                    throw;
                }
            }
        }
    }
}
