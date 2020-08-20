# DIO-8/4RE-UBC サンプル

USB-I2C変換ボード（[DIO-8/4RE-UBC](https://www.y2c.co.jp/i2c-r/dio-8-4re-ubc.html) ）のサンプルです。  

### フォルダ構成

プロジェクト名|フレームワーク|UI|概要|言語|注記
---|---|---|---|---|---
Dio84ReUbc.CoreLibrary|.NET  Core|-|ライブラリ|C#|※1
Dio84ReUbc.CoreSample|.NET  Core|-|サンプルアプリケーション<br>（UI非依存部分）|C#|-
Dio84ReUbc.CoreSampleWpf|.NET  Core|WPF|サンプルアプリケーション|C#|-
Dio84ReUbc.NfLibrary|.NET  Framework|-|ライブラリ|C#|※2
Dio84ReUbc.NfSample|.NET  Framework|-|サンプルアプリケーション<br>（UI非依存部分）|C#|-
Dio84ReUbc.NfSampleWinform|.NET  Framework|WinForms|サンプルアプリケーション|C#|-
Dio84ReUbc.NfSampleWinformVb|.NET  Framework|WinForms|サンプルアプリケーション|Visual Basic|-


※1  

.NET Core（クロスプラットフォーム）に対応したライブラリです。  
今後の機能拡張などはこちらを優先しておこないます。  
フレームワークに制約がないのであれば、こちらをお使いいただくのがおすすめです。

※2  

.NET Frameworkに対応したライブラリです。  
C#の古いバージョンや他の言語にも移植しやすいように、C#特有の機能（例外・継承・var・null演算子など）は極力使用しないようにしています。

### [使用方法](https://www.y2c.co.jp/i2c-r/dio-8-4re-ubc/windows.html)  

使用例

* [パソコンからアナログ値を測定する（アナログ入力32点を増設）](
https://www.y2c.co.jp/i2c-r/aio-32-0ra-irc/windows.html)

* [パソコンから絶縁デジタル入出力を制御する（絶縁デジタル入力8点・絶縁デジタル出力4点を増設）](https://www.y2c.co.jp/i2c-r/dio-8-4rd-irc/windows.html)

* [パソコンからDCモータを制御する](https://www.y2c.co.jp/i2c-r/dio-8-4re-ubc/adafruit2348.html)

* [パソコンからOLEDディスプレイを制御する](https://www.y2c.co.jp/i2c-r/dio-8-4re-ubc/mikroe1649.html)