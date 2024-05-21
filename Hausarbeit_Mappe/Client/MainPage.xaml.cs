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
using NAudio.Wave;
using NAudio.Wave.SampleProviders;

namespace Client
{
    /// <summary>
    /// Interaktionslogik für MainPage.xaml
    /// </summary>
    public partial class MainPage : UserControl
    {
        private WaveOutEvent _waveOutEvent;
        private AudioFileReader _audioFileReader;
        private DispatcherTimer _timer;
        private MainWindow _mainWindow;
        private bool repeat = false;


        public MainPage(MainWindow wnd)
        {
            InitializeComponent();
            ContentFrame.NavigationService.Navigate(new _Suche());
            _mainWindow = wnd;
            _waveOutEvent = new WaveOutEvent();
            _timer = new DispatcherTimer();

            _audioFileReader = new AudioFileReader("C:\\Users\\Tim\\Desktop\\Hausarbeit\\Hausarbeit\\Hausarbeit_Mappe\\Client\\Lieder\\Test.mp3");

            _waveOutEvent.Init(_audioFileReader);

            TotalTimeTextBlock.Text = _audioFileReader.TotalTime.ToString(@"mm\:ss");   
            PositionSlider.Maximum = _audioFileReader.TotalTime.TotalSeconds;
            _timer.Interval = TimeSpan.FromSeconds(1);
            _timer.Tick += Timer_Tick;
            _waveOutEvent.PlaybackStopped += Songende;
        }

        private void Start_Click(object sender, RoutedEventArgs e)
        {
            _waveOutEvent.Play();
            _timer.Start();
            
            

        }

        private void Timer_Tick(object sender, EventArgs e)
        {
            if (_audioFileReader != null && _waveOutEvent.PlaybackState == PlaybackState.Playing)
            {
                PositionSlider.Value = _audioFileReader.CurrentTime.TotalSeconds;
                CurrentTimeTextBlock.Text = _audioFileReader.CurrentTime.ToString(@"mm\:ss");
                 
            }
        }

        private void Stop_Click(object sender, RoutedEventArgs e)
        {
            _waveOutEvent.Pause();
            _timer.Stop();
        }

        private void Lautstärke_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            if(_audioFileReader != null)
            {
                _audioFileReader.Volume = (float)Lautstärke.Value;
            }
        }

        private void PositionSlider_PreviewMouseUp(object sender, MouseButtonEventArgs e)
        {
            if (_audioFileReader != null)
            {
                _audioFileReader.CurrentTime = TimeSpan.FromSeconds(PositionSlider.Value);
            }
        }

        private void Restart_Click(object sender, RoutedEventArgs e)
        {
            if(_audioFileReader != null)
            {
                _audioFileReader.Position = 0;
                _waveOutEvent.Play();
                _timer.Start();
            }
        }

        private void Loop_Checked(object sender, RoutedEventArgs e)
        {
            repeat = true;
        }

        private void Loop_Unchecked(object sender, RoutedEventArgs e)
        {
            repeat = false;
        }

        private void Songende(object sender, StoppedEventArgs e)
        {
            if(repeat && _audioFileReader != null) 
            {
                _audioFileReader.Position = 0;
                _waveOutEvent.Play();
            }
        }

        private void Benutzer_Click(object sender, RoutedEventArgs e)
        {
            ContentFrame.NavigationService.Navigate(new _Benutzer(_mainWindow, this));

        }

        private void Favoriten_Click(object sender, RoutedEventArgs e)
        {
            ContentFrame.NavigationService.Navigate(new _Favoriten());

        }

        private void Playlists_Click(object sender, RoutedEventArgs e)
        {
            ContentFrame.NavigationService.Navigate(new _Playlists());

        }

        private void Suche_Click(object sender, RoutedEventArgs e)
        {
            ContentFrame.NavigationService.Navigate(new _Suche());

        }

        public void Stopthemusic()
        {
            _waveOutEvent.Stop();
        }
    }
}
