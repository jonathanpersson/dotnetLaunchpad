using Midi.Devices;
using Midi.Enums;
using Midi.Messages;

namespace DotnetLaunchpad;

public class Launchpad : IMidiDevice
{
    private const int ControlRowY = 0;
    private const int ControlRowX = 8;

    private static readonly Pitch[,] Pads =
    {
        {Pitch.CNeg1, Pitch.CSharpNeg1, Pitch.DNeg1, Pitch.DSharpNeg1, Pitch.ENeg1, Pitch.FNeg1, Pitch.FSharpNeg1, Pitch.GNeg1},
        {Pitch.E0, Pitch.F0, Pitch.FSharp0, Pitch.G0, Pitch.GSharp0, Pitch.A0, Pitch.ASharp0, Pitch.B0},
        {Pitch.GSharp1, Pitch.A1, Pitch.ASharp1, Pitch.B1, Pitch.C2, Pitch.CSharp2, Pitch.D2, Pitch.DSharp2},
        {Pitch.C3, Pitch.CSharp3, Pitch.D3, Pitch.DSharp3, Pitch.E3, Pitch.F3, Pitch.FSharp3, Pitch.G3},
        {Pitch.E4, Pitch.F4, Pitch.FSharp4, Pitch.G4, Pitch.GSharp4, Pitch.A4, Pitch.ASharp4, Pitch.B4},
        {Pitch.GSharp5, Pitch.A5, Pitch.ASharp5, Pitch.B5, Pitch.C6, Pitch.CSharp6, Pitch.D6, Pitch.DSharp6},
        {Pitch.C7, Pitch.CSharp7, Pitch.D7, Pitch.DSharp7, Pitch.E7, Pitch.F7, Pitch.FSharp7, Pitch.G7},
        {Pitch.E8, Pitch.F8, Pitch.FSharp8, Pitch.G8, Pitch.GSharp8, Pitch.A8, Pitch.ASharp8, Pitch.B8}
    };

    private static readonly Pitch[] ControlsRight =
    {
        Pitch.GSharpNeg1, Pitch.C1, Pitch.E2, Pitch.GSharp3, Pitch.C5, Pitch.E6, Pitch.GSharp7, Pitch.C9
    };

    private static readonly int[] ControlsTop =
    {
        104, 105, 106, 107, 108, 109, 110, 111
    };

    /// <summary>
    /// Launchpad device name
    /// </summary>
    public string Name { get; }
    
    /// <summary>
    /// Launchpad MIDI Input
    /// </summary>
    public InputDevice Input { get; }
    
    /// <summary>
    /// Launchpad MIDI Output
    /// </summary>
    public OutputDevice Output { get; }
    
    public event EventHandler? LaunchpadInputEventHandler;

    public Launchpad(IMidiDevice device)
    {
        Name = device.Name;
        Input = device.Input;
        Output = device.Output;
    }

    public Launchpad(string name, InputDevice input, OutputDevice output)
    {
        Name = name;
        Input = input;
        Output = output;
    }

    /// <summary>
    /// Check if button is part of the top-row of control buttons
    /// </summary>
    /// <param name="key">Key to check</param>
    /// <returns>True iff key is in the top control row</returns>
    public static bool IsTopControl(Pitch key)
    {
        return false;
    }

    /// <summary>
    /// Check if button is part of the top-row of control buttons
    /// </summary>
    /// <param name="key">Key to check</param>
    /// <returns>True iff key is in the top control row</returns>
    public static bool IsTopControl(int key)
    {
        return ControlsTop.Contains(key);
    }

    /// <summary>
    /// Check if button is part of the right-row of control buttons
    /// </summary>
    /// <param name="key">Key to check</param>
    /// <returns>True iff key is in the right control row</returns>
    public static bool IsRightControl(Pitch key)
    {
        return ControlsRight.Contains(key);
    }

    /// <summary>
    /// Check if button is part of the right-row of control buttons
    /// </summary>
    /// <param name="key">Key to check</param>
    /// <returns>True iff key is in the right control row</returns>
    public static bool IsRightControl(int key)
    {
        return false;
    }

    /// <summary>
    /// Check if a button is part of one of the control rows
    /// </summary>
    /// <param name="key">Key to check</param>
    /// <returns>True iff key is ín one of the control rows</returns>
    public static bool IsControl(Pitch key)
    {
        return IsTopControl(key) || IsRightControl(key);
    }

    /// <summary>
    /// Check if a button is part of one of the control rows
    /// </summary>
    /// <param name="key">Key to check</param>
    /// <returns>True iff key is ín one of the control rows</returns>
    public static bool IsControl(int key)
    {
        return IsTopControl(key) || IsRightControl(key);
    }

    /// <summary>
    /// Check if a button is part of the main grid of notes.
    /// </summary>
    /// <param name="key">Key to check</param>
    /// <returns>True if key is not a control note</returns>
    public static bool IsNote(Pitch key)
    {
        return !IsControl(key);
    }

    /// <summary>
    /// Check if a button is part of the main grid of notes.
    /// </summary>
    /// <param name="key">Key to check</param>
    /// <returns>True if key is not a control note</returns>
    public static bool IsNote(int key)
    {
        return !IsControl(key);
    }

    /// <summary>
    /// Translate pitch to coordinates on launchpad
    /// </summary>
    /// <param name="key">The pitch to translate</param>
    /// <returns>X,Y location of button press</returns>
    /// <exception cref="ArgumentException">Given pitch is not valid</exception>
    private static (int X, int Y) GetCoordinates(Pitch key)
    {
        if (IsControl(key)) 
            return (8, Array.IndexOf(ControlsRight, key) + 1);

        for (int y = 0; y < 8; y++)
        {
            for (int x = 0; x < 8; x++)
            {
                if (Pads[y, x] == key)
                    return (x, y + 1);
            }
        }

        throw new ArgumentException($"Pitch does not exist as control or pad: {key}");
    }

    /// <summary>
    /// Translate control to coordinates on launchpad
    /// </summary>
    /// <param name="key">The control to translate</param>
    /// <returns>X,Y location of button press</returns>
    /// <exception cref="ArgumentException">Given control is not valid</exception>
    private static (int X, int Y) GetCoordinates(int key)
    {
        if (IsControl(key))
            return (Array.IndexOf(ControlsTop, key), 0);

        throw new ArgumentException($"Control key does not exist as a control or pad: {key}");
    }

    /// <summary>
    /// Set LED light at given location
    /// </summary>
    /// <param name="location">X,Y location of button to set light of</param>
    /// <param name="velocity">Determines color</param>
    public void SetLight((int X, int Y) location, sbyte velocity)
    {
        if (location is { X: 8, Y: 0 }) return;
        if (location.Y == ControlRowY) SetLight(ControlsTop[location.X], velocity);
        else if (location.X == ControlRowX) SetLight(ControlsRight[location.Y - 1], velocity);
        else SetLight(Pads[location.Y - 1, location.X], velocity);
    }

    /// <summary>
    /// Set LED light of given control
    /// </summary>
    /// <param name="control">Control to set light of</param>
    /// <param name="velocity">Determines color</param>
    public void SetLight(int control, sbyte velocity)
    {
        try
        {
            Output.SendControlChange(Channel.Channel1, (Control)control, velocity);
        }
        catch (DeviceException)
        {
            Console.WriteLine($"ERROR: Failed to send signal ({velocity}) to MIDI output: {Name} {control}");
        }
    }

    /// <summary>
    /// Set LED light of given pitch
    /// </summary>
    /// <param name="pitch">Pitch to set light of</param>
    /// <param name="velocity">Determines color</param>
    public void SetLight(Pitch pitch, sbyte velocity)
    {
        try
        {
            Output.SendNoteOn(Channel.Channel1, pitch, velocity);
        }
        catch (DeviceException)
        {
            Console.WriteLine($"ERROR: Failed to send signal ({velocity}) to MIDI output: {Name} {pitch}");
        }
    }

    /// <summary>
    /// Clear launchpad LEDs
    /// </summary>
    public void Clear()
    {
        for (int y = 0; y <= 8; y++)
        {
            for (int x = 0; x <= 8; x++)
            {
                SetLight((x, y), 0);
            }
        }
    }

    /// <summary>
    /// Default MIDI input handler for Note-on event
    /// </summary>
    /// <param name="msg">MIDI Note-on Event Message</param>
    /// todo: Different OnPress and OnRelease events?
    public void MidiPressHandler(NoteOnMessage msg)
    {
        if (msg.Velocity > 0) return;
        (int X, int Y) coordinates = GetCoordinates(msg.Pitch);
        // Console.WriteLine($"EVENT: Pitch {msg.Pitch} (mapped to {coordinates}) detected!");
        if (LaunchpadInputEventHandler == null) return;
        LaunchpadInputEventArgs e = new(coordinates);
        LaunchpadInputEventHandler(this, e);
    }

    /// <summary>
    /// Default MIDI input handler for Control changes
    /// </summary>
    /// <param name="msg">MIDI Control Change Message</param>
    /// todo: Different OnPress and OnRelease events? (2)
    public void MidiPressHandler(ControlChangeMessage msg)
    {
        if (msg.Value > 0) return;
        (int X, int Y) coordinates = GetCoordinates((int) msg.Control);
        // Console.WriteLine($"EVENT: Control {msg.Control} (mapped to {coordinates}) detected!");
        if (LaunchpadInputEventHandler == null) return;
        LaunchpadInputEventArgs e = new(coordinates);
        LaunchpadInputEventHandler(this, e);
    }
}