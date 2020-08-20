// MIT License
//
// Copyright (c) Y2 Corporation

using System;
using Y2.Ft4222.Core;

namespace Y2.Dio84ReUbc.Core
{
    public sealed class Dio016 : IDio016
    {
        private readonly Pca9535 _ioExpander;

        public Dio016(IFt4222I2cMaster i2c, byte slaveAddress = 0x24)
        {
            _ioExpander = new Pca9535(i2c, slaveAddress);
        }

        public bool IsInitialized { get; private set; }

        public void Initialize()
        {
            // 接続状態確認の為、ダミーリード
            var _ = IoExpanderReadPort(0);

            byte[] values = { 0xff, 0xff };
            IoExpanderWritePort(0, values);
            values[0] = 0x00;
            values[1] = 0x00;
            IoExpanderSetPortDirection(0, values);
            IsInitialized = true;
        }

        public byte[] ReadRegister(Pca9535.Register pca9535Register, int length)
        {
            return _ioExpander.ReadRegister(pca9535Register, length);
        }

        public void WriteAll(uint value)
        {
            if (!IsInitialized)
                Initialize();

            byte[] valueByte = { (byte)(~value & 0xff), (byte)(~(value >> 8) & 0xff) };
            IoExpanderWritePort(0, valueByte);
        }

        public void WritePort(int port, byte value)
        {
            if (port < 0 || 1 < port)
                throw new ArgumentOutOfRangeException(nameof(port));

            if (!IsInitialized)
                Initialize();

            value = (byte)~value;
            IoExpanderWritePort(port, value);
        }

        public void WritePin(int pin, PinState pinState)
        {
            if (pin < 0 || 15 < pin)
                throw new ArgumentOutOfRangeException(nameof(pin));

            if (!IsInitialized)
                Initialize();

            var state = pinState == PinState.Off;
            PinToDevicePin(pin, out var devicePort, out var devicePin);
            IoExpanderWritePin(devicePort, devicePin, state);
        }

        private static void PinToDevicePin(int pin, out int devicePort, out int devicePin)
        {
            if (pin < 8)
                devicePort = 0;
            else if (pin < 16)
                devicePort = 1;
            else
                devicePort = 2;

            devicePin = pin % 8;
        }

        private void IoExpanderSetPortDirection(int port, byte[] values)
        {
            _ioExpander.SetPortDirection(port, values);
        }

        private byte IoExpanderReadPort(int port)
        {
            return _ioExpander.ReadPort(port);
        }

        private void IoExpanderWritePort(int port, byte[] values)
        {
            _ioExpander.WritePort(port, values);
        }

        private void IoExpanderWritePort(int port, byte value)
        {
            _ioExpander.WritePort(port, value);
        }

        private void IoExpanderWritePin(int port, int pin, bool state)
        {
            _ioExpander.WritePin(port, pin, state);
        }
    }
}
