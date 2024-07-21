using System.Collections;
using System.Collections.ObjectModel;

namespace MusicPlayer;

class Program
{
    static void Main(string[] args)
    {
        ISong currentSong = new Mp3Song("assets/tvari-tokyo-cafe-159065.mp3", "tokyo cafe");
        Console.WriteLine("Song Opened.");
        Console.WriteLine(currentSong.ToString());
        byte[] by = currentSong.getSongInfo();
        Console.WriteLine(by.Length);
        List<Mp3Frame> mp3Frames = currentSong.decodeSong();

        Dictionary<string, int> differentMPEGVersion = new Dictionary<string, int>(){
            {"MPEG Version 2.5", 0},
            {"Reserved Error", 0},
            {"MPEG Version 2", 0},
            {"MPEG Version 1", 0}
        };

        int counter = 0;
        for (int idx = 0; idx < mp3Frames.Count; idx++){
            differentMPEGVersion[mp3Frames[idx].MpegVersion] += 1;
            if (mp3Frames[idx].samplingRateFrequency == 44100){
                counter++;
            }
        }
        var keys = differentMPEGVersion.Keys.ToArray();
        var vals = differentMPEGVersion.Values.ToArray();

        for (int idx = 0; idx < 4; idx++){
            Console.WriteLine($"Version: {keys[idx]} has {vals[idx]} occurences");
        }
        Console.WriteLine($"The sample frequency is 44,100 {counter} times");
    }
}