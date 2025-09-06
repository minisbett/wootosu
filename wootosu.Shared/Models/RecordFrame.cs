using Newtonsoft.Json;

namespace wootosu.Shared.Models;

public class RecordFrame
{
  public RecordFrame() { }

  public RecordFrame(long scanIndex, int timestamp, VirtualKey keyCode, float analogValue)
  {
    ScanIndex = scanIndex;
    Timestamp = timestamp;
    KeyCode = keyCode;
    AnalogValue = analogValue;
  }

  [JsonProperty("scan_index")]
  public long ScanIndex { get; private set; }

  [JsonProperty("timestamp")]
  public int Timestamp { get; private set; }

  [JsonProperty("key_code")]
  public VirtualKey KeyCode { get; private set; }

  [JsonProperty("analog_value")]
  public float AnalogValue { get; private set; }
}
