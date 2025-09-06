using Newtonsoft.Json;
using WootingAnalogSDKNET;

namespace wootosu.Shared.Models;

public class RecordingDeviceInfo
{
#pragma warning disable CS8618
  public RecordingDeviceInfo() { }

  public RecordingDeviceInfo(DeviceInfo deviceInfo)
  {
    VendorId = deviceInfo.vendor_id;
    ProductId = deviceInfo.product_id;
    ManufacturerName = deviceInfo.manufacturer_name;
    DeviceName = deviceInfo.device_name;
    DeviceId = deviceInfo.device_id;
    DeviceType = deviceInfo.device_type;
  }

  [JsonProperty("vendor_id")]
  public ushort VendorId { get; private set; }

  [JsonProperty("product_id")]
  public ushort ProductId { get; private set; }

  [JsonProperty("manufacturer_name")]
  public string ManufacturerName { get; private set; }

  [JsonProperty("device_name")]
  public string DeviceName { get; private set; }

  [JsonProperty("device_id")]
  public ulong DeviceId { get; private set; }

  [JsonProperty("device_type")]
  public DeviceType DeviceType { get; private set; }
}
