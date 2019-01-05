using System.ComponentModel;

namespace HaruImageBlockConverter.Convert
{
    class ProgressData : INotifyPropertyChanged
    {
        
        private string _fileName { set; get; }

        private string _progressMsg { set; get; }
        
        private int _total { set; get; }
        
        private int _complete { set; get; }

        /// <summary>
        /// ファイル名
        /// </summary>
        public string FileName
        {
            get
            {
                return this._fileName;
            }
            set
            {
                this._fileName = value;
                this.OnPropertyChanged(nameof(FileName));

                return;
            }
        }

        /// <summary>
        /// 処理中メッセージ
        /// </summary>
        /// <returns></returns>
        public string ProgressMsg
        {
            get
            {
                return this._progressMsg;
            }
            set
            {
                this._progressMsg = value;
                this.OnPropertyChanged(nameof(ProgressMsg));

                return;
            }
        }

        /// <summary>
        /// 合計処理回数
        /// </summary>
        public int Total
        {
            get
            {
                return this._total;
            }
            set
            {
                this._total = value;
                this.OnPropertyChanged(nameof(Total));

                return;
            }
        }

        /// <summary>
        /// 現在終了処理回数
        /// </summary>
        public int Complete
        {
            get
            {
                return this._complete;
            }
            set
            {
                this._complete = value;
                this.OnPropertyChanged(nameof(Complete));

                return;
            }
        }

        public event PropertyChangedEventHandler PropertyChanged = null;

        protected void OnPropertyChanged(string info)
        {
            this.PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(info));
        }
    }
}
