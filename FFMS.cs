using System;
using System.Runtime.InteropServices;

namespace FFMSsharp
{
    #region Interop

    static partial class Interop
    {
        [DllImport("ffms2.dll", SetLastError = false, CharSet = CharSet.Ansi)]
        public static extern void FFMS_Init(int unused, int UseUTF8Paths);

        [DllImport("ffms2.dll", SetLastError = false, CharSet = CharSet.Ansi)]
        public static extern int FFMS_GetLogLevel();

        [DllImport("ffms2.dll", SetLastError = false, CharSet = CharSet.Ansi)]
        public static extern void FFMS_SetLogLevel(int Level);

        [DllImport("ffms2.dll", SetLastError = false, CharSet = CharSet.Ansi)]
        public static extern int FFMS_GetPixFmt(string Name);

        [DllImport("ffms2.dll", SetLastError = false, CharSet = CharSet.Ansi)]
        public static extern int FFMS_GetPresentSources();

        [DllImport("ffms2.dll", SetLastError = false, CharSet = CharSet.Ansi)]
        public static extern int FFMS_GetEnabledSources();

        [DllImport("ffms2.dll", SetLastError = false, CharSet = CharSet.Ansi)]
        public static extern int FFMS_GetVersion();
    }

    #endregion

    #region Constants

    /// <summary>
    /// Log level for libavformat
    /// </summary>
    public enum AvLogLevel
    {
        /// <summary>
        /// No output
        /// </summary>
        Quiet = -8,
        /// <summary>
        /// Something went really wrong and we will crash now
        /// </summary>
        Panic = 0,
        /// <summary>
        /// Something went wrong and recovery is not possible
        /// </summary>
        /// <remarks>
        /// For example, no header was found for a format which depends
        /// on headers or an illegal combination of parameters is used.
        /// </remarks>
        Fatal = 8,
        /// <summary>
        /// Something went wrong and cannot losslessly be recovered
        /// </summary>
        /// <remarks>
        /// However, not all future data is affected.
        /// </remarks>
        Error = 16,
        /// <summary>
        /// Something somehow does not look correct
        /// </summary>
        /// <remarks>
        /// This may or may not lead to problems. An example would be the use of '-vstrict -2'.
        /// </remarks>
        Warning = 24,
        /// <summary>
        /// Show regular information
        /// </summary>
        Info = 32,
        /// <summary>
        /// Show lots of information
        /// </summary>
        Verbose = 40,
        /// <summary>
        /// Stuff which is only useful for libav* developers
        /// </summary>
        Debug = 48
    }

    #endregion

    #region Simple classes for special data representation

    /// <summary>
    /// Simple representation of a rational int
    /// </summary>
    public class Rational
    {
        /// <summary>
        /// The numerator
        /// </summary>
        public int Numerator;
        /// <summary>
        /// The denominator
        /// </summary>
        public int Denominator;

        internal Rational(int Numerator, int Denominator)
        {
            this.Numerator = Numerator;
            this.Denominator = Denominator;
        }
    }

    /// <summary>
    /// Simple representation of a rational long
    /// </summary>
    public class LongRational
    {
        /// <summary>
        /// The numerator
        /// </summary>
        public long Numerator;
        /// <summary>
        /// The denominator
        /// </summary>
        public long Denominator;

        internal LongRational(long Numerator, long Denominator)
        {
            this.Numerator = Numerator;
            this.Denominator = Denominator;
        }
    }

    /// <summary>
    /// Simple representation of a selection rectangle
    /// </summary>
    public class Selection
    {
        /// <summary>
        /// Amount of Top to crop
        /// </summary>
        public int Top;
        /// <summary>
        /// Amount of Left to crop
        /// </summary>
        public int Left;
        /// <summary>
        /// Amount of Right to crop
        /// </summary>
        public int Right;
        /// <summary>
        /// Amount of Bottom to crop
        /// </summary>
        public int Bottom;

        internal Selection(int Top, int Left, int Right, int Bottom)
        {
            this.Top = Top;
            this.Left = Left;
            this.Right = Right;
            this.Bottom = Bottom;
        }
    }

    /// <summary>
    /// Simple representation of dimensions
    /// </summary>
    public class Size
    {
        /// <summary>
        /// The width
        /// </summary>
        public int Width;
        /// <summary>
        /// The height
        /// </summary>
        public int Height;

        internal Size(int Width, int Height)
        {
            this.Width = Width;
            this.Height = Height;
        }
    }

    #endregion

    /// <summary>
    /// Container for generic FFMS2 functions
    /// </summary>
    public static class FFMS2
    {
        #region Private properties

        private static bool initialized;
        private static int presentSources;
        private static int enabledSources;

        #endregion

        #region Accessors

        /// <summary>
        /// Is FFMS2 initialized?
        /// </summary>
        public static bool Initialized { get { return initialized; } }
        /// <summary>
        /// Source modules that the library was compiled with
        /// </summary>
        /// <remarks>
        /// <para>In FFMS2, the equivalent is <c>FFMS_GetPresentSources</c>.</para>
        /// </remarks>
        public static int PresentSources { get { return presentSources; } }
        /// <summary>
        /// Source modules that are actually available for use
        /// </summary>
        /// <remarks>
        /// <para>In FFMS2, the equivalent is <c>FFMS_GetEnabledSources</c>.</para>
        /// </remarks>
        public static int EnabledSources { get { return enabledSources; } }

        #endregion

        #region Methods

        /// <summary>
        /// Initialize the FFMS2 library
        /// </summary>
        /// <remarks>
        /// <para>In FFMS2, the equivalent is <c>FFMS_Init</c>.</para>
        /// <para>Must be called before anything else.</para>
        /// </remarks>
        public static void Initialize()
        {
            if (initialized)
                return;
            
            Interop.FFMS_Init(0, 0);
            presentSources = Interop.FFMS_GetPresentSources();
            enabledSources = Interop.FFMS_GetEnabledSources();

            initialized = true;
        }

        /// <summary>
        /// Is the source compiled in the library?
        /// </summary>
        /// <param name="option">The source in question</param>
        /// <returns>The result</returns>
        public static bool IsSourcePresent(Sources option)
        {
            return Convert.ToBoolean(presentSources & (int)option);
        }

        /// <summary>
        /// Is the source currently enabled?
        /// </summary>
        /// <param name="option">The source in question</param>
        /// <returns>The result</returns>
        public static bool IsSourceEnabled(Sources option)
        {
            return Convert.ToBoolean(enabledSources & (int)option);
        }

        /// <summary>
        /// Get the FFMS_VERSION constant
        /// </summary>
        /// <remarks>
        /// <para>In FFMS2, the equivalent is <c>FFMS_GetVersion</c>.</para>
        /// <para>You may want to use <see cref="GetVersionString">GetVersionString</see> if you just want to print the version.</para>
        /// </remarks>
        /// <returns>FFMS_VERSION constant as defined in ffms.h as an integer</returns>
        /// <seealso cref="GetVersionString"/>
        public static int GetVersion()
        {
            return Interop.FFMS_GetVersion();
        }

        /// <summary>
        /// Get a human-readable version of the FFMS_VERSION constant
        /// </summary>
        /// <returns>A pretty version string</returns>
        /// <seealso cref="GetVersion"/>
        public static string GetVersionString()
        {
            int version = GetVersion();

            int major = version >> 24;
            int minor = (version >> 16) & 0xFF;
            int micro = (version >> 8) & 0xFF;
            int bump = version & 0xFF;

            if (bump != 0)
            {
                return string.Format("{0}.{1}.{2}.{3}", major, minor, micro, bump);
            }
            else if (micro != 0)
            {
                return string.Format("{0}.{1}.{2}", major, minor, micro);
            }
            else
            {
                return string.Format("{0}.{1}", major, minor);
            }
        }

        /// <summary>
        /// Gets FFmpeg message level
        /// </summary>
        /// <remarks>
        /// <para>In FFMS2, the equivalent is <c>FFMS_GetLogLevel</c>.</para>
        /// </remarks>
        /// <returns>The result</returns>
        public static AvLogLevel GetLogLevel()
        {
            return (AvLogLevel)Interop.FFMS_GetLogLevel();
        }

        /// <summary>
        /// Sets FFmpeg message level
        /// </summary>
        /// <remarks>
        /// <para>In FFMS2, the equivalent is <c>FFMS_SetLogLevel</c>.</para>
        /// </remarks>
        /// <param name="Level">The new message level</param>
        public static void SetLogLevel(AvLogLevel Level)
        {
            Interop.FFMS_SetLogLevel((int)Level);
        }

        /// <summary>
        /// Gets a colorspace identifier from a colorspace name
        /// </summary>
        /// <remarks>
        /// <para>In FFMS2, the equivalent is <c>FFMS_GetPixFmt</c>.</para>
        /// <para>Translates a given pixel format name to an integer constant representing it, suitable for passing to <see cref="VideoSource.SetOutputFormat">VideoSource.SetOutputFormat</see>.</para>
        /// <para>This function exists so that you don't have to include a FFmpeg header file in every single program you ever write.</para>
        /// <para>For a list of colorspaces and their names, see libavutil/pixfmt.h.</para>
        /// <para>To get the name of a colorspace, strip the leading PIX_FMT_ and convert the remainder to lowercase.</para>
        /// <para>For example, the name of PIX_FMT_YUV420P is yuv420p.</para>
        /// <para>It is strongly recommended to use this function instead of including pixfmt.h directly, since this function guarantees that you will always get the constant definitions from the version of FFmpeg that FFMS2 was linked against.</para>
        /// </remarks>
        /// <param name="Name"></param>
        /// <returns></returns>
        public static int GetPixFmt(string Name)
        {
            return Interop.FFMS_GetPixFmt(Name);
        }

        #endregion
    }
}