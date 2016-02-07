FFMS# 
=====

Notice
------
**This project has moved to [GitGud](http://gitgud.io/nixx/FFMSSharp). See you there!**

C# wrapper for [FFMS2](//github.com/ffms/ffms2)

### How to use
1. Download the [latest FFMSSharp.dll](//github.com/nixxquality/FFMSSharp/releases)
2. Reference it in your C# application
3. Download [ffms2.dll](//github.com/ffms/ffms2/releases)
4. Put it next to your .exe file (or in a folder near your .exe file, see ```FFMS2.Initialize(string)```)
5. Use FFMS2!

### Things to keep in mind
- Right now the code is in a very immature state, do not use it in production or bad things might happen.

### Example
```C#
// Initialize the library. You have to call this before anything else.
FFMSSharp.FFMS2.Initialize();

// Index the source file.
string sourcefile = "somefilename";
var indexer = new FFMSSharp.Indexer(sourcefile);
var index = indexer.Index();

// Retrieve the track number of the first video track
int trackno = index.GetFirstTrackOfType(FFMSSharp.TrackType.Video);

// We now have enough information to create the video source object
var videosource = index.VideoSource(sourcefile, trackno);

// Check how many frames the video has
int num_frames = videosource.NumberOfFrames;

// Get the first frame for examination so we know what we're getting.
// This is required because resolution and colorspace is a per frame property and NOT global for the video.
var propframe = videosource.GetFrame(0);

/* Now you may want to do something with the info; particularly interesting values are:
 * propframe.EncodedResolution.Width; (frame width in pixels)
 * propframe.EncodedResolution.Height; (frame height in pixels)
 * propframe.EncodedPixelFormat; (actual frame colorspace)
 */

/* If you want to change the output colorspace or resize the output frame size, now is the time to do it.
 * IMPORTANT: This step is also required to prevent resolution and colorspace changes midstream.
 * You can you can always tell a frame's original properties by examining the Encoded* properties in FFMSSharp.Frame.
 * See libavutil/pixfmt.h for the list of pixel formats/colorspaces.
 * To get the name of a given pixel format, strip the leading PIX_FMT_ and convert to lowercase.
 * For example, PIX_FMT_YUV420P becomes "yuv420p".
 */

// A list of the acceptable output formats
List<int> pixfmts = new List<int>();
pixfmts.Add(FFMSSharp.FFMS2.GetPixelFormat("bgra"));

videosource.SetOutputFormat(pixfmts, propframe.EncodedResolution.Width, propframe.EncodedResolution.Height, FFMSSharp.Resizer.Bicubic);

// Now we're ready to actually retrieve the video frames.
int framenumber = 0;
var curframe = videosource.GetFrame(framenumber); // Valid until next call to GetFrame on the same video object

// Do something with curframe
// Continue doing this until you're bored, or something

// Everything _should_ get cleaned up automatically.
```
