using Newtonsoft.Json;

namespace wootosu.Shared.Models;

public class WootingAnalogSdkInfo
{
#pragma warning disable CS8618
  public WootingAnalogSdkInfo() { }

  public WootingAnalogSdkInfo(byte[] certificate, long fileSize, string fileHash)
  {
    Certificate = certificate;
    FileSize = fileSize;
    FileHash = fileHash;
  }

  [JsonProperty("certificate")]
  public byte[] Certificate { get; private set; }

  [JsonProperty("file_size")]
  public long FileSize { get; private set; }

  [JsonProperty("file_hash")]
   public string FileHash { get; private set; }
}
