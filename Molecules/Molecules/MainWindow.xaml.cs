using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Windows;
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
        private const int MaxSpeed = 30;
        private const int MaternitySize = 75;
        private const int AirPumpSize = 75;
        #endregion constants

        private int xMax = CanvasSize - CircleDiameter;
        private int yMax = CanvasSize - CircleDiameter;
        private readonly HashSet<Ellipse> molecules = new HashSet<Ellipse>();
        private readonly Random _random = new Random();
        private readonly BackgroundWorker _backgroundWorker;
        public MainWindow()
        {
            InitializeComponent();
            Maternity.Width = MaternitySize;
            Maternity.Height = MaternitySize;
            AirPump.Width = AirPumpSize;
            AirPump.Height = AirPumpSize;

            _backgroundWorker = new BackgroundWorker();

            _backgroundWorker.DoWork += (s, ev) =>
            {
                Dispatcher.Invoke(DrawNewEllipse);
                Dispatcher.Invoke(Move);
            };


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
            var newValue = oldValue + _random.Next(MaxSpeed) - _random.Next(MaxSpeed / 2);
            while (newValue > xMax)
                newValue = oldValue + _random.Next(MaxSpeed) - _random.Next(MaxSpeed / 2);

            return newValue;
        }

        private void DrawNewEllipse()
        {
            if (molecules.Count >= 20) return;

            Ellipse ellipse = new Ellipse();

            ellipse.Stroke = SystemColors.WindowFrameBrush;
            ellipse.Height = CircleDiameter;
            ellipse.Width = CircleDiameter;
            ellipse.Margin = new Thickness(CircleDiameter);
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
