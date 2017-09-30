using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
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
        private const int Frequency = 100;
        private const int MaxSpeed = 100;
        private const int MaternitySize = 75;
        private const int AirPumpSize = 75;
        private const int capacity = 2;
        #endregion constants

        private int xMax = CanvasSize - AirPumpSize - CircleDiameter / 2;
        private readonly HashSet<Ellipse> _molecules = new HashSet<Ellipse>();
        private readonly Random _random = new Random();
        private readonly BackgroundWorker _backgroundWorker;
        private readonly Pool<Ellipse> _pool;
        private Task _drawNew;
        private Task _moveTask;
        private Task _deleteTask;

        public MainWindow()
        {
            InitializeComponent();

            _pool = new Pool<Ellipse>(new MoleculeFactory(Dispatcher), capacity);

            _backgroundWorker = new BackgroundWorker();

            StartDrawTask();
            StartMoveTask();
            StartDeleteTask();

            _backgroundWorker.DoWork += (s, ev) =>
            {
                Dispatcher.Invoke(StartWork);
            };
        }

        private void StartWork()
        {
            if (_drawNew.IsCompleted)
                StartDrawTask();

            if (_moveTask.IsCompleted)
                StartMoveTask();

            if (_deleteTask.IsCompleted)
                StartDeleteTask();
        }

        private void StartDrawTask()
        {
            _drawNew = new Task(DrawNewEllipse);
            _drawNew.Start();
        }

        private void StartMoveTask()
        {
            _moveTask = new Task(Move);
            _moveTask.Start();
        }
        private void StartDeleteTask()
        {
            _deleteTask = new Task(Delete);
            _deleteTask.Start();
        }

        private void Delete()
        {
            Dispatcher.Invoke(() =>
            {
                List<Ellipse> moleculesToDelete = new List<Ellipse>();

                foreach (var molecule in _molecules)
                {
                    if (molecule.Margin.Left < xMax || molecule.Margin.Top < xMax) continue;

                    MyCanvas.Children.Remove(molecule);
                    _pool.ReturnInstance(molecule);
                    moleculesToDelete.Add(molecule);
                }

                for (int i = 0; i < moleculesToDelete.Count; i++)
                {
                    _molecules.Remove(moleculesToDelete[i]);
                }
            });
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
            Dispatcher.Invoke(() =>
            {
                foreach (var molecule in _molecules)
                {
                    Thickness newMargin = new Thickness();
                    newMargin.Left = GetNewValue(molecule.Margin.Left);
                    newMargin.Right = GetNewValue(molecule.Margin.Right);
                    newMargin.Top = GetNewValue(molecule.Margin.Top);
                    newMargin.Bottom = GetNewValue(molecule.Margin.Bottom);
                    molecule.Margin = newMargin;
                }
            });
        }

        private double GetNewValue(double oldValue)
        {
            int speed = MaxSpeed / 2;
            if (oldValue >= CanvasSize / 3 && oldValue <= CanvasSize * 2 / 3)
                speed = MaxSpeed;
            if (oldValue <= CanvasSize / 4 || oldValue >= CanvasSize * 3 / 4)
                speed = MaxSpeed / 10;
            if (oldValue > xMax)
                return oldValue - _random.Next(speed);

            return oldValue + _random.Next(MaxSpeed) - _random.Next(speed / 2);
        }

        private void DrawNewEllipse()
        {
            Ellipse ellipse = _pool.GiveWait();

            if (ellipse == default(Ellipse))
                return;

            Dispatcher.Invoke(() =>
            {
                _molecules.Add(ellipse);
                MyCanvas.Children.Add(ellipse);
            });
        }

        private void DispatcherTimer_Tick(object sender, EventArgs e)
        {
            if (!_backgroundWorker.IsBusy)
                _backgroundWorker.RunWorkerAsync();
        }
    }

}
