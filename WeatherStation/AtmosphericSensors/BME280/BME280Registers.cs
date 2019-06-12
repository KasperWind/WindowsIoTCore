namespace AtmosphericSensors.BME280
{
    public enum BME280Registers : byte
    {
        BME280RegisterDIG_T1 = 0x88,
        BME280RegisterDIG_T2 = 0x8A,
        BME280RegisterDIG_T3 = 0x8C,
        BME280RegisterDIG_P1 = 0x8E,
        BME280RegisterDIG_P2 = 0x90,
        BME280RegisterDIG_P3 = 0x92,
        BME280RegisterDIG_P4 = 0x94,
        BME280RegisterDIG_P5 = 0x96,
        BME280RegisterDIG_P6 = 0x98,
        BME280RegisterDIG_P7 = 0x9A,
        BME280RegisterDIG_P8 = 0x9C,
        BME280RegisterDIG_P9 = 0x9E,
        BME280RegisterDIG_H1 = 0xA1,
        BME280RegisterDIG_H2 = 0xE1,
        BME280RegisterDIG_H3 = 0xE3,
        BME280RegisterDIG_H4 = 0xE4,
        BME280RegisterDIG_H5 = 0xE5,
        BME280RegisterDIG_H6 = 0xE7,
        BME280RegisterChipId = 0xD0,
        BME280RegisterVersion = 0xD1,
        BME280RegisterSoftReset = 0xE0,
        BME280RegisterCal26 = 0xE1,

        BME280RegisterControlHumid = 0xF2,
        BME280RegisterControl = 0xF4,
        BME280RegisterConfig = 0xF5,

        BME280RegisterPressureDataMSB = 0xF7,
        BME280RegisterPressureDataLSB = 0xF8,
        BME280RegisterPressureDataXLSB = 0xF9,

        BME280RegisterTemperatureDataMSB = 0xFA,
        BME280RegisterTemperatureDataLSB = 0xFB,
        BME280RegisterTemperatureDataXLSB = 0xFC,

        BME280RegisterHumidityDataMSB = 0xFD,
        BME280RegisterHumidityDataLSB = 0xFE,
    }
}