using System;
using System.Runtime.InteropServices;
using System.Drawing;

namespace FFMSsharp
{
    #region Interop

    struct FFMS_Frame
    {
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 4)]
        public IntPtr[] Data;
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 4)]
        public int[] Linesize;
        public int EncodedWidth;
        public int EncodedHeight;
        public int EncodedPixelFormat;
        public int ScaledWidth;
        public int ScaledHeight;
        public int ConvertedPixelFormat;
        public int KeyFrame;
        public int RepeatPict;
        public int InterlacedFrame;
        public int TopFieldFirst;
        public char PictType;
        public int ColorSpace;
        public int ColorRange;
    }

    #endregion

    #region Constants

    /// <summary>
    /// Identifies the color coefficients used for a YUV stream
    /// </summary>
    /// <remarks>
    /// <para>In FFMS2, the equivalent is <c>FFMS_ColorSpaces</c>.</para>
    /// <para>The numerical constants are the same as in the MPEG-2 specification.</para>
    /// <para>Some of these are specified or aliased in a number of places.</para>
    /// </remarks>
    public enum ColorSpaces
    {
        /// <summary>
        /// RGB
        /// </summary>
        /// <remarks>
        /// <para>In FFMS2, the equivalent is <c>FFMS_CS_RGB</c>.</para>
        /// </remarks>
        RGB = 0,
        /// <summary>
        /// ITU-T Rec. 709
        /// </summary>
        /// <remarks>
        /// <para>In FFMS2, the equivalent is <c>FFMS_CS_BT709</c>.</para>
        /// <para>Equivalent to ITU-R BT1361, IEC 61966-2-4 xvYCC709 and SMPTE RP177 Annex B.</para>
        /// </remarks>
        BT709 = 1,
        /// <summary>
        /// Unspecified
        /// </summary>
        /// <remarks>
        /// <para>In FFMS2, the equivalent is <c>FFMS_CS_UNSPECIFIED</c>.</para>
        /// </remarks>
        Unspecified = 2,
        /// <summary>
        /// FCC
        /// </summary>
        /// <remarks>
        /// <para>In FFMS2, the equivalent is <c>FFMS_CS_FCC</c>.</para>
        /// </remarks>
        FCC = 4,
        /// <summary>
        /// ITU-R BT. 470
        /// </summary>
        /// <remarks>
        /// <para>In FFMS2, the equivalent is <c>FFMS_CS_BT470BG</c>.</para>
        /// <para>Also known as ITU-T Rec. 601.</para>
        /// <para>Equivalent to ITU-R BT601-6 625, ITU-R BT1358 625, ITU-R BT1700 625 PAL &amp; SECAM and IEC 61966-2-4 xvYCC601.</para>
        /// </remarks>
        BT470BG = 5,
        /// <summary>
        /// SMPTE standard 170 M
        /// </summary>
        /// <remarks>
        /// <para>In FFMS2, the equivalent is <c>FFMS_CS_SMPTE170M</c>.</para>
        /// <para>Functionally the same as BT470BG, and is furthermore equivalent to ITU-R BT601-6 525, ITU-R BT1358 525, and ITU-R BT1700 NTSC.</para>
        /// </remarks>
        SMPTE170M = 6,
        /// <summary>
        /// SMPTE standard 240 M
        /// </summary>
        /// <remarks>
        /// <para>In FFMS2, the equivalent is <c>FFMS_CS_SMPTE240M</c>.</para>
        /// </remarks>
        SMPTE240M = 7
    }

    /// <summary>
    /// Identifies the valid range of luma values in a YUV stream
    /// </summary>
    /// <remarks>
    /// <para>In FFMS2, the equivalent is <c>FFMS_ColorRanges</c>.</para>
    /// </remarks>
    public enum ColorRanges
    {
        /// <summary>
        /// Unspecified
        /// </summary>
        /// <remarks>
        /// <para>In FFMS2, the equivalent is <c>FFMS_CR_UNSPECIFIED</c>.</para>
        /// </remarks>
        Unspecified = 0,
        /// <summary>
        /// TV range
        /// </summary>
        /// <remarks>
        /// <para>In FFMS2, the equivalent is <c>FFMS_CR_MPEG</c>.</para>
        /// <para>Valid luma values range from 16 to 235 with 8-bit color.</para>
        /// </remarks>
        MPEG = 1,
        /// <summary>
        /// Full range
        /// </summary>
        /// <remarks>
        /// <para>In FFMS2, the equivalent is <c>FFMS_CR_JPEG</c>.</para>
        /// <para>All representable luma values are valid.</para>
        /// </remarks>
        JPEG = 2,
    }

    #endregion

    /// <summary>
    /// A single video frame
    /// </summary>
    /// <remarks>
    /// <para>In FFMS2, the equivalent is <c>FFMS_Frame</c>.</para>
    /// <para>See <see cref="VideoSource.GetFrame(int)">VideoSource.GetFrame</see> on how to create a <see cref="Frame">Frame object</see>.</para>
    /// </remarks>
    public class Frame
    {
        FFMS_Frame FFMS_Frame;

        #region Accessors

        /// <summary>
        /// An array of pointers to the pixel data
        /// </summary>
        /// <remarks>
        /// <para>In FFMS2, the equivalent is <c>FFMS_Frame->Data</c>.</para>
        /// <para>Planar formats use more than one plane, for example YV12 uses one plane each for the Y, U and V data.</para>
        /// <para>Packed formats (such as the various RGB32 flavors) use only the first plane.</para>
        /// <para>If you want to determine if plane i contains data or not, check for <see cref="Linesize"/>[i] != 0.</para>
        /// </remarks>
        public IntPtr[] Data
        { get { return FFMS_Frame.Data; } }
        /// <summary>
        /// An array of integers representing the length of each scan line in each of the four picture planes, in bytes
        /// </summary>
        /// <remarks>
        /// <para>In FFMS2, the equivalent is <c>FFMS_Frame->Linesize</c>.</para>
        /// <para>In alternative terminology, this is the "pitch" of the plane.</para>
        /// <para>Usually, the total size in bytes of picture plane i is <see cref="Linesize"/>[i] * <see cref="Resolution">Resolution.Height</see>, but do note that some pixel formats (most notably YV12) has vertical chroma subsampling, and then the U/V planes may be of a different height than the primary plane.</para>
        /// <para>This may be negative; if so the image is stored inverted in memory and Data actually points of the last row of the data.</para>
        /// <para>You usually do not need to worry about this, as it mostly works correctly by default if you're processing the image correctly.</para>
        /// </remarks>
        public int[] Linesize
        { get { return FFMS_Frame.Linesize; } }
        /// <summary>
        /// The original resolution of the frame (in pixels)
        /// </summary>
        /// <remarks>
        /// <para>In FFMS2, the equivalent is <c>FFMS_Frame->EncodedWidth</c> and <c>EncodedHeight</c>.</para>
        /// <para>As encoded in the compressed file, before any scaling was applied.</para>
        /// <para>Note that this must not necessarily be the same for all frames in a stream.</para>
        /// </remarks>
        public Size EncodedResolution
        { get { return new Size(FFMS_Frame.EncodedWidth, FFMS_Frame.EncodedHeight); } }
        /// <summary>
        /// The original pixel format of the frame
        /// </summary>
        /// <remarks>
        /// <para>In FFMS2, the equivalent is <c>FFMS_Frame->EncodedPixelFormat</c>.</para>
        /// <para>As encoded in the compressed file.</para>
        /// </remarks>
        public int EncodedPixelFormat
        { get { return FFMS_Frame.EncodedPixelFormat; } }
        /// <summary>
        /// The output resolution of the frame (in pixels)
        /// </summary>
        /// <remarks>
        /// <para>In FFMS2, the equivalent is <c>FFMS_Frame->ScaledWidth</c> and <c>ScaledHeight</c>.</para>
        /// <para>The resolution of what is actually stored in the <see cref="Data"/> field.</para>
        /// </remarks>
        public Size Resolution
        { get { return new Size(FFMS_Frame.ScaledWidth, FFMS_Frame.ScaledHeight); } }
        /// <summary>
        /// The output pixel format of the frame
        /// </summary>
        /// <remarks>
        /// <para>In FFMS2, the equivalent is <c>FFMS_Frame->ConvertedPixelFormat</c>.</para>
        /// <para>The pixel format of what is actually stored in the <see cref="Data"/> field.</para>
        /// </remarks>
        public int PixelFormat
        { get { return FFMS_Frame.ConvertedPixelFormat; } }
        /// <summary>
        /// Is this a keyframe?
        /// </summary>
        /// <remarks>
        /// <para>In FFMS2, the equivalent is <c>FFMS_Frame->KeyFrame</c>.</para>
        /// </remarks>
        public bool KeyFrame
        { get { return FFMS_Frame.KeyFrame != 0; } }
        /// <summary>
        /// An integer representing the RFF flag for this frame
        /// </summary>
        /// <remarks>
        /// <para>In FFMS2, the equivalent is <c>FFMS_Frame->RepeatPict</c>.</para>
        /// <para>The frame shall be displayed for 1+<see cref="RepeatPict"/> time units, where the time units are expressed in the special RFF timebase available in <see cref="VideoSource.RFF"/>.</para>
        /// <para>Note that if you actually end up using this, you need to ignore the usual timestamps (calculated via the <see cref="Track.TimeBase"/> and the <see cref="FrameInfo.PTS"/>) since they are fundamentally incompatible with RFF flags.</para>
        /// </remarks>
        public int RepeatPict
        { get { return FFMS_Frame.RepeatPict; } }
        /// <summary>
        /// Is this an interlaced frame?
        /// </summary>
        /// <remarks>
        /// <para>In FFMS2, the equivalent is <c>FFMS_Frame->InterlacedFrame</c>.</para>
        /// </remarks>
        public bool InterlacedFrame
        { get { return FFMS_Frame.InterlacedFrame != 0; } }
        /// <summary>
        /// Is the top field first?
        /// </summary>
        /// <remarks>
        /// <para>In FFMS2, the equivalent is <c>FFMS_Frame->TopFieldFirst</c>.</para>
        /// <para>Only relevant if <see cref="InterlacedFrame"/> is nonzero.</para>
        /// </remarks>
        public bool TopFieldFirst
        { get { return FFMS_Frame.TopFieldFirst != 0; } }
        /// <summary>
        /// A single character denoting coding type (I/B/P etc) of the compressed frame
        /// </summary>
        /// <remarks>
        /// <para>In FFMS2, the equivalent is <c>FFMS_Frame.PictType</c>.</para>
        /// </remarks>
        public char FrameType
        { get { return FFMS_Frame.PictType; } }
        /// <summary>
        /// Identifies the YUV color coefficients used in the frame
        /// </summary>
        /// <remarks>
        /// <para>In FFMS2, the equivalent is <c>FFMS_Frame->ColorSpace</c>.</para>
        /// </remarks>
        /// <seealso cref="ColorSpaces"/>
        public ColorSpaces ColorSpace
        { get { return (ColorSpaces)FFMS_Frame.ColorSpace; } }
        /// <summary>
        /// Identifies the luma range of the frame
        /// </summary>
        /// <remarks>
        /// <para>In FFMS2, the equivalent is <c>FFMS_Frame->ColorRange</c>.</para>
        /// </remarks>
        /// <seealso cref="ColorRanges"/>
        public ColorRanges ColorRange
        { get { return (ColorRanges)FFMS_Frame.ColorRange; } }

        #endregion

        #region Constructor

        internal Frame(IntPtr Frame)
        {
            FFMS_Frame = (FFMS_Frame)Marshal.PtrToStructure(Frame, typeof(FFMS_Frame));
        }

        #endregion

        #region Object creation

        /// <summary>
        /// Turn the pixel data into a <see cref="Bitmap">Bitmap</see>
        /// </summary>
        /// <remarks>
        /// <para>This function only works if you've <see cref="VideoSource.SetOutputFormat">set the PixelFormat</see> to "bgra".</para>
        /// </remarks>
        /// <returns>The generated <see cref="Bitmap">Bitmap object</see></returns>
        public Bitmap GetBitmap()
        {
            if (FFMS_Frame.ConvertedPixelFormat != FFMS2.GetPixFmt("bgra"))
                throw new Exception("You can only use this function with the brga output format");
            return new Bitmap(FFMS_Frame.ScaledWidth, FFMS_Frame.ScaledHeight, FFMS_Frame.ScaledWidth * 4, System.Drawing.Imaging.PixelFormat.Format32bppArgb, FFMS_Frame.Data[0]);
        }

        #endregion
    }
}
