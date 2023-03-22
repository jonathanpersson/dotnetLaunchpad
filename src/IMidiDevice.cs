using Midi.Devices;
using Midi.Messages;

namespace DotnetLaunchpad;

public interface IMidiDevice
{
    public string Name { get; }
    public InputDevice Input { get; }
    public OutputDevice Output { get; }
    
    public void MidiPressHandler(NoteOnMessage msg);
    public void MidiPressHandler(ControlChangeMessage msg);
}