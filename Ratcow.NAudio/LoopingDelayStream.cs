using System;
using NAudio.Wave;
using System.IO;

namespace Ratcow.NAudio
{
    /// <summary>
    /// A simple extension to the LoopStream detailed in this article:
    /// 
    /// http://mark-dot-net.blogspot.co.uk/2009/10/looped-playback-in-net-with-naudio.html
    /// 
    /// This class will automatically loop the sample, adding a delay to the end. This is 
    /// very useful for a recorded announcement or a repetetive sound required to be played
    /// on a loop.
    /// 
    /// How it works: we basically do a bait and switch. We replace the original WaveStream
    /// we were passed with a wrapper around a MemoryStream that has padding shoved on the end.
    /// I have no idea if this will scale past short samples with 10 or so seconds delat, but
    /// that was my initial use case.
    /// </summary>
    public class LoopingDelayStream : WaveStream
    {
        MemoryStream sourceStream;
        readonly TimeSpan delay;
        readonly int delayLength = 0;
        readonly WaveFormat waveFormat;
        public bool EnableLooping { get; set; }

        public LoopingDelayStream(WaveStream originalSourceStream, TimeSpan delay)
        {
            this.EnableLooping = true;
            this.waveFormat = originalSourceStream.WaveFormat;

            //copy the data
            sourceStream = new MemoryStream();
            originalSourceStream.CopyTo(sourceStream);

            this.delay = delay;
            PadStreamWithSilence();
        }

        void PadStreamWithSilence()
        {
            var bytesMS = (waveFormat.AverageBytesPerSecond / 1000) * ((int)delay.TotalMilliseconds);

            Fill(sourceStream, 0, bytesMS);
            sourceStream.Position = 0;
        }

        /// <summary>
        /// Not sure this is absolutely optimal
        /// </summary>
        void Fill(Stream stream, byte value, int count)
        {
            var buffer = new byte[64];
            for (int i = 0; i < buffer.Length; i++)
            {
                buffer[i] = value;
            }
            while (count > buffer.Length)
            {
                stream.Write(buffer, 0, buffer.Length);
                count -= buffer.Length;
            }
            stream.Write(buffer, 0, count);
        }

        public override WaveFormat WaveFormat
        {
            get { return waveFormat; }
        }

        public override long Length
        {
            get { return sourceStream.Length; }
        }

        public override long Position
        {
            get { return sourceStream.Position; }
            set
            {
                sourceStream.Position = value;
            }
        }

        public override int Read(byte[] buffer, int offset, int count)
        {
            int totalBytesRead = 0;

            while (totalBytesRead < count)
            {
                int bytesRead = sourceStream.Read(buffer, offset + totalBytesRead, count - totalBytesRead);
                if (bytesRead == 0)
                {
                    if (sourceStream.Position == 0 || !EnableLooping)
                    {
                        // something wrong with the source stream
                        break;
                    }
                    // loop
                    sourceStream.Position = 0;
                }
                totalBytesRead += bytesRead;
            }
            return totalBytesRead;
        }
    }
}
