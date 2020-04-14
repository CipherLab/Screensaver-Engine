using System;
using System.Windows.Forms;

namespace FancyTiling
{
    public partial class SettingsForm : Form
    {

        public Settings Settings { get; internal set; }

      
        public SettingsForm()
        {
            InitializeComponent();
            Settings = new Settings();
            LoadSettings();
        }

        /// <summary>
        /// Load display text from the Registry
        /// </summary>
        private void LoadSettings()
        {
            Settings.LoadFromReg();
            lblDirectory.Text = this.Settings.Path;
            chkMirror.Checked = this.Settings.Fancytile;
            chkShuffle.Checked = this.Settings.Shuffle;
            numericUpDown1.Value = this.Settings.Speed;
        }

     

        private void okButton_Click(object sender, EventArgs e)
        {
            this.Settings.SaveSettings();
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
                    Settings.Path = f.SelectedPath;
                    lblDirectory.Text = f.SelectedPath;
                }
            }

        }

        private void numericUpDown1_ValueChanged(object sender, EventArgs e)
        {
            Settings.Speed = (int)((NumericUpDown)sender).Value;
        }

        private void chkShuffle_CheckedChanged(object sender, EventArgs e)
        {
            Settings.Shuffle = ((CheckBox)sender).Checked;
        }

        private void chkMirror_CheckedChanged(object sender, EventArgs e)
        {
            Settings.Fancytile = ((CheckBox)sender).Checked;
        }
    }
}
