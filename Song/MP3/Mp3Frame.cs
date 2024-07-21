namespace MusicPlayer;

public class Mp3Frame{
    public Boolean syncwordFound{get; set;}
    public string MpegVersion{get; set;}
    public string layer{get; set;}
    public Boolean prot{get; set;}
    public int bitRate{get; set;}
    public int samplingRateFrequency{get ; set;}
    public Boolean padded{get; set;}
    public Boolean privateUsed{get; set;}
    public string channelMode{get; set;}
    public string modeExtension{get; set;}
    public Boolean copyright{get; set;}
    public Boolean original{get; set;}
    public string emphasis{get; set;}
    public int frameSize {get; set;}

    public Mp3Frame(){
        this.MpegVersion = "";
        this.channelMode = "";
        this.layer = "";
        this.modeExtension = "";
        this.emphasis = "";
    }

    public override string ToString()
    {
        return $"MPEG Version: {MpegVersion}, Layer Version: {layer}, Protection: {prot}, Bit Rate: {bitRate}, Frequency: {samplingRateFrequency}, Padded: {padded}, Private used: {privateUsed}, Channel Mode: {channelMode}, Mode Extension: {modeExtension}, Copyright: {copyright}, Original: {original}, Emphasis: {emphasis}, Frame Size: {frameSize}";
    }

    public void calculateSize(){

        int padding = 0;
        
        if (padded){
            if (layer == "Layer 1" ){
                padding = 4;
            }
            else {
                padding = 1;
            }
        }
    

        if (layer == "Layer 1"){
            frameSize = (((12 * bitRate * 1000) / samplingRateFrequency) + (padding * 4));
        }
        else if (layer == "Layer 2"){
            frameSize = ((144 * bitRate * 1000) / samplingRateFrequency) + padding;
        }
        else {
            if (MpegVersion == "MPEG Version 1"){
                frameSize = (((144 * bitRate * 1000) / samplingRateFrequency) + padding);
            }
            else {
                frameSize = (((72 * bitRate * 1000) / samplingRateFrequency) + padding);
            }
        }
    }
}