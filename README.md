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

GUIExt is able to hide buttons from the gear menu through modification of `UnityInjector/Config/GUIExtButtons.ini`. The buttons to be hidden or visible are customisable for each scene. This does not improve performance, but it can reduce clutter, especially if many plugins are loaded which utilise GearMenu or GUIExt. An example of this is a greatly cleaned up title screen menu:

![Title Screen Menu](img/title.png)

The format of `UnityInjector/Config/GUIExtButtons.ini` is:
```ini
[Global]
; Buttons in this section are updated on every scene change, allowing for either whitelisting or blacklisting depending on usage
ToTitle=True
; ToTitle is the name of the return to title screen button. In this case, we wish to only disable it in the title screen, so we enable it everywhere else
Shop=False
; In this example, we wish to disable the Shop button entirely. This button only appears if you are connected to the internet.
[SceneTitle]
; Every subsequent section has the same name as a scene. Only a few are listed in the default config, but any scene in the game can be used.
ToTitle=False
; As mentioned above, we wish to disable the return to title screen button while in the title screen, as it is useless.
Help=False
; The Help button is also greyed out in the title screen, so we will hide it.
Dic=False
; This is the glossary popup button, we don't need it in the title screen.
Info=False
; This is the tutorial popup button, we don't need it in the title screen.
Example Plugin=False
; The majority of plugins which use GearMenu can be hidden additionally. More details on their naming is provided below.
[SceneEdit]
; This is the edit menu, we could hide or show other buttons here if desired
```


## SystemShortcut.VisibleExplanation
As GUIExt is loaded as a plugin, it is able to monitor the status of the gear menu. CM3D2 plugins which were ported to COM3D2


