using System;
using System.Windows;
using System.Windows.Media;
using System.Windows.Shapes;
using System.Windows.Threading;

namespace Molecules
{
    public class MoleculeFactory : IFactory<Ellipse>
    {
        private const int CircleDiameter = 60;
        private readonly Dispatcher _dispetcher;
        private readonly Thickness _startMargin = new Thickness(0);
        public MoleculeFactory(Dispatcher dispetcher)
        {
            this._dispetcher = dispetcher;
        }

        public Ellipse NewInstance()
        {
            return _dispetcher.Invoke(() =>
            {
                Ellipse ellipse = new Ellipse();

                ellipse.Stroke = SystemColors.WindowFrameBrush;
                ellipse.Height = CircleDiameter;
                ellipse.Width = CircleDiameter;
                ellipse.Margin = _startMargin;
                ellipse.Fill = new SolidColorBrush(Colors.Blue);

                return ellipse;
            });
        }

        public void Reset(Ellipse instance)
        {
            instance.Margin = _startMargin;
        }
    }
}
