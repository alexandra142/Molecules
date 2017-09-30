using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Windows;
using System.Windows.Media;
using System.Windows.Shapes;

namespace Molecules
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        #region constants
        private const int CanvasSize = 500;
        private const int CircleDiameter = 60;
        private const int Frequency = 500;
        private const int MaxSpeed = 100;
        private const int MaternitySize = 75;
        private const int AirPumpSize = 75;
        private const int WindowCanvasDiff = 50;
        #endregion constants

        private int xMax = CanvasSize - AirPumpSize- CircleDiameter/2;
        private readonly HashSet<Ellipse> molecules = new HashSet<Ellipse>();
        private readonly Random _random = new Random();
        private readonly BackgroundWorker _backgroundWorker;
        public MainWindow()
        {
            InitializeComponent();
            MyCanvas.Width = CanvasSize;
            MyCanvas.Height = CanvasSize;
            Maternity.Width = MaternitySize;
            Maternity.Height = MaternitySize;
            AirPump.Width = AirPumpSize;
            AirPump.Height = AirPumpSize;

            _backgroundWorker = new BackgroundWorker();

            _backgroundWorker.DoWork += (s, ev) =>
            {
                Dispatcher.Invoke(DrawNewEllipse);
                Dispatcher.Invoke(Move);
                Dispatcher.Invoke(Delete);
            };
        }

        private void Delete()
        {
            foreach (var molecule in molecules)
            {
                if(molecule.Margin.Left > xMax && molecule.Margin.Top > xMax)
                    MyCanvas.Children.Remove(molecule);
            }
        }

        private void MainWindow_OnLoaded(object sender, RoutedEventArgs e)
        {

            System.Windows.Threading.DispatcherTimer dispatcherTimer = new System.Windows.Threading.DispatcherTimer();
            dispatcherTimer.Tick += DispatcherTimer_Tick;
            dispatcherTimer.Interval = new TimeSpan(0, 0, 0, 0, Frequency);
            dispatcherTimer.Start();
        }

        private void Move()
        {
            foreach (var molecule in molecules)
            {

                Thickness newMargin = new Thickness();
                newMargin.Left = GetNewValue(molecule.Margin.Left);
                newMargin.Right = GetNewValue(molecule.Margin.Right);
                newMargin.Top = GetNewValue(molecule.Margin.Top);
                newMargin.Bottom = GetNewValue(molecule.Margin.Bottom);
                molecule.Margin = newMargin;
            }

        }

        private double GetNewValue(double oldValue)
        {
            int speed = MaxSpeed/2;
            if(oldValue >= CanvasSize/3 && oldValue <= CanvasSize*2/3)
            speed = MaxSpeed;
            if (oldValue <= CanvasSize / 4 || oldValue >= CanvasSize * 3 / 4)
                speed = MaxSpeed/10;
            if (oldValue > xMax)
                return oldValue - _random.Next(speed);

            return oldValue + _random.Next(MaxSpeed) - _random.Next(speed / 2);
        }

        private void DrawNewEllipse()
        {
            if (molecules.Count >= 2) return;

            Ellipse ellipse = new Ellipse();

            ellipse.Stroke = SystemColors.WindowFrameBrush;
            ellipse.Height = CircleDiameter;
            ellipse.Width = CircleDiameter;
            ellipse.Margin = new Thickness(0);
            ellipse.Fill = new SolidColorBrush(Colors.Blue);
            MyCanvas.Children.Add(ellipse);
            molecules.Add(ellipse);
        }

        private void DispatcherTimer_Tick(object sender, EventArgs e)
        {
            if (!_backgroundWorker.IsBusy)
                _backgroundWorker.RunWorkerAsync();
        }
    }
}
