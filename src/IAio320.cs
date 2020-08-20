// MIT License
//
// Copyright (c) Y2 Corporation

using System.Collections.Generic;

namespace Y2.Dio84ReUbc.Core
{
    /// <summary>
    /// Interface for a AIO-32/0RA-IRC
    /// </summary>
    public interface IAio320
    {
        /// <summary>
        /// 初期化済みか？
        /// </summary>
        bool IsInitialized { get; }

        /// <summary>
        /// 初期化をする。
        /// </summary>
        void Initialize();

        /// <summary>
        /// アナログ値（バイナリ）を取得する。
        /// </summary>
        /// <param name="channel">チャネル</param>
        /// <param name="dataRate">データーレート</param>
        /// <param name="pga">ゲイン</param>
        /// <returns>アナログ値（バイナリ）</returns>
        int ReadRaw(int channel, Aio320.DataRate dataRate = Aio320.DataRate.Sps128, Aio320.Pga pga = Aio320.Pga.Fs10035mV);

        /// <summary>
        /// 複数のアナログ値（バイナリ）を取得する。
        /// </summary>
        /// <param name="startChannel">開始チャネル</param>
        /// <param name="numOfChannels">チャネル数</param>
        /// <param name="dataRate">データレート</param>
        /// <param name="pga">ゲイン</param>
        /// <returns>複数のアナログ値（バイナリ）</returns>
        List<int> ReadRaw(int startChannel, int numOfChannels, Aio320.DataRate dataRate = Aio320.DataRate.Sps128, Aio320.Pga pga = Aio320.Pga.Fs10035mV);

        /// <summary>
        /// 複数のアナログ値（電圧）を取得する。
        /// </summary>
        /// <param name="startChannel">開始チャネル</param>
        /// <param name="numOfChannels">チャネル数</param>
        /// <param name="dataRate">データレート</param>
        /// <param name="pga">ゲイン</param>
        /// <returns>複数のアナログ値（電圧）</returns>
        List<double> ReadVoltage(int startChannel, int numOfChannels, Aio320.DataRate dataRate = Aio320.DataRate.Sps128, Aio320.Pga pga = Aio320.Pga.Fs10035mV);
    }
}
