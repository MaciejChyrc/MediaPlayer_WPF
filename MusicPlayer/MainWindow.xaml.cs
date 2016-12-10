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
using Microsoft.Win32;
using System.Windows.Threading;

namespace MusicPlayer
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private double lastVolSliderValue;
        DispatcherTimer timer = new DispatcherTimer ();
        public MainWindow ()
        {
            InitializeComponent ();
            timer.Interval = TimeSpan.FromSeconds (1);
            timer.Tick += Ticktock;
            timer.Start ();
            volSlider.Minimum = 0;
            volSlider.Maximum = 1;
            volSlider.Value = 0.1;
            mePlayer.Volume = volSlider.Value;
        }

        private void Ticktock (object sender, EventArgs e)
        {
            timeSlider.Value = mePlayer.Position.TotalSeconds;
        }

        private void playButton_Click (object sender, RoutedEventArgs e)
        {
            mePlayer.Play ();
            timer.Start ();
        }

        private void pauseButton_Click (object sender, RoutedEventArgs e)
        {
            mePlayer.Pause ();
            timer.Stop ();
        }
        private void volSlider_ValueChanged (object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            mePlayer.Volume = volSlider.Value;
        }

        private void fileDialogButton_Click (object sender, RoutedEventArgs e)
        {
            try
            {
                OpenFileDialog fileDialog = new OpenFileDialog ();
                fileDialog.DefaultExt = "*.*";
                fileDialog.Filter = "All Files (*.*)|*.*|MP3 Files (*.mp3)|*.mp3|MP4 Files (*.mp4)|*.mp4|AVI Files (*.avi)|*.avi";
                Nullable<bool> result = fileDialog.ShowDialog ();
                /* Effect of couple of minutes of free time. Untested, likely to rename and relocate it
                 * in future use.
                 * Should work as a playlist creator from picked files.
                List<Uri> fileList = new List<Uri> ();

                foreach (string fileName in fileDialog.FileNames)
                {
                    Uri src = new Uri (fileName);
                    fileList.Add (src);
                }
                */
                if (result == true)
                {
                    string fileName = fileDialog.FileName;
                    fileChosenTxtBlock.Text = fileName;
                    mePlayer.Close ();
                    Uri sourcePath = new Uri (fileName);
                    mePlayer.Source = sourcePath;
                    timeSlider.Value = 0;
                    mePlayer.Position = TimeSpan.FromSeconds (timeSlider.Value);
                    timeSlider.Value = mePlayer.Position.Seconds;
                    mePlayer.Play ();
                }
            }
            catch (Exception)
            {
                MessageBox.Show ("Błąd wczytywania pliku.");
            }
        }

        private void muteButton_Click (object sender, RoutedEventArgs e)
        {
            if (mePlayer.Volume != 0)
            {
                lastVolSliderValue = volSlider.Value;
                volSlider.Value = 0;
                mePlayer.Volume = 0;
            }
            else if (mePlayer.Volume == 0)
            {
                volSlider.Value = lastVolSliderValue;
                mePlayer.Volume = volSlider.Value;
            }
        }

        private void mePlayer_MediaOpened (object sender, RoutedEventArgs e)
        {
            TimeSpan ts = mePlayer.NaturalDuration.TimeSpan;
            timeSlider.Maximum = ts.TotalSeconds;
            timeSlider.SmallChange = 1;
        }

        private void timeSlider_ValueChanged (object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            mePlayer.Position = TimeSpan.FromSeconds (timeSlider.Value);
            if (mePlayer.NaturalDuration.HasTimeSpan)
            mediaTimeBlock.Text = String.Format ("{0:00}:{1:00}/{2:00}:{3:00}", mePlayer.Position.Minutes, mePlayer.Position.Seconds, mePlayer.NaturalDuration.TimeSpan.Minutes, mePlayer.NaturalDuration.TimeSpan.Seconds);
        }
    }
}