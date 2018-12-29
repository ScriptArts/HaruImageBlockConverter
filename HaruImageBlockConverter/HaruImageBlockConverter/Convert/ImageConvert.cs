using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OrangeNBT;
using OrangeNBT.Data.Format;
using OrangeNBT.NBT;

namespace HaruImageBlockConverter.Convert
{
    internal class ImageConvert
    {
        public TagCompound ToSchematic(Bitmap bitmap)
        {
            var schematic = new Schematic(1,2,3);

            return schematic.BuildTag();
        }

        public TagCompound ToNBT(Bitmap bitmap)
        {
            var schematic = new Structure(1, 2, 3);

            return schematic.BuildTag();
        }
    }
}
