using System.Windows;
using System.Windows.Documents;
using System.Windows.Media;

namespace PresentationCombiner.Behavior
{
    class ListViewDragAdorner : Adorner
    {
        private AdornerLayer adornerLayer;

        public bool IsAboveElement { get; set; }

        public ListViewDragAdorner(UIElement adornedElement, AdornerLayer adornerLayer)
            : base(adornedElement)
        {
            this.adornerLayer = adornerLayer;
            this.adornerLayer.Add(this);
        }

        internal void Update()
        {
            this.adornerLayer.Update(this.AdornedElement);
            this.Visibility = Visibility.Visible;
        }

        public void Remove()
        {
            this.Visibility = Visibility.Collapsed;
        }

        protected override void OnRender(System.Windows.Media.DrawingContext drawingContext)
        {
            var size = this.AdornedElement.DesiredSize;

            double width = size.Width;
            double height = size.Height;
            double renderRadius = 5.0;

            Rect adornedElementRect = new Rect(size);

            var renderBrush = new SolidColorBrush(Colors.Red);
            renderBrush.Opacity = 0.5;
            Pen renderPen = new Pen(new SolidColorBrush(Colors.White), 1.5);

            if(this.IsAboveElement)
            {
                drawingContext.DrawEllipse(renderBrush, renderPen, adornedElementRect.TopLeft,  renderRadius, renderRadius);
                drawingContext.DrawEllipse(renderBrush, renderPen, adornedElementRect.TopRight, renderRadius, renderRadius);
            }
            else
            {
                drawingContext.DrawEllipse(renderBrush, renderPen, adornedElementRect.BottomLeft,  renderRadius, renderRadius);
                drawingContext.DrawEllipse(renderBrush, renderPen, adornedElementRect.BottomRight, renderRadius, renderRadius);
            }
        }
    }
}
