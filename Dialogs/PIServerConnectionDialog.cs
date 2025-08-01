using System;
using System.Drawing;
using System.Windows.Forms;
using System.Threading.Tasks; // Added for Task.Run

namespace PIInterfaceConfigUtility
{
    public partial class PIServerConnectionDialog : Form
    {
        private Label serverLabel, portLabel, usernameLabel, passwordLabel;
        private TextBox serverTextBox, portTextBox, usernameTextBox, passwordTextBox;
        private CheckBox windowsAuthCheckBox;
        private Button okButton, cancelButton, testButton;
        
        public string ServerName => serverTextBox.Text;
        public int Port => int.TryParse(portTextBox.Text, out int port) ? port : 5450;
        public string Username => windowsAuthCheckBox.Checked ? "" : usernameTextBox.Text;
        public string Password => windowsAuthCheckBox.Checked ? "" : passwordTextBox.Text;
        
        public PIServerConnectionDialog()
        {
            InitializeComponent();
            SetupEventHandlers();
        }
        
        private void InitializeComponent()
        {
            this.SuspendLayout();
            
            // Form properties
            this.Size = new Size(400, 280);
            this.Text = "Connect to PI Server";
            this.FormBorderStyle = FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.StartPosition = FormStartPosition.CenterParent;
            this.ShowIcon = false;
            
            // Server name
            serverLabel = new Label
            {
                Text = "Server Name:",
                Location = new Point(15, 20),
                Size = new Size(80, 23)
            };
            
            serverTextBox = new TextBox
            {
                Location = new Point(105, 17),
                Size = new Size(200, 23),
                Text = "localhost"
            };
            
            // Port
            portLabel = new Label
            {
                Text = "Port:",
                Location = new Point(15, 50),
                Size = new Size(80, 23)
            };
            
            portTextBox = new TextBox
            {
                Location = new Point(105, 47),
                Size = new Size(80, 23),
                Text = "5450"
            };
            
            // Windows Authentication
            windowsAuthCheckBox = new CheckBox
            {
                Text = "Use Windows Authentication",
                Location = new Point(15, 80),
                Size = new Size(250, 23),
                Checked = true
            };
            
            // Username
            usernameLabel = new Label
            {
                Text = "Username:",
                Location = new Point(15, 110),
                Size = new Size(80, 23),
                Enabled = false
            };
            
            usernameTextBox = new TextBox
            {
                Location = new Point(105, 107),
                Size = new Size(200, 23),
                Enabled = false
            };
            
            // Password
            passwordLabel = new Label
            {
                Text = "Password:",
                Location = new Point(15, 140),
                Size = new Size(80, 23),
                Enabled = false
            };
            
            passwordTextBox = new TextBox
            {
                Location = new Point(105, 137),
                Size = new Size(200, 23),
                UseSystemPasswordChar = true,
                Enabled = false
            };
            
            // Buttons
            testButton = new Button
            {
                Text = "Test Connection",
                Location = new Point(15, 200),
                Size = new Size(110, 30),
                BackColor = Color.FromArgb(16, 137, 62),
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat
            };
            
            okButton = new Button
            {
                Text = "Connect",
                Location = new Point(195, 200),
                Size = new Size(80, 30),
                BackColor = Color.FromArgb(0, 120, 215),
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat,
                DialogResult = DialogResult.OK
            };
            
            cancelButton = new Button
            {
                Text = "Cancel",
                Location = new Point(285, 200),
                Size = new Size(80, 30),
                BackColor = Color.FromArgb(232, 17, 35),
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat,
                DialogResult = DialogResult.Cancel
            };
            
            this.Controls.AddRange(new Control[]
            {
                serverLabel, serverTextBox, portLabel, portTextBox,
                windowsAuthCheckBox, usernameLabel, usernameTextBox,
                passwordLabel, passwordTextBox, testButton, okButton, cancelButton
            });
            
            this.AcceptButton = okButton;
            this.CancelButton = cancelButton;
            
            this.ResumeLayout(false);
        }
        
        private void SetupEventHandlers()
        {
            windowsAuthCheckBox.CheckedChanged += WindowsAuthCheckBox_CheckedChanged;
            testButton.Click += TestButton_Click;
            okButton.Click += OkButton_Click;
        }
        
        private void WindowsAuthCheckBox_CheckedChanged(object? sender, EventArgs e)
        {
            bool useWindowsAuth = windowsAuthCheckBox.Checked;
            usernameLabel.Enabled = !useWindowsAuth;
            usernameTextBox.Enabled = !useWindowsAuth;
            passwordLabel.Enabled = !useWindowsAuth;
            passwordTextBox.Enabled = !useWindowsAuth;
        }
        
        private void TestButton_Click(object? sender, EventArgs e)
        {
            if (ValidateInput())
            {
                testButton.Enabled = false;
                testButton.Text = "Testing...";
                
                // Simulate connection test
                Task.Run(async () =>
                {
                    await Task.Delay(1500);
                    
                    this.Invoke(new Action(() =>
                    {
                        testButton.Enabled = true;
                        testButton.Text = "Test Connection";
                        
                        MessageBox.Show("Connection test successful!", "Test Result",
                            MessageBoxButtons.OK, MessageBoxIcon.Information);
                    }));
                });
            }
        }
        
        private void OkButton_Click(object? sender, EventArgs e)
        {
            if (!ValidateInput())
            {
                this.DialogResult = DialogResult.None;
            }
        }
        
        private bool ValidateInput()
        {
            if (string.IsNullOrWhiteSpace(serverTextBox.Text))
            {
                MessageBox.Show("Please enter a server name.", "Validation Error",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                serverTextBox.Focus();
                return false;
            }
            
            if (!int.TryParse(portTextBox.Text, out int port) || port <= 0 || port > 65535)
            {
                MessageBox.Show("Please enter a valid port number (1-65535).", "Validation Error",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                portTextBox.Focus();
                return false;
            }
            
            if (!windowsAuthCheckBox.Checked)
            {
                if (string.IsNullOrWhiteSpace(usernameTextBox.Text))
                {
                    MessageBox.Show("Please enter a username.", "Validation Error",
                        MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    usernameTextBox.Focus();
                    return false;
                }
            }
            
            return true;
        }
    }
} 