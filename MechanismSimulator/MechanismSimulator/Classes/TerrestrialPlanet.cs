using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace MechanismSimulator.Classes
{
    internal class TerrestrialPlanet : World
    {
        public TerrestrialPlanet(Canvas spaceCanvas, int R) : base(spaceCanvas, R)
        {
            r = R;
        }
        public void GenerateTerrestrialPlanet(string planetName, int lifeLevel, int r, int waterLevel, int temperatureLevel, bool LoraxMode, int moonNumber, double planetarydist)
        {
            
            
            Random rand = new Random();
            GenerateWorld(SpaceCanvas, circle, r, moonNumber);
            Canvas spotCanvas = new Canvas();
            spotCanvas.Width = circle.Width;
            spotCanvas.Height = circle.Height;
            int grassNum = (int)(circle.Width * circle.Height * Math.Pow(lifeLevel, 2) / 5000); // Adjust factor for distribution
            for (int i = 0; i < grassNum; i++)
            {
                Ellipse grassSpot = new Ellipse();
                grassSpot.Width = rand.Next(15, 25); // Adjust spot size range
                grassSpot.Height = grassSpot.Width;
                if (LoraxMode == true)
                {
                    int colorRand = rand.Next(1, 4);
                    if (colorRand == 1)
                    {
                        grassSpot.Fill = new SolidColorBrush(Color.FromRgb(152, 251, 152));

                    }
                    else if (colorRand == 2)
                    {
                        grassSpot.Fill = new SolidColorBrush(Color.FromRgb(255, 182, 193));

                    }
                    else if (colorRand == 3)
                    {
                        grassSpot.Fill = new SolidColorBrush(Color.FromRgb(255, 255, 102));

                    }
                    else
                    {
                        grassSpot.Fill = new SolidColorBrush(Color.FromRgb(221, 160, 221));

                    }
                }
                else if (temperatureLevel == 0)
                    grassSpot.Fill = new SolidColorBrush(Color.FromRgb(0, (byte)rand.Next(30, 50), 0)); // Darker green for colder temperatures
                else if (temperatureLevel == 1) 
                {
                    grassSpot.Fill = new SolidColorBrush(Color.FromRgb(0, (byte)rand.Next(50, 70), 0));
                }
                else
                    grassSpot.Fill = new SolidColorBrush(Color.FromRgb((byte)rand.Next(30, 45), (byte)rand.Next(25, 30), (byte)rand.Next(10, 20))); // Regular green

                double grassSpotX = rand.NextDouble() * circle.Width;
                double grassSpotY = rand.NextDouble() * circle.Height;

                Canvas.SetLeft(grassSpot, grassSpotX);
                Canvas.SetTop(grassSpot, grassSpotY);

                spotCanvas.Children.Add(grassSpot);
                grassSpot.Tag = "terrestrialplanet";
            }
            // Randomly generate water
            int waterNum = (int)(circle.Width * circle.Height * Math.Pow(waterLevel, 2) / 5000); // Adjust factor for distribution
            for (int i = 0; i < waterNum; i++)
            {
                Ellipse waterSpot = new Ellipse();
                waterSpot.Width = rand.Next(1, 25); // Adjust spot size range
                waterSpot.Height = waterSpot.Width;
                if (temperatureLevel == 0)
                {
                    waterSpot.Fill = new SolidColorBrush(Color.FromRgb((byte)rand.Next(200, 229), (byte)rand.Next(240, 244), (byte)rand.Next(250, 256))); // Lighter blue for colder temperatures
                }
                else if (temperatureLevel == 1)
                {
                    waterSpot.Fill = new SolidColorBrush(Color.FromRgb(0, 0, (byte)rand.Next(100, 255))); // Regular blue
                }
                else
                {
                    waterSpot.Fill = new SolidColorBrush(Color.FromRgb((byte)rand.Next(229, 255), (byte)rand.Next(0, 40), (byte)rand.Next(0, 20))); // Lighter blue for colder temperatures
                }

                double waterX = rand.NextDouble() * circle.Width;
                double waterY = rand.NextDouble() * circle.Height;

                Canvas.SetLeft(waterSpot, waterX);
                Canvas.SetTop(waterSpot, waterY);

                spotCanvas.Children.Add(waterSpot);
                waterSpot.Tag = "terrestrialplanet";
                
                
            }

            // Fill the rest of the circle with a randomly generated stone
            SolidColorBrush landBrush = new SolidColorBrush();
            if (temperatureLevel == 0)
            {
                landBrush.Color = Color.FromRgb(230, 230, 230);
            }
            else if (temperatureLevel == 1)
            {
                landBrush.Color = Color.FromRgb(100, 100, 100);
            }
            else
            {
                landBrush.Color = Color.FromRgb(20, 20, 20);
            }
            Ellipse stone = new Ellipse();
            stone.Width = circle.Width;
            stone.Height = circle.Height;
            stone.Fill = landBrush;
            stone.Tag = "terrestrialplanet";
            circle.Tag = "terrestrialplanet";
            // Add the spot canvas and grey rectangle to a grid
            Grid grid = new Grid();
            grid.Children.Add(stone);
            grid.Children.Add(spotCanvas);

            circle.Fill = new VisualBrush(grid);

            // Position the circle in the canvas
            Canvas.SetLeft(circle, ((SpaceCanvas.ActualWidth - circle.Width) / 2) + planetarydist);
            Canvas.SetTop(circle, ((SpaceCanvas.ActualWidth - circle.Width) / 2) + planetarydist);
            GenerateMoons(SpaceCanvas, circle,moonNumber);
            
        }
        public JObject GenerateTerrestrialPlanetAttributesToJson(string planetName, int lifeLevel, int r, int waterLevel, int temperatureLevel, bool loraxMode, int moonNumber, double planetaryDist)
        {
            JObject terrestrialPlanetJson = new JObject();
            terrestrialPlanetJson["type"] = "TerrestrialPlanet";
            terrestrialPlanetJson["name"] = planetName;
            terrestrialPlanetJson["lifelevel"] = lifeLevel;
            terrestrialPlanetJson["radius"] = r;
            terrestrialPlanetJson["waterlevel"] = waterLevel;
            terrestrialPlanetJson["temperature"] = temperatureLevel;
            terrestrialPlanetJson["moonNumber"] = moonNumber;
            terrestrialPlanetJson["lm"] = loraxMode;
            terrestrialPlanetJson["distancefromstar"] = planetaryDist;

            return terrestrialPlanetJson;
        }
    }
}
