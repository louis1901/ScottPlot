using ScottPlot;
using ScottPlot.Plottable;
using System;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Threading;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace Demo2
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {

        private SignalPlotXY mScottWpfPlotItem;
        private Tooltip mTooltip;
        private SignalPlotXY mPlot;
        private Crosshair mCrosshair;
        double[] feq;
        double[] pwr;

        public MainWindow()
        {
            InitializeComponent();
            feq = File.ReadAllLines(@"frq.txt").Select(s => Convert.ToDouble(s)).ToArray();
            pwr = File.ReadAllLines(@"pwr.txt").Select(s => Convert.ToDouble(s)).ToArray();
            //ScottWpfPlot.RightClicked -= ScottWpfPlot.DefaultRightClickEvent;
            //ScottWpfPlot.RightClicked += DeployCustomMenu;
            //mCrosshair = ScottWpfPlot.Plot.AddCrosshair(0, 0);
            //mCrosshair.IsVisible = false;
            //mTooltip = ScottWpfPlot.Plot.AddTooltip(label: " ", x: 0, y: 0);
            //mTooltip.IsVisible = false;
        }

        private void ScottWpfPlot_RightClicked(object sender, EventArgs e)
        {
            ContextMenu rightClickMenu = new ContextMenu();

            //MenuItem ShowCrosshairMenuItem = new MenuItem() { Header = "Show Crosshair" };
            //ShowCrosshairMenuItem.Click += ShowCrosshairMenuItem_Click;
            //rightClickMenu.Items.Add(ShowCrosshairMenuItem);

            MenuItem ShowPointMenuItem = new MenuItem() { Header = "Show Point Value" };
            //ShowPointMenuItem.IsCheckable = true;
            ShowPointMenuItem.Click += ShowPointMenuItem_Click;
            rightClickMenu.Items.Add(ShowPointMenuItem);

            MenuItem AutoAxisMenuItem = new MenuItem() { Header = "Zoom to Fit Data" };
            AutoAxisMenuItem.Click += (obj, eventArgs) => { ScottWpfPlot.Plot?.AxisAuto(); ScottWpfPlot.Render(); };
            rightClickMenu.Items.Add(AutoAxisMenuItem);

            rightClickMenu.IsOpen = true;
        }

        private void ShowCrosshairMenuItem_Click(object sender, RoutedEventArgs e)
        {
            if (mCrosshair == null)
            {
                mCrosshair = ScottWpfPlot.Plot.AddCrosshair(0, 0);
                mCrosshair.IsVisible = false;
            }
            mCrosshair.IsVisible = !mCrosshair.IsVisible;
            ScottWpfPlot.Render();
        }

        private void ShowPointMenuItem_Click(object sender, RoutedEventArgs e)
        {
            if (mTooltip == null)
            {
                mTooltip = ScottWpfPlot.Plot.AddTooltip(label: " ", x: 0, y: 0);
                mTooltip.FillColor = System.Drawing.Color.AliceBlue;
                mTooltip.BorderWidth = 1;
                mTooltip.IsVisible = false;
            }
            mTooltip.IsVisible = !mTooltip.IsVisible;
            ScottWpfPlot.Render();
        }

        private void ScottWpfPlot_MouseMove(object sender, System.Windows.Input.MouseEventArgs e)
        {
            if (mCrosshair != null && mCrosshair.IsVisible)
            {
                (double mouseCoordX, double mouseCoordY) = ScottWpfPlot.GetMouseCoordinates();
                double xyRatio = ScottWpfPlot.Plot.XAxis.Dims.PxPerUnit / ScottWpfPlot.Plot.YAxis.Dims.PxPerUnit;
                (double pointX, double pointY, int pointIndex) = mPlot.GetPointNearestX(mouseCoordX);
                mCrosshair.X = pointX;
                mCrosshair.Y = pointY;
                ScottWpfPlot.Render();
            }

            if (mTooltip != null && mTooltip.IsVisible)
            {
                (double mouseCoordX, double mouseCoordY) = ScottWpfPlot.GetMouseCoordinates();
                double xyRatio = ScottWpfPlot.Plot.XAxis.Dims.PxPerUnit / ScottWpfPlot.Plot.YAxis.Dims.PxPerUnit;
                (double pointX, double pointY, int pointIndex) = mPlot.GetPointNearestX(mouseCoordX);

                mTooltip.X = pointX;
                mTooltip.Y = pointY;
                mTooltip.Label = $"Freq:{pointX}\nAmpl:{pointY}";

                ScottWpfPlot.Render();
            }
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            ScottWpfPlot.Configuration.IgnoreMouseDragDistance = 15;
            ScottWpfPlot.Configuration.DoubleClickBenchmark = false;
            ScottWpfPlot.Plot.Title("Freq vs. Ampl");
            ScottWpfPlot.Plot.XLabel("Freq");
            ScottWpfPlot.Plot.YLabel("Ampl");
            ScottWpfPlot.RightClicked -= ScottWpfPlot.DefaultRightClickEvent;
            ScottWpfPlot.RightClicked += ScottWpfPlot_RightClicked;
            ScottWpfPlot.MouseMove += ScottWpfPlot_MouseMove;

            BackgroundWorker worker = new BackgroundWorker();
            worker.DoWork += Worker_DoWork;
            worker.RunWorkerAsync();


            #region MyRegion
            //var plt = new ScottPlot.Plot(600, 400);

            //// generate random, unevenly-spaced data
            //var rand = new Random(2);
            //int pointCount = 100_000;
            //double[] ys = new double[pointCount];
            //double[] xs = new double[pointCount];
            //for (int i = 1; i < ys.Length; i++)
            //{
            //    ys[i] = ys[i - 1] + rand.NextDouble() - .5;
            //    xs[i] = xs[i - 1] + rand.NextDouble();
            //}

            //var sig = plt.AddSignalXY(xs, ys);
            //sig.OffsetX = 10_000;
            //sig.OffsetY = 100;

            //plt.SaveFig("signalxy_offset.png");
            //return;

            //int count = 262144;
            //double[] xs = DataGen.Consecutive(count, 491.52e6 / count, -491.52e6 / 2);
            //double[] values = DataGen.RandomWalk(null, count);
            //var sig = ScottWpfPlot.Plot.AddSignal(values);
            //sig.OffsetX = 100;
            //sig.OffsetY = 100;
            ////sig.XAxisIndex = 300;
            ////Random rand = new Random(0);
            //////for (int i = 0; i < 5; i++)
            //////{
            ////    // add a new signal plot with one million points
            ////    double[] values = DataGen.RandomWalk(rand, 262144);
            ////    ScottWpfPlot.Plot.AddSignal(values);
            //////}

            //ScottWpfPlot.Plot.Benchmark(enable: true);
            //ScottWpfPlot.Plot.SaveFig("signalxy_offset.png");

            ////ScottWpfPlot.Plot.AddSignal(DataGen.Sin(51));
            //ScottWpfPlot.Render(); 
            #endregion
        }

        private void Worker_DoWork(object sender, DoWorkEventArgs e)
        {
            int i = 0;
            while (true)
            {
                //int count = 262144;
                //double[] xsDsOrig = DataGen.Consecutive(count, 491.52e6 / count, -491.52e6 / 2);
                //double[] ysDs = DataGen.RandomWalk(null, count);
                double[] xsDsOrig = feq;
                double[] ysDs = pwr;
                int count = pwr.Length;
                int len = Math.Min(xsDsOrig.Length, ysDs.Length);
                i++;

                ScottWpfPlot.Dispatcher.Invoke(() =>
                {
                    //ScottWpfPlot.Reset();

                    double[] xsDs = xsDsOrig.Select(x => x / 1e6).ToArray();
                    if (mPlot == null)
                    {
                        mPlot = ScottWpfPlot.Plot.AddSignalXY(xsDs, ysDs, color: System.Drawing.Color.Green);
                        //mPlot.SampleRate = 491.52e6;//xScale;
                        mPlot.LineWidth = 0.1;
                    }
                    else
                    {
                        mPlot.Update(ysDs);
                        //mPlot.Xs = xsDs;
                        //mPlot.Ys = ysDs;
                    }

                    ScottWpfPlot.Plot.Clear(typeof(ScatterPlot));
                    ScottWpfPlot.Plot.Clear(typeof(Text));
                    ScottWpfPlot.Plot.AddPoint(xsDs[count / 2], ysDs[count / 2], color: System.Drawing.Color.Red, size: 4);
                    var t1 = ScottWpfPlot.Plot.AddText("Fin", xsDs[count / 2], ysDs[count / 2], 12, System.Drawing.Color.Black);
                    t1.Alignment = Alignment.LowerCenter;

                    //ScottWpfPlot.Plot.Clear(typeof(HLine));
                    //ScottWpfPlot.Plot.AddHorizontalLine(y: -ysDs[count / 3], color: System.Drawing.Color.Red, width: 1, label: "SpurLine", style: LineStyle.Dot);

                    //ScottWpfPlot.Plot.Clear(typeof(VLine));
                    //ScottWpfPlot.Plot.AddVerticalLine(x: xsDs[count / 3], color: System.Drawing.Color.Black, width: 1, label: "vline", style: LineStyle.DashDot);

                    //ScottWpfPlot.Plot.Clear(typeof(Tooltip));
                    ScottWpfPlot.Plot.Legend();


                    ScottWpfPlot.Render();
                });
                Thread.Sleep(2000);
            }

        }

        #region MyRegion
        public void ExecuteRecipe(Plot plt)
        {

            int count = 262144;
            double[] xs = DataGen.Consecutive(count, 491.52 / count, -491.52 / 2);
            double[] values = DataGen.RandomWalk(null, count);

            mCrosshair = ScottWpfPlot.Plot.AddCrosshair(0, 0);
            mCrosshair.IsVisible = false;

            mPlot = plt.AddSignalXY(xs, values);

            mTooltip = ScottWpfPlot.Plot.AddTooltip(label: " ", x: 0, y: 0);
            mTooltip.FillColor = System.Drawing.Color.AliceBlue;
            mTooltip.BorderWidth = 1;
            mTooltip.IsVisible = false;

            plt.AddPoint(25, 0.8, color: System.Drawing.Color.Red, size: 4);
            var t1 = plt.AddText("#1", 25, 0.8, 12, System.Drawing.Color.Red);
            t1.Alignment = Alignment.LowerCenter;


            plt.AddVerticalLine(x: 33, color: System.Drawing.Color.Red, width: 1, style: LineStyle.Dot);

            //var vline = plt.AddVerticalLine(23);
            //vline.LineWidth = 1;
            //vline.PositionLabel = "x=23";
            //vline.PositionLabelBackground = vline.Color;
            //vline.DragEnabled = true;
            //Func<double, string> xFormatter = x => $"X={x:F2}";
            //vline.PositionFormatter = xFormatter;





            //var rand = new Random(0);
            //double[] values = DataGen.RandomWalk(rand, 262144);
            //int sampleRate = 491520000/ 262144;

            //// Signal plots require a data array and a sample rate (points per unit)
            //var sig = plt.AddSignal(values, sampleRate);
            //sig.OffsetX = -245760000;
            //sig.OffsetY = 100;

            plt.Benchmark(enable: true);
            plt.Title($"Signal Plot: One Million Points");
        }

        private void OnMouseMove(object sender, MouseEventArgs e)
        {

            if (mCrosshair == null
             || mPlot == null
             || mTooltip == null)
                return;

            if (mTooltip.IsVisible)
            {
                (double mouseCoordX, double mouseCoordY) = ScottWpfPlot.GetMouseCoordinates();
                double xyRatio = ScottWpfPlot.Plot.XAxis.Dims.PxPerUnit / ScottWpfPlot.Plot.YAxis.Dims.PxPerUnit;
                (double pointX, double pointY, int pointIndex) = mPlot.GetPointNearestX(mouseCoordX);



                //int pixelX = (int)e.MouseDevice.GetPosition(ScottWpfPlot).X;
                //int pixelY = (int)e.MouseDevice.GetPosition(ScottWpfPlot).Y;

                //(double coordinateX, double coordinateY) = ScottWpfPlot.GetMouseCoordinates();

                //XPixelLabel.Content = $"{pixelX:0.000}";
                //YPixelLabel.Content = $"{pixelY:0.000}";

                //XCoordinateLabel.Content = $"{ScottWpfPlot.Plot.GetCoordinateX(pixelX):0.00000000}";
                //YCoordinateLabel.Content = $"{ScottWpfPlot.Plot.GetCoordinateY(pixelY):0.00000000}";

                // mCrosshair.X = pointX;// coordinateX;
                // mCrosshair.Y = pointY;// coordinateY;

                mTooltip.X = pointX;
                mTooltip.Y = pointY;
                mTooltip.Label = $"Freq:{pointX}\nAmpl:{pointY}";

                ScottWpfPlot.Render();
            }

        }

        private void wpfPlot1_MouseEnter(object sender, MouseEventArgs e)
        {
            if (mCrosshair == null)
                return;
            //MouseTrackLabel.Content = "Mouse ENTERED the plot";
            // mCrosshair.IsVisible = true;
        }

        private void wpfPlot1_MouseLeave(object sender, MouseEventArgs e)
        {
            if (mCrosshair == null)
                return;
            //MouseTrackLabel.Content = "Mouse LEFT the plot";
            //XPixelLabel.Content = "--";
            //YPixelLabel.Content = "--";
            //XCoordinateLabel.Content = "--";
            //YCoordinateLabel.Content = "--";

            mCrosshair.IsVisible = false;
            ScottWpfPlot.Render();
        }

        #endregion    
    }

}
