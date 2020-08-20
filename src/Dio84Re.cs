// MIT License
//
// Copyright (c) Y2 Corporation

using Y2.Ft4222.Core;

namespace Y2.Dio84ReUbc.Core
{
    /// <summary>
    /// DIO-8/4RE-UBC
    /// </summary>
    public sealed class Dio84Re : Ft4222I2cMaster, IDio84Re
    {
        private const int DefaultDeviceAddress = 0x26;

        private readonly int _slaveAddress;
        private Dio84 _dio84;

        /// <summary>
        /// Initializes a new instance of the <see cref="Dio84Re"/> class.
        /// </summary>
        /// <param name="i2cFrequencyKbps">The speed of I2C transmission.</param>
        public Dio84Re(uint i2cFrequencyKbps)
            : this(DefaultDeviceAddress, i2cFrequencyKbps)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="Dio84Re"/> class.
        /// </summary>
        /// <param name="deviceAddress">The bus address of the I2C device.</param>
        /// <param name="i2cFrequencyKbps">The speed of I2C transmission.</param>
        public Dio84Re(int deviceAddress = DefaultDeviceAddress, uint i2cFrequencyKbps = 100)
            : base(new Ft4222I2cConnectionSettings(0, deviceAddress, i2cFrequencyKbps))
        {
            _slaveAddress = deviceAddress;
        }

        /// <inheritdoc/>
        public bool IsInitialized => _dio84 != null && _dio84.IsInitialized;

        /// <inheritdoc/>
        public void Initialize()
        {
            _dio84 = new Dio84(this, _slaveAddress);
            _dio84.Initialize();
        }

        /// <inheritdoc/>
        public byte ReadPort()
        {
            return _dio84.ReadPort();
        }

        /// <inheritdoc/>
        public PinState ReadPin(int bitNumber)
        {
            return _dio84.ReadPin(bitNumber);
        }

        /// <inheritdoc/>
        public void WritePort(byte value)
        {
            _dio84.WritePort(value);
        }

        /// <inheritdoc/>
        public void WritePin(int bitNumber, PinState pinState)
        {
            _dio84.WritePin(bitNumber, pinState);
        }

        /// <inheritdoc/>
        public void SetMikroBusResetPin(PinState pinState)
        {
            _dio84.SetMikroBusResetPin(pinState);
        }
    }
}
