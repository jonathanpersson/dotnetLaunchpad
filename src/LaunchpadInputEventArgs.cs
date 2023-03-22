using Midi.Enums;

namespace DotnetLaunchpad;

/// <summary>
/// Launchpad button press event
/// </summary>
public class LaunchpadInputEventArgs : EventArgs
{
    /// <summary>
    /// Location of input
    /// </summary>
    public (int X, int Y) Location { get; }

    /// <summary>
    /// X-location of button press
    /// </summary>
    public int X => Location.X;

    /// <summary>
    /// Y-location of button press
    /// </summary>
    public int Y => Location.Y;

    public LaunchpadInputEventArgs((int X, int Y) location)
    {
        Location = location;
    }
}