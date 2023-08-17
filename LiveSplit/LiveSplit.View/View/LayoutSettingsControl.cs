using System;
using System.Drawing;
using System.Windows.Forms;
using LiveSplit.UI;
using LiveSplit.Options;
using System.Linq;
using System.Text.RegularExpressions;
using Microsoft.WindowsAPICodePack.Dialogs;

namespace LiveSplit.View
{
    public partial class LayoutSettingsControl : UserControl
    {
        public LayoutSettingsControl()
        {
            InitializeComponent();
        }

        public Options.LayoutSettings Settings { get; set; }
        public ILayout Layout { get; set; }

        private Image originalBackgroundImage { get; set; }

        private string dynamicBackgroundImagesFolder { get; set; }
        protected bool IsInDialogMode = false;
        public string TimerFont { get { return SettingsHelper.FormatFont(Settings.TimerFont); } }
        public string MainFont { get { return SettingsHelper.FormatFont(Settings.TimesFont); } }
        public string SplitNamesFont { get { return SettingsHelper.FormatFont(Settings.TextFont); } }

        public float Opacity { get { return Settings.Opacity * 100f; } set { Settings.Opacity = value / 100f; } }
        public float ImageOpacity { get { return Settings.ImageOpacity * 100f; } set { Settings.ImageOpacity = value / 100f; } }
        public float ImageBlur { get { return Settings.ImageBlur * 100f; } set { Settings.ImageBlur = value / 100f; } }

        public LayoutSettingsControl(Options.LayoutSettings settings, ILayout layout)
        {
            InitializeComponent();
            Settings = settings;
            Layout = layout;
            chkBestSegments.DataBindings.Add("Checked", Settings, "ShowBestSegments", false, DataSourceUpdateMode.OnPropertyChanged);
            chkAlwaysOnTop.DataBindings.Add("Checked", Settings, "AlwaysOnTop", false, DataSourceUpdateMode.OnPropertyChanged);
            chkAntiAliasing.DataBindings.Add("Checked", Settings, "AntiAliasing", false, DataSourceUpdateMode.OnPropertyChanged);
            chkDropShadows.DataBindings.Add("Checked", Settings, "DropShadows", false, DataSourceUpdateMode.OnPropertyChanged);
            chkRainbow.DataBindings.Add("Checked", Settings, "UseRainbowColor", false, DataSourceUpdateMode.OnPropertyChanged);
            dynamicBackgroundCheckBox.DataBindings.Add("Checked", Settings, "DynamicBackground", false, DataSourceUpdateMode.OnPropertyChanged);
            dynamicBackgroundTimeCheckBox.DataBindings.Add("Checked", Settings, "DynamicBackgroundTime", false, DataSourceUpdateMode.OnPropertyChanged);
            dynamicBackgroundKeyCheckBox.DataBindings.Add("Checked", Settings, "DynamicBackgroundKey", false, DataSourceUpdateMode.OnPropertyChanged);
            dynamicBackgroundSplitCheckBox.DataBindings.Add("Checked", Settings, "DynamicBackgroundSplit", false, DataSourceUpdateMode.OnPropertyChanged);
            dynamicBackgroundChangeRandomRadioButton.DataBindings.Add("Checked", Settings, "DynamicBackgroundRandom", false, DataSourceUpdateMode.OnPropertyChanged);
            dynamicBackgroundChangeSequenceRadioButton.DataBindings.Add("Checked", Settings, "DynamicBackgroundSequence", false, DataSourceUpdateMode.OnPropertyChanged);
            timeBetweenBackgroundChangeTextBox.DataBindings.Add("Text", Settings, "TimeBetweenBackgroundChange", false, DataSourceUpdateMode.OnPropertyChanged);
            unitForTimeBackgroundChangeComboBox.DataBindings.Add("SelectedItem", Settings, "UnitForTimeBetweenBackgroundChange", false, DataSourceUpdateMode.OnPropertyChanged);
            btnTextColor.DataBindings.Add("BackColor", Settings, "TextColor", false, DataSourceUpdateMode.OnPropertyChanged);
            btnBackground.DataBindings.Add("BackColor", Settings, "BackgroundColor", false, DataSourceUpdateMode.OnPropertyChanged);
            btnBackground2.DataBindings.Add("BackColor", Settings, "BackgroundColor2", false, DataSourceUpdateMode.OnPropertyChanged);
            btnThinSep.DataBindings.Add("BackColor", Settings, "ThinSeparatorsColor", false, DataSourceUpdateMode.OnPropertyChanged);
            btnSeparators.DataBindings.Add("BackColor", Settings, "SeparatorsColor", false, DataSourceUpdateMode.OnPropertyChanged);
            btnPB.DataBindings.Add("BackColor", Settings, "PersonalBestColor", false, DataSourceUpdateMode.OnPropertyChanged);
            btnGlod.DataBindings.Add("BackColor", Settings, "BestSegmentColor", false, DataSourceUpdateMode.OnPropertyChanged);
            btnAheadGaining.DataBindings.Add("BackColor", Settings, "AheadGainingTimeColor", false, DataSourceUpdateMode.OnPropertyChanged);
            btnAheadLosing.DataBindings.Add("BackColor", Settings, "AheadLosingTimeColor", false, DataSourceUpdateMode.OnPropertyChanged);
            btnBehindGaining.DataBindings.Add("BackColor", Settings, "BehindGainingTimeColor", false, DataSourceUpdateMode.OnPropertyChanged);
            btnBehindLosing.DataBindings.Add("BackColor", Settings, "BehindLosingTimeColor", false, DataSourceUpdateMode.OnPropertyChanged);
            btnNotRunning.DataBindings.Add("BackColor", Settings, "NotRunningColor", false, DataSourceUpdateMode.OnPropertyChanged);
            btnPausedColor.DataBindings.Add("BackColor", Settings, "PausedColor", false, DataSourceUpdateMode.OnPropertyChanged);
            btnTextOutlineColor.DataBindings.Add("BackColor", Settings, "TextOutlineColor", false, DataSourceUpdateMode.OnPropertyChanged);
            btnShadowsColor.DataBindings.Add("BackColor", Settings, "ShadowsColor", false, DataSourceUpdateMode.OnPropertyChanged);
            lblTimer.DataBindings.Add("Text", this, "TimerFont", false, DataSourceUpdateMode.OnPropertyChanged);
            lblText.DataBindings.Add("Text", this, "SplitNamesFont", false, DataSourceUpdateMode.OnPropertyChanged);
            lblTimes.DataBindings.Add("Text", this, "MainFont", false, DataSourceUpdateMode.OnPropertyChanged);
            trkOpacity.DataBindings.Add("Value", this, "Opacity", false, DataSourceUpdateMode.OnPropertyChanged);
            chkMousePassThroughWhileRunning.DataBindings.Add("Checked", Settings, "MousePassThroughWhileRunning", false, DataSourceUpdateMode.OnPropertyChanged);
            trkImageOpacity.DataBindings.Add("Value", this, "ImageOpacity", false, DataSourceUpdateMode.OnPropertyChanged);
            trkBlur.DataBindings.Add("Value", this, "ImageBlur", false, DataSourceUpdateMode.OnPropertyChanged);

            cmbBackgroundType.SelectedItem = GetBackgroundTypeString(Settings.BackgroundType);
            originalBackgroundImage = Settings.BackgroundImage;
            dynamicBackgroundImagesFolder = Settings.BackgroundImageFolder;
        }        

        private string GetBackgroundTypeString(BackgroundType type)
        {
            switch (type)
            {
                case BackgroundType.HorizontalGradient:
                    return "Horizontal Gradient";
                case BackgroundType.VerticalGradient:
                    return "Vertical Gradient";
                case BackgroundType.Image:
                    return "Image";
                default:
                    return "Solid Color";
            }
        }

        void cmbGradientType_SelectedIndexChanged(object sender, EventArgs e)
        {
            var selectedItem = cmbBackgroundType.SelectedItem.ToString();
            btnBackground.Visible = selectedItem != "Solid Color" && selectedItem != "Image";
            btnBackground2.DataBindings.Clear();
            lblImageOpacity.Enabled = lblBlur.Enabled = trkImageOpacity.Enabled = trkBlur.Enabled = selectedItem == "Image";
            if (selectedItem == "Image")
            {
                btnBackground2.BackgroundImage = Settings.BackgroundImage;
                btnBackground2.BackColor = Color.Transparent;
                lblBackground.Text = "Image:";
            }
            else
            {
                btnBackground2.BackgroundImage = null;
                btnBackground2.DataBindings.Add("BackColor", Settings, btnBackground.Visible ? "BackgroundColor2" : "BackgroundColor", false, DataSourceUpdateMode.OnPropertyChanged);
                lblBackground.Text = "Color:";
            }
            Settings.BackgroundType = (BackgroundType)Enum.Parse(typeof(BackgroundType), selectedItem.Replace(" ", ""));
        }

        private void ColorButtonClick(object sender, EventArgs e)
        {
            SettingsHelper.ColorButtonClick((Button)sender, this);
        }

        private void BackgroundColorButtonClick(object sender, EventArgs e)
        {
            if (cmbBackgroundType.SelectedItem.ToString() == "Image")
            {
                var dialog = new OpenFileDialog();
                dialog.Filter = "Image Files|*.BMP;*.JPG;*.GIF;*.JPEG;*.PNG|All files (*.*)|*.*";
                dialog.Title = "Set Background Image...";
                var result = dialog.ShowDialog();
                if (result == DialogResult.OK)
                {
                    try
                    {
                        var image = Image.FromFile(dialog.FileName);
                        if (Settings.BackgroundImage != null && Settings.BackgroundImage != originalBackgroundImage)
                            Settings.BackgroundImage.Dispose();

                        Settings.BackgroundImage = ((Button)sender).BackgroundImage = image;
                    }
                    catch (Exception ex)
                    {
                        Log.Error(ex);
                        MessageBox.Show("Could not load image!", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                }
            }
            else
            {
                SettingsHelper.ColorButtonClick((Button)sender, this);
            }
        }

        private void btnTimer_Click(object sender, EventArgs e)
        {
            var timerFont = new Font(Settings.TimerFont.FontFamily.Name, (Settings.TimerFont.Size / 50f) * 18f, Settings.TimerFont.Style, GraphicsUnit.Point);
            var dialog = SettingsHelper.GetFontDialog(timerFont, 7, 20);
            dialog.FontChanged += (s, ev) => updateTimerFont(((CustomFontDialog.FontChangedEventArgs)ev).NewFont);
            dialog.ShowDialog(this);
            lblTimer.Text = TimerFont;
        }

        private void updateTimerFont(Font timerFont)
        {
            Settings.TimerFont = new Font(timerFont.FontFamily.Name, (timerFont.Size / 18f) * 50f, timerFont.Style, GraphicsUnit.Pixel);
        }

        private void btnTimes_Click(object sender, EventArgs e)
        {
            var dialog = SettingsHelper.GetFontDialog(Settings.TimesFont, 7, 20);
            dialog.FontChanged += (s, ev) => Settings.TimesFont = ((CustomFontDialog.FontChangedEventArgs)ev).NewFont;
            dialog.ShowDialog(this);
            lblTimes.Text = MainFont;
        }

        private void btnTextFont_Click(object sender, EventArgs e)
        {
            var dialog = SettingsHelper.GetFontDialog(Settings.TextFont, 7, 20);
            dialog.FontChanged += (s, ev) => Settings.TextFont = ((CustomFontDialog.FontChangedEventArgs)ev).NewFont;
            dialog.ShowDialog(this);
            lblText.Text = SplitNamesFont;
        }

        private void chkRainbow_CheckedChanged(object sender, EventArgs e)
        {
            label9.Enabled = btnGlod.Enabled = !chkRainbow.Checked;
        }

        private void chkAntiAliasing_CheckedChanged(object sender, EventArgs e)
        {
            lblOutlines.Enabled = btnTextOutlineColor.Enabled = chkAntiAliasing.Checked;
        }

        private void LayoutSettingsControl_Load(object sender, EventArgs e)
        {
            chkRainbow_CheckedChanged(null, null);
            chkAntiAliasing_CheckedChanged(null, null);
            dynamicBackgroundCheckBox_CheckedChanged(null, null);
        }

        private void buttonBackgroundFolder_Click(object sender, EventArgs e)
        {
            if (CommonFileDialog.IsPlatformSupported)
            {
                using (CommonOpenFileDialog dialog = new CommonOpenFileDialog())
                {
                    dialog.IsFolderPicker = true;
                    dialog.Multiselect = false;
                    var result = dialog.ShowDialog(this.Handle);
                    if (result == CommonFileDialogResult.Ok)
                    {
                        dynamicBackgroundImagesFolder = dialog.FileName;
                        Settings.BackgroundImageFolder = dynamicBackgroundImagesFolder;
                        TimerForm timerForm;
                        try
                        {
                            timerForm = Application.OpenForms.OfType<TimerForm>().First();
                            timerForm.RerollIndexForNextBackgroundImage();
                        }
                        catch (Exception e2)
                        {
                            Log.Error(e2);
                        }
                    }
                }
            }
            else
            {
                using (FolderBrowserDialog dialog = new FolderBrowserDialog())
                {
                    dialog.RootFolder = Environment.SpecialFolder.MyMusic;
                    if (dialog.ShowDialog() != DialogResult.Cancel)
                    {
                        dynamicBackgroundImagesFolder = dialog.SelectedPath;
                        Settings.BackgroundImageFolder = dynamicBackgroundImagesFolder;
                    }
                }
            }
        }

        private void dynamicBackgroundCheckBox_CheckedChanged(object sender, EventArgs e)
        {
            if(dynamicBackgroundCheckBox.Checked == false)
            {
                dynamicBackgroundChangeRandomRadioButton.Enabled = false;
                dynamicBackgroundChangeSequenceRadioButton.Enabled = false;
                buttonBackgroundFolder.Enabled = false;
                dynamicBackgroundTimeCheckBox.Enabled = false;
                dynamicBackgroundKeyCheckBox.Enabled = false;
                dynamicBackgroundSplitCheckBox.Enabled = false;
                timeBetweenBackgroundChangeTextBox.Enabled = false;
                unitForTimeBackgroundChangeComboBox.Enabled = false;
                cmbBackgroundType.Enabled = true;
                btnChangeBackground.Enabled = false;
                btnBackground2.Enabled = true;
            }
            else
            {
                dynamicBackgroundChangeRandomRadioButton.Enabled = true;
                dynamicBackgroundChangeSequenceRadioButton.Enabled = true;
                buttonBackgroundFolder.Enabled = true;
                dynamicBackgroundTimeCheckBox.Enabled = true;
                dynamicBackgroundKeyCheckBox.Enabled = true;
                dynamicBackgroundSplitCheckBox.Enabled = true;
                timeBetweenBackgroundChangeTextBox.Enabled = true;
                unitForTimeBackgroundChangeComboBox.Enabled = true;
                cmbBackgroundType.SelectedItem = cmbBackgroundType.Text = "Image";
                cmbBackgroundType.Enabled = false;
                btnChangeBackground.Enabled = true;
                btnBackground2.Enabled = false;
            }
        }

        private void btnChangeBackground_Click(object sender, EventArgs e)
        {
            if (dynamicBackgroundImagesFolder != null)
            {
                TimerForm timerForm;
                try
                {
                    timerForm = Application.OpenForms.OfType<TimerForm>().First();
                    timerForm.changeBackgroundImage();
                }
                catch (Exception e2)
                {
                    Log.Error(e2);
                }
            }
        }

        private void dynamicBackgroundChangeRandomRadioButton_CheckedChanged(object sender, EventArgs e)
        {
            dynamicBackgroundChangeRandomRadioButton.Checked = !dynamicBackgroundChangeSequenceRadioButton.Checked;
        }

        private void dynamicBackgroundChangeSequenceRadioButton_CheckedChanged(object sender, EventArgs e)
        {
            dynamicBackgroundChangeSequenceRadioButton.Checked = !dynamicBackgroundChangeRandomRadioButton.Checked;
        }

        private void timeBetweenBackgroundChangeTextBox_KeyPress(object sender, KeyPressEventArgs e)
        {
            e.Handled = !char.IsDigit(e.KeyChar);
        }
    }
}
