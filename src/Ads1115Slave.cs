// MIT License
//
// Copyright (c) Y2 Corporation

using System;
using System.Threading;
using Y2.Ft4222.Core;

namespace Y2.Dio84ReUbc.Core
{
    /// <summary>
    /// ADS1115
    /// </summary>
    public class Ads1115Slave : Ft4222I2cSlaveDevice
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="Ads1115Slave"/> class.
        /// </summary>
        /// <param name="i2c">The I2C master device.</param>
        /// <param name="slaveAddress">The bus address of the I2C device.</param>
        public Ads1115Slave(IFt4222I2cMaster i2c, int slaveAddress)
            : base(i2c, slaveAddress)
        {
            if (i2c == null)
                throw new ArgumentNullException(nameof(i2c));

            if (i2c.FrequencyKbps > 400)
                throw new ArgumentOutOfRangeException(nameof(i2c));
        }

        /// <summary>
        /// Input multiplexer configuration
        /// </summary>
        public enum Mux
        {
            /// <summary>
            /// AIN0 - AIN1
            /// </summary>
            Ain0Ain1,

            /// <summary>
            /// AIN0 - AIN3
            /// </summary>
            Ain0Ain3,

            /// <summary>
            /// AIN1 - AIN3
            /// </summary>
            Ain1Ain3,

            /// <summary>
            /// AIN2 - AIN3
            /// </summary>
            Ain2Ain3,

            /// <summary>
            /// AIN0 - GND
            /// </summary>
            Ain0Gnd,

            /// <summary>
            /// AIN1 - GND
            /// </summary>
            Ain1Gnd,

            /// <summary>
            /// AIN2 - GND
            /// </summary>
            Ain2Gnd,

            /// <summary>
            /// AIN3 - GND
            /// </summary>
            Ain3Gnd
        }

        /// <summary>
        /// Programmable gain amplifier configuration
        /// </summary>
        public enum Pga
        {
            /// <summary>
            /// Full Scale: 6.144V
            /// </summary>
            Fs6144mV,

            /// <summary>
            /// Full Scale: 4.096V
            /// </summary>
            Fs4096mV,

            /// <summary>
            /// Full Scale: 2.048V
            /// </summary>
            Fs2048mV,

            /// <summary>
            /// Full Scale: 1.024V
            /// </summary>
            Fs1024mV,

            /// <summary>
            /// Full Scale: 0.512V
            /// </summary>
            Fs512mV,

            /// <summary>
            /// Full Scale: 0.256V
            /// </summary>
            Fs256mV
        }

        /// <summary>
        /// Control the data rate setting.
        /// </summary>
        public enum DataRate
        {
            /// <summary>
            /// 8SPS
            /// </summary>
            Sps8,

            /// <summary>
            /// 16SPS
            /// </summary>
            Sps16,

            /// <summary>
            /// 32SPS
            /// </summary>
            Sps32,

            /// <summary>
            /// 64SPS
            /// </summary>
            Sps64,

            /// <summary>
            /// 128SPS, Default
            /// </summary>
            Sps128,

            /// <summary>
            /// 250SPS
            /// </summary>
            Sps250,

            /// <summary>
            /// 475SPS
            /// </summary>
            Sps475,

            /// <summary>
            /// 860SPS
            /// </summary>
            Sps860
        }

        private enum Register : byte
        {
            Conversion = 0x00,
            Config = 0x01,
            //// LoThresh = 0x02,
            //// HiThresh = 0x03
        }

        private enum SingleShotCoversion : byte
        {
            //// NoEffect,
            Begin = 1
        }

        private enum Mode : byte
        {
            //// Continuous,
            PowerDownSingleShot = 1
        }

        private enum ComparatorQueue : byte
        {
            Disable = 0x03
        }

        /// <summary>
        /// アナログ値（バイナリ）を取得する。
        /// </summary>
        /// <param name="mux">入力マルチプレクサ</param>
        /// <param name="dataRate">データーレート</param>
        /// <param name="pga">ゲイン</param>
        /// <returns>アナログ値（バイナリ）</returns>
        public int ReadRaw(Mux mux, DataRate dataRate = DataRate.Sps128, Pga pga = Pga.Fs2048mV)
        {
            ReadOnlySpan<byte> config = stackalloc byte[]
            {
                (byte)Register.Config,
                (byte)(((byte)SingleShotCoversion.Begin << 7) | ((byte)mux << 4) | ((byte)pga << 1) | (byte)Mode.PowerDownSingleShot),
                (byte)(((byte)dataRate << 5) | (byte)ComparatorQueue.Disable)
            };
            Write(config);
            ReadOnlySpan<byte> conversion = stackalloc byte[] { (byte)Register.Conversion };
            WriteEx(I2cMasterFlags.Start, conversion);
            switch (dataRate)
            {
                case DataRate.Sps8:
                    Thread.Sleep(126);
                    break;
                case DataRate.Sps16:
                    Thread.Sleep(64);
                    break;
                case DataRate.Sps32:
                    Thread.Sleep(33);
                    break;
                case DataRate.Sps64:
                    Thread.Sleep(17);
                    break;
                case DataRate.Sps128:
                    Thread.Sleep(9);
                    break;
                case DataRate.Sps250:
                    Thread.Sleep(5);
                    break;
                case DataRate.Sps475:
                    Thread.Sleep(4);
                    break;
                case DataRate.Sps860:
                    Thread.Sleep(3);
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(dataRate));
            }

            Span<byte> readBuffer = stackalloc byte[2];
            ReadEx(I2cMasterFlags.RepeatedStart | I2cMasterFlags.Stop, readBuffer);
            return (short)((readBuffer[0] << 8) | readBuffer[1]);
        }
    }
}
