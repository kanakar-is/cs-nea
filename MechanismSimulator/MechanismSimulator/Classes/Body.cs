using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace MechanismSimulator.Classes
{
    internal class Body
    {
        protected Canvas SpaceCanvas;

        public Body(Canvas spaceCanvas)
        {
            SpaceCanvas = spaceCanvas;
        }
    }
}
