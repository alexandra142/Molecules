using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Molecules
{
    public class MoleculeFactory<T> : IFactory<T> where T is Ellipse
    {
        public Ellipse NewInstance()
        {
            Ellipse ellipse = new Ellipse();

            ellipse.Stroke = SystemColors.WindowFrameBrush;
            ellipse.Height = CircleDiameter;
            ellipse.Width = CircleDiameter;
            ellipse.Margin = new Thickness(0);
            ellipse.Fill = new SolidColorBrush(Colors.Blue);

            return ellipse;
        }
    }
}
