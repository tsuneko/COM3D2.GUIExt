using System;
using System.Collections.Generic;
using System.Linq;
using System.IO;
using System.Reflection;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityInjector.Attributes;
using PluginExt;

namespace COM3D2.GUIExtBase
{
    [PluginFilter("COM3D2x64"), PluginName("COM3D2.GUIExt.Plugin"), PluginVersion("0.0.0.5")]
    public class GUIExtPlugin : ExPluginBase
    {
        public PluginConfig config;

        private string currentScene = "Global";
        private Dictionary<string, bool> hiddenButtons;
        private Dictionary<string, Dictionary<string, bool>> buttonsConfig = new Dictionary<string, Dictionary<string, bool>>();
        private List<string> DefaultUIButtons = new List<string>() { "Config", "Ss", "SsUi", "ToTitle", "Info", "Help", "Dic", "Exit" };

        private SystemShortcut _SysShortcut = GameMain.Instance.SysShortcut;
        private GameObject _Base;
        private GameObject _Grid;
        private UIGrid _UIGrid;
        private List<Transform> children;
        private List<string> visibleChildren;
        private int numButtons;

        public class PluginConfig
        {
            public int MaxButtonsPerLine = -1;
        }

        private void loadButtonsConfig()
        {
            buttonsConfig["Global"] = new Dictionary<string, bool>();
            if (!File.Exists("UnityInjector\\Config\\GUIExtButtons.ini"))
            {
                WriteLine("No buttons config found. Creating default buttons config file at UnityInjector/Config/GUIExtButtons.ini", true);
                buttonsConfig["Global"]["ToTitle"] = true;
                buttonsConfig["Global"]["Shop"] = false;
                buttonsConfig["SceneTitle"] = new Dictionary<string, bool>();
                buttonsConfig["SceneTitle"]["ToTitle"] = false;
                buttonsConfig["SceneTitle"]["Help"] = false;
                buttonsConfig["SceneTitle"]["Dic"] = false;
                buttonsConfig["SceneTitle"]["Info"] = false;
                buttonsConfig["SceneEdit"] = new Dictionary<string, bool>();
                saveButtonsConfig();
            }
            WriteLine("Loading config file...");
            string section = "Global";
            string line;
            StreamReader file = new StreamReader("UnityInjector\\Config\\GUIExtButtons.ini");
            while ((line = file.ReadLine()) != null)
            {
                line = line.Trim();
                if (line.StartsWith("[") && line.EndsWith("]"))
                {
                    section = line.Substring(1, line.Length - 2);
                }
                else if (line.Contains('='))
                {
                    string[] tokens = line.Split('=');
                    string k = tokens[0].Trim();
                    string v = tokens[1].Trim().ToLower();
                    if (k.Length > 0 && (v == "true" || v == "false"))
                    {
                        if (!buttonsConfig.ContainsKey(section))
                        {
                            buttonsConfig[section] = new Dictionary<string, bool>();
                        }
                        if (v == "true")
                        {
                            buttonsConfig[section][k] = true;
                        }
                        else
                        {
                            buttonsConfig[section][k] = false;
                        }
                    }
                }
            }
            WriteLine("Done.");
        }

        private void saveButtonsConfig()
        {
            using (StreamWriter file = new StreamWriter("UnityInjector\\Config\\GUIExtButtons.ini", true))
            {
                foreach (string section in buttonsConfig.Keys)
                {
                    file.WriteLine("[" + section + "]");
                    foreach (string k in buttonsConfig[section].Keys)
                    {
                        file.WriteLine(k + "=" + buttonsConfig[section][k].ToString());
                    }
                }
            }
            WriteLine("Saved Config.");
        }

        public string getTooltip()
        {
            UILabel _labelExplanation = typeof(SystemShortcut).GetField("m_labelExplanation", BindingFlags.Instance | BindingFlags.NonPublic).GetValue(_SysShortcut) as UILabel;
            return _labelExplanation.text;
        }

        public bool getUIObjects(Dictionary<string, bool> hiddenButtons, string currentScene, bool changedScene = false)
        {
            _Base = _SysShortcut.transform.Find("Base").gameObject;
            _Grid = _Base.transform.Find("Grid").gameObject;
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
                            WriteLine("[" + currentScene + "] Removed button: " + DefaultUIButtons[i]);
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
                            WriteLine("[" + currentScene + "] Removed button: " + children[i].name);
                        }
                        continue;
                    }
                    visibleChildren.Add(children[i].name);
                }
                return true;
            }
            return false;
        }

        public void fixTooltips()
        {
            foreach (Transform child in children)
            {
                if (!DefaultUIButtons.Contains(child.name))
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
                                EventDelegate.Set(_UIEventTrigger.onHoverOver, () => { GUIExt.VisibleExplanationRaw(child.name, _SysShortcut); });
                                WriteLine("[" + currentScene + "] Resolved empty tooltip for: " + child.name);
                            }
                            _SysShortcut.VisibleExplanation(null, false);
                        }
                    }
                }
            }
        }

        private void updateHiddenButtons()
        {
            hiddenButtons = new Dictionary<string, bool>();
            foreach (string k in buttonsConfig["Global"].Keys)
            {
                hiddenButtons[k] = buttonsConfig["Global"][k];
            }
            if (buttonsConfig.ContainsKey(currentScene))
            {
                foreach (string k in buttonsConfig[currentScene].Keys)
                {
                    hiddenButtons[k] = buttonsConfig[currentScene][k];
                }
            }
        }

        public void Awake()
        {
            try
            {
                GameObject.DontDestroyOnLoad(this);
                SceneManager.sceneLoaded += OnSceneLoaded;
                if (GameMain.Instance.CMSystem.NetUse)
                {
                    DefaultUIButtons.Insert(3, "Shop");
                }
                config = ReadConfig<PluginConfig>();
                SaveConfig<PluginConfig>(config);
                loadButtonsConfig();
            }
            catch (Exception e)
            {
                WriteLine(e.ToString(), true);
            }
            WriteLine("Intialised.");
        }

        public void Update()
        {
            try
            {
                if (getUIObjects(hiddenButtons, currentScene))
                {
                    GUIExt.repositionButtons(config.MaxButtonsPerLine);
                    fixTooltips();
                }
            }
            catch (Exception e)
            {
                WriteLine(e.ToString(), true);
            }
        }

        private void OnSceneLoaded(Scene scene, LoadSceneMode sceneMode)
        {
            currentScene = scene.name;
            updateHiddenButtons();
            try
            {
                if (getUIObjects(hiddenButtons, currentScene, true))
                {
                    GUIExt.repositionButtons(config.MaxButtonsPerLine);
                    fixTooltips();
                }
            }
            catch (Exception e)
            {
                WriteLine(e.ToString(), true);
            }
        }

        private void WriteLine(string message, bool error = false)
        {
            GUIExt.WriteLine("[GUIExt] ", ConsoleColor.Cyan, message, ConsoleColor.White, error);
        }
    }
}
