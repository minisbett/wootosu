using System.Diagnostics;
using System.Globalization;
using System.Security.Cryptography;
using System.Text;
using Newtonsoft.Json;
using Spectre.Console;
using WootingAnalogSDKNET;
using wootosu.Recorder;
using wootosu.Shared;
using wootosu.Shared.Models;
using static System.Console;
using Recorder = wootosu.Recorder.Recorder;

public class Program
{
  public static readonly string VERSION = "1.0.0";

  public static void Main(string[] args)
  {
    CultureInfo.CurrentCulture = CultureInfo.InvariantCulture;
    Title = $"wootosu.Recorder v{VERSION}";

    Step1_DeviceSelection();
  }

  private static void Step1_DeviceSelection()
  {
    Clear();
    (int deviceNo, WootingAnalogResult error) = WootingAnalogSDK.Initialise();
    if (error < 0)
    {
      WriteLine();
      AnsiConsole.MarkupLine($"[red]Error initialising Wooting Analog SDK:[/] [yellow]{error}[/]");
      ReadKey();
      return;
    }
    else if (deviceNo == 0)
    {
      WriteLine();
      AnsiConsole.MarkupLine("[red]No Wooting devices found.[/]");
      ReadKey();
      return;
    }

    WootingAnalogSDK.SetKeycodeMode(KeycodeType.VirtualKey);

    (List<DeviceInfo> devices, error) = WootingAnalogSDK.GetConnectedDevicesInfo();
    if (error != WootingAnalogResult.Ok)
    {
      WriteLine();
      AnsiConsole.MarkupLine($"[red]Error getting connected devices:[/] [yellow]{error}[/]");
      ReadKey();
      return;
    }

    if (devices.Count == 0)
    {
      WriteLine();
      AnsiConsole.WriteLine("[red]No Wooting devices connected.[/");
      ReadKey();
      return;
    }

    DeviceInfo deviceInfo = devices[0];
    if (devices.Count > 1)
    {
      Clear();
      string choice = AnsiConsole.Prompt(
        new SelectionPrompt<string>()
          .Title("Multiple devices found. Please select a Wooting device:")
          .AddChoices(devices.Select(x => $"{x.device_name} ({x.device_id})")));

      deviceInfo = devices.First(x => choice.Contains(x.device_id.ToString()));
    }

    Clear();
    AnsiConsole.MarkupLine($"Selected device: [blue]{deviceInfo.device_name}[/] ({deviceInfo.device_id})");
    WriteLine();
    AnsiConsole.MarkupLine("[yellow]NOTE: This recorder will record key presses performed by you while it is running.[/]");
    AnsiConsole.MarkupLine("[yellow]      Please only use this recorder and share the output when asked to by trusted sources.[/]");
    WriteLine();
    WriteLine("Press Enter to continue...");
    while (ReadKey(true).Key != ConsoleKey.Enter)
      ;

    Step2_Recorder(deviceInfo);
  }


  private static void Step2_Recorder(DeviceInfo deviceInfo)
  {
    Clear();
    CursorVisible = false;
    Recorder recorder = new(deviceInfo);
    recorder.Start();
    long startTime = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds();

    AnsiConsole.MarkupLine("[springgreen3_1]⬤ Recording:[/]           [gray]Press [[ESC]] to stop.[/]");
    WriteLine();
    WriteLine("Frames:");

    bool isEscPressed = false;
    new Thread(() =>
    {
      while (ReadKey(true).Key != ConsoleKey.Escape)
        ;

      isEscPressed = true;
    }).Start();

    Task consoleRenderTask = Task.Run(async () =>
    {
      while (!isEscPressed)
      {
        await Task.Delay(50);

        // Elapsed recording time
        SetCursorPosition(13, 0);
        WriteLine($"{(int)recorder.Elapsed.TotalMinutes:0#}:{recorder.Elapsed.Seconds:0#}");

        // Last 20 frames
        SetCursorPosition(0, 3);
        foreach (RecordFrame frame in recorder.Frames.TakeLast(20).Reverse())
          AnsiConsole.MarkupLine($"[gray]{frame.Timestamp / 1000d,7:N2}s[/] [green]{frame.KeyCode}[/] {frame.AnalogValue * Wooting.TRAVEL_DISTANCE_MM:N2}mm            ");
      }
    });

    while (!isEscPressed)
      recorder.Process();

    recorder.Stop();
    long endTime = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds();

    consoleRenderTask.Wait();

    RecordingInfo recordingInfo = new(startTime, endTime, false);
    RecordingDeviceInfo recordingDeviceInfo = new(deviceInfo);
    WootingAnalogSdkInfo? sdkInfo = Utils.CollectWootingAnalogSdkInfo();
    Recording recording = new(recordingInfo, recordingDeviceInfo, [.. recorder.Frames], [.. recorder.Errors], sdkInfo);

    Step3_Export(recording);
  }

  private static void Step3_Export(Recording recording)
  {
    string filePath = Path.Combine("recordings", $"recording-{DateTime.UtcNow:yyyy-MM-dd_HH-mm-ss}.json");
    string json = JsonConvert.SerializeObject(recording, Formatting.Indented);

    Directory.CreateDirectory("recordings");
    File.WriteAllText(filePath, json);

    string recordingHash = Convert.ToHexStringLower(MD5.HashData(Encoding.Default.GetBytes(json)));

    WootingAnalogSDK.UnInitialise();

    Clear();
    TimeSpan duration = TimeSpan.FromMilliseconds(recording.RecordingInfo.EndTime - recording.RecordingInfo.StartTime);
    AnsiConsole.MarkupLine($"[red]■ Recording stopped.[/]     [gray]Duration: {(int)duration.TotalMinutes:0#}:{duration.Seconds:0#}[/]");
    WriteLine();
    AnsiConsole.MarkupLine($"Recorded device: [blue]{recording.DeviceInfo.DeviceName}[/] ({recording.DeviceInfo.DeviceId})");
    AnsiConsole.MarkupLine($"Recorded frames: [yellow]{recording.Frames.Length}[/] ({recording.Errors.Length} [red]errors[/])");
    AnsiConsole.MarkupLine($"Recording saved to: [aqua]{filePath}[/]");
    AnsiConsole.MarkupLine($"Recording hash: [lime]{recordingHash}[/]");
    WriteLine();

    bool isFileDeleted = false;
    while (true)
    {
      List<string> allChoices = ["View recording file", "Open recordings folder", "Delete recording", "Perform another recording", "Exit"];
      List<string> choices = [.. allChoices];
      if (isFileDeleted)
      {
        choices.RemoveAt(2);
        choices.RemoveAt(0);
      }

      string choice = AnsiConsole.Prompt(
          new SelectionPrompt<string>()
            .Title("How would you like to proceed?")
            .AddChoices(choices));

      switch (allChoices.IndexOf(choice))
      {
        case 0:
          Process.Start(new ProcessStartInfo
          {
            FileName = filePath,
            UseShellExecute = true
          });
          break;

        case 1:
          Process.Start("explorer.exe", isFileDeleted ? $"\"{new FileInfo(filePath).DirectoryName}\"" : $"/select,\"{filePath}\"");
          break;

        case 2:
          File.Delete(filePath);
          isFileDeleted = true;
          break;

        case 3:
          Step1_DeviceSelection();
          return;

        case 4:
          return;
      }
    }
  }
}