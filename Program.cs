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

        // create list to hold frames
        List<Mp3Frame> mp3Frames = new List<Mp3Frame>();

        int index = 0;

        while(index < by.Length){
            // search for sync word and make use of bit operations need 11111111 111????? bit
            if (by[index] == 0xFF && (by[index + 1] & 0xE0) == 0xE0){
                Mp3Frame currentFrame = new Mp3Frame();
                
            }
        }

    }
}
