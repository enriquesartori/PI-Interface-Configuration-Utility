using System;
using System.Drawing;
using System.Windows.Forms;
using PIInterfaceConfigUtility.Models;

namespace PIInterfaceConfigUtility.Dialogs
{
    public partial class WriteValueDialog : Form
    {
        private Label? pointNameLabel;
        private Label? valueLabel;
        private TextBox? valueTextBox;
        private Label? valueTypeLabel;
        private ComboBox? valueTypeComboBox;
        private Label? timestampLabel;
        private DateTimePicker? timestampPicker;
        private CheckBox? useCurrentTimeCheckBox;
        private Button? okButton;
        private Button? cancelButton;

        public string PointName { get; private set; } = "";
        public object? Value { get; private set; }
        public DateTime Timestamp { get; private set; }
        public ValueType ValueType { get; private set; }

        public WriteValueDialog(string pointName)
        {
            PointName = pointName;
            InitializeComponent();
        }

        private void InitializeComponent()
        {
            this.Text = "Write Value to PI Point";
            this.Size = new Size(400, 300);
            this.StartPosition = FormStartPosition.CenterParent;
            this.FormBorderStyle = FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.MinimizeBox = false;

            CreateControls();
            LayoutControls();
            SetupEventHandlers();
        }

        private void CreateControls()
        {
            pointNameLabel = new Label { Text = $"Point: {PointName}", Font = new Font("Segoe UI", 9F, FontStyle.Bold) };
            
            valueLabel = new Label { Text = "Value:" };
            valueTextBox = new TextBox();

            valueTypeLabel = new Label { Text = "Value Type:" };
            valueTypeComboBox = new ComboBox
            {
                DropDownStyle = ComboBoxStyle.DropDownList
            };
            valueTypeComboBox.Items.AddRange(new object[]
            {
                ValueType.Number,
                ValueType.Text,
                ValueType.Boolean
            });
            valueTypeComboBox.SelectedIndex = 0;

            timestampLabel = new Label { Text = "Timestamp:" };
            timestampPicker = new DateTimePicker
            {
                Format = DateTimePickerFormat.Custom,
                CustomFormat = "yyyy-MM-dd HH:mm:ss",
                Value = DateTime.Now
            };

            useCurrentTimeCheckBox = new CheckBox
            {
                Text = "Use current time",
                Checked = true
            };

            okButton = new Button
            {
                Text = "Write Value",
                DialogResult = DialogResult.OK
            };

            cancelButton = new Button
            {
                Text = "Cancel",
                DialogResult = DialogResult.Cancel
            };

            this.AcceptButton = okButton;
            this.CancelButton = cancelButton;
        }

        private void LayoutControls()
        {
            const int margin = 20;
            const int labelWidth = 80;
            const int controlWidth = 250;
            const int rowHeight = 35;
            int currentY = margin;

            // Point Name
            pointNameLabel!.Location = new Point(margin, currentY);
            pointNameLabel.Size = new Size(controlWidth + labelWidth, 20);
            currentY += rowHeight;

            // Value
            valueLabel!.Location = new Point(margin, currentY);
            valueLabel.Size = new Size(labelWidth, 20);
            valueTextBox!.Location = new Point(margin + labelWidth + 10, currentY);
            valueTextBox.Size = new Size(controlWidth, 20);
            currentY += rowHeight;

            // Value Type
            valueTypeLabel!.Location = new Point(margin, currentY);
            valueTypeLabel.Size = new Size(labelWidth, 20);
            valueTypeComboBox!.Location = new Point(margin + labelWidth + 10, currentY);
            valueTypeComboBox.Size = new Size(controlWidth, 20);
            currentY += rowHeight;

            // Timestamp
            timestampLabel!.Location = new Point(margin, currentY);
            timestampLabel.Size = new Size(labelWidth, 20);
            timestampPicker!.Location = new Point(margin + labelWidth + 10, currentY);
            timestampPicker.Size = new Size(controlWidth, 20);
            currentY += rowHeight;

            // Use Current Time
            useCurrentTimeCheckBox!.Location = new Point(margin + labelWidth + 10, currentY);
            useCurrentTimeCheckBox.Size = new Size(controlWidth, 20);
            currentY += 50;

            // Buttons
            okButton!.Location = new Point(margin + controlWidth - 80, currentY);
            okButton.Size = new Size(100, 25);
            cancelButton!.Location = new Point(margin + controlWidth + 10, currentY);
            cancelButton.Size = new Size(75, 25);

            // Add controls to form
            this.Controls.AddRange(new Control[]
            {
                pointNameLabel, valueLabel, valueTextBox,
                valueTypeLabel, valueTypeComboBox,
                timestampLabel, timestampPicker,
                useCurrentTimeCheckBox,
                okButton, cancelButton
            });
        }

        private void SetupEventHandlers()
        {
            useCurrentTimeCheckBox!.CheckedChanged += UseCurrentTimeCheckBox_CheckedChanged;
            okButton!.Click += OkButton_Click;

            // Initial state
            UseCurrentTimeCheckBox_CheckedChanged(null, EventArgs.Empty);
        }

        private void UseCurrentTimeCheckBox_CheckedChanged(object? sender, EventArgs e)
        {
            timestampPicker!.Enabled = !useCurrentTimeCheckBox!.Checked;
            if (useCurrentTimeCheckBox.Checked)
            {
                timestampPicker.Value = DateTime.Now;
            }
        }

        private void OkButton_Click(object? sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(valueTextBox!.Text))
            {
                MessageBox.Show("Please enter a value.", "Validation Error",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                valueTextBox.Focus();
                return;
            }

            try
            {
                ValueType = (ValueType)valueTypeComboBox!.SelectedItem!;
                
                // Parse value based on type
                switch (ValueType)
                {
                    case ValueType.Number:
                        if (double.TryParse(valueTextBox.Text, out double numValue))
                            Value = numValue;
                        else
                            throw new FormatException("Invalid number format");
                        break;
                    case ValueType.Boolean:
                        if (bool.TryParse(valueTextBox.Text, out bool boolValue))
                            Value = boolValue;
                        else
                            throw new FormatException("Invalid boolean format (use true/false)");
                        break;
                    case ValueType.Text:
                    default:
                        Value = valueTextBox.Text;
                        break;
                }

                Timestamp = useCurrentTimeCheckBox!.Checked ? DateTime.Now : timestampPicker!.Value;

                this.DialogResult = DialogResult.OK;
                this.Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Invalid value: {ex.Message}", "Validation Error",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                valueTextBox.Focus();
            }
        }
    }

    public enum ValueType
    {
        Number,
        Text,
        Boolean
    }
} 