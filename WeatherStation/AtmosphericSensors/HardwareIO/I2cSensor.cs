using System;
using Windows.Devices.Enumeration;
using Windows.Devices.I2c;

namespace AtmosphericSensors.HardwareIO
{
    public class I2cSensor : II2cSensor
    {
        private I2cDevice i2cSensor;

        public I2cSensor(byte i2cAddress)
        {
            CreateDevice(i2cAddress);
        }

        private async void CreateDevice(byte i2cAddress)
        {
            var settings = new I2cConnectionSettings(i2cAddress);
            var deviceSelector = I2cDevice.GetDeviceSelector();
            var devices = await DeviceInformation.FindAllAsync(deviceSelector);
            i2cSensor = await I2cDevice.FromIdAsync(devices[0].Id, settings);
        }

        public byte ReadRegister(byte registerAddress)
        {
            byte[] read = new byte[1];
            i2cSensor.WriteRead(new byte[registerAddress], read);
            return read[0];
        }

        public long Read16bitRegister(byte addressLSB, byte addressMSB)
        {
            var data = new byte[2];
            data[0] = ReadRegister(addressLSB);
            data[1] = ReadRegister(addressMSB);
            return (long)(data[0] << 8 | data[1]);
        }

        public long Read20bitRegister(byte addressLSB, byte address, byte addressMSB)
        {
            var data = new byte[3];
            data[0] = ReadRegister(addressLSB);
            data[1] = ReadRegister(address);
            data[2] = ReadRegister(addressMSB);
            return (long)(data[0] << 12 | data[1] << 4 | ((data[2] >> 4) & 0x0F));
        }
    }
}
