using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows.Ink;
using System.Windows.Media;
using System.Windows.Shapes;

namespace MechanismSimulator.Classes
{
    internal class GaseousPlanet : World
    {
        public GaseousPlanet(Canvas spaceCanvas, int R) : base(spaceCanvas, R)
        {
            r = R;
        }
        public void GenerateGaseousPlanet(string planetName, int temperatureLevel, int r, int moonNumber, double planetarydist)
        {
            Random rand = new Random();
            
            // Generate the base color based on temperature level
            SolidColorBrush baseColor = new SolidColorBrush();
            if (temperatureLevel == 0)
            {
                baseColor.Color = Color.FromRgb((byte)rand.Next(200, 256), (byte)rand.Next(230, 256), (byte)rand.Next(240, 256)); // Very light baby blue ice color
            }
            else if (temperatureLevel == 1)
            {
                baseColor.Color = Color.FromRgb((byte)rand.Next(160, 180), (byte)rand.Next(150, 160), (byte)rand.Next(120, 150)); // Normal gas giant base color
            }
            else if (temperatureLevel == 2)
            {
                baseColor.Color = Color.FromRgb((byte)rand.Next(100, 256), 0, 0); // Red/dark red base color
            }
            circle.Tag = "gaseousplanet";
            // Create the base ellipse
            GenerateWorld(SpaceCanvas, circle, r, moonNumber);
            Canvas.SetZIndex(SpaceCanvas, 0);
            Canvas stripeCanvas = new Canvas();
            stripeCanvas.Width = circle.Width;
            stripeCanvas.Height = circle.Height;
            circle.Fill = baseColor;
            Canvas.SetLeft(circle, ((SpaceCanvas.ActualWidth - circle.Width) / 2) + planetarydist);
            Canvas.SetTop(circle, ((SpaceCanvas.ActualHeight - circle.Height) / 2) + planetarydist);
            // Create stripes
            int numStripes = rand.Next(5, 10); // Random number of stripes
            for (int i = 0; i < numStripes; i++)
            {
                SolidColorBrush stripeColor = new SolidColorBrush();
                stripeColor.Color = Color.FromRgb((byte)(baseColor.Color.R + (byte)rand.Next(-20, 20)), (byte)(baseColor.Color.G + (byte)rand.Next(-20, 20)), (byte)(baseColor.Color.B + (byte)rand.Next(-20, 20))); // Random stripe color

                Rectangle stripe = new Rectangle();
                Canvas.SetZIndex(stripe, 4);
                stripe.Width = 200;
                stripe.Height = 50 / numStripes; // Equal height for each stripe
                stripe.Fill = stripeColor;

                
                Canvas.SetTop(stripe, i * (r * 2 / numStripes));
                Canvas.SetLeft(stripe, 0);
                stripeCanvas.Children.Add(stripe);
                stripe.Tag = "gaseousplanet";
            }
            GenerateMoons(SpaceCanvas, circle, moonNumber);
        }
        public JObject GenerateGaseousPlanetAttributesToJson(string planetName, int temperatureLevel, int r, int moonNumber, double planetaryDist)
        {
            JObject gaseousPlanetJson = new JObject();
            gaseousPlanetJson["type"] = "GaseousPlanet";
            gaseousPlanetJson["name"] = planetName;
            gaseousPlanetJson["radius"] = r;
            gaseousPlanetJson["moonnumber"] = moonNumber;
            gaseousPlanetJson["temperaturelevel"] = temperatureLevel;
            gaseousPlanetJson["distancefromstar"] = planetaryDist;

            return gaseousPlanetJson;
        }
    }
}
