using System;
using UnityEngine;
using UnityInjector.Attributes;
using PluginExt;
using COM3D2.GUIExtBase;

namespace COM3D2.TestPlugin.Plugin
{
    [PluginFilter("COM3D2x64"), PluginName("COM3D2.TestPlugin.Plugin"), PluginVersion("0.0.0.1")]
    public class TestPlugin : ExPluginBase
    {
        public GameObject button;
        public GameObject blue;
        public GameObject green;

        bool blue_enabled = false;
        bool green_enabled = false;

        public void Awake()
        {
            WriteLine("Test Message!");
            WriteLine("Test Error Message!", true);
            Write("c", ConsoleColor.Red);
            Write("o", ConsoleColor.Yellow);
            Write("l", ConsoleColor.Green);
            Write("o", ConsoleColor.Cyan);
            Write("r", ConsoleColor.Blue);
            Write("s", ConsoleColor.Magenta);
            Write("\n", ConsoleColor.White);
            WriteLine2("Test Message without Prefix!");

            GameObject.DontDestroyOnLoad(this);
            button = GUIExt.Add("TestPlugin", "Toggle TestPlugin", icon, (go) => { enabled = !enabled; });

            enabled = false;

            WriteLine("Loaded.");
        }

        private void createButtons()
        {
            blue = GUIExt.Add("RedButton", "TestButton", (go) =>
            {
                blue_enabled = !blue_enabled;
                if (blue_enabled)
                {
                    GUIExt.SetFrameColor(blue, Color.blue);
                    WriteLine("Blue Enabled!");
                }
                else
                {
                    GUIExt.ResetFrameColor(blue);
                    WriteLine("Blue Disabled!");
                }
            });

            green = GUIExt.Add("GreenButton", "TestButton2", (go) =>
            {
                green_enabled = !green_enabled;
                if (green_enabled)
                {
                    GUIExt.SetFrameColor(green, Color.green);
                    WriteLine("Green Enabled!");
                }
                else
                {
                    GUIExt.ResetFrameColor(green);
                    WriteLine("Green Disabled!");
                }
            });
        }

        private void destroyButtons()
        {
            GUIExt.Destroy(blue);
            GUIExt.Destroy(green);
        }

        private void OnEnable()
        {
            WriteLine("Plugin Enabled!");
            GUIExt.SetFrameColor(button, Color.red);
            createButtons();
        }

        private void OnDisable()
        {
            WriteLine("Plugin Disabled!");
            GUIExt.ResetFrameColor(button);
            destroyButtons();
        }

        private void WriteLine(string message, bool error = false)
        {
            GUIExt.WriteLine("[TestPlugin] ", ConsoleColor.Green, message, ConsoleColor.White, error);
        }

        private void Write(string message, ConsoleColor color)
        {
            GUIExt.Write(message, color, false);
        }

        private void WriteLine2(string message, bool prefix = true, bool error = false)
        {
            if (prefix)
            {
                GUIExt.WriteLine("[TestPlugin] ", ConsoleColor.Green, message, ConsoleColor.White, error);
            }
            else
            {
                GUIExt.WriteLine(message, ConsoleColor.White, error);
            }
        }

        private byte[] icon = Convert.FromBase64String(
            "iVBORw0KGgoAAAANSUhEUgAAACAAAAAgCAYAAABzenr0AAAACXBIWXMAAAsTAAALEwEAmpwYAAAKT2lDQ1BQaG" +
            "90b3Nob3AgSUNDIHByb2ZpbGUAAHjanVNnVFPpFj333vRCS4iAlEtvUhUIIFJCi4AUkSYqIQkQSoghodkVUcER" +
            "RUUEG8igiAOOjoCMFVEsDIoK2AfkIaKOg6OIisr74Xuja9a89+bN/rXXPues852zzwfACAyWSDNRNYAMqUIeEe" +
            "CDx8TG4eQuQIEKJHAAEAizZCFz/SMBAPh+PDwrIsAHvgABeNMLCADATZvAMByH/w/qQplcAYCEAcB0kThLCIAU" +
            "AEB6jkKmAEBGAYCdmCZTAKAEAGDLY2LjAFAtAGAnf+bTAICd+Jl7AQBblCEVAaCRACATZYhEAGg7AKzPVopFAF" +
            "gwABRmS8Q5ANgtADBJV2ZIALC3AMDOEAuyAAgMADBRiIUpAAR7AGDIIyN4AISZABRG8lc88SuuEOcqAAB4mbI8" +
            "uSQ5RYFbCC1xB1dXLh4ozkkXKxQ2YQJhmkAuwnmZGTKBNA/g88wAAKCRFRHgg/P9eM4Ors7ONo62Dl8t6r8G/y" +
            "JiYuP+5c+rcEAAAOF0ftH+LC+zGoA7BoBt/qIl7gRoXgugdfeLZrIPQLUAoOnaV/Nw+H48PEWhkLnZ2eXk5NhK" +
            "xEJbYcpXff5nwl/AV/1s+X48/Pf14L7iJIEyXYFHBPjgwsz0TKUcz5IJhGLc5o9H/LcL//wd0yLESWK5WCoU41" +
            "EScY5EmozzMqUiiUKSKcUl0v9k4t8s+wM+3zUAsGo+AXuRLahdYwP2SycQWHTA4vcAAPK7b8HUKAgDgGiD4c93" +
            "/+8//UegJQCAZkmScQAAXkQkLlTKsz/HCAAARKCBKrBBG/TBGCzABhzBBdzBC/xgNoRCJMTCQhBCCmSAHHJgKa" +
            "yCQiiGzbAdKmAv1EAdNMBRaIaTcA4uwlW4Dj1wD/phCJ7BKLyBCQRByAgTYSHaiAFiilgjjggXmYX4IcFIBBKL" +
            "JCDJiBRRIkuRNUgxUopUIFVIHfI9cgI5h1xGupE7yAAygvyGvEcxlIGyUT3UDLVDuag3GoRGogvQZHQxmo8WoJ" +
            "vQcrQaPYw2oefQq2gP2o8+Q8cwwOgYBzPEbDAuxsNCsTgsCZNjy7EirAyrxhqwVqwDu4n1Y8+xdwQSgUXACTYE" +
            "d0IgYR5BSFhMWE7YSKggHCQ0EdoJNwkDhFHCJyKTqEu0JroR+cQYYjIxh1hILCPWEo8TLxB7iEPENyQSiUMyJ7" +
            "mQAkmxpFTSEtJG0m5SI+ksqZs0SBojk8naZGuyBzmULCAryIXkneTD5DPkG+Qh8lsKnWJAcaT4U+IoUspqShnl" +
            "EOU05QZlmDJBVaOaUt2ooVQRNY9aQq2htlKvUYeoEzR1mjnNgxZJS6WtopXTGmgXaPdpr+h0uhHdlR5Ol9BX0s" +
            "vpR+iX6AP0dwwNhhWDx4hnKBmbGAcYZxl3GK+YTKYZ04sZx1QwNzHrmOeZD5lvVVgqtip8FZHKCpVKlSaVGyov" +
            "VKmqpqreqgtV81XLVI+pXlN9rkZVM1PjqQnUlqtVqp1Q61MbU2epO6iHqmeob1Q/pH5Z/YkGWcNMw09DpFGgsV" +
            "/jvMYgC2MZs3gsIWsNq4Z1gTXEJrHN2Xx2KruY/R27iz2qqaE5QzNKM1ezUvOUZj8H45hx+Jx0TgnnKKeX836K" +
            "3hTvKeIpG6Y0TLkxZVxrqpaXllirSKtRq0frvTau7aedpr1Fu1n7gQ5Bx0onXCdHZ4/OBZ3nU9lT3acKpxZNPT" +
            "r1ri6qa6UbobtEd79up+6Ynr5egJ5Mb6feeb3n+hx9L/1U/W36p/VHDFgGswwkBtsMzhg8xTVxbzwdL8fb8VFD" +
            "XcNAQ6VhlWGX4YSRudE8o9VGjUYPjGnGXOMk423GbcajJgYmISZLTepN7ppSTbmmKaY7TDtMx83MzaLN1pk1mz" +
            "0x1zLnm+eb15vft2BaeFostqi2uGVJsuRaplnutrxuhVo5WaVYVVpds0atna0l1rutu6cRp7lOk06rntZnw7Dx" +
            "tsm2qbcZsOXYBtuutm22fWFnYhdnt8Wuw+6TvZN9un2N/T0HDYfZDqsdWh1+c7RyFDpWOt6azpzuP33F9JbpL2" +
            "dYzxDP2DPjthPLKcRpnVOb00dnF2e5c4PziIuJS4LLLpc+Lpsbxt3IveRKdPVxXeF60vWdm7Obwu2o26/uNu5p" +
            "7ofcn8w0nymeWTNz0MPIQ+BR5dE/C5+VMGvfrH5PQ0+BZ7XnIy9jL5FXrdewt6V3qvdh7xc+9j5yn+M+4zw33j" +
            "LeWV/MN8C3yLfLT8Nvnl+F30N/I/9k/3r/0QCngCUBZwOJgUGBWwL7+Hp8Ib+OPzrbZfay2e1BjKC5QRVBj4Kt" +
            "guXBrSFoyOyQrSH355jOkc5pDoVQfujW0Adh5mGLw34MJ4WHhVeGP45wiFga0TGXNXfR3ENz30T6RJZE3ptnMU" +
            "85ry1KNSo+qi5qPNo3ujS6P8YuZlnM1VidWElsSxw5LiquNm5svt/87fOH4p3iC+N7F5gvyF1weaHOwvSFpxap" +
            "LhIsOpZATIhOOJTwQRAqqBaMJfITdyWOCnnCHcJnIi/RNtGI2ENcKh5O8kgqTXqS7JG8NXkkxTOlLOW5hCepkL" +
            "xMDUzdmzqeFpp2IG0yPTq9MYOSkZBxQqohTZO2Z+pn5mZ2y6xlhbL+xW6Lty8elQfJa7OQrAVZLQq2QqboVFoo" +
            "1yoHsmdlV2a/zYnKOZarnivN7cyzytuQN5zvn//tEsIS4ZK2pYZLVy0dWOa9rGo5sjxxedsK4xUFK4ZWBqw8uI" +
            "q2Km3VT6vtV5eufr0mek1rgV7ByoLBtQFr6wtVCuWFfevc1+1dT1gvWd+1YfqGnRs+FYmKrhTbF5cVf9go3Hjl" +
            "G4dvyr+Z3JS0qavEuWTPZtJm6ebeLZ5bDpaql+aXDm4N2dq0Dd9WtO319kXbL5fNKNu7g7ZDuaO/PLi8ZafJzs" +
            "07P1SkVPRU+lQ27tLdtWHX+G7R7ht7vPY07NXbW7z3/T7JvttVAVVN1WbVZftJ+7P3P66Jqun4lvttXa1ObXHt" +
            "xwPSA/0HIw6217nU1R3SPVRSj9Yr60cOxx++/p3vdy0NNg1VjZzG4iNwRHnk6fcJ3/ceDTradox7rOEH0x92HW" +
            "cdL2pCmvKaRptTmvtbYlu6T8w+0dbq3nr8R9sfD5w0PFl5SvNUyWna6YLTk2fyz4ydlZ19fi753GDborZ752PO" +
            "32oPb++6EHTh0kX/i+c7vDvOXPK4dPKy2+UTV7hXmq86X23qdOo8/pPTT8e7nLuarrlca7nuer21e2b36RueN8" +
            "7d9L158Rb/1tWeOT3dvfN6b/fF9/XfFt1+cif9zsu72Xcn7q28T7xf9EDtQdlD3YfVP1v+3Njv3H9qwHeg89Hc" +
            "R/cGhYPP/pH1jw9DBY+Zj8uGDYbrnjg+OTniP3L96fynQ89kzyaeF/6i/suuFxYvfvjV69fO0ZjRoZfyl5O/bX" +
            "yl/erA6xmv28bCxh6+yXgzMV70VvvtwXfcdx3vo98PT+R8IH8o/2j5sfVT0Kf7kxmTk/8EA5jz/GMzLdsAAAAg" +
            "Y0hSTQAAeiUAAICDAAD5/wAAgOkAAHUwAADqYAAAOpgAABdvkl/FRgAAAGVJREFUeNrs1UEKwCAMRFFHev8rT7" +
            "eCbbREyKJ/loPEhwSUbbfC9FYcAADKAdfU6OGUD/ZLgAeI1wNe+2hOCIii4JIdaBrgjU7ZHfj6AsotofgLAAAA" +
            "AAAAAAAAfg+4AQAA//8DAMcQFz191ZDxAAAAAElFTkSuQmCC");
    }
}
