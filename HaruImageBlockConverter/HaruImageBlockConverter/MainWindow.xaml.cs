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
using System.IO;
using HaruImageBlockConverter.Convert;
using System.Drawing;

namespace HaruImageBlockConverter
{
    /// <summary>
    /// MainWindow.xaml の相互作用ロジック
    /// </summary>
    public partial class MainWindow : Window
    {
        private List<BlockColor> blockColors = new List<BlockColor>();


        public MainWindow()
        {
            InitializeComponent();

            this.DragEnter += Image_DragEnter;
            this.LogText.DragEnter += Image_DragEnter;
            this.Drop += Image_DragDrop;
            this.LogText.Drop += Image_DragDrop;
            
            if (Directory.Exists("Block"))
            {
                string[] files = System.IO.Directory.GetFiles(@"Block", "*", System.IO.SearchOption.TopDirectoryOnly);
                
                foreach (string file in files)
                {
                    try
                    {
                        using (Bitmap bitmap = new Bitmap(file))
                        {
                            var color = bitmap.GetPixel(0, 0);
                            BlockColor blockColor = new BlockColor()
                            {
                                R = color.R,
                                G = color.G,
                                B = color.B,
                                BlockName = System.IO.Path.GetFileNameWithoutExtension(file)
                            };

                            blockColors.Add(blockColor);
                        }
                    }
                    catch(Exception)
                    {
                        MessageBox.Show("カラー定義ファイルの読み込みに失敗しました\nソフトウェアを終了します。", "ハルの画像ブロック変換ソフト",
                            MessageBoxButton.OK, MessageBoxImage.Error);
                        Application.Current.Shutdown();
                    }
                }
            }
            else
            {
                MessageBox.Show("カラー定義フォルダが見つかりませんでした\nソフトウェアを終了します。", "ハルの画像ブロック変換ソフト",
                    MessageBoxButton.OK, MessageBoxImage.Error);
                Application.Current.Shutdown();
            }
        }

        private void Image_DragEnter(object sender,DragEventArgs e)
        {
            if (e.Data.GetDataPresent(DataFormats.FileDrop))
            {
                e.Effects = DragDropEffects.Copy;
            }
            else
            {
                e.Effects = DragDropEffects.None;
            }
        }

        private void Image_DragDrop(object sender, DragEventArgs e)
        {
            //ドロップされたすべてのファイル名を取得する
            string[] files = (string[])e.Data.GetData(DataFormats.FileDrop, false);
            
            foreach(var file in files)
            {
                AppendLog("ファイル変換開始：" + System.IO.Path.GetFileName(file));

                try
                {
                    using (Bitmap bitmap = new Bitmap(file))
                    {
                        
                    }
                }
                catch (Exception)
                {
                    AppendLog("非対応形式の為処理をスキップします。");
                }
            }
        }

        /// <summary>
        /// 1行ログを出力
        /// </summary>
        /// <param name="msg">ログ内容</param>
        private void AppendLog(string msg)
        {
            this.LogText.AppendText(msg + Environment.NewLine);
        }
    }
}
