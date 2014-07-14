using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Documents;

namespace PresentationCombiner.Behavior
{
    class ListViewDragAdornerManager
    {
        private AdornerLayer adornerLayer;
        private ListViewDragAdorner adorner;

        private bool shouldCreateNewAdorner = false;

        public ListViewDragAdornerManager(AdornerLayer adornerLayer)
        {
            this.adornerLayer = adornerLayer;
        }

        internal void Update(UIElement adornedElement, bool isAboveElement)
        {
            if (adorner != null && !shouldCreateNewAdorner)
            {
                // Don't need to create an adorner, already sorted.
                return;
            }
            else
            {
                this.Clear();
                this.adorner = new ListViewDragAdorner(adornedElement, this.adornerLayer);
                this.adorner.IsAboveElement = isAboveElement;
                this.adorner.Update();
                this.shouldCreateNewAdorner = false;
            }
        }

        internal void Clear()
        {
            if (this.adorner != null)
            {
                this.adorner.Remove();
                this.shouldCreateNewAdorner = true;
            }
        }
    }
}
