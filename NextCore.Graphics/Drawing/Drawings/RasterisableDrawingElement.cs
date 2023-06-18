namespace NextCore.Graphics.Drawings
{
    public abstract class RasterisableDrawingElement : DrawingElement
    {
        public abstract void Rasterise(int[] buffer, int offsetX, int offsetY);
    }
}
