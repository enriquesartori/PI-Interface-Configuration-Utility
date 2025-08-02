using System;
using System.Drawing;
using System.Windows.Forms;
using PIInterfaceConfigUtility.Models;

namespace PIInterfaceConfigUtility.Dialogs
{
    public partial class PIServerConnectionDialog : Form
    {
        // Make all UI controls nullable
        private Label? serverLabel;
        private TextBox? serverNameTextBox;
        private Label? portLabel;
        private NumericUpDown? portNumeric;
        private Label? usernameLabel;
        private TextBox? usernameTextBox;
        private Label? passwordLabel;
        private TextBox? passwordTextBox;
        private CheckBox? windowsAuthCheckBox;
        private Button? testConnectionButton;
        private Button? okButton;
        private Button? cancelButton;

        public PIServerConnection Connection { get; private set; } = new();

        public PIServerConnectionDialog()
        {
            InitializeComponent();
        }

        private void InitializeComponent()
        {
            this.Text = "Connect to PI Server";
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
            serverLabel = new Label { Text = "Server Name:" };
            serverNameTextBox = new TextBox { Text = "localhost" };

            portLabel = new Label { Text = "Port:" };
            portNumeric = new NumericUpDown
            {
                Minimum = 1,
                Maximum = 65535,
                Value = 5450
            };

            usernameLabel = new Label { Text = "Username:" };
            usernameTextBox = new TextBox();

            passwordLabel = new Label { Text = "Password:" };
            passwordTextBox = new TextBox { UseSystemPasswordChar = true };

            windowsAuthCheckBox = new CheckBox
            {
                Text = "Use Windows Authentication",
                Checked = true
            };

            testConnectionButton = new Button { Text = "Test Connection" };

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
            const int controlWidth = 200;
            const int margin = 20;
            const int rowHeight = 35;
            int currentY = margin;

            // Server Name
            serverLabel!.Location = new Point(margin, currentY);
            serverLabel.Size = new Size(labelWidth, 20);
            serverNameTextBox!.Location = new Point(margin + labelWidth + 10, currentY);
            serverNameTextBox.Size = new Size(controlWidth, 20);
            currentY += rowHeight;

            // Port
            portLabel!.Location = new Point(margin, currentY);
            portLabel.Size = new Size(labelWidth, 20);
            portNumeric!.Location = new Point(margin + labelWidth + 10, currentY);
            portNumeric.Size = new Size(100, 20);
            currentY += rowHeight;

            // Windows Auth checkbox
            windowsAuthCheckBox!.Location = new Point(margin, currentY);
            windowsAuthCheckBox.Size = new Size(200, 20);
            currentY += rowHeight;

            // Username
            usernameLabel!.Location = new Point(margin, currentY);
            usernameLabel.Size = new Size(labelWidth, 20);
            usernameTextBox!.Location = new Point(margin + labelWidth + 10, currentY);
            usernameTextBox.Size = new Size(controlWidth, 20);
            currentY += rowHeight;

            // Password
            passwordLabel!.Location = new Point(margin, currentY);
            passwordLabel.Size = new Size(labelWidth, 20);
            passwordTextBox!.Location = new Point(margin + labelWidth + 10, currentY);
            passwordTextBox.Size = new Size(controlWidth, 20);
            currentY += 50;

            // Test Connection button
            testConnectionButton!.Location = new Point(margin, currentY);
            testConnectionButton.Size = new Size(120, 25);
            currentY += 40;

            // OK and Cancel buttons
            okButton!.Location = new Point(margin + controlWidth - 80, currentY);
            okButton.Size = new Size(75, 25);
            cancelButton!.Location = new Point(margin + controlWidth + 5, currentY);
            cancelButton.Size = new Size(75, 25);

            // Add all controls to form
            this.Controls.AddRange(new Control[]
            {
                serverLabel, serverNameTextBox,
                portLabel, portNumeric,
                windowsAuthCheckBox,
                usernameLabel, usernameTextBox,
                passwordLabel, passwordTextBox,
                testConnectionButton,
                okButton, cancelButton
            });
        }

        private void SetupEventHandlers()
        {
            windowsAuthCheckBox!.CheckedChanged += WindowsAuthCheckBox_CheckedChanged;
            testConnectionButton!.Click += TestConnectionButton_Click;
            okButton!.Click += OkButton_Click;

            // Initial state
            WindowsAuthCheckBox_CheckedChanged(null, EventArgs.Empty);
        }

        private void WindowsAuthCheckBox_CheckedChanged(object? sender, EventArgs e)
        {
            bool useWindows = windowsAuthCheckBox!.Checked;
            usernameLabel!.Enabled = !useWindows;
            usernameTextBox!.Enabled = !useWindows;
            passwordLabel!.Enabled = !useWindows;
            passwordTextBox!.Enabled = !useWindows;
        }

        private async void TestConnectionButton_Click(object? sender, EventArgs e)
        {
            testConnectionButton!.Enabled = false;
            testConnectionButton.Text = "Testing...";

            try
            {
                // Create test connection
                var testConnection = new PIServerConnection
                {
                    ServerName = serverNameTextBox!.Text.Trim(),
                    Port = (int)portNumeric!.Value,
                    Username = usernameTextBox!.Text.Trim(),
                    Password = passwordTextBox!.Text,
                    UseWindowsAuthentication = windowsAuthCheckBox!.Checked
                };

                // Test with real PI server manager
                var realManager = new Services.RealPIServerManager();
                bool success = await realManager.ConnectAsync(testConnection.ServerName, 
                    testConnection.Username, testConnection.Password);

                if (success)
                {
                    MessageBox.Show("✓ Connection successful!\nReal PI Server detected.", "Test Connection",
                        MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
                else
                {
                    MessageBox.Show("⚠ Connection failed.\nServer not reachable or no PI System detected.", "Test Connection",
                        MessageBoxButtons.OK, MessageBoxIcon.Warning);
                }

                realManager.Disconnect();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"❌ Connection test failed:\n{ex.Message}", "Test Connection",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            finally
            {
                testConnectionButton!.Enabled = true;
                testConnectionButton.Text = "Test Connection";
            }
        }

        private void OkButton_Click(object? sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(serverNameTextBox!.Text))
            {
                MessageBox.Show("Please enter a server name.", "Validation Error",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                serverNameTextBox.Focus();
                return;
            }

            // Create the connection object
            Connection = new PIServerConnection
            {
                ServerName = serverNameTextBox.Text.Trim(),
                Port = (int)portNumeric!.Value,
                Username = usernameTextBox!.Text.Trim(),
                Password = passwordTextBox!.Text,
                UseWindowsAuthentication = windowsAuthCheckBox!.Checked
            };

            this.DialogResult = DialogResult.OK;
            this.Close();
        }
    }
} 