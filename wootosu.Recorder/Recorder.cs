using System.Diagnostics;
using WootingAnalogSDKNET;
using wootosu.Shared;
using wootosu.Shared.Models;

namespace wootosu.Recorder;

internal class Recorder(DeviceInfo deviceInfo)
{
  private readonly Stopwatch _watch = new();
  private readonly List<RecordFrame> _frames = [];
  private readonly List<RecordError> _errors = [];
  private Dictionary<short, float> _lastBuffer = [];
  private long _scanIndex = 0;

  public IReadOnlyList<RecordFrame> Frames => _frames.AsReadOnly();

  public IReadOnlyList<RecordError> Errors => _errors.AsReadOnly();

  public TimeSpan Elapsed => _watch.Elapsed;

  public void Start() => _watch.Start();

  public void Stop() => _watch.Stop();

  public void Process()
  {
    if (!_watch.IsRunning)
      throw new InvalidOperationException("The recorder is not running.");

    (List<(short, float)> rawBuffer, WootingAnalogResult error) = WootingAnalogSDK.ReadFullBuffer(deviceID: deviceInfo.device_id);
    if (error != WootingAnalogResult.Ok)
    {
      _errors.Add(new(_scanIndex, (int)_watch.ElapsedMilliseconds, error));
      return;
    }

    Dictionary<short, float> buffer = rawBuffer.ToDictionary(x => x.Item1, x => x.Item2);
    float timestamp = _watch.ElapsedTicks * 1f / Stopwatch.Frequency * 1000;

    foreach (KeyValuePair<short, float> keyState in buffer)
      if (!_lastBuffer.TryGetValue(keyState.Key, out float analogValue) || keyState.Value != analogValue)
        _frames.Add(new(_scanIndex, timestamp, (VirtualKey)keyState.Key, keyState.Value));

    foreach(KeyValuePair<short, float> keyState in _lastBuffer)
      if (!buffer.ContainsKey(keyState.Key))
        _frames.Add(new(_scanIndex, timestamp, (VirtualKey)keyState.Key, 0));

    _lastBuffer = buffer;
    _scanIndex++;
  }
}