namespace NextCore.Graphics.Drawings
{
    public class Drawing
    {
        private List<DrawingElement> _drawingElements;

        public void Draw(DrawingElement drawingElement)
        {
            _drawingElements.Add(drawingElement);
        }
    }
}
