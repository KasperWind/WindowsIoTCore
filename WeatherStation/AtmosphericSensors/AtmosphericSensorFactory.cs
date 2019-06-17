using System;

namespace AtmosphericSensors
{
    public class AtmosphericSensorFactory
    {
        public static IAtmosphericSensor CreateSensor(AtmosphericSensorTypes SensorType, int Address)
        {
            return GetSensor(SensorType, Address);
        }

        private static IAtmosphericSensor GetSensor(AtmosphericSensorTypes SensorType, int Address)
        {
            switch (SensorType)
            {
                case AtmosphericSensorTypes.BME280:
                    return InstanceOfBME280(Address);
                default:
                    throw new ArgumentOutOfRangeException("SensorType", "The sensor type requested isnt valid");
            }
        }

        private static IAtmosphericSensor InstanceOfBME280(int Address)
        {
            ValidateAdressParameter(Address);
            return CreateSensorObject(Address);
        }

        private static IAtmosphericSensor CreateSensorObject(int Address)
        {
            var sensor = new HardwareIO.I2cSensor((byte)Address);
            return new BME280.BME280(sensor, new BME280.BME280Compensations(sensor));
        }

        private static void ValidateAdressParameter(int Address)
        {
            if (Address > byte.MaxValue || Address < byte.MinValue)
            {
                throw new ArgumentOutOfRangeException(
                    "Address", $"The value is outside the permitable value between 0x0 - 0xFF");
            }
        }
    }
}