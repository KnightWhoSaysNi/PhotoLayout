using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Input;

namespace PhotoLayout.Controls
{
    // Core code from: http://www.codeproject.com/Articles/168176/Zooming-and-panning-in-WPF-with-fixed-focus
    // TODO Make child confined inside the border, i.e. not let any part of the border be empty in any moment.
    public class ManipulationBorder : Border
    {
        #region - Fields -

        private const double ZoomInValue = 1.2;
        private const double ZoomOutValue = 0.8;

        private UIElement child;
        /// <summary>
        /// Offset of the child element.
        /// </summary>
        private Point origin;
        /// <summary>
        /// Position of the mouse inside the border.
        /// </summary>
        private Point start;

        #endregion

        #region - Constructors -

        public ManipulationBorder() : base()
        {
            InitializeManipulations();
        }

        #endregion

        #region - Properties - 

        public override UIElement Child
        {
            get { return base.Child; }
            set
            {                
                this.child = value;
                base.Child = value;
            }
        }

        #endregion

        #region - Public methods -

        #endregion

        #region - Protected methods -

        protected override Size MeasureOverride(Size constraint)
        {
            return base.MeasureOverride(constraint);
        }

        protected override Size ArrangeOverride(Size finalSize)
        {
            return base.ArrangeOverride(finalSize);
        }

        #endregion

        #region - Private methods - 

        /// <summary>
        /// Initializes manipulations of the child element. Zoom and panning manipulations.
        /// </summary>
        private void InitializeManipulations()
        {
            MouseWheel += ManipulationBorder_MouseWheel;
            MouseLeftButtonDown += ManipulationBorder_MouseLeftButtonDown;
            MouseLeftButtonUp += ManipulationBorder_MouseLeftButtonUp;
            MouseMove += ManipulationBorder_MouseMove;
        }

        /// <summary>
        /// Resolves zooming in or out of the child element -> Scales its in X and Y coordinates.
        /// </summary>
        private void ManipulationBorder_MouseWheel(object sender, MouseWheelEventArgs e)
        {
            if (this.child != null)
            {
                // TODO Create a real solution to the problem of child going outside of the parent border
                if (e.Delta < 0 && this.child.RenderTransform.Value.M11 < 1 && this.child.RenderTransform.Value.M22 < 1)
                {
                    return;
                }

                Point positionInChild = e.GetPosition(this.child);
                Matrix matrix = this.child.RenderTransform.Value;
                if (e.Delta > 0)
                {
                    // Zoom in
                    matrix.ScaleAtPrepend(ZoomInValue, ZoomInValue, positionInChild.X, positionInChild.Y);
                }
                else
                {
                    // Zoom out
                    matrix.ScaleAtPrepend(ZoomOutValue, ZoomOutValue, positionInChild.X, positionInChild.Y);
                }

                this.child.RenderTransform = new MatrixTransform(matrix);
            }
        }

        /// <summary>
        /// Captures positions of mouse inside the border and the offset of the child element.
        /// </summary>        
        private void ManipulationBorder_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            if (this.IsMouseCaptured)
            {
                return;
            }

            if (this.child != null)
            {
                this.child.CaptureMouse();
                this.start = e.GetPosition(this);
                this.origin.X = this.child.RenderTransform.Value.OffsetX;
                this.origin.Y = this.child.RenderTransform.Value.OffsetY;
            }
        }

        /// <summary>
        /// Releases mouse capture of the child.
        /// </summary>        
        private void ManipulationBorder_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            if (this.child != null)
            {
                this.child.ReleaseMouseCapture();
            }
        }

        /// <summary>
        /// Panning/drag manipulation of the child element.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ManipulationBorder_MouseMove(object sender, MouseEventArgs e)
        {
            if (this.child != null)
            {
                if (!this.child.IsMouseCaptured)
                {
                    return;
                }

                Point positionInBorder = e.GetPosition(this);
                Matrix matrix = this.child.RenderTransform.Value;
                matrix.OffsetX = this.origin.X + (positionInBorder.X - this.start.X);
                matrix.OffsetY = this.origin.Y + (positionInBorder.Y - this.start.Y);
                this.child.RenderTransform = new MatrixTransform(matrix);
            }
            
        }
    
        #endregion
    }
}
