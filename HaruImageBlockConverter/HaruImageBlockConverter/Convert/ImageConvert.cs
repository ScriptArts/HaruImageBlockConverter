using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using OrangeNBT;
using OrangeNBT.Data.AnvilImproved;
using OrangeNBT.Data.Format;
using OrangeNBT.NBT;

namespace HaruImageBlockConverter.Convert
{
    internal class ImageConvert
    {
        private readonly BlockColor[] blockColors;

        // 誤差拡散：Floyd-Steinberg
        private readonly int[] DitherX = { 1, -1, 0, 1 };
        private readonly int[] DitherY = { 0, 1, 1, 1 };
        private readonly double[] DitherErr = { 7d / 16d, 3d / 16d, 5d / 16d, 1d / 16d };

        public ImageConvert(BlockColor[] blockColors)
        {
            this.blockColors = blockColors;
        }


        /// <summary>
        /// BitmapをSchematicに変換
        /// </summary>
        /// <param name="bitmap"></param>
        /// <returns></returns>
        public TagCompound ToSchematic(Bitmap bitmap)
        {
            var schematic = new Schematic(bitmap.Width, 1, bitmap.Height);
            var result = Convert(bitmap);

            for (var x = 0; x < bitmap.Width; x++)
            {
                for (var y = 0; y < bitmap.Height; y++)
                {
                    var blockColor = result[x, y];

                    try
                    {
                        var block = AnvilImprovedDataProvider.Instance.GetBlock("minecraft:" + blockColor.BlockName);
                        schematic.SetBlock(x, 0, y, block.DefaultBlockSet);
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show(blockColor.BlockName);
                    }
                }
            }

            return schematic.BuildTag();
        }


        /// <summary>
        /// BitmapをNBT変換
        /// </summary>
        /// <param name="bitmap"></param>
        /// <returns></returns>
        public TagCompound ToNBT(Bitmap bitmap)
        {
            var structure = new Structure(bitmap.Width, 1, bitmap.Height);
            var result = Convert(bitmap);

            for (var x = 0; x < bitmap.Width; x++)
            {
                for (var y = 0; y < bitmap.Height; y++)
                {
                    var blockColor = result[x, y];

                    try
                    {
                        var block = AnvilImprovedDataProvider.Instance.GetBlock("minecraft:" + blockColor.BlockName);
                        structure.SetBlock(x, 0, y, block.DefaultBlockSet);
                    }
                    catch (Exception)
                    {
                        MessageBox.Show(blockColor.BlockName);
                    }

                }
            }

            return structure.BuildTag();
        }

        /// <summary>
        /// 誤差拡散しつつ減色
        /// </summary>
        /// <param name="bmp"></param>
        /// <returns>変換結果</returns>
        private BlockColor[,] Convert(Bitmap bmp)
        {
            using (var accessor = new BitmapAccessor(bmp))
            {
                var w = bmp.Width;
                var h = bmp.Height;

                var result = new BlockColor[w, h];

                int[][] ditherData = { new int[w * 3], new int[w * 3] };

                for (var y = 0; y < h; y++)
                {
                    // Recycle
                    var shift = ditherData[0];
                    Array.Copy(ditherData, 1, ditherData, 0, ditherData.Length - 1);
                    for (var i = 0; i < shift.Length; i++)
                    {
                        shift[i] = 0;
                    }
                    ditherData[ditherData.Length - 1] = shift;

                    for (var x = 0; x < w; x++)
                    {
                        var color = accessor.GetPixel(x, y);

                        var r = Math.Max(0, Math.Min(255, color.R + ditherData[0][x * 3 + 0]));
                        var g = Math.Max(0, Math.Min(255, color.G + ditherData[0][x * 3 + 1]));
                        var b = Math.Max(0, Math.Min(255, color.B + ditherData[0][x * 3 + 2]));

                        var nearestMatch = FindNearest(r, g, b);
                        result[x, y] = nearestMatch;

                        var rDiff = r - nearestMatch.R;
                        var gDiff = g - nearestMatch.G;
                        var bDiff = b - nearestMatch.B;

                        for (var k = 0; k < DitherX.Length; k++)
                        {
                            var tmpX = DitherX[k] + x;
                            var tmpY = DitherY[k];

                            if ((tmpY < h) && (tmpX < w) && (tmpX > 0))
                            {
                                ditherData[tmpY][tmpX * 3 + 0] += (int)Math.Floor(DitherErr[k] * rDiff);
                                ditherData[tmpY][tmpX * 3 + 1] += (int)Math.Floor(DitherErr[k] * gDiff);
                                ditherData[tmpY][tmpX * 3 + 2] += (int)Math.Floor(DitherErr[k] * bDiff);
                            }
                        }
                    }
                }

                return result;
            }
        }

        /// <summary>
        /// 最も近い色を持つ BlockColor を返す
        /// </summary>
        /// <param name="r">R</param>
        /// <param name="g">G</param>
        /// <param name="b">B</param>
        /// <returns>BlockColor</returns>
        private BlockColor FindNearest(int r, int g, int b)
        {
            var bestDiff = double.MaxValue;
            var bestMatch = new BlockColor();

            double diff;
            foreach (var block in blockColors)
            {
                var rDiff = block.R - r;
                var gDiff = block.G - g;
                var bDiff = block.B - b;

                if ((diff = Math.Sqrt(rDiff * rDiff + gDiff * gDiff + bDiff * bDiff)) < bestDiff)
                {
                    bestDiff = diff;
                    bestMatch = block;
                }
            }

            return bestMatch;
        }
    }
}
