using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using OrangeNBT;
using OrangeNBT.Data.Format;
using OrangeNBT.NBT;

namespace HaruImageBlockConverter.Convert
{
    internal class ImageConvert
    {
        private readonly BlockColor[] blockColors;

        public ImageConvert(BlockColor[] blockColors)
        {
            this.blockColors = blockColors;
        }

        public TagCompound ToSchematic(Bitmap bitmap)
        {
            var schematic = new Schematic(bitmap.Width, 1, bitmap.Height);

            var ditheringBitmap = FloydSteinberg(bitmap);

            using (BitmapAccessor accessor = new BitmapAccessor(bitmap))
            {
                for (int x = 0; x < accessor.Width; x++)
                {
                    for (int y = 0; y < accessor.Height; y++)
                    {

                        Color color = accessor.GetPixel(x, y);

                        BlockColor blockColor = RGBToBlockColor(color);

                        OrangeNBT.Data.IBlock block = OrangeNBT.Data.AnvilImproved.AnvilImprovedDataProvider.Instance.GetBlock("minecraft:" + blockColor.BlockName);

                        schematic.SetBlock(x, 0, y, block.DefaultBlockSet);
                    }
                }
            }

            return schematic.BuildTag();
        }

        public TagCompound ToNBT(Bitmap bitmap)
        {
            var structure = new Structure(bitmap.Width, 1, bitmap.Height);

            var ditheringBitmap = FloydSteinberg(bitmap);

            using (BitmapAccessor accessor = new BitmapAccessor(bitmap))
            {
                for (int x = 0; x < accessor.Width; x++)
                {
                    for (int y = 0; y < accessor.Height; y++)
                    {

                        Color color = accessor.GetPixel(x, y);

                        BlockColor blockColor = RGBToBlockColor(color);

                        try
                        {
                            OrangeNBT.Data.IBlock block = OrangeNBT.Data.AnvilImproved.AnvilImprovedDataProvider.Instance.GetBlock("minecraft:" + blockColor.BlockName);

                            structure.SetBlock(x, 0, y, block.DefaultBlockSet);
                        }
                        catch(Exception ex)
                        {
                            MessageBox.Show(blockColor.BlockName);
                        }
                        
                    }
                }
            }

            return structure.BuildTag();
        }

        #region FloydSteinberg


        /// <summary>
        /// FloydSteinberg誤差拡散を行う
        /// </summary>
        /// <param name="bmp"></param>
        /// <returns></returns>
        private Bitmap FloydSteinberg(Bitmap bmp)
        {
            using (BitmapAccessor accessor = new BitmapAccessor(bmp))
            {
                for (int y = 0; y < bmp.Height; y++)
                {
                    for (int x = 0; x < bmp.Width; x++)
                    {

                        Color bitmapRGB = accessor.GetPixel(x, y);
                        var block = RGBToBlockColor(bitmapRGB);

                        var blockRGB = Color.FromArgb(block.R, block.G, block.B);

                        if (x < accessor.Width - 1)
                        {
                            var nextPixel = accessor.GetPixel(x + 1, y);
                            var d = (decimal)7 / (decimal)16;
                            accessor.SetPixel(x + 1, y, GetDitheringColor(d, nextPixel, bitmapRGB, blockRGB));
                        }
                        /*
                        if (x < accessor.Width - 2)
                        {
                            var nextPixel = accessor.GetPixel(x + 2, y);
                            var d = (decimal)4 / (decimal)42;
                            accessor.SetPixel(x + 2, y, GetDitheringColor(d, nextPixel, pc, bc));
                        }
                        */
                        /*
                        if (x > 1 && y < accessor.Height - 1)
                        {
                            var nextPixel = accessor.GetPixel(x - 2, y + 1);
                            var d = (decimal)2 / (decimal)42;
                            accessor.SetPixel(x - 2, y + 1, GetDitheringColor(d, nextPixel, pc, bc));
                        }
                        */

                        if (x > 0 && y < accessor.Height - 1)
                        {
                            var nextPixel = accessor.GetPixel(x - 1, y + 1);
                            var d = (decimal)3 / (decimal)16;
                            accessor.SetPixel(x - 1, y + 1, GetDitheringColor(d, nextPixel, bitmapRGB, blockRGB));
                        }

                        if (y < accessor.Height - 1)
                        {
                            var nextPixel = accessor.GetPixel(x, y + 1);
                            var d = (decimal)5 / (decimal)42;
                            accessor.SetPixel(x, y + 1, GetDitheringColor(d, nextPixel, bitmapRGB, blockRGB));
                        }

                        if (x < accessor.Width - 1 && y < accessor.Height - 1)
                        {
                            var nextPixel = accessor.GetPixel(x + 1, y + 1);
                            var d = (decimal)1 / (decimal)42;
                            accessor.SetPixel(x + 1, y + 1, GetDitheringColor(d, nextPixel, bitmapRGB, blockRGB));
                        }

                        /*
                        if (x < accessor.Width - 2 && y < accessor.Height - 1)
                        {
                            var nextPixel = accessor.GetPixel(x + 2, y + 1);
                            var d = (decimal)2 / (decimal)42;
                            accessor.SetPixel(x + 2, y + 1, GetDitheringColor(d, nextPixel, pc, bc));
                        }
                        */
                        /*
                        if (x > 1 && y < accessor.Height - 2)
                        {
                            var nextPixel = accessor.GetPixel(x - 2, y + 2);
                            var d = (decimal)1 / (decimal)42;
                            accessor.SetPixel(x - 2, y + 2, GetDitheringColor(d, nextPixel, pc, bc));
                        }

                        //
                        if (x > 0 && y < accessor.Height - 2)
                        {
                            var nextPixel = accessor.GetPixel(x - 1, y + 1);
                            var d = (decimal)2 / (decimal)42;
                            accessor.SetPixel(x - 1, y + 2, GetDitheringColor(d, nextPixel, pc, bc));
                        }

                        //
                        if (y < accessor.Height - 2)
                        {
                            var nextPixel = accessor.GetPixel(x, y + 2);
                            var d = (decimal)4 / (decimal)42;
                            accessor.SetPixel(x, y + 2, GetDitheringColor(d, nextPixel, pc, bc));
                        }

                        //
                        if (x < accessor.Width - 2 && y < accessor.Height - 2)
                        {
                            var nextPixel = accessor.GetPixel(x + 1, y + 2);
                            var d = (decimal)2 / (decimal)42;
                            accessor.SetPixel(x + 1, y + 2, GetDitheringColor(d, nextPixel, pc, bc));
                        }

                        if (x < accessor.Width - 2 && y < accessor.Height - 2)
                        {
                            var nextPixel = accessor.GetPixel(x + 2, y + 2);
                            var d = (decimal)1 / (decimal)42;
                            accessor.SetPixel(x + 2, y + 2, GetDitheringColor(d, nextPixel, pc, bc));
                        }
                        */
                    }
                }
            }
            return bmp;
        }

        /// <summary>
        /// 誤差拡散
        /// </summary>
        /// <param name="d">誤差</param>
        /// <param name="nextPixel">次のピクセルの色</param>
        /// <param name="pictureColor">画像の色</param>
        /// <param name="blockColor"></param>
        /// <returns></returns>
        private Color GetDitheringColor(decimal d, Color nextPixel, Color pictureColor, Color blockColor)
        {
            int gosaR = pictureColor.R - blockColor.R;
            int gosaG = pictureColor.G - blockColor.G;
            int gosaB = pictureColor.B - blockColor.B;
            int r = Math.Max(0, Math.Min(255, (int)Math.Floor(nextPixel.R + (gosaR * d))));
            int g = Math.Max(0, Math.Min(255, (int)Math.Floor(nextPixel.G + (gosaG * d))));
            int b = Math.Max(0, Math.Min(255, (int)Math.Floor(nextPixel.B + (gosaB * d))));

            return Color.FromArgb(r, g, b);
        }

        #endregion FloydSteinberg



        /// <summary>
        /// 色から近似色のブロックを取得する
        /// </summary>
        /// <param name="color"></param>
        /// <returns></returns>
        private BlockColor RGBToBlockColor(Color color)
        {
            double distance = double.MaxValue;
            var ret = new BlockColor();

            foreach (BlockColor block in blockColors)
            {
                double n = GetApproximate(color, block);

                if (distance >= n)
                {
                    distance = n;
                    ret = block;
                }

            }
            return ret;
        }

        /// <summary>
        /// 色から近似色を取得する
        /// </summary>
        /// <param name="color"></param>
        /// <param name="block"></param>
        /// <returns></returns>
        private double GetApproximate(Color color, BlockColor blockColor)
        {
            double r = color.R - blockColor.R;
            double g = color.G - blockColor.G;
            double b = color.B - blockColor.B;
            double ret = Math.Sqrt(r * r + g * g + b * b);

            if (ret < 0)
            {
                ret = (-ret);
            }
            return ret;
        }
    }
}
