// MIT License
//
// Copyright (c) Y2 Corporation

using System;
using System.Threading;
using Iot.Device.Ft4222;
using Y2.Ft4222.Core;

namespace Y2.Dio84ReUbc.Core
{
    public class Ads1115Slave : Ft4222I2cSlaveDevice
    {
        public Ads1115Slave(IFt4222I2cMaster i2c, int slaveAddress)
            : base(i2c, slaveAddress)
        {
            if (i2c == null)
                throw new ArgumentNullException(nameof(i2c));

            if (i2c.FrequencyKbps > 400)
                throw new ArgumentOutOfRangeException(nameof(i2c));
        }

        public enum Mux
        {
            Ain0Ain1,
            Ain0Ain3,
            Ain1Ain3,
            Ain2Ain3,
            Ain0Gnd,
            Ain1Gnd,
            Ain2Gnd,
            Ain3Gnd
        }

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

        public enum DataRate
        {
            Sps8,
            Sps16,
            Sps32,
            Sps64,
            Sps128,  // Default
            Sps250,
            Sps475,
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
            WriteEx(I2cMasterFlag.Start, conversion);
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
            ReadEx(I2cMasterFlag.RepeatedStart | I2cMasterFlag.Stop, readBuffer);
            return (short)((readBuffer[0] << 8) | readBuffer[1]);
        }
    }
}
