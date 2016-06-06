using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NAudio.Wave;

namespace VoicerClient.Sound
{
    public class Audio : IDisposable
    {
        public static readonly int BYTES_PER_100MS = 8820;
        protected static WaveFormat waveFormat;
        protected static double Threshhold;

        protected WaveIn sourceStream;
        protected DirectSoundOut waveOut;
        protected BufferedWaveProvider audioBufferProvider;

        private EventHandler<WaveInEventArgs> customDataRecievedEvent;

        private bool isRecording = false;
        private bool isPlaying = false;

        public bool IsRecording
        {
            get { return this.isRecording; }
        }

        public bool IsPlaying
        {
            get { return this.isPlaying; }
        }
        

        static Audio()
        {
            waveFormat = new WaveFormat(44100, 16, 1);
            Threshhold = 0.001;
        }

        public Audio()
        {
            customDataRecievedEvent = null;
        }

        static public void SetThreashhold(double value)
        {
            if (value > 0)
                Threshhold = value;
        }

        public WaveIn StartRecording(EventHandler<WaveInEventArgs> dataIn = null)
        {
            if (isRecording)
                return null;

            sourceStream = new WaveIn();
            sourceStream.WaveFormat = waveFormat;

            sourceStream.BufferMilliseconds = 50;
            sourceStream.NumberOfBuffers = 3;

            sourceStream.DataAvailable += new EventHandler<WaveInEventArgs>(sourceStream_DataAvailable);
            if (dataIn != null)
                customDataRecievedEvent = dataIn;

            sourceStream.StartRecording();
            isRecording = true;

            return sourceStream;
        }

        public void StopRecording()
        {
            if (isRecording)
            {
                sourceStream.StopRecording();
                sourceStream.Dispose();
                sourceStream = null;
                isRecording = false;
            }
        }

        // Start playing sound from the buffer
        public void StartSound()
        {
            if (isPlaying)
                return;

            audioBufferProvider = new BufferedWaveProvider(waveFormat);
            audioBufferProvider.BufferDuration = TimeSpan.FromMilliseconds(1000);

            waveOut = new DirectSoundOut();
            waveOut.Init(audioBufferProvider);
            waveOut.Play();
            isPlaying = true;
        }

        // Stop playing buffer sounds
        public void StopSound(int channel = -1)
        {
            if (isPlaying)
            {
                waveOut.Stop();
                waveOut.Dispose();
                waveOut = null;

                audioBufferProvider.ClearBuffer();
                audioBufferProvider = null;
                isPlaying = false;
            }
        }

        //Add sounds to buffer as a byte array.
        public void AddSound(byte[] buffer)
        {
            if (isPlaying)
            {
                audioBufferProvider.AddSamples(buffer, 0, buffer.Length);
            }
        }


        // Default event for sound capture, automatically adds sounds to buffer
        private void sourceStream_DataAvailable(object sender, WaveInEventArgs e)
        {
            // Noise Reduction
            double maxAbs = 0;

            for (int i = 0; i < e.BytesRecorded/4; i += 4)
            {
                short sample16Bit = BitConverter.ToInt16(e.Buffer, i);
                double volume = Math.Abs(sample16Bit / 32768.0);

                if (volume > maxAbs)
                {
                    //Console.WriteLine(volume);
                    maxAbs = volume;
                }
            }

            if (maxAbs < Threshhold)
            {
                //Console.WriteLine("Supressed");
                return;
            }

            if (customDataRecievedEvent == null)
            {
                //if (isRecording)
                //    AddSound(e.Buffer);
            }
            else customDataRecievedEvent.Invoke(sender, e);
        }

        public void Dispose()
        {
            StopRecording();
            StopSound();
            customDataRecievedEvent = null;
        }
    }
}
