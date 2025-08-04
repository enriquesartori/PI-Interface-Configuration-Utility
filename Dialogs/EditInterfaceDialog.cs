using System;
using System.Windows.Forms;
using PIInterfaceConfigUtility.Models;
using System.Linq;

namespace PIInterfaceConfigUtility.Dialogs
{
    /// <summary>
    /// Dialog for editing existing PI Interface configurations
    /// </summary>
    public partial class EditInterfaceDialog : Form
    {
        private PIInterface _interface;
        // UI Controls - make all nullable
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

            InitializeComponents();
            LoadInterfaceData();
        }

        private void InitializeComponents()
        {
            Text = "Edit PI Interface";
            Size = new System.Drawing.Size(500, 450);
            StartPosition = FormStartPosition.CenterParent;
            FormBorderStyle = FormBorderStyle.FixedDialog;
            MaximizeBox = false;
            MinimizeBox = false;

            CreateControls();
            LayoutControls();
            SetupEventHandlers();
        }

        private void CreateControls()
        {
            // Name
            _nameTextBox = new TextBox();

            // Type
            _typeComboBox = new ComboBox
            {
                DropDownStyle = ComboBoxStyle.DropDownList
            };
            _typeComboBox.Items.AddRange(Enum.GetValues(typeof(InterfaceType)).Cast<object>().ToArray());

            // Description
            _descriptionTextBox = new TextBox { Multiline = true };

            // Service Name
            _serviceNameTextBox = new TextBox();

            // Config File Path
            _configFilePathTextBox = new TextBox();

            // Log File Path
            _logFilePathTextBox = new TextBox();

            // Enabled
            _enabledCheckBox = new CheckBox { Text = "Enabled", Checked = true };

            // Auto Start
            _autoStartCheckBox = new CheckBox { Text = "Auto Start", Checked = false };

            // Buttons
            _okButton = new Button
            {
                Text = "OK",
                DialogResult = DialogResult.OK,
                Size = new System.Drawing.Size(75, 25)
            };

            _cancelButton = new Button
            {
                Text = "Cancel",
                DialogResult = DialogResult.Cancel,
                Size = new System.Drawing.Size(75, 25)
            };

            AcceptButton = _okButton;
            CancelButton = _cancelButton;
        }

        private void LayoutControls()
        {
            const int margin = 20;
            const int labelWidth = 120;
            const int controlWidth = 300;
            const int rowHeight = 35;
            int currentY = margin;

            // Helper method to create label and position control
            void AddRow(string labelText, Control control, int height = 23)
            {
                var label = new Label
                {
                    Text = labelText,
                    Location = new System.Drawing.Point(margin, currentY),
                    Size = new System.Drawing.Size(labelWidth, 20)
                };
                
                control.Location = new System.Drawing.Point(margin + labelWidth + 10, currentY);
                control.Size = new System.Drawing.Size(controlWidth, height);
                
                Controls.Add(label);
                Controls.Add(control);
                currentY += height > 23 ? height + 15 : rowHeight;
            }

            AddRow("Name:", _nameTextBox!);
            AddRow("Type:", _typeComboBox!);
            AddRow("Description:", _descriptionTextBox!, 60);
            AddRow("Service Name:", _serviceNameTextBox!);
            AddRow("Config File Path:", _configFilePathTextBox!);
            AddRow("Log File Path:", _logFilePathTextBox!);

            // Checkboxes
            _enabledCheckBox!.Location = new System.Drawing.Point(margin + labelWidth + 10, currentY);
            Controls.Add(_enabledCheckBox);
            currentY += 25;

            _autoStartCheckBox!.Location = new System.Drawing.Point(margin + labelWidth + 10, currentY);
            Controls.Add(_autoStartCheckBox);
            currentY += 40;

            // Buttons
            _okButton!.Location = new System.Drawing.Point(controlWidth + margin - 80, currentY);
            _cancelButton!.Location = new System.Drawing.Point(controlWidth + margin + 5, currentY);

            Controls.Add(_okButton);
            Controls.Add(_cancelButton);
        }

        private void SetupEventHandlers()
        {
            _okButton!.Click += OkButton_Click;
        }

        private void LoadInterfaceData()
        {
            _nameTextBox!.Text = _interface.Name;
            _typeComboBox!.SelectedItem = _interface.Type;
            _descriptionTextBox!.Text = _interface.Description;
            _serviceNameTextBox!.Text = _interface.ServiceName;
            _configFilePathTextBox!.Text = _interface.ConfigFilePath;
            _logFilePathTextBox!.Text = _interface.LogFilePath;
            _enabledCheckBox!.Checked = _interface.IsEnabled;
            _autoStartCheckBox!.Checked = _interface.AutoStart;
        }

        private void OkButton_Click(object? sender, EventArgs e)
        {
            // Update interface with form values
            _interface.Name = _nameTextBox!.Text;
            _interface.Type = (InterfaceType)_typeComboBox!.SelectedItem!;
            _interface.Description = _descriptionTextBox!.Text;
            _interface.ServiceName = _serviceNameTextBox!.Text;
            _interface.ConfigFilePath = _configFilePathTextBox!.Text;
            _interface.LogFilePath = _logFilePathTextBox!.Text;
            _interface.IsEnabled = _enabledCheckBox!.Checked;
            _interface.AutoStart = _autoStartCheckBox!.Checked;

            DialogResult = DialogResult.OK;
            Close();
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