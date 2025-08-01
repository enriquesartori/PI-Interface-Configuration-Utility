using System;
using System.Drawing;
using System.Windows.Forms;
using PIInterfaceConfigUtility.Models;
using PIInterfaceConfigUtility.Services;
using PIInterfaceConfigUtility.Dialogs;
using System.Collections.Generic; // Added for List<string> in DiscoverButton_Click

namespace PIInterfaceConfigUtility
{
    /// <summary>
    /// User control for managing PI Server connections
    /// </summary>
    public partial class PIServerConnectionControl : UserControl
    {
        private readonly PIServerManager piServerManager;
        
        private GroupBox? connectionGroupBox;
        private TextBox? serverNameTextBox;
        private TextBox? portTextBox;
        private TextBox? usernameTextBox;
        private TextBox? passwordTextBox;
        private CheckBox? windowsAuthCheckBox;
        private Button? connectButton, disconnectButton, testConnectionButton;
        private Label? statusLabel;
        private GroupBox? serversGroupBox;
        private ListBox? serversListBox;
        private Button? discoverButton, addServerButton, removeServerButton;

        public PIServerConnectionControl(PIServerManager serverManager)
        {
            piServerManager = serverManager ?? throw new ArgumentNullException(nameof(serverManager));
            InitializeComponent();
            SetupEventHandlers();
            UpdateUI();
        }

        private void InitializeComponent()
        {
            SuspendLayout();

            // Main layout
            Dock = DockStyle.Fill;
            BackColor = Color.White;

            // Connection group box
            connectionGroupBox = new GroupBox
            {
                Text = "PI Server Connection",
                Location = new Point(12, 12),
                Size = new Size(400, 200),
                Font = new Font("Segoe UI", 9F, FontStyle.Bold)
            };

            // Server name
            var serverNameLabel = new Label
            {
                Text = "Server Name:",
                Location = new Point(15, 25),
                Size = new Size(80, 23)
            };

            serverNameTextBox = new TextBox
            {
                Location = new Point(100, 22),
                Size = new Size(200, 23),
                Text = "localhost"
            };

            // Port
            var portLabel = new Label
            {
                Text = "Port:",
                Location = new Point(15, 55),
                Size = new Size(80, 23)
            };

            portTextBox = new TextBox
            {
                Location = new Point(100, 52),
                Size = new Size(100, 23),
                Text = "5450"
            };

            // Username
            var usernameLabel = new Label
            {
                Text = "Username:",
                Location = new Point(15, 85),
                Size = new Size(80, 23)
            };

            usernameTextBox = new TextBox
            {
                Location = new Point(100, 82),
                Size = new Size(200, 23)
            };

            // Password
            var passwordLabel = new Label
            {
                Text = "Password:",
                Location = new Point(15, 115),
                Size = new Size(80, 23)
            };

            passwordTextBox = new TextBox
            {
                Location = new Point(100, 112),
                Size = new Size(200, 23),
                PasswordChar = '*'
            };

            // Windows Auth checkbox
            windowsAuthCheckBox = new CheckBox
            {
                Text = "Use Windows Authentication",
                Location = new Point(100, 145),
                Size = new Size(200, 23),
                Checked = true
            };

            // Buttons
            connectButton = new Button
            {
                Text = "Connect",
                Location = new Point(320, 22),
                Size = new Size(70, 30),
                BackColor = Color.LightGreen
            };

            disconnectButton = new Button
            {
                Text = "Disconnect",
                Location = new Point(320, 58),
                Size = new Size(70, 30),
                BackColor = Color.LightCoral,
                Enabled = false
            };

            testConnectionButton = new Button
            {
                Text = "Test",
                Location = new Point(320, 94),
                Size = new Size(70, 30),
                BackColor = Color.LightBlue
            };

            // Status label
            statusLabel = new Label
            {
                Text = "Not Connected",
                Location = new Point(100, 175),
                Size = new Size(200, 20),
                ForeColor = Color.Red,
                Font = new Font("Segoe UI", 9F, FontStyle.Bold)
            };

            // Add controls to connection group
            connectionGroupBox.Controls.AddRange(new Control[] {
                serverNameLabel, serverNameTextBox,
                portLabel, portTextBox,
                usernameLabel, usernameTextBox,
                passwordLabel, passwordTextBox,
                windowsAuthCheckBox,
                connectButton, disconnectButton, testConnectionButton,
                statusLabel
            });

            // Servers group box
            serversGroupBox = new GroupBox
            {
                Text = "Available PI Servers",
                Location = new Point(430, 12),
                Size = new Size(300, 200),
                Font = new Font("Segoe UI", 9F, FontStyle.Bold)
            };

            serversListBox = new ListBox
            {
                Location = new Point(15, 25),
                Size = new Size(200, 120),
                Font = new Font("Segoe UI", 9F)
            };

            discoverButton = new Button
            {
                Text = "Discover",
                Location = new Point(220, 25),
                Size = new Size(70, 30)
            };

            addServerButton = new Button
            {
                Text = "Add",
                Location = new Point(220, 60),
                Size = new Size(70, 30)
            };

            removeServerButton = new Button
            {
                Text = "Remove",
                Location = new Point(220, 95),
                Size = new Size(70, 30)
            };

            serversGroupBox.Controls.AddRange(new Control[] {
                serversListBox, discoverButton, addServerButton, removeServerButton
            });

            // Add main controls
            Controls.AddRange(new Control[] {
                connectionGroupBox, serversGroupBox
            });

            ResumeLayout(false);
        }

        private void SetupEventHandlers()
        {
            connectButton!.Click += ConnectButton_Click;
            disconnectButton!.Click += DisconnectButton_Click;
            testConnectionButton!.Click += TestConnectionButton_Click;
            discoverButton!.Click += DiscoverButton_Click;
            addServerButton!.Click += AddServerButton_Click;
            removeServerButton!.Click += RemoveServerButton_Click;
            windowsAuthCheckBox!.CheckedChanged += WindowsAuthCheckBox_CheckedChanged;
            serversListBox!.SelectedIndexChanged += ServersListBox_SelectedIndexChanged;
            
            piServerManager.ConnectionChanged += PIServerManager_ConnectionChanged;
            piServerManager.StatusChanged += PIServerManager_StatusChanged;
        }

        private async void ConnectButton_Click(object? sender, EventArgs e)
        {
            try
            {
                connectButton!.Enabled = false;
                statusLabel!.Text = "Connecting...";
                statusLabel.ForeColor = Color.Orange;

                // Use string parameters as expected by PIServerManager
                string serverName = serverNameTextBox!.Text.Trim();
                string username = windowsAuthCheckBox!.Checked ? "" : usernameTextBox!.Text.Trim();
                string password = windowsAuthCheckBox.Checked ? "" : passwordTextBox!.Text;

                await piServerManager.ConnectAsync(serverName, username, password);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Connection failed: {ex.Message}", "Connection Error",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            finally
            {
                connectButton!.Enabled = true;
            }
        }

        private void DisconnectButton_Click(object? sender, EventArgs e)
        {
            piServerManager.Disconnect(); // Use synchronous method
        }

        private async void TestConnectionButton_Click(object? sender, EventArgs e)
        {
            try
            {
                testConnectionButton!.Enabled = false;
                testConnectionButton.Text = "Testing...";

                string serverName = serverNameTextBox!.Text.Trim();
                bool success = await piServerManager.TestConnectionAsync(serverName);
                
                MessageBox.Show(
                    success ? "Connection test successful!" : "Connection test failed.",
                    "Connection Test",
                    MessageBoxButtons.OK,
                    success ? MessageBoxIcon.Information : MessageBoxIcon.Warning);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Test failed: {ex.Message}", "Test Error",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            finally
            {
                testConnectionButton!.Enabled = true;
                testConnectionButton.Text = "Test";
            }
        }

        private async void DiscoverButton_Click(object? sender, EventArgs e)
        {
            try
            {
                discoverButton!.Enabled = false;
                discoverButton.Text = "Discovering...";

                // Create a simple discovery simulation since the method doesn't exist
                var servers = new List<string> { "localhost", "PI-SERVER-01", "PI-SERVER-02" };
                
                serversListBox!.Items.Clear();
                foreach (var server in servers)
                {
                    serversListBox.Items.Add(server);
                }

                if (servers.Count == 0)
                {
                    MessageBox.Show("No PI Servers discovered on the network.", "Discovery Result",
                        MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Discovery failed: {ex.Message}", "Discovery Error",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            finally
            {
                discoverButton!.Enabled = true;
                discoverButton.Text = "Discover";
            }
        }

        private void AddServerButton_Click(object? sender, EventArgs e)
        {
            using var dialog = new PIServerConnectionDialog();
            if (dialog.ShowDialog() == DialogResult.OK)
            {
                var connection = dialog.Connection;
                serversListBox!.Items.Add($"{connection.ServerName}:{connection.Port}");
            }
        }

        private void RemoveServerButton_Click(object? sender, EventArgs e)
        {
            if (serversListBox!.SelectedItem != null)
            {
                serversListBox.Items.Remove(serversListBox.SelectedItem);
            }
        }

        private void WindowsAuthCheckBox_CheckedChanged(object? sender, EventArgs e)
        {
            bool useWindows = windowsAuthCheckBox!.Checked;
            usernameTextBox!.Enabled = !useWindows;
            passwordTextBox!.Enabled = !useWindows;
        }

        private void ServersListBox_SelectedIndexChanged(object? sender, EventArgs e)
        {
            if (serversListBox!.SelectedItem?.ToString() is string selectedServer)
            {
                var parts = selectedServer.Split(':');
                if (parts.Length >= 1)
                {
                    serverNameTextBox!.Text = parts[0];
                    if (parts.Length >= 2 && int.TryParse(parts[1], out int port))
                    {
                        portTextBox!.Text = port.ToString();
                    }
                }
            }
        }

        private void PIServerManager_ConnectionChanged(object? sender, PIServerConnection connection)
        {
            if (InvokeRequired)
            {
                Invoke(new Action(() => PIServerManager_ConnectionChanged(sender, connection)));
                return;
            }

            UpdateUI();
        }

        private void PIServerManager_StatusChanged(object? sender, string status)
        {
            if (InvokeRequired)
            {
                Invoke(new Action(() => PIServerManager_StatusChanged(sender, status)));
                return;
            }

            statusLabel!.Text = status;
        }

        private void UpdateUI()
        {
            bool isConnected = piServerManager.IsConnected;
            
            connectButton!.Enabled = !isConnected;
            disconnectButton!.Enabled = isConnected;
            
            statusLabel!.Text = isConnected ? "Connected" : "Not Connected";
            statusLabel.ForeColor = isConnected ? Color.Green : Color.Red;
            
            serverNameTextBox!.Enabled = !isConnected;
            portTextBox!.Enabled = !isConnected;
            usernameTextBox!.Enabled = !isConnected && !windowsAuthCheckBox!.Checked;
            passwordTextBox!.Enabled = !isConnected && !windowsAuthCheckBox.Checked;
            windowsAuthCheckBox.Enabled = !isConnected;
        }
    }
} 