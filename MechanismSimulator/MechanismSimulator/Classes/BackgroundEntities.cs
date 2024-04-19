using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Converters;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace MechanismSimulator.Classes
{
     internal class BackgroundEntity : Body
        {
            Random rand = new Random();
        public BackgroundEntity(Canvas spaceCanvas, double r, double θ)
                : base(spaceCanvas)
            {
            SpaceCanvas = spaceCanvas;
            for (int i = 0; i < 1000; i++) 
            {
                CreateCometImage(r, θ);
            }
            


        }


        public UIElement Shape { get; set; }

        private void CreateCometImage(double r, double θ)
        {
            
            Ellipse bGent = new Ellipse
            {
                Width = r,
                Height = θ,
                Fill = Brushes.White // You can change color or use gradients here
            };

            // Set the position of the comet
            Canvas.SetLeft(bGent, rand.NextDouble() * 5000);
            Canvas.SetBottom(bGent, rand.NextDouble() * 5000);

            // Add the comet to the canvas
            bGent.Tag = "bgent";
            SpaceCanvas.Children.Add(bGent);
        }
    }
}
