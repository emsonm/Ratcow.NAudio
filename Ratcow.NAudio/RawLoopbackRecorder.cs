using System;
using NAudio.Wave;

namespace Ratcow.NAudio
{
    /// <summary>
    ///  Loopback recorder that presents all data to events
    ///  This is the recorder in its simplest form. The data can 
    ///  then be presented to another process or stream for
    ///  encoding.
    /// 
    ///  Based on the code from this article: 
    ///   * NAudio Loopback Record (record what you hear through the speaker)
    ///   * http://www.blakepell.com/2013-07-26-naudio-loopback-record-what-you-hear-through-the-speaker
    /// </summary>
    public class RawLoopbackRecorder
    {
        public IWaveIn WaveIn { get; private set; }
        public bool IsRecording { get; private set; }

        public RawLoopbackRecorder()
        {
            WaveIn = new WasapiLoopbackCapture();
        }

        public bool StartRecording()
        {
            // If we are currently record then go ahead and exit out.
            if (!IsRecording == true)
            {
                WaveIn.DataAvailable += OnDataAvailable;
                WaveIn.RecordingStopped += OnRecordingStopped;
                WaveIn.StartRecording();
                IsRecording = true;

                return true;
            }
            return false;
        }

        public void StopRecording()
        {
            if (WaveIn == null)
            {
                return;
            }

            WaveIn.StopRecording();
        }


        void OnRecordingStopped(object sender, StoppedEventArgs e)
        {
            RecordingStopped?.Invoke(this, e);

            if (WaveIn != null)
            {
                WaveIn.Dispose();
                WaveIn = null;
            }

            IsRecording = false;

            if (e.Exception != null)
            {
                throw e.Exception;
            }
        }

        void OnDataAvailable(object sender, WaveInEventArgs e)
        {
            DataAvailable?.Invoke(this, e);
        }

        //
        // Summary:
        //     Indicates recorded data is available
        public event EventHandler<WaveInEventArgs> DataAvailable;
        //
        // Summary:
        //     Indicates that all recorded data has now been received.
        public event EventHandler<StoppedEventArgs> RecordingStopped;
    }
}


