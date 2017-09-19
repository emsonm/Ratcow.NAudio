using NAudio.Lame;
using System.IO;

namespace Ratcow.NAudio
{
    /// <summary>
    ///  This builds on the WavLoopbackRecorder and creates a workalike which records 
    ///  directly to MP3 using Lame. This uses NAudio.Lame to encode the WAV strem to MP3.
    ///  
    ///  Set Preset to another value to get differing quality/size.
    ///  Pass an ID3TagData to "start recording" to add the ID3 tags to the file.
    ///  
    ///  Usage:
    ///    var l = new MP3LoopbackRecorder();
    ///    l.StartRecording("audio");
    ///    ...
    ///    l.StopRecording();
    ///    
    ///   TODO: Implement IDisposable.
    ///         Create a common interface.
    /// </summary>
    public class MP3LoopbackRecorder
    {
        RawLoopbackRecorder recorder;
        LameMP3FileWriter writer;

        public bool IsRecording { get { return recorder != null && recorder.IsRecording; } }
        public LAMEPreset Preset { get; set; }

        public MP3LoopbackRecorder()
        {
            Preset = LAMEPreset.STANDARD;
        }

        public void StartRecording(string filename, ID3TagData id3data = null)
        {
            filename = Path.ChangeExtension(filename, ".mp3");

            recorder = new RawLoopbackRecorder();
            writer = new LameMP3FileWriter(filename, recorder.WaveIn.WaveFormat, Preset, id3data);

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


