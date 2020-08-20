// MIT License
//
// Copyright (c) Y2 Corporation

using System;
using Iot.Device.Ft4222;
using Y2.Ft4222.Core;

namespace Y2.Dio84ReUbc.Core
{
    /// <summary>
    /// PCA9554
    /// </summary>
    public class Pca9554 : Ft4222I2cSlaveDevice
    {
        private byte _outputValue = 0xff;
        private byte _portDirection = 0xff;

        /// <summary>
        /// Initializes a new instance of the <see cref="Pca9554"/> class.
        /// </summary>
        /// <param name="i2c">The I2C master device.</param>
        /// <param name="slaveAddress">The bus address of the I2C device.</param>
        public Pca9554(IFt4222I2cMaster i2c, int slaveAddress)
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
            /// Input Port
            /// </summary>
            InputPort,

            /// <summary>
            /// Output Port
            /// </summary>
            OutputPort,

            /// <summary>
            /// Polarity Inversion
            /// </summary>
            PolarityInversion,

            /// <summary>
            /// IO Configutation
            /// </summary>
            IoConfiguration
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
        /// ポートの入出力方向を設定
        /// </summary>
        /// <param name="value">設定値</param>
        public void SetPortDirection(byte value)
        {
            ReadOnlySpan<byte> writeBuffer = stackalloc byte[]
            {
                (byte)Register.IoConfiguration, value
            };
            Write(writeBuffer);
            _portDirection = value;
        }

        /// <summary>
        /// ピンの入出力方向を設定
        /// </summary>
        /// <param name="pin">ピン番号</param>
        /// <param name="pinDirection">設定値</param>
        public void SetPinDirection(int pin, PinDirection pinDirection)
        {
            if (pin < 0 || 7 < pin)
                throw new ArgumentOutOfRangeException(nameof(pin));

            byte value;
            if (pinDirection == PinDirection.Input)
                value = (byte)(_portDirection | (1 << pin));
            else
                value = (byte)(_portDirection & ~(1 << pin));
            SetPortDirection(value);
        }

        /// <summary>
        /// ポートの読み出し
        /// </summary>
        /// <returns>読み出された値</returns>
        public byte ReadPort()
        {
            return ReadRegister(Register.InputPort);
        }

        /// <summary>
        /// ピンの状態の読み出し
        /// </summary>
        /// <param name="pin">ピン番号</param>
        /// <returns>ピンの状態</returns>
        public bool ReadPin(int pin)
        {
            if (pin < 0 || 7 < pin)
                throw new ArgumentOutOfRangeException(nameof(pin));
            var ip = ReadPort();
            return (ip & (1 << pin)) != 0;
        }

        /// <summary>
        /// ポートの制御
        /// </summary>
        /// <param name="value">設定値</param>
        public void WritePort(byte value)
        {
            byte[] writeBuffer = { (byte)Register.OutputPort, value };
            Write(writeBuffer);
            _outputValue = value;
        }

        /// <summary>
        /// ピンの制御
        /// </summary>
        /// <param name="pin">ピン番号</param>
        /// <param name="state">設定値</param>
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

        private byte ReadRegister(Register pca9554Register)
        {
            ReadOnlySpan<byte> writeBuffer = stackalloc byte[] { (byte)pca9554Register };
            WriteEx(I2cMasterFlag.Start, writeBuffer);
            Span<byte> value = stackalloc byte[1];
            ReadEx(I2cMasterFlag.RepeatedStart | I2cMasterFlag.Stop, value);
            return value[0];
        }
    }
}
