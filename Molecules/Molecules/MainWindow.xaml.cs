using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
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
        private const int CircleDiameter = 50;
        private const int Frequency = 50;
        private const int MaxSpeed = 10;
        private const int MaternitySize = 65;
        private const int AirPumpSize = 65;
        #endregion constants

        private int xMax = CanvasSize - CircleDiameter;
        private int yMax = CanvasSize - CircleDiameter;
        private HashSet<Molecule> molecules = new HashSet<Molecule>();
        private Random random = new Random();

        public MainWindow()
        {
            InitializeComponent();
            Ellipse ellipse = new Ellipse();

            ellipse.Stroke = SystemColors.WindowFrameBrush;
            ellipse.Height = 30;
            ellipse.Width = 50;
            ellipse.Margin = new Thickness(50);
            MyCanvas.Children.Add(ellipse);
        }
    }
}
