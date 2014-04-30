using System;
using System.Runtime.InteropServices;
using System.Runtime.Serialization;

namespace FFMSsharp
{
    #region Interop

    [StructLayout(LayoutKind.Sequential)]
    unsafe struct FFMS_ErrorInfo
    {
        internal FFMS_Errors ErrorType;
        internal FFMS_Errors SubType;
        public int BufferSize;
        [MarshalAs(UnmanagedType.LPStr)]
        public string Buffer;
    }

    #endregion

    #region Constants

    /// <summary>Used to identify errors.</summary>
    public enum FFMS_Errors
    {
        /// <summary>No error.</summary>
        FFMS_ERROR_SUCCESS = 0,

        // Main types - where the error occurred
        /// <summary>Index file handling</summary>
        FFMS_ERROR_INDEX = 1,
        /// <summary>Indexing</summary>
        FFMS_ERROR_INDEXING,
        /// <summary>Video post-processing (libpostproc)</summary>
        FFMS_ERROR_POSTPROCESSING,
        /// <summary>Image scaling (libswscale)</summary>
        FFMS_ERROR_SCALING,
        /// <summary>Audio/Video decoding</summary>
        FFMS_ERROR_DECODING,
        /// <summary>Seeking</summary>
        FFMS_ERROR_SEEKING,
        /// <summary>File parsing</summary>
        FFMS_ERROR_PARSER,
        /// <summary>Track handling</summary>
        FFMS_ERROR_TRACK,
        /// <summary>WAVE64 file writer</summary>
        FFMS_ERROR_WAVE_WRITER,
        /// <summary>Operation aborted</summary>
        FFMS_ERROR_CANCELLED,
        /// <summary>Audio re-sampling (libavresample)</summary>
        FFMS_ERROR_RESAMPLING,

        // Subtypes - what caused the error
        /// <summary>Unknown error</summary>
        FFMS_ERROR_UNKNOWN = 20,
        /// <summary>Format or operation is not supported with this binary</summary>
        FFMS_ERROR_UNSUPPORTED,
        /// <summary>Cannot read from file</summary>
        FFMS_ERROR_FILE_READ,
        /// <summary>Cannot write to file</summary>
        FFMS_ERROR_FILE_WRITE,
        /// <summary>No such file or directory</summary>
        FFMS_ERROR_NO_FILE,
        /// <summary>Wrong version</summary>
        FFMS_ERROR_VERSION,
        /// <summary>Out of memory</summary>
        FFMS_ERROR_ALLOCATION_FAILED,
        /// <summary>Invalid or nonsensical argument</summary>
        FFMS_ERROR_INVALID_ARGUMENT,
        /// <summary>Decoder error</summary>
        FFMS_ERROR_CODEC,
        /// <summary>Requested mode or operation unavailable in this binary</summary>
        FFMS_ERROR_NOT_AVAILABLE,
        /// <summary>Provided index does not match the file</summary>
        FFMS_ERROR_FILE_MISMATCH,
        /// <summary>Problem exists between keyboard and chair</summary>
        FFMS_ERROR_USER
    }

    #endregion

    #region Exceptions

    /// <summary>
    /// Generic parent class for all other FFMS exceptions
    /// </summary>
    [Serializable()]
    public class FFMSException : Exception
    {
        private FFMS_Errors ErrorType;

        /// <summary></summary>
        protected FFMSException()
            : base()
        { }

        internal FFMSException(FFMS_Errors errorType) :
            base("")
        {
            ErrorType = errorType;
        }

        internal FFMSException(FFMS_Errors errorType, string message) : 
            base(message)
        {
            ErrorType = errorType;
        }

        internal FFMSException(FFMS_Errors errorType, string message, Exception innerException) :
            base(message, innerException)
        {
            ErrorType = errorType;
        }

        /// <summary></summary>
        protected FFMSException(SerializationInfo info, StreamingContext context)
            : base(info, context)
        { }

        internal FFMS_Errors Where
        { get { return ErrorType; } }
    }

    /// <summary>
    /// Unknown error
    /// </summary>
    /// <remarks>
    /// <para>In FFMS2, the equivalent is <c>FFMS_ERROR_UNKNOWN</c>.</para>
    /// </remarks>
    public class FFMSUnknownErrorException : FFMSException
    {
        internal FFMSUnknownErrorException(FFMS_Errors errorType, string message) : base(errorType, message) { }
    }

    /// <summary>
    /// Format or operation is not supported with this binary
    /// </summary>
    /// <remarks>
    /// <para>In FFMS2, the equivalent is <c>FFMS_ERROR_UNSUPPORTED</c>.</para>
    /// </remarks>
    public class FFMSUnsupportedException : FFMSException
    {
        internal FFMSUnsupportedException(FFMS_Errors errorType, string message) : base(errorType, message) { }
    }

    /// <summary>
    /// Cannot read from file
    /// </summary>
    /// <remarks>
    /// <para>In FFMS2, the equivalent is <c>FFMS_ERROR_FILE_READ</c>.</para>
    /// </remarks>
    public class FFMSFileReadException : FFMSException
    {
        internal FFMSFileReadException(FFMS_Errors errorType, string message) : base(errorType, message) { }
    }

    /// <summary>
    /// Cannot write to file
    /// </summary>
    /// <remarks>
    /// <para>In FFMS2, the equivalent is <c>FFMS_ERROR_FILE_WRITE</c>.</para>
    /// </remarks>
    public class FFMSFileWriteException : FFMSException
    {
        internal FFMSFileWriteException(FFMS_Errors errorType, string message) : base(errorType, message) { }
    }

    /// <summary>
    /// No such file or directory
    /// </summary>
    /// <remarks>
    /// <para>In FFMS2, the equivalent is <c>FFMS_ERROR_NO_FILE</c>.</para>
    /// </remarks>
    public class FFMSNoFileException : FFMSException
    {
        internal FFMSNoFileException(FFMS_Errors errorType, string message) : base(errorType, message) { }
    }

    /// <summary>
    /// Wrong version
    /// </summary>
    /// <remarks>
    /// <para>In FFMS2, the equivalent is <c>FFMS_ERROR_VERSION</c>.</para>
    /// </remarks>
    public class FFMSVersionException : FFMSException
    {
        internal FFMSVersionException(FFMS_Errors errorType, string message) : base(errorType, message) { }
    }

    /// <summary>
    /// Out of memory
    /// </summary>
    /// <remarks>
    /// <para>In FFMS2, the equivalent is <c>FFMS_ERROR_ALLOCATION_FAILED</c>.</para>
    /// </remarks>
    public class FFMSAllocationFailedException : FFMSException
    {
        internal FFMSAllocationFailedException(FFMS_Errors errorType, string message) : base(errorType, message) { }
    }

    /// <summary>
    /// Invalid or nonsensical argument
    /// </summary>
    /// <remarks>
    /// <para>In FFMS2, the equivalent is <c>FFMS_ERROR_INVALID_ARGUMENT</c>.</para>
    /// </remarks>
    public class FFMSInvalidArgumentException : FFMSException
    {
        internal FFMSInvalidArgumentException(FFMS_Errors errorType, string message) : base(errorType, message) { }
    }

    /// <summary>
    /// Decoder error
    /// </summary>
    /// <remarks>
    /// <para>In FFMS2, the equivalent is <c>FFMS_ERROR_CODEC</c>.</para>
    /// </remarks>
    public class FFMSCodecException : FFMSException
    {
        internal FFMSCodecException(FFMS_Errors errorType, string message) : base(errorType, message) { }
    }

    /// <summary>
    /// Requested mode or operation unavailable in this binary
    /// </summary>
    /// <remarks>
    /// <para>In FFMS2, the equivalent is <c>FFMS_ERROR_NOT_AVAILABLE</c>.</para>
    /// </remarks>
    public class FFMSNotAvailableException : FFMSException
    {
        internal FFMSNotAvailableException(FFMS_Errors errorType, string message) : base(errorType, message) { }
    }

    /// <summary>
    /// Provided index does not match the file
    /// </summary>
    /// <remarks>
    /// <para>In FFMS2, the equivalent is <c>FFMS_ERROR_FILE_MISMATCH</c>.</para>
    /// </remarks>
    public class FFMSFileMismatchException : FFMSException
    {
        internal FFMSFileMismatchException(FFMS_Errors errorType, string message) : base(errorType, message) { }
    }

    /// <summary>
    /// Problem exists between keyboard and chair
    /// </summary>
    /// <remarks>
    /// <para>In FFMS2, the equivalent is <c>FFMS_ERROR_USER</c>.</para>
    /// </remarks>
    public class FFMSUserException : FFMSException
    {
        internal FFMSUserException(FFMS_Errors errorType, string message) : base(errorType, message) { }
    }

    #endregion

    static class ErrorHandling
    {
        public static Exception ExceptionFromErrorInfo(FFMS_ErrorInfo ErrorInfo)
        {
            switch (ErrorInfo.SubType)
            {
                case FFMS_Errors.FFMS_ERROR_UNKNOWN:
                    return new FFMSUnknownErrorException(ErrorInfo.ErrorType, ErrorInfo.Buffer);
                case FFMS_Errors.FFMS_ERROR_UNSUPPORTED:
                    return new FFMSUnsupportedException(ErrorInfo.ErrorType, ErrorInfo.Buffer);
                case FFMS_Errors.FFMS_ERROR_FILE_READ:
                    return new FFMSFileReadException(ErrorInfo.ErrorType, ErrorInfo.Buffer);
                case FFMS_Errors.FFMS_ERROR_FILE_WRITE:
                    return new FFMSFileWriteException(ErrorInfo.ErrorType, ErrorInfo.Buffer);
                case FFMS_Errors.FFMS_ERROR_NO_FILE:
                    return new FFMSNoFileException(ErrorInfo.ErrorType, ErrorInfo.Buffer);
                case FFMS_Errors.FFMS_ERROR_VERSION:
                    return new FFMSVersionException(ErrorInfo.ErrorType, ErrorInfo.Buffer);
                case FFMS_Errors.FFMS_ERROR_ALLOCATION_FAILED:
                    return new FFMSAllocationFailedException(ErrorInfo.ErrorType, ErrorInfo.Buffer);
                case FFMS_Errors.FFMS_ERROR_INVALID_ARGUMENT:
                    return new FFMSInvalidArgumentException(ErrorInfo.ErrorType, ErrorInfo.Buffer);
                case FFMS_Errors.FFMS_ERROR_CODEC:
                    return new FFMSCodecException(ErrorInfo.ErrorType, ErrorInfo.Buffer);
                case FFMS_Errors.FFMS_ERROR_NOT_AVAILABLE:
                    return new FFMSNotAvailableException(ErrorInfo.ErrorType, ErrorInfo.Buffer);
                case FFMS_Errors.FFMS_ERROR_FILE_MISMATCH:
                    return new FFMSFileMismatchException(ErrorInfo.ErrorType, ErrorInfo.Buffer);
                case FFMS_Errors.FFMS_ERROR_USER:
                    return new FFMSUserException(ErrorInfo.ErrorType, ErrorInfo.Buffer);
            }
            
            return new FFMSException(ErrorInfo.ErrorType, ErrorInfo.Buffer);
        }
    }
}