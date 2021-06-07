# SongChartVisualizer
SongChartVisualizer is a small mods that shows a configurable in-game graph that displays the NPS over time.
It also has the ability to warn you when you're about to reach peak NPS. The mod can be configured in the Mod Settings.

## Installation
Installation is fairly simple.
1. Grab the latest plugin release from either ModAssistant (if it's available) or the [releases page](https://github.com/ErisApps/SongChartVisualizer/releases)
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