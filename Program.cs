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

        // create list to hold frames
        List<Mp3Frame> mp3Frames = new List<Mp3Frame>();

        int index = 0;

        // find the ID3 tag if present
        // check for the ID3v2 version that starts at the beginning
        // ONLY checking right now. Later implement how to handle
        if(by[0] == 'I' && by[1] == 'D' && by[2] == '3'){
            Console.WriteLine((char) by[0]);
            Console.WriteLine((char) by[1]);
            Console.WriteLine((char) by[2]);
            Console.WriteLine("Tag found");
        }
        else {
            Console.WriteLine(by[0]);
            Console.WriteLine(by[1]);
            Console.WriteLine(by[2]);
            Console.WriteLine("Tag not found");
        }

        //check for the ID3v1 that is at the end of it
        int positionOfTag = by.Length - 128 - 1;
        if(by[positionOfTag] == 'T' && by[positionOfTag + 1] == 'A' && by[positionOfTag + 2] == 'G'){
            Console.WriteLine((char) by[positionOfTag]);
            Console.WriteLine((char) by[positionOfTag + 1]);
            Console.WriteLine((char) by[positionOfTag + 2]);
            Console.WriteLine("Tag found");
        }
        else{
            Console.WriteLine((char) by[positionOfTag]);
            Console.WriteLine((char) by[positionOfTag + 1]);
            Console.WriteLine((char) by[positionOfTag + 2]);
            Console.WriteLine("Tag not found");
        }


        while(index < by.Length){
            // search for sync word and make use of bit operations need 11111111 111????? bit
            if (by[index] == 0xFF && (by[index + 1] & 0xE0) == 0xE0){
                Mp3Frame currentFrame = new Mp3Frame();
                currentFrame.syncwordFound = true;
                index += 1;
                if ((by[index] & 0xF8) == 0xE0){
                    currentFrame.MpegVersion = Utils.MPEGVersion[0xE0];
                }
                else if ((by[index] & 0xF8) == 0xE8){
                    currentFrame.MpegVersion = Utils.MPEGVersion[0xE8];
                }
                else if ((by[index] & 0xF8) == 0xF0){
                    currentFrame.MpegVersion = Utils.MPEGVersion[0xF0];
                }
                else if ((by[index] & 0xF8) == 0xF8){
                    currentFrame.MpegVersion = Utils.MPEGVersion[0xF8];
                }
                else {
                    Console.WriteLine("Error detected.");
                }

                // check for the layer as the next two bits are layer
                // shift the bits to the right and compare to ??????11
                int shifted = (by[index] >> 1) & 0x03;
                switch (shifted){
                    case 0b00:
                        currentFrame.layer = "Reserved";
                        break;
                    case 0b01:
                        currentFrame.layer = "Layer 3";
                        break;
                    case 0b10:
                        currentFrame.layer = "Layer 2";
                        break;
                    case 0b11:
                        currentFrame.layer = "Layer 1";
                        break;
                }

                // check for the protection bit
                if ((by[index] & 0x01) == 0x01){
                    currentFrame.prot = true;
                }
                else{
                    currentFrame.prot = false;
                }

                // move index along as we are now entering the third byte of the header
                index += 1;

                // check for the bit rate
                int[] bitRate;

                string versionLayerCombination = $"{currentFrame.MpegVersion} {currentFrame.layer}";

                switch(versionLayerCombination){
                    case "MPEG Version 1 Layer 1":
                        bitRate = Utils.version1Layer1;
                        break;
                    case "MPEG Version 1 Layer 2":
                        bitRate = Utils.version1Layer2;
                        break;
                    case "MPEG Version 1 Layer 3":
                        bitRate = Utils.version1Layer3;
                        break;
                    case "MPEG Version 2 Layer 1":
                    case "MPEG Version 2.5 Layer 1":
                        bitRate = Utils.version2Layer1;
                        break;
                    case "MPEG Version 2 Layer 2":
                    case "MPEG Version 2.5 Layer 2":
                    case "MPEG Version 2 Layer 3":
                    case "MPEG Version 2.5 Layer 3":
                        bitRate = Utils.version2RemainingLayers;
                        break;
                    default:
                        bitRate = [-1];
                        break;    
                }

                if (bitRate[0] == -1){
                    break;
                }
                
                shifted = (by[index] >> 4) & 0xF;
                Console.WriteLine(shifted);
                currentFrame.bitRate = bitRate[shifted];

                // find the sampling rate 
                shifted = (by[index] >> 2) & 0x3;

                switch (currentFrame.MpegVersion){
                    case "MPEG Version 1":
                        currentFrame.samplingRateFrequency = Utils.version1[shifted];
                        break;
                    case "MPEG Version 2":
                        currentFrame.samplingRateFrequency = Utils.version2[shifted];
                        break;
                    case "MPEG Version 2.5":
                        currentFrame.samplingRateFrequency = Utils.version2dot5[shifted];
                        break;
                    default:
                        break;
                }

                mp3Frames.Add(currentFrame);
            }
            else 
            {
                index += 1;
            }
        }

        Dictionary<string, int> differentMPEGVersion = new Dictionary<string, int>(){
            {"MPEG Version 2.5", 0},
            {"Reserved Error", 0},
            {"MPEG Version 2", 0},
            {"MPEG Version 1", 0}
        };

        Console.WriteLine(mp3Frames[0].MpegVersion);

        for (int idx = 0; idx < mp3Frames.Count; idx++){
            differentMPEGVersion[mp3Frames[idx].MpegVersion] += 1;
        }
        var keys = differentMPEGVersion.Keys.ToArray();
        var vals = differentMPEGVersion.Values.ToArray();

        for (int idx = 0; idx < 4; idx++){
            Console.WriteLine($"Version: {keys[idx]} has {vals[idx]} occurences");
        }

        Console.WriteLine(mp3Frames[0].ToString());
        Console.WriteLine(by[2]);
    }
}