namespace FFMSSharp
{
    #region Interop

    struct FFMS_FrameInfo
    {
        public long PTS;
        public int RepeatPict;
        public int KeyFrame;
    }

    #endregion

    /// <summary>
    /// Information about a single video frame
    /// </summary>
    /// <remarks>
    /// <para>In FFMS2, the equivalent is <c>FFMS_FrameInfo</c>.</para>
    /// <para>See <see cref="Track.GetFrameInfo">Track.GetFrameInfo</see> on how to create a <see cref="FrameInfo">FrameInfo object</see>.</para>
    /// </remarks>
    public class FrameInfo
    {
        FFMS_FrameInfo Info;

        #region Accessors

        /// <summary>
        /// The decoding timestamp of the frame
        /// </summary>
        /// <remarks>
        /// <para>In FFMS2, the equivalent is <c>FFMS_FrameInfo->PTS</c>.</para>
        /// <para>To convert this to a timestamp in wallclock milliseconds, use the relation long timestamp = (long)((<see cref="FrameInfo.PTS"/> * <see cref="Track.TimeBaseNumerator">Track.TimeBase.Numerator</see>) / (double)<see cref="Track.TimeBaseDenominator">Track.TimeBase.Denumerator</see>).</para>
        /// </remarks>
        public long PTS
        { get { return Info.PTS; } }
        /// <summary>
        /// RFF flag for the frame
        /// </summary>
        /// <remarks>
        /// <para>In FFMS2, the equivalent is <c>FFMS_FrameInfo->RepeatPict</c>.</para>
        /// </remarks>
        /// <seealso cref="Frame.RepeatPicture"/>
        public int RepeatPicture
        { get { return Info.RepeatPict; } }
        /// <summary>
        /// Is this a keyframe?
        /// </summary>
        /// <remarks>
        /// <para>In FFMS2, the equivalent is <c>FFMS_FrameInfo->KeyFrame</c>.</para>
        /// </remarks>
        public bool KeyFrame
        { get { return Info.KeyFrame != 0; } }

        #endregion

        #region Constructor

        internal FrameInfo(FFMS_FrameInfo FrameInfo)
        {
            this.Info = FrameInfo;
        }

        #endregion
    }
}
