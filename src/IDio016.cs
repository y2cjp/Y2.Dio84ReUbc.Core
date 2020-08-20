// MIT License
//
// Copyright (c) Y2 Corporation

namespace Y2.Dio84ReUbc.Core
{
    public interface IDio016
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
        /// 全てのピンに出力をする。
        /// </summary>
        /// <param name="value">出力値</param>
        void WriteAll(uint value);

        /// <summary>
        /// ひとつのピンに出力をする。
        /// </summary>
        /// <param name="pin">ピン番号</param>
        /// <param name="pinState">出力値</param>
        void WritePin(int pin, PinState pinState);

        /// <summary>
        /// ひとつのポート（8ビット）に出力をする
        /// </summary>
        /// <param name="port">ポート番号</param>
        /// <param name="value">出力値</param>
        void WritePort(int port, byte value);
    }
}
