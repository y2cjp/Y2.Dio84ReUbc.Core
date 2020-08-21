// MIT License
//
// Copyright (c) Y2 Corporation

using System;
using Y2.Ft4222.Core;

namespace Y2.Dio84ReUbc.Core
{
    /// <summary>
    /// PCA9535
    /// </summary>
    public class Pca9535 : Ft4222I2cSlaveDevice
    {
        private const byte PortMax = 1;

        private readonly Memory<byte> _outputValue = new byte[] { 0xff, 0xff };
        private readonly Memory<byte> _portDirection = new byte[] { 0xff, 0xff };

        /// <summary>
        /// Initializes a new instance of the <see cref="Pca9535"/> class.
        /// </summary>
        /// <param name="i2c">The I2C master device.</param>
        /// <param name="slaveAddress">The bus address of the I2C device.</param>
        public Pca9535(IFt4222I2cMaster i2c, int slaveAddress)
            : base(i2c, slaveAddress)
        {
            if (i2c == null)
                throw new ArgumentNullException(nameof(i2c));

            if (i2c.FrequencyKbps > 400)
                throw new ArgumentOutOfRangeException(nameof(i2c));
        }

        /// <summary>
        /// レジスタ
        /// </summary>
        public enum Register
        {
            /// <summary>
            /// Input Port 0
            /// </summary>
            InputPort0 = 0x00,

            /// <summary>
            /// Output Port 0
            /// </summary>
            OutputPort0 = 0x02,

            /// <summary>
            /// Polarity Inversion 0
            /// </summary>
            PolarityInversion0 = 0x04,

            /// <summary>
            /// IO Configuration 0
            /// </summary>
            IoConfiguration0 = 0x06
        }

        /// <summary>
        /// 入出力方向
        /// </summary>
        public enum PinDirection
        {
            /// <summary>
            /// 出力
            /// </summary>
            Output,

            /// <summary>
            /// 入力
            /// </summary>
            Input
        }

        /// <summary>
        /// 複数のポートの入出力方向を設定
        /// </summary>
        /// <param name="port">ポート番号</param>
        /// <param name="values">設定値</param>
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

        /// <summary>
        /// ポートの入出力方向を設定
        /// </summary>
        /// <param name="port">ポート番号</param>
        /// <param name="value">設定値</param>
        public void SetPortDirection(int port, byte value)
        {
            ReadOnlySpan<byte> buffer = new[] { value };
            SetPortDirection(port, buffer);
        }

        /// <summary>
        /// ピンの入出力方向を設定
        /// </summary>
        /// <param name="port">ポート番号</param>
        /// <param name="pin">ピン番号</param>
        /// <param name="pinDirection">設定値</param>
        public void SetPinDirection(int port, int pin, PinDirection pinDirection)
        {
            if (port < 0 || PortMax < port)
                throw new ArgumentOutOfRangeException(nameof(port));

            if (pin < 0 || 7 < pin)
                throw new ArgumentOutOfRangeException(nameof(pin));

            byte value;
            if (pinDirection == PinDirection.Input)
                value = (byte)(_portDirection.Span[port] | (1 << pin));
            else
                value = (byte)(_portDirection.Span[port] & ~(1 << pin));

            SetPortDirection(port, value);
        }

        /// <summary>
        /// レジスタの読み出し
        /// </summary>
        /// <param name="pca9535Register">レジスタ</param>
        /// <param name="numOfPort">読み出すレジスタ数</param>
        /// <returns>読み出された値</returns>
        public byte[] ReadRegister(Register pca9535Register, int numOfPort)
        {
            ReadOnlySpan<byte> writeBuffer = stackalloc byte[] { (byte)pca9535Register };
            WriteEx(I2cMasterFlags.Start, writeBuffer);
            Span<byte> buffer = stackalloc byte[numOfPort];
            ReadEx(I2cMasterFlags.RepeatedStart | I2cMasterFlags.Stop, buffer);
            return buffer.ToArray();
        }

        /// <summary>
        /// ポートの読み出し
        /// </summary>
        /// <param name="port">ポート番号</param>
        /// <returns>読み出された値</returns>
        public byte ReadPort(int port)
        {
            var values = ReadPort(port, 1);
            return values[0];
        }

        /// <summary>
        /// 複数ポートの読み出し
        /// </summary>
        /// <param name="port">ポート番号</param>
        /// <param name="numOfPort">読み出すポートの数</param>
        /// <returns>読み出された値</returns>
        public byte[] ReadPort(int port, int numOfPort)
        {
            if (port < 0 || PortMax < port)
                throw new ArgumentOutOfRangeException(nameof(port));

            return ReadRegister(Register.InputPort0 + (byte)port, numOfPort);
        }

        /// <summary>
        /// ピンの状態の読み出し
        /// </summary>
        /// <param name="port">ポート番号</param>
        /// <param name="pin">ピン番号</param>
        /// <returns>ピンの状態</returns>
        public bool ReadPin(int port, int pin)
        {
            if (port < 0 || PortMax < port)
                throw new ArgumentOutOfRangeException(nameof(port));

            if (pin < 0 || 7 < pin)
                throw new ArgumentOutOfRangeException(nameof(pin));

            var value = ReadPort(port);
            return (value & (1 << pin)) != 0;
        }

        /// <summary>
        /// 複数ポートの制御
        /// </summary>
        /// <param name="port">ポート番号</param>
        /// <param name="values">設定値</param>
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

        /// <summary>
        /// ポートの制御
        /// </summary>
        /// <param name="port">ポート番号</param>
        /// <param name="value">設定値</param>
        public void WritePort(int port, byte value)
        {
            byte[] buffer = { value };
            WritePort(port, buffer);
        }

        /// <summary>
        /// ピンの制御
        /// </summary>
        /// <param name="port">ポート番号</param>
        /// <param name="pin">ピン番号</param>
        /// <param name="state">設定値</param>
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
