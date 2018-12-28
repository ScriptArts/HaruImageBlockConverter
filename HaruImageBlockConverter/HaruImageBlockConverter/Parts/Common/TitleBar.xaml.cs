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

namespace HaruImageBlockConverter.Parts.Common
{
    /// <summary>
    /// TitleBar.xaml の相互作用ロジック
    /// </summary>
    public partial class TitleBar : UserControl
    {
        /// <summary>
        /// 現在のウィンドウ
        /// </summary>
        Window window;

        public TitleBar()
        {
            InitializeComponent();
        }

        /// <summary>
        /// タイトルバーロードイベント処理
        /// </summary>
        /// <param name="sender">呼び出し元情報</param>
        /// <param name="e">イベント情報</param>
        private void TitleBar_Loaded(object sender, RoutedEventArgs e)
        {
            window = Window.GetWindow(this);

            //画面状態変更イベントを登録
            Application.Current.MainWindow.StateChanged += MainWindow_StateChanged;

            //タイトルのテキストを設定
            this.TitleText.FontFamily = new System.Windows.Media.FontFamily(System.Drawing.SystemFonts.MenuFont.FontFamily.Name);

            //タイトルバー初期化処理
            MainWindow_StateChanged(null, null);

            //タイトルバーをドラッグムーブできるように
            this.MouseLeftButtonDown += (asender, ae) => window.DragMove();
        }

        /// <summary>
        /// 画面状態変更時処理
        /// </summary>
        /// <param name="sender">呼び出し元情報</param>
        /// <param name="e">イベント情報</param>
        /// <remarks>
        /// タイトルバーの初回ロード時にも実行
        /// </remarks>
        private void MainWindow_StateChanged(object sender, EventArgs e)
        {
            if (Application.Current.MainWindow.WindowState == WindowState.Normal)
            {
                this.MaximizeButton.Visibility = System.Windows.Visibility.Visible;
                this.RestoreButton.Visibility = System.Windows.Visibility.Collapsed;
            }
            else if (Application.Current.MainWindow.WindowState == WindowState.Maximized)
            {
                this.MaximizeButton.Visibility = System.Windows.Visibility.Collapsed;
                this.RestoreButton.Visibility = System.Windows.Visibility.Visible;
            }
        }

        /// <summary>
        /// クローズボタンクリック処理
        /// </summary>
        /// <param name="sender">呼び出し元情報</param>
        /// <param name="e">イベント情報</param>
        private void CloseButton_Click(object sender, RoutedEventArgs e)
        {
            SystemCommands.CloseWindow(window);
        }

        /// <summary>
        /// 最大化ボタンクリック処理
        /// </summary>
        /// <param name="sender">呼び出し元情報</param>
        /// <param name="e">イベント情報</param>
        private void MaximizeButton_Click(object sender, RoutedEventArgs e)
        {
            SystemCommands.MaximizeWindow(window);
        }

        /// <summary>
        /// リストアボタンクリック処理
        /// </summary>
        /// <param name="sender">呼び出し元情報</param>
        /// <param name="e">イベント情報</param>
        private void RestoreButton_Click(object sender, RoutedEventArgs e)
        {
            SystemCommands.RestoreWindow(window);
        }

        /// <summary>
        /// 最小化ボタンクリック処理
        /// </summary>
        /// <param name="sender">呼び出し元情報</param>
        /// <param name="e">イベント情報</param>
        private void MinimizeButton_Click(object sender, RoutedEventArgs e)
        {
            SystemCommands.MinimizeWindow(window);
        }

        /// <summary>
        /// ボタンの非表示
        /// </summary>
        public void SubWindowToState()
        {
            MaximizeButton.Visibility = Visibility.Collapsed;
            RestoreButton.Visibility = Visibility.Collapsed;
            MinimizeButton.Visibility = Visibility.Collapsed;
        }
    }
}
