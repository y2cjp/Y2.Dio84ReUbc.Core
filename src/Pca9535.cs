// MIT License
//
// Copyright (c) Y2 Corporation

using System;
using Iot.Device.Ft4222;
using Y2.Ft4222.Core;

namespace Y2.Dio84ReUbc.Core
{
    public class Pca9535 : Ft4222I2cSlaveDevice
    {
        private const byte PortMax = 1;

        private readonly Memory<byte> _outputValue = new byte[] { 0xff, 0xff };
        private readonly Memory<byte> _portDirection = new byte[] { 0xff, 0xff };

        public Pca9535(IFt4222I2cMaster i2c, int slaveAddress)
            : base(i2c, slaveAddress)
        {
            if (i2c == null)
                throw new ArgumentNullException(nameof(i2c));

            if (i2c.FrequencyKbps > 400)
                throw new ArgumentOutOfRangeException(nameof(i2c));
        }

        public enum Register
        {
            InputPort0 = 0x00,
            OutputPort0 = 0x02,
            PolarityInversion0 = 0x04,
            IoConfiguration0 = 0x06
        }

        public enum PinDirection
        {
            Output,
            Input
        }

        public void SetPortDirection(int port, ReadOnlySpan<byte> values)
        {
            if (port < 0 || PortMax < port)
                throw new ArgumentOutOfRangeException(nameof(port));

            var length = values.Length;
            if (length < 1 || PortMax + 1 < port + length)
                throw new ArgumentOutOfRangeException(nameof(port));

            if (values == null)
                throw new ArgumentNullException(nameof(values));

            Span<byte> buffer = stackalloc byte[values.Length + 1];
            buffer[0] = (byte)(Register.IoConfiguration0 + port);
            values.CopyTo(buffer.Slice(1));
            Write(buffer);
            values.CopyTo(_portDirection.Slice(port).Span);
        }

        public void SetPortDirection(int port, byte value)
        {
            ReadOnlySpan<byte> buffer = new[] { value };
            SetPortDirection(port, buffer);
        }

        public void SetPinDirection(int port, int pin, PinDirection pinDir)
        {
            if (port < 0 || PortMax < port)
                throw new ArgumentOutOfRangeException(nameof(port));

            if (pin < 0 || 7 < pin)
                throw new ArgumentOutOfRangeException(nameof(pin));

            byte value;
            if (pinDir == PinDirection.Input)
                value = (byte)(_portDirection.Span[port] | (1 << pin));
            else
                value = (byte)(_portDirection.Span[port] & ~(1 << pin));

            SetPortDirection(port, value);
        }

        public byte[] ReadRegister(Register pca9535Register, int length)
        {
            ReadOnlySpan<byte> writeBuffer = stackalloc byte[] { (byte)pca9535Register };
            WriteEx(I2cMasterFlag.Start, writeBuffer);
            Span<byte> buffer = stackalloc byte[length];
            ReadEx(I2cMasterFlag.RepeatedStart | I2cMasterFlag.Stop, buffer);
            return buffer.ToArray();
        }

        public void I2CReadEx(Span<byte> buffer, I2cMasterFlag flags)
        {
            ReadEx(flags, buffer);
        }

        public byte ReadPort(int port)
        {
            var values = ReadPort(port, 1);
            return values[0];
        }

        public byte[] ReadPort(int port, int length)
        {
            if (port < 0 || PortMax < port)
                throw new ArgumentOutOfRangeException(nameof(port));

            return ReadRegister(Register.InputPort0 + (byte)port, length);
        }

        public bool ReadPin(int port, int pin)
        {
            if (port < 0 || PortMax < port)
                throw new ArgumentOutOfRangeException(nameof(port));

            if (pin < 0 || 7 < pin)
                throw new ArgumentOutOfRangeException(nameof(pin));

            var value = ReadPort(port);
            return (value & (1 << pin)) != 0;
        }

        public void WritePort(int port, ReadOnlySpan<byte> values)
        {
            if (port < 0 || PortMax < port)
                throw new ArgumentOutOfRangeException(nameof(port));

            if (values == null)
                throw new ArgumentNullException(nameof(values));

            var length = values.Length;
            if (length < 1 || PortMax + 1 < port + length)
                throw new ArgumentOutOfRangeException(nameof(values));

            Span<byte> buffer = stackalloc byte[values.Length + 1];
            buffer[0] = (byte)(Register.OutputPort0 + port);
            values.CopyTo(buffer.Slice(1));
            Write(buffer);
            values.CopyTo(_outputValue.Slice(port).Span);
        }

        public void WritePort(int port, byte value)
        {
            byte[] buffer = { value };
            WritePort(port, buffer);
        }

        public void WritePin(int port, int pin, bool state)
        {
            if (port < 0 || PortMax < port)
                throw new ArgumentOutOfRangeException(nameof(port));

            if (pin < 0 || 7 < pin)
                throw new ArgumentOutOfRangeException(nameof(pin));

            byte value;
            if (state)
                value = (byte)(_outputValue.Span[port] | (1 << pin));
            else
                value = (byte)(_outputValue.Span[port] & ~(1 << pin));
            WritePort(port, value);
        }
    }
}
