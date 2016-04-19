using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace PhotoLayout.Controls
{
    public class ManipulationBorder : Border
    {
        #region - Fields -

        private UIElement child;
        private Point origin;
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
                if (value != null && value != this.child)
                {
                    InitializeChildTransforms(value);
                }                
                base.Child = value;
            }
        }

        #endregion

        #region - Public methods -

        #endregion

        #region - Private methods -

        private void InitializeChildTransforms(UIElement element)
        {
            this.child = element;

            TransformGroup group = new TransformGroup();
            ScaleTransform scale = new ScaleTransform();
            group.Children.Add(scale);
            TranslateTransform translate = new TranslateTransform();
            group.Children.Add(translate);

            this.child.RenderTransform = group;
            this.child.RenderTransformOrigin = new Point(0, 0);
        }

        private void InitializeManipulations()
        {
            MouseWheel += ManipulationBorder_MouseWheel;
            MouseLeftButtonDown += ManipulationBorder_MouseLeftButtonDown;
            MouseLeftButtonUp += ManipulationBorder_MouseLeftButtonUp;
            MouseMove += ManipulationBorder_MouseMove;
        }

        private void ManipulationBorder_MouseWheel(object sender, System.Windows.Input.MouseWheelEventArgs e)
        {
            if (this.child != null)
            {

            }
        }

        private void ManipulationBorder_MouseLeftButtonDown(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            throw new NotImplementedException();
        }

        private void ManipulationBorder_MouseLeftButtonUp(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            throw new NotImplementedException();
        }

        private void ManipulationBorder_MouseMove(object sender, System.Windows.Input.MouseEventArgs e)
        {
            throw new NotImplementedException();
        }
        #endregion
    }
}
