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
    [PluginFilter("COM3D2x64"), PluginName("COM3D2.GUIExt.Plugin"), PluginVersion("0.0.0.6")]
    public class GUIExtPlugin : ExPluginBase
    {
        public PluginConfig config;

        private string currentScene = "Global";
        private List<Transform> children = GameMain.Instance.SysShortcut.transform.Find("Base").gameObject.transform.Find("Grid").gameObject.GetComponent<UIGrid>().GetChildList();
        private Dictionary<string, bool> hiddenButtonsDic;
        private List<string> hiddenButtonsList;
        private Dictionary<string, Dictionary<string, bool>> buttonsConfig = new Dictionary<string, Dictionary<string, bool>>();
        private List<string> DefaultUIButtons = new List<string>() { "Config", "Ss", "SsUi", "ToTitle", "Info", "Help", "Dic", "Exit" };

        private int numButtons;
        
        public class PluginConfig
        {
			public bool PluginEnabled = true;
            public bool RemoveButtons = true;
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
            UILabel _labelExplanation = typeof(SystemShortcut).GetField("m_labelExplanation", BindingFlags.Instance | BindingFlags.NonPublic).GetValue(GameMain.Instance.SysShortcut) as UILabel;
            return _labelExplanation.text;
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
                                EventDelegate.Set(_UIEventTrigger.onHoverOver, () => { GUIExt.VisibleExplanationRaw(child.name, GameMain.Instance.SysShortcut); });
                                WriteLine("[" + currentScene + "] Resolved empty tooltip for: " + child.name);
                            }
                            GameMain.Instance.SysShortcut.VisibleExplanation(null, false);
                        }
                    }
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
                if (!config.PluginEnabled)
                {
                    enabled = false;
                }
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
                children = GameMain.Instance.SysShortcut.transform.Find("Base").gameObject.transform.Find("Grid").gameObject.GetComponent<UIGrid>().GetChildList();
                if (children.Count != numButtons)
                {
                    numButtons = children.Count;
                    GUIExt.repositionButtons(config.MaxButtonsPerLine, hiddenButtonsList);
                    fixTooltips();
                }
            }
            catch (Exception e)
            {
                WriteLine(e.ToString(), true);
            }
        }

        private void UpdateHiddenButtons()
        {
            hiddenButtonsDic = new Dictionary<string, bool>();
            foreach (string k in buttonsConfig["Global"].Keys)
            {
                hiddenButtonsDic[k] = buttonsConfig["Global"][k];
            }
            if (buttonsConfig.ContainsKey(currentScene))
            {
                foreach (string k in buttonsConfig[currentScene].Keys)
                {
                    hiddenButtonsDic[k] = buttonsConfig[currentScene][k];
                }
            }
            hiddenButtonsList = new List<string>();
            foreach (string k in hiddenButtonsDic.Keys)
            {
                if (hiddenButtonsDic[k] == false)
                {
                    hiddenButtonsList.Add(k);
                }
            }
        }

        private void OnSceneLoaded(Scene scene, LoadSceneMode sceneMode)
        {
            currentScene = scene.name;
            UpdateHiddenButtons();
            try
            {
                GUIExt.repositionButtons(config.MaxButtonsPerLine, config.RemoveButtons ? hiddenButtonsList : null);
                fixTooltips();
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
