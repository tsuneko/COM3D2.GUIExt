using System;
using System.Collections.Generic;
using System.Linq;
using System.IO;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityInjector.Attributes;
using PluginExt;

namespace COM3D2.GUIExtBase
{
    [PluginFilter("COM3D2x64"), PluginName("COM3D2.GUIExt.Plugin"), PluginVersion("0.0.0.2")]
    public class GUIExtPlugin : ExPluginBase
    {
        public class PluginConfig
        {
            public int MaxButtonsPerLine = -1;
        }

        public string currentScene = "Global";
        private Dictionary<string, bool> hiddenButtons;

        public PluginConfig config;
        public Dictionary<string, Dictionary<string, bool>> buttonsConfig = new Dictionary<string, Dictionary<string, bool>>();
        private void loadButtonsConfig()
        {
            buttonsConfig["Global"] = new Dictionary<string, bool>();
            if (!File.Exists("UnityInjector\\Config\\GUIExtButtons.ini"))
            {
                GUIExt.OutputConsole("No buttons config found. Creating default buttons config file at UnityInjector/Config/GUIExtButtons.ini");
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
            GUIExt.OutputConsole("Loading config file...");
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
            GUIExt.OutputConsole("Done.");
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
            GUIExt.OutputConsole("Saved Config.");
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

        public GameObject button1;
        public GameObject button2;
        public void Awake()
        {
            try
            {
                GameObject.DontDestroyOnLoad(this);
                SceneManager.sceneLoaded += OnSceneLoaded;
                GUIExt.Initialise();

                config = ReadConfig<PluginConfig>();
                SaveConfig<PluginConfig>(config);
                loadButtonsConfig();

                updateHiddenButtons();
                GUIExt.getUIObjects(hiddenButtons, currentScene, true);
            }
            catch (Exception e)
            {
                GUIExt.OutputErrorConsole(e.ToString());
            }
            GUIExt.OutputConsole("Intialised.");
        }

        public void Update()
        {
            try
            {
                if (GUIExt.getUIObjects(hiddenButtons, currentScene))
                {
                    GUIExt.updateUIObjects(currentScene, config.MaxButtonsPerLine);
                }
            }
            catch (Exception e)
            {
                GUIExt.OutputErrorConsole(e.ToString());
            }
        }

        public void OnEnable()
        {
            GUIExt.SetFrameColor(button1, Color.red);
        }

        public void OnDisable()
        {
            GUIExt.ResetFrameColor(button1);
        }

        private void OnSceneLoaded(Scene scene, LoadSceneMode sceneMode)
        {
            currentScene = scene.name;
            updateHiddenButtons();
            try
            {
                if (GUIExt.getUIObjects(hiddenButtons, currentScene, true))
                {
                    GUIExt.updateUIObjects(currentScene, config.MaxButtonsPerLine);
                }
            }
            catch (Exception e)
            {
                GUIExt.OutputErrorConsole(e.ToString());
            }
        }
    }
}
