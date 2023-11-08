Looking at your code, one thing that could explain the behavior is if, in `InitializeComponent`, events like `TextBox.TextChanged` are being subscribed to before that control's _initial_ value is set, because then those initial (blank) values could be being saved into the configuration and then when you load the configuration you get nothing. I think your idea of writing a Json file (to the user's AppData folder) is a good one. And just to be on the safe side, try doing things in this order:

```
public partial class MainForm : Form
{
    string configPath { get; } =
        Path.Combine(
            Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData),
            Assembly.GetEntryAssembly().GetName().Name,
            "config.json"
    );
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
___

**Loading the configuration from the Json file**

```
private void LoadConfiguration()
{
    if (File.Exists(configPath))
    {
        ConfigurationHelper =
            JsonConvert
            .DeserializeObject<Dictionary<string, object>>(File.ReadAllText(configPath));
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
    else
    {
        Directory.CreateDirectory(Path.GetDirectoryName(configPath));
        ConfigurationHelper = new Dictionary<string, object>();
    }
}
```


**Subscribe to change events**

Make sure that you go through the Designer.cs file. Either remove the existing handlers for control change events like `TextChanged` or `SelectionIndexChanged` entirely, _or_ at least go through those handlers making sure they don't do any writing to the configuration file. There's a real possibility that these can cause a race condition that could contribute to the problem you're seeing. *To be clear, it's perfectly ok to subscribe to an event multiple times to multiple handlers. If you choose to leave the existing handlers in place, don't let them write to your `SaveConfiguration` method.*
___
After the configuration is already loaded, subscribe to the events this way instead:

```
    private void SubscribeToEvents()
    {
        foreach (var control in IterateControls(Controls))
        {
            if (control is TextBox textBox)
            {
                // You're still free to subscribe to these events
                // in other places for different purposes.
                textBox.TextChanged += (sender, e) => 
                    SaveConfiguration(textBox, textBox.Text);
            }
            else if (control is NumericUpDown numeric)
            {
                numeric.ValueChanged += (sender, e) => 
                    SaveConfiguration(numeric, numeric.Value);
            }
            else if (control is ComboBox comboBox)
            {
                comboBox.SelectionChangeCommitted += (sender, e) => 
                    SaveConfiguration(comboBox, comboBox.SelectedIndex);
            }
            else if (control is TrackBar trackBar)
            {
                trackBar.ValueChanged += (sender, e) =>
                    SaveConfiguration(trackBar, trackBar.Value);
            }
            else if (control is CheckBox checkBox)
            {
                checkBox.CheckedChanged += (sender, e) => 
                    SaveConfiguration(checkBox, checkBox.Checked);
            }
        }
    }
```


**Save individual control changes**


When changes occur, just serialize the dictionary. As written, it's not terribly efficient. Every keystroke in any textbox writes the entire file. So optimize by waiting for changes to settle and maybe writing the congig file asynchronously or even just doing it when the app closes.

```

    private void SaveConfiguration(Control control, object value)
    {
        ConfigurationHelper[control.Name] = value.ToString();
        // Save every change OR: - Save on app close? - Save after time delay (consolidate rapid changes)?
        File.WriteAllText(configPath, JsonConvert.SerializeObject(ConfigurationHelper));
    }
```

