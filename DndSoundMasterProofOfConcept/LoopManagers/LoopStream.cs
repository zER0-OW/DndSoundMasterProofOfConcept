using NAudio.Wave;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DndSoundMasterProofOfConcept.LoopManagers
{
    /// <summary>
    /// Stream for looping playback
    /// </summary>
    /// <summary>
    /// Stream for looping playback
    /// </summary>
    public class LoopStream : WaveStream
    {
        
        WaveStream sourceStream;

        /// <summary>
        /// Creates a new Loop stream
        /// </summary>
        /// <param name="sourceStream">The stream to read from. Note: the Read method of this stream should return 0 when it reaches the end
        /// or else we will not loop to the start again.</param>
        public LoopStream(WaveStream sourceStream, LoopSettings loopSettings)
        {
            LoopSettings settings = loopSettings;
            this.sourceStream = sourceStream;
            this.EnableLooping = true;
            defaultLoop = new Loop (new TimeSpan (0,0,0), sourceStream.TotalTime);
            if(settings.loopList.Count > 0) 
            {
                currentLoop = settings.loopList[0];
            }
            else
                currentLoop = defaultLoop;
           


        }

        public LoopSettings Settings { get; set; }

        public Loop defaultLoop;

        public Loop currentLoop { get; set; }
        /// <summary>
        /// Use this to turn looping on or off
        /// </summary>
        public bool EnableLooping { get; set; }

        /// <summary>
        /// Return source stream's wave format
        /// </summary>
        public override WaveFormat WaveFormat
        {
            get { return sourceStream.WaveFormat; }
        }

        /// <summary>
        /// LoopStream simply returns
        /// </summary>
        public override long Length
        {
            get { return sourceStream.Length; }
        }

        /// <summary>
        /// LoopStream simply passes on positioning to source stream
        /// </summary>
        public override long Position
        {
            get { return sourceStream.Position; }
            set { sourceStream.Position = value; }
        }

        public override int Read(byte[] buffer, int offset, int count)
        {
            if(currentLoop == null)
            {
                
            }
            int totalBytesRead = 0;

            while (totalBytesRead < count)
            {
                int bytesRead = sourceStream.Read(buffer, offset + totalBytesRead, count - totalBytesRead);

                if (EnableLooping)
                {
                    if (sourceStream.CurrentTime >= currentLoop.end)
                    {
                        // loop
                        sourceStream.CurrentTime = currentLoop.start;
                    }
                }


                totalBytesRead += bytesRead;
            }
            return totalBytesRead;

        }

        public void SetLoop(int index)
        {
            if(Settings != null)
            {
                if (Settings.loopList.Count > index)
                {
                    currentLoop = Settings.loopList[index];
                }
            }
        }
        
    }
}
