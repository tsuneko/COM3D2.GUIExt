# COM3D2.GUIExt
Extension of [CM3D2.GearMenu](https://github.com/neguse11/CM3D2.GearMenu) ported for COM3D2, partially hotfixing the lack of tooltips for ported CM3D2 plugins, which were broken due to a change in `SystemShortcut.VisibleExplanation`.

## Requirements

- UnityInjector.dll 1.0.4.1+
- ExIni.dll 1.0.2.1+
- PluginExt.dll 2.0.5727+
All of the required dlls are bundled in Sybaris and BepInEx AIO packs.

## Compiling Requirements

- Target Framework .NET 3.5

## Usage

Unlike GearMenu, GUIExt is built as a plugin with the button methods exposed statically. The basic usage is essentially identical to reduce effort in porting from CM3D2 to COM3D2. The main emphasis of this library was to avoid causing issues with ported CM3D2 plugins which are using GearMenu.

```C#
using COM3D2.GUIExt;

namespace COM3D2.Example.Plugin
  [PluginName("Example"), PluginVersion("0.0.0.1")]
  public class ExamplePlugin : PluginBase
  {
    GameObject goButton;
    void Awake()
    {
      goButton = GUIExt.Add("Example", "Toggle Example Button", (go) => { enabled = !enabled; });
    }
    void OnEnable()
    {
      GUIExt.SetFrameColor(goButton, Color.red);
    }
    void OnDisable()
    {
      GUIExt.ResetFrameColor(goButton);
    }
    void OnGUI() {
      GUI.Label(new Rect(0, 0, Screen.width, Screen.height), "Example Text");
    }
  }
}
```

## Extension Capabilities

GUIExt is able to hide buttons from the gear menu through modification of `UnityInjector/Config/GUIExtButtons.ini`. The buttons to be hidden or visible are customisable for each scene. This does not improve performance, but it can reduce clutter, especially if many plugins are loaded which utilise GearMenu or GUIExt.


## SystemShortcut.VisibleExplanation
As GUIExt is loaded as a plugin, it is able to monitor the status of the gear menu. CM3D2 plugins which were ported to COM3D2


