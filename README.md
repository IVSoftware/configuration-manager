Looking at your code, one thing that could explain the behavior is if events like TextBox.TextChanged are being subscribed to before InitializeComponent sets the _initial_ values, because then those initial (blank) values could be being saved into the configuration. I think your idea of writing a Json file to your AppSettings folder is a good one. And just to be on the safe side, try doing things in this order:

```
public partial class MainForm : Form
{
    string configPath { get; } =
        Path.Combine(
            Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData),
            Assembly.GetEntryAssembly().GetName().Name);
    public MainForm()
    {
        InitializeComponent();
        // Restore values BEFORE subscribing to events
        LoadConfiguration();
        // NOW you can safely subscribe.
        SubscribeToEvents();
    }
    Dictionary<string, object> ConfigurationHelper { get; set; }
    .
    .
    .
}
```

**Loading the configuration from the Json file**

```
    private void LoadConfiguration()
    {
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
                    if (ConfigurationHelper.TryGetValue(key, out var value))
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
```

**Subscribe to change events**

Make sure that you go through the Designer.cs file and remove the lines that subscribe to control change events like `TextChanged` or `SelectionIndexChanged`. It's likely that these are the cause of the problem. After the configuration is already loaded, subscribe to the events this way instead:

```
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
```

**Save individual control changes**


When changes occur, just serialize the dictionary. As written, it's not terribly efficient. Every keystroke in any textbox writes the entire file. So optimize by waiting for changes to settle and maybe writing the congif file asynchronously or even just doing it when the app closes.

```

    private void SaveConfiguration(Control control, object value)
    {
        ConfigurationHelper[control.Name] = value.ToString();
        // Save every change OR: - Save on app close? - Save after time delay (consolidate rapid changes)?
        File.WriteAllText(configPath, JsonConvert.SerializeObject(ConfigurationHelper));
    }
```

