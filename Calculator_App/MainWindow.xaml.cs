using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace Calculator_App
{
    /// <summary>
    /// MainWindow.xaml の相互作用ロジック
    /// </summary>


    public partial class MainWindow : Window
    {
        /// <summary>
        /// 計算失敗メッセージ
        /// </summary>
        static readonly string CalcFailMessage = "計算失敗";

        /// <summary>
        /// 計算式最大長
        /// </summary>
        static readonly int MaxFormlaLength = 16;

        /// <summary>
        /// コンストラクター
        /// </summary>
        public MainWindow()
        {
            InitializeComponent();

            // イベント初期化
            InitEvents();
        }


        /// <summary>
        /// イベントをセットする
        /// </summary>
        private void InitEvents()
        {
            // rootGrid配下のコントロールを探す
            foreach (var ctrl in LogicalTreeHelper.GetChildren(rootGrid))
            {
                // ボタン要素以外は無視する
                if (!(ctrl is Button))
                {
                    // ループをスキップする
                    continue;
                }

                // ボタンがクリックされたときの処理
                (ctrl as Button).Click += (sender, e) =>
                {
                    // ボタンの文字列を取得する
                    string inKey = (sender as Button).Content.ToString();

                    switch (inKey)
                    {
                        case "AC":
                            // クリアする
                            formula.Text = "0";
                            break;
                        case "=":
                            if (formula.Text == CalcFailMessage)
                            {
                                formula.Text = "0";
                            }
                            else
                            {
                                // 計算結果表示
                                formula.Text = Calculate(formula.Text);
                            }
                            break;
                        default:
                            if (formula.Text.Length >= MaxFormlaLength)
                            {
                                break;
                            }
                            else if(formula.Text == "0" && (inKey != "/" && inKey != "*" && inKey != "-" && inKey != "+" ))
                            {
                                // 0が入力されていて数字が入力されたらその値を設定
                                formula.Text = inKey;
                            }
                            else if(formula.Text == CalcFailMessage)
                            {
                                // 計算失敗メッセージが表示されていたら入力された値を設定
                                formula.Text = inKey;
                            }
                            else
                            {
                                // 入力され値を式に設定
                                formula.Text += inKey;
                            }
                            break;
                        
                    }
                };
            }
        }

        /// <summary>
        /// 計算結果実行
        /// </summary>
        /// <returns></returns>
        private string Calculate(string formula)
        {
            try
            {
                var p = new System.Diagnostics.Process();

                // システムフォルダを取得
                string systemFolder = Environment.GetFolderPath(Environment.SpecialFolder.System);
                // PowerShellのフルパスを組み立て
                string powerShellFullPath = System.IO.Path.Combine(systemFolder, @"WindowsPowerShell\v1.0\powershell.exe");

                // 実行するファイル
                p.StartInfo.FileName = powerShellFullPath;
                // パラメータ（計算式）
                p.StartInfo.Arguments = "-Command " + formula;

                // 標準入力リダイレクトしない
                p.StartInfo.RedirectStandardInput = false;
                // 標準出力リダイレクトする
                p.StartInfo.RedirectStandardOutput = true;

                // シェル機能使用しない
                p.StartInfo.UseShellExecute = false;
                // ウィンドウ開かない
                p.StartInfo.CreateNoWindow = true;

                // 実行
                p.Start();
                // 結果を一行読む
                string calcResult = p.StandardOutput.ReadLine();

                p.WaitForExit();
                p.Close();

                if (string.IsNullOrEmpty(calcResult))
                {
                    // 計算結果なし（計算失敗）
                    return CalcFailMessage;
                }
                else
                {
                    // 計算結果を返す
                    return calcResult;
                }
            }
            catch
            {
                return CalcFailMessage;
            }
        }
    }
}
