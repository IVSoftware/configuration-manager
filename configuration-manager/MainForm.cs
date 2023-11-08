using Newtonsoft.Json;
using System.Reflection;

namespace configuration_manager
{
    public partial class MainForm : Form
    {
        string configPath { get; } =
            Path.Combine(
                Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData),
                Assembly.GetEntryAssembly().GetName().Name);
        public MainForm()
        {
            InitializeComponent();
            LoadConfiguration();
            SubscribeToEvents();
        }
        private void LoadConfiguration()
        {   
            // Restore values BEFORE subscribing to events
            if (File.Exists(configPath))
            {
                ConfigurationHelper =
                    JsonConvert
                    .DeserializeObject<Dictionary<string, object>>(File.ReadAllText(configPath));
            }
            else
            {
                Directory.CreateDirectory(Path.GetDirectoryName(configPath));
                ConfigurationHelper = new Dictionary<string, object>();
            }
            foreach (var key in ConfigurationHelper.Keys)
            {
                if (Controls[key] is Control control)
                {
                    if (control is TextBox textBox)
                    {
                        if(ConfigurationHelper.TryGetValue(key, out var value))
                        {
                            textBox.Text = value.ToString();
                        }
                    }
                    else if (control is NumericUpDown numeric)
                    {
                        if (ConfigurationHelper.TryGetValue(key, out var o) && int.TryParse($"{o}", out int value))
                        {
                            numeric.Value = value;
                        }
                    }
                    else if (control is ComboBox comboBox)
                    {
                        if (ConfigurationHelper.TryGetValue(key, out var o) && int.TryParse($"{o}", out int value))
                        {
                            comboBox.SelectedIndex = value;
                        }
                    }
                    else if (control is TrackBar trackBar)
                    {
                        if (ConfigurationHelper.TryGetValue(key, out var o) && int.TryParse($"{o}", out int value))
                        {
                            trackBar.Value = value;
                        }
                    }
                    else if (control is CheckBox checkBox)
                    {
                        if (ConfigurationHelper.TryGetValue(key, out var o) && bool.TryParse($"{o}", out bool value))
                        {
                            checkBox.Checked = value;
                        }
                    }
                }
            }
        }
        private void SubscribeToEvents()
        {
            foreach (var control in IterateControls(Controls))
            {
                if (control is TextBox textBox)
                {
                    textBox.TextChanged += (sender, e) => SaveConfiguration(textBox, textBox.Text);
                }
                else if (control is NumericUpDown numeric)
                {
                    numeric.ValueChanged += (sender, e) => SaveConfiguration(numeric, numeric.Value);
                }
                else if (control is ComboBox comboBox)
                {
                    comboBox.SelectionChangeCommitted += (sender, e) => SaveConfiguration(comboBox, comboBox.SelectedIndex);
                }
                else if (control is TrackBar trackBar)
                {
                    trackBar.ValueChanged += (sender, e) => SaveConfiguration(trackBar, trackBar.Value);
                }
                else if (control is CheckBox checkBox)
                {
                    checkBox.CheckedChanged += (sender, e) => SaveConfiguration(checkBox, checkBox.Checked);
                }
            }
        }
        private void SaveConfiguration(Control control, object value)
        {
            ConfigurationHelper[control.Name] = value.ToString();
            // Save every change OR: - Save on app close? - Save after time delay (consolidate rapid changes)?
            File.WriteAllText(configPath, JsonConvert.SerializeObject(ConfigurationHelper));
        }
        Dictionary<string, object> ConfigurationHelper { get; set; }
        IEnumerable<Control> IterateControls(Control.ControlCollection controls)
        {
            foreach (Control control in controls)
            {
                yield return control;
                foreach (Control child in IterateControls(control.Controls))
                {
                    yield return child;
                }
            }
        }
    }
}