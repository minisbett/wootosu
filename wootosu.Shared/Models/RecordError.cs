using Newtonsoft.Json;
using WootingAnalogSDKNET;

namespace wootosu.Shared.Models;

public class RecordError
{
  public RecordError() { }

  public RecordError(long scanIndex, int timestamp, WootingAnalogResult errorCode)
  {
    ScanIndex = scanIndex;
    Timestamp = timestamp;
    ErrorCode = errorCode;
  }

  [JsonProperty("scan_index")]
  public long ScanIndex { get; private set; }

  [JsonProperty("timestamp")]
  public int Timestamp { get; private set; }

  [JsonProperty("error_code")]
  public WootingAnalogResult ErrorCode { get; private set; }
}
