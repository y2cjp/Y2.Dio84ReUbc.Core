// MIT License
//
// Copyright (c) Y2 Corporation

using System;
using Y2.Ft4222.Core;

namespace Y2.Dio84ReUbc.Core
{
    /// <summary>
    /// ピンの状態
    /// </summary>
    public enum PinState
    {
        /// <summary>
        /// Off
        /// </summary>
        Off,

        /// <summary>
        /// On
        /// </summary>
        On
    }

    /// <summary>
    /// DIO-84RD-IRC
    /// </summary>
    public sealed class Dio84 : IDio84
    {
        private readonly Pca9535 _ioExpander;
        private PinState _mikroBusResetPin;

        /// <summary>
        /// Initializes a new instance of the <see cref="Dio84"/> class.
        /// </summary>
        /// <param name="i2c">The I2C master device.</param>
        /// <param name="slaveAddress">The bus address of the I2C device.</param>
        public Dio84(IFt4222I2cMaster i2c, int slaveAddress = 0x23)
        {
            _ioExpander = new Pca9535(i2c, slaveAddress);
        }

        /// <inheritdoc/>
        public bool IsInitialized { get; private set; }

        /// <inheritdoc/>
        public void Initialize()
        {
            _ioExpander.WritePort(0, 0xff);
            _ioExpander.SetPortDirection(0, 00);
            var value = _ioExpander.ReadPort(0);
            _mikroBusResetPin = (value & 0x80) != 0 ? PinState.Off : PinState.On;
            IsInitialized = true;
        }

        /// <inheritdoc/>
        public byte ReadPort()
        {
            return (byte)~_ioExpander.ReadPort(1);
        }

        /// <inheritdoc/>
        public PinState ReadPin(int pin)
        {
            if (pin < 0 || 7 < pin)
                throw new ArgumentOutOfRangeException(nameof(pin));
            var state = _ioExpander.ReadPin(1, (byte)pin);
            return state == false ? PinState.On : PinState.Off;
        }

        /// <inheritdoc/>
        public void WritePort(byte value)
        {
            if (!IsInitialized)
                Initialize();

            if (_mikroBusResetPin == PinState.On)
                value |= 0x80;
            else
                value &= 0x7f;

            _ioExpander.WritePort(0, (byte)~value);
        }

        /// <inheritdoc/>
        public void WritePin(int pin, PinState pinState)
        {
            if (pin < 0 || 3 < pin)
                throw new ArgumentOutOfRangeException(nameof(pin));
            if (!IsInitialized)
                Initialize();

            var state = pinState == PinState.Off;
            _ioExpander.WritePin(0, (byte)pin, state);
        }

        /// <inheritdoc/>
        public void SetMikroBusResetPin(PinState pinState)
        {
            if (!IsInitialized)
                Initialize();

            var state = pinState == PinState.Off;
            _ioExpander.WritePin(0, 7, state);        // ResetOff(/RST=H), ResetOn(/RST=L)
            _mikroBusResetPin = pinState;
        }
    }
}
