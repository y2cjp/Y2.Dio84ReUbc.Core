// MIT License
//
// Copyright (c) Y2 Corporation

using Y2.Ft4222.Core;

namespace Y2.Dio84ReUbc.Core
{
    public sealed class Dio84Re : Ft4222I2cMaster, IDio84Re
    {
        private const int DefaultDeviceAddress = 0x26;

        private readonly int _slaveAddress;
        private Dio84 _dio84;

        public Dio84Re(uint i2cFrequencyKbps)
            : this(DefaultDeviceAddress, i2cFrequencyKbps)
        {
        }

        public Dio84Re(int deviceAddress = DefaultDeviceAddress, uint i2cFrequencyKbps = 100)
            : base(new Ft4222I2cConnectionSettings(0, deviceAddress, i2cFrequencyKbps))
        {
            _slaveAddress = deviceAddress;
        }

        public bool IsInitialized => _dio84 != null && _dio84.IsInitialized;

        public void Initialize()
        {
            _dio84 = new Dio84(this, _slaveAddress);
            _dio84.Initialize();
        }

        public byte ReadPort()
        {
            return _dio84.ReadPort();
        }

        public PinState ReadPin(int bitNumber)
        {
            return _dio84.ReadPin(bitNumber);
        }

        public void WritePort(byte value)
        {
            _dio84.WritePort(value);
        }

        public void WritePin(int bitNumber, PinState pinState)
        {
            _dio84.WritePin(bitNumber, pinState);
        }

        public void SetMikroBusResetPin(PinState pinState)
        {
            _dio84.SetMikroBusResetPin(pinState);
        }
    }
}
