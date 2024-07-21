using System.Collections;
using System.Collections.ObjectModel;
using System.Security.Cryptography;

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
            Console.WriteLine("Start of the file Tag found");
        }
        else {

            Console.WriteLine("Start of the file tag not found");
        }

        //check for the ID3v1 that is at the end of it
        int positionOfTag = by.Length - 128 - 1;
        if(by[positionOfTag] == 'T' && by[positionOfTag + 1] == 'A' && by[positionOfTag + 2] == 'G'){
            Console.WriteLine("End of the file tag found");
        }
        else{
            Console.WriteLine("End of the file tag not found");
        }

        Dictionary<int, int> frameSizes = new Dictionary<int, int>();


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

                
                shifted = (by[index] >> 4) & 0xF;
                /**
                Console.WriteLine(index);
                int count = mp3Frames.Count - 1;
                if (mp3Frames.Count > 0){
                    Console.WriteLine(mp3Frames.Count + ": " + mp3Frames[count].ToString());
                };
                **/

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

                // find the padding
                shifted = (by[index] >> 1) & 0x1;

                if (shifted == 1){
                    currentFrame.padded = true;
                }
                else {
                    currentFrame.padded = false;
                }

                // find whether the private bit is used
                shifted = (by[index]) & 0x1;
                if (shifted == 1){
                    currentFrame.privateUsed = true;
                }
                else {
                    currentFrame.privateUsed = false;
                }

                index += 1;

                // find the channel mode

                shifted = (by[index] >> 6) & 3;

                if (shifted == 0){
                    currentFrame.channelMode = "Stereo";
                }
                else if (shifted == 1){
                    currentFrame.channelMode = "Joint Stereo";
                }
                else if (shifted == 2){
                    currentFrame.channelMode = "Dual Channel";
                }
                else {
                    currentFrame.channelMode = "Single Channel";
                }

                // find the Mode Extension. Only relevant if Joint Stereo
                shifted = (by[index] >> 4) & 3;
                if (currentFrame.channelMode == "Joint Stereo"){
                    if (currentFrame.layer == "Layer 1" || currentFrame.layer == "Layer 2"){
                        // indicates which bands are used from x - 31
                        if (shifted == 0){
                            currentFrame.modeExtension = "4";
                        }
                        else if (shifted == 1){
                            currentFrame.modeExtension = "8";
                        }
                        else if (shifted == 2){
                            currentFrame.modeExtension = "12";
                        }
                        else {
                            currentFrame.modeExtension = "16";
                        }
                    }
                    else {
                        // layer 3 has different settings than other layers
                        if (shifted == 0){
                            currentFrame.modeExtension = "None";
                        }
                        else if (shifted == 1){
                            currentFrame.modeExtension = "Intensity Stereo";
                        }
                        else if (shifted == 2){
                            currentFrame.modeExtension = "MS Stereo";
                        }
                        else {
                            currentFrame.modeExtension = "Both";
                        }
                    }
                }
                else {
                    currentFrame.modeExtension = "Not in use";
                }

                // find copyright bit
                shifted = (by[index] >> 3) & 1;
                if (shifted == 1){
                    currentFrame.copyright = true;
                }
                else {
                    currentFrame.copyright = false;
                }

                // find the original bit
                shifted = (by[index] >> 2) & 1;
                if (shifted == 1){
                    currentFrame.original = true;
                }
                else {
                    currentFrame.original = false;
                }

                // determine the emphasis
                shifted = by[index] & 3;
                if (shifted == 0){
                    currentFrame.emphasis = "None";
                }
                else if (shifted == 1){
                    currentFrame.emphasis = "50/15 ms";
                }
                else if (shifted == 2){
                    currentFrame.emphasis = "reserved";
                }
                else {
                    currentFrame.emphasis = "CCIT J.17";
                }

                // reset if bad header
                if (currentFrame.bitRate == -1 || currentFrame.samplingRateFrequency == -1){
                    index -= 2;
                    continue;
                }

                currentFrame.calculateSize();

                mp3Frames.Add(currentFrame);

                index += currentFrame.frameSize - 4;
                if (frameSizes.ContainsKey(currentFrame.frameSize)){
                    frameSizes[currentFrame.frameSize] += 1;
                }
                else {
                    frameSizes[currentFrame.frameSize] = 1;
                }
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

        int[] key = frameSizes.Keys.ToArray();
        vals = frameSizes.Values.ToArray();

        for (int idx = 0; idx < key.Length; idx++){
            Console.WriteLine($"Framesize: {key[idx]} has {vals[idx]} occurences");
        }

    }
}