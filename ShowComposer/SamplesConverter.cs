using NAudio.Wave;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ShowComposer
{
    [Obsolete]
    class SamplesConverter
    {
        private byte[] data;

        public byte[] Data
        {
            get { return data; }
            set { data = value; }
        }
        private int[] amplitudes;

        public int[] Amplitudes
        {
            get { return amplitudes; }
            set { amplitudes = value; }
        }
        private float[] samples;

        private void ReadSamples(WaveStream waveStream)
        {
            if (data == null)
            {
                data = new byte[(int)waveStream.Length];
                waveStream.Read(data, 0, (int)waveStream.Length);
            }
        }


        /// <summary>
        /// Get the amplitudes of the wave samples (depends on the header).
        /// </summary>
        /// <returns>Array of amplitudes.</returns>
        public Int32[] getSamplesAmplitudes(WaveStream waveStream, int bitsPerSample)
        {
            ReadSamples(waveStream);

            if (amplitudes == null)
            {
                byte[] buffer = data;

                int startIndex = 0;
                int length = buffer.Length;
                int sampleIndex = 0;

                if (bitsPerSample == 8)
                {
                    int size = bitsPerSample / 8;
                    amplitudes = new Int32[length / size];
                    var cl = length / size;
                    for (int i = 0; i < cl; i++)
                        amplitudes[i] = (buffer[i]);
                }
                else if (bitsPerSample == 16)
                {
                    int size = bitsPerSample / 8;
                    amplitudes = new Int32[length / size];
                    var cl = amplitudes.Length;

                    for (; sampleIndex < cl; startIndex += size, sampleIndex++)
                        amplitudes[sampleIndex] = BitConverter.ToInt16(buffer, startIndex);
                }
                else if (bitsPerSample == 24)
                {
                    int size = bitsPerSample / 8;

                    amplitudes = new Int32[length / size];
                    var cl = amplitudes.Length;

                    for (; sampleIndex < cl; startIndex += size, sampleIndex++)
                    {
                        amplitudes[sampleIndex] = ((buffer[startIndex + 2] << 16 | buffer[startIndex + 1] << 8) | buffer[startIndex]);
                        if (amplitudes[sampleIndex] > 8388607)
                            amplitudes[sampleIndex] -= 16777216;
                        if (amplitudes[sampleIndex] < -8388608)
                            amplitudes[sampleIndex] += 16777215;
                    }
                }
                else if (bitsPerSample == 32)
                {
                    int size = bitsPerSample / 8;
                    amplitudes = new Int32[length / size];
                    var cl = amplitudes.Length;

                    for (; sampleIndex < cl; startIndex += size, sampleIndex++)
                        amplitudes[sampleIndex] = BitConverter.ToInt32(buffer, startIndex);
                }
                else
                {
                    throw new ArgumentException("bitsPerSample");
                }
            }
            return amplitudes;
        }

        public float[] getSamples(WaveStream waveStream, int bitsPerSample)
        {
            if (samples == null)
            {
                Int32[] amps = getSamplesAmplitudes(waveStream, bitsPerSample);
                int length = amps.Length;
                samples = new float[length];

                if (bitsPerSample == 8)
                {
                    for (int i = 0; i < length; i++)
                        samples[i] = (amps[i] - 127) / 128f;
                }
                else if (bitsPerSample == 16)
                {
                    for (int i = 0; i < length; i++)
                        samples[i] = amps[i] / 32768f;
                }
                else if (bitsPerSample == 24)
                {
                    for (int i = 0; i < length; i++)
                        samples[i] = amps[i] / 8388608f;
                }
                else if (bitsPerSample == 32)
                {
                    for (int i = 0; i < length; i++)
                        samples[i] = amps[i] / 2147483648f;
                }
                else
                {
                    throw new ArgumentException("bitsPerSample");
                }
            }

            return samples;
        }
    }
}
