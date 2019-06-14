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
using AtmosphericSensors.HardwareIO;
using AtmosphericSensors;
using System.Diagnostics;
using System.Timers;

// The Blank Page item template is documented at https://go.microsoft.com/fwlink/?LinkId=402352&clcid=0x409

namespace WeatherStation
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class MainPage : Page
    {
        public MainPage()
        {
            this.InitializeComponent();
            var timer = new Timer(1000);
            timer.Elapsed += Timer_Elapsed;
            timer.AutoReset = false;
            System.Threading.Thread.Sleep(500);
            LoadTemperature();
            timer.Start();

        }

        I2cSensor sensor = null;
        AtmosphericSensors.BME280.BME280 tester = null;

        private void Timer_Elapsed(object sender, ElapsedEventArgs e)
        {
            var timer = (Timer)sender;
            LoadTemperature();
            timer.Start();
        }

        private void LoadTemperatureButton_Click(object sender, RoutedEventArgs e)
        {
            LoadTemperature();
        }

        private void LoadTemperature()
        {
            try
            {
                
                if (sensor == null)
                {
                    sensor = new I2cSensor(0x77);
                }
                if (tester == null)
                {
                    tester = new AtmosphericSensors.BME280.BME280(sensor);
                }
                Debug.WriteLine("Temperature: {0:F3}°C; Humidity: {1:F3}%rH; Pressure: {2:F3}hPa", tester.GetTemperature(), tester.GetHumidity(), tester.GetBarometricPressure());
            }
            catch (Exception)
            {

            }
        }
    }
}
