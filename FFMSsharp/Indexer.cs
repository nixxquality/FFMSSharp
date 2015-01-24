using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;

namespace FFMSSharp
{
    #region Interop

    [SuppressMessage("ReSharper", "InconsistentNaming")]
    static partial class NativeMethods
    {
        [DllImport("ffms2.dll", SetLastError = false)]
        public static extern SafeIndexerHandle FFMS_CreateIndexerWithDemuxer(byte[] SourceFile, int Demuxer, ref FFMS_ErrorInfo ErrorInfo);

        [DllImport("ffms2.dll", SetLastError = false)]
        public static extern void FFMS_CancelIndexing(IntPtr Indexer);

        [DllImport("ffms2.dll", SetLastError = false)]
        public static extern int FFMS_GetSourceTypeI(SafeIndexerHandle Indexer);

        [DllImport("ffms2.dll", SetLastError = false)]
        public static extern int FFMS_GetNumTracksI(SafeIndexerHandle Indexer);

        [DllImport("ffms2.dll", SetLastError = false)]
        public static extern int FFMS_GetTrackTypeI(SafeIndexerHandle Indexer, int Track);

        [DllImport("ffms2.dll", SetLastError = false)]
        public static extern IntPtr FFMS_GetCodecNameI(SafeIndexerHandle Indexer, int Track);

        [DllImport("ffms2.dll", SetLastError = false)]
        public static extern IntPtr FFMS_GetFormatNameI(SafeIndexerHandle Indexer);

        [DllImport("ffms2.dll", SetLastError = false)]
        public static extern int FFMS_DefaultAudioFilename(string SourceFile, int Track, ref FFMS_AudioProperties AP, StringBuilder FileName, int FNSize, IntPtr Private);

        [DllImport("ffms2.dll", SetLastError = false, BestFitMapping = false, ThrowOnUnmappableChar = true)]
        public static extern SafeIndexHandle FFMS_DoIndexing(SafeIndexerHandle Indexer, int IndexMask, int DumpMask, TAudioNameCallback ANC, [MarshalAs(UnmanagedType.LPStr)] string ANCPrivate, int ErrorHandling, TIndexCallback IC, IntPtr ICPrivate, ref FFMS_ErrorInfo ErrorInfo);

        public delegate int TAudioNameCallback(string SourceFile, int Track, ref FFMS_AudioProperties AP, StringBuilder FileName, int FNSize, IntPtr Private);
        public delegate int TIndexCallback(long Current, long Total, IntPtr ICPrivate);
    }

    internal class SafeIndexerHandle : SafeHandle
    {
        private SafeIndexerHandle()
            : base(IntPtr.Zero, true)
        {
        }

        public override bool IsInvalid
        {
            get { return handle == IntPtr.Zero; }
        }

        protected override bool ReleaseHandle()
        {
            NativeMethods.FFMS_CancelIndexing(handle);
            return true;
        }
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
        /// <summary>
        /// Current amount of indexing done
        /// </summary>
        public long Current { get; private set; }

        /// <summary>
        /// Total amount of indexing to do
        /// </summary>
        public long Total { get; private set; }

        internal IndexingProgressChangeEventArgs(long current, long total)
        {
            Current = current;
            Total = total;
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
        readonly SafeIndexerHandle _handle;

        #region Properties

        /// <summary>
        /// Use this to check if the Indexer is currently working
        /// </summary>
        public bool IsIndexing { get; private set; }

        /// <summary>
        /// Use this to cancel indexing at any point
        /// </summary>
        public bool CancelIndexing { get; set; }

        /// <summary>
        /// Source module that was used to open the indexer
        /// </summary>
        /// <remarks>
        /// <para>In FFMS2, the equivalent is <c>FFMS_GetSourceTypeI</c>.</para>
        /// </remarks>
        /// <seealso cref="FFMSSharp.Index.Source"/>
        public Source Source
        { get { return (Source)NativeMethods.FFMS_GetSourceTypeI(_handle); } }

        /// <summary>
        /// The number of tracks
        /// </summary>
        /// <remarks>
        /// <para>In FFMS2, the equivalent is <c>FFMS_GetNumTrackI</c>.</para>
        /// <para>Does the same thing as <see cref="FFMSSharp.Index.NumberOfTracks">Index.NumberOfTracks</see> but does not require having the file indexed first.</para>
        /// </remarks>
        /// <seealso cref="FFMSSharp.Index.NumberOfTracks"/>
        public int NumberOfTracks
        { get { return NativeMethods.FFMS_GetNumTracksI(_handle); } }

        /// <summary>
        /// The name of the container format of the media file
        /// </summary>
        /// <remarks>
        /// <para>In FFMS2, the equivalent is <c>FFMS_GetFormatNameI</c>.</para>
        /// </remarks>
        public string FormatName
        { get { return Marshal.PtrToStringAnsi(NativeMethods.FFMS_GetFormatNameI(_handle)); } }

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
            if (sourceFile == null) throw new ArgumentNullException(@"sourceFile");

            var err = new FFMS_ErrorInfo
            {
                BufferSize = 1024,
                Buffer = new String((char) 0, 1024)
            };

            byte[] sourceFileBytes = Encoding.UTF8.GetBytes(sourceFile);
            _handle = NativeMethods.FFMS_CreateIndexerWithDemuxer(sourceFileBytes, (int)demuxer, ref err);

            if (!_handle.IsInvalid) return;

            if (err.ErrorType == FFMS_Errors.FFMS_ERROR_PARSER && err.SubType == FFMS_Errors.FFMS_ERROR_FILE_READ)
                throw new System.IO.FileLoadException(err.Buffer);

            throw new NotImplementedException(string.Format(System.Globalization.CultureInfo.CurrentCulture, "Unknown FFMS2 error encountered: ({0}, {1}, '{2}'). Please report this issue on FFMSSharp's GitHub.", err.ErrorType, err.SubType, err.Buffer));
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
        /// Releases the unmanaged resources used by the <see cref="Indexer"/>.
        /// </summary>
        /// <param name="disposing">This doesn't do anything.</param>
        protected virtual void Dispose(bool disposing)
        {
            if (_handle != null && !_handle.IsInvalid)
            {
                _handle.Dispose();
            }
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
        /// <exception cref="ObjectDisposedException">Calling this function after you have already called <see cref="Index"/>.</exception>
        public TrackType GetTrackType(int track)
        {
            if (_handle.IsInvalid) throw new ObjectDisposedException(@"Indexer");

            if (track < 0 || track > NativeMethods.FFMS_GetNumTracksI(_handle))
                throw new ArgumentOutOfRangeException(@"track", "That track doesn't exist.");

            return (TrackType)NativeMethods.FFMS_GetTrackTypeI(_handle, track);
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
        /// <exception cref="ObjectDisposedException">Calling this function after you have already called <see cref="Index"/>.</exception>
        public string GetCodecName(int track)
        {
            if (_handle.IsInvalid) throw new ObjectDisposedException(@"Indexer");

            if (track < 0 || track > NativeMethods.FFMS_GetNumTracksI(_handle))
                throw new ArgumentOutOfRangeException(@"track", "That track doesn't exist.");

            return Marshal.PtrToStringAnsi(NativeMethods.FFMS_GetCodecNameI(_handle, track));
        }

        #endregion

        #region Object creation

        /// <summary>
        /// Index the media file
        /// </summary>
        /// <remarks>
        /// <para>In FFMS2, the equivalent is <c>FFMS_DoIndexing</c>.</para>
        /// <para>By default, you will index all <see cref="TrackType.Audio">Audio</see> tracks.</para>
        /// </remarks>
        /// <param name="audioIndex">A list of specific <see cref="TrackType.Audio">Audio</see> tracks to index</param>
        /// <param name="audioDump">A list of <see cref="TrackType.Audio">Audio</see> tracks to dump while indexing</param>
        /// <param name="audioDumpFileName">The filename format for audio tracks getting dumped
        /// <para>The following variables can be used:</para>
        /// <para><c>%sourcefile%</c> - same as the source file name, i.e. the file the audio is decoded from</para>
        /// <para><c>%trackn%</c> - the track number</para>
        /// <para><c>%trackzn%</c> - the track number zero padded to 2 digits</para>
        /// <para><c>%samplerate%</c> - the audio sample rate</para>
        /// <para><c>%channels%</c> - number of audio channels</para>
        /// <para><c>%bps%</c> - bits per sample</para>
        /// <para><c>%delay%</c> - delay, or more exactly the first timestamp encountered in the audio stream</para>
        /// <para>Example string: <c>%sourcefile%_track%trackzn%.w64</c></para></param>
        /// <param name="indexErrorHandling">Control behavior when a decoding error is encountered</param>
        /// <returns>The generated <see cref="FFMSSharp.Index">Index</see> object</returns>
        /// <event cref="UpdateIndexProgress">Called to give you an update on indexing progress</event>
        /// <event cref="OnIndexingCompleted">Called when the indexing has finished</event>
        /// <exception cref="NotSupportedException">Attempting to index a codec not supported by the indexer</exception>
        /// <exception cref="System.IO.InvalidDataException">Failure to index a file that should be supported</exception>
        /// <exception cref="OperationCanceledException">Canceling the indexing process</exception>
        /// <exception cref="ObjectDisposedException">Calling this function after you have already called <see cref="Index"/>.</exception>
        public Index Index(IEnumerable<int> audioIndex = null, IEnumerable<int> audioDump = null, string audioDumpFileName = null, IndexErrorHandling indexErrorHandling = IndexErrorHandling.Abort)
        {
            if (_handle.IsInvalid) throw new ObjectDisposedException(@"Indexer");

            var indexMask = -1;
            if (audioIndex != null)
            {
                indexMask = audioIndex.Aggregate(0, (current, track) => current | (1 << track));
            }

            var dumpMask = 0;
            if (audioDump != null)
            {
                if (audioDumpFileName == null)
                    throw new ArgumentNullException(@"audioDumpFileName", "You must specify a filename format if you want to dump audio files.");

                dumpMask = audioDump.Aggregate(dumpMask, (current, track) => current | (1 << track));
            }

            var err = new FFMS_ErrorInfo
            {
                BufferSize = 1024,
                Buffer = new String((char)0, 1024)
            };
            SafeIndexHandle index;
            IsIndexing = true;
            CancelIndexing = false;

            lock (this)
            {
                index = NativeMethods.FFMS_DoIndexing(_handle, indexMask, dumpMask, AudioNameCallback, audioDumpFileName, (int)indexErrorHandling, IndexingCallback, IntPtr.Zero, ref err);
            }

            _handle.SetHandleAsInvalid(); // "Note that calling this function destroys the FFMS_Indexer object and frees the memory allocated by FFMS_CreateIndexer (even if indexing fails for any reason)."
            IsIndexing = false;

            if (!index.IsInvalid) return new Index(index);

            if (err.ErrorType == FFMS_Errors.FFMS_ERROR_CODEC && err.SubType == FFMS_Errors.FFMS_ERROR_UNSUPPORTED)
                throw new NotSupportedException(err.Buffer);
            if (err.ErrorType == FFMS_Errors.FFMS_ERROR_UNSUPPORTED && err.SubType == FFMS_Errors.FFMS_ERROR_DECODING)
                throw new NotSupportedException(err.Buffer);
            if (err.ErrorType == FFMS_Errors.FFMS_ERROR_CODEC && err.SubType == FFMS_Errors.FFMS_ERROR_DECODING)
                throw new System.IO.InvalidDataException(err.Buffer);
            if (err.ErrorType == FFMS_Errors.FFMS_ERROR_CANCELLED && err.SubType == FFMS_Errors.FFMS_ERROR_USER)
                throw new OperationCanceledException(err.Buffer);
            if (err.ErrorType == FFMS_Errors.FFMS_ERROR_INDEXING && err.SubType == FFMS_Errors.FFMS_ERROR_PARSER)
                throw new System.IO.InvalidDataException(err.Buffer);

            throw new NotImplementedException(string.Format(System.Globalization.CultureInfo.CurrentCulture, "Unknown FFMS2 error encountered: ({0}, {1}, '{2}'). Please report this issue on FFMSSharp's GitHub.", err.ErrorType, err.SubType, err.Buffer));
        }

        #endregion

        #region Callback stuff

        [SuppressMessage("ReSharper", "InconsistentNaming")]
        static int AudioNameCallback(string SourceFile, int Track, ref FFMS_AudioProperties AP, StringBuilder FileName, int FNSize, IntPtr Private)
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

        [SuppressMessage("ReSharper", "InconsistentNaming")]
        int IndexingCallback(long Current, long Total, IntPtr ICPrivate)
        {
            lock (this)
            {
                if (UpdateIndexProgress != null) UpdateIndexProgress(this, new IndexingProgressChangeEventArgs(Current, Total));

                if (OnIndexingCompleted == null) return CancelIndexing ? 1 : 0;

                if (Current == Total) OnIndexingCompleted(this, new EventArgs());
            }
            return CancelIndexing ? 1 : 0;
        }

        #endregion
    }
}
