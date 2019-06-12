namespace AtmosphericSensors.BME280
{
    public enum BME280Registers : byte
    {
        DigT1 = 0x88,
        DigT2 = 0x8A,
        DigT3 = 0x8C,
        DigP1 = 0x8E,
        DigP2 = 0x90,
        DigP3 = 0x92,
        DigP4 = 0x94,
        DigP5 = 0x96,
        DigP6 = 0x98,
        DigP7 = 0x9A,
        DigP8 = 0x9C,
        DigP9 = 0x9E,
        DigH1 = 0xA1,
        DigH2 = 0xE1,
        DigH3 = 0xE3,
        DigH4 = 0xE4,
        DigH5 = 0xE5,
        DigH6 = 0xE7,
        ChipId = 0xD0,
        Version = 0xD1,
        SoftReset = 0xE0,
        Cal26 = 0xE1,

        ControlHumid = 0xF2,
        Control = 0xF4,
        Config = 0xF5,

        PressureDataMSB = 0xF7,
        PressureDataLSB = 0xF8,
        PressureDataXLSB = 0xF9,

        TemperatureDataMSB = 0xFA,
        TemperatureDataLSB = 0xFB,
        TemperatureDataXLSB = 0xFC,

        HumidityDataMSB = 0xFD,
        HumidityDataLSB = 0xFE,
    }
}