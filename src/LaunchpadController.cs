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
    /// <param name="device">The MIDI device to connect to</param>
    /// <returns>True iff connection successful</returns>
    public static bool Connect(IMidiDevice device)
    {
        device.Input.Open();
        device.Input.NoteOn += device.MidiPressHandler;
        device.Input.ControlChange += device.MidiPressHandler;
        device.Input.StartReceiving(null);
        device.Output.Open();
        return device.Input.IsOpen && device.Output.IsOpen;
    }

    /// <summary>
    /// Disconnect from MIDI device
    /// </summary>
    /// <param name="device">The MIDI device  to disconnect from</param>
    /// <returns>True iff disconnection successful</returns>
    public static bool Disconnect(IMidiDevice device)
    {
        if (device.Input.IsOpen)
        {
            if (device is Launchpad l) l.Clear();
            device.Input.StopReceiving();
            device.Input.Close();
        }
        if (device.Output.IsOpen) device.Output.Close();
        return !device.Input.IsOpen && !device.Output.IsOpen;
    }
}