using Newtonsoft.Json;

namespace wootosu.Shared.Models;

public class RecordingInfo
{
  public RecordingInfo() { }

  public RecordingInfo(long recordStartTime, long recordEndTime, bool isTrimmed)
  {
    StartTime = recordStartTime;
    EndTime = recordEndTime;
    IsTrimmed = isTrimmed;
  }

  [JsonProperty("start_time")]
  public long StartTime { get; private set; }

  [JsonProperty("end_time")]
  public long EndTime { get; private set; }

  [JsonProperty("is_trimmed")]
  public bool IsTrimmed { get; private set; }
}
