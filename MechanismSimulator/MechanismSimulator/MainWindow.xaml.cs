using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.IO;
using System.Windows.Threading;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using Newtonsoft.Json;
using MechanismSimulator.Classes;
using System.Collections;
using System.Globalization;
using System.Windows.Media.Media3D;
using System.Reflection.Emit;
using System.Threading;
using System.Text.RegularExpressions;
using System.Diagnostics.Eventing.Reader;
using Newtonsoft.Json.Linq;

namespace MechanismSimulator
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        List<JObject> terrestrialPlanetsJson = new List<JObject>();
        List<JObject> gaseousPlanetsJson = new List<JObject>();
        List<JObject> starsJson = new List<JObject>();
        JArray fullJson = new JArray();
        public string filen;
        private List<Body> allBodies = new List<Body>();
        private string[,] months = { { "January", "February", "March", "April", "May", "June", "July", "August", "September", "October", "November", "December" },{ "Poseidonios", "Aphroditios", "Artemisios", "Dionysios", "Hermaios", "Demetrios", "Kleonaios", "Hyperetaios", "Eukleios", "Aristaios", "Diogenes", "Poseidonios" }, { "Thoth", "Phaophi", "Athyr", "Choiak", "Tybi", "Mechir", "Phamenoth", "Pharmouthi", "Pachon", "Payni", "Epiphi", "Mesore" } };
        public Queue<string> rightQueue = new Queue<string>();
        public double DegreesToRadians(double degrees)
        {
            return degrees * Math.PI / 180.0;
        }
        
        private Point lastMousePos;
        
        private Random rand = new Random();
        private DispatcherTimer realTimer = new DispatcherTimer();
        private TranslateTransform translateTransform = new TranslateTransform();
        private double bgEntRate, olympiadNeedleRate, sarosNeedleRate, callippicNeedleRate, exeligmosNeedleRate, dialMonthRate, planetMoveRate;
        private DateTime startTime = DateTime.Now;

        protected const double olympiadConstant = 223/48;
        protected const double sarosConstant = 1;
        protected const double dialMonthConstant = -223/12;
        protected const double callippicConstant = 1/4.75;
        protected const double exeligmosConstant = 2/3.0;
        protected const double bgEntConstant = 90/12;
        protected const double planetSpeedConstant = 1;
        private double sliderValue;
        private int currentYear;
        private int pastMonth;
        private int monthState;
        public bool starLoaded;
        private double zoomMatrixScale = 1.0;
        public int globalFieldStrength;
        public MainWindow()
        {
            
            InitializeComponent();
            LoadBackgroundEntities();
            // Apply transformations to the SpaceCanvas
            SpaceCanvas.RenderTransform = new TransformGroup()
            {
                Children = new TransformCollection()
                {
                    translateTransform
                }
            };

            SpaceCanvas.MouseLeftButtonDown += SpaceCanvas_MouseLeftButtonDown;
            SpaceCanvas.MouseMove += SpaceCanvas_MouseMove;
            SpaceCanvas.MouseLeftButtonUp += SpaceCanvas_MouseLeftButtonUp;
            realTimer.Interval = TimeSpan.FromMilliseconds(1);
            realTimer.Tick += Timer_Tick;
            realTimer.Start();

        }
        private void Timer_Tick(object sender, EventArgs e)
        {
            TimeSpan elapsedTime = DateTime.Now - startTime;
            // Move stars
            foreach (UIElement element in SpaceCanvas.Children) 
            {
                if (element is Ellipse bgent && bgent.Tag as string == "bgent")
                {
                    Canvas.SetLeft(bgent, Canvas.GetLeft(bgent) - bgEntRate*rand.NextDouble()*10);
                    if (Canvas.GetLeft(bgent) < -2500)
                    {
                        Canvas.SetLeft(bgent, 2500);
                        Canvas.SetTop(bgent, rand.NextDouble() * 2500 * ((rand.Next(1) * 2) - 1 ));
                    }
                }
                else if (element is Ellipse planet && (planet.Tag as string == "terrestrialplanet" || planet.Tag as string == "gaseousplanet"))
                {
                    //Random internalrand = new Random();
                    //// Rotate terrestrial and gaseous planets around the center of the canvas
                    //double centerX = SpaceCanvas.ActualWidth / 2;
                    //double centerY = SpaceCanvas.ActualHeight / 2;
                    //double radius = Math.Min(centerX, centerY) - planet.Width / 2;

                    //double anglePerSecond = 0.1; // Adjust rotation speed as needed
                    //double currentAngle = (double)planet.GetValue(Canvas.LeftProperty) * Math.PI / 180;

                    //// Update angle
                    //currentAngle += planetMoveRate * anglePerSecond * elapsedTime.TotalSeconds;

                    //// Calculate new position
                    //double newX = (centerX + planetMoveRate * radius * Math.Cos(currentAngle));
                    //double newY = (centerY + planetMoveRate *radius * Math.Sin(currentAngle));

                    //// Update position
                    //Canvas.SetLeft(planet, newX);
                    //Canvas.SetTop(planet, newY);
                }
            }

            RotateTransform dialMonthRotateTransform = dialMonth.RenderTransform as RotateTransform;
            dialMonthRotateTransform.Angle += dialMonthRate;
            RotateTransform sarosCycleRotateTransform = SarosNeedle.RenderTransform as RotateTransform;
            sarosCycleRotateTransform.Angle += sarosNeedleRate;
            RotateTransform olympiadRotateTransform = OlympiadNeedle.RenderTransform as RotateTransform;
            olympiadRotateTransform.Angle += olympiadNeedleRate;
            RotateTransform callippicRotateTransform = CallippicNeedle.RenderTransform as RotateTransform;
            callippicRotateTransform.Angle += callippicNeedleRate;
            RotateTransform exeligmosRotateTransform = ExeligmosNeedle.RenderTransform as RotateTransform;
            exeligmosRotateTransform.Angle += exeligmosNeedleRate;
           
            realTimeLabel.Content = $"Real Time Elapsed: {elapsedTime.ToString(@"hh\:mm\:ss")}";
            int monthIndex = (((int)Math.Floor((dialMonthRotateTransform.Angle * 0.034) / (dialMonthRate - 1)))%12);
            int dayIndex = (((int)Math.Floor((dialMonthRotateTransform.Angle) / (dialMonthRate - 1)))%365);
            if ((monthIndex == 0) && (pastMonth != monthIndex))
            {
                currentYear++;
                yearLabel.Content = $"Years passed: {currentYear}";
            }
            monthLabel.Content = $"Current Month: {months[monthState,monthIndex]}";
            dayLabel.Content = $"Current Day: {dayIndex}";
            pastMonth = monthIndex;
        }
        private void SpeedSlider_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
           sliderValue = ((Slider)sender).Value;
           olympiadNeedleRate =+ sliderValue * olympiadConstant;
           sarosNeedleRate =+ sliderValue * sarosConstant;
           callippicNeedleRate =+ sliderValue * callippicConstant;
           exeligmosNeedleRate =+ sliderValue * exeligmosConstant;
           bgEntRate =+ sliderValue * bgEntConstant; 
           dialMonthRate =+ sliderValue * dialMonthConstant;
           planetMoveRate =+ sliderValue * planetSpeedConstant;
        }
        private void ZoomIn_Click(object sender, RoutedEventArgs e)
        {
            ApplyZoomMatrix(1.1);

        }
        private void ZoomOut_Click(object sender, RoutedEventArgs e)
        {
            ApplyZoomMatrix(0.9);
        }
        private void ApplyZoomMatrix(double factor)
        {
            // Create a scaling transformation matrix
            zoomMatrixScale *= factor;
            Matrix scaleMatrix = new Matrix();
            scaleMatrix.Scale(zoomMatrixScale, zoomMatrixScale);



            // Apply the scaling transformation to each UI element in the canvas
            foreach (UIElement element in SpaceCanvas.Children)
            {
                element.RenderTransformOrigin = new Point(0.5,0.5);
                element.RenderTransform = new MatrixTransform(scaleMatrix);
            }
        }
        private void SetDialEnglish(object sender, RoutedEventArgs e)
        {
            dialMonth.Source = new BitmapImage(new Uri("pack://application:,,,/Assets/english.png")); ;
            monthState = 0;
        }
        private void SetDialCorinthian(object sender, RoutedEventArgs e)
        {
            dialMonth.Source = new BitmapImage(new Uri("pack://application:,,,/Assets/corinthian.png")); ;
            monthState = 1;
        }
        private void SetDialSothic(object sender, RoutedEventArgs e)
        {
            dialMonth.Source = new BitmapImage(new Uri("pack://application:,,,/Assets/sothic.png")); ;
            monthState = 2;
        }

        

        private void SpaceCanvas_MouseMove(object sender, MouseEventArgs e)

        {

            if (SpaceCanvas.IsMouseCaptured)
            {
                Point currentMousePos = e.GetPosition(SpaceCanvas);
                double dx = currentMousePos.X - lastMousePos.X;
                double dy = currentMousePos.Y - lastMousePos.Y;
                

                foreach (UIElement child in SpaceCanvas.Children)
                {
                    // Apply translation to the child element
                    TranslateTransform translate = child.RenderTransform as TranslateTransform;
                    if (translate == null)
                    {
                        translate = new TranslateTransform();
                        child.RenderTransform = translate;
                    }

                    translate.X += dx;
                    translate.Y += dy;
                }

                lastMousePos = currentMousePos;
                SetupCanvasClipping();
                
            }

        }

        private void SetupCanvasClipping()
        {
            RectangleGeometry clippingRegion = new RectangleGeometry(new Rect(0, 0, SpaceCanvas.ActualWidth, SpaceCanvas.ActualHeight));
            SpaceCanvas.Clip = clippingRegion;
        }
        private void SpaceCanvas_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            lastMousePos = e.GetPosition(SpaceCanvas);
            SpaceCanvas.CaptureMouse();
        }
        private void SpaceCanvas_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            SpaceCanvas.ReleaseMouseCapture();
        }

       public void LoadBackgroundEntities()
        {
                BackgroundEntity bgent = new BackgroundEntity(SpaceCanvas, 2, 2);
                allBodies.Add(bgent);
        }
        private void GenerateTerrestrialPlanet_Click(object sender, RoutedEventArgs e)
        {
            string errorMessage = "";

            if (string.IsNullOrEmpty(planetNameTextBox.Text))
            {
                errorMessage += "\n- Planet name is required.";
            }

            if (!int.TryParse(lifeTextBox.Text, out int lifeLevel))
            {
                errorMessage += "\n- Life level must be a valid integer.";
            }
            if (!int.TryParse(moonNumberTextBox.Text, out int moonNumber))
            {
                errorMessage += "\n- Moon number must be a valid integer.";
            }
            if (!int.TryParse(radiusTextBox.Text, out int r))
            {
                errorMessage += "\n- Radius must be a valid integer.";
            }

            if (!int.TryParse(waterTextBox.Text, out int waterLevel))
            {
                errorMessage += "\n- Water level must be a valid integer.";
            }

            if (HighPlanetTemperature.IsChecked == false && MediumPlanetTemperature.IsChecked == false && LowPlanetTemperature.IsChecked == false)
            {
                errorMessage += "\n- Temperature level must be selected.";
            }

            if (!string.IsNullOrEmpty(errorMessage))
            {
                MessageBox.Show($"Invalid input(s): {errorMessage}");
                return;
            }
            {
                int temperatureLevel = 0;
                if (HighPlanetTemperature.IsChecked == true)
                {
                    temperatureLevel = 2;
                }
                else if (MediumPlanetTemperature.IsChecked == true)
                {
                    temperatureLevel = 1;
                }
                string planetName = planetNameTextBox.Text;
                bool LoraxMode = false;
                if (LoraxModeBox.IsChecked == true)
                {
                    LoraxMode = true;
                }
                TerrestrialPlanet terrestrialPlanet = new TerrestrialPlanet(PlanetCanvas, r);
                terrestrialPlanet.GenerateTerrestrialPlanet(planetName, lifeLevel, r, waterLevel, temperatureLevel, LoraxMode, moonNumber, 0);
                
            }
        }
        public void AddPlanetToQueue(string planetName, int lifeLevel, int waterLevel, int radius, int temperature, int moonNumber)
        {
            // Add item to the queue
            rightQueue.Enqueue($"Planet {rightQueue.Count}: {planetName} - Life: {lifeLevel} - Water/Lava: {waterLevel} - Radius: {radius} - Temp: {temperature} - Moons: {moonNumber}");
            // Update ListBox
            UpdateQueue();
        }
        public void AddStarToQueue(string starName, int fieldStrength, int radius, int temperature, int systype)
        {
            // Add item to the queue
            rightQueue.Enqueue($"Star: {starName} - Field Strength: {fieldStrength} - Radius: {radius} - Temperature: {temperature} - Star Number: {systype + 1}");
            // Update ListBox
            UpdateQueue();
        }
        private void UpdateQueue()
        {
            // Clear the ListBox
            RightQueueBox.Items.Clear();
            // Add each item from the queue to the ListBox
            foreach (var item in rightQueue)
            {
                RightQueueBox.Items.Add(item);
            }
        }
        private void ClearStarCanvas_Click(object sender, RoutedEventArgs e)
        {
            StarCanvas.Children.Clear();
        }
        private void ClearPlanetCanvas_Click(object sender, RoutedEventArgs e)
        {
            PlanetCanvas.Children.Clear();
        }
        private void LoadStar_Click(object sender, RoutedEventArgs e)
        {
            string errorMessage = "";

            if (string.IsNullOrEmpty(starNameTextBox.Text))
            {
                errorMessage += "\n- Star name is required.";
            }

            if (!int.TryParse(fieldStrengthTextBox.Text, out int fieldStrength))
            {
                errorMessage += "\n- Field strength must be a valid integer.";
            }

            if (!int.TryParse(starRadiusTextBox.Text, out int r))
            {
                errorMessage += "\n- Radius must be a valid integer.";
            }

            if (UnarySystem.IsChecked == false && BinarySystem.IsChecked == false && TrinarySystem.IsChecked == false)
            {
                errorMessage += "\n- System type must be selected.";
            }

            if (HighStarTemperature.IsChecked == false && MediumStarTemperature.IsChecked == false && LowStarTemperature.IsChecked == false)
            {
                errorMessage += "\n- Temperature level must be selected.";
            }

            if (!string.IsNullOrEmpty(errorMessage))
            {
                MessageBox.Show($"Invalid input(s): {errorMessage}");
                return;
            }
            else if (starLoaded == true)
            {
                MessageBox.Show("A Star has already been generated.");
                return;
            }

            {
                int systype = 0;
                int temperatureLevel = 0;
                if (HighStarTemperature.IsChecked == true)
                {
                    temperatureLevel = 2;
                }
                else if (MediumStarTemperature.IsChecked == true)
                {
                    temperatureLevel = 1;
                }
                if (BinarySystem.IsChecked == true)
                {
                    systype = 1;
                }
                else if (TrinarySystem.IsChecked == true)
                {
                    systype = 2;
                }
                globalFieldStrength = fieldStrength;
                string starName = starNameTextBox.Text;

                Star  star = new Star(SpaceCanvas, r);
                starLoaded= true;
                star.GenerateStar(starName, r, temperatureLevel, systype);
                AddStarToQueue(starName, fieldStrength, r, temperatureLevel, systype);
                JObject starJson = star.GenerateStarAttributesToJson(starName, fieldStrength, r, temperatureLevel, systype);
                starsJson.Add(starJson);
                if (DeathStarBox.IsChecked == true)
                {
                    DeathStarSpace.Visibility = Visibility.Visible;
                }
                else if (DysonSphereBox.IsChecked == true)
                {
                    DysonSphereSpace.Visibility = Visibility.Visible;
                }
            }
        }
        private void GenerateStar_Click(object sender, RoutedEventArgs e)
        {
            string errorMessage = "";

            if (string.IsNullOrEmpty(starNameTextBox.Text))
            {
                errorMessage += "\n- Star name is required.";
            }

            if (!int.TryParse(fieldStrengthTextBox.Text, out int fieldStrength))
            {
                errorMessage += "\n- Field strength must be a valid integer.";
            }

            if (!int.TryParse(starRadiusTextBox.Text, out int r))
            {
                errorMessage += "\n- Radius must be a valid integer.";
            }

            if (UnarySystem.IsChecked == false && BinarySystem.IsChecked == false && TrinarySystem.IsChecked == false)
            {
                errorMessage += "\n- System type must be selected.";
            }

            if (HighStarTemperature.IsChecked == false && MediumStarTemperature.IsChecked == false && LowStarTemperature.IsChecked == false)
            {
                errorMessage += "\n- Temperature level must be selected.";
            }

            if (!string.IsNullOrEmpty(errorMessage))
            {
                MessageBox.Show("Invalid input(s):" + errorMessage);
                return;
            }
            {
                int temperatureLevel = 0;
                int systype = 0;
                if (HighStarTemperature.IsChecked == true)
                {
                    temperatureLevel = 2;
                }
                else if (MediumStarTemperature.IsChecked == true)
                {
                    temperatureLevel = 1;
                }
                if (BinarySystem.IsChecked == true)
                {
                    systype = 1;
                }
                else if (TrinarySystem.IsChecked == true)
                {
                    systype = 2;
                }

              
                string starName = starNameTextBox.Text;

                Star star = new Star(StarCanvas, r);

                star.GenerateStar(starName, r, temperatureLevel, systype);
                if (DeathStarBox.IsChecked == true)
                {
                    DeathStarCanvas.Visibility = Visibility.Visible;
                }
                else if (DysonSphereBox.IsChecked == true)
                {
                    DysonSphereCanvas.Visibility = Visibility.Visible;
                }
            }
        }
        private void LoadTerrestrialPlanet_Click(object sender, RoutedEventArgs e) 
        {

            string errorMessage = "";

            if (string.IsNullOrEmpty(planetNameTextBox.Text))
            {
                errorMessage += "\n- Planet name is required.";
            }

            if (!int.TryParse(lifeTextBox.Text, out int lifeLevel))
            {
                errorMessage += "\n- Life level must be a valid integer.";
            }

            if (!int.TryParse(moonNumberTextBox.Text, out int moonNumber))
            {
                errorMessage += "\n- Moon number must be a valid integer.";
            }

            if (!int.TryParse(radiusTextBox.Text, out int r))
            {
                errorMessage += "\n- Radius must be a valid integer.";
            }

            if (!int.TryParse(waterTextBox.Text, out int waterLevel))
            {
                errorMessage += "\n- Water level must be a valid integer.";
            }

            if (HighPlanetTemperature.IsChecked == false && MediumPlanetTemperature.IsChecked == false && LowPlanetTemperature.IsChecked == false)
            {
                errorMessage += "\n- Temperature level must be selected.";
            }

            if (!string.IsNullOrEmpty(errorMessage))
            {
                MessageBox.Show("Invalid input(s):" + errorMessage);
                return;
            }
            else if (starLoaded != true)
            {
                MessageBox.Show("A Star must be generated first.");
                return;
            }
            {
                int temperatureLevel = 0;
                if (HighPlanetTemperature.IsChecked == true)
                {
                    temperatureLevel = 2;
                }
                else if (MediumPlanetTemperature.IsChecked == true)
                {
                    temperatureLevel = 1;
                }
                string planetName = planetNameTextBox.Text;
                bool LoraxMode = false;
                if (LoraxModeBox.IsChecked == true)
                {
                    LoraxMode = true;
                }
                TerrestrialPlanet terrestrialPlanet = new TerrestrialPlanet(SpaceCanvas, r);
                double planetarydist = globalFieldStrength * rand.NextDouble() + 100;
                terrestrialPlanet.GenerateTerrestrialPlanet(planetName, lifeLevel, r, waterLevel, temperatureLevel, LoraxMode, moonNumber, planetarydist);
                AddPlanetToQueue(planetName, lifeLevel,waterLevel,r,temperatureLevel,moonNumber);
                JObject terrestrialPlanetJson = terrestrialPlanet.GenerateTerrestrialPlanetAttributesToJson(planetName, lifeLevel, r, waterLevel, temperatureLevel, LoraxMode, moonNumber, planetarydist);
                terrestrialPlanetsJson.Add(terrestrialPlanetJson);
            }
        }
        private void LoadGaseousPlanet_Click(object sender, RoutedEventArgs e)
        {
            string errorMessage = "";

            if (string.IsNullOrEmpty(planetNameTextBox.Text))
            {
                errorMessage += "\n- Planet name is required.";
            }

            if (!int.TryParse(moonNumberTextBox.Text, out int moonNumber))
            {
                errorMessage += "\n- Moon number must be a valid integer.";
            }

            if (!int.TryParse(radiusTextBox.Text, out int r))
            {
                errorMessage += "\n- Radius must be a valid integer.";
            }

            if (HighPlanetTemperature.IsChecked == false && MediumPlanetTemperature.IsChecked == false && LowPlanetTemperature.IsChecked == false)
            {
                errorMessage += "\n- Temperature level must be selected.";
            }

            if (!string.IsNullOrEmpty(errorMessage))
            {
                MessageBox.Show("Invalid input(s):" + errorMessage);
                return;
            }
            else if (starLoaded != true)
            {
                MessageBox.Show("A Star must be generated first.");
                return;
            }
            {
                int temperatureLevel = 0;
                if (HighPlanetTemperature.IsChecked == true)
                {
                    temperatureLevel = 2;
                }
                else if (MediumPlanetTemperature.IsChecked == true)
                {
                    temperatureLevel = 1;
                }
                string planetName = planetNameTextBox.Text;

                GaseousPlanet gaseousPlanet = new GaseousPlanet(SpaceCanvas, r);
                double planetarydist = globalFieldStrength * rand.NextDouble() + 100;

                gaseousPlanet.GenerateGaseousPlanet(planetName, temperatureLevel, r, moonNumber, planetarydist);
                AddPlanetToQueue(planetName, 0, 0, r, temperatureLevel, moonNumber);
                JObject gaseousPlanetJson = gaseousPlanet.GenerateGaseousPlanetAttributesToJson(planetName, temperatureLevel, r, moonNumber, planetarydist);
                gaseousPlanetsJson.Add(gaseousPlanetJson);
            }
        }
        
        private void GenerateGaseousPlanet_Click(object sender, RoutedEventArgs e)
        {
            string errorMessage = "";

            if (string.IsNullOrEmpty(planetNameTextBox.Text))
            {
                errorMessage += "\n- Planet name is required.";
            }

            if (!int.TryParse(moonNumberTextBox.Text, out int moonNumber))
            {
                errorMessage += "\n- Moon number must be a valid integer.";
            }

            if (!int.TryParse(radiusTextBox.Text, out int r))
            {
                errorMessage += "\n- Radius must be a valid integer.";
            }

            if (HighPlanetTemperature.IsChecked == false && MediumPlanetTemperature.IsChecked == false)
            {
                errorMessage += "\n- Temperature level must be selected.";
            }

            if (!string.IsNullOrEmpty(errorMessage))
            {
                MessageBox.Show("Invalid input(s):" + errorMessage);
                return;
            }
            {
                int temperatureLevel = 0;
                if (HighPlanetTemperature.IsChecked == true)
                {
                    temperatureLevel = 2;
                }
                else if (MediumPlanetTemperature.IsChecked == true)
                {
                    temperatureLevel = 1;
                }
                string planetName = planetNameTextBox.Text;

                GaseousPlanet gaseousPlanet = new GaseousPlanet(PlanetCanvas, r);
                gaseousPlanet.GenerateGaseousPlanet(planetName, temperatureLevel, r, moonNumber, 0);
            }
            
        }

        private void GeneratePlanetName_Click(object sender, RoutedEventArgs e)
        {
            SuggestedPlanetTextBox.Text = $"Suggested Planet Name: {EntityRegex.CalculatePlanetaryRegex()}";
        }
        private void GenerateStarName_Click(object sender, RoutedEventArgs e)
        {
            SuggestedStarTextBox.Text = $"Suggested Star Name: {EntityRegex.CalculateStarRegex()}";
        }
        private void RefreshJson_Click(object sender, RoutedEventArgs e)
        {
            JsonListBox.Items.Clear();
            string[] jsonFiles = Directory.GetFiles("Presets/", "*.json");
            foreach (string jsonFile in jsonFiles)
            {
                JsonListBox.Items.Add(System.IO.Path.GetFileName(jsonFile));
            }

        }

        private void LoadJson_Click(object sender, RoutedEventArgs e)
        {
            string checknull = (string)JsonListBox.SelectedItem;
            
            if (checknull != null)
            {
                string selectedJson = JsonListBox.SelectedItem.ToString();
                Parser parser = new Parser(SpaceCanvas, this, starLoaded);
                parser.ParseSavedJson(selectedJson);
            }
            else
            {
                MessageBox.Show("Please select a file to load.");
            }
        }
        private void SaveJson_Click(object sender, RoutedEventArgs e)
        {
            fullJson.Merge(starsJson);
            fullJson.Merge(terrestrialPlanetsJson);
            fullJson.Merge(gaseousPlanetsJson);
            Parser parser = new Parser(SpaceCanvas, this, true);
            parser.SaveToJson(fullJson);
        }
        private void RemoveLatestEntity_Click(object sender, RoutedEventArgs e)
        {
            rightQueue.Dequeue();
            UpdateQueue();
        }
        private void CloseButton_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }
    }
}
