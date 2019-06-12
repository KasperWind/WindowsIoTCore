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

        private byte[] WriteThenRead(byte address, int size)
        {
            byte[] write = { address };
            byte[] read = new byte[size];
            Array.Fill<byte>(read, 0x00);
            i2cSensor.WriteRead(write, read);
            return read;
        }

        public byte ReadRegister(byte address)
        {
            var read = WriteThenRead(address, 1);
            return read[0];
        }

        public UInt16 ReadUInt16Register(byte address)
        {
            var read = WriteThenRead(address, 2);
            int[] reading = new int[2];
            reading[0] = read[0];
            reading[1] = read[1] << 8;
            return (UInt16)(reading[1] + reading[0]);
            
        }

        public Int16 ReadInt16Register(byte address)
        {
            var read = WriteThenRead(address, 2);
            int[] reading = new int[2];
            reading[0] = read[0];
            reading[1] = read[1] << 8;
            return (Int16)(reading[1] + reading[0]);
        }

        public Int32 Read20bitRegister(byte address)
        {
            var msb = ReadRegister(address);
            var lsb = ReadRegister((byte)(address + 1));
            var xlsb = ReadRegister((byte)(address + 2));

            return ((msb << 12) + (lsb << 4) + (xlsb >> 4));
        }

    }
}
