using System;
using System.Threading;
using System.Threading.Tasks;
using Windows.Devices.Enumeration;
using Windows.Devices.I2c;

namespace AtmosphericSensors.HardwareIO
{
    public class I2cSensor : II2cSensor
    {
        private I2cDevice i2cSensor;

        public I2cSensor(byte i2cAddress)
        {
            Task.WaitAll(CreateDevice(i2cAddress));
        }

        private async Task CreateDevice(byte i2cAddress)
        {
            var settings = new I2cConnectionSettings(i2cAddress);
            var controller = await I2cController.GetDefaultAsync();
            i2cSensor = controller.GetDevice(settings);
        }

        public byte ReadRegister(byte registerAddress)
        {
            byte[] read = new byte[1];
            i2cSensor.Write(new byte[registerAddress]);
            Thread.Sleep(1);
            i2cSensor.Read(read);
            return read[0];
        }

        public long Read16bitRegister(byte addressLSB, byte addressMSB)
        {
            byte[] write = { addressLSB, addressMSB };
            var data = new byte[2];
            i2cSensor.Write(write);
            Thread.Sleep(1);
            i2cSensor.Read(data);
            return data[0] << 8 | data[1];
        }

        public long Read20bitRegister(byte addressLSB, byte address, byte addressMSB)
        {
            byte[] write = { addressLSB, address, addressMSB };
            var data = new byte[3];
            i2cSensor.Write(write);
            Thread.Sleep(1);
            i2cSensor.Read(data);
            return data[0] << 12 | data[1] << 4 | ((data[2] >> 4) & 0x0F);
        }
    }
}
