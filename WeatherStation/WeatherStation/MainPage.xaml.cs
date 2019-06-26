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
using AtmosphericSensors.BME280;
using AtmosphericSensors;
using System.Diagnostics;
using System.Timers;
using Microsoft.Azure.Devices.Client;
using Newtonsoft.Json;

// The Blank Page item template is documented at https://go.microsoft.com/fwlink/?LinkId=402352&clcid=0x409

namespace WeatherStation
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class MainPage : Page
    {
        private I2cSensor sensor = null;
        private BME280 tester = null;
        private BME280Compensations compensations = null;

        private string deviceKey = "r6AJ8t4MnilDYBJcSx7my1+JB6AdLcAUEMOwMwF+P1M=";
        private string deviceId = "kbw-raspi";
        private string iotHubHostName = "wind79-test-hub.azure-devices.net";
        private DeviceAuthenticationWithRegistrySymmetricKey deviceAuthentication;
        private DeviceClient deviceClient;
        private long messageId = 0; 

        public MainPage()
        {
            this.InitializeComponent();
            var timer = new Timer(2000);
            timer.Elapsed += Timer_Elapsed;
            timer.AutoReset = false;
            System.Threading.Thread.Sleep(500);
            LoadTemperature();
            timer.Start();
            deviceAuthentication = new DeviceAuthenticationWithRegistrySymmetricKey(deviceId, deviceKey);
            deviceClient = DeviceClient.Create(iotHubHostName, deviceAuthentication, TransportType.Mqtt);
        }

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
                if (compensations == null)
                {
                    compensations = new BME280Compensations(sensor);
                }
                if (tester == null)
                {
                    tester = new BME280(sensor, compensations);
                }
                var temperature = tester.GetTemperature();
                var humidity = tester.GetHumidity();
                var baroPressure = tester.GetBarometricPressure();
                Debug.WriteLine("Temperature: {0:F3}°C; Humidity: {1:F3}%rH; Pressure: {2:F3}hPa", temperature, humidity, baroPressure);
                var message = new
                {
                    messsageId = messageId++,
                    deviceId = deviceId,
                    temperature = temperature,
                    humidity = humidity,
                    barometricPressure = baroPressure
                };
                string messageString = JsonConvert.SerializeObject(message);
                Message IoTMessage = new Message(System.Text.Encoding.UTF8.GetBytes(messageString));
                IoTMessage.ContentEncoding = "utf-8";
                IoTMessage.ContentType = "application/json";


                deviceClient.SendEventAsync(IoTMessage);
            }
            catch (Exception ex)
            {
                Debug.WriteLine("Exception: {0}", ex.Message);
            }
        }
    }
}
