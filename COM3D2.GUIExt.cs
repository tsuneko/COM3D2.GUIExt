using System;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;
using UnityInjector.ConsoleUtil;

namespace COM3D2.GUIExt
{
    public static class GUIExt
    {
        private static List<string> DefaultUIButtons = new List<string>() { "Config", "Ss", "SsUi", "ToTitle", "Info", "Help", "Dic", "Exit" };
        private static SystemShortcut _SysShortcut;
        private static GameObject _Base;
        private static GameObject _Grid;
        private static UISprite _UIBase;
        private static UIGrid _UIGrid;
        private static List<Transform> children;
        private static List<string> visibleChildren;
        private static int numButtons;

        public static void OutputConsolePrefix(string prefix = "[GUIExt.Plugin] ", ConsoleColor color = ConsoleColor.Cyan)
        {
            SafeConsole.ForegroundColor = color;
            Console.Write(prefix);
            SafeConsole.ForegroundColor = ConsoleColor.White;
        }

        public static void OutputConsole(string msg)
        {
            OutputConsolePrefix();
            Console.WriteLine(msg);
        }

        public static void OutputErrorConsole(string msg)
        {
            OutputConsolePrefix();
            OutputConsolePrefix("[ERROR] ", ConsoleColor.Red);
            Console.WriteLine(msg);
        }

        public static void Initialise()
        {
            if (GameMain.Instance.CMSystem.NetUse)
            {
                DefaultUIButtons.Insert(3, "Shop");
            }
        }

        public static GameObject Add(string name, string tooltip, Action<GameObject> action)
        {
            return Add(name, tooltip, DefaultIcon, action);
        }

        public static GameObject Add(string name, string tooltip, byte[] png, Action<GameObject> action)
        {
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

        public static bool getUIObjects(Dictionary<string, bool> hiddenButtons, string currentScene, bool changedScene = false)
        {
            _SysShortcut = GameMain.Instance.SysShortcut;
            _Base = _SysShortcut.transform.Find("Base").gameObject;
            _Grid = _Base.transform.Find("Grid").gameObject;
            _UIBase = _Base.GetComponent<UISprite>();
            _UIGrid = _Grid.GetComponent<UIGrid>();
            children = _UIGrid.GetChildList();
            if (children.Count != numButtons || changedScene == true)
            {
                numButtons = children.Count;
                visibleChildren = new List<string>();
                for (int i = 0; i < DefaultUIButtons.Count; i++)
                {
                    if (hiddenButtons.ContainsKey(DefaultUIButtons[i]) && hiddenButtons[DefaultUIButtons[i]] == false)
                    {
                        if (changedScene)
                        {
                            OutputConsole("[" + currentScene + "] Removed button: " + DefaultUIButtons[i]);
                        }
                        continue;
                    }
                    visibleChildren.Add(DefaultUIButtons[i]);
                }
                for (int i = 0; i < children.Count; i++)
                {
                    if (DefaultUIButtons.Contains(children[i].name))
                    {
                        continue;
                    }
                    else if (hiddenButtons.ContainsKey(children[i].name) && hiddenButtons[children[i].name] == false)
                    {
                        if (changedScene)
                        {
                            OutputConsole("[" + currentScene + "] Removed button: " + children[i].name);
                        }
                        continue;
                    }
                    visibleChildren.Add(children[i].name);
                }
                return true;
            }
            return false;
        }

        public static void updateUIObjects(string currentScene, int maxButtonsPerLine = -1)
        {
            // Fix positions
            float width = _UIGrid.cellWidth;
            float height = width;
            _UIGrid.pivot = UIWidget.Pivot.TopLeft;
            _UIGrid.arrangement = UIGrid.Arrangement.CellSnap;
            _UIGrid.sorting = UIGrid.Sorting.None;
            _UIGrid.maxPerLine = (int)(Screen.width / (width / UIRoot.GetPixelSizeAdjustment(_Base)) * (3f / 4f));
            if (maxButtonsPerLine > 0)
            {
                _UIGrid.maxPerLine = Math.Min(_UIGrid.maxPerLine, maxButtonsPerLine);
            }
            int maxButtons = _UIGrid.maxPerLine;
            int buttonsX = Math.Min(visibleChildren.Count, maxButtons);
            int buttonsY = Math.Max(1, (visibleChildren.Count - 1) / maxButtons + 1);
            _UIBase.pivot = UIWidget.Pivot.TopRight;
            int baseMarginWidth = (int)(width * 3 / 2 + 8);
            int baseMarginHeight = (int)(height / 2);
            _UIBase.width = (int)(baseMarginWidth + width * buttonsX);
            _UIBase.height = (int)(baseMarginHeight + height * buttonsY + 2f);
            float baseOffsetHeight = baseMarginHeight * 1.5f + 1f;
            _UIBase.transform.localPosition = new Vector3(946f, 502f + baseOffsetHeight, 0f);
            _UIGrid.transform.localPosition = new Vector3(-2f -2 * width, - baseOffsetHeight, 0f);

            int i = 0;
            foreach (Transform child in children)
            {
                child.localPosition = new Vector3(-10000f, -10000f, 0f);

                if (visibleChildren.Contains(child.name))
                {
                    UIEventTrigger _UIEventTrigger = child.GetComponent<UIEventTrigger>();
                    if (_UIEventTrigger != null)
                    {
                        if (_UIEventTrigger.onHoverOver != null)
                        {
                            foreach (EventDelegate _event in _UIEventTrigger.onHoverOver)
                            {
                                _event.Execute();
                            }
                            if (getTooltip() == "")
                            {
                                EventDelegate.Set(_UIEventTrigger.onHoverOver, () => { VisibleExplanationRaw(child.name); });
                                OutputConsole("[" + currentScene + "] Resolved empty tooltip for: " + child.name);
                            }
                            _SysShortcut.VisibleExplanation(null, false);
                        }
                    }
                    child.localPosition = new Vector3((i % maxButtons) * -width, (i / maxButtons) * -height, 0f);
                    i++;
                }
            }

            UISprite _tooltip = typeof(SystemShortcut).GetField("m_spriteExplanation", BindingFlags.Instance | BindingFlags.NonPublic).GetValue(_SysShortcut) as UISprite;
            Vector3 pos = _tooltip.transform.localPosition;
            pos.y = _Base.transform.localPosition.y - _UIBase.height - _tooltip.height;
            _tooltip.transform.localPosition = pos;
        }

        public static string getTooltip()
        {
            UILabel _labelExplanation = typeof(SystemShortcut).GetField("m_labelExplanation", BindingFlags.Instance | BindingFlags.NonPublic).GetValue(_SysShortcut) as UILabel;
            return _labelExplanation.text;
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
