using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OrangeNBT.Data;

namespace HaruImageBlockConverter.Convert
{
    internal class BlockColor
    {
        /// <summary>
        /// 赤色
        /// </summary>
        public byte R { set; get; }
        
        /// <summary>
        /// 緑色
        /// </summary>
        public byte G { set; get; }

        /// <summary>
        /// 青色
        /// </summary>
        public byte B { set; get; }

        /// <summary>
        /// ブロック
        /// </summary>
        public string BlockName { set; get; }
    }
}
