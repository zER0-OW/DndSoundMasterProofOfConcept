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
using System.Windows.Shapes;
using NAudio.Wave;


namespace DndSoundMasterProofOfConcept
{
    /// <summary>
    /// Interaction logic for Window1.xaml
    /// </summary>
    public partial class Window1 : Window
    {
        private WaveOutEvent outputDevice;
        private AudioFileReader audioFile;
        string audioFilePath = @"C:\Users\Mo\Downloads\01. Elden Ring.mp3";
        public Window1()
        {
            InitializeComponent();
           
        }

     

        private void playButton_Click(object sender, RoutedEventArgs e)
        {
            if (outputDevice == null)
            {
                outputDevice = new WaveOutEvent();
                outputDevice.PlaybackStopped += OnPlaybackStopped;
            }
            if (audioFile == null)
            {
                audioFile = new AudioFileReader(audioFilePath);
                outputDevice.Init(audioFile);
            }
            outputDevice.Play();


        }

        private void stopButton_Click(object sender, RoutedEventArgs e)
        {
            outputDevice?.Stop();
        }

        private void OnPlaybackStopped(object sender, StoppedEventArgs args)
        {
            outputDevice.Dispose();
            outputDevice = null;
            audioFile.Dispose();
            audioFile = null;
        }
    }
}
