# COM3D2.GUIExt TestPlugin

This example plugin showcases simple usage of standalone GUIExt. The plugin has also been compiled into `COM3D2.TestPlugin.Plugin.v0.0.01.zip`.

TestPlugin loads and immediately disables itself.

![output](../img/testpluginoutput.PNG)
![output](../img/testpluginmenu-disabled.PNG)

Upon clicking on the TestPlugin menu button, it is enabled and creates two new buttons.

![output](../img/testpluginmenu-enabled.png)

Clicking these buttons showcases different outline colours as well as console output.

![output](../img/testpluginmenu-buttons.png)
![output](../img/testpluginmenuoutput2.PNG)

When the TestPlugin button is clicked again, it be disabled and destroy the two buttons.

This should be compatible with PluginManager, although I haven't tested it.
