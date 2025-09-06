using Newtonsoft.Json;

namespace wootosu.Shared.Models;

public class Recording
{
#pragma warning disable CS8618
  public Recording() { }

  public Recording(RecordingInfo recordingInfo, RecordingDeviceInfo deviceInfo, RecordFrame[] frames, RecordError[] errors, WootingAnalogSdkInfo? wootingAnalogSDKInfo)
  {
    RecordingInfo = recordingInfo;
    DeviceInfo = deviceInfo;
    Frames = frames;
    Errors = errors;
    WootingAnalogSdkInfo = wootingAnalogSDKInfo;
  }

  [JsonProperty("recording_info")]
  public RecordingInfo RecordingInfo { get; private set; }
  
  [JsonProperty("device_info")]
  public RecordingDeviceInfo DeviceInfo { get; private set; }

  [JsonProperty("frames")]
  public RecordFrame[] Frames { get; private set; }

  [JsonProperty("errors")]
  public RecordError[] Errors { get; private set; }

  [JsonProperty("wooting_analog_sdk_info")]
  public WootingAnalogSdkInfo? WootingAnalogSdkInfo { get; private set; }
}
