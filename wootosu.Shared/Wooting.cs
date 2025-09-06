namespace wootosu.Shared;

/// <summary>
/// Provides constants and utility functions for Wooting-related calculations and specifications.
/// </summary>
public static class Wooting
{
  /// <summary>
  /// The travel distance of a Wooting keyboard key in millimeters.
  /// </summary>
  public const int TRAVEL_DISTANCE_MM = 4;

  /// <summary>
  /// Converts the provided analog value (0.0 to 1.0) to millimeters.
  /// </summary>
  public static double AnalogToMm(double analogValue) => analogValue * TRAVEL_DISTANCE_MM;

  /// <summary>
  /// Converts the provided millimeter value to an analog value (0.0 to 1.0).
  /// </summary>
  public static double MmToAnalog(double mm) => mm / TRAVEL_DISTANCE_MM;
}
