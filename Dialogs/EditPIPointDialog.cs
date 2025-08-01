using System;
using System.Drawing;
using System.Windows.Forms;
using PIInterfaceConfigUtility.Models;

namespace PIInterfaceConfigUtility
{
    public partial class EditPIPointDialog : Form
    {
        private readonly PIPoint originalPoint;
        private TextBox nameTextBox, descriptionTextBox, sourceAddressTextBox, unitsTextBox;
        private ComboBox typeComboBox;
        private CheckBox enabledCheckBox, archiveCheckBox, filterDuplicatesCheckBox;
        private NumericUpDown scanIntervalNumeric, conversionFactorNumeric, conversionOffsetNumeric;
        private PropertyGrid propertiesGrid;
        private Button okButton, cancelButton;
        
        public PIPoint PIPoint { get; private set; }
        
        public EditPIPointDialog(PIPoint point)
        {
            originalPoint = point ?? throw new ArgumentNullException(nameof(point));
            PIPoint = ClonePoint(originalPoint);
            InitializeComponent();
            PopulateFields();
        }
        
        private PIPoint ClonePoint(PIPoint source)
        {
            var clone = new PIPoint(source.Name, source.SourceAddress, source.Type)
            {
                Id = source.Id,
                Description = source.Description,
                Status = source.Status,
                Units = source.Units,
                MinValue = source.MinValue,
                MaxValue = source.MaxValue,
                DigitalStates = source.DigitalStates,
                ScanInterval = source.ScanInterval,
                Enabled = source.Enabled,
                Archive = source.Archive,
                InterfaceId = source.InterfaceId,
                LastUpdate = source.LastUpdate,
                CurrentValue = source.CurrentValue,
                UpdateCount = source.UpdateCount,
                ConversionFactor = source.ConversionFactor,
                ConversionOffset = source.ConversionOffset,
                ConversionFormula = source.ConversionFormula,
                FilterDuplicates = source.FilterDuplicates,
                CompressionDeviation = source.CompressionDeviation,
                CompressionTimeDeadband = source.CompressionTimeDeadband
            };
            
            foreach (var attr in source.Attributes)
                clone.Attributes[attr.Key] = attr.Value;
                
            return clone;
        }
        
        private void InitializeComponent()
        {
            this.SuspendLayout();
            
            this.Size = new Size(500, 650);
            this.Text = "Edit PI Point";
            this.FormBorderStyle = FormBorderStyle.Sizable;
            this.MinimumSize = new Size(500, 650);
            this.StartPosition = FormStartPosition.CenterParent;
            this.ShowIcon = false;
            
            var mainPanel = new Panel
            {
                Dock = DockStyle.Fill,
                Padding = new Padding(15),
                AutoScroll = true
            };
            
            // Basic Information Group
            var basicGroup = new GroupBox
            {
                Text = "Basic Information",
                Location = new Point(0, 0),
                Size = new Size(460, 150),
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
                Location = new Point(0, 160),
                Size = new Size(460, 120),
                Font = new Font("Segoe UI", 9F, FontStyle.Bold)
            };
            
            // Enabled
            enabledCheckBox = new CheckBox
            {
                Text = "Enabled",
                Location = new Point(15, 25),
                Size = new Size(80, 23)
            };
            
            // Archive
            archiveCheckBox = new CheckBox
            {
                Text = "Archive Data",
                Location = new Point(105, 25),
                Size = new Size(100, 23)
            };
            
            // Filter Duplicates
            filterDuplicatesCheckBox = new CheckBox
            {
                Text = "Filter Duplicates",
                Location = new Point(215, 25),
                Size = new Size(120, 23)
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
                enabledCheckBox, archiveCheckBox, filterDuplicatesCheckBox,
                scanLabel, scanIntervalNumeric
            });
            
            // Conversion Group
            var conversionGroup = new GroupBox
            {
                Text = "Value Conversion",
                Location = new Point(0, 290),
                Size = new Size(460, 100),
                Font = new Font("Segoe UI", 9F, FontStyle.Bold)
            };
            
            // Conversion Factor
            var factorLabel = new Label
            {
                Text = "Factor:",
                Location = new Point(15, 25),
                Size = new Size(60, 23)
            };
            
            conversionFactorNumeric = new NumericUpDown
            {
                Location = new Point(85, 22),
                Size = new Size(100, 23),
                DecimalPlaces = 6,
                Minimum = -1000000,
                Maximum = 1000000,
                Value = 1
            };
            
            // Conversion Offset
            var offsetLabel = new Label
            {
                Text = "Offset:",
                Location = new Point(200, 25),
                Size = new Size(50, 23)
            };
            
            conversionOffsetNumeric = new NumericUpDown
            {
                Location = new Point(260, 22),
                Size = new Size(100, 23),
                DecimalPlaces = 6,
                Minimum = -1000000,
                Maximum = 1000000,
                Value = 0
            };
            
            conversionGroup.Controls.AddRange(new Control[]
            {
                factorLabel, conversionFactorNumeric, offsetLabel, conversionOffsetNumeric
            });
            
            // Advanced Properties Group
            var propertiesGroup = new GroupBox
            {
                Text = "Advanced Properties",
                Location = new Point(0, 400),
                Size = new Size(460, 180),
                Font = new Font("Segoe UI", 9F, FontStyle.Bold)
            };
            
            propertiesGrid = new PropertyGrid
            {
                Location = new Point(15, 25),
                Size = new Size(430, 140),
                PropertySort = PropertySort.Categorized,
                HelpVisible = false
            };
            
            propertiesGroup.Controls.Add(propertiesGrid);
            
            // Buttons
            okButton = new Button
            {
                Text = "Save Changes",
                Location = new Point(270, 590),
                Size = new Size(100, 30),
                BackColor = Color.FromArgb(0, 120, 215),
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat,
                DialogResult = DialogResult.OK
            };
            
            cancelButton = new Button
            {
                Text = "Cancel",
                Location = new Point(380, 590),
                Size = new Size(80, 30),
                BackColor = Color.FromArgb(108, 117, 125),
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat,
                DialogResult = DialogResult.Cancel
            };
            
            mainPanel.Controls.AddRange(new Control[]
            {
                basicGroup, configGroup, conversionGroup, propertiesGroup, okButton, cancelButton
            });
            
            this.Controls.Add(mainPanel);
            this.AcceptButton = okButton;
            this.CancelButton = cancelButton;
            
            okButton.Click += OkButton_Click;
            
            this.ResumeLayout(false);
        }
        
        private void PopulateFields()
        {
            nameTextBox.Text = PIPoint.Name;
            typeComboBox.SelectedItem = PIPoint.Type;
            sourceAddressTextBox.Text = PIPoint.SourceAddress;
            unitsTextBox.Text = PIPoint.Units;
            descriptionTextBox.Text = PIPoint.Description;
            enabledCheckBox.Checked = PIPoint.Enabled;
            archiveCheckBox.Checked = PIPoint.Archive;
            filterDuplicatesCheckBox.Checked = PIPoint.FilterDuplicates;
            scanIntervalNumeric.Value = PIPoint.ScanInterval;
            conversionFactorNumeric.Value = (decimal)PIPoint.ConversionFactor;
            conversionOffsetNumeric.Value = (decimal)PIPoint.ConversionOffset;
            
            propertiesGrid.SelectedObject = PIPoint;
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
            
            // Update point with form values
            PIPoint.Name = nameTextBox.Text;
            PIPoint.Type = (PIPointType)typeComboBox.SelectedItem;
            PIPoint.SourceAddress = sourceAddressTextBox.Text;
            PIPoint.Units = unitsTextBox.Text;
            PIPoint.Description = descriptionTextBox.Text;
            PIPoint.Enabled = enabledCheckBox.Checked;
            PIPoint.Archive = archiveCheckBox.Checked;
            PIPoint.FilterDuplicates = filterDuplicatesCheckBox.Checked;
            PIPoint.ScanInterval = (int)scanIntervalNumeric.Value;
            PIPoint.ConversionFactor = (double)conversionFactorNumeric.Value;
            PIPoint.ConversionOffset = (double)conversionOffsetNumeric.Value;
        }
    }
} 