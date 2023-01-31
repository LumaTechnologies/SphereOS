using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NextCore.Graphics.Drawings
{
    public abstract class RasterisableDrawingElement : DrawingElement
    {
        public abstract void Rasterise(int[] buffer, int offsetX, int offsetY);
    }
}
