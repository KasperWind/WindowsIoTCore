using System;

namespace AtmosphericSensors.HardwareIO
{
    public interface II2cSensor
    {
        byte ReadRegister(byte registerAddress);
        UInt16 ReadUInt16Register(byte address);
        Int16 ReadInt16Register(byte address);
        Int32 Read20bitRegister(byte address);
    }
}