﻿using System;
using Microsoft.Win32;

namespace FancyTiling
{
    public class Settings
    {
        private readonly string _softwareFancytilingscreensaver = "SOFTWARE\\FancyTilingScreenSaver";

        public Settings()
        {
            this.Path = "c:\\";
            this.Speed = 15;
            this.Shuffle = true;
            this.Fancytile = true;
        }

        public string Path { get; set; }
        public int Speed { get; set; }
        public bool Shuffle { get; set; }
        public bool Fancytile { get; set; }

        public Settings LoadFromReg()
        {
            RegistryKey key = Registry.CurrentUser.OpenSubKey(_softwareFancytilingscreensaver);
            if (key == null)
            {
                this.Path = "c:\\";
                this.Speed = 15;
                this.Shuffle = true;
                this.Fancytile = true;
                SaveSettings();
                return this;
            }

            this.Path = (string)key.GetValue(nameof(Settings.Path));
            this.Speed = (int)key.GetValue(nameof(Settings.Speed));
            this.Shuffle = Convert.ToBoolean(key.GetValue(nameof(Settings.Shuffle)));
            this.Fancytile = Convert.ToBoolean(key.GetValue(nameof(Settings.Fancytile)));
            return this;
        }
        /// <summary>
        /// Save text into the Registry.
        /// </summary>
        public void SaveSettings()
        {
            // Create or get existing subkey
            RegistryKey key = Registry.CurrentUser.CreateSubKey(_softwareFancytilingscreensaver);
            key.SetValue(nameof(this.Path), this.Path);
            key.SetValue(nameof(this.Speed), this.Speed);
            key.SetValue(nameof(this.Shuffle), this.Shuffle);
            key.SetValue(nameof(this.Fancytile), this.Fancytile);

        }
    }
}