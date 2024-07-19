namespace MusicPlayer;

abstract class Utils
{
    public static Dictionary<int, string> MPEGVersion = new Dictionary<int, string>{
        {0xE0, "MPEG Version 2.5"},
        {0xE8, "Reserved Error"},
        {0xF0, "MPEG Version 2"},
        {0xF8, "MPEG Version 1"}
    };

    public static int[] version1Layer1 = {
        1,
        32,
        64,
        96,
        128,
        160,
        192,
        224,
        256,
        288,
        320,
        352,
        384,
        416,
        448,
        -1
    };

    public static int[] version1Layer2 = {
        1,
        32,
        48,
        56,
        64,
        80,
        96,
        112,
        128,
        160,
        192,
        224,
        256,
        320,
        384,
        -1
    };

    public static int[] version1Layer3 = {
        1,
        32,
        40,
        48,
        56,
        64,
        80,
        96,
        112,
        128,
        160,
        192,
        224,
        256,
        320,
        -1
    };

    public static int[] version2Layer1 = {
        1,
        32,
        48,
        56,
        64,
        80,
        96,
        112,
        128,
        144,
        160,
        176,
        192,
        224,
        256,
        -1        
    };

    public static int[] version2RemainingLayers = {
        1,
        8,
        16,
        24,
        32,
        40,
        48,
        56,
        64,
        80,
        96,
        112,
        128,
        144,
        160,
        -1
    };

    public static int[] version1 = {
        44100,
        48000,
        32000,
        -1
    };

    public static int[] version2 = {
        22050,
        24000,
        16000,
        -1,
    };

    public static int[] version2dot5 = {
        11025,
        12000,
        8000,
        -1
    };
}