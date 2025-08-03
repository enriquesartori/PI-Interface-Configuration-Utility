using System;
using System.Drawing;
using System.Windows.Forms;
using PIInterfaceConfigUtility.Models;

namespace PIInterfaceConfigUtility.Dialogs
{
    public partial class EditPIPointDialog : Form
    {
        // Make all UI controls nullable
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
        private CheckBox? filterDuplicatesCheckBox;
        private Label? scanIntervalLabel;
        private NumericUpDown? scanIntervalNumeric;
        private Label? conversionFactorLabel;
        private NumericUpDown? conversionFactorNumeric;
        private Button? okButton;
        private Button? cancelButton;

        public PIPoint PIPoint { get; private set; }

        public EditPIPointDialog(PIPoint point)
        {
            PIPoint = point;
            InitializeComponent();
            LoadPointData();
        }

        private void InitializeComponent()
        {
            this.Text = "Edit PI Point";
            this.Size = new Size(500, 450);
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
            nameTextBox = new TextBox { ReadOnly = true, BackColor = SystemColors.Control };

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

            enabledCheckBox = new CheckBox { Text = "Enabled" };
            archiveCheckBox = new CheckBox { Text = "Archive" };
            filterDuplicatesCheckBox = new CheckBox { Text = "Filter Duplicates" };

            scanIntervalLabel = new Label { Text = "Scan Interval (ms):" };
            scanIntervalNumeric = new NumericUpDown
            {
                Minimum = 100,
                Maximum = 3600000,
                Increment = 100
            };

            conversionFactorLabel = new Label { Text = "Conversion Factor:" };
            conversionFactorNumeric = new NumericUpDown
            {
                Minimum = -999999,
                Maximum = 999999,
                DecimalPlaces = 6,
                Value = 1
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
            const int labelWidth = 120;
            const int controlWidth = 250;
            const int rowHeight = 35;
            const int leftMargin = 20;
            int currentY = 20;

            // Name (read-only)
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

            // Checkboxes row
            enabledCheckBox!.Location = new Point(leftMargin + labelWidth + 10, currentY);
            enabledCheckBox.Size = new Size(80, 20);
            archiveCheckBox!.Location = new Point(leftMargin + labelWidth + 100, currentY);
            archiveCheckBox.Size = new Size(80, 20);
            filterDuplicatesCheckBox!.Location = new Point(leftMargin + labelWidth + 190, currentY);
            filterDuplicatesCheckBox.Size = new Size(120, 20);
            currentY += rowHeight;

            // Scan Interval
            scanIntervalLabel!.Location = new Point(leftMargin, currentY);
            scanIntervalLabel.Size = new Size(labelWidth, 20);
            scanIntervalNumeric!.Location = new Point(leftMargin + labelWidth + 10, currentY);
            scanIntervalNumeric.Size = new Size(controlWidth, 20);
            currentY += rowHeight;

            // Conversion Factor
            conversionFactorLabel!.Location = new Point(leftMargin, currentY);
            conversionFactorLabel.Size = new Size(labelWidth, 20);
            conversionFactorNumeric!.Location = new Point(leftMargin + labelWidth + 10, currentY);
            conversionFactorNumeric.Size = new Size(controlWidth, 20);
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
                enabledCheckBox, archiveCheckBox, filterDuplicatesCheckBox,
                scanIntervalLabel, scanIntervalNumeric,
                conversionFactorLabel, conversionFactorNumeric,
                okButton, cancelButton
            });
        }

        private void SetupEventHandlers()
        {
            okButton!.Click += OkButton_Click;
        }

        private void LoadPointData()
        {
            if (PIPoint == null) return;

            nameTextBox!.Text = PIPoint.Name;
            descriptionTextBox!.Text = PIPoint.Description;
            sourceAddressTextBox!.Text = PIPoint.SourceAddress;
            unitsTextBox!.Text = PIPoint.Units;
            
            // Set data type
            for (int i = 0; i < typeComboBox!.Items.Count; i++)
            {
                if (typeComboBox.Items[i].Equals(PIPoint.DataType))
                {
                    typeComboBox.SelectedIndex = i;
                    break;
                }
            }

            enabledCheckBox!.Checked = PIPoint.IsEnabled;
            archiveCheckBox!.Checked = PIPoint.IsArchiving;
            filterDuplicatesCheckBox!.Checked = PIPoint.FilterDuplicates;
            scanIntervalNumeric!.Value = PIPoint.ScanInterval;
            conversionFactorNumeric!.Value = (decimal)PIPoint.ConversionFactor;
        }

        private void OkButton_Click(object? sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(sourceAddressTextBox!.Text))
            {
                MessageBox.Show("Please enter a source address.", "Validation Error",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                sourceAddressTextBox.Focus();
                return;
            }

            // Update the PI Point with form values
            PIPoint.Description = descriptionTextBox!.Text.Trim();
            PIPoint.SourceAddress = sourceAddressTextBox.Text.Trim();
            PIPoint.Units = unitsTextBox!.Text.Trim();
            PIPoint.DataType = (PIPointDataType)typeComboBox!.SelectedItem!;
            PIPoint.IsEnabled = enabledCheckBox!.Checked;
            PIPoint.IsArchiving = archiveCheckBox!.Checked;
            PIPoint.FilterDuplicates = filterDuplicatesCheckBox!.Checked;
            PIPoint.ScanInterval = (int)scanIntervalNumeric!.Value;
            PIPoint.ConversionFactor = (double)conversionFactorNumeric!.Value;
            PIPoint.LastUpdateTime = DateTime.Now;

            this.DialogResult = DialogResult.OK;
            this.Close();
        }
    }
} 