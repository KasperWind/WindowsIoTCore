using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Devices.Enumeration;
using Windows.Devices.I2c;
using AtmosphericSensors.HardwareIO;
using System.Runtime.CompilerServices;

[assembly: InternalsVisibleTo("AtmosphericSensors.tests")]
namespace AtmosphericSensors
{
    public class AtmosphericSensorBME280 //: ITemperatureSensor, IBarometricPressureSensor, IHumiditySensor
    {
        private readonly II2cSensor bme280sensor;
        private readonly AtmosphericSensorBME280Compensations compensations;

        private DateTimeOffset lastReading = DateTimeOffset.MinValue;
        private const int minTimeBetweenReadings = 500;
        
        public AtmosphericSensorBME280(II2cSensor bme280sensor) //0x77
        {
            this.bme280sensor = bme280sensor;
            compensations = new AtmosphericSensorBME280Compensations(bme280sensor);
        }

        internal long ReadRawHumidity()
        {
            return bme280sensor.Read16bitRegister(0xFE, 0xFD);
        }

        internal long ReadRawPressure()
        {
            return bme280sensor.Read20bitRegister(0xF8, 0xF7, 0xF9);
        }

        internal long ReadRawTemperature()
        {
            return bme280sensor.Read20bitRegister(0xFB, 0xFA, 0xFC);
        }
    }
}
