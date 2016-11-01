using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Drawing;

namespace _4kMagnifier
{
    class RegionSplice
    {
        public System.Windows.Point TopLeftOriginal { get; set; }
        public System.Windows.Point TopLeftFinal { get; set; }
        public int HeightOriginal { get; set; }
        public int WidthOriginal { get; set; }
        public int ScaleFactor { get; set; }
        public Rectangle CropArea { get; set; }
        public MainWindow MainWin { get; set; }

        public System.Windows.Point WindowMargin { get; set; }
        public bool MouseDragEnabled;

        private IntPtr AoEHandle;

        /// <summary>
        /// Creates a new instance of RegionSplice. Using Pixels in screen space (3840 x 2160);
        /// </summary>
        /// <param name="topLeftOriginal"></param>
        /// <param name="widthOriginal"></param>
        /// <param name="heightOriginal"></param>
        /// <param name="topLeftFinal"></param>
        /// <param name="scaleFactor"></param>
        public RegionSplice(System.Windows.Point topLeftOriginal, int widthOriginal, int heightOriginal, System.Windows.Point topLeftFinal, int scaleFactor, IntPtr aoeHandle, MainWindow window, bool mouseDragEnabled)
        {
            this.TopLeftOriginal = topLeftOriginal;
            this.TopLeftFinal = topLeftFinal;
            this.WidthOriginal = widthOriginal;
            this.HeightOriginal = heightOriginal;
            this.ScaleFactor = scaleFactor;
            this.AoEHandle = aoeHandle;
            this.MainWin = window;
            this.MouseDragEnabled = mouseDragEnabled;

            this.CropArea = new Rectangle((int)this.TopLeftOriginal.X, (int)this.TopLeftOriginal.Y, this.WidthOriginal, this.HeightOriginal);

            var MainWinMargin = MainWin.TransformToPixels(MainWin.Top, MainWin.Left);
            var marginTemp = new System.Windows.Point(TopLeftFinal.X - MainWinMargin.Y, TopLeftFinal.Y - MainWinMargin.X);
            this.WindowMargin = MainWin.TransformToUnits(marginTemp.X, marginTemp.Y);
        }

        private bool MouseDown = false;

        public void MouseLeftButtonDown(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            MouseDown = true;
            System.Windows.Controls.Image image = sender as System.Windows.Controls.Image;
            var location = MainWin.TransformToPixels(e.GetPosition(image).X, e.GetPosition(image).Y);

            ClickOnPointTool.ClickOnPoint(AoEHandle, this.CalculatePointToClick(location.X, location.Y));
        }

        public void MouseMove(object sender, System.Windows.Input.MouseEventArgs e)
        {
            if (MouseDown && this.MouseDragEnabled)
            {
                System.Windows.Controls.Image image = sender as System.Windows.Controls.Image;
                var location = MainWin.TransformToPixels(e.GetPosition(image).X, e.GetPosition(image).Y);

                ClickOnPointTool.ClickOnPoint(AoEHandle, this.CalculatePointToClick(location.X, location.Y));
            }
        }

        public void MouseLeftButtonUp(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            MouseDown = false;
        }

        /// <summary>
        /// Converts enhanced image coordinates to original screen coordinates
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <returns></returns>
        private System.Drawing.Point CalculatePointToClick(double x, double y)
        {
            var scaledX = x / this.ScaleFactor;
            var scaledY = y / this.ScaleFactor;

            return new System.Drawing.Point(Convert.ToInt16(this.TopLeftOriginal.X) + Convert.ToInt16(scaledX), Convert.ToInt16(this.TopLeftOriginal.Y) + Convert.ToInt16(scaledY));
        }

    }
}
