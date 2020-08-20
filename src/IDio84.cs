// MIT License
//
// Copyright (c) Y2 Corporation

namespace Y2.Dio84ReUbc.Core
{
    /// <summary>
    /// Interface for a DIO-8/4RD-IRC
    /// </summary>
    public interface IDio84
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
        /// ピンの状態を読み出す。
        /// </summary>
        /// <param name="pin">ピン番号</param>
        /// <returns>ピンの状態</returns>
        PinState ReadPin(int pin);

        /// <summary>
        /// ポート（8ビット）の状態を読み出す。
        /// </summary>
        /// <returns>ポートの状態</returns>
        byte ReadPort();

        /// <summary>
        /// ピンに出力をする。
        /// </summary>
        /// <param name="pin">ピン番号</param>
        /// <param name="pinState">出力値</param>
        void WritePin(int pin, PinState pinState);

        /// <summary>
        /// ポート（8ビット）に出力をする
        /// </summary>
        /// <param name="value">出力値</param>
        void WritePort(byte value);

        /// <summary>
        /// MikroBusのリセットピンを制御する。
        /// </summary>
        /// <param name="pinState">出力値</param>
        void SetMikroBusResetPin(PinState pinState);
    }
}
