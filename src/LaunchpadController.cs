using Midi.Devices;

namespace DotnetLaunchpad;

public static class LaunchpadController
{
    /// <summary>
    /// Get all MIDI devices that are launchpads
    /// </summary>
    /// <returns>All MIDI devices that are launchpads</returns>
    public static IEnumerable<Launchpad> GetLaunchpads()
    {
        List<Launchpad> launchpads = new();
        Dictionary<string, InputDevice> inputDevices = new();
        
        foreach (IInputDevice device in DeviceManager.InputDevices)
        {
            Console.WriteLine($"Found MIDI Input Device: {device.Name}");
            inputDevices.Add(device.Name, (InputDevice) device);
        }
        
        foreach (IOutputDevice device in DeviceManager.OutputDevices)
        {
            Console.WriteLine($"Found MIDI Output Device: {device.Name}");
            if (!inputDevices.ContainsKey(device.Name) || !device.Name.ToLower().Contains("launchpad")) continue;
            launchpads.Add(new Launchpad(device.Name, inputDevices[device.Name], (OutputDevice)device));
        }

        Console.WriteLine($"Identified {launchpads.Count} Launchpads");

        return launchpads.ToArray();
    }

    /// <summary>
    /// Connect to MIDI device
    /// </summary>
    /// <param name="launchpad">The device (launchpad) to connect to</param>
    /// <returns>True iff connection successful</returns>
    public static bool Connect(Launchpad launchpad)
    {
        launchpad.Input.Open();
        launchpad.Input.NoteOn += launchpad.MidiPressHandler; // Main 8x8 + Right row
        launchpad.Input.ControlChange += launchpad.MidiPressHandler; // Top row
        launchpad.Input.StartReceiving(null);
        launchpad.Output.Open();
        return launchpad.Input.IsOpen && launchpad.Output.IsOpen;
    }

    /// <summary>
    /// Disconnect from MIDI device
    /// </summary>
    /// <param name="launchpad">The device (launchpad) to disconnect from</param>
    /// <returns>True iff disconnection successful</returns>
    public static bool Disconnect(Launchpad launchpad)
    {
        if (launchpad.Input.IsOpen)
        {
            launchpad.Clear();
            launchpad.Input.StopReceiving();
            launchpad.Input.Close();
        }
        if (launchpad.Output.IsOpen) launchpad.Output.Close();
        return !launchpad.Input.IsOpen && !launchpad.Output.IsOpen;
    }
}