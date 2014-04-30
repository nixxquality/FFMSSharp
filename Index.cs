﻿using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;

namespace FFMSsharp
{
    #region Interop

    static partial class NativeMethods
    {
        [DllImport("ffms2.dll", SetLastError = false, CharSet = CharSet.Ansi)]
        public static extern IntPtr FFMS_ReadIndex(string IndexFile, ref FFMS_ErrorInfo ErrorInfo);

        [DllImport("ffms2.dll", SetLastError = false, CharSet = CharSet.Ansi)]
        public static extern void FFMS_DestroyIndex(IntPtr Index);

        [DllImport("ffms2.dll", SetLastError = false, CharSet = CharSet.Ansi)]
        public static extern int FFMS_GetSourceType(IntPtr Index);

        [DllImport("ffms2.dll", SetLastError = false, CharSet = CharSet.Ansi)]
        public static extern int FFMS_GetErrorHandling(IntPtr Index);

        [DllImport("ffms2.dll", SetLastError = false, CharSet = CharSet.Ansi)]
        public static extern int FFMS_GetFirstTrackOfType(IntPtr Index, int TrackType, ref FFMS_ErrorInfo ErrorInfo);

        [DllImport("ffms2.dll", SetLastError = false, CharSet = CharSet.Ansi)]
        public static extern int FFMS_GetFirstIndexedTrackOfType(IntPtr Index, int TrackType, ref FFMS_ErrorInfo ErrorInfo);

        [DllImport("ffms2.dll", SetLastError = false, CharSet = CharSet.Ansi)]
        public static extern int FFMS_GetNumTracks(IntPtr Index);

        [DllImport("ffms2.dll", SetLastError = false, CharSet = CharSet.Ansi)]
        public static extern int FFMS_WriteIndex(string IndexFile, IntPtr TrackIndices, ref FFMS_ErrorInfo ErrorInfo);

        [DllImport("ffms2.dll", SetLastError = false, CharSet = CharSet.Ansi)]
        public static extern int FFMS_IndexBelongsToFile(IntPtr Index, string SourceFile, ref FFMS_ErrorInfo ErrorInfo);

        [DllImport("ffms2.dll", SetLastError = false, CharSet = CharSet.Ansi)]
        public static extern IntPtr FFMS_CreateVideoSource(string SourceFile, int Track, IntPtr Index, int Threads, int SeekMode, ref FFMS_ErrorInfo ErrorInfo);

        [DllImport("ffms2.dll", SetLastError = false, CharSet = CharSet.Ansi)]
        public static extern IntPtr FFMS_CreateAudioSource(string SourceFile, int Track, IntPtr Index, int DelayMode, ref FFMS_ErrorInfo ErrorInfo);

        [DllImport("ffms2.dll", SetLastError = false, CharSet = CharSet.Ansi)]
        public static extern IntPtr FFMS_GetTrackFromIndex(IntPtr Index, int Track);
    }

    #endregion

    #region Constants

    /// <summary>
    /// Used to control the way seeking is handled
    /// </summary>
    /// <remarks>
    /// <para>In FFMS2, the equivalent is <c>FFMS_SeekMode</c>.</para>
    /// </remarks>
    /// <seealso cref="VideoSource" />
    public enum SeekMode
    {
        /// <summary>
        /// Linear access without rewind
        /// </summary>
        /// <remarks>
        /// <para>In FFMS2, the equivalent is <c>FFMS_SEEK_LINEAR_NO_RW</c>.</para>
        /// <para>Will throw an error if each successive requested frame number isn't bigger than the last one.</para>
        /// <para>Only intended for opening images but might work on well with some obscure video format.</para>
        /// </remarks>
        LinearNoRewind = -1,
        /// <summary>
        /// Linear access
        /// </summary>
        /// <remarks>
        /// <para>In FFMS2, the equivalent is <c>FFMS_SEEK_LINEAR</c>.</para>
        /// <para>If you request frame n without having requested frames 0 to n-1 in order first, all frames from 0 to n will have to be decoded before n can be delivered.</para>
        /// <para>The definition of slow, but should make some formats "usable".</para>
        /// </remarks>
        Linear = 0,
        /// <summary>
        /// Safe normal
        /// </summary>
        /// <remarks>
        /// <para>In FFMS2, the equivalent is <c>FFMS_SEEK_NORMAL</c>.</para>
        /// <para>Bases seeking decisions on the keyframe positions reported by libavformat.</para>
        /// </remarks>
        Normal = 1,
        /// <summary>
        /// Unsafe normal
        /// </summary>
        /// <remarks>
        /// <para>In FFMS2, the equivalent is <c>FFMS_SEEK_UNSAFE</c>.</para>
        /// <para>Same as <see cref="Normal" /> but no error will be thrown if the exact destination has to be guessed.</para>
        /// </remarks>
        Unsafe = 2,
        /// <summary>
        /// Aggressive
        /// </summary>
        /// <remarks>
        /// <para>In FFMS2, the equivalent is <c>FFMS_SEEK_AGGRESSIVE</c>.</para>
        /// <para>Seeks in the forward direction even if no closer keyframe is known to exist.</para>
        /// <para>Only useful for testing and containers where libavformat doesn't report keyframes properly.</para>
        /// </remarks>
        Aggressive = 3
    }

    /// <summary>
    /// Used to control behavior when a decoding error is encountered
    /// </summary>
    /// <remarks>
    /// <para>In FFMS2, the equivalent is <c>FFMS_IndexErrorHandling</c>.</para>
    /// </remarks>
    public enum IndexErrorHandling
    {
        /// <summary>
        /// Abort indexing and raise an exception
        /// </summary>
        /// <remarks>
        /// <para>In FFMS2, the equivalent is <c>FFMS_IEH_ABORT</c>.</para>
        /// </remarks>
        Abort = 0,
        /// <summary>
        /// Clear all indexing entries for the track
        /// </summary>
        /// <remarks>
        /// <para>In FFMS2, the equivalent is <c>FFMS_IEH_CLEAR_TRACK</c>.</para>
        /// <para>Returns a blank track.</para>
        /// </remarks>
        ClearTrack = 1,
        /// <summary>
        /// Stop indexing but keep previous indexing entries
        /// </summary>
        /// <remarks>
        /// <para>In FFMS2, the equivalent is <c>FFMS_IEH_STOP_TRACK</c>.</para>
        /// <para>Returns a track that stops where the error occurred.</para>
        /// </remarks>
        StopTrack = 2,
        /// <summary>
        /// Ignore the error and pretend it's raining
        /// </summary>
        /// <remarks>
        /// <para>In FFMS2, the equivalent is <c>FFMS_IEH_IGNORE</c>.</para>
        /// </remarks>
        Ignore = 3
    }

    /// <summary>
    /// Controls how audio with a non-zero first PTS is handled
    /// </summary>
    /// <remarks>
    /// <para>In FFMS2, the equivalent is <c>FFMS_AudioDelayModes</c>.</para>
    /// <para>In other words: what FFMS does about audio delay.</para>
    /// </remarks>
    public enum AudioDelayModes
    {
        /// <summary>
        /// No adjustment is made
        /// </summary>
        /// <remarks>
        /// <para>In FFMS2, the equivalent is <c>FFMS_DELAY_NO_SHIFT</c>.</para>
        /// <para>The first decodable audio sample becomes the first sample in the output.</para>
        /// <para>May lead to audio/video desync.</para>
        /// </remarks>
        NoShift = -3,
        /// <summary>
        /// Samples are created (with silence) or discarded so that sample 0 in the decoded audio starts at time zero
        /// </summary>
        /// <remarks>
        /// <para>In FFMS2, the equivalent is <c>FFMS_DELAY_TIME_ZERO</c>.</para>
        /// </remarks>
        TimeZero = -2,
        /// <summary>
        /// Samples are created (with silence) or discarded so that sample 0 in the decoded audio starts at the same time as frame 0 of the first video track
        /// </summary>
        /// <remarks>
        /// <para>In FFMS2, the equivalent is <c>FFMS_DELAY_FIRST_VIDEO_TRACK</c>.</para>
        /// <para>This is what most users want and is a sane default.</para>
        /// </remarks>
        FirstVideoTrack = -1
    }

    #endregion

    /// <summary>
    /// Index of a media file
    /// </summary>
    /// <remarks>
    /// <para>In FFMS2, the equivalent is <c>FFMS_Index</c>.</para>
    /// <para>To get an Index for a media file you haven't indexed yet, use the <see cref="Indexer">Indexer</see> class.</para>
    /// </remarks>
    public class Index : IDisposable
    {
        IntPtr FFMS_Index;
        bool disposed = false;

        #region Constructor and destructor

        /// <summary>
        /// Read an index from disk
        /// </summary>
        /// <remarks>
        /// <para>In FFMS2, the equivalent is <c>FFMS_ReadIndex</c>.</para>
        /// </remarks>
        /// <param name="IndexFile">Can be an absolute or relative path</param>
        /// <exception cref="IOException">Trying to read an invalid index file.</exception>
        /// <exception cref="NotSupportedException">Trying to read an index file for a <see cref="Sources">Source</see> that is not available in the ffms2.dll.</exception>
        public Index(string IndexFile)
        {
            FFMS_ErrorInfo err = new FFMS_ErrorInfo();
            err.BufferSize = 1024;
            err.Buffer = new String((char)0, 1024);

            FFMS_Index = NativeMethods.FFMS_ReadIndex(IndexFile, ref err);

            if (FFMS_Index == IntPtr.Zero)
            {
                if (err.ErrorType == FFMS_Errors.FFMS_ERROR_PARSER && err.SubType == FFMS_Errors.FFMS_ERROR_FILE_READ)
                    throw new System.IO.IOException(err.Buffer);
                if (err.ErrorType == FFMS_Errors.FFMS_ERROR_INDEX && err.SubType == FFMS_Errors.FFMS_ERROR_NOT_AVAILABLE)
                    throw new NotSupportedException(err.Buffer);

                throw new NotImplementedException(string.Format("Unknown FFMS2 error encountered: ({0}, {1}, '{2}'). Please report this issue on FFMSsharp's GitHub.", err.ErrorType, err.SubType, err.Buffer));
            }
        }

        internal Index(IntPtr Index)
        {
            FFMS_Index = Index;
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (!disposed)
            {
                if (FFMS_Index != IntPtr.Zero)
                {
                    NativeMethods.FFMS_DestroyIndex(FFMS_Index);
                    FFMS_Index = IntPtr.Zero;
                }
                disposed = true;
            }
        }

        /// <summary>
        /// Index destruction
        /// </summary>
        /// <remarks>
        /// <para>In FFMS2, the equivalent is <c>FFMS_DestroyIndex</c>.</para>
        /// </remarks>
        ~Index()
        {
            Dispose(false);
        }

        #endregion

        #region Methods

        /// <summary>
        /// Get which source module was used in creating the index
        /// </summary>
        /// <remarks>
        /// <para>In FFMS2, the equivalent is <c>FFMS_GetSourceType</c>.</para>
        /// </remarks>
        /// <returns>Source module</returns>
        /// <seealso cref="FFMSsharp.Indexer.GetSourceType"/>
        public Sources GetSourceType()
        {
            return (Sources)NativeMethods.FFMS_GetSourceType(FFMS_Index);
        }

        /// <summary>
        /// Get which error handling method was used in creating of the index
        /// </summary>
        /// <remarks>
        /// <para>In FFMS2, the equivalent is <c>FFMS_GetErrorHandling</c>.</para>
        /// </remarks>
        /// <returns>Error handling method</returns>
        /// <seealso cref="FFMSsharp.Indexer.Index(FFMSsharp.IndexErrorHandling)"/>
        /// <seealso cref="FFMSsharp.Indexer.Index(List&lt;int&gt;, FFMSsharp.IndexErrorHandling)"/>
        public IndexErrorHandling GetErrorHandling()
        {
            return (IndexErrorHandling)NativeMethods.FFMS_GetErrorHandling(FFMS_Index);
        }

        /// <summary>
        /// Get the track number of the first track of a specific type
        /// </summary>
        /// <remarks>
        /// <para>In FFMS2, the equivalent is <c>FFMS_GetFirstTrackOfType</c>.</para>
        /// </remarks>
        /// <param name="Type">Track type</param>
        /// <returns>Track number</returns>
        /// <seealso cref="GetFirstIndexedTrackOfType"/>
        /// <exception cref="System.Collections.Generic.KeyNotFoundException">Trying to find a type of track that doesn't exist in the media file.</exception>
        public int GetFirstTrackOfType(TrackType Type)
        {
            FFMS_ErrorInfo err = new FFMS_ErrorInfo();
            err.BufferSize = 1024;
            err.Buffer = new String((char)0, 1024);

            int track = NativeMethods.FFMS_GetFirstTrackOfType(FFMS_Index, (int)Type, ref err);

            if (track < 0)
            {
                if (err.ErrorType == FFMS_Errors.FFMS_ERROR_INDEX && err.SubType == FFMS_Errors.FFMS_ERROR_NOT_AVAILABLE)
                    throw new System.Collections.Generic.KeyNotFoundException(err.Buffer);

                throw new NotImplementedException(string.Format("Unknown FFMS2 error encountered: ({0}, {1}, '{2}'). Please report this issue on FFMSsharp's GitHub.", err.ErrorType, err.SubType, err.Buffer));
            }

            return track;
        }

        /// <summary>
        /// Get the track number of the first indexed track of a specific type
        /// </summary>
        /// <remarks>
        /// <para>In FFMS2, the equivalent is <c>FFMS_GetFirstIndexedTrackOfType</c>.</para>
        /// <para>Does the exact same thing as <see cref="GetFirstTrackOfType">GetFirstTrackOfType</see> but ignores tracks that have not been indexed.</para>
        /// </remarks>
        /// <param name="Type">Track type</param>
        /// <returns>Track number</returns>
        /// <seealso cref="GetFirstTrackOfType"/>
        /// <exception cref="System.Collections.Generic.KeyNotFoundException">Trying to find a type of track that doesn't exist in the media file.</exception>
        public int GetFirstIndexedTrackOfType(TrackType Type)
        {
            FFMS_ErrorInfo err = new FFMS_ErrorInfo();
            err.BufferSize = 1024;
            err.Buffer = new String((char)0, 1024);

            int track = NativeMethods.FFMS_GetFirstIndexedTrackOfType(FFMS_Index, (int)Type, ref err);

            if (track < 0)
            {
                if (err.ErrorType == FFMS_Errors.FFMS_ERROR_INDEX && err.SubType == FFMS_Errors.FFMS_ERROR_NOT_AVAILABLE)
                    throw new System.Collections.Generic.KeyNotFoundException(err.Buffer);

                throw new NotImplementedException(string.Format("Unknown FFMS2 error encountered: ({0}, {1}, '{2}'). Please report this issue on FFMSsharp's GitHub.", err.ErrorType, err.SubType, err.Buffer));
            }

            return track;
        }

        /// <summary>
        /// Get the total number of tracks in the index
        /// </summary>
        /// <remarks>
        /// <para>In FFMS2, the equivalent is <c>FFMS_GetNumTracks</c>.</para>
        /// </remarks>
        /// <returns>Total number of tracks</returns>
        public int GetNumTracks()
        {
            return NativeMethods.FFMS_GetNumTracks(FFMS_Index);
        }

        /// <summary>
        /// Write the index to disk
        /// </summary>
        /// <remarks>
        /// <para>In FFMS2, the equivalent is <c>FFMS_WriteIndex</c>.</para>
        /// </remarks>
        /// <param name="IndexFile">Can be an absolute or relative path; it will be truncated and overwritten if it already exists</param>
        /// <exception cref="FFMSException"/>
        public void WriteIndex(string IndexFile)
        {
            FFMS_ErrorInfo err = new FFMS_ErrorInfo();
            err.BufferSize = 1024;
            err.Buffer = new String((char)0, 1024);

            if (NativeMethods.FFMS_WriteIndex(IndexFile, FFMS_Index, ref err) != 0)
                throw ErrorHandling.ExceptionFromErrorInfo(err);
        }

        /// <summary>
        /// Check if the index belongs to a specific file
        /// </summary>
        /// <remarks>
        /// <para>In FFMS2, the equivalent is <c>FFMS_IndexBelongsToFile</c>.</para>
        /// <para>Makes a heuristic (but very reliable) guess about whether the index is of the <paramref name="SourceFile"/> or not.</para>
        /// <para>Useful to determine if the index object you just created by <see cref="FFMSsharp.Index.Index(string)">loading an index file from disk</see> is actually relevant to your interests, since the only two ways to pair up index files with source files are a) trust the user blindly, or b) comparing the filenames; neither is very reliable.</para>
        /// </remarks>
        /// <param name="SourceFile">File to check against</param>
        /// <returns>True or false depending on the result</returns>
        public bool BelongsToFile(string SourceFile)
        {
            FFMS_ErrorInfo err = new FFMS_ErrorInfo();
            err.BufferSize = 1024;
            err.Buffer = new String((char)0, 1024);

            if (NativeMethods.FFMS_IndexBelongsToFile(FFMS_Index, SourceFile, ref err) != 0)
            {
                if (err.ErrorType == FFMS_Errors.FFMS_ERROR_INDEX && err.SubType == FFMS_Errors.FFMS_ERROR_FILE_MISMATCH)
                    return false;

                throw new NotImplementedException(string.Format("Unknown FFMS2 error encountered: ({0}, {1}, '{2}'). Please report this issue on FFMSsharp's GitHub.", err.ErrorType, err.SubType, err.Buffer));
            }

            return true;
        }

        #endregion

        #region Object creation

        /// <summary>
        /// Create a <see cref="FFMSsharp.VideoSource">VideoSource object</see>
        /// </summary>
        /// <remarks>
        /// <para>In FFMS2, the equivalent is <c>FFMS_CreateVideoSource</c>.</para>
        /// <para>Note that the index object is copied into the <see cref="FFMSsharp.VideoSource">VideoSource object</see> upon its creation, so once you've created the video source you can generally destroy the index object immediately, since all info you can retrieve from it is also retrievable from the <see cref="FFMSsharp.VideoSource">VideoSource object</see>.</para>
        /// </remarks>
        /// <param name="SourceFile">The media file. Can be an absolute or relative path</param>
        /// <param name="Track">Track number of the specific video track</param>
        /// <param name="Threads">Number of threads used for decoding
        /// <para>Anything less than 1 will use threads equal to the number of CPU cores.</para>
        /// <para>Values &gt;1 have no effect if FFmpeg was not compiled with threading support.</para></param>
        /// <param name="SeekMode">Controls how seeking (random access) is handled and hence affects frame accuracy
        /// <para>Has no effect on Matroska files, where the equivalent of Normal is always used.</para>
        /// <para>LinearNoRw may come in handy if you want to open images.</para></param>
        /// <returns>The generated <see cref="FFMSsharp.VideoSource">VideoSource object</see></returns>
        /// <exception cref="FFMSException"/>
        /// <seealso cref="AudioSource"/>
        /// <seealso cref="GetFirstTrackOfType"/>
        /// <seealso cref="GetFirstIndexedTrackOfType"/>
        public VideoSource VideoSource(string SourceFile, int Track, int Threads = 1, SeekMode SeekMode = SeekMode.Normal)
        {
            IntPtr videoSource = IntPtr.Zero;
            FFMS_ErrorInfo err = new FFMS_ErrorInfo();
            err.BufferSize = 1024;
            err.Buffer = new String((char)0, 1024);

            videoSource = NativeMethods.FFMS_CreateVideoSource(SourceFile, Track, FFMS_Index, Threads, (int)SeekMode, ref err);

            if (videoSource == IntPtr.Zero)
                throw ErrorHandling.ExceptionFromErrorInfo(err);

            return new FFMSsharp.VideoSource(videoSource);
        }

        /// <summary>
        /// Create an <see cref="FFMSsharp.AudioSource">AudioSource object</see>
        /// </summary>
        /// <remarks>
        /// <para>In FFMS2, the equivalent is <c>FFMS_CreateAudioSource</c>.</para>
        /// <para>Note that the index object is copied into the <see cref="FFMSsharp.AudioSource">AudioSource object</see> upon its creation, so once you've created the video source you can generally destroy the index object immediately, since all info you can retrieve from it is also retrievable from the <see cref="FFMSsharp.AudioSource">AudioSource object</see>.</para>
        /// </remarks>
        /// <param name="SourceFile">The media file. Can be an absolute or relative path</param>
        /// <param name="Track">Track number of the specific audio track</param>
        /// <param name="DelayMode">Controls how audio with a non-zero first PTS is handled; in other words what FFMS does about audio delay.</param>
        /// <returns>The generated <see cref="FFMSsharp.AudioSource">AudioSource object</see></returns>
        /// <exception cref="FFMSException"/>
        /// <seealso cref="VideoSource"/>
        /// <seealso cref="GetFirstTrackOfType"/>
        /// <seealso cref="GetFirstIndexedTrackOfType"/>
        public AudioSource AudioSource(string SourceFile, int Track, AudioDelayModes DelayMode = AudioDelayModes.FirstVideoTrack)
        {
            IntPtr audioSource = IntPtr.Zero;
            FFMS_ErrorInfo err = new FFMS_ErrorInfo();
            err.BufferSize = 1024;
            err.Buffer = new String((char)0, 1024);

            audioSource = NativeMethods.FFMS_CreateAudioSource(SourceFile, Track, FFMS_Index, (int)DelayMode, ref err);

            if (audioSource == IntPtr.Zero)
                throw ErrorHandling.ExceptionFromErrorInfo(err);

            return new FFMSsharp.AudioSource(audioSource);
        }

        /// <summary>
        /// Create a <see cref="FFMSsharp.Track">Track object</see>
        /// </summary>
        /// <remarks>
        /// <para>In FFMS2, the equivalent is <c>FFMS_GetTrackFromIndex</c>.</para>
        /// <para>Use this function if you don't want to (or cannot) open the track with <see cref="VideoSource">VideoSource</see> or <see cref="AudioSource">AudioSource</see> first.</para>
        /// <para>If you already have a <see cref="FFMSsharp.VideoSource">VideoSource object</see> or <see cref="FFMSsharp.AudioSource">AudioSource object</see> it's safer to use <see cref="FFMSsharp.VideoSource.GetTrack">GetTrack</see>/<see cref="FFMSsharp.AudioSource.GetTrack">GetTrack</see> instead.</para>
        /// <para>The returned <see cref="FFMSsharp.Track">Track object</see> is only valid until its parent <see cref="Index">Index object</see> is destroyed.</para>
        /// <para>Requesting indexing information for a track that has not been indexed will not cause an error, it will just return an empty FFMS_Track (check for >0 frames using <see cref="FFMSsharp.Track.GetNumFrames">GetNumFrames</see> to see if the returned object actually contains indexing information).</para>
        /// </remarks>
        /// <param name="Track">Track number</param>
        /// <returns>The generated <see cref="FFMSsharp.Track">Track object</see></returns>
        /// <seealso cref="GetFirstTrackOfType"/>
        /// <seealso cref="GetFirstIndexedTrackOfType"/>
        /// <exception cref="ArgumentOutOfRangeException">Trying to access a Track that doesn't exist.</exception>
        public Track GetTrack(int Track)
        {
            IntPtr track = IntPtr.Zero;

            if (Track < 0 || Track > NativeMethods.FFMS_GetNumTracks(FFMS_Index))
                throw new ArgumentOutOfRangeException("Track", "That track doesn't exist.");

            track = NativeMethods.FFMS_GetTrackFromIndex(FFMS_Index, Track);

            return new FFMSsharp.Track(track);
        }

        #endregion
    }
}
