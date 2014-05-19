using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;

namespace FFMSSharp
{
    #region Interop

    [StructLayout(LayoutKind.Sequential)]
    class FFMS_VideoProperties
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
        [DllImport("ffms2.dll", SetLastError = false)]
        public static extern IntPtr FFMS_GetVideoProperties(IntPtr V);

        [DllImport("ffms2.dll", SetLastError = false)]
        public static extern void FFMS_DestroyVideoSource(IntPtr V);

        [DllImport("ffms2.dll", SetLastError = false)]
        public static extern int FFMS_SetOutputFormatV2(IntPtr V, int[] TargetFormats, int Width, int Height, int Resizer, ref FFMS_ErrorInfo ErrorInfo);

        [DllImport("ffms2.dll", SetLastError = false)]
        public static extern void FFMS_ResetOutputFormatV(IntPtr V);

        [DllImport("ffms2.dll", SetLastError = false)]
        public static extern int FFMS_SetInputFormatV(IntPtr V, int ColorSpace, int ColorRange, int PixelFormat, ref FFMS_ErrorInfo ErrorInfo);

        [DllImport("ffms2.dll", SetLastError = false)]
        public static extern void FFMS_ResetInputFormatV(IntPtr V);

        [DllImport("ffms2.dll", SetLastError = false)]
        public static extern IntPtr FFMS_GetFrame(IntPtr V, int n, ref FFMS_ErrorInfo ErrorInfo);

        [DllImport("ffms2.dll", SetLastError = false)]
        public static extern IntPtr FFMS_GetFrameByTime(IntPtr V, double Time, ref FFMS_ErrorInfo ErrorInfo);

        [DllImport("ffms2.dll", SetLastError = false)]
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
    public enum Resizer
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
        /// <seealso cref="Frame.RepeatPicture"/>
        /// <seealso cref="RFFDenominator"/>
        public int RFFNumerator
        { get { return VP.RFFNumerator; } }
        /// <summary>
        /// The special RFF timebase
        /// </summary>
        /// <remarks>
        /// <para>In FFMS2, the equivalent is <c>FFMS_VideoProperties->RFFDenominator</c>.</para>
        /// </remarks>
        /// <seealso cref="Frame.RepeatPicture"/>
        /// <seealso cref="RFFNumerator"/>
        public int RFFDenominator
        { get { return VP.RFFDenominator; } }
        /// <summary>
        /// The number of frames in the video track
        /// </summary>
        /// <remarks>
        /// <para>In FFMS2, the equivalent is <c>FFMS_VideoProperties->NumFrames</c>.</para>
        /// </remarks>
        public int NumberOfFrames
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

        FFMSSharp.Track track;
        /// <summary>
        /// Retrieves track info
        /// </summary>
        /// <remarks>
        /// <para>In FFMS2, the equivalent is <c>FFMS_GetTrackFromVideo</c>.</para>
        /// <para>It's generally safer to use this instead of <see cref="Index.GetTrack">Index.GetTrack</see>, since unlike that function it cannot cause access violations if you specified an nonexistent track number, return a <see cref="Track">Track object</see> that doesn't actually contain any indexing information, or return an object that ceases to be valid when the index is destroyed.</para>
        /// <para>Note that the <see cref="Track">Track object</see> is only valid until its parent <see cref="VideoSource">AudioSource object</see> is destroyed.</para>
        /// </remarks>
        public Track Track
        {
            get
            {
                if (track == null)
                {
                    IntPtr trackPtr = IntPtr.Zero;
                    trackPtr = NativeMethods.FFMS_GetTrackFromVideo(FFMS_VideoSource);
                    track = new FFMSSharp.Track(trackPtr);
                }
                return track;
            }
        }

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
        /// <para>In FFMS2, the equivalent is <c>FFMS_SetOutputFormatV2</c>.</para>
        /// </remarks>
        /// <param name="targetFormats">The desired output colorspace(s)
        /// <para>The destination that gives the least lossy conversion from the source colorspace will automatically be selected, ON A FRAME BASIS.</para>
        /// <para>To get the integer constant representing a given colorspace, see <see cref="FFMS2.GetPixelFormat">GetPixFmt</see>.</para>
        /// </param>
        /// <param name="width">The desired image width, in pixels
        /// <para>If you do not want to resize just pass the input dimensions.</para>
        /// </param>
        /// <param name="height">The desired image height, in pixels
        /// <para>If you do not want to resize just pass the input dimensions.</para>
        /// </param>
        /// <param name="resizer">The desired image resizing algorithm.
        /// <para>You must choose one even if you're not actually rescaling the image, because the video may change resolution mid-stream and then you will be using a resizer whether you want it or not (you will only know that the resolution changed after you actually decoded a frame with a new resolution), and it may also get used for rescaling subsampled chroma planes.</para>
        /// </param>
        /// <seealso cref="ResetOutputFormat"/>
        /// <exception cref="ArgumentOutOfRangeException">Trying to set the desired image resolution to an invalid size like 0, 0.</exception>
        /// <exception cref="ArgumentNullException">Trying to supply a null list of <paramref name="targetFormats"/>.</exception>
        /// <exception cref="ArgumentException">Trying to set an invalid output format.</exception>
        public void SetOutputFormat(ICollection<int> targetFormats, int width, int height, Resizer resizer)
        {
            if (width <= 0)
                throw new ArgumentOutOfRangeException("width", "Invalid image width.");
            if (height <= 0)
                throw new ArgumentOutOfRangeException("height", "Invalid image height.");
            if (targetFormats == null)
                throw new ArgumentNullException("targetFormats");

            FFMS_ErrorInfo err = new FFMS_ErrorInfo();
            err.BufferSize = 1024;
            err.Buffer = new String((char)0, 1024);

            int[] targetFormatsArray = new int[targetFormats.Count + 1];
            targetFormats.CopyTo(targetFormatsArray, 0);
            targetFormatsArray[targetFormats.Count] = -1;

            if (NativeMethods.FFMS_SetOutputFormatV2(FFMS_VideoSource, targetFormatsArray, width, height, (int)resizer, ref err) != 0)
            {
                if (err.ErrorType == FFMS_Errors.FFMS_ERROR_SCALING && err.SubType == FFMS_Errors.FFMS_ERROR_INVALID_ARGUMENT)
                    throw new ArgumentException(err.Buffer);
                if (err.ErrorType == FFMS_Errors.FFMS_ERROR_DECODING && err.SubType == FFMS_Errors.FFMS_ERROR_CODEC)
                    throw new ArgumentException(err.Buffer);

                throw new NotImplementedException(string.Format(System.Globalization.CultureInfo.CurrentCulture, "Unknown FFMS2 error encountered: ({0}, {1}, '{2}'). Please report this issue on FFMSSharp's GitHub.", err.ErrorType, err.SubType, err.Buffer));
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
        /// <para>In FFMS2, the equivalent is <c>FFMS_SetInputFormatV</c>.</para>
        /// <para>Override the source colorspace passed to SWScale for conversions and resizing for all further calls to <see cref="GetFrame(int)">GetFrame</see>, until the next time you call <see cref="SetInputFormat(ColorSpace, ColorRange)">SetInputFormat</see> or <see cref="ResetInputFormat">ResetInputFormat</see>.</para>
        /// <para>This is intended primarily for compatibility with programs which use the wrong YUV colorspace when converting to or from RGB, but can also be useful for files which have incorrect colorspace flags.</para>
        /// <para>Values passed are not checked for sanity; if you wish you may tell FFMS2 to pretend that a RGB files is actually YUV using this function, but doing so is unlikely to have useful results.</para>
        /// <para>This function only has an effect if the output format is also set with <see cref="SetOutputFormat">SetOutputFormat</see>.</para>
        /// </remarks>
        /// <param name="colorSpace">The desired input colorspace</param>
        /// <param name="colorRange">The desired input colorrange</param>
        /// <seealso cref="ResetInputFormat"/>
        /// <exception cref="ArgumentException">Trying to set an invalid output format.</exception>
        public void SetInputFormat(ColorSpace colorSpace = ColorSpace.Unspecified, ColorRange colorRange = ColorRange.Unspecified)
        {
            SetInputFormat(FFMS2.GetPixelFormat(""), colorSpace, colorRange);
        }
        /// <summary>
        /// Override the source format for video frames
        /// </summary>
        /// <remarks>
        /// <para>In FFMS2, the equivalent is <c>FFMS_SetInputFormatV</c>.</para>
        /// <para>Override the source colorspace passed to SWScale for conversions and resizing for all further calls to <see cref="GetFrame(int)">GetFrame</see>, until the next time you call <see cref="SetInputFormat(int, ColorSpace, ColorRange)">SetInputFormat</see> or <see cref="ResetInputFormat">ResetInputFormat</see>.</para>
        /// <para>This is intended primarily for compatibility with programs which use the wrong YUV colorspace when converting to or from RGB, but can also be useful for files which have incorrect colorspace flags.</para>
        /// <para>Values passed are not checked for sanity; if you wish you may tell FFMS2 to pretend that a RGB files is actually YUV using this function, but doing so is unlikely to have useful results.</para>
        /// <para>This function only has an effect if the output format is also set with <see cref="SetOutputFormat">SetOutputFormat</see>.</para>
        /// </remarks>
        /// <param name="pixelFormat">The desired input pixel format</param>
        /// <param name="colorSpace">The desired input colorspace</param>
        /// <param name="colorRange">The desired input colorrange</param>
        /// <seealso cref="ResetInputFormat"/>
        /// <exception cref="ArgumentException">Trying to set an insane input format.</exception>
        public void SetInputFormat(int pixelFormat, ColorSpace colorSpace = ColorSpace.Unspecified, ColorRange colorRange = ColorRange.Unspecified)
        {
            FFMS_ErrorInfo err = new FFMS_ErrorInfo();
            err.BufferSize = 1024;
            err.Buffer = new String((char)0, 1024);

            if (NativeMethods.FFMS_SetInputFormatV(FFMS_VideoSource, (int)colorSpace, (int)colorRange, pixelFormat, ref err) != 0)
            {
                if (err.ErrorType == FFMS_Errors.FFMS_ERROR_SCALING && err.SubType == FFMS_Errors.FFMS_ERROR_INVALID_ARGUMENT)
                    throw new ArgumentException(err.Buffer);
                if (err.ErrorType == FFMS_Errors.FFMS_ERROR_DECODING && err.SubType == FFMS_Errors.FFMS_ERROR_CODEC)
                    throw new ArgumentException(err.Buffer);

                throw new NotImplementedException(string.Format(System.Globalization.CultureInfo.CurrentCulture, "Unknown FFMS2 error encountered: ({0}, {1}, '{2}'). Please report this issue on FFMSSharp's GitHub.", err.ErrorType, err.SubType, err.Buffer));
            }
        }

        /// <summary>
        /// Resets the video input format to the values specified in the source file
        /// </summary>
        /// <remarks>
        /// <para>In FFMS2, the equivalent is <c>FFMS_ResetInputFormatV</c>.</para>
        /// </remarks>
        /// <seealso cref="SetInputFormat(ColorSpace, ColorRange)"/>
        /// <seealso cref="SetInputFormat(int, ColorSpace, ColorRange)"/>
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
        /// <param name="frame">The frame number to get
        /// <para>Frame numbering starts from zero, and hence the first frame is number 0 (not 1) and the last frame is number <see cref="NumberOfFrames">NumFrames</see> - 1.</para>
        /// </param>
        /// <returns>The generated <see cref="Frame">Frame object</see>.</returns>
        /// <seealso cref="GetFrame(double)"/>
        /// <exception cref="ArgumentOutOfRangeException">Trying to access a Frame that doesn't exist.</exception>
        public Frame GetFrame(int frame)
        {
            if (frame < 0 || frame > NumberOfFrames - 1)
                throw new ArgumentOutOfRangeException("frame", "That frame doesn't exist.");

            FFMS_ErrorInfo err = new FFMS_ErrorInfo();
            err.BufferSize = 1024;
            err.Buffer = new String((char)0, 1024);

            IntPtr framePtr = IntPtr.Zero;
            lock (this)
            {
                framePtr = NativeMethods.FFMS_GetFrame(FFMS_VideoSource, frame, ref err);
            }

            if (framePtr == IntPtr.Zero)
            {
                throw new NotImplementedException(string.Format(System.Globalization.CultureInfo.CurrentCulture, "Unknown FFMS2 error encountered: ({0}, {1}, '{2}'). Please report this issue on FFMSSharp's GitHub.", err.ErrorType, err.SubType, err.Buffer));
            }

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
        /// <param name="time">Timestamp</param>
        /// <returns>The generated <see cref="Frame">Frame object</see>.</returns>
        /// <seealso cref="GetFrame(int)"/>
        /// <exception cref="ArgumentOutOfRangeException">Trying to access a Frame that doesn't exist.</exception>
        public Frame GetFrame(double time)
        {
            if (time < 0 || time > LastTime)
                throw new ArgumentOutOfRangeException("time", "That frame doesn't exist.");

            FFMS_ErrorInfo err = new FFMS_ErrorInfo();
            err.BufferSize = 1024;
            err.Buffer = new String((char)0, 1024);

            IntPtr framePtr = IntPtr.Zero;
            lock (this)
            {
                 framePtr = NativeMethods.FFMS_GetFrameByTime(FFMS_VideoSource, time, ref err);
            }

            if (framePtr == IntPtr.Zero)
            {
                throw new NotImplementedException(string.Format(System.Globalization.CultureInfo.CurrentCulture, "Unknown FFMS2 error encountered: ({0}, {1}, '{2}'). Please report this issue on FFMSSharp's GitHub.", err.ErrorType, err.SubType, err.Buffer));
            }

            return new Frame(framePtr);
        }

        #endregion
    }
}
