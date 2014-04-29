using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using FFMSsharp;
using System.Drawing;
using System.Collections.Generic;
using System.IO;

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
            Console.WriteLine(FFMS2.GetVersionString());
        }

        [TestMethod]
        public void GetSetLogLevel()
        {
            FFMS2.SetLogLevel(AvLogLevel.Debug);
            Assert.AreEqual(AvLogLevel.Debug, FFMS2.GetLogLevel());
            FFMS2.SetLogLevel(AvLogLevel.Quiet);
            Assert.AreEqual(AvLogLevel.Quiet, FFMS2.GetLogLevel());
        }

        [TestMethod]
        public void GetPixFmt()
        {
            Assert.AreNotEqual(-1, FFMS2.GetPixFmt("yuv420p"));
            Assert.AreEqual(-1, FFMS2.GetPixFmt("none"));
        }

        [TestMethod]
        public void IndexerMatroska()
        {
            Assert.IsTrue(FFMS2.IsSourceEnabled(Sources.Matroska));
            Indexer indexer = new Indexer("h264_720p_hp_5.1_3mbps_vorbis_styled_and_unstyled_subs_suzumiya.mkv");

            Assert.AreEqual(5, indexer.GetNumTracks());
            Assert.AreEqual(FFMSsharp.TrackType.Video, indexer.GetTrackType(0));
            Assert.AreEqual("matroska", indexer.GetFormatName());
            Assert.AreEqual("h264", indexer.GetCodecName(0));
        }

        [TestMethod]
        [ExpectedException(typeof(FFMSFileReadException))] // Yes. This surprised me too. It's not a NO_FILE error.
        public void IndexerFileNotFound()
        {
            Indexer indexer = new Indexer("this file doesn't exist.avi");
        }

        [TestMethod]
        public void IndexAndAPIFunctions()
        {
            Assert.IsTrue(FFMS2.IsSourceEnabled(Sources.Matroska));
            Indexer indexer = new Indexer("h264_720p_hp_5.1_3mbps_vorbis_styled_and_unstyled_subs_suzumiya.mkv");

            Index index = indexer.Index();

            Assert.AreEqual(Sources.Matroska, index.GetSourceType());
            Assert.AreEqual(IndexErrorHandling.Abort, index.GetErrorHandling());
            Assert.AreEqual(1, index.GetFirstTrackOfType(TrackType.Audio));
            Assert.AreEqual(1, index.GetFirstIndexedTrackOfType(TrackType.Audio));

            index.WriteIndex("h264_720p_hp_5.1_3mbps_vorbis_styled_and_unstyled_subs_suzumiya.ffindex");
        }

        [TestMethod]
        public void IndexAudioIndex()
        {
            Assert.IsTrue(FFMS2.IsSourceEnabled(Sources.Matroska));
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
            Assert.IsTrue(FFMS2.IsSourceEnabled(Sources.Matroska));
            Index index = new Index("h264_720p_hp_5.1_3mbps_vorbis_styled_and_unstyled_subs_suzumiya.ffindex");
            
            Assert.IsTrue(index.BelongsToFile("h264_720p_hp_5.1_3mbps_vorbis_styled_and_unstyled_subs_suzumiya.mkv"));
        }
        
        [TestMethod]
        [ExpectedException(typeof(FFMSFileMismatchException))]
        public void IndexBelongsToFile()
        {
            Indexer indexer = new Indexer("h264_720p_hp_3.1_600kbps_aac_mp3_dual_audio_harry_potter.mkv");
            Index index = indexer.Index();
            index.WriteIndex("h264_720p_hp_3.1_600kbps_aac_mp3_dual_audio_harry_potter.ffindex");

            Index indexFromFile = new Index("h264_720p_hp_3.1_600kbps_aac_mp3_dual_audio_harry_potter.ffindex");
            indexFromFile.BelongsToFile("h264_720p_hp_5.1_3mbps_vorbis_styled_and_unstyled_subs_suzumiya.mkv", true);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentOutOfRangeException))]
        public void IndexGetTrackTooHigh()
        {
            Index index = new Index("h264_720p_hp_5.1_3mbps_vorbis_styled_and_unstyled_subs_suzumiya.ffindex");
            index.GetTrack(6);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentOutOfRangeException))]
        public void IndexGetTrackNegative()
        {
            Index index = new Index("h264_720p_hp_5.1_3mbps_vorbis_styled_and_unstyled_subs_suzumiya.ffindex");
            index.GetTrack(-1);
        }

        [TestMethod]
        public void VideoSourceAndAPIFunctions()
        {
            Assert.IsTrue(FFMS2.IsSourceEnabled(Sources.Matroska));
            Index index = new Index("h264_720p_hp_5.1_3mbps_vorbis_styled_and_unstyled_subs_suzumiya.ffindex");

            VideoSource source = index.VideoSource("h264_720p_hp_5.1_3mbps_vorbis_styled_and_unstyled_subs_suzumiya.mkv", 0);

            Assert.AreEqual(30000, source.FPSNumerator);
            Assert.AreEqual(15712911, source.RFFNumerator);
            Assert.AreEqual(2157, source.NumFrames);
            Assert.AreEqual(409440, source.SampleAspectRatioNumerator);
            Assert.AreEqual(0, source.Crop.Left);
            Assert.IsFalse(source.TopFieldFirst);
            Assert.AreEqual(0, source.FirstTime);
            Assert.AreEqual(71.939, source.LastTime);
        }

        [TestMethod]
        public void FrameAndAPIFunctions()
        {
            Assert.IsTrue(FFMS2.IsSourceEnabled(Sources.Matroska));
            Index index = new Index("h264_720p_hp_5.1_3mbps_vorbis_styled_and_unstyled_subs_suzumiya.ffindex");
            VideoSource source = index.VideoSource("h264_720p_hp_5.1_3mbps_vorbis_styled_and_unstyled_subs_suzumiya.mkv", 0);

            Frame frame = source.GetFrame(20);
            List<int> targetFormats = new List<int>();
            targetFormats.Add(FFMS2.GetPixFmt("bgra"));

            source.SetOutputFormat(targetFormats, frame.EncodedResolution.Width, frame.EncodedResolution.Height, Resizers.Lanczos);
            frame = source.GetFrame(20);

            Assert.AreEqual(704, frame.EncodedResolution.Width);
            Assert.AreEqual(0, frame.EncodedPixelFormat);
            Assert.AreEqual(704, frame.Resolution.Width);
            Assert.AreEqual(FFMS2.GetPixFmt("bgra"), frame.PixelFormat);
            Assert.AreEqual(false, frame.KeyFrame);
            Assert.AreEqual(0, frame.RepeatPict);
            Assert.AreEqual(false, frame.InterlacedFrame);
            Assert.AreEqual('P', frame.FrameType);
            Assert.AreEqual(ColorSpaces.BT470BG, frame.ColorSpace);
            Assert.AreEqual(ColorRanges.MPEG, frame.ColorRange);

            Bitmap bitmap = frame.GetBitmap();
            bitmap.Save("frame.png", System.Drawing.Imaging.ImageFormat.Png);
            Console.WriteLine("Please confirm that {0}\\frame.png looks good.", Environment.CurrentDirectory);

            source.SetInputFormat(ColorSpaces.RGB);
            frame = source.GetFrame(20);
            Assert.AreEqual(ColorSpaces.RGB, frame.ColorSpace);

            source.ResetInputFormat();
            frame = source.GetFrame(20);
            Assert.AreEqual(ColorSpaces.BT470BG, frame.ColorSpace);
        }

        [TestMethod]
        public void AudioSourceAndAPIFunctions()
        {
            Assert.IsTrue(FFMS2.IsSourceEnabled(Sources.Matroska));
            Index index = new Index("h264_720p_hp_5.1_3mbps_vorbis_styled_and_unstyled_subs_suzumiya.ffindex");

            AudioSource source = index.AudioSource("h264_720p_hp_5.1_3mbps_vorbis_styled_and_unstyled_subs_suzumiya.mkv", 1);

            Assert.AreEqual(SampleFormat.float_t, source.SampleFormat);
            Assert.AreEqual(48000, source.SampleRate);
            Assert.AreEqual(32, source.BitsPerSample);
            Assert.AreEqual(2, source.Channels);
            Assert.AreEqual(3, source.ChannelLayout);
            Assert.AreEqual(3446272, source.NumSamples);
            Assert.AreEqual(0, source.FirstTime);
            Assert.AreEqual(71.768, source.LastTime);

            byte[] buffer = source.GetAudio(0, 100000);

            File.WriteAllBytes("samples.dat", buffer);
            Console.WriteLine("Please confirm that {0}\\samples.dat sounds good.", Environment.CurrentDirectory);
        }

        [TestMethod]
        public void VideoTrackAndAPIFunctions()
        {
            Assert.IsTrue(FFMS2.IsSourceEnabled(Sources.Matroska));
            Index index = new Index("h264_720p_hp_5.1_3mbps_vorbis_styled_and_unstyled_subs_suzumiya.ffindex");
            VideoSource source = index.VideoSource("h264_720p_hp_5.1_3mbps_vorbis_styled_and_unstyled_subs_suzumiya.mkv", 0);

            Track track = source.GetTrack();

            Assert.AreEqual(TrackType.Video, track.Type);
            Assert.AreEqual(1, track.TimeBaseNumerator);
            Assert.AreEqual(2157, track.GetNumFrames());

            FrameInfo frameinfo = track.GetFrameInfo(20);

            Assert.AreEqual(667000000, frameinfo.PTS);
            Assert.AreEqual(1, frameinfo.RepeatPict);
            Assert.AreEqual(false, frameinfo.KeyFrame);

            track.WriteTimecodes("timecodes.txt");
            Console.WriteLine("Please confirm that {0}\\timecodes.txt looks correct.", Environment.CurrentDirectory);
        }

        [TestMethod]
        public void AudioTrackAndAPIFunctions()
        {
            Assert.IsTrue(FFMS2.IsSourceEnabled(Sources.Matroska));
            Index index = new Index("h264_720p_hp_5.1_3mbps_vorbis_styled_and_unstyled_subs_suzumiya.ffindex");
            AudioSource source = index.AudioSource("h264_720p_hp_5.1_3mbps_vorbis_styled_and_unstyled_subs_suzumiya.mkv", 1);

            Track track = source.GetTrack();

            Assert.AreEqual(TrackType.Audio, track.Type);
            Assert.AreEqual(1, track.TimeBaseNumerator);
            Assert.AreEqual(4490, track.GetNumFrames());

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
            Assert.IsTrue(FFMS2.IsSourceEnabled(Sources.Matroska));
            Index index = new Index("h264_720p_hp_5.1_3mbps_vorbis_styled_and_unstyled_subs_suzumiya.ffindex");

            Track track = index.GetTrack(2);

            Assert.AreEqual(TrackType.Subtitle, track.Type);
            Assert.AreEqual(1, track.TimeBaseNumerator);
            Assert.AreEqual(0, track.GetNumFrames());
        }
    }
}