using System;
using System.Windows;
using System.Windows.Shapes;
using System.Windows.Threading;

namespace Molecules
{
    class MoleculeFactory : IFactory<Ellipse>
    {
        private const int CircleDiameter = 50;
        private object _syncObj = new object();
        public Ellipse NewInstance(Dispatcher dispatcher)
        {
            dispatcher.Invoke((Action) (() =>
            {
                Ellipse ellipse = new Ellipse();

                ellipse.Stroke = SystemColors.WindowFrameBrush;
                ellipse.Height = CircleDiameter;
                ellipse.Width = CircleDiameter;
                ellipse.StrokeThickness = 5;

            }));
            return ellipse;

        }
    }
}
