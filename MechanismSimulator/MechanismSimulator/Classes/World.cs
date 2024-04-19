using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using System.Windows.Media.Animation;
using System.Windows;

namespace MechanismSimulator.Classes
{
    internal class World : Body

    {
        protected int r;
        protected internal Ellipse circle;
        public World(Canvas spaceCanvas, int R) : base(spaceCanvas)
        {
            SpaceCanvas = spaceCanvas;
            r = R;
            circle = new Ellipse();
        }
        public static void GenerateWorld(Canvas SpaceCanvas, Ellipse circle, int r, int moonNumber)
        {
            
            circle.Width = r * 2;
            circle.Height = r * 2;
            SpaceCanvas.Children.Add(circle);
        }
        public void GenerateMoons(Canvas SpaceCanvas, Ellipse circle, int moonNumber) 
        {
            for (int i = 0; i < moonNumber; i++)
            {
                Random moonRand = new Random();
                Ellipse moon = new Ellipse();
                moon.Width = moonRand.Next(3) + 2;
                moon.Height = moonRand.Next(3) + 2;
                moon.Fill = Brushes.Gray;
                SpaceCanvas.Children.Add(moon);

                // Set initial position of moon
                Canvas.SetLeft(moon, Canvas.GetTop(circle)/2);
                Canvas.SetTop(moon, Canvas.GetTop(circle)/2);

                // Create and start animation for the moon
                CreateMoonAnimation(moon, circle);
            }
        }
        private void CreateMoonAnimation(Ellipse moon, Ellipse circle)
        {
            Random moonRand = new Random();
            int timetaken = moonRand.Next(3) + 2;
            
            DoubleAnimationUsingKeyFrames animationX = new DoubleAnimationUsingKeyFrames();
            animationX.RepeatBehavior = RepeatBehavior.Forever;
            DoubleAnimationUsingKeyFrames animationY = new DoubleAnimationUsingKeyFrames();
            animationY.RepeatBehavior = RepeatBehavior.Forever;

            // Create keyframes for X and Y positions
            animationX.KeyFrames.Add(new LinearDoubleKeyFrame(Canvas.GetLeft(circle) + timetaken, TimeSpan.Zero));
            animationY.KeyFrames.Add(new LinearDoubleKeyFrame(Canvas.GetTop(circle) + timetaken, TimeSpan.Zero));

            // Adding orbiting positions, adjust as needed
            animationX.KeyFrames.Add(new LinearDoubleKeyFrame(Canvas.GetLeft(circle) + r*2-5, TimeSpan.FromSeconds(timetaken)));
            animationY.KeyFrames.Add(new LinearDoubleKeyFrame(Canvas.GetTop(circle) + r*2-5, TimeSpan.FromSeconds(timetaken)));

            animationX.KeyFrames.Add(new LinearDoubleKeyFrame(Canvas.GetLeft(circle), TimeSpan.FromSeconds(4)));
            animationY.KeyFrames.Add(new LinearDoubleKeyFrame(Canvas.GetTop(circle), TimeSpan.FromSeconds(4)));

            // Apply the animation to the moon
            Storyboard.SetTarget(animationX, moon);
            Storyboard.SetTargetProperty(animationX, new PropertyPath(Canvas.LeftProperty));
            Storyboard.SetTarget(animationY, moon);
            Storyboard.SetTargetProperty(animationY, new PropertyPath(Canvas.TopProperty));

            // Create storyboard and add animations
            Storyboard storyboard = new Storyboard();
            storyboard.Children.Add(animationX);
            storyboard.Children.Add(animationY);

            // Start the storyboard
            storyboard.Begin();
        }
    }

}
