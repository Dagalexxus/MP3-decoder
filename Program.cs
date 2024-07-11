namespace MusicPlayer;

class Program
{
    static void Main(string[] args)
    {
        ISong currentSong = new Mp3Song("assets/tvari-tokyo-cafe-159065.mp3", "tokyo cafe");
        Console.WriteLine("Song Opened.");
        Console.WriteLine(currentSong.ToString());
    }
}
