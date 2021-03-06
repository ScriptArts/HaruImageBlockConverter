﻿using OrangeNBT.Data.AnvilImproved;
using OrangeNBT.Data.Format;
using OrangeNBT.NBT;
using System;
using System.Drawing;
using System.Windows;
using System.Windows.Media.Imaging;

namespace HaruImageBlockConverter.Convert
{
    internal class ImageConvert
    {
        private readonly BlockColor[] blockColors;
        private ProgressData convertFile;

        // 誤差拡散：Floyd-Steinberg
        private readonly int[] DitherX = { 1, -1, 0, 1 };
        private readonly int[] DitherY = { 0, 1, 1, 1 };
        private readonly double[] DitherErr = { 7d / 16d, 3d / 16d, 5d / 16d, 1d / 16d };

        public ImageConvert(ProgressData convertFile, BlockColor[] blockColors)
        {
            this.convertFile = convertFile;
            this.blockColors = blockColors;
        }


        /// <summary>
        /// BitmapをSchematicに変換
        /// </summary>
        /// <param name="bitmap"></param>
        /// <returns></returns>
        public TagCompound ToSchematic(Bitmap bitmap, bool isDither)
        {
            var schematic = new Schematic(bitmap.Width, 1, bitmap.Height);
            var result = isDither
                ? DitherConvert(bitmap)
                : Convert(bitmap);

            for (var y = 0; y < bitmap.Height; y++)
            {
                for (var x = 0; x < bitmap.Width; x++)
                {
                    var blockColor = result[x, y];

                    try
                    {
                        var block = AnvilImprovedDataProvider.Instance.GetBlock("minecraft:" + blockColor.BlockName);
                        schematic.SetBlock(x, 0, y, block.DefaultBlockSet);
                    }
                    catch (Exception)
                    {
                        MessageBox.Show(blockColor.BlockName);
                    }
                }
                convertFile.Complete++;
            }

            return schematic.BuildTag();
        }


        /// <summary>
        /// BitmapをNBT変換
        /// </summary>
        /// <param name="bitmap"></param>
        /// <returns></returns>
        public TagCompound ToNBT(Bitmap bitmap, bool isDither)
        {
            var structure = new Structure(bitmap.Width, 1, bitmap.Height);
            var result = isDither
                ? DitherConvert(bitmap)
                : Convert(bitmap);

            for (var y = 0; y < bitmap.Height; y++)
            {
                for (var x = 0; x < bitmap.Width; x++)
                {
                    if (bitmap.GetPixel(x,y).A == 0)
                    {
                        continue;
                    }

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
                convertFile.Complete++;
            }

            return structure.BuildTag();
        }

        public TagCompound ToMap(Bitmap bitmap, bool isDither)
        {
            var result = isDither
                ? DitherConvert(bitmap)
                : Convert(bitmap);

            var mapByte = new byte[16384];

            int i = 0;

            for (var y = 0; y < bitmap.Height; y++)
            {
                for (var x = 0; x < bitmap.Width; x++)
                {
                    if (bitmap.GetPixel(x, y).A == 0)
                    {
                        continue;
                    }

                    var blockColor = result[x, y];

                    try
                    {
                        mapByte[i] = byte.Parse(blockColor.BlockName);
                        i++;
                    }
                    catch (Exception)
                    {
                        MessageBox.Show(blockColor.BlockName);
                    }
                }
                convertFile.Complete++;
            }

            var compound = new TagCompound()
            {
                new TagCompound("data")
                {
                    new TagByte("dimension", 0),
                    new TagByte("scale",4),
                    new TagByteArray("colors", mapByte),
                    new TagList("banners"),
                    new TagList("frames"),
                    new TagByte("trackingPosition", 0),
                    new TagByte("unlimitedTracking", 0),
                    new TagInt("xCenter", 2147483647),
                    new TagInt("zCenter", 2147483647)

                }
            };

            return compound;
        }

        /// <summary>
        /// 誤差拡散しつつ減色
        /// </summary>
        /// <param name="bmp"></param>
        /// <returns>変換結果</returns>
        private BlockColor[,] DitherConvert(Bitmap bmp)
        {
            using (var accessor = new BitmapAccessor((Bitmap)bmp.Clone()))
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
                        bmp.SetPixel(x, y, Color.FromArgb(nearestMatch.R, nearestMatch.G, nearestMatch.B));

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
                    convertFile.Complete++;
                }

                return result;
            }
        }

        /// <summary>
        /// 近似色のみで減色
        /// </summary>
        /// <param name="bmp"></param>
        /// <returns>変換結果</returns>
        private BlockColor[,] Convert(Bitmap bmp)
        {
            using (var accessor = new BitmapAccessor((Bitmap)bmp.Clone()))
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

                        var nearestMatch = FindNearest(color.R, color.G, color.B);
                        result[x, y] = nearestMatch;
                        bmp.SetPixel(x, y, Color.FromArgb(nearestMatch.R, nearestMatch.G, nearestMatch.B));
                    }
                    convertFile.Complete++;
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

        /// <summary>
        /// BitmapをBitmapSourceに変換する
        /// </summary>
        /// <param name="source"></param>
        /// <returns></returns>
        public BitmapSource ConvertBitmap(Bitmap source)
        {
            return System.Windows.Interop.Imaging.CreateBitmapSourceFromHBitmap(
                          source.GetHbitmap(),
                          IntPtr.Zero,
                          Int32Rect.Empty,
                          BitmapSizeOptions.FromEmptyOptions());
        }

    }
}
