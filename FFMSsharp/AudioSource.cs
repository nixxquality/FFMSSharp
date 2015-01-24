using System;
using System.Diagnostics.CodeAnalysis;
using System.Runtime.InteropServices;

namespace FFMSSharp
{
    #region Interop

    // ReSharper disable once InconsistentNaming
    struct FFMS_AudioProperties
    {
        public int SampleFormat;
        public int SampleRate;
        public int BitsPerSample;
        public int Channels;
        public long ChannelLayout;
        public long NumSamples;
        public double FirstTime;
        public double LastTime;
    }

    [SuppressMessage("ReSharper", "InconsistentNaming")]
    static partial class NativeMethods
    {
        [DllImport("ffms2.dll", SetLastError = false)]
        public static extern IntPtr FFMS_GetAudioProperties(IntPtr A);

        [DllImport("ffms2.dll", SetLastError = false)]
        public static extern void FFMS_DestroyAudioSource(IntPtr A);

        [DllImport("ffms2.dll", SetLastError = false)]
        public static extern int FFMS_GetAudio(IntPtr A, byte[] Buf, long Start, long Count, ref FFMS_ErrorInfo ErrorInfo);

        [DllImport("ffms2.dll", SetLastError = false)]
        public static extern IntPtr FFMS_GetTrackFromAudio(IntPtr A);
    }

    #endregion

    #region Constants

    /// <summary>
    /// Identifies various audio sample formats
    /// </summary>
    /// <remarks>
    /// <para>In FFMS2, the equivalent is <c>FFMS_SampleFormat</c>.</para>
    /// </remarks>
    [SuppressMessage("ReSharper", "InconsistentNaming")]
    public enum SampleFormat
    {
        /// <summary>
        /// One 8-bit unsigned integer (uint8_t) per sample
        /// </summary>
        /// <remarks>
        /// <para>In FFMS2, the equivalent is <c>FFMS_FMT_U8</c>.</para>
        /// </remarks>
        uint8_t = 0,
        /// <summary>
        /// One 16-bit signed integer (int16_t) per sample
        /// </summary>
        /// <remarks>
        /// <para>In FFMS2, the equivalent is <c>FFMS_FMT_S16</c>.</para>
        /// </remarks>
        int16_t,
        /// <summary>
        /// One 32-bit signed integer (int32_t) per sample
        /// </summary>
        /// <remarks>
        /// <para>In FFMS2, the equivalent is <c>FFMS_FMT_S32</c>.</para>
        /// </remarks>
        int32_t,
        /// <summary>
        /// One 32-bit (single precision) floating point value (float_t) per sample
        /// </summary>
        /// <remarks>
        /// <para>In FFMS2, the equivalent is <c>FFMS_FMT_FLT</c>.</para>
        /// </remarks>
        float_t,
        /// <summary>
        /// One 64-bit (double precision) floating point value (double_t) per sample
        /// </summary>
        /// <remarks>
        /// <para>In FFMS2, the equivalent is <c>FFMS_FMT_DBL</c>.</para>
        /// </remarks>
        double_t
    }

    /// <summary>
    /// Describes the audio channel layout of an audio stream
    /// </summary>
    /// <remarks>
    /// <para>In FFMS2, the equivalent is <c>FFMS_AudioChannel</c>.</para>
    /// </remarks>
    [FlagsAttribute]
    public enum AudioChannels
    {
        /// <summary>
        /// Front Left
        /// </summary>
        /// <remarks>
        /// <para>In FFMS2, the equivalent is <c>FFMS_CH_FRONT_LEFT</c>.</para>
        /// </remarks>
        FrontLeft = 0x00000001,
        /// <summary>
        /// Front Right
        /// </summary>
        /// <remarks>
        /// <para>In FFMS2, the equivalent is <c>FFMS_CH_FRONT_RIGHT</c>.</para>
        /// </remarks>
        FrontRight = 0x00000002,
        /// <summary>
        /// Front Center
        /// </summary>
        /// <remarks>
        /// <para>In FFMS2, the equivalent is <c>FFMS_CH_FRONT_CENTER</c>.</para>
        /// </remarks>
        FrontCenter = 0x00000004,
        /// <summary>
        /// Low Frequency Effects
        /// </summary>
        /// <remarks>
        /// <para>In FFMS2, the equivalent is <c>FFMS_CH_LOW_FREQUENCE</c>.</para>
        /// </remarks>
        LowFrequency = 0x00000008,
        /// <summary>
        /// Back Left
        /// </summary>
        /// <remarks>
        /// <para>In FFMS2, the equivalent is <c>FFMS_CH_BACK_LEFT</c>.</para>
        /// </remarks>
        BackLeft = 0x00000010,
        /// <summary>
        /// Back Right
        /// </summary>
        /// <remarks>
        /// <para>In FFMS2, the equivalent is <c>FFMS_CH_BACK_RIGHT</c>.</para>
        /// </remarks>
        BackRight = 0x00000020,
        /// <summary>
        /// Front Left of Center
        /// </summary>
        /// <remarks>
        /// <para>In FFMS2, the equivalent is <c>FFMS_CH_FRONT_LEFT_OF_CENTER</c>.</para>
        /// </remarks>
        FrontLeftOfCenter = 0x00000040,
        /// <summary>
        /// Front Right of Center
        /// </summary>
        /// <remarks>
        /// <para>In FFMS2, the equivalent is <c>FFMS_CH_FRONT_RIGHT_OF_CENTER</c>.</para>
        /// </remarks>
        FrontRightOfCenter = 0x00000080,
        /// <summary>
        /// Back Center
        /// </summary>
        /// <remarks>
        /// <para>In FFMS2, the equivalent is <c>FFMS_CH_BACK_CENTER</c>.</para>
        /// </remarks>
        BackCenter = 0x00000100,
        /// <summary>
        /// Side Left
        /// </summary>
        /// <remarks>
        /// <para>In FFMS2, the equivalent is <c>FFMS_CH_SIDE_LEFT</c>.</para>
        /// </remarks>
        SideLeft = 0x00000200,
        /// <summary>
        /// Side Right
        /// </summary>
        /// <remarks>
        /// <para>In FFMS2, the equivalent is <c>FFMS_CH_SIDE_RIGHT</c>.</para>
        /// </remarks>
        SideRight = 0x00000400,
        /// <summary>
        /// Top Center
        /// </summary>
        /// <remarks>
        /// <para>In FFMS2, the equivalent is <c>FFMS_CH_TOP_CENTER</c>.</para>
        /// </remarks>
        TopCenter = 0x00000800,
        /// <summary>
        /// Top Front Left
        /// </summary>
        /// <remarks>
        /// <para>In FFMS2, the equivalent is <c>FFMS_CH_TOP_FRONT_LEFT</c>.</para>
        /// </remarks>
        TopFrontLeft = 0x00001000,
        /// <summary>
        /// Top Front Center
        /// </summary>
        /// <remarks>
        /// <para>In FFMS2, the equivalent is <c>FFMS_CH_TOP_FRONT_CENTER</c>.</para>
        /// </remarks>
        TopFrontCenter = 0x00002000,
        /// <summary>
        /// Top Front Right
        /// </summary>
        /// <remarks>
        /// <para>In FFMS2, the equivalent is <c>FFMS_CH_TOP_FRONT_RIGHT</c>.</para>
        /// </remarks>
        TopFrontRight = 0x00004000,
        /// <summary>
        /// Top Back Left
        /// </summary>
        /// <remarks>
        /// <para>In FFMS2, the equivalent is <c>FFMS_CH_TOP_BACK_LEFT</c>.</para>
        /// </remarks>
        TopBackLeft = 0x00008000,
        /// <summary>
        /// Top Back Center
        /// </summary>
        /// <remarks>
        /// <para>In FFMS2, the equivalent is <c>FFMS_CH_TOP_BACK_CENTER</c>.</para>
        /// </remarks>
        TopBackCenter = 0x00010000,
        /// <summary>
        /// Top Back Right
        /// </summary>
        /// <remarks>
        /// <para>In FFMS2, the equivalent is <c>FFMS_CH_TOP_BACK_RIGHT</c>.</para>
        /// </remarks>
        TopBackRight = 0x00020000,
        /// <summary>
        /// Stereo Left
        /// </summary>
        /// <remarks>
        /// <para>In FFMS2, the equivalent is <c>FFMS_CH_STEREO_LEFT</c>.</para>
        /// </remarks>
        StereoLeft = 0x20000000,
        /// <summary>
        /// Stereo Right
        /// </summary>
        /// <remarks>
        /// <para>In FFMS2, the equivalent is <c>FFMS_CH_STERO_RIGHT</c>.</para>
        /// </remarks>
        StereoRight = 0x40000000
    }

    #endregion

    /// <summary>
    /// Video source
    /// </summary>
    /// <remarks>
    /// <para>In FFMS2, the equivalent is <c>FFMS_AudioSource</c>.</para>
    /// <para>See <see cref="Index.AudioSource">Index.AudioSource</see> on how to create a <see cref="AudioSource">AudioSource object</see>.</para>
    /// <para>Note that there is no equivalent for FFMS2's <c>FFMS_GetAudioProperties</c> as it is called during construction of the <see cref="AudioSource">AudioSource object</see>.</para>
    /// </remarks>
    public class AudioSource
    {
        readonly IntPtr _nativePtr;
        FFMS_AudioProperties _ap;

        #region Properties

        /// <summary>
        /// Audio sample format
        /// </summary>
        /// <remarks>
        /// <para>In FFMS2, the equivalent is <c>FFMS_AudioProperties->SampleFormat</c>.</para>
        /// </remarks>
        public SampleFormat SampleFormat
        { get { return (SampleFormat)_ap.SampleFormat; } }

        /// <summary>
        /// Sample rate, in samples per second
        /// </summary>
        /// <remarks>
        /// <para>In FFMS2, the equivalent is <c>FFMS_AudioProperties->SampleRate</c>.</para>
        /// </remarks>
        public int SampleRate
        { get { return _ap.SampleRate; } }

        /// <summary>
        /// Bits per audio sample
        /// </summary>
        /// <remarks>
        /// <para>In FFMS2, the equivalent is <c>FFMS_AudioProperties->BitsPerSample</c>.</para>
        /// <para>Note that this signifies the number of bits actually used to code each sample, not the number of bits used to store each sample, and may hence be different from what the SampleFormat would imply.</para>
        /// <para>Figuring out which bytes are significant and which aren't is left as an exercise for the reader.</para>
        /// </remarks>
        public int BitsPerSample
        { get { return _ap.BitsPerSample; } }

        /// <summary>
        /// The number of audio channels
        /// </summary>
        /// <remarks>
        /// <para>In FFMS2, the equivalent is <c>FFMS_AudioProperties->Channels</c>.</para>
        /// </remarks>
        public int Channels
        { get { return _ap.Channels; } }

        /// <summary>
        /// The channel layout of the audio stream
        /// </summary>
        /// <remarks>
        /// <para>In FFMS2, the equivalent is <c>FFMS_AudioProperties->ChannelLayout</c>.</para>
        /// <para>Constructed by binary OR'ing the relevant integers from <see cref="ChannelLayout"/> together, which means that if the audio has the channel AudioChannel.Example, the operation (ChannelOrder &amp; AudioChannel.Example) will evaluate to true.</para>
        /// </remarks>
        public long ChannelLayout
        { get { return _ap.ChannelLayout; } }

        /// <summary>
        /// The number of samples in the audio track
        /// </summary>
        /// <remarks>
        /// <para>In FFMS2, the equivalent is <c>FFMS_AudioProperties->NumSamples</c>.</para>
        /// </remarks>
        public long NumberOfSamples
        { get { return _ap.NumSamples; } }

        /// <summary>
        /// The first timestamp of the stream, in seconds
        /// </summary>
        /// <remarks>
        /// <para>In FFMS2, the equivalent is <c>FFMS_AudioProperties->FirstTime</c>.</para>
        /// <para>Useful if you want to know if the stream has a delay, or for quickly determining its length in seconds.</para>
        /// </remarks>
        /// <seealso cref="LastTime"/>
        public double FirstTime
        { get { return _ap.FirstTime; } }

        /// <summary>
        /// The last timestamp of the stream, in seconds
        /// </summary>
        /// <remarks>
        /// <para>In FFMS2, the equivalent is <c>FFMS_AudioProperties->LastTime</c>.</para>
        /// <para>Useful if you want to know if the stream has a delay, or for quickly determining its length in seconds.</para>
        /// </remarks>
        /// <seealso cref="FirstTime"/>
        public double LastTime
        { get { return _ap.LastTime; } }

        Track _track;

        /// <summary>
        /// Retrieves track info
        /// </summary>
        /// <remarks>
        /// <para>In FFMS2, the equivalent is <c>FFMS_GetTrackFromAudio</c>.</para>
        /// <para>It's generally safer to use this instead of <see cref="Index.GetTrack">Index.GetTrack</see>, since unlike that function it cannot cause access violations if you specified an nonexistent track number, return a <see cref="Track">Track object</see> that doesn't actually contain any indexing information, or return an object that ceases to be valid when the index is destroyed.</para>
        /// <para>Note that the <see cref="Track">Track object</see> is only valid until its parent <see cref="AudioSource">AudioSource object</see> is destroyed.</para>
        /// </remarks>
        public Track Track
        {
            get
            {
                if (_track != null) return _track;

                var trackPtr = NativeMethods.FFMS_GetTrackFromAudio(_nativePtr);
                _track = new Track(trackPtr);

                return _track;
            }
        }
        

        #endregion

        #region Constructor and destructor

        internal AudioSource(IntPtr audioSource)
        {
            _nativePtr = audioSource;
            var propPtr = NativeMethods.FFMS_GetAudioProperties(audioSource);
            _ap = (FFMS_AudioProperties)Marshal.PtrToStructure(propPtr, typeof(FFMS_AudioProperties));
        }

        /// <summary>
        /// Audio source destructor
        /// </summary>
        /// <remarks>
        /// <para>In FFMS2, the equivalent is <c>FFMS_DestroyAudioSource</c>.</para>
        /// </remarks>
        ~AudioSource()
        {
            NativeMethods.FFMS_DestroyAudioSource(_nativePtr);
        }

        #endregion

        #region Methods

        /// <summary>
        /// Decode a number of audio samples
        /// </summary>
        /// <remarks>
        /// <para>In FFMS2, the equivalent is <c>FFMS_GetAudio</c>.</para>
        /// <para>The output is <paramref name="count"/> samples long, starting from <paramref name="start"/> (inclusive).</para>
        /// </remarks>
        /// <param name="start">The first sample to decode
        /// <para>Sample numbers start from zero and hence the last sample in the stream is number <see cref="NumberOfSamples"/> minus 1.</para>
        /// </param>
        /// <param name="count">The amount of samples to decode
        /// <para>Sample numbers start from zero and hence the last sample in the stream is number <see cref="NumberOfSamples"/> minus 1.</para>
        /// </param>
        /// <returns>The raw audio data</returns>
        /// <threadsafety instance="false"/>
        /// <exception cref="ArgumentOutOfRangeException">Trying to access audio samples that are out of range of the stream.</exception>
        /// <exception cref="NotSupportedException">Trying to start half-way into an unseekable audio stream.</exception>
        public byte[] GetAudio(long start, long count)
        {
            if (start < 0 || start > NumberOfSamples - 1)
                throw new ArgumentOutOfRangeException(@"start", "Invalid start sample.");
            if (count < 0 || start + count > NumberOfSamples - 1)
                throw new ArgumentOutOfRangeException(@"count", "Invalid sample count.");

            var err = new FFMS_ErrorInfo
            {
                BufferSize = 1024,
                Buffer = new String((char) 0, 1024)
            };

            var buffer = new byte[(_ap.BitsPerSample / 8) * _ap.Channels * count];

            int success;
            lock (this)
            {
                success = NativeMethods.FFMS_GetAudio(_nativePtr, buffer, start, count, ref err);
            }

            if (success == 0) return buffer;

            if (err.ErrorType == FFMS_Errors.FFMS_ERROR_SEEKING && err.SubType == FFMS_Errors.FFMS_ERROR_CODEC)
                throw new NotSupportedException(err.Buffer);

            throw new NotImplementedException(string.Format(System.Globalization.CultureInfo.CurrentCulture, "Unknown FFMS2 error encountered: ({0}, {1}, '{2}'). Please report this issue on FFMSSharp's GitHub.", err.ErrorType, err.SubType, err.Buffer));
        }

        #endregion
    }
}
