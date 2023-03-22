# dotnetLaunchpad
dotnetLaunchpad is a library that lets you control a Novation Launchpad in your .NET program.

## Features
- Quickly find all connected Launchpad MIDI devices
- Control connected launchpads lights using simple (X,Y) coordinates
- Add your own custom events to all pad-presses
- Control both the 8x8 pad grid and the two 1x8 control button strips
- Easily control multiple launchpads separately (if you for some reason need more than 80 macro buttons)

## Get started
Clone (or download) the dotnetLaunchpad repo and import the project into your existing solution. 
Then add the dotnetLaunchpad project into your own project's dependencies. Make sure all nuget dependencies
are downloaded, and you're good to go!

### Get connected launchpads
To get a list of all connected launchpads, use the ```GetLaunchpads()``` method in the
```LaunchpadController``` class like so:

````csharp
List<Launchpad> launchpads = new(LaunchpadController.GetLaunchpads());
````

This will give you a list containing all connected launchpads as instances of ```Launchpad```. The launchpads in the
list still need to be connected to. To **connect** to one (or more) of them, use the ```Connect()``` method in the
```LaunchpadController```. For instance:

````csharp
if (LaunchpadController.Connect(launchpad))
{
    // successfully connected, do your thing...
}
````

Also always remember to **disconnect** from any connected launchpads before quitting:

````csharp
if (LaunchpadController.Disconnect(launchpad))
{
    // successfully disconnected, do your thing...
}
````

### Control a launchpad's lights
Control a launchpad's lights using its ```SetLight(location, velocity)``` method. Like so:

````csharp
(int X, int Y) location = (3, 4);
launchpad.SetLight(location, 3); // 3 == Bright red on older Launchpad models
````

### Add an event handler for pad presses

````csharp
launchpad.LaunchpadInputEventHandler += MyLaunchpadInputEventHandler;
````
or

````csharp
launchpad.LaunchpadInputEventHandler += (sender, eventArgs) =>
{
    // your event handler here
};
````
The ```EventArgs``` object contains information about the (X,Y)-location of the button press. For example, you
could turn on the light for each button that is pressed like so:

````csharp
launchpad.LaunchpadInputEventHandler += (sender, eventArgs) =>
{
    if (sender is not Launchpad launchpad || eventArgs is not LaunchpadInputEventArgs e) return;
    launchpad.SetLight(e.Location, 3);
};
````

## To-do's
- Replace current event handler with two different ones (pad press, and pad release)
- Light color presets to replace current magic-number guessing system. (Requires supporting multiple configurations
for different Launchpad generations)
- Validate support for different Launchpads
- Support for non-launchpad MIDI devices with similar pad controls
