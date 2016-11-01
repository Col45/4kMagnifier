using System;
using System.Drawing;
using System.Windows;
using System.Windows.Forms;
using System.Windows.Interop;
using System.Windows.Media;
using System.Windows.Threading;

namespace _4kMagnifier
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private IntPtr thisHandle;
        private IntPtr AoEHandle;
        private bool displayOn;
        private Timer Timer;
        private Bitmap ScreenBitmap;

        private RegionSplice UIPanelRS { get; set; }
        private RegionSplice UnitGroup1RS { get; set; }
        private RegionSplice UnitGroup2RS { get; set; }
        private RegionSplice MiniMapRS { get; set; }

        public MainWindow()
        {
            InitializeComponent();

            this.AoEHandle = (IntPtr)WindowsServices.FindWindow(null, "Age of Empires II: HD Edition");
            if(this.AoEHandle == (IntPtr)0)
            {
                this.Visibility = Visibility.Hidden;
            }

            System.Windows.Point screenSize = this.TransformToUnits(Screen.PrimaryScreen.Bounds.Width, Screen.PrimaryScreen.Bounds.Height);
            this.ScreenBitmap = new Bitmap(Screen.PrimaryScreen.Bounds.Width, Screen.PrimaryScreen.Bounds.Height);

            Top = screenSize.Y * 0.8379;

            this.InitializeUI();
            
            
            this.displayOn = true;

            Timer = new Timer();
            Timer.Tick += new EventHandler(Timer_Tick);
            Timer.Interval = 200;
            Timer.Start();

            ShowInTaskbar = false;
        }

        private void InitializeUI()
        {
            this.UIPanelRS = new RegionSplice(new System.Windows.Point(0, 1985), 1563, 175, new System.Windows.Point(0, 1810), 2, this.AoEHandle, this, false);
            this.UnitGroup1RS = new RegionSplice(new System.Windows.Point(1563, 2012), 614, 40, new System.Windows.Point(1895, 1946), 2, this.AoEHandle, this, false);
            this.UnitGroup2RS = new RegionSplice(new System.Windows.Point(2178, 2012), 614, 40, new System.Windows.Point(1895, 2030), 2, this.AoEHandle, this, false);
            this.MiniMapRS = new RegionSplice(new System.Windows.Point(3480, 1985), 360, 175, new System.Windows.Point(3120, 1810), 2, this.AoEHandle, this, true);

            SetImages();
        }

        private void Timer_Tick(object sender, EventArgs e)
        {
            SetUIImage();
        }

        protected override void OnSourceInitialized(EventArgs e)
        {
            base.OnSourceInitialized(e);
            thisHandle = new WindowInteropHelper(this).Handle;
            this.SetNoFocus();
        }

        private void SetNoFocus()
        {
            WindowsServices.SetWindowLong(thisHandle, WindowsServices.GWL_EXSTYLE,
            WindowsServices.GetWindowLong(thisHandle, WindowsServices.GWL_EXSTYLE) | WindowsServices.WS_EX_NOACTIVATE);
        }

        private void SetImages()
        {
            var ImageUISize = this.TransformToUnits( this.UIPanelRS.WidthOriginal * this.UIPanelRS.ScaleFactor, this.UIPanelRS.HeightOriginal * this.UIPanelRS.ScaleFactor);
            ImageUI.Width = ImageUISize.X;
            ImageUI.Height = ImageUISize.Y;

            var Units1Size = this.TransformToUnits( this.UnitGroup1RS.WidthOriginal * this.UnitGroup1RS.ScaleFactor, this.UnitGroup1RS.HeightOriginal * this.UnitGroup1RS.ScaleFactor);
            Units1.Width = Units1Size.X;
            Units1.Height = Units1Size.Y;

            var Units2Size = this.TransformToUnits( this.UnitGroup2RS.WidthOriginal * this.UnitGroup2RS.ScaleFactor, this.UnitGroup2RS.HeightOriginal * this.UnitGroup2RS.ScaleFactor);
            Units2.Width = Units2Size.X;
            Units2.Height = Units2Size.Y;

            var MiniMapSize = this.TransformToUnits( this.MiniMapRS.WidthOriginal*this.MiniMapRS.ScaleFactor, this.MiniMapRS.HeightOriginal * this.MiniMapRS.ScaleFactor);
            Minimap.Width = MiniMapSize.X;
            Minimap.Height = MiniMapSize.Y;

            ImageUI.MouseLeftButtonDown += this.UIPanelRS.MouseLeftButtonDown;
            Units1.MouseLeftButtonDown += this.UnitGroup1RS.MouseLeftButtonDown;
            Units2.MouseLeftButtonDown += this.UnitGroup2RS.MouseLeftButtonDown;
            Minimap.MouseLeftButtonDown += this.MiniMapRS.MouseLeftButtonDown;

            ImageUI.MouseMove += this.UIPanelRS.MouseMove;
            Units1.MouseMove += this.UnitGroup1RS.MouseMove;
            Units2.MouseMove += this.UnitGroup2RS.MouseMove;
            Minimap.MouseMove += this.MiniMapRS.MouseMove;

            ImageUI.MouseLeftButtonUp += this.UIPanelRS.MouseLeftButtonUp;
            Units1.MouseLeftButtonUp += this.UnitGroup1RS.MouseLeftButtonUp;
            Units2.MouseLeftButtonUp += this.UnitGroup2RS.MouseLeftButtonUp;
            Minimap.MouseLeftButtonUp += this.MiniMapRS.MouseLeftButtonUp;

            ImageUI.Margin = new Thickness(UIPanelRS.WindowMargin.X, UIPanelRS.WindowMargin.Y, 0, 0);
            Units1.Margin = new Thickness(UnitGroup1RS.WindowMargin.X, UnitGroup1RS.WindowMargin.Y, 0, 0);
            Units2.Margin = new Thickness(UnitGroup2RS.WindowMargin.X, UnitGroup2RS.WindowMargin.Y, 0, 0);
            Minimap.Margin = new Thickness(MiniMapRS.WindowMargin.X, MiniMapRS.WindowMargin.Y, 0, 0);
        }

        private void SetUIImage()
        {
            Bitmap screenCapture = WindowsServices.CaptureWindow(AoEHandle, ScreenBitmap);
            ImageUI.Source = WindowsServices.LoadBitmap(WindowsServices.CropImage(screenCapture, this.UIPanelRS.CropArea));
            Units1.Source = WindowsServices.LoadBitmap(WindowsServices.CropImage(screenCapture, this.UnitGroup1RS.CropArea));
            Units2.Source = WindowsServices.LoadBitmap(WindowsServices.CropImage(screenCapture, this.UnitGroup2RS.CropArea));
            Minimap.Source = WindowsServices.LoadBitmap(WindowsServices.CropImage(screenCapture, this.MiniMapRS.CropArea));
        }

        /// <summary>
        /// Transforms pixels to device independent units (1/96 of an inch)
        /// </summary>
        /// <param name="visual">a visual object</param>
        /// <param name="unitX">a device independent unit value X</param>
        /// <param name="unitY">a device independent unit value Y</param>
        /// <param name="pixelX">returns the X value in pixels</param>
        /// <param name="pixelY">returns the Y value in pixels</param>
        public System.Windows.Point TransformToUnits(double pixelX, double pixelY)
        {
            Matrix matrix;
            var source = PresentationSource.FromVisual(this);
            if (source != null)
            {
                matrix = source.CompositionTarget.TransformFromDevice;
            }
            else
            {
                using (var src = new HwndSource(new HwndSourceParameters()))
                {
                    matrix = src.CompositionTarget.TransformFromDevice;
                }
            }

            return new System.Windows.Point((int)(matrix.M11 * pixelX), (int)(matrix.M22 * pixelY));
        }

        /// <summary>
        /// Transforms device independent units (1/96 of an inch)
        /// to pixels
        /// </summary>
        /// <param name="visual">a visual object</param>
        /// <param name="unitX">a device independent unit value X</param>
        /// <param name="unitY">a device independent unit value Y</param>
        /// <param name="pixelX">returns the X value in pixels</param>
        /// <param name="pixelY">returns the Y value in pixels</param>
        public System.Windows.Point TransformToPixels(double unitX, double unitY)
        {
            Matrix matrix;
            var source = PresentationSource.FromVisual(this);
            if (source != null)
            {
                matrix = source.CompositionTarget.TransformToDevice;
            }
            else
            {
                using (var src = new HwndSource(new HwndSourceParameters()))
                {
                    matrix = src.CompositionTarget.TransformToDevice;
                }
            }

            return new System.Windows.Point((int)(matrix.M11 * unitX), (int)(matrix.M22 * unitY));
        }

        public void ToggleDisplay()
        {
            IntPtr newHandle = (IntPtr)WindowsServices.FindWindow(null, "Age of Empires II: HD Edition");
            if(newHandle == (IntPtr)0)
            {
                this.Visibility = Visibility.Hidden;
                this.Timer.Enabled = false;
                return;
            }
            else if(newHandle != this.AoEHandle)
            {
                this.AoEHandle = newHandle;
                this.InitializeUI();
            }

            if (this.displayOn)
            {
                //this.Hide();
                this.Visibility = Visibility.Hidden;
                this.Timer.Enabled = false;
            }
            else
            {
                //this.Show();
                this.Visibility = Visibility.Visible;
                this.Timer.Enabled = true;
            }


            WindowsServices.SetForegroundWindow(AoEHandle);

            this.displayOn = !displayOn;
        }
        
    }
}