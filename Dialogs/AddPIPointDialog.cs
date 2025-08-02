using System;
using System.Drawing;
using System.Windows.Forms;
using PIInterfaceConfigUtility.Models;

namespace PIInterfaceConfigUtility.Dialogs
{
    public partial class AddPIPointDialog : Form
    {
        private Label? nameLabel;
        private TextBox? nameTextBox;
        private Label? descriptionLabel;
        private TextBox? descriptionTextBox;
        private Label? sourceAddressLabel;
        private TextBox? sourceAddressTextBox;
        private Label? unitsLabel;
        private TextBox? unitsTextBox;
        private Label? typeLabel;
        private ComboBox? typeComboBox;
        private CheckBox? enabledCheckBox;
        private CheckBox? archiveCheckBox;
        private Label? scanIntervalLabel;
        private NumericUpDown? scanIntervalNumeric;
        private Button? okButton;
        private Button? cancelButton;

        public PIPoint PIPoint { get; private set; } = new();

        public AddPIPointDialog()
        {
            InitializeComponent();
        }

        private void InitializeComponent()
        {
            this.Text = "Add PI Point";
            this.Size = new Size(400, 350);
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
            nameLabel = new Label { Text = "Name:" };
            nameTextBox = new TextBox();

            descriptionLabel = new Label { Text = "Description:" };
            descriptionTextBox = new TextBox();

            sourceAddressLabel = new Label { Text = "Source Address:" };
            sourceAddressTextBox = new TextBox();

            unitsLabel = new Label { Text = "Units:" };
            unitsTextBox = new TextBox();

            typeLabel = new Label { Text = "Data Type:" };
            typeComboBox = new ComboBox
            {
                DropDownStyle = ComboBoxStyle.DropDownList
            };
                         typeComboBox.Items.AddRange(new object[]
             {
                 PIPointDataType.Float32,
                 PIPointDataType.Float64,
                 PIPointDataType.Int16,
                 PIPointDataType.Int32,
                 PIPointDataType.String,
                 PIPointDataType.Digital
             });
            typeComboBox.SelectedIndex = 0;

            enabledCheckBox = new CheckBox
            {
                Text = "Enabled",
                Checked = true
            };

            archiveCheckBox = new CheckBox
            {
                Text = "Archive",
                Checked = true
            };

            scanIntervalLabel = new Label { Text = "Scan Interval (ms):" };
            scanIntervalNumeric = new NumericUpDown
            {
                Minimum = 100,
                Maximum = 3600000,
                Value = 5000,
                Increment = 100
            };

            okButton = new Button
            {
                Text = "OK",
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
            const int labelWidth = 100;
            const int controlWidth = 250;
            const int rowHeight = 35;
            const int leftMargin = 20;
            int currentY = 20;

            // Name
            nameLabel!.Location = new Point(leftMargin, currentY);
            nameLabel.Size = new Size(labelWidth, 20);
            nameTextBox!.Location = new Point(leftMargin + labelWidth + 10, currentY);
            nameTextBox.Size = new Size(controlWidth, 20);
            currentY += rowHeight;

            // Description
            descriptionLabel!.Location = new Point(leftMargin, currentY);
            descriptionLabel.Size = new Size(labelWidth, 20);
            descriptionTextBox!.Location = new Point(leftMargin + labelWidth + 10, currentY);
            descriptionTextBox.Size = new Size(controlWidth, 20);
            currentY += rowHeight;

            // Source Address
            sourceAddressLabel!.Location = new Point(leftMargin, currentY);
            sourceAddressLabel.Size = new Size(labelWidth, 20);
            sourceAddressTextBox!.Location = new Point(leftMargin + labelWidth + 10, currentY);
            sourceAddressTextBox.Size = new Size(controlWidth, 20);
            currentY += rowHeight;

            // Units
            unitsLabel!.Location = new Point(leftMargin, currentY);
            unitsLabel.Size = new Size(labelWidth, 20);
            unitsTextBox!.Location = new Point(leftMargin + labelWidth + 10, currentY);
            unitsTextBox.Size = new Size(controlWidth, 20);
            currentY += rowHeight;

            // Type
            typeLabel!.Location = new Point(leftMargin, currentY);
            typeLabel.Size = new Size(labelWidth, 20);
            typeComboBox!.Location = new Point(leftMargin + labelWidth + 10, currentY);
            typeComboBox.Size = new Size(controlWidth, 20);
            currentY += rowHeight;

            // Checkboxes
            enabledCheckBox!.Location = new Point(leftMargin + labelWidth + 10, currentY);
            enabledCheckBox.Size = new Size(100, 20);
            archiveCheckBox!.Location = new Point(leftMargin + labelWidth + 120, currentY);
            archiveCheckBox.Size = new Size(100, 20);
            currentY += rowHeight;

            // Scan Interval
            scanIntervalLabel!.Location = new Point(leftMargin, currentY);
            scanIntervalLabel.Size = new Size(labelWidth, 20);
            scanIntervalNumeric!.Location = new Point(leftMargin + labelWidth + 10, currentY);
            scanIntervalNumeric.Size = new Size(controlWidth, 20);
            currentY += 50;

            // Buttons
            okButton!.Location = new Point(leftMargin + controlWidth - 80, currentY);
            okButton.Size = new Size(75, 25);
            cancelButton!.Location = new Point(leftMargin + controlWidth + 5, currentY);
            cancelButton.Size = new Size(75, 25);

            // Add controls to form
            this.Controls.AddRange(new Control[]
            {
                nameLabel, nameTextBox,
                descriptionLabel, descriptionTextBox,
                sourceAddressLabel, sourceAddressTextBox,
                unitsLabel, unitsTextBox,
                typeLabel, typeComboBox,
                enabledCheckBox, archiveCheckBox,
                scanIntervalLabel, scanIntervalNumeric,
                okButton, cancelButton
            });
        }

        private void SetupEventHandlers()
        {
            okButton!.Click += OkButton_Click;
        }

        private void OkButton_Click(object? sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(nameTextBox!.Text))
            {
                MessageBox.Show("Please enter a point name.", "Validation Error",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                nameTextBox.Focus();
                return;
            }

            if (string.IsNullOrWhiteSpace(sourceAddressTextBox!.Text))
            {
                MessageBox.Show("Please enter a source address.", "Validation Error",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                sourceAddressTextBox.Focus();
                return;
            }

            // Create the PI Point
            PIPoint = new PIPoint
            {
                Name = nameTextBox.Text.Trim(),
                Description = descriptionTextBox!.Text.Trim(),
                SourceAddress = sourceAddressTextBox.Text.Trim(),
                Units = unitsTextBox!.Text.Trim(),
                                 DataType = (PIPointDataType)typeComboBox!.SelectedItem!,
                 IsEnabled = enabledCheckBox!.Checked,
                 IsArchiving = archiveCheckBox!.Checked,
                 ScanInterval = (int)scanIntervalNumeric!.Value,
                 Status = PIPointStatus.Good,
                 CreatedTime = DateTime.Now,
                 LastUpdateTime = DateTime.Now
            };

            this.DialogResult = DialogResult.OK;
            this.Close();
        }
    }
} 