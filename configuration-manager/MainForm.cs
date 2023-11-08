using System.Resources;

namespace configuration_manager
{
    public partial class MainForm : Form
    {
        public MainForm()
        {
            InitializeComponent();
        }
        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);
            foreach (var control in IterateControls(Controls))
            {
                var value = Properties.Settings.Default[control.Name];
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
    }
}