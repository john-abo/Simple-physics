/*
 * Plenty of code here was borrowed from the internet. This was simply an experiment with the .NET platform and WPF
 * There's still plenty that I'd like to add, considering the window sizes and some of the physics and stats; This 
 * is only the beginning. I'm still pleased with how this came out and I expect to use this for more experimentation.
 */

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Windows.Threading;

namespace sample_keypress_listen
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {

        private bool started = false;

        // I should implement it so that these values are set in a file so the users
        // can adjust the physics and give opinion
        // Actually I could just make sliders
        // ... or both?

        // Object acceleration
        // Added accel is the force the user is applying to the object
        // while the key is held down
        private float accelX = 0;
        private float addedAccelX = 0;
        private float accelY = -1;
        private float addedAccelY = 0;

        // Object velocity
        private float veloX = 0;
        private float veloY = 0;

        // May implement friction as a function of the velocity in the same dimension
        //private float frictX = 0;
        //private float frictY = 0;
        // I guess I could just multiply this by a number <1 with each tick lols


        // Collision Dampening
        private float efficiency = 0.7f;

        // Debug counter and dimensions of the block
        //private int counter = 0;
        private int blockDimension = 30;

        DispatcherTimer gameTimer = new DispatcherTimer();

        public MainWindow()
        {
            InitializeComponent();

            gameTimer.Tick += GameTimerEvent;
            gameTimer.Interval = TimeSpan.FromMilliseconds(16);
        }

        /*
         * Actions that occur for each game tick
         */
        private void GameTimerEvent(object sender, EventArgs e)
        {
            // Prints some stats for reading
            //counter++;
            Stats.Text = "";
            //Stats.Text += $"Horizontal Acceleration: {accelX}\n";
            //Stats.Text += $"Vertical Acceleration: {accelY}\n";
            //Stats.Text += $"Counter mod: {counter % 60}\n";   
            Stats.Text += $"Horizontal Velocity: {Math.Round(veloX, 2)}\t";
            Stats.Text += $"Vertical Velocity: {Math.Round(veloY, 2)}\n";
            Stats.Text += $"Coords: ({Math.Round(Canvas.GetLeft(block), 2)},{Math.Round(Canvas.GetBottom(block), 2)})";

            // Time for some physics logic, oh no
            // I need to find a property of block that I can manipulate to control its position in the canvas
            // I think another thing that would help would be getting the object to come to a complete stop once it reaches a low enough velocity

            // Horizontal property
            // This moves the element, but if it reaches the edge of the screen it stops and pushes it back to display
            if (Canvas.GetLeft(block) >= 0 && Canvas.GetLeft(block) <= (TestSpace.Width - blockDimension))
            {
                Canvas.SetLeft(block, Canvas.GetLeft(block) + veloX);
            } else if (Canvas.GetLeft(block) < 0)
            {
                Canvas.SetLeft(block, 0);
                // Reset all values in this this direction
                veloX = (-veloX) * efficiency;
                //addedAccelX = 0;
            } else if (Canvas.GetLeft(block) > (TestSpace.Width - blockDimension))
            {
                Canvas.SetLeft(block, (TestSpace.Width - blockDimension));
                veloX = (-veloX) * efficiency;
                //addedAccelX = 0;
            }

            // Vertical property
            if (Canvas.GetBottom(block) >= 0 && Canvas.GetBottom(block) <= (TestSpace.Height - blockDimension))
            {
                Canvas.SetBottom(block, Canvas.GetBottom(block) + veloY);
            }
            else if (Canvas.GetBottom(block) < 0)
            {
                Canvas.SetBottom(block, 0);
                // Reset all values in this this direction
                veloY = (-veloY) * efficiency;
                //addedAccelY = 0;
            }
            else if (Canvas.GetBottom(block) > (TestSpace.Height - blockDimension))
            {
                Canvas.SetBottom(block, (TestSpace.Height - blockDimension));
                veloY = (-veloY) * efficiency;
                //addedAccelY = 0;
            }

            // Applies acceleration
            veloY += (accelY + addedAccelY);
            veloX += (accelX + addedAccelX);
        }

        /*
         * Switching between the opening page and interactive page
         */
        private void switchPage(object sender, RoutedEventArgs e)
        {

            if (started == false)
            {
                // This part is important, gotta set the focus for the element
                keyplease.Focus();
                
            }

            if (Page1.IsVisible == true)
            {
                Page1.Visibility = Visibility.Collapsed;
                Page2.Visibility = Visibility.Visible;
                gameTimer.Start();
            }
            else
            {
                gameTimer.Stop();
                Page2.Visibility = Visibility.Collapsed;
                Page1.Visibility = Visibility.Visible;
            }
        }

        /*
         * Debugging purposes
         * Reacts to users key press and writes to text block
         */
        private void pressed(object sender, KeyEventArgs e)
        {
            float userAccel = 2;
            float jumpSpeed = 15;
            // Just a note for something I wanted to do:
            // If I want to add elements to the canvas then I just need to add the following 
            // <canvas name>.Children.Add(<Some element>);
            // Of course I need to initialize the values for the element, but I can just copy the stats of the block
            // I think I'll also add the new elements to a list so I can keep track of all entities in the 

            Tab2_head.Text = $"key pressed: {e.Key}";
            if (e.Key == Key.D)
            {
                addedAccelX = userAccel;
            } else if (e.Key == Key.A)
            {
                addedAccelX = -userAccel;
            }
            else if (e.Key == Key.W)
            {
                addedAccelY = 0;
                veloY = jumpSpeed;
            }
            else if (e.Key == Key.S)
            {
                addedAccelY = -userAccel;
            }
        }

        /*
         * Debugging purposes
         * Reacts to users key release and writes to text block
         */
        private void released(object sender, KeyEventArgs e)
        {
            Tab2_head.Text = "key released";
            if (e.Key == Key.D)
            {
                addedAccelX = 0;
            }
            else if (e.Key == Key.A)
            {
                addedAccelX = 0;
            }
            else if (e.Key == Key.W)
            {
                addedAccelY = 0;
            }
            else if (e.Key == Key.S)
            {
                addedAccelY = 0;
            }
        }

    }
}
