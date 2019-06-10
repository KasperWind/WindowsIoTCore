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
            byte[] write = { registerAddress, (byte)(registerAddress + 1) };
            byte[] read = new byte[2];
            WriteThenRead(write, read);
            return read[0];
        }

        private void WriteThenRead(byte[] write, byte[] read)
        {
            i2cSensor.Write(write);
            Thread.Sleep(1);
            i2cSensor.Read(read);
        }

        public long Read16bitRegister(byte addressLSB, byte addressMSB)
        {
            byte[] write = { addressLSB, addressMSB };
            var read = new byte[2];
            WriteThenRead(write, read);
            return read[0] << 8 | read[1];
        }

        public long Read20bitRegister(byte addressLSB, byte address, byte addressMSB)
        {
            byte[] write = { addressLSB, address, addressMSB };
            var read = new byte[3];
            WriteThenRead(write, read);
            return read[0] << 12 | read[1] << 4 | ((read[2] >> 4) & 0x0F);
        }
    }
}
