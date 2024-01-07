using System;
using System.Collections.Generic;
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
        int waveFormRendererWidth;
        bool playing = false;
        private System.Threading.Timer timer;
        private double sampleRate;      // The sample rate of your audio file
        private TimeSpan currentPosition;
        private TimeSpan totalTime;
        private WaveOutEvent outputDevice;
        private AudioFileReader audioFile;
        string audioFilePath = @"C:\Users\Mo\Downloads\01. Elden Ring.mp3";
        public Window1()
        {

            InitializeComponent();
            timer = new System.Threading.Timer(TimerCallback, null, 0, 100); // Adjust the period as needed
           


        }

        private void TimerCallback(object state)
        {
            Dispatcher.Invoke(() => UpdateRedLine());
        }

        private void UpdateRedLinePosition()
        {


            double currentTimeInSeconds = outputDevice.GetPositionTimeSpan().TotalSeconds;

            // Convert playback position to pixel position
            int horizontalPosition = (int)((currentTimeInSeconds / totalTime.TotalSeconds) * waveFormRendererWidth);

            redLine.X1 = horizontalPosition;
            redLine.X2 = horizontalPosition;
            
        }

        private void UpdateRedLine()
        {
            if(playing) 
            {
                currentPosition = outputDevice.GetPositionTimeSpan();
                UpdateRedLinePosition();
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
                audioFile = new AudioFileReader(audioFilePath);
                outputDevice.Init(audioFile);
                StandardWaveFormRendererSettings myRendererSettings = new StandardWaveFormRendererSettings();
                myRendererSettings.Width = (int)waveImage.Width;
                myRendererSettings.TopHeight = 96;
                myRendererSettings.BottomHeight = 96;
                myRendererSettings.PixelsPerPeak = 1;
                waveFormRendererWidth = myRendererSettings.Width;
                AveragePeakProvider averagePeakProvider = new AveragePeakProvider(4);

                WaveFormRenderer renderer = new WaveFormRenderer();
                using (var waveStream = new AudioFileReader(audioFilePath))
                {
                        
                   
                    totalTime = waveStream.TotalTime;
                    //waveImage.Source = ImageSourceFromBitmap((Bitmap)renderer.Render(waveStream,averagePeakProvider, myRendererSettings));
                    waveImage.Source = ConvertImageToBitmapImage(renderer.Render(waveStream, averagePeakProvider, myRendererSettings));
                }
            }
            outputDevice.Play();
            playing = true;

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
            audioFile.Dispose();
            audioFile = null;
            playing = false;
        }

        [DllImport("gdi32.dll", EntryPoint = "DeleteObject")]
        [return: MarshalAs(UnmanagedType.Bool)]
        public static extern bool DeleteObject([In] IntPtr hObject);

        public ImageSource ImageSourceFromBitmap(Bitmap bmp)
        {
            var handle = bmp.GetHbitmap();
            try
            {
                return Imaging.CreateBitmapSourceFromHBitmap(handle, IntPtr.Zero, Int32Rect.Empty, BitmapSizeOptions.FromEmptyOptions());
            }
            finally { DeleteObject(handle); }
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
    }


}
