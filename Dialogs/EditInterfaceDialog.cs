using System;
using System.Windows.Forms;
using PIInterfaceConfigUtility.Models;

namespace PIInterfaceConfigUtility.Dialogs
{
    /// <summary>
    /// Dialog for editing existing PI Interface configurations
    /// </summary>
    public partial class EditInterfaceDialog : Form
    {
        private PIInterface _interface;
        // Make all UI controls nullable
        private TextBox? _nameTextBox;
        private ComboBox? _typeComboBox;
        private TextBox? _descriptionTextBox;
        private TextBox? _serviceNameTextBox;
        private TextBox? _configFilePathTextBox;
        private TextBox? _logFilePathTextBox;
        private CheckBox? _enabledCheckBox;
        private CheckBox? _autoStartCheckBox;
        private Button? _okButton;
        private Button? _cancelButton;

        public PIInterface Interface => _interface;

        public EditInterfaceDialog(PIInterface interfaceToEdit)
        {
            _interface = new PIInterface
            {
                Id = interfaceToEdit.Id,
                Name = interfaceToEdit.Name,
                Type = interfaceToEdit.Type,
                Description = interfaceToEdit.Description,
                ServiceName = interfaceToEdit.ServiceName,
                ConfigFilePath = interfaceToEdit.ConfigFilePath,
                LogFilePath = interfaceToEdit.LogFilePath,
                Status = interfaceToEdit.Status,
                IsEnabled = interfaceToEdit.IsEnabled,
                AutoStart = interfaceToEdit.AutoStart
            };

            InitializeComponent();
            LoadInterfaceData();
        }

        private void InitializeComponent()
        {
            Text = "Edit PI Interface";
            Size = new System.Drawing.Size(500, 400);
            StartPosition = FormStartPosition.CenterParent;
            FormBorderStyle = FormBorderStyle.FixedDialog;
            MaximizeBox = false;
            MinimizeBox = false;

            // Create controls
            var nameLabel = new Label
            {
                Text = "Interface Name:",
                Location = new System.Drawing.Point(12, 15),
                Size = new System.Drawing.Size(120, 23),
                TextAlign = System.Drawing.ContentAlignment.MiddleLeft
            };

            _nameTextBox = new TextBox
            {
                Location = new System.Drawing.Point(140, 12),
                Size = new System.Drawing.Size(330, 23),
                MaxLength = 100
            };

            var typeLabel = new Label
            {
                Text = "Interface Type:",
                Location = new System.Drawing.Point(12, 50),
                Size = new System.Drawing.Size(120, 23),
                TextAlign = System.Drawing.ContentAlignment.MiddleLeft
            };

            _typeComboBox = new ComboBox
            {
                Location = new System.Drawing.Point(140, 47),
                Size = new System.Drawing.Size(330, 23),
                DropDownStyle = ComboBoxStyle.DropDownList
            };

            // Populate interface types
            foreach (InterfaceType type in Enum.GetValues<InterfaceType>())
            {
                _typeComboBox.Items.Add(type);
            }

            var descriptionLabel = new Label
            {
                Text = "Description:",
                Location = new System.Drawing.Point(12, 85),
                Size = new System.Drawing.Size(120, 23),
                TextAlign = System.Drawing.ContentAlignment.MiddleLeft
            };

            _descriptionTextBox = new TextBox
            {
                Location = new System.Drawing.Point(140, 82),
                Size = new System.Drawing.Size(330, 60),
                Multiline = true,
                ScrollBars = ScrollBars.Vertical,
                MaxLength = 500
            };

            var serviceNameLabel = new Label
            {
                Text = "Service Name:",
                Location = new System.Drawing.Point(12, 155),
                Size = new System.Drawing.Size(120, 23),
                TextAlign = System.Drawing.ContentAlignment.MiddleLeft
            };

            _serviceNameTextBox = new TextBox
            {
                Location = new System.Drawing.Point(140, 152),
                Size = new System.Drawing.Size(330, 23),
                MaxLength = 100
            };

            var configFileLabel = new Label
            {
                Text = "Config File Path:",
                Location = new System.Drawing.Point(12, 190),
                Size = new System.Drawing.Size(120, 23),
                TextAlign = System.Drawing.ContentAlignment.MiddleLeft
            };

            _configFilePathTextBox = new TextBox
            {
                Location = new System.Drawing.Point(140, 187),
                Size = new System.Drawing.Size(330, 23),
                MaxLength = 260
            };

            var logFileLabel = new Label
            {
                Text = "Log File Path:",
                Location = new System.Drawing.Point(12, 225),
                Size = new System.Drawing.Size(120, 23),
                TextAlign = System.Drawing.ContentAlignment.MiddleLeft
            };

            _logFilePathTextBox = new TextBox
            {
                Location = new System.Drawing.Point(140, 222),
                Size = new System.Drawing.Size(330, 23),
                MaxLength = 260
            };

            _enabledCheckBox = new CheckBox
            {
                Text = "Enabled for data collection",
                Location = new System.Drawing.Point(140, 260),
                Size = new System.Drawing.Size(200, 23),
                Checked = true
            };

            _autoStartCheckBox = new CheckBox
            {
                Text = "Auto-start with system",
                Location = new System.Drawing.Point(140, 290),
                Size = new System.Drawing.Size(200, 23),
                Checked = false
            };

            _okButton = new Button
            {
                Text = "Save Changes",
                Location = new System.Drawing.Point(310, 330),
                Size = new System.Drawing.Size(80, 30),
                DialogResult = DialogResult.OK
            };
            _okButton.Click += OkButton_Click;

            _cancelButton = new Button
            {
                Text = "Cancel",
                Location = new System.Drawing.Point(395, 330),
                Size = new System.Drawing.Size(75, 30),
                DialogResult = DialogResult.Cancel
            };

            // Add controls to form
            Controls.AddRange(new Control[] {
                nameLabel, _nameTextBox,
                typeLabel, _typeComboBox,
                descriptionLabel, _descriptionTextBox,
                serviceNameLabel, _serviceNameTextBox,
                configFileLabel, _configFilePathTextBox,
                logFileLabel, _logFilePathTextBox,
                _enabledCheckBox, _autoStartCheckBox,
                _okButton, _cancelButton
            });

            AcceptButton = _okButton;
            CancelButton = _cancelButton;
        }

        private void LoadInterfaceData()
        {
            _nameTextBox.Text = _interface.Name;
            _typeComboBox.SelectedItem = _interface.Type;
            _descriptionTextBox.Text = _interface.Description;
            _serviceNameTextBox.Text = _interface.ServiceName;
            _configFilePathTextBox.Text = _interface.ConfigFilePath;
            _logFilePathTextBox.Text = _interface.LogFilePath;
            _enabledCheckBox.Checked = _interface.IsEnabled;
            _autoStartCheckBox.Checked = _interface.AutoStart;
        }

        private void OkButton_Click(object? sender, EventArgs e)
        {
            if (ValidateInput())
            {
                SaveChanges();
                DialogResult = DialogResult.OK;
                Close();
            }
        }

        private bool ValidateInput()
        {
            // Validate name
            if (string.IsNullOrWhiteSpace(_nameTextBox.Text))
            {
                MessageBox.Show("Please enter an interface name.", "Validation Error",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                _nameTextBox.Focus();
                return false;
            }

            // Validate type selection
            if (_typeComboBox.SelectedItem == null)
            {
                MessageBox.Show("Please select an interface type.", "Validation Error",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                _typeComboBox.Focus();
                return false;
            }

            // Validate service name
            if (string.IsNullOrWhiteSpace(_serviceNameTextBox.Text))
            {
                MessageBox.Show("Please enter a service name.", "Validation Error",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                _serviceNameTextBox.Focus();
                return false;
            }

            return true;
        }

        private void SaveChanges()
        {
            _interface.Name = _nameTextBox.Text.Trim();
            _interface.Type = (InterfaceType)_typeComboBox.SelectedItem!;
            _interface.Description = _descriptionTextBox.Text.Trim();
            _interface.ServiceName = _serviceNameTextBox.Text.Trim();
            _interface.ConfigFilePath = _configFilePathTextBox.Text.Trim();
            _interface.LogFilePath = _logFilePathTextBox.Text.Trim();
            _interface.IsEnabled = _enabledCheckBox.Checked;
            _interface.AutoStart = _autoStartCheckBox.Checked;
        }
    }
} 