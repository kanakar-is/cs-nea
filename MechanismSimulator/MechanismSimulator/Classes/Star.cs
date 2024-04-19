using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
 
namespace MechanismSimulator.Classes
{
        internal class Star : Body
        {
            protected int r;
            public Star(Canvas spaceCanvas, int R) : base(spaceCanvas)
            {
                r = R;
            }

        public void GenerateStar(string starName, int r, int temperatureLevel, int systype)
        {
            Random rand = new Random();



            if (systype == 0)
            {
                // For unary system, draw a single star in the center
                DrawStar((SpaceCanvas.ActualWidth - r) / 2 + 20, (SpaceCanvas.ActualHeight - r) / 2 + 20, r, temperatureLevel, rand);
            }
            else if (systype == 1)
            {
                // For binary system, draw two stars next to each other
                int spacing = 20; // Adjust the spacing between stars as needed
                int totalWidth = 2 * spacing + 2 * r;

                DrawStar((SpaceCanvas.ActualWidth - totalWidth) / 2, (SpaceCanvas.ActualHeight - r) / 2, r, temperatureLevel, rand);
                DrawStar((SpaceCanvas.ActualWidth + totalWidth) / 2 - r, (SpaceCanvas.ActualHeight - r) / 2, r, temperatureLevel, rand);
            }
            else if (systype == 2)
            {
                // For trinary system, draw three stars forming a triangle
                double centerX = SpaceCanvas.ActualWidth / 2;
                double centerY = SpaceCanvas.ActualHeight / 2;
                double Ang = rand.NextDouble() * 2 * Math.PI; // Initial random angle

                for (int i = 0; i < 3; i++)
                {
                    double x = centerX + r * Math.Cos(Ang + i * 2 * Math.PI / 3);
                    double y = centerY + r * Math.Sin(Ang + i * 2 * Math.PI / 3);
                    DrawStar(x, y, r, temperatureLevel, rand);
                }
            }
        }

        private void DrawStar(double x, double y, int r, int temperatureLevel, Random rand)
        {
            Ellipse starCircle = new Ellipse();
            starCircle.Width = r * 2;
            starCircle.Height = r * 2;
            starCircle.StrokeThickness = 2;

            // Determine star color based on temperature level
            Color starColor = new Color();
            if (temperatureLevel == 0)
            {
                starColor = Color.FromRgb((byte)rand.Next(105, 200), 0, 0); // Red for lowest temperature
            }
            else if (temperatureLevel == 1)
            {
                starColor = Color.FromRgb((byte)rand.Next(210, 256), (byte)rand.Next(106, 156), 0); // Orange for medium temperature
            }
            else
            {
                starColor = Color.FromRgb((byte)rand.Next(200, 229), (byte)rand.Next(240, 244), (byte)rand.Next(250, 256)); // Blue for highest temperature
            }
            starCircle.Fill = new SolidColorBrush(starColor);

            // Position the star circle in the canvas
            Canvas.SetLeft(starCircle, x - r);
            Canvas.SetTop(starCircle, y - r);

            // Add the star circle to the canvas
            SpaceCanvas.Children.Add(starCircle);
        }
        public JObject GenerateStarAttributesToJson(string starName, int fieldstrength, int r, int temperatureLevel, int systype)
        {
            JObject starJson = new JObject();
            starJson["type"] = "Star";
            starJson["name"] = starName;
            starJson["fieldstrength"] = fieldstrength;
            starJson["radius"] = r;
            starJson["temperature"] = temperatureLevel;
            starJson["systype"] = systype;

            return starJson;
        }
    }
}
