using System;
using System.Runtime.InteropServices;

namespace FFMSsharp
{
    #region Interop

    struct FFMS_TrackTimeBase
    {
        public long Num;
        public long Den;
    }

    static partial class Interop
    {
        [DllImport("ffms2.dll", SetLastError = false, CharSet = CharSet.Ansi)]
        public static extern IntPtr FFMS_GetTimeBase(IntPtr T);

        [DllImport("ffms2.dll", SetLastError = false, CharSet = CharSet.Ansi)]
        public static extern int FFMS_GetTrackType(IntPtr T);

        [DllImport("ffms2.dll", SetLastError = false, CharSet = CharSet.Ansi)]
        public static extern int FFMS_GetNumFrames(IntPtr T);

        [DllImport("ffms2.dll", SetLastError = false, CharSet = CharSet.Ansi)]
        public static extern IntPtr FFMS_GetFrameInfo(IntPtr T, int Frame);

        [DllImport("ffms2.dll", SetLastError = false, CharSet = CharSet.Ansi)]
        public static extern int FFMS_WriteTimecodes(IntPtr T, string TimecodeFile, ref FFMS_ErrorInfo ErrorInfo);
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
    /// <para>See <see cref="Index.GetTrack">Index.GetTrack</see>, <see cref="VideoSource.GetTrack">VideoSource.GetTrack</see> or <see cref="AudioSource.GetTrack">AudioSource.GetTrack</see> on how to create a <see cref="Track">Track object</see>.</para>
    /// </remarks>
    public class Track
    {
        #region Private properties

        IntPtr FFMS_Track;
        FFMS_TrackTimeBase TrackTimeBase;
        TrackType TrackType;

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
        /// The <see cref="TrackType"/> of the track
        /// </summary>
        /// <remarks>
        /// <para>In FFMS2, the equivalent is <c>FFMS_GetTrackType</c>.</para>
        /// </remarks>
        public TrackType Type
        { get { return TrackType; } }

        #endregion

        #region Constructor

        internal Track(IntPtr Track)
        {
            FFMS_Track = Track;
            IntPtr propPtr = Interop.FFMS_GetTimeBase(Track);
            TrackTimeBase = (FFMS_TrackTimeBase)Marshal.PtrToStructure(propPtr, typeof(FFMS_TrackTimeBase));
            TrackType = (TrackType)Interop.FFMS_GetTrackType(FFMS_Track);
        }

        #endregion

        #region Methods

        /// <summary>
        /// The number of frames in the track
        /// </summary>
        /// <remarks>
        /// <para>In FFMS2, the equivalent is <c>FFMS_GetNumFrames</c>.</para>
        /// <para>For a video track this is the number of video frames, for an audio track it's the number of packets.</para>
        /// <para>A return value of 0 indicates the track has not been indexed.</para>
        /// </remarks>
        /// <returns>Number of frames</returns>
        public int GetNumFrames()
        {
            return Interop.FFMS_GetNumFrames(FFMS_Track);
        }

        /// <summary>
        /// Writes timecodes for the track to disk
        /// </summary>
        /// <remarks>
        /// <para>In FFMS2, the equivalent is <c>FFMS_WriteTimecodes</c>.</para>
        /// <para>Writes Matroska v2 timecodes for the track to the given file.</para>
        /// <para>Only meaningful for video tracks. </para>
        /// </remarks>
        /// <param name="TimecodeFile">Can be a relative or absolute path. The file will be truncated and overwritten if it already exists.</param>
        /// <exception cref="FFMSException"/>
        public void WriteTimecodes(string TimecodeFile)
        {
            FFMS_ErrorInfo err = new FFMS_ErrorInfo();
            err.BufferSize = 1024;
            err.Buffer = new String((char)0, 1024);

            if (Interop.FFMS_WriteTimecodes(FFMS_Track, TimecodeFile, ref err) != 0)
                throw ErrorHandling.ExceptionFromErrorInfo(err);
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
        /// <param name="Frame">Frame number</param>
        /// <returns>The generated <see cref="FrameInfo">FrameInfo object</see>.</returns>
        public FrameInfo GetFrameInfo(int Frame)
        {
            if (TrackType != TrackType.Video)
                throw new Exception("You can only use this function on video tracks.");

            IntPtr FrameInfoPtr = Interop.FFMS_GetFrameInfo(FFMS_Track, Frame);

            return new FrameInfo((FFMS_FrameInfo)Marshal.PtrToStructure(FrameInfoPtr, typeof(FFMS_FrameInfo)));
        }

        #endregion
    }
}
