using NAudio.Wave;
using System.IO;

namespace Ratcow.NAudio
{
    /// <summary>
    ///  Takes the RawLoopbackRecorder and composes it in to a recorder that puts the 
    ///  stream to a WAV file. This is more or less what the example in the article 
    ///  that the RawLoopbackRecorder is based on did. We do some simplification of the 
    ///  set-up to make the code more compact.
    /// </summary>
    public class WavLoopbackRecorder
    {
        RawLoopbackRecorder recorder;
        WaveFileWriter writer;

        public bool IsRecording { get { return recorder != null && recorder.IsRecording; } }

        public void StartRecording(string filename)
        {
            filename = Path.ChangeExtension(filename, ".wav");

            recorder = new RawLoopbackRecorder();
            writer = new WaveFileWriter(filename, recorder.WaveIn.WaveFormat);

            recorder.RecordingStopped += (s, e) =>
            {
                writer?.Close();
            };

            recorder.DataAvailable += (s, e) =>
            {
                writer?.Write(e.Buffer, 0, e.BytesRecorded);
            };

            recorder.StartRecording();
        }


        public void StopRecording()
        {
            recorder.StopRecording();
        }
    }
}


