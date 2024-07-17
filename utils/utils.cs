namespace MusicPlayer;

abstract class Utils
{
    public static Dictionary<int, string> MPEGVersion = new Dictionary<int, string>{
        {0xE0, "MPEG Version 2.5"},
        {0xE8, "Reserved Error"},
        {0xF0, "MPEG Version 2"},
        {0xF8, "MPEG Version 1"}
    };
}