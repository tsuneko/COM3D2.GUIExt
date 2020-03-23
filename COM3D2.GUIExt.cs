using System;
using System.Reflection;
using UnityEngine;
using UnityInjector.ConsoleUtil;

namespace COM3D2.GUIExtBase
{
    public static class GUIExt
    { 
        private static SystemShortcut _SysShortcut = GameMain.Instance.SysShortcut;

        public static void WriteLine(string prefix, ConsoleColor prefixColor, string message, ConsoleColor messageColor, bool error = false)
        {
            SafeConsole.ForegroundColor = prefixColor;
            Console.Write(prefix);
            if (error)
            {
                SafeConsole.ForegroundColor = ConsoleColor.Red;
                Console.Write("[ERROR] ");
            }
            SafeConsole.ForegroundColor = messageColor;
            Console.WriteLine(message);
        }

        public static void WriteLine(string message, ConsoleColor messageColor, bool error = false)
        {
            if (error)
            {
                SafeConsole.ForegroundColor = ConsoleColor.Red;
                Console.Write("[ERROR] ");
            }
            SafeConsole.ForegroundColor = messageColor;
            Console.WriteLine(message);
        }

        public static void Write(string prefix, ConsoleColor prefixColor, string message, ConsoleColor messageColor, bool error = false)
        {
            SafeConsole.ForegroundColor = prefixColor;
            Console.Write(prefix);
            if (error)
            {
                SafeConsole.ForegroundColor = ConsoleColor.Red;
                Console.Write("[ERROR] ");
            }
            SafeConsole.ForegroundColor = messageColor;
            Console.Write(message);
        }

        public static void Write(string message, ConsoleColor messageColor, bool error = false)
        {
            if (error)
            {
                SafeConsole.ForegroundColor = ConsoleColor.Red;
                Console.Write("[ERROR] ");
            }
            SafeConsole.ForegroundColor = messageColor;
            Console.Write(message);
        }

        public static GameObject Add(string name, string tooltip, Action<GameObject> action)
        {
            return Add(name, tooltip, DefaultIcon, action);
        }

        public static GameObject Add(string name, string tooltip, byte[] png, Action<GameObject> action)
        {
            GameObject _Base = _SysShortcut.transform.Find("Base").gameObject;
            GameObject _Grid = _Base.transform.Find("Grid").gameObject;
            GameObject button = NGUITools.AddChild(_Grid, UTY.GetChildObject(_Grid, "Config", true));
            button.name = name;
            EventDelegate.Set(button.GetComponent<UIButton>().onClick, () => { action(button); });
            UIEventTrigger trigger = button.GetComponent<UIEventTrigger>();
            EventDelegate.Set(trigger.onHoverOver, () => { VisibleExplanationRaw(tooltip); });
            EventDelegate.Set(trigger.onHoverOut, () => { _SysShortcut.VisibleExplanation(null, false); });
            EventDelegate.Set(trigger.onDragStart, () => { _SysShortcut.VisibleExplanation(null, false); });
            UISprite sprite = button.GetComponent<UISprite>();
            sprite.type = UIBasicSprite.Type.Filled;
            sprite.fillAmount = 0.0f;
            Texture2D texture = new Texture2D(1, 1);
            texture.LoadImage(png);
            UITexture uitexture = NGUITools.AddWidget<UITexture>(button);
            uitexture.material = new Material(uitexture.shader);
            uitexture.material.mainTexture = texture;
            uitexture.MakePixelPerfect();
            return button;
        }

        public static void Destroy(GameObject button)
        {
            if (button != null)
            {
                NGUITools.Destroy(button);
            }
        }

        public static void SetFrameColor(GameObject button, Color color)
        {
            UITexture uitexture = button.GetComponentInChildren<UITexture>();
            if (uitexture == null)
            {
                return;
            }
            Texture2D texture = uitexture.mainTexture as Texture2D;
            if (texture == null)
            {
                return;
            }
            for (int x = 1; x < texture.width - 1; x++)
            {
                texture.SetPixel(x, 0, color);
                texture.SetPixel(x, texture.height - 1, color);
            }
            for (int y = 1; y < texture.height - 1; y++)
            {
                texture.SetPixel(0, y, color);
                texture.SetPixel(texture.width - 1, y, color);
            }
            texture.Apply();
        }

        public static void ResetFrameColor(GameObject button)
        {
            SetFrameColor(button, new Color(1f, 1f, 1f, 0f));
        }

        public static void VisibleExplanationRaw(string text, bool visible = true)
        {
            UILabel _labelExplanation = typeof(SystemShortcut).GetField("m_labelExplanation", BindingFlags.Instance | BindingFlags.NonPublic).GetValue(_SysShortcut) as UILabel;
            _labelExplanation.text = text;
            _labelExplanation.width = 0;
            _labelExplanation.MakePixelPerfect();
            UISprite _spriteExplanation = typeof(SystemShortcut).GetField("m_spriteExplanation", BindingFlags.Instance | BindingFlags.NonPublic).GetValue(_SysShortcut) as UISprite;
            _spriteExplanation.width = _labelExplanation.width + 15;
            _spriteExplanation.gameObject.SetActive(visible);
        }

        public static byte[] DefaultIcon = Convert.FromBase64String(
                "iVBORw0KGgoAAAANSUhEUgAAACAAAAAgCAIAAAD8GO2jAAAAA3NCSVQICAjb4U/g" +
                "AAAACXBIWXMAABYlAAAWJQFJUiTwAAAA/0lEQVRIie2WPYqFMBRGb35QiARM4QZS" +
                "uAX3X7sDkWwgRYSQgJLEKfLGh6+bZywG/JrbnZPLJfChfd/hzuBb6QBA89i2zTln" +
                "jFmWZV1XAPjrZgghAKjrum1bIUTTNFVVvQXOOaXUNE0xxhDC9++llBDS972U8iTQ" +
                "Ws/zPAyDlPJreo5SahxHzrkQAo4baK0B4Dr9gGTgW4Ax5pxfp+dwzjH+JefhvaeU" +
                "lhJQSr33J0GMsRT9A3j7P3gEj+ARPIJHUFBACCnLPYAvAWPsSpn4SAiBMXYSpJSs" +
                "taUE1tqU0knQdR0AKKWu0zMkAwEA5QZnjClevHIvegnuq47o37frH81sg91rI7H3" +
                "AAAAAElFTkSuQmCC");
    }
}
