﻿using System;

namespace AtmosphericSensors.HardwareIO
{
    public interface II2cSensor
    {
        byte ReadRegister(byte address);
        UInt16 ReadUInt16Register(byte address);
        Int16 ReadInt16Register(byte address);
        Int32 Read20bitRegister(byte address);

        void WriteRegister(byte address, byte[] data);
    }
}