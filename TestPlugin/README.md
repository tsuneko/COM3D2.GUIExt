# COM3D2.GUIExt TestPlugin

This example plugin showcases simple usage of `COM3D2.GUIExt.cs`.

## Example

TestPlugin loads and immediately disables itself.

![output](../img/testpluginoutput.png)
![menu disabled](../img/testpluginmenu-disabled.png)

Upon clicking on the TestPlugin menu button, it is enabled and creates two new buttons.

![menu enabled](../img/testpluginmenu-enabled.png)

Clicking these buttons showcases different outline colours as well as console output.

![menu buttons](../img/testpluginmenu-buttons-enabled.png)
![more output](../img/testpluginoutput2.PNG)

When the TestPlugin button is clicked again, it be disabled and destroy the two buttons.

This should be compatible with PluginManager, although I haven't tested it.
