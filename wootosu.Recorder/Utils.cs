using System.Security.Cryptography;
using System.Security.Cryptography.X509Certificates;
using wootosu.Shared.Models;

#pragma warning disable SYSLIB0057

namespace wootosu.Recorder;

internal static class Utils
{
  private const string WOOTING_ANALOG_SDK_LOCATION = "C:\\Program Files\\wooting-analog-sdk\\wooting_analog_sdk.dll";

  public static WootingAnalogSdkInfo? CollectWootingAnalogSdkInfo()
  {
    if (!File.Exists(WOOTING_ANALOG_SDK_LOCATION))
      return null;

    byte[] content = File.ReadAllBytes(WOOTING_ANALOG_SDK_LOCATION);
    string fileHash = Convert.ToHexStringLower(MD5.HashData(content));

    X509Certificate2 cert = new(X509Certificate.CreateFromSignedFile(WOOTING_ANALOG_SDK_LOCATION));
    byte[] certificate = cert.Export(X509ContentType.Cert);

    return new(certificate, content.Length, fileHash);
  }
}
