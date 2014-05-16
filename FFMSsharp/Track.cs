using System;
using System.Runtime.InteropServices;

namespace FFMSSharp
{
    #region Interop

    struct FFMS_TrackTimeBase
    {
        public long Num;
        public long Den;
    }

    static partial class NativeMethods
    {
        [DllImport("ffms2.dll", SetLastError = false)]
        public static extern IntPtr FFMS_GetTimeBase(IntPtr T);

        [DllImport("ffms2.dll", SetLastError = false)]
        public static extern int FFMS_GetTrackType(IntPtr T);

        [DllImport("ffms2.dll", SetLastError = false)]
        public static extern int FFMS_GetNumFrames(IntPtr T);

        [DllImport("ffms2.dll", SetLastError = false)]
        public static extern IntPtr FFMS_GetFrameInfo(IntPtr T, int Frame);

        [DllImport("ffms2.dll", SetLastError = false)]
        public static extern int FFMS_WriteTimecodes(IntPtr T, byte[] TimecodeFile, ref FFMS_ErrorInfo ErrorInfo);
    }

    #endregion

    #region Constants

    /// <summary>
    /// Used for determining the type of a given track
    /// </summary>
    /// <remarks>
    /// <para>In FFMS2, the equivalent is <c>FFMS_TrackType</c>.</para>
    /// <para>Note that there are currently no functions to handle any type of track other than <see cref="Video" /> and <see cref="Audio"/>.</para>
    /// </remarks>
    public enum TrackType
    {
        /// <summary>
        /// Unknown
        /// </summary>
        /// <remarks>
        /// <para>In FFMS2, the equivalent is <c>FFMS_TYPE_UNKNOWN</c>.</para>
        /// </remarks>
        Unknown = -1,
        /// <summary>
        /// Video
        /// </summary>
        /// <remarks>
        /// <para>In FFMS2, the equivalent is <c>FFMS_TYPE_VIDEO</c>.</para>
        /// </remarks>
        Video,
        /// <summary>
        /// Audio
        /// </summary>
        /// <remarks>
        /// <para>In FFMS2, the equivalent is <c>FFMS_TYPE_AUDIO</c>.</para>
        /// </remarks>
        Audio,
        /// <summary>
        /// Data
        /// </summary>
        /// <remarks>
        /// <para>In FFMS2, the equivalent is <c>FFMS_TYPE_DATA</c>.</para>
        /// </remarks>
        Data,
        /// <summary>
        /// Subtitle
        /// </summary>
        /// <remarks>
        /// <para>In FFMS2, the equivalent is <c>FFMS_TYPE_SUBTITLE</c>.</para>
        /// </remarks>
        Subtitle,
        /// <summary>
        /// Attachment
        /// </summary>
        /// <remarks>
        /// <para>In FFMS2, the equivalent is <c>FFMS_TYPE_ATTACHMENT</c>.</para>
        /// </remarks>
        Attachment
    }

    #endregion

    /// <summary>
    /// A track of media
    /// </summary>
    /// <remarks>
    /// <para>In FFMS2, the equivalent is <c>FFMS_Track</c>.</para>
    /// <para>See <see cref="Index.GetTrack">Index.GetTrack</see>, <see cref="VideoSource.Track">VideoSource.Track</see> or <see cref="AudioSource.Track">AudioSource.Track</see> on how to create a <see cref="Track">Track object</see>.</para>
    /// </remarks>
    public class Track
    {
        #region Private properties

        IntPtr FFMS_Track;
        FFMS_TrackTimeBase TrackTimeBase;

        #endregion

        #region Accessors

        /// <summary>
        /// The basic time unit of the track
        /// </summary>
        /// <remarks>
        /// <para>In FFMS2, the equivalent is <c>FFMS_TrackTimeBase</c> and <c>FFMS_GetTimeBase</c>.</para>
        /// <para>Only meaningful for video tracks.</para>
        /// <para>Note that while this rational number may occasionally turn out to be equal to 1/framerate for some CFR video tracks, it really has no relation whatsoever with the video framerate and you should definitely not assume anything framerate-related based on it.</para>
        /// </remarks>
        /// <seealso cref="TimeBaseDenominator"/>
        public long TimeBaseNumerator
        { get { return TrackTimeBase.Num; } }

        /// <summary>
        /// The basic time unit of the track
        /// </summary>
        /// <remarks>
        /// <para>In FFMS2, the equivalent is <c>FFMS_TrackTimeBase</c> and <c>FFMS_GetTimeBase</c>.</para>
        /// <para>Only meaningful for video tracks.</para>
        /// <para>Note that while this rational number may occasionally turn out to be equal to 1/framerate for some CFR video tracks, it really has no relation whatsoever with the video framerate and you should definitely not assume anything framerate-related based on it.</para>
        /// </remarks>
        /// <seealso cref="TimeBaseNumerator"/>
        public long TimeBaseDenominator
        { get { return TrackTimeBase.Den; } }

        /// <summary>
        /// The type of the track
        /// </summary>
        /// <remarks>
        /// <para>In FFMS2, the equivalent is <c>FFMS_GetTrackType</c>.</para>
        /// </remarks>
        public TrackType TrackType
        { get { return (TrackType)NativeMethods.FFMS_GetTrackType(FFMS_Track); } }

        /// <summary>
        /// The number of frames in the track
        /// </summary>
        /// <remarks>
        /// <para>In FFMS2, the equivalent is <c>FFMS_GetNumFrames</c>.</para>
        /// <para>For a video track this is the number of video frames, for an audio track it's the number of packets.</para>
        /// <para>A return value of 0 indicates the track has not been indexed.</para>
        /// </remarks>
        public int NumberOfFrames
        { get { return NativeMethods.FFMS_GetNumFrames(FFMS_Track); } }

        #endregion

        #region Constructor

        internal Track(IntPtr Track)
        {
            FFMS_Track = Track;
            IntPtr propPtr = NativeMethods.FFMS_GetTimeBase(Track);
            TrackTimeBase = (FFMS_TrackTimeBase)Marshal.PtrToStructure(propPtr, typeof(FFMS_TrackTimeBase));
        }

        #endregion

        #region Methods

        /// <summary>
        /// Writes timecodes for the track to disk
        /// </summary>
        /// <remarks>
        /// <para>In FFMS2, the equivalent is <c>FFMS_WriteTimecodes</c>.</para>
        /// <para>Writes Matroska v2 timecodes for the track to the given file.</para>
        /// <para>Only meaningful for video tracks. </para>
        /// </remarks>
        /// <param name="timecodeFile">Can be a relative or absolute path. The file will be truncated and overwritten if it already exists.</param>
        /// <exception cref="System.IO.IOException">Failure to open or write to the file</exception>
        public void WriteTimecodes(string timecodeFile)
        {
            if (timecodeFile == null)
                throw new ArgumentNullException("timecodeFile");

            FFMS_ErrorInfo err = new FFMS_ErrorInfo();
            err.BufferSize = 1024;
            err.Buffer = new String((char)0, 1024);

            byte[] TimecodeFile = new byte[timecodeFile.Length];
            TimecodeFile = System.Text.Encoding.UTF8.GetBytes(timecodeFile);
            if (NativeMethods.FFMS_WriteTimecodes(FFMS_Track, TimecodeFile, ref err) != 0)
            {
                if (err.ErrorType == FFMS_Errors.FFMS_ERROR_PARSER && err.SubType == FFMS_Errors.FFMS_ERROR_FILE_READ) // FFMS2 2.19 throws this type of error
                    throw new System.IO.IOException(err.Buffer);
                if (err.ErrorType == FFMS_Errors.FFMS_ERROR_TRACK && err.SubType == FFMS_Errors.FFMS_ERROR_NO_FILE)
                    throw new System.IO.IOException(err.Buffer);
                if (err.ErrorType == FFMS_Errors.FFMS_ERROR_TRACK && err.SubType == FFMS_Errors.FFMS_ERROR_FILE_WRITE)
                    throw new System.IO.IOException(err.Buffer);

                throw new NotImplementedException(string.Format(System.Globalization.CultureInfo.CurrentCulture, "Unknown FFMS2 error encountered: ({0}, {1}, '{2}'). Please report this issue on FFMSSharp's GitHub.", err.ErrorType, err.SubType, err.Buffer));
            }
        }

        #endregion

        #region Object creation

        /// <summary>
        /// Gets information about a specific frame
        /// </summary>
        /// <remarks>
        /// <para>In FFMS2, the equivalent is <c>FFMS_GetFrameInfo</c>.</para>
        /// <para>Gets information about the given frame (identified by its frame number) from the indexing information in the <see cref="Track">Track object</see> and returns it as a <see cref="FrameInfo">FrameInfo object</see>.</para>
        /// </remarks>
        /// <param name="frame">Frame number</param>
        /// <returns>The generated <see cref="FrameInfo">FrameInfo object</see>.</returns>
        public FrameInfo GetFrameInfo(int frame)
        {
            if (TrackType != TrackType.Video)
                throw new InvalidOperationException("You can only use this function on video tracks.");

            IntPtr FrameInfoPtr = NativeMethods.FFMS_GetFrameInfo(FFMS_Track, frame);

            return new FrameInfo((FFMS_FrameInfo)Marshal.PtrToStructure(FrameInfoPtr, typeof(FFMS_FrameInfo)));
        }

        #endregion
    }
}
