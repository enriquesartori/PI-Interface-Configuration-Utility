using System;
using System.Drawing;
using System.Windows.Forms;

namespace PIInterfaceConfigUtility
{
    public partial class WriteValueDialog : Form
    {
        private readonly string pointName;
        private TextBox valueTextBox;
        private ComboBox valueTypeComboBox;
        private DateTimePicker timestampPicker;
        private CheckBox useCurrentTimeCheckBox;
        private Button okButton, cancelButton;
        
        public object Value { get; private set; } = 0;
        public DateTime Timestamp { get; private set; } = DateTime.Now;
        
        public WriteValueDialog(string pointName)
        {
            this.pointName = pointName ?? throw new ArgumentNullException(nameof(pointName));
            InitializeComponent();
        }
        
        private void InitializeComponent()
        {
            this.SuspendLayout();
            
            this.Size = new Size(400, 300);
            this.Text = $"Write Value to {pointName}";
            this.FormBorderStyle = FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.StartPosition = FormStartPosition.CenterParent;
            this.ShowIcon = false;
            
            // Point Info
            var pointLabel = new Label
            {
                Text = $"Writing value to PI Point: {pointName}",
                Location = new Point(15, 15),
                Size = new Size(350, 23),
                Font = new Font("Segoe UI", 9F, FontStyle.Bold),
                ForeColor = Color.FromArgb(0, 120, 215)
            };
            
            // Value Type
            var typeLabel = new Label
            {
                Text = "Value Type:",
                Location = new Point(15, 50),
                Size = new Size(80, 23)
            };
            
            valueTypeComboBox = new ComboBox
            {
                Location = new Point(105, 47),
                Size = new Size(120, 23),
                DropDownStyle = ComboBoxStyle.DropDownList
            };
            
            valueTypeComboBox.Items.AddRange(new[]
            {
                "Number",
                "Text",
                "Boolean"
            });
            valueTypeComboBox.SelectedIndex = 0;
            
            // Value
            var valueLabel = new Label
            {
                Text = "Value:",
                Location = new Point(15, 85),
                Size = new Size(80, 23)
            };
            
            valueTextBox = new TextBox
            {
                Location = new Point(105, 82),
                Size = new Size(250, 23),
                Text = "0"
            };
            
            // Timestamp Group
            var timestampGroup = new GroupBox
            {
                Text = "Timestamp",
                Location = new Point(15, 120),
                Size = new Size(350, 80),
                Font = new Font("Segoe UI", 9F, FontStyle.Bold)
            };
            
            useCurrentTimeCheckBox = new CheckBox
            {
                Text = "Use current time",
                Location = new Point(15, 25),
                Size = new Size(150, 23),
                Checked = true
            };
            
            timestampPicker = new DateTimePicker
            {
                Location = new Point(15, 50),
                Size = new Size(200, 23),
                Format = DateTimePickerFormat.Custom,
                CustomFormat = "yyyy-MM-dd HH:mm:ss",
                Value = DateTime.Now,
                Enabled = false
            };
            
            timestampGroup.Controls.AddRange(new Control[]
            {
                useCurrentTimeCheckBox, timestampPicker
            });
            
            // Value Examples
            var examplesLabel = new Label
            {
                Text = "Examples:\nNumber: 123.45, -67.89\nText: \"Hello World\", \"Status OK\"\nBoolean: true, false, 1, 0",
                Location = new Point(15, 210),
                Size = new Size(350, 60),
                Font = new Font("Segoe UI", 8F),
                ForeColor = Color.Gray
            };
            
            // Buttons
            okButton = new Button
            {
                Text = "Write Value",
                Location = new Point(190, 230),
                Size = new Size(90, 30),
                BackColor = Color.FromArgb(0, 120, 215),
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat,
                DialogResult = DialogResult.OK
            };
            
            cancelButton = new Button
            {
                Text = "Cancel",
                Location = new Point(290, 230),
                Size = new Size(80, 30),
                BackColor = Color.FromArgb(108, 117, 125),
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat,
                DialogResult = DialogResult.Cancel
            };
            
            this.Controls.AddRange(new Control[]
            {
                pointLabel, typeLabel, valueTypeComboBox,
                valueLabel, valueTextBox, timestampGroup,
                examplesLabel, okButton, cancelButton
            });
            
            this.AcceptButton = okButton;
            this.CancelButton = cancelButton;
            
            useCurrentTimeCheckBox.CheckedChanged += UseCurrentTimeCheckBox_CheckedChanged;
            valueTypeComboBox.SelectedIndexChanged += ValueTypeComboBox_SelectedIndexChanged;
            okButton.Click += OkButton_Click;
            
            this.ResumeLayout(false);
        }
        
        private void UseCurrentTimeCheckBox_CheckedChanged(object? sender, EventArgs e)
        {
            timestampPicker.Enabled = !useCurrentTimeCheckBox.Checked;
            if (useCurrentTimeCheckBox.Checked)
            {
                timestampPicker.Value = DateTime.Now;
            }
        }
        
        private void ValueTypeComboBox_SelectedIndexChanged(object? sender, EventArgs e)
        {
            var selectedType = valueTypeComboBox.SelectedItem?.ToString();
            
            valueTextBox.Text = selectedType switch
            {
                "Number" => "0",
                "Text" => "Hello World",
                "Boolean" => "true",
                _ => "0"
            };
        }
        
        private void OkButton_Click(object? sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(valueTextBox.Text))
            {
                MessageBox.Show("Please enter a value.", "Validation Error",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                this.DialogResult = DialogResult.None;
                return;
            }
            
            try
            {
                var selectedType = valueTypeComboBox.SelectedItem?.ToString();
                var inputValue = valueTextBox.Text.Trim();
                
                Value = selectedType switch
                {
                    "Number" => ParseNumber(inputValue),
                    "Text" => inputValue.Trim('"'),
                    "Boolean" => ParseBoolean(inputValue),
                    _ => inputValue
                };
                
                Timestamp = useCurrentTimeCheckBox.Checked ? DateTime.Now : timestampPicker.Value;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Invalid value format: {ex.Message}", "Validation Error",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                this.DialogResult = DialogResult.None;
            }
        }
        
        private object ParseNumber(string input)
        {
            if (double.TryParse(input, out double doubleValue))
                return doubleValue;
            if (int.TryParse(input, out int intValue))
                return intValue;
                
            throw new FormatException($"'{input}' is not a valid number.");
        }
        
        private bool ParseBoolean(string input)
        {
            var lowerInput = input.ToLower();
            
            if (lowerInput == "true" || lowerInput == "1" || lowerInput == "yes" || lowerInput == "on")
                return true;
            if (lowerInput == "false" || lowerInput == "0" || lowerInput == "no" || lowerInput == "off")
                return false;
                
            throw new FormatException($"'{input}' is not a valid boolean value.");
        }
    }
} 