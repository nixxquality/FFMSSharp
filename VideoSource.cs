using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;

namespace FFMSsharp
{
    #region Interop

    struct FFMS_VideoProperties
    {
        public int FPSDenominator;
        public int FPSNumerator;
        public int RFFDenominator;
        public int RFFNumerator;
        public int NumFrames;
        public int SARNum;
        public int SARDen;
        public int CropTop;
        public int CropBottom;
        public int CropLeft;
        public int CropRight;
        public int TopFieldFirst;
        public int ColorSpace;
        public int ColorRange;
        public double FirstTime;
        public double LastTime;
    }

    static partial class NativeMethods
    {
        [DllImport("ffms2.dll", SetLastError = false, CharSet = CharSet.Ansi)]
        public static extern IntPtr FFMS_GetVideoProperties(IntPtr V);

        [DllImport("ffms2.dll", SetLastError = false, CharSet = CharSet.Ansi)]
        public static extern void FFMS_DestroyVideoSource(IntPtr V);

        [DllImport("ffms2.dll", SetLastError = false, CharSet = CharSet.Ansi)]
        public static extern int FFMS_SetOutputFormatV2(IntPtr V, int[] TargetFormats, int Width, int Height, int Resizer, ref FFMS_ErrorInfo ErrorInfo);

        [DllImport("ffms2.dll", SetLastError = false, CharSet = CharSet.Ansi)]
        public static extern void FFMS_ResetOutputFormatV(IntPtr V);

        [DllImport("ffms2.dll", SetLastError = false, CharSet = CharSet.Ansi)]
        public static extern int FFMS_SetInputFormatV(IntPtr V, int ColorSpace, int ColorRange, int PixelFormat, ref FFMS_ErrorInfo ErrorInfo);

        [DllImport("ffms2.dll", SetLastError = false, CharSet = CharSet.Ansi)]
        public static extern void FFMS_ResetInputFormatV(IntPtr V);

        [DllImport("ffms2.dll", SetLastError = false, CharSet = CharSet.Ansi)]
        public static extern IntPtr FFMS_GetFrame(IntPtr V, int n, ref FFMS_ErrorInfo ErrorInfo);

        [DllImport("ffms2.dll", SetLastError = false, CharSet = CharSet.Ansi)]
        public static extern IntPtr FFMS_GetFrameByTime(IntPtr V, double Time, ref FFMS_ErrorInfo ErrorInfo);

        [DllImport("ffms2.dll", SetLastError = false, CharSet = CharSet.Ansi)]
        public static extern IntPtr FFMS_GetTrackFromVideo(IntPtr V);
    }

    #endregion

    #region Constants

    /// <summary>
    /// Describes various image resizing algorithms
    /// </summary>
    /// <remarks>
    /// <para>In FFMS2, the equivalent is <c>FFMS_Resizers</c>.</para>
    /// </remarks>
    public enum Resizers
    {
        /// <summary>
        /// Bilinear (Fast)
        /// </summary>
        /// <remarks>
        /// <para>In FFMS2, the equivalent is <c>FFMS_RESIZERS_FAST_BILINEAR</c>.</para>
        /// </remarks>
        BilinearFast = 0x01,
        /// <summary>
        /// Bilinear
        /// </summary>
        /// <remarks>
        /// <para>In FFMS2, the equivalent is <c>FFMS_RESIZERS_BILINEAR</c>.</para>
        /// </remarks>
        Bilinear = 0x02,
        /// <summary>
        /// Bicubic
        /// </summary>
        /// <remarks>
        /// <para>In FFMS2, the equivalent is <c>FFMS_RESIZERS_BICUBIC</c>.</para>
        /// </remarks>
        Bicubic = 0x04,
        /// <summary>
        /// X
        /// </summary>
        /// <remarks>
        /// <para>In FFMS2, the equivalent is <c>FFMS_RESIZERS_X</c>.</para>
        /// </remarks>
        X = 0x08,
        /// <summary>
        /// Point
        /// </summary>
        /// <remarks>
        /// <para>In FFMS2, the equivalent is <c>FFMS_RESIZERS_POINT</c>.</para>
        /// </remarks>
        Point = 0x10,
        /// <summary>
        /// Area
        /// </summary>
        /// <remarks>
        /// <para>In FFMS2, the equivalent is <c>FFMS_RESIZERS_AREA</c>.</para>
        /// </remarks>
        Area = 0x20,
        /// <summary>
        /// Bicubic (Linear) <!-- I think? -->
        /// </summary>
        /// <remarks>
        /// <para>In FFMS2, the equivalent is <c>FFMS_RESIZERS_BICUBLIN</c>.</para>
        /// </remarks>
        BicubLin = 0x40,
        /// <summary>
        /// Gauss
        /// </summary>
        /// <remarks>
        /// <para>In FFMS2, the equivalent is <c>FFMS_RESIZERS_GAUSS</c>.</para>
        /// </remarks>
        Gauss = 0x80,
        /// <summary>
        /// Sinc
        /// </summary>
        /// <remarks>
        /// <para>In FFMS2, the equivalent is <c>FFMS_RESIZERS_SINC</c>.</para>
        /// </remarks>
        Sinc = 0x100,
        /// <summary>
        /// Lanczos
        /// </summary>
        /// <remarks>
        /// <para>In FFMS2, the equivalent is <c>FFMS_RESIZERS_LANCZOS</c>.</para>
        /// </remarks>
        Lanczos = 0x200,
        /// <summary>
        /// Spline
        /// </summary>
        /// <remarks>
        /// <para>In FFMS2, the equivalent is <c>FFMS_RESIZERS_SPLINE</c>.</para>
        /// </remarks>
        Spline = 0x400
    }

    #endregion

    /// <summary>
    /// Video source
    /// </summary>
    /// <remarks>
    /// <para>In FFMS2, the equivalent is <c>FFMS_VideoSource</c>.</para>
    /// <para>See <see cref="Index.VideoSource">Index.VideoSource</see> on how to create a <see cref="VideoSource">VideoSource object</see>.</para>
    /// <para>Note that there is no equivalent for FFMS2's <c>FFMS_GetVideoProperties</c> as it is called during construction of the <see cref="VideoSource">VideoSource object</see>.</para>
    /// </remarks>
    public class VideoSource
    {
        #region Private properties

        IntPtr FFMS_VideoSource;
        FFMS_VideoProperties VP;

        #endregion

        #region Accessors

        /// <summary>
        /// The nominal framerate of the track
        /// </summary>
        /// <remarks>
        /// <para>In FFMS2, the equivalent is <c>FFMS_VideoProperties->FPSNumerator</c>.</para>
        /// <para>For Matroska files, this number is based on the average frame duration of all frames, while for everything else it's based on the duration of the first frame.</para>
        /// <para>While it might seem tempting to use these values to extrapolate wallclock timestamps for each frame, you really shouldn't do that since it makes your code unable to handle variable framerate properly.</para>
        /// <para>The ugly reality is that these values are pretty much only useful for informational purposes; they are only somewhat reliable for antiquated containers like AVI.</para>
        /// <para>Normally they should never be used for practical purposes; generate individual frame timestamps from <see cref="FrameInfo.PTS">FrameInfo.PTS</see> instead.</para>
        /// </remarks>
        /// <seealso cref="FPSDenominator"/>
        public int FPSNumerator
        { get { return VP.FPSNumerator; } }
        /// <summary>
        /// The nominal framerate of the track
        /// </summary>
        /// <remarks>
        /// <para>In FFMS2, the equivalent is <c>FFMS_VideoProperties->FPSDenominator</c>.</para>
        /// <para>For Matroska files, this number is based on the average frame duration of all frames, while for everything else it's based on the duration of the first frame.</para>
        /// <para>While it might seem tempting to use these values to extrapolate wallclock timestamps for each frame, you really shouldn't do that since it makes your code unable to handle variable framerate properly.</para>
        /// <para>The ugly reality is that these values are pretty much only useful for informational purposes; they are only somewhat reliable for antiquated containers like AVI.</para>
        /// <para>Normally they should never be used for practical purposes; generate individual frame timestamps from <see cref="FrameInfo.PTS">FrameInfo.PTS</see> instead.</para>
        /// </remarks>
        /// <seealso cref="FPSNumerator"/>
        public int FPSDenominator
        { get { return VP.FPSDenominator; } }
        /// <summary>
        /// The special RFF timebase
        /// </summary>
        /// <remarks>
        /// <para>In FFMS2, the equivalent is <c>FFMS_VideoProperties->RFFNumerator</c>.</para>
        /// </remarks>
        /// <seealso cref="Frame.RepeatPict"/>
        /// <seealso cref="RFFDenominator"/>
        public int RFFNumerator
        { get { return VP.RFFNumerator; } }
        /// <summary>
        /// The special RFF timebase
        /// </summary>
        /// <remarks>
        /// <para>In FFMS2, the equivalent is <c>FFMS_VideoProperties->RFFDenominator</c>.</para>
        /// </remarks>
        /// <seealso cref="Frame.RepeatPict"/>
        /// <seealso cref="RFFNumerator"/>
        public int RFFDenominator
        { get { return VP.RFFDenominator; } }
        /// <summary>
        /// The number of frames in the video track
        /// </summary>
        /// <remarks>
        /// <para>In FFMS2, the equivalent is <c>FFMS_VideoProperties->NumFrames</c>.</para>
        /// </remarks>
        public int NumFrames
        { get { return VP.NumFrames; } }
        /// <summary>
        /// Sample aspect ratio of the video frames
        /// </summary>
        /// <remarks>
        /// <para>In FFMS2, the equivalent is <c>FFMS_VideoProperties->SARNum</c> and <c>SARDen</c>.</para>
        /// <para>Note that this is a metadata setting that you are free to ignore, but if you want the proper display aspect ratio with anamorphic material, you should honor it.</para>
        /// <para>On the other hand, there are situations (like when encoding) where you should probably ignore it because the user expects it to be ignored.</para>
        /// </remarks>
        /// <seealso cref="SampleAspectRatioDenominator"/>
        public int SampleAspectRatioNumerator
        { get { return VP.SARNum; } }
        /// <summary>
        /// Sample aspect ratio of the video frames
        /// </summary>
        /// <remarks>
        /// <para>In FFMS2, the equivalent is <c>FFMS_VideoProperties->SARNum</c> and <c>SARDen</c>.</para>
        /// <para>Note that this is a metadata setting that you are free to ignore, but if you want the proper display aspect ratio with anamorphic material, you should honor it.</para>
        /// <para>On the other hand, there are situations (like when encoding) where you should probably ignore it because the user expects it to be ignored.</para>
        /// </remarks>
        /// <seealso cref="SampleAspectRatioNumerator"/>
        public int SampleAspectRatioDenominator
        { get { return VP.SARDen; } }
        /// <summary>
        /// The number of pixels you should crop the frame before displaying it
        /// </summary>
        /// <remarks>
        /// <para>In FFMS2, the equivalent is <c>FFMS_VideoProperties->CropTop</c>, <c>CropBottom</c>, <c>CropLeft</c> and <c>CropRight</c>.</para>
        /// </remarks>
        /// <seealso cref="Selection"/>
        public Selection Crop
        { get { return new Selection(VP.CropTop, VP.CropLeft, VP.CropRight, VP.CropBottom); } }
        /// <summary>
        /// Is the top field first?
        /// </summary>
        /// <remarks>
        /// <para>In FFMS2, the equivalent is <c>FFMS_VideoProperties->TopFieldFirst</c>.</para>
        /// </remarks>
        public bool TopFieldFirst
        { get { return VP.TopFieldFirst != 0; } }
        /// <summary>
        /// The first timestamp of the stream, in seconds
        /// </summary>
        /// <remarks>
        /// <para>In FFMS2, the equivalent is <c>FFMS_VideoProperties->FirstTime</c>.</para>
        /// <para>Useful if you want to know if the stream has a delay, or for quickly determining its length in seconds.</para>
        /// </remarks>
        /// <seealso cref="LastTime"/>
        public double FirstTime
        { get { return VP.FirstTime; } }
        /// <summary>
        /// The first timestamp of the stream, in seconds
        /// </summary>
        /// <remarks>
        /// <para>In FFMS2, the equivalent is <c>FFMS_VideoProperties->LastTime</c>.</para>
        /// <para>Useful if you want to know if the stream has a delay, or for quickly determining its length in seconds.</para>
        /// </remarks>
        /// <seealso cref="FirstTime"/>
        public double LastTime
        { get { return VP.LastTime; } }

        #endregion

        #region Constructor and destructor

        internal VideoSource(IntPtr VideoSource)
        {
            FFMS_VideoSource = VideoSource;
            IntPtr propPtr = NativeMethods.FFMS_GetVideoProperties(VideoSource);
            VP = (FFMS_VideoProperties)Marshal.PtrToStructure(propPtr, typeof(FFMS_VideoProperties));
        }

        /// <summary>
        /// Video source destructor
        /// </summary>
        /// <remarks>
        /// <para>In FFMS2, the equivalent is <c>FFMS_DestroyVideoSource</c>.</para>
        /// </remarks>
        ~VideoSource()
        {
            NativeMethods.FFMS_DestroyVideoSource(FFMS_VideoSource);
        }

        #endregion

        #region Methods

        /// <summary>
        /// Sets the output format for video frames
        /// </summary>
        /// <remarks>
        /// <para>In FFMS2, the equivalent is <c>FFMS2_SetOutputFormatV2</c>.</para>
        /// </remarks>
        /// <param name="TargetFormats">The desired output colorspace(s)
        /// <para>The destination that gives the least lossy conversion from the source colorspace will automatically be selected, ON A FRAME BASIS.</para>
        /// <para>To get the integer constant representing a given colorspace, see <see cref="FFMS2.GetPixFmt">GetPixFmt</see>.</para>
        /// </param>
        /// <param name="Width">The desired image width, in pixels
        /// <para>If you do not want to resize just pass the input dimensions.</para>
        /// </param>
        /// <param name="Height">The desired image height, in pixels
        /// <para>If you do not want to resize just pass the input dimensions.</para>
        /// </param>
        /// <param name="Resizer">The desired image resizing algorithm.
        /// <para>You must choose one even if you're not actually rescaling the image, because the video may change resolution mid-stream and then you will be using a resizer whether you want it or not (you will only know that the resolution changed after you actually decoded a frame with a new resolution), and it may also get used for rescaling subsampled chroma planes.</para>
        /// </param>
        /// <exception cref="FFMSException"/>
        /// <seealso cref="ResetOutputFormat"/>
        /// <exception cref="ArgumentOutOfRangeException">Trying to set the desired image resolution to an invalid size like 0, 0.</exception>
        public void SetOutputFormat(List<int> TargetFormats, int Width, int Height, Resizers Resizer)
        {
            if (Width <= 0)
                throw new ArgumentOutOfRangeException("Width", "Invalid image width.");
            if (Height <= 0)
                throw new ArgumentOutOfRangeException("Height", "Invalid image height.");

            FFMS_ErrorInfo err = new FFMS_ErrorInfo();
            err.BufferSize = 1024;
            err.Buffer = new String((char)0, 1024);

            int[] targetFormats = new int[TargetFormats.Count + 1];
            for (int i = 0; i < TargetFormats.Count; i++)
            {
                targetFormats[i] = TargetFormats[i];
            }
            targetFormats[TargetFormats.Count] = -1;

            if (NativeMethods.FFMS_SetOutputFormatV2(FFMS_VideoSource, targetFormats, Width, Height, (int)Resizer, ref err) != 0)
            {
                if (err.ErrorType == FFMS_Errors.FFMS_ERROR_SCALING && err.SubType == FFMS_Errors.FFMS_ERROR_INVALID_ARGUMENT)
                    throw new ArgumentException(err.Buffer);
                else
                    throw new NotImplementedException(string.Format("Unknown FFMS2 error encountered: '{0}'. Please report this issue on FFMSsharp's GitHub.", err.Buffer));
            }
        }

        /// <summary>
        /// Resets the video output format
        /// </summary>
        /// <param>
        /// <para>In FFMS2, the equivalent is <c>FFMS_ResetOutputFormatV</c>.</para>
        /// <para>Resets the output format so that no conversion takes place.</para>
        /// <para>Note that the results of this function may vary wildly, particularly if the video changes resolution mid-stream.</para>
        /// <para>If you call it, you'd better call <see cref="GetFrame(int)">GetFrame</see> afterwards and examine the properties to see what you actually ended up with.</para>
        /// </param>
        public void ResetOutputFormat()
        {
            NativeMethods.FFMS_ResetOutputFormatV(FFMS_VideoSource);
        }

        /// <summary>
        /// Override the source format for video frames
        /// </summary>
        /// <remarks>
        /// <para>In FFMS2, the equivalent is <c>FFMS2_SetInputFormatV</c>.</para>
        /// <para>Override the source colorspace passed to SWScale for conversions and resizing for all further calls to <see cref="GetFrame(int)">GetFrame</see>, until the next time you call <see cref="SetInputFormat(ColorSpaces, ColorRanges)">SetInputFormat</see> or <see cref="ResetInputFormat">ResetInputFormat</see>.</para>
        /// <para>This is intended primarily for compatibility with programs which use the wrong YUV colorspace when converting to or from RGB, but can also be useful for files which have incorrect colorspace flags.</para>
        /// <para>Values passed are not checked for sanity; if you wish you may tell FFMS2 to pretend that a RGB files is actually YUV using this function, but doing so is unlikely to have useful results.</para>
        /// <para>This function only has an effect if the output format is also set with <see cref="SetOutputFormat">SetOutputFormat</see>.</para>
        /// </remarks>
        /// <param name="ColorSpace">The desired input colorspace</param>
        /// <param name="ColorRange">The desired input colorrange</param>
        /// <exception cref="FFMSException"/>
        /// <seealso cref="ResetInputFormat"/>
        public void SetInputFormat(ColorSpaces ColorSpace = ColorSpaces.Unspecified, ColorRanges ColorRange = ColorRanges.Unspecified)
        {
            SetInputFormat(FFMS2.GetPixFmt(""), ColorSpace, ColorRange);
        }
        /// <summary>
        /// Override the source format for video frames
        /// </summary>
        /// <remarks>
        /// <para>In FFMS2, the equivalent is <c>FFMS2_SetInputFormatV</c>.</para>
        /// <para>Override the source colorspace passed to SWScale for conversions and resizing for all further calls to <see cref="GetFrame(int)">GetFrame</see>, until the next time you call <see cref="SetInputFormat(int, ColorSpaces, ColorRanges)">SetInputFormat</see> or <see cref="ResetInputFormat">ResetInputFormat</see>.</para>
        /// <para>This is intended primarily for compatibility with programs which use the wrong YUV colorspace when converting to or from RGB, but can also be useful for files which have incorrect colorspace flags.</para>
        /// <para>Values passed are not checked for sanity; if you wish you may tell FFMS2 to pretend that a RGB files is actually YUV using this function, but doing so is unlikely to have useful results.</para>
        /// <para>This function only has an effect if the output format is also set with <see cref="SetOutputFormat">SetOutputFormat</see>.</para>
        /// </remarks>
        /// <param name="PixelFormat">The desired input pixel format</param>
        /// <param name="ColorSpace">The desired input colorspace</param>
        /// <param name="ColorRange">The desired input colorrange</param>
        /// <exception cref="FFMSException"/>
        /// <seealso cref="ResetInputFormat"/>
        public void SetInputFormat(int PixelFormat, ColorSpaces ColorSpace = ColorSpaces.Unspecified, ColorRanges ColorRange = ColorRanges.Unspecified)
        {
            FFMS_ErrorInfo err = new FFMS_ErrorInfo();
            err.BufferSize = 1024;
            err.Buffer = new String((char)0, 1024);

            if (NativeMethods.FFMS_SetInputFormatV(FFMS_VideoSource, (int)ColorSpace, (int)ColorRange, PixelFormat, ref err) != 0)
                throw ErrorHandling.ExceptionFromErrorInfo(err);
        }

        /// <summary>
        /// Resets the video input format to the values specified in the source file
        /// </summary>
        /// <remarks>
        /// <para>In FFMS2, the equivalent is <c>FFMS_ResetInputFormatV</c>.</para>
        /// </remarks>
        /// <seealso cref="SetInputFormat(ColorSpaces, ColorRanges)"/>
        /// <seealso cref="SetInputFormat(int, ColorSpaces, ColorRanges)"/>
        public void ResetInputFormat()
        {
            NativeMethods.FFMS_ResetInputFormatV(FFMS_VideoSource);
        }

        /// <summary>
        /// Retrieves a video frame
        /// </summary>
        /// <remarks>
        /// <para>In FFMS2, the equivalent is <c>FFMS_GetFrame</c>.</para>
        /// <para>The returned frame is owned by the given <see cref="VideoSource">VideoSource object</see>, and remains valid until the video source is destroyed, a different frame is requested from the video source, or the video source's input or output format is changed.</para>
        /// </remarks>
        /// <param name="Frame">The frame number to get
        /// <para>Frame numbering starts from zero, and hence the first frame is number 0 (not 1) and the last frame is number <see cref="NumFrames">NumFrames</see> - 1.</para>
        /// </param>
        /// <returns>The generated <see cref="Frame">Frame object</see>.</returns>
        /// <exception cref="FFMSException"/>
        /// <seealso cref="GetFrame(double)"/>
        /// <exception cref="ArgumentOutOfRangeException">Trying to access a Frame that doesn't exist.</exception>
        public Frame GetFrame(int Frame)
        {
            if (Frame < 0 || Frame > NumFrames - 1)
                throw new ArgumentOutOfRangeException("Frame", "That frame doesn't exist.");

            FFMS_ErrorInfo err = new FFMS_ErrorInfo();
            err.BufferSize = 1024;
            err.Buffer = new String((char)0, 1024);

            IntPtr framePtr = IntPtr.Zero;
            lock (this)
            {
                framePtr = NativeMethods.FFMS_GetFrame(FFMS_VideoSource, Frame, ref err);
            }

            if (framePtr == IntPtr.Zero)
                throw ErrorHandling.ExceptionFromErrorInfo(err);

            return new Frame(framePtr);
        }

        /// <summary>
        /// Retrieves a video frame at a timestamp
        /// </summary>
        /// <remarks>
        /// <para>In FFMS2, the equivalent is <c>FFMS_GetFrameByTime</c>.</para>
        /// <para>The returned frame is owned by the given <see cref="VideoSource">VideoSource object</see>, and remains valid until the video source is destroyed, a different frame is requested from the video source, or the video source's input or output format is changed.</para>
        /// <para>Does the exact same thing as <see cref="GetFrame(int)">GetFrame</see> except instead of giving it a frame number you give it a timestamp in seconds, and it will retrieve the frame that starts closest to that timestamp.</para>
        /// <para>This function exists for the people who are too lazy to build and traverse a mapping between frame numbers and timestamps themselves.</para>
        /// </remarks>
        /// <param name="Time">Timestamp</param>
        /// <returns>The generated <see cref="Frame">Frame object</see>.</returns>
        /// <exception cref="FFMSException"/>
        /// <seealso cref="GetFrame(int)"/>
        /// <exception cref="ArgumentOutOfRangeException">Trying to access a Frame that doesn't exist.</exception>
        public Frame GetFrame(double Time)
        {
            if (Time < 0 || Time > LastTime)
                throw new ArgumentOutOfRangeException("Time", "That frame doesn't exist.");

            FFMS_ErrorInfo err = new FFMS_ErrorInfo();
            err.BufferSize = 1024;
            err.Buffer = new String((char)0, 1024);

            IntPtr framePtr = IntPtr.Zero;
            lock (this)
            {
                 framePtr = NativeMethods.FFMS_GetFrameByTime(FFMS_VideoSource, Time, ref err);
            }

            if (framePtr == IntPtr.Zero)
                throw ErrorHandling.ExceptionFromErrorInfo(err);

            return new Frame(framePtr);
        }

        /// <summary>
        /// Retrieves track info
        /// </summary>
        /// <remarks>
        /// <para>In FFMS2, the equivalent is <c>FFMS_GetTrackFromVideo</c>.</para>
        /// <para>It's generally safer to use this function instead of <see cref="Index.GetTrack">Index.GetTrack</see>, since unlike that function it cannot cause access violations if you specified an nonexistent track number, return a <see cref="Track">Track object</see> that doesn't actually contain any indexing information, or return an object that ceases to be valid when the index is destroyed.</para>
        /// <para>Note that the returned <see cref="Track">Track object</see> is only valid until its parent <see cref="VideoSource">VideoSource object</see> is destroyed. </para>
        /// </remarks>
        /// <returns>The generated <see cref="Track">Track object</see></returns>
        public Track GetTrack()
        {
            IntPtr track = IntPtr.Zero;

            track = NativeMethods.FFMS_GetTrackFromVideo(FFMS_VideoSource);

            return new FFMSsharp.Track(track);
        }

        #endregion
    }
}
