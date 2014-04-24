using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;

namespace FFMSsharp
{
    #region Interop

    static partial class Interop
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
    public enum Sources
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
    /// <para>In FFMS2, the equivalent is <c>FFMS2_Indexer</c>.</para>
    /// </remarks>
    public class Indexer
    {
        #region Private properties

        private IntPtr FFMS_Indexer;
        private bool isIndexing = false;

        #endregion

        #region Accessors

        /// <summary>
        /// Use this to check if the Indexer is currently working
        /// </summary>
        public bool IsIndexing
        { get { return isIndexing; } }

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
        /// <param name="SourceFile">The media file</param>
        /// <param name="Demuxer">What demuxer to use</param>
        public Indexer(string SourceFile, Sources Demuxer = Sources.Default)
        {
            FFMS_ErrorInfo err = new FFMS_ErrorInfo();
            err.BufferSize = 1024;
            err.Buffer = new String((char)0, 1024);

            FFMS_Indexer = Interop.FFMS_CreateIndexerWithDemuxer(SourceFile, (int)Demuxer, ref err);

            if (FFMS_Indexer == IntPtr.Zero)
                throw ErrorHandling.ExceptionFromErrorInfo(err);
        }

        /// <summary>
        /// Indexer destruction
        /// </summary>
        /// <remarks>
        /// <para>In FFMS2, the equivalent is <c>FFMS_CancelIndexing</c>.</para>
        /// </remarks>
        ~Indexer()
        {
            if (FFMS_Indexer != IntPtr.Zero)
                Interop.FFMS_CancelIndexing(FFMS_Indexer);
        }

        #endregion

        #region Methods

        /// <summary>
        /// Get which source module was used to open the indexer
        /// </summary>
        /// <remarks>
        /// <para>In FFMS2, the equivalent is <c>FFMS_GetSourceTypeI</c>.</para>
        /// </remarks>
        /// <returns>Source module</returns>
        /// <seealso cref="FFMSsharp.Index.GetSourceType"/>
        public Sources GetSourceType()
        {
            return (Sources)Interop.FFMS_GetSourceTypeI(FFMS_Indexer);
        }

        /// <summary>
        /// Get the number of tracks
        /// </summary>
        /// <remarks>
        /// <para>In FFMS2, the equivalent is <c>FFMS_GetNumTrackI</c>.</para>
        /// <para>Does the same thing as <see cref="FFMSsharp.Index.GetNumTracks">Index.GetNumTracks</see> but does not require having the file indexed first.</para>
        /// </remarks>
        /// <returns>Total number of tracks</returns>
        /// <seealso cref="FFMSsharp.Index.GetNumTracks"/>
        public int GetNumTracks()
        {
            return Interop.FFMS_GetNumTracksI(FFMS_Indexer);
        }

        /// <summary>
        /// Get the track type of a specific track
        /// </summary>
        /// <remarks>
        /// <para>In FFMS2, the equivalent is <c>FFMS_GetTrackTypeI</c>.</para>
        /// <para>Does the same thing as <see cref="FFMSsharp.Track.Type">Track.Type</see> but does not require having the file indexed first.</para>
        /// <para>If you have indexed the file, use <see cref="FFMSsharp.Track.Type">Track.Type</see> instead since the <c>FFMS_Indexer</c> object is destroyed when the index is created.</para>
        /// <para>Note that specifying an invalid track number may lead to undefined behavior.</para>
        /// </remarks>
        /// <param name="Track">Track number</param>
        /// <returns>Track type</returns>
        /// <seealso cref="FFMSsharp.Track.Type"/>
        public TrackType GetTrackType(int Track)
        {
            return (TrackType)Interop.FFMS_GetTrackTypeI(FFMS_Indexer, Track);
        }

        /// <summary>
        /// Get the name of the codec used for a specific track
        /// </summary>
        /// <remarks>
        /// <para>In FFMS2, the equivalent is <c>FFMS2_GetCodecNameI</c>.</para>
        /// <para>Note that specifying an invalid track number may lead to undefined behavior.</para>
        /// </remarks>
        /// <param name="Track">Track number</param>
        /// <returns>The human-readable name ("long name" in FFmpeg terms) of the codec</returns>
        public string GetCodecName(int Track)
        {
            return Marshal.PtrToStringAnsi(Interop.FFMS_GetCodecNameI(FFMS_Indexer, Track));
        }

        /// <summary>
        /// Get the name of the container format of the media file
        /// </summary>
        /// <remarks>
        /// <para>In FFMS2, the equivalent is <c>FFMS2_GetFormatNameI</c>.</para>
        /// </remarks>
        /// <returns>The human-readable name ("long name" in FFmpeg terms) of the format</returns>
        public string GetFormatName()
        {
            return Marshal.PtrToStringAnsi(Interop.FFMS_GetFormatNameI(FFMS_Indexer));
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
                index = Interop.FFMS_DoIndexing(FFMS_Indexer, AudioIndexMask, AudioDumpMask, AudioNameCallback, IntPtr.Zero, (int)IndexErrorHandling, IndexingCallback, IntPtr.Zero, ref err);
            }

            isIndexing = false;
            FFMS_Indexer = IntPtr.Zero;

            if (index == IntPtr.Zero)
                throw ErrorHandling.ExceptionFromErrorInfo(err);

            return new FFMSsharp.Index(index);
        }

        /// <summary>
        /// Index the media file
        /// </summary>
        /// <remarks>
        /// <para>In FFMS2, the equivalent is <c>FFMS2_DoIndexing</c>.</para>
        /// <para>This overload will index all <see cref="TrackType.Audio">Audio</see> tracks.</para>
        /// </remarks>
        /// <param name="IndexErrorHandling">Control behavior when a decoding error is encountered</param>
        /// <returns>The generated <see cref="FFMSsharp.Index">Index</see> object</returns>
        /// <event cref="UpdateIndexProgress">Called to give you an update on indexing progress</event>
        /// <event cref="OnIndexingCompleted">Called when the indexing has finished</event>
        /// <exception cref="FFMSException"/>
        public Index Index(IndexErrorHandling IndexErrorHandling = IndexErrorHandling.Abort)
        {
            return Index(-1, 0, IndexErrorHandling);
        }

        /// <summary>
        /// Index the media file
        /// </summary>
        /// <remarks>
        /// <para>In FFMS2, the equivalent is <c>FFMS2_DoIndexing</c>.</para>
        /// </remarks>
        /// <param name="AudioIndex">A list of specific <see cref="TrackType.Audio">Audio</see> tracks to index</param>
        /// <param name="IndexErrorHandling">Control behavior when a decoding error is encountered</param>
        /// <returns>The generated <see cref="FFMSsharp.Index">Index</see> object.</returns>
        /// <event cref="UpdateIndexProgress">Called to give you an update on indexing progress</event>
        /// <event cref="OnIndexingCompleted">Called when the indexing has finished</event>
        /// <exception cref="FFMSException"/>
        public Index Index(List<int> AudioIndex, IndexErrorHandling IndexErrorHandling = IndexErrorHandling.Abort)
        {
            int IndexMask = 0;
            foreach (int Track in AudioIndex)
            {
                IndexMask = IndexMask | (1 << Track);
            }

            return Index(IndexMask, 0, IndexErrorHandling);
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
            return Interop.FFMS_DefaultAudioFilename(SourceFile, Track, ref AP, FileName, FNSize, Private);
        }

        /// <summary>
        /// Delegate for the <see cref="UpdateIndexProgress">UpdateIndexProgress</see> event
        /// </summary>
        /// <param name="sender">The indexer</param>
        /// <param name="e">Progress</param>
        public delegate void IndexingProgressChange(object sender, IndexingProgressChangeEventArgs e);
        /// <summary>
        /// Called to give you an update on indexing progress
        /// </summary>
        /// <seealso cref="IndexingProgressChange"/>
        /// <seealso cref="IndexingProgressChangeEventArgs"/>
        public event IndexingProgressChange UpdateIndexProgress;

        /// <summary>
        /// Delegate for the <see cref="OnIndexingCompleted">OnIndexingCompleted</see> event
        /// </summary>
        /// <param name="sender">The indexer</param>
        public delegate void IndexingCompleted(object sender);
        /// <summary>
        /// Called when the indexing has finished
        /// </summary>
        /// <seealso cref="IndexingCompleted"/>
        public event IndexingCompleted OnIndexingCompleted;

        int IndexingCallback(long Current, long Total, IntPtr ICPrivate)
        {
            lock (this)
            {
                if (UpdateIndexProgress != null)
                    UpdateIndexProgress(this, new IndexingProgressChangeEventArgs(Current, Total));

                if (OnIndexingCompleted != null)
                    if (Current == Total)
                        OnIndexingCompleted(this);
            }
            return 0;
        }

        #endregion
    }
}
