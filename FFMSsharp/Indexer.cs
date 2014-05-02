using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;

namespace FFMSSharp
{
    #region Interop

    static partial class NativeMethods
    {
        [DllImport("ffms2.dll", SetLastError = false, CharSet = CharSet.Ansi)]
        public static extern int FFMS_GetSourceTypeI(IntPtr Indexer);

        [DllImport("ffms2.dll", SetLastError = false, CharSet = CharSet.Ansi)]
        public static extern int FFMS_GetNumTracksI(IntPtr Indexer);

        [DllImport("ffms2.dll", SetLastError = false, CharSet = CharSet.Ansi)]
        public static extern int FFMS_GetTrackTypeI(IntPtr Indexer, int Track);

        [DllImport("ffms2.dll", SetLastError = false, CharSet = CharSet.Ansi)]
        public static extern IntPtr FFMS_GetCodecNameI(IntPtr Indexer, int Track);

        [DllImport("ffms2.dll", SetLastError = false, CharSet = CharSet.Ansi)]
        public static extern IntPtr FFMS_GetFormatNameI(IntPtr Indexer);

        [DllImport("ffms2.dll", SetLastError = false, CharSet = CharSet.Ansi)]
        public static extern IntPtr FFMS_CreateIndexerWithDemuxer(string SourceFile, int Demuxer, ref FFMS_ErrorInfo ErrorInfo);

        [DllImport("ffms2.dll", SetLastError = false, CharSet = CharSet.Ansi)]
        public static extern void FFMS_CancelIndexing(IntPtr Indexer);

        [DllImport("ffms2.dll", SetLastError = false, CharSet = CharSet.Ansi)]
        public static extern int FFMS_DefaultAudioFilename(string SourceFile, int Track, ref FFMS_AudioProperties AP, IntPtr FileName, int FNSize, IntPtr Private);

        [DllImport("ffms2.dll", SetLastError = false, CharSet = CharSet.Ansi)]
        public static extern IntPtr FFMS_DoIndexing(IntPtr Indexer, int IndexMask, int DumpMask, TAudioNameCallback ANC, IntPtr ANCPrivate, int ErrorHandling, TIndexCallback IC, IntPtr ICPrivate, ref FFMS_ErrorInfo ErrorInfo);

        public delegate int TAudioNameCallback(string SourceFile, int Track, ref FFMS_AudioProperties AP, IntPtr FileName, int FNSize, IntPtr Private);
        public delegate int TIndexCallback(long Current, long Total, IntPtr ICPrivate);
    }

    #endregion

    #region Constants

    /// <summary>
    /// Identifies source modules
    /// </summary>
    /// <remarks>
    /// <para>In FFMS2, the equivalent is <c>FFMS_Sources</c>.</para>
    /// </remarks>
    public enum Source
    {
        /// <summary>
        /// Default
        /// </summary>
        /// <remarks>
        /// <para>In FFMS2, the equivalent is <c>FFMS_SOURCE_DEFAULT</c>.</para>
        /// </remarks>
        Default = 0x00,
        /// <summary>
        /// libavformat (Libav/FFmpeg)
        /// </summary>
        /// <remarks>
        /// <para>In FFMS2, the equivalent is <c>FFMS_SOURCE_LAVF</c>.</para>
        /// </remarks>
        Lavf = 0x01,
        /// <summary>
        /// Haali's BSD-licensed native Matroska parsing library
        /// </summary>
        /// <remarks>
        /// <para>In FFMS2, the equivalent is <c>FFMS_SOURCE_MATROSKA</c>.</para>
        /// </remarks>
        Matroska = 0x02,
        /// <summary>
        /// Haali's closed-source DirectShow splitter (MPEG TS/PS)
        /// </summary>
        /// <remarks>
        /// <para>In FFMS2, the equivalent is <c>FFMS_SOURCE_HAALIMPEG</c>.</para>
        /// </remarks>
        Haalimpeg = 0x04,
        /// <summary>
        /// Haali's closed-source DirectShow splitter (Ogg/OGM)
        /// </summary>
        /// <remarks>
        /// <para>In FFMS2, the equivalent is <c>FFMS_SOURCE_HAALIOGG</c>.</para>
        /// </remarks>
        Haaliogg = 0x08
    }

    #endregion

    /// <summary>
    /// Event arguments for the IndexingProgressChange delegate
    /// </summary>
    public class IndexingProgressChangeEventArgs : EventArgs
    {
        private long current;
        private long total;

        /// <summary>
        /// Current amount of indexing done
        /// </summary>
        public long Current
        { get { return current; } }
        /// <summary>
        /// Total amount of indexing to do
        /// </summary>
        public long Total
        { get { return total; } }

        internal IndexingProgressChangeEventArgs(long Current, long Total)
        {
            current = Current;
            total = Total;
        }
    }

    /// <summary>
    /// Media file indexer
    /// </summary>
    /// <remarks>
    /// <para>In FFMS2, the equivalent is <c>FFMS_Indexer</c>.</para>
    /// </remarks>
    public class Indexer : IDisposable
    {
        #region Private properties

        IntPtr FFMS_Indexer;
        bool isIndexing = false;
        bool disposed = false;

        #endregion

        #region Accessors

        /// <summary>
        /// Use this to check if the Indexer is currently working
        /// </summary>
        public bool IsIndexing
        { get { return isIndexing; } }
        /// <summary>
        /// Source module that was used to open the indexer
        /// </summary>
        /// <remarks>
        /// <para>In FFMS2, the equivalent is <c>FFMS_GetSourceTypeI</c>.</para>
        /// </remarks>
        /// <seealso cref="FFMSSharp.Index.Source"/>
        public Source Source
        { get { return (Source)NativeMethods.FFMS_GetSourceTypeI(FFMS_Indexer); } }
        /// <summary>
        /// The number of tracks
        /// </summary>
        /// <remarks>
        /// <para>In FFMS2, the equivalent is <c>FFMS_GetNumTrackI</c>.</para>
        /// <para>Does the same thing as <see cref="FFMSSharp.Index.NumberOfTracks">Index.NumberOfTracks</see> but does not require having the file indexed first.</para>
        /// </remarks>
        /// <seealso cref="FFMSSharp.Index.NumberOfTracks"/>
        public int NumberOfTracks
        { get { return NativeMethods.FFMS_GetNumTracksI(FFMS_Indexer); } }
        /// <summary>
        /// The name of the container format of the media file
        /// </summary>
        /// <remarks>
        /// <para>In FFMS2, the equivalent is <c>FFMS_GetFormatNameI</c>.</para>
        /// </remarks>
        public string FormatName
        { get { return Marshal.PtrToStringAnsi(NativeMethods.FFMS_GetFormatNameI(FFMS_Indexer)); } }

        #endregion

        #region Constructor and destructor

        /// <summary>
        /// Create an indexer of a media file
        /// </summary>
        /// <remarks>
        /// <para>In FFMS2, the equivalent is <c>FFMS_CreateIndexer</c> or <c>FFMS_CreateIndexerWithDemuxer</c>.</para>
        /// <para>The chosen demuxer gets used for both indexing and decoding later on. Only force one if you know what you're doing.</para>
        /// <para>Picking a demuxer that doesn't work on your file will not cause automatic fallback on lavf or automatic probing; it'll just cause indexer creation to fail.</para>
        /// </remarks>
        /// <param name="sourceFile">The media file</param>
        /// <param name="demuxer">What demuxer to use</param>
        /// <exception cref="System.IO.FileLoadException">Failure to load the media file</exception>
        public Indexer(string sourceFile, Source demuxer = Source.Default)
        {
            FFMS_ErrorInfo err = new FFMS_ErrorInfo();
            err.BufferSize = 1024;
            err.Buffer = new String((char)0, 1024);

            FFMS_Indexer = NativeMethods.FFMS_CreateIndexerWithDemuxer(sourceFile, (int)demuxer, ref err);

            if (FFMS_Indexer == IntPtr.Zero)
            {
                if (err.ErrorType == FFMS_Errors.FFMS_ERROR_PARSER && err.SubType == FFMS_Errors.FFMS_ERROR_FILE_READ)
                    throw new System.IO.FileLoadException(err.Buffer);

                throw new NotImplementedException(string.Format(System.Globalization.CultureInfo.CurrentCulture, "Unknown FFMS2 error encountered: ({0}, {1}, '{2}'). Please report this issue on FFMSSharp's GitHub.", err.ErrorType, err.SubType, err.Buffer));
            }
        }

        /// <summary>
        /// Releases all resources used by the <see cref="Indexer"/>.
        /// </summary>
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        /// <summary>
        /// Releases the unmanaged resources used by the <see cref="Indexer"/> and optionally releases the managed resources.
        /// </summary>
        /// <param name="disposing">true to release both managed and unmanaged resources; false to release only unmanaged resources.</param>
        protected virtual void Dispose(bool disposing)
        {
            if (!disposed)
            {
                if (FFMS_Indexer != IntPtr.Zero)
                {
                    NativeMethods.FFMS_CancelIndexing(FFMS_Indexer);
                    FFMS_Indexer = IntPtr.Zero;
                }
                disposed = true;
            }
        }

        /// <summary>
        /// Indexer destruction
        /// </summary>
        /// <remarks>
        /// <para>In FFMS2, the equivalent is <c>FFMS_CancelIndexing</c>.</para>
        /// </remarks>
        ~Indexer()
        {
            Dispose(false);
        }

        #endregion

        #region Methods

        /// <summary>
        /// Get the track type of a specific track
        /// </summary>
        /// <remarks>
        /// <para>In FFMS2, the equivalent is <c>FFMS_GetTrackTypeI</c>.</para>
        /// <para>Does the same thing as <see cref="FFMSSharp.Track.TrackType">Track.Type</see> but does not require having the file indexed first.</para>
        /// <para>If you have indexed the file, use <see cref="FFMSSharp.Track.TrackType">Track.Type</see> instead since the <c>FFMS_Indexer</c> object is destroyed when the index is created.</para>
        /// </remarks>
        /// <param name="track">Track number</param>
        /// <returns>Track type</returns>
        /// <seealso cref="FFMSSharp.Track.TrackType"/>
        /// <exception cref="ArgumentOutOfRangeException">Trying to access a Track that doesn't exist.</exception>
        public TrackType GetTrackType(int track)
        {
            if (track < 0 || track > NativeMethods.FFMS_GetNumTracksI(FFMS_Indexer))
                throw new ArgumentOutOfRangeException("track", "That track doesn't exist.");

            return (TrackType)NativeMethods.FFMS_GetTrackTypeI(FFMS_Indexer, track);
        }

        /// <summary>
        /// Get the name of the codec used for a specific track
        /// </summary>
        /// <remarks>
        /// <para>In FFMS2, the equivalent is <c>FFMS_GetCodecNameI</c>.</para>
        /// </remarks>
        /// <param name="track">Track number</param>
        /// <returns>The human-readable name ("long name" in FFmpeg terms) of the codec</returns>
        /// <exception cref="ArgumentOutOfRangeException">Trying to access a Track that doesn't exist.</exception>
        public string GetCodecName(int track)
        {
            if (track < 0 || track > NativeMethods.FFMS_GetNumTracksI(FFMS_Indexer))
                throw new ArgumentOutOfRangeException("track", "That track doesn't exist.");

            return Marshal.PtrToStringAnsi(NativeMethods.FFMS_GetCodecNameI(FFMS_Indexer, track));
        }

        #endregion

        #region Object creation

        private Index Index(int AudioIndexMask, int AudioDumpMask, IndexErrorHandling IndexErrorHandling)
        {
            IntPtr index = IntPtr.Zero;
            FFMS_ErrorInfo err = new FFMS_ErrorInfo();
            err.BufferSize = 1024;
            err.Buffer = new String((char)0, 1024);
            isIndexing = true;

            lock(this)
            {
                index = NativeMethods.FFMS_DoIndexing(FFMS_Indexer, AudioIndexMask, AudioDumpMask, AudioNameCallback, IntPtr.Zero, (int)IndexErrorHandling, IndexingCallback, IntPtr.Zero, ref err);
            }

            isIndexing = false;
            FFMS_Indexer = IntPtr.Zero;

            if (index == IntPtr.Zero)
            {
                if (err.ErrorType == FFMS_Errors.FFMS_ERROR_CODEC && err.SubType == FFMS_Errors.FFMS_ERROR_UNSUPPORTED)
                    throw new NotSupportedException(err.Buffer);
                if (err.ErrorType == FFMS_Errors.FFMS_ERROR_CODEC && err.SubType == FFMS_Errors.FFMS_ERROR_DECODING)
                    throw new System.IO.InvalidDataException(err.Buffer);
                if (err.ErrorType == FFMS_Errors.FFMS_ERROR_CANCELLED && err.SubType == FFMS_Errors.FFMS_ERROR_USER)
                    throw new OperationCanceledException(err.Buffer);
                if (err.ErrorType == FFMS_Errors.FFMS_ERROR_INDEXING && err.SubType == FFMS_Errors.FFMS_ERROR_PARSER)
                    throw new System.IO.InvalidDataException(err.Buffer);

                throw new NotImplementedException(string.Format(System.Globalization.CultureInfo.CurrentCulture, "Unknown FFMS2 error encountered: ({0}, {1}, '{2}'). Please report this issue on FFMSSharp's GitHub.", err.ErrorType, err.SubType, err.Buffer));
            }

            return new FFMSSharp.Index(index);
        }

        /// <summary>
        /// Index the media file
        /// </summary>
        /// <remarks>
        /// <para>In FFMS2, the equivalent is <c>FFMS_DoIndexing</c>.</para>
        /// <para>By default, you will index all <see cref="TrackType.Audio">Audio</see> tracks.</para>
        /// </remarks>
        /// <param name="audioIndex">A list of specific <see cref="TrackType.Audio">Audio</see> tracks to index</param>
        /// <param name="indexErrorHandling">Control behavior when a decoding error is encountered</param>
        /// <returns>The generated <see cref="FFMSSharp.Index">Index</see> object</returns>
        /// <event cref="UpdateIndexProgress">Called to give you an update on indexing progress</event>
        /// <event cref="OnIndexingCompleted">Called when the indexing has finished</event>
        /// <exception cref="NotSupportedException">Attempting to index a codec not supported by the indexer</exception>
        /// <exception cref="System.IO.InvalidDataException">Failure to index a file that should be supported</exception>
        /// <exception cref="OperationCanceledException">Canceling the indexing process</exception>
        public Index Index(IEnumerable<int> audioIndex = null, IndexErrorHandling indexErrorHandling = IndexErrorHandling.Abort)
        {
            int indexMask = -1;

            if (audioIndex != null)
            {
                indexMask = 0;
                foreach (int Track in audioIndex)
                {
                    indexMask = indexMask | (1 << Track);
                }
            }

            return Index(indexMask, 0, indexErrorHandling);
        }

        /*
         * Audio dumping is broken, so this constructor is hidden.
         * 
        public Index Index(List<int> AudioIndex, List<int> AudioDump, IndexErrorHandling IndexErrorHandling = IndexErrorHandling.Abort)
        {
            int IndexMask = 0;
            if (AudioIndex != null)
            {
                foreach (int Track in AudioIndex)
                {
                    IndexMask = IndexMask | (1 << Track);
                }
            }

            int DumpMask = 0;
            foreach (int Track in AudioDump)
            {
                DumpMask = DumpMask | (1 << Track);
            }

            return Index(IndexMask, DumpMask, IndexErrorHandling);
        }
        */

        #endregion

        #region Callback stuff

        int AudioNameCallback(string SourceFile, int Track, ref FFMS_AudioProperties AP, IntPtr FileName, int FNSize, IntPtr Private)
        {
            return NativeMethods.FFMS_DefaultAudioFilename(SourceFile, Track, ref AP, FileName, FNSize, Private);
        }

        /// <summary>
        /// Called to give you an update on indexing progress
        /// </summary>
        /// <seealso cref="IndexingProgressChangeEventArgs"/>
        public event EventHandler<IndexingProgressChangeEventArgs> UpdateIndexProgress;

        /// <summary>
        /// Called when the indexing has finished
        /// </summary>
        public event EventHandler OnIndexingCompleted;

        int IndexingCallback(long Current, long Total, IntPtr ICPrivate)
        {
            lock (this)
            {
                if (UpdateIndexProgress != null)
                    UpdateIndexProgress(this, new IndexingProgressChangeEventArgs(Current, Total));

                if (OnIndexingCompleted != null)
                    if (Current == Total)
                        OnIndexingCompleted(this, new EventArgs());
            }
            return 0;
        }

        #endregion
    }
}
