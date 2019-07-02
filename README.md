# GroorineCore

A port of [Groorine](https://github.com/xeltica/groorine_1) to .NET Standard 2.0

## Example

```cs
using System.IO;
using Groorine;
using Groorine.DataModel;

// Load MIDI File
var smf = File.OpenRead("/path/to/midi.mid");
var data = SmfParser.Parse(smf);

// Play loaded file
var player = new Player();
player.Load(data);
player.Play();

// Create buffer of music
var buf = player.CreateBuffer(200);

while (player.IsPlaying)
{
    player.GetBuffer(buf);
    // use buf to output audio (implement platform-specificly)
}
```

## LICENSE

[MIT](LICENSE)