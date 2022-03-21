# SongChartVisualizer
SongChartVisualizer is a small mods that shows a configurable in-game graph that displays the NPS over time.
It also has the ability to warn you when you're about to reach peak NPS. The mod can be configured in the Mod Settings.

## Installation
This mod requires a few other mods in order to work.

- BSIPA v4.2.2 or higher
- BeatSaberMarkupLanguage v1.6.3 or higher
- SiraUtil v3.0.5 or higher

Installation is fairly simple.

1. Grab the latest plugin release from BeatMods/ModAssistant (once it's available) or from the [releases page](https://github.com/ErisApps/SongChartVisualizer/releases) (once there is actually a
   release)
2. Drop the .dll file in the Plugins folder of your Beat Saber installation.
3. Boot it up (or reboot)

## Developers
To build this project you will need to create a `ChartPlugin/SongChartVisualizer.csproj.user` file specifying where the game is located:

```xml
<?xml version="1.0" encoding="utf-8"?>
<Project xmlns="http://schemas.microsoft.com/developer/msbuild/2003">
  <PropertyGroup>
    <!-- Change this path if necessary. Make sure it doesn't end with a backslash. -->
    <BeatSaberDir>D:\Program Files (x86)\Steam\steamapps\common\Beat Saber</BeatSaberDir>
  </PropertyGroup>
</Project>
```

### Credits
Credit where credit is due:
- [@Shoko84](https://github.com/Shoko84) for writing the original mod
- [@MildPanda (Opzon)](https://github.com/MildPanda) for mod ideas in the original mod