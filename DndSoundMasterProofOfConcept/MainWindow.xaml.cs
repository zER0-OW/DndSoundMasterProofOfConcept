using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Forms;
using System.Windows.Input;
using System.Windows.Interop;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using DndSoundMasterProofOfConcept.LoopManagers;
using NAudio.Utils;
using NAudio.Wave;
using NAudio.WaveFormRenderer;


namespace DndSoundMasterProofOfConcept
{
    /// <summary>
    /// Interaction logic for Window1.xaml
    /// </summary>
    public partial class Window1 : Window
    {
        private System.Windows.Point startPoint;
        private System.Windows.Shapes.Rectangle currentRectangle;
        bool canceled;
        bool isDrawing;
        LoopStream loopStream;
      
        bool playing = false;
        private System.Threading.Timer timer;
        private double sampleRate;      // The sample rate of your audio file
       
        private TimeSpan totalTime;
        private WaveOutEvent outputDevice;
        private AudioFileReader audioFile;
        string audioFilePath = @"C:\Users\Mo\Downloads\01. Elden Ring.mp3";
        public Window1()
        {

            InitializeComponent();
            timer = new System.Threading.Timer(TimerCallback, null, 0, 100); // Adjust the period as needed
            SetWaveImage();

        }

        private void TimerCallback(object state)
        {
            Dispatcher.Invoke(() => UpdateRedLine());
        }

    

        private void UpdateRedLine()
        {
            if(playing) 
            {
                redLine.X1 = redLine.X2 = ConvertTimeToPosition(loopStream.CurrentTime);
            }
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

                SetWaveImage();
                var waveStream = new AudioFileReader(audioFilePath);
               loopStream = new LoopStream(waveStream);

                totalTime = waveStream.TotalTime;
                
                outputDevice.Init(loopStream);
              

            }

            outputDevice.Play();
            playing = true;

        }

        private void SetWaveImage()
        {
            var waveStream = new AudioFileReader(audioFilePath);
            StandardWaveFormRendererSettings myRendererSettings = new StandardWaveFormRendererSettings();
            myRendererSettings.Width = Screen.PrimaryScreen.Bounds.Width;
            myRendererSettings.TopHeight = 96;
            myRendererSettings.BottomHeight = 96;
            myRendererSettings.PixelsPerPeak = 1;
         
            AveragePeakProvider averagePeakProvider = new AveragePeakProvider(4);

            WaveFormRenderer renderer = new WaveFormRenderer();

            waveImage.Source = ConvertImageToBitmapImage(renderer.Render(waveStream, averagePeakProvider, myRendererSettings));
            waveStream.Dispose();
            waveStream = null;



        }


        private void stopButton_Click(object sender, RoutedEventArgs e)
        {
            outputDevice?.Stop();
            waveImage.Source = null;
        }

        private void OnPlaybackStopped(object sender, StoppedEventArgs args)
        {
            outputDevice.Dispose();
            outputDevice = null;
            audioFile = null;
            playing = false;
        }


        private BitmapImage ConvertImageToBitmapImage(System.Drawing.Image image)
        {
            // Convert System.Drawing.Image to BitmapImage
            BitmapImage bitmapImage = new BitmapImage();

            // Create a MemoryStream and write the image data to it
            using (System.IO.MemoryStream memoryStream = new System.IO.MemoryStream())
            {
                // Save the image to the MemoryStream in the desired format
                image.Save(memoryStream, System.Drawing.Imaging.ImageFormat.Png);

                // Set the BitmapImage stream source
                bitmapImage.BeginInit();
                bitmapImage.StreamSource = new System.IO.MemoryStream(memoryStream.ToArray());
                bitmapImage.EndInit();
            }

            return bitmapImage;
        }

        private void Window_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            waveImage.Width = Width;
        }

        private void waveImage_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            startPoint = e.GetPosition(waveImage);
            currentRectangle = new()
            {
                Stroke = System.Windows.Media.Brushes.Red,
                StrokeThickness = 2,
                Width = 0,
                Height = waveImage.Height
            };

            Canvas.SetLeft(currentRectangle, startPoint.X);
            Canvas.SetTop(currentRectangle,0);

            canvasOverlay.Children.Add(currentRectangle);
            isDrawing = true;
        }

        private void waveImage_MouseMove(object sender, System.Windows.Input.MouseEventArgs e)
        {
            if (isDrawing)
            {
                
                System.Windows.Point currentPoint = e.GetPosition(waveImage);

                double width = currentPoint.X - startPoint.X;
                

                currentRectangle.Width = Math.Max(0, width);
               
            }
        }

        private void Image_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            isDrawing = false; 
            Trace.WriteLine("Starttime: " + ConvertPositionIntoTime(Canvas.GetLeft(currentRectangle)) + " Endtime: " + ConvertPositionIntoTime(Canvas.GetLeft(currentRectangle) + currentRectangle.Width));
        }

        int ConvertTimeToPosition(TimeSpan currentTime)
        {
            int horizontalPosition = (int)((currentTime.TotalSeconds/ totalTime.TotalSeconds) * waveImage.Width);

            return horizontalPosition;
        }

        TimeSpan ConvertPositionIntoTime(int position)
        {
            return ConvertPositionIntoTime((double)position);
        }
        TimeSpan ConvertPositionIntoTime(double position)
        {
            TimeSpan currentTime = TimeSpan.FromSeconds((double)(position * totalTime.TotalSeconds) / waveImage.Width);
            return currentTime;
        }

        private void canvasOverlay_MouseRightButtonDown(object sender, MouseButtonEventArgs e)
        {
            isDrawing = false;
            canvasOverlay.Children.Remove(currentRectangle); 
        }
    }


}
