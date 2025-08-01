using System;
using System.Drawing;
using System.Windows.Forms;
using PIInterfaceConfigUtility.Models;

namespace PIInterfaceConfigUtility
{
    public partial class AddPIPointDialog : Form
    {
        private TextBox nameTextBox, descriptionTextBox, sourceAddressTextBox, unitsTextBox;
        private ComboBox typeComboBox;
        private CheckBox enabledCheckBox, archiveCheckBox;
        private NumericUpDown scanIntervalNumeric;
        private Button okButton, cancelButton;
        
        public PIPoint PIPoint { get; private set; }
        
        public AddPIPointDialog()
        {
            PIPoint = new PIPoint();
            InitializeComponent();
        }
        
        private void InitializeComponent()
        {
            this.SuspendLayout();
            
            this.Size = new Size(450, 400);
            this.Text = "Add New PI Point";
            this.FormBorderStyle = FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.StartPosition = FormStartPosition.CenterParent;
            this.ShowIcon = false;
            
            // Basic Information Group
            var basicGroup = new GroupBox
            {
                Text = "Basic Information",
                Location = new Point(15, 15),
                Size = new Size(410, 150),
                Font = new Font("Segoe UI", 9F, FontStyle.Bold)
            };
            
            // Name
            var nameLabel = new Label
            {
                Text = "Point Name:",
                Location = new Point(15, 25),
                Size = new Size(80, 23)
            };
            
            nameTextBox = new TextBox
            {
                Location = new Point(105, 22),
                Size = new Size(280, 23)
            };
            
            // Type
            var typeLabel = new Label
            {
                Text = "Data Type:",
                Location = new Point(15, 55),
                Size = new Size(80, 23)
            };
            
            typeComboBox = new ComboBox
            {
                Location = new Point(105, 52),
                Size = new Size(150, 23),
                DropDownStyle = ComboBoxStyle.DropDownList
            };
            
            foreach (PIPointType type in Enum.GetValues<PIPointType>())
            {
                typeComboBox.Items.Add(type);
            }
            typeComboBox.SelectedIndex = 0;
            
            // Source Address
            var addressLabel = new Label
            {
                Text = "Source Address:",
                Location = new Point(15, 85),
                Size = new Size(100, 23)
            };
            
            sourceAddressTextBox = new TextBox
            {
                Location = new Point(105, 82),
                Size = new Size(150, 23)
            };
            
            // Units
            var unitsLabel = new Label
            {
                Text = "Units:",
                Location = new Point(270, 85),
                Size = new Size(40, 23)
            };
            
            unitsTextBox = new TextBox
            {
                Location = new Point(315, 82),
                Size = new Size(70, 23)
            };
            
            // Description
            var descLabel = new Label
            {
                Text = "Description:",
                Location = new Point(15, 115),
                Size = new Size(80, 23)
            };
            
            descriptionTextBox = new TextBox
            {
                Location = new Point(105, 112),
                Size = new Size(280, 23)
            };
            
            basicGroup.Controls.AddRange(new Control[]
            {
                nameLabel, nameTextBox, typeLabel, typeComboBox,
                addressLabel, sourceAddressTextBox, unitsLabel, unitsTextBox,
                descLabel, descriptionTextBox
            });
            
            // Configuration Group
            var configGroup = new GroupBox
            {
                Text = "Configuration",
                Location = new Point(15, 175),
                Size = new Size(410, 120),
                Font = new Font("Segoe UI", 9F, FontStyle.Bold)
            };
            
            // Enabled
            enabledCheckBox = new CheckBox
            {
                Text = "Enabled",
                Location = new Point(15, 25),
                Size = new Size(80, 23),
                Checked = true
            };
            
            // Archive
            archiveCheckBox = new CheckBox
            {
                Text = "Archive Data",
                Location = new Point(105, 25),
                Size = new Size(100, 23),
                Checked = true
            };
            
            // Scan Interval
            var scanLabel = new Label
            {
                Text = "Scan Interval (ms):",
                Location = new Point(15, 55),
                Size = new Size(120, 23)
            };
            
            scanIntervalNumeric = new NumericUpDown
            {
                Location = new Point(145, 52),
                Size = new Size(100, 23),
                Minimum = 100,
                Maximum = 60000,
                Value = 1000
            };
            
            configGroup.Controls.AddRange(new Control[]
            {
                enabledCheckBox, archiveCheckBox, scanLabel, scanIntervalNumeric
            });
            
            // Buttons
            okButton = new Button
            {
                Text = "Add Point",
                Location = new Point(270, 320),
                Size = new Size(80, 30),
                BackColor = Color.FromArgb(0, 120, 215),
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat,
                DialogResult = DialogResult.OK
            };
            
            cancelButton = new Button
            {
                Text = "Cancel",
                Location = new Point(360, 320),
                Size = new Size(80, 30),
                BackColor = Color.FromArgb(108, 117, 125),
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat,
                DialogResult = DialogResult.Cancel
            };
            
            this.Controls.AddRange(new Control[]
            {
                basicGroup, configGroup, okButton, cancelButton
            });
            
            this.AcceptButton = okButton;
            this.CancelButton = cancelButton;
            
            okButton.Click += OkButton_Click;
            
            this.ResumeLayout(false);
        }
        
        private void OkButton_Click(object? sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(nameTextBox.Text))
            {
                MessageBox.Show("Please enter a point name.", "Validation Error",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                this.DialogResult = DialogResult.None;
                return;
            }
            
            if (string.IsNullOrWhiteSpace(sourceAddressTextBox.Text))
            {
                MessageBox.Show("Please enter a source address.", "Validation Error",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                this.DialogResult = DialogResult.None;
                return;
            }
            
            // Create the PI point with form values
            PIPoint = new PIPoint(nameTextBox.Text, sourceAddressTextBox.Text, (PIPointType)typeComboBox.SelectedItem)
            {
                Description = descriptionTextBox.Text,
                Units = unitsTextBox.Text,
                Enabled = enabledCheckBox.Checked,
                Archive = archiveCheckBox.Checked,
                ScanInterval = (int)scanIntervalNumeric.Value
            };
        }
    }
} 