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
using OrangeNBT.NBT;
using Microsoft.Win32;

namespace HaruImageBlockConverter
{
    /// <summary>
    /// MainWindow.xaml の相互作用ロジック
    /// </summary>
    public partial class MainWindow : Window
    {
        private ProgressData convertFile = new ProgressData();

        private List<BlockColor> blockColors = new List<BlockColor>();

        public MainWindow()
        {
            InitializeComponent();

            this.DataContext = convertFile;
            this.DragEnter += Image_DragEnter;
            this.Drop += Image_DragDrop;

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
                    catch (Exception)
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

        /// <summary>
        /// 画像を選択ボタンクリックイベント
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Button_Click(object sender, RoutedEventArgs e)
        {
            // ダイアログのインスタンスを生成
            var dialog = new OpenFileDialog();

            // ファイルの種類を設定
            dialog.Filter = "全てのファイル (*.*)|*.*";

            // ダイアログを表示する
            if (dialog.ShowDialog() == true)
            {
                convertFile.FileName = dialog.FileName;
                ConvertStart();
            }
        }

        private void Image_DragEnter(object sender, DragEventArgs e)
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

            foreach (var file in files)
            {
                convertFile.FileName = file;
                ConvertStart();
            }
        }

        /// <summary>
        /// ファイル変換を開始する
        /// </summary>
        /// <param name="file"></param>
        private async void ConvertStart()
        {
            try
            {
                string file = convertFile.FileName;
                string fileName = System.IO.Path.GetFileName(file);
                
                using (Bitmap bitmap = new Bitmap(file))
                {
                    ImageConvert imageConvert = new ImageConvert(convertFile, blockColors.ToArray());
                    TagCompound compound = new TagCompound();
                    string savefile = System.IO.Path.Combine(System.IO.Path.GetDirectoryName(file), System.IO.Path.GetFileNameWithoutExtension(file));
                    bool convertType = (bool)NBTRadioButton.IsChecked;
                    convertFile.Total = bitmap.Height * 2;
                    convertFile.Complete = 0;
                    convertFile.ProgressMsg = "変換中";

                    ProgressBorder.Visibility = Visibility.Visible;
                    ProgressButton.Visibility = Visibility.Visible;

                    // 非同期処理
                    var task = Task.Run(() =>
                    {
                        if (convertType)
                        {
                            compound = imageConvert.ToNBT(bitmap);
                            savefile += ".nbt";
                        }
                        else
                        {
                            compound = imageConvert.ToSchematic(bitmap);
                            savefile += ".schematic";
                        }

                        convertFile.ProgressMsg = "出力中";

                        OrangeNBT.NBT.IO.NBTFile.ToFile(savefile, compound);
                    });

                    await task;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(@"変換に失敗しました\" + ex.Message, "ハルの画像変換ソフト",
                    MessageBoxButton.OK, MessageBoxImage.Error);
            }
            finally
            {
                ProgressBorder.Visibility = Visibility.Collapsed;
                ProgressButton.Visibility = Visibility.Collapsed;
            }
        }
    }
}
