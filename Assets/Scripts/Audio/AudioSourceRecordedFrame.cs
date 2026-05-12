namespace Assets.Scripts.Audio
{
    public sealed class AudioSourceRecordedFrame
    {
        public float[] Data { get; private set; }
        public int Channels { get; private set; }
        public int SampleRate { get; private set; }

        public AudioSourceRecordedFrame(float[] data, int channels, int sampleRate)
        {
            Data = data;
            Channels = channels;
            SampleRate = sampleRate;
        }
    }
}
