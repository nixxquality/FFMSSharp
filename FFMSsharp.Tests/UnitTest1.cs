using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using FFMSSharp;
using System.Drawing;
using System.Collections.Generic;
using System.IO;
using System.Security.Cryptography;

namespace Tests
{
    [TestClass]
    public class UnitTest1
    {
        [TestInitialize]
        public void Initialize()
        {
            FFMS2.Initialize();
            Assert.IsTrue(FFMS2.Initialized);
            Console.WriteLine(FFMS2.VersionString);
        }

        [TestMethod]
        public void GetSetLogLevel()
        {
            FFMS2.LogLevel = AVLogLevel.Debug;
            Assert.AreEqual(AVLogLevel.Debug, FFMS2.LogLevel);
            FFMS2.LogLevel = AVLogLevel.Quiet;
            Assert.AreEqual(AVLogLevel.Quiet, FFMS2.LogLevel);
        }

        [TestMethod]
        public void GetPixFmt()
        {
            Assert.AreNotEqual(-1, FFMS2.GetPixelFormat("yuv420p"));
            Assert.AreEqual(-1, FFMS2.GetPixelFormat("none"));
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public void GetPixFmtUTF8()
        {
            FFMS2.GetPixelFormat("おはよう");
        }

        [TestMethod]
        public void IndexerMatroska()
        {
            Indexer indexer = new Indexer("h264_720p_hp_5.1_3mbps_vorbis_styled_and_unstyled_subs_suzumiya.mkv");

            Assert.AreEqual(5, indexer.NumberOfTracks);
            Assert.AreEqual(FFMSSharp.TrackType.Video, indexer.GetTrackType(0));
            Assert.AreEqual("matroska", indexer.FormatName);
            Assert.AreEqual("h264", indexer.GetCodecName(0));
        }

        [TestMethod]
        public void IndexerUTF8()
        {
            Indexer indexer = new Indexer("おはよう.mkv");
        }

        [TestMethod]
        [ExpectedException(typeof(FileLoadException))]
        public void IndexerFileNotFound()
        {
            Indexer indexer = new Indexer("this file doesn't exist.avi");
        }

        [TestMethod]
        [ExpectedException(typeof(OperationCanceledException))]
        public void IndexerCancelIndexing()
        {
            Indexer indexer = new Indexer("h264_720p_hp_5.1_3mbps_vorbis_styled_and_unstyled_subs_suzumiya.mkv");
            indexer.UpdateIndexProgress += delegate(object sender, IndexingProgressChangeEventArgs e)
            {
                indexer.CancelIndexing = true;
            };

            indexer.Index();
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentOutOfRangeException))]
        public void IndexerGetTrackTypeOutOfRange()
        {
            Indexer indexer = new Indexer("h264_720p_hp_5.1_3mbps_vorbis_styled_and_unstyled_subs_suzumiya.mkv");
            indexer.GetTrackType(6);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentOutOfRangeException))]
        public void IndexerGetCodecNameOutOfRange()
        {
            Indexer indexer = new Indexer("h264_720p_hp_5.1_3mbps_vorbis_styled_and_unstyled_subs_suzumiya.mkv");
            indexer.GetCodecName(6);
        }

        [TestMethod]
        public void IndexAndAPIFunctions()
        {
            Indexer indexer = new Indexer("h264_720p_hp_5.1_3mbps_vorbis_styled_and_unstyled_subs_suzumiya.mkv");

            Index index = indexer.Index();

            Assert.AreEqual(Source.Matroska, index.Source);
            Assert.AreEqual(IndexErrorHandling.Abort, index.IndexErrorHandling);
            Assert.AreEqual(1, index.GetFirstTrackOfType(TrackType.Audio));
            Assert.AreEqual(1, index.GetFirstIndexedTrackOfType(TrackType.Audio));

            index.WriteIndex("h264_720p_hp_5.1_3mbps_vorbis_styled_and_unstyled_subs_suzumiya.ffindex");
        }

        [TestMethod]
        public void IndexWriteIndexUTF8()
        {
            Indexer indexer = new Indexer("おはよう.mkv");

            Index index = indexer.Index();

            index.WriteIndex("おはよう.ffindex");
        }

        [TestMethod]
        [ExpectedException(typeof(System.Collections.Generic.KeyNotFoundException))]
        public void IndexGetFirstTrackOfTypeNotAvailable()
        {
            Index index = new Index("h264_720p_hp_5.1_3mbps_vorbis_styled_and_unstyled_subs_suzumiya.ffindex");

            index.GetFirstTrackOfType(TrackType.Data);
        }

        [TestMethod]
        [ExpectedException(typeof(System.Collections.Generic.KeyNotFoundException))]
        public void IndexGetFirstIndexedTrackOfTypeNotAvailable()
        {
            Index index = new Index("h264_720p_hp_5.1_3mbps_vorbis_styled_and_unstyled_subs_suzumiya.ffindex");

            index.GetFirstIndexedTrackOfType(TrackType.Data);
        }

        [TestMethod]
        public void IndexAudioIndex()
        {
            Indexer indexer = new Indexer("h264_720p_hp_3.1_600kbps_aac_mp3_dual_audio_harry_potter.mkv");

            List<int> AudioIndexList = new List<int>();
            AudioIndexList.Add(2);
            Index index = indexer.Index(AudioIndexList);

            Assert.AreEqual(1, index.GetFirstTrackOfType(TrackType.Audio));
            Assert.AreEqual(2, index.GetFirstIndexedTrackOfType(TrackType.Audio));
        }

        [TestMethod]
        public void ReadIndex()
        {
            Index index = new Index("h264_720p_hp_5.1_3mbps_vorbis_styled_and_unstyled_subs_suzumiya.ffindex");
            
            Assert.IsTrue(index.BelongsToFile("h264_720p_hp_5.1_3mbps_vorbis_styled_and_unstyled_subs_suzumiya.mkv"));
        }

        [TestMethod]
        public void ReadIndexUTF8()
        {
            Index index = new Index("おはよう.ffindex");

            Assert.IsTrue(index.BelongsToFile("おはよう.mkv"));
        }

        [TestMethod]
        [ExpectedException(typeof(IOException))]
        public void ReadIndexFileNotFound()
        {
            Index index = new Index("this file doesn't exist.avi");
        }
        
        [TestMethod]
        public void IndexBelongsToFile()
        {
            Indexer indexer = new Indexer("h264_720p_hp_3.1_600kbps_aac_mp3_dual_audio_harry_potter.mkv");
            indexer.Index().WriteIndex("h264_720p_hp_3.1_600kbps_aac_mp3_dual_audio_harry_potter.ffindex");

            Index indexFromFile = new Index("h264_720p_hp_3.1_600kbps_aac_mp3_dual_audio_harry_potter.ffindex");
            Assert.IsTrue(indexFromFile.BelongsToFile("h264_720p_hp_3.1_600kbps_aac_mp3_dual_audio_harry_potter.mkv"));
            Assert.IsFalse(indexFromFile.BelongsToFile("h264_720p_hp_5.1_3mbps_vorbis_styled_and_unstyled_subs_suzumiya.mkv"));
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentOutOfRangeException))]
        public void IndexGetTrackOutOfRange()
        {
            Index index = new Index("h264_720p_hp_5.1_3mbps_vorbis_styled_and_unstyled_subs_suzumiya.ffindex");
            index.GetTrack(6);
        }

        [TestMethod]
        public void VideoSourceAndAPIFunctions()
        {
            Index index = new Index("h264_720p_hp_5.1_3mbps_vorbis_styled_and_unstyled_subs_suzumiya.ffindex");

            VideoSource source = index.VideoSource("h264_720p_hp_5.1_3mbps_vorbis_styled_and_unstyled_subs_suzumiya.mkv", 0);

            Assert.AreEqual(30000, source.FPSNumerator);
            Assert.AreEqual(15712911, source.RFFNumerator);
            Assert.AreEqual(2157, source.NumberOfFrames);
            Assert.AreEqual(409440, source.SampleAspectRatioNumerator);
            Assert.AreEqual(0, source.Crop.Left);
            Assert.IsFalse(source.TopFieldFirst);
            Assert.AreEqual(0, source.FirstTime);
            Assert.AreEqual(71.939, source.LastTime);
        }

        [TestMethod]
        public void VideoSourceUTF8()
        {
            Index index = new Index("おはよう.ffindex");

            VideoSource source = index.VideoSource("おはよう.mkv", 0);
        }

        [TestMethod]
        [ExpectedException(typeof(FileLoadException))]
        public void VideoSourceFileLoadException()
        {
            Index index = new Index("h264_720p_hp_5.1_3mbps_vorbis_styled_and_unstyled_subs_suzumiya.ffindex");
            VideoSource source = index.VideoSource("this file doesn't exist.avi", 0);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public void VideoSourceArgumentException()
        {
            Index index = new Index("h264_720p_hp_5.1_3mbps_vorbis_styled_and_unstyled_subs_suzumiya.ffindex");
            VideoSource source = index.VideoSource("h264_720p_hp_5.1_3mbps_vorbis_styled_and_unstyled_subs_suzumiya.mkv", 10);
        }

        [TestMethod]
        [ExpectedException(typeof(InvalidOperationException))]
        public void VideoSourceInvalidOperationException()
        {
            Index index = new Index("h264_720p_hp_5.1_3mbps_vorbis_styled_and_unstyled_subs_suzumiya.ffindex");
            VideoSource source = index.VideoSource("h264_720p_hp_3.1_600kbps_aac_mp3_dual_audio_harry_potter.mkv", 0);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentOutOfRangeException))]
        public void VideoSourceGetFrameIntOutOfRange()
        {
            Index index = new Index("h264_720p_hp_5.1_3mbps_vorbis_styled_and_unstyled_subs_suzumiya.ffindex");
            VideoSource source = index.VideoSource("h264_720p_hp_5.1_3mbps_vorbis_styled_and_unstyled_subs_suzumiya.mkv", 0);

            source.GetFrame(3000);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentOutOfRangeException))]
        public void VideoSourceGetFrameDoubleOutOfRange()
        {
            Index index = new Index("h264_720p_hp_5.1_3mbps_vorbis_styled_and_unstyled_subs_suzumiya.ffindex");
            VideoSource source = index.VideoSource("h264_720p_hp_5.1_3mbps_vorbis_styled_and_unstyled_subs_suzumiya.mkv", 0);

            source.GetFrame((double)80);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentOutOfRangeException))]
        public void VideoSourceSetOutputFormatWidthOutOfRange()
        {
            Index index = new Index("h264_720p_hp_5.1_3mbps_vorbis_styled_and_unstyled_subs_suzumiya.ffindex");
            VideoSource source = index.VideoSource("h264_720p_hp_5.1_3mbps_vorbis_styled_and_unstyled_subs_suzumiya.mkv", 0);

            source.SetOutputFormat(new List<int>(), 0, 10, Resizer.Lanczos);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentOutOfRangeException))]
        public void VideoSourceSetOutputFormatHeightOutOfRange()
        {
            Index index = new Index("h264_720p_hp_5.1_3mbps_vorbis_styled_and_unstyled_subs_suzumiya.ffindex");
            VideoSource source = index.VideoSource("h264_720p_hp_5.1_3mbps_vorbis_styled_and_unstyled_subs_suzumiya.mkv", 0);

            source.SetOutputFormat(new List<int>(), 10, 0, Resizer.Lanczos);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public void VideoSourceSetOutputFormatInvalid()
        {
            Index index = new Index("h264_720p_hp_5.1_3mbps_vorbis_styled_and_unstyled_subs_suzumiya.ffindex");
            VideoSource source = index.VideoSource("h264_720p_hp_5.1_3mbps_vorbis_styled_and_unstyled_subs_suzumiya.mkv", 0);

            var list = new List<int>();
            list.Add(FFMS2.GetPixelFormat("none"));
            source.SetOutputFormat(list, 100, 100, Resizer.Bilinear);
        }

        [TestMethod]
        public void FrameAndAPIFunctions()
        {
            Index index = new Index("h264_720p_hp_5.1_3mbps_vorbis_styled_and_unstyled_subs_suzumiya.ffindex");
            VideoSource source = index.VideoSource("h264_720p_hp_5.1_3mbps_vorbis_styled_and_unstyled_subs_suzumiya.mkv", 0);

            Frame frame = source.GetFrame(20);
            List<int> targetFormats = new List<int>();
            targetFormats.Add(FFMS2.GetPixelFormat("bgra"));

            source.SetOutputFormat(targetFormats, frame.EncodedResolution.Width, frame.EncodedResolution.Height, Resizer.Lanczos);
            frame = source.GetFrame(20);

            Assert.AreEqual(704, frame.EncodedResolution.Width);
            Assert.AreEqual(0, frame.EncodedPixelFormat);
            Assert.AreEqual(704, frame.Resolution.Width);
            Assert.AreEqual(FFMS2.GetPixelFormat("bgra"), frame.PixelFormat);
            Assert.AreEqual(false, frame.KeyFrame);
            Assert.AreEqual(0, frame.RepeatPicture);
            Assert.AreEqual(false, frame.InterlacedFrame);
            Assert.AreEqual('P', frame.FrameType);
            Assert.AreEqual(ColorSpace.BT470BG, frame.ColorSpace);
            Assert.AreEqual(ColorRange.MPEG, frame.ColorRange);

            Bitmap bitmap = frame.Bitmap;
            bitmap.Save("frame.png", System.Drawing.Imaging.ImageFormat.Png);
            using (var md5 = MD5.Create())
            using (var stream = File.OpenRead("frame.png"))
            {
                Assert.AreEqual("36-7F-6A-D6-7A-12-12-EB-8E-4A-3F-DC-D0-F6-F6-13", BitConverter.ToString(md5.ComputeHash(stream)));
            }

            source.SetInputFormat(ColorSpace.RGB);
            frame = source.GetFrame(20);
            Assert.AreEqual(ColorSpace.RGB, frame.ColorSpace);

            source.ResetInputFormat();
            frame = source.GetFrame(20);
            Assert.AreEqual(ColorSpace.BT470BG, frame.ColorSpace);
        }

        [TestMethod]
        public void AudioSourceAndAPIFunctions()
        {
            Index index = new Index("h264_720p_hp_5.1_3mbps_vorbis_styled_and_unstyled_subs_suzumiya.ffindex");

            AudioSource source = index.AudioSource("h264_720p_hp_5.1_3mbps_vorbis_styled_and_unstyled_subs_suzumiya.mkv", 1);

            Assert.AreEqual(SampleFormat.float_t, source.SampleFormat);
            Assert.AreEqual(48000, source.SampleRate);
            Assert.AreEqual(32, source.BitsPerSample);
            Assert.AreEqual(2, source.Channels);
            Assert.AreEqual(3, source.ChannelLayout);
            Assert.AreEqual(3446272, source.NumberOfSamples);
            Assert.AreEqual(0, source.FirstTime);
            Assert.AreEqual(71.768, source.LastTime);

            byte[] buffer = source.GetAudio(0, 100000);

            File.WriteAllBytes("samples.dat", buffer);
            using (var md5 = MD5.Create())
            using (var stream = File.OpenRead("samples.dat"))
            {
                Assert.AreEqual("D0-55-BB-E0-35-DA-B4-1E-70-EA-D2-DE-DE-BF-13-DC", BitConverter.ToString(md5.ComputeHash(stream)));
            }
        }

        [TestMethod]
        public void AudioSourceUTF8()
        {
            Index index = new Index("おはよう.ffindex");

            AudioSource source = index.AudioSource("おはよう.mkv", 1);
        }

        [TestMethod]
        [ExpectedException(typeof(FileLoadException))]
        public void AudioSourceFileLoadException()
        {
            Index index = new Index("h264_720p_hp_5.1_3mbps_vorbis_styled_and_unstyled_subs_suzumiya.ffindex");
            AudioSource source = index.AudioSource("this file doesn't exist.avi", 1);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public void AudioSourceArgumentException()
        {
            Index index = new Index("h264_720p_hp_5.1_3mbps_vorbis_styled_and_unstyled_subs_suzumiya.ffindex");
            AudioSource source = index.AudioSource("h264_720p_hp_5.1_3mbps_vorbis_styled_and_unstyled_subs_suzumiya.mkv", 10);
        }

        [TestMethod]
        [ExpectedException(typeof(InvalidOperationException))]
        public void AudioSourceInvalidOperationException()
        {
            Index index = new Index("h264_720p_hp_5.1_3mbps_vorbis_styled_and_unstyled_subs_suzumiya.ffindex");
            AudioSource source = index.AudioSource("h264_720p_hp_3.1_600kbps_aac_mp3_dual_audio_harry_potter.mkv", 1);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentOutOfRangeException))]
        public void AudioSourceGetAudioStartOutOfRange()
        {
            Index index = new Index("h264_720p_hp_5.1_3mbps_vorbis_styled_and_unstyled_subs_suzumiya.ffindex");
            AudioSource source = index.AudioSource("h264_720p_hp_5.1_3mbps_vorbis_styled_and_unstyled_subs_suzumiya.mkv", 1);

            source.GetAudio(-1, 10);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentOutOfRangeException))]
        public void AudioSourceGetAudioSamplesOutOfRange()
        {
            Index index = new Index("h264_720p_hp_5.1_3mbps_vorbis_styled_and_unstyled_subs_suzumiya.ffindex");
            AudioSource source = index.AudioSource("h264_720p_hp_5.1_3mbps_vorbis_styled_and_unstyled_subs_suzumiya.mkv", 1);

            source.GetAudio(3446271, 10);
        }

        [TestMethod]
        public void VideoTrackAndAPIFunctions()
        {
            Index index = new Index("h264_720p_hp_5.1_3mbps_vorbis_styled_and_unstyled_subs_suzumiya.ffindex");
            VideoSource source = index.VideoSource("h264_720p_hp_5.1_3mbps_vorbis_styled_and_unstyled_subs_suzumiya.mkv", 0);

            Track track = source.Track;

            Assert.AreEqual(TrackType.Video, track.TrackType);
            Assert.AreEqual(1, track.TimeBaseNumerator);
            Assert.AreEqual(2157, track.NumberOfFrames);

            FrameInfo frameinfo = track.GetFrameInfo(20);

            Assert.AreEqual(667000000, frameinfo.PTS);
            Assert.AreEqual(1, frameinfo.RepeatPicture);
            Assert.AreEqual(false, frameinfo.KeyFrame);

            track.WriteTimecodes("timecodes.txt");
            using (var md5 = MD5.Create())
            using (var stream = File.OpenRead("timecodes.txt"))
            {
                Assert.AreEqual("99-ED-D0-4D-D5-09-42-48-60-E8-CD-DC-A5-F7-68-C3", BitConverter.ToString(md5.ComputeHash(stream)));
            }
        }

        [TestMethod]
        public void TrackWriteTimecodesUTF8()
        {
            Index index = new Index("h264_720p_hp_5.1_3mbps_vorbis_styled_and_unstyled_subs_suzumiya.ffindex");
            VideoSource source = index.VideoSource("h264_720p_hp_5.1_3mbps_vorbis_styled_and_unstyled_subs_suzumiya.mkv", 0);

            source.Track.WriteTimecodes("おはよう.txt");
            Assert.IsTrue(File.Exists("おはよう.txt"));
        }

        [TestMethod]
        [ExpectedException(typeof(IOException))]
        public void TrackWriteTimecodesIOException()
        {
            Index index = new Index("h264_720p_hp_5.1_3mbps_vorbis_styled_and_unstyled_subs_suzumiya.ffindex");
            VideoSource source = index.VideoSource("h264_720p_hp_5.1_3mbps_vorbis_styled_and_unstyled_subs_suzumiya.mkv", 0);

            source.Track.WriteTimecodes(Path.Combine("this directory doesn't exist", "timecodes.txt"));
        }

        [TestMethod]
        public void AudioTrackAndAPIFunctions()
        {
            Index index = new Index("h264_720p_hp_5.1_3mbps_vorbis_styled_and_unstyled_subs_suzumiya.ffindex");
            AudioSource source = index.AudioSource("h264_720p_hp_5.1_3mbps_vorbis_styled_and_unstyled_subs_suzumiya.mkv", 1);

            Track track = source.Track;

            Assert.AreEqual(TrackType.Audio, track.TrackType);
            Assert.AreEqual(1, track.TimeBaseNumerator);
            Assert.AreEqual(4490, track.NumberOfFrames);

            try
            {
                track.GetFrameInfo(20); // It won't pass this.
                Assert.Fail();
            }
            catch
            { }
        }

        [TestMethod]
        public void GenericTrackAndAPIFunctions()
        {
            Index index = new Index("h264_720p_hp_5.1_3mbps_vorbis_styled_and_unstyled_subs_suzumiya.ffindex");

            Track track = index.GetTrack(2);

            Assert.AreEqual(TrackType.Subtitle, track.TrackType);
            Assert.AreEqual(1, track.TimeBaseNumerator);
            Assert.AreEqual(0, track.NumberOfFrames);
        }
    }
}