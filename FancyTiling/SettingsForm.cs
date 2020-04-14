using System;
using System.Windows.Forms;
using Microsoft.Win32;

namespace FancyTiling
{
    public partial class SettingsForm : Form
    {
        public SettingsForm()
        {
            InitializeComponent();
            LoadSettings();
        }

        /// <summary>
        /// Load display text from the Registry
        /// </summary>
        private void LoadSettings()
        {
            RegistryKey key = Registry.CurrentUser.OpenSubKey("SOFTWARE\\MirrorScalingScreenSaver\\text");
            //if (key == null)
            //    textBox.Text = "C# Screen Saver";
            //else
            //    textBox.Text = (string)key.GetValue("text");
        }

        /// <summary>
        /// Save text into the Registry.
        /// </summary>
        private void SaveSettings()
        {
            // Create or get existing subkey
            RegistryKey key = Registry.CurrentUser.CreateSubKey("SOFTWARE\\MirrorScalingScreenSaver\\text");

            //key.SetValue("text", textBox.Text);
            //key.SetValue("text", textBox.Text);
            //key.SetValue("text", textBox.Text);
            //key.SetValue("text", textBox.Text);
        }

        private void okButton_Click(object sender, EventArgs e)
        {
            SaveSettings();
            Close();
        }

        private void cancelButton_Click(object sender, EventArgs e)
        {
            Close();
        }

        private void btnBrowse_Click(object sender, EventArgs e)
        {
            using (var f = new FolderBrowserDialog())
            {
                f.ShowDialog(this);
                if (!string.IsNullOrEmpty(f.SelectedPath))
                {

                }
            }

        }
    }
}
