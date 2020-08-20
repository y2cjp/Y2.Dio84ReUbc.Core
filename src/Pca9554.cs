// MIT License
//
// Copyright (c) Y2 Corporation

using System;
using Iot.Device.Ft4222;
using Y2.Ft4222.Core;

namespace Y2.Dio84ReUbc.Core
{
    public class Pca9554 : Ft4222I2cSlaveDevice
    {
        private byte _outputValue = 0xff;
        private byte _portDirection = 0xff;

        public Pca9554(IFt4222I2cMaster i2c, int slaveAddress)
            : base(i2c, slaveAddress)
        {
            if (i2c == null)
                throw new ArgumentNullException(nameof(i2c));

            if (i2c.FrequencyKbps > 400)
                throw new ArgumentOutOfRangeException(nameof(i2c));
        }

        public enum Register
        {
            InputPort,
            OutputPort,
            PolarityInversion,
            IoConfiguration
        }

        public enum PinDirection
        {
            Output,
            Input
        }

        public void SetPortDirection(byte value)
        {
            ReadOnlySpan<byte> writeBuffer = stackalloc byte[]
            {
                (byte)Register.IoConfiguration, value
            };
            Write(writeBuffer);
            _portDirection = value;
        }

        public void SetPinDirection(int pin, PinDirection pinDir)
        {
            if (pin < 0 || 7 < pin)
                throw new ArgumentOutOfRangeException(nameof(pin));

            byte value;
            if (pinDir == PinDirection.Input)
                value = (byte)(_portDirection | (1 << pin));
            else
                value = (byte)(_portDirection & ~(1 << pin));
            SetPortDirection(value);
        }

        public byte ReadRegister(Register pca9554Register)
        {
            ReadOnlySpan<byte> writeBuffer = stackalloc byte[] { (byte)pca9554Register };
            WriteEx(I2cMasterFlag.Start, writeBuffer);
            Span<byte> value = stackalloc byte[1];
            ReadEx(I2cMasterFlag.RepeatedStart | I2cMasterFlag.Stop, value);
            return value[0];
        }

        public byte ReadPort()
        {
            return ReadRegister(Register.InputPort);
        }

        public bool ReadPin(int pin)
        {
            if (pin < 0 || 7 < pin)
                throw new ArgumentOutOfRangeException(nameof(pin));
            var ip = ReadPort();
            return (ip & (1 << pin)) != 0;
        }

        public void WritePort(byte value)
        {
            byte[] writeBuffer = { (byte)Register.OutputPort, value };
            Write(writeBuffer);
            _outputValue = value;
        }

        public void WritePin(int pin, bool state)
        {
            if (pin < 0 || 7 < pin)
                throw new ArgumentOutOfRangeException(nameof(pin));
            byte value;
            if (state)
                value = (byte)(_outputValue | (1 << pin));
            else
                value = (byte)(_outputValue & ~(1 << pin));
            WritePort(value);
        }
    }
}
