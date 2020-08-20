// MIT License
//
// Copyright (c) Y2 Corporation

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using Y2.Ft4222.Core;

namespace Y2.Dio84ReUbc.Core
{
    public sealed class Aio320 : IAio320
    {
        private readonly Pca9554 _pca9554;      // Multiplexer
        private readonly Ads1115Slave _ads1115; // ADC
        private byte _mux;

        public Aio320(IFt4222I2cMaster i2c, int adcAddress = 0x49, int muxAddress = 0x3e)
        {
            _pca9554 = new Pca9554(i2c, muxAddress);
            _ads1115 = new Ads1115Slave(i2c, adcAddress);
            _mux = 0xff;
        }

        public enum Pga
        {
            /// <summary>
            /// Full Scale: 10.0352V
            /// </summary>
            Fs10035mV = Ads1115Slave.Pga.Fs2048mV,

            /// <summary>
            /// Full Scale: 5.0176V
            /// </summary>
            Fs5018mV = Ads1115Slave.Pga.Fs1024mV,

            /// <summary>
            /// Full Scale: 2.5088V
            /// </summary>
            Fs2509mV = Ads1115Slave.Pga.Fs512mV,

            /// <summary>
            /// Full Scale: 1.2544V
            /// </summary>
            Fs1254mV = Ads1115Slave.Pga.Fs256mV
        }

        public enum DataRate
        {
            Sps8 = Ads1115Slave.DataRate.Sps8,
            Sps16 = Ads1115Slave.DataRate.Sps16,
            Sps32 = Ads1115Slave.DataRate.Sps32,
            Sps64 = Ads1115Slave.DataRate.Sps64,
            Sps128 = Ads1115Slave.DataRate.Sps128,
            Sps250 = Ads1115Slave.DataRate.Sps250,
            Sps475 = Ads1115Slave.DataRate.Sps475,
            Sps860 = Ads1115Slave.DataRate.Sps860
        }

        public bool IsInitialized { get; private set; }

        public void Initialize()
        {
            _pca9554.SetPortDirection(0x00);
            IsInitialized = true;
        }

        public int ReadRaw(int channel, DataRate dataRate = DataRate.Sps128, Pga pga = Pga.Fs10035mV)
        {
            if (!IsInitialized)
                Initialize();

            byte extMux;
            Ads1115Slave.Mux adcMux;
            if (channel < 16)
            {
                extMux = (byte)((_mux & 0xf0) | channel);
                adcMux = Ads1115Slave.Mux.Ain0Gnd;
            }
            else if (channel < 32)
            {
                extMux = (byte)((_mux & 0x0f) | ((channel & 0x0f) << 4));
                adcMux = Ads1115Slave.Mux.Ain1Gnd;
            }
            else if (channel < 48)
            {
                extMux = (byte)((_mux & 0xf0) | channel);
                adcMux = Ads1115Slave.Mux.Ain0Ain3;
            }
            else if (channel < 64)
            {
                extMux = (byte)((_mux & 0x0f) | ((channel & 0x0f) << 4));
                adcMux = Ads1115Slave.Mux.Ain1Ain3;
            }
            else if (channel < 256)
            {
                throw new ArgumentOutOfRangeException(nameof(channel));
            }
            else
            {
                extMux = (byte)(channel & 0xff);
                adcMux = Ads1115Slave.Mux.Ain0Ain1;
            }

            _pca9554.WritePort(extMux);
            _mux = extMux;
            Thread.Sleep(1);
            return _ads1115.ReadRaw(adcMux, (Ads1115Slave.DataRate)dataRate, (Ads1115Slave.Pga)pga);
        }

        public List<int> ReadRaw(int startChannel, int numOfChannels, DataRate dataRate = DataRate.Sps128, Pga pga = Pga.Fs10035mV)
        {
            var values = new List<int>();
            for (var ch = startChannel; ch < startChannel + numOfChannels; ch++)
            {
                var tmp = ReadRaw(ch, dataRate, pga);
                values.Add(tmp);
            }

            return values;
        }

        public List<double> ReadVoltage(int startChannel, int numOfChannels, DataRate dataRate = DataRate.Sps128, Pga pga = Pga.Fs10035mV)
        {
            return ReadRaw(startChannel, numOfChannels, dataRate, pga).Select(x => ToVolt(x, pga)).ToList();
        }

        private static double ToVolt(int value, Pga pga)
        {
            switch (pga)
            {
                case Pga.Fs1254mV:
                    return 1.2544F * value / 32767;
                case Pga.Fs2509mV:
                    return 2.5088F * value / 32767;
                case Pga.Fs5018mV:
                    return 5.0176F * value / 32767;
                case Pga.Fs10035mV:
                    return 10.0352F * value / 32767;
                default:
                    return 0;
            }
        }
    }
}
