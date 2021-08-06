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
using OrangeNBT.NBT.IO;

namespace HaruImageBlockConverter
{
    /// <summary>
    /// MainWindow.xaml の相互作用ロジック
    /// </summary>
    public partial class MainWindow : Window
    {
        private ProgressData convertFile = new ProgressData();

        private List<BlockColor> blockColors = new List<BlockColor>();

        private List<BlockColor> mapColors = new List<BlockColor>();

        public MainWindow()
        {
            InitializeComponent();

            this.DataContext = convertFile;
            this.DragEnter += Image_DragEnter;
            this.Drop += Image_DragDrop;

            if (!(File.Exists(@"\Colors\BlockColor.csv") && File.Exists(@"\Colors\MapColor.csv")))
            {
                // ブロックRGB
                using (var sr = new System.IO.StreamReader(@"Colors\BlockColor.csv", System.Text.Encoding.Default))
                {
                    // 読み込みできる文字がなくなるまで繰り返す
                    while (sr.Peek() >= 0)
                    {
                        string line = sr.ReadLine();

                        string[] split = line.Split(',');

                        var bc = new BlockColor()
                        {
                            BlockName = split[0],
                            R = byte.Parse(split[1]),
                            G = byte.Parse(split[2]),
                            B = byte.Parse(split[3])
                        };

                        blockColors.Add(bc);
                    }
                }

                // マップRGB
                using (var sr = new System.IO.StreamReader(@"Colors\MapColor.csv", System.Text.Encoding.Default))
                {
                    // 読み込みできる文字がなくなるまで繰り返す
                    while (sr.Peek() >= 0)
                    {
                        string line = sr.ReadLine();

                        string[] split = line.Split(',');

                        var bc = new BlockColor()
                        {
                            BlockName = split[0],
                            R = byte.Parse(split[1]),
                            G = byte.Parse(split[2]),
                            B = byte.Parse(split[3])
                        };

                        mapColors.Add(bc);
                    }
                }
            }
            else
            {
                MessageBox.Show("カラー定義ファイルが見つかりませんでした\nソフトウェアを終了します。", "ハルの画像ブロック変換ソフト",
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
            var dialog = new OpenFileDialog
            {
                Filter = "全てのファイル (*.*)|*.*",
                Title = "画像ファイルを選択"
            };

            // ダイアログを表示する
            if (dialog.ShowDialog() == true)
            {
                string[] files = new string[1];
                files[0] = dialog.FileName;
                ConvertStart(files);
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

            ConvertStart(files);
        }

        /// <summary>
        /// ファイル変換を開始する
        /// </summary>
        /// <param name="file"></param>
        private async void ConvertStart(string[] files)
        {
            foreach (var file in files)
            {
                try
                {
                    convertFile.FileName = file;
                    string fileName = System.IO.Path.GetFileName(file);

                    using (Bitmap bitmap = new Bitmap(file))
                    {
                        ImageConvert imageConvert = null;
                        
                        bool outputType = (bool)NBTRadioButton.IsChecked;
                        bool isDither = (bool)FloydSteinbergRadioButton.IsChecked;
                        convertFile.Total = bitmap.Height * 2;
                        convertFile.Complete = 0;

                        ProgressBorder.Visibility = Visibility.Visible;
                        ConvertButton.Visibility = Visibility.Visible;
                        this.AllowDrop = false;

                        byte convertType;

                        if (NBTRadioButton.IsChecked.Value)
                        {
                            convertType = 0;
                        }
                        else if (SchematicRadioButton.IsChecked.Value)
                        {
                            convertType = 1;
                        }
                        else
                        {
                            convertType = 2;
                        }

                        var savefiles = new Dictionary<string, TagCompound>();

                        // 非同期処理
                        var convertTask = Task.Run(() =>
                        {
                            switch (convertType)
                            {
                                case 0:
                                    {
                                        imageConvert = new ImageConvert(convertFile, blockColors.ToArray());
                                        var compound = imageConvert.ToNBT(bitmap, isDither);
                                        string savefile = System.IO.Path.Combine(System.IO.Path.GetDirectoryName(file), System.IO.Path.GetFileNameWithoutExtension(file)) + ".nbt";
                                        savefiles.Add(savefile, compound);
                                    }
                                    break;
                                case 1:
                                    {
                                        imageConvert = new ImageConvert(convertFile, blockColors.ToArray());
                                        var compound = imageConvert.ToSchematic(bitmap, isDither);
                                        string savefile = System.IO.Path.Combine(System.IO.Path.GetDirectoryName(file), System.IO.Path.GetFileNameWithoutExtension(file)) + ".schematic";
                                        savefiles.Add(savefile, compound);
                                    }
                                    break;
                                case 2:
                                    {
                                        imageConvert = new ImageConvert(convertFile, mapColors.ToArray());
                                        int count = 0;

                                        for (int y = 0; y < (bitmap.Height / 128) + 1; y++)
                                        {
                                            for (int x = 0; x < (bitmap.Width / 128) + 1; x++)
                                            {
                                                int ry = 0;
                                                int rx = 0;
                                                int nowx = x * 128;
                                                int nowy = y * 128;

                                                if (bitmap.Height - nowy >= 128)
                                                {
                                                    ry = 128;
                                                }
                                                else
                                                {
                                                    ry = bitmap.Height - nowy;
                                                }

                                                if (bitmap.Width - nowx >= 128)
                                                {
                                                    rx = 128;
                                                }
                                                else
                                                {
                                                    rx = bitmap.Width - nowx;
                                                }
                                                if (rx >= 0 && ry >= 0)
                                                {
                                                    System.Drawing.Rectangle rect = new System.Drawing.Rectangle(nowx, nowy, rx, ry);
                                                    using (Bitmap bmpNew = bitmap.Clone(rect, bitmap.PixelFormat))
                                                    {
                                                        var compound = imageConvert.ToMap(bmpNew, isDither);
                                                        string savefile = System.IO.Path.Combine(System.IO.Path.GetDirectoryName(file), "map_" + count.ToString() + ".dat");
                                                        savefiles.Add(savefile, compound);
                                                    }
                                                }
                                                count++;
                                            }
                                        }
                                    }
                                    break;
                            }
                        });

                        await convertTask;

                        ConvertButton.Visibility = Visibility.Collapsed;
                        BuildButton.Visibility = Visibility.Visible;

                        var buildTask = Task.Run(() =>
                        {
                            foreach(string key in savefiles.Keys)
                            {
                                OrangeNBT.NBT.IO.NBTFile.ToFile(key, savefiles[key]);
                            }
                        });

                        await buildTask;

                        ImageView.Source = imageConvert.ConvertBitmap(bitmap);
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
                    ConvertButton.Visibility = Visibility.Collapsed;
                    BuildButton.Visibility = Visibility.Collapsed;
                    this.AllowDrop = true;
                }
            }
        }
    }
}
