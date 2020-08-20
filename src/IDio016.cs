// MIT License
//
// Copyright (c) Y2 Corporation

namespace Y2.Dio84ReUbc.Core
{
    /// <summary>
    /// Interface for a DIO-0/16RC-IRC
    /// </summary>
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
        /// ピンに出力をする。
        /// </summary>
        /// <param name="pin">ピン番号</param>
        /// <param name="pinState">出力値</param>
        void WritePin(int pin, PinState pinState);

        /// <summary>
        /// ポート（8ビット）に出力をする
        /// </summary>
        /// <param name="port">ポート番号</param>
        /// <param name="value">出力値</param>
        void WritePort(int port, byte value);

        /// <summary>
        /// レジスタを読み出す。
        /// </summary>
        /// <param name="pca9535Register">レジスタ</param>
        /// <param name="numOfRegisters">読み出すレジスタの数</param>
        /// <returns>読み出した値</returns>
        byte[] ReadRegister(Pca9535.Register pca9535Register, int numOfRegisters);
    }
}
