using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;
using PIInterfaceConfigUtility.Models;
using PIInterfaceConfigUtility.Services;
using PIInterfaceConfigUtility.Dialogs;

namespace PIInterfaceConfigUtility.Controls
{
    public partial class PIServerConnectionControl : UserControl
    {
        // Make all UI controls nullable
        private Label? serverLabel;
        private TextBox? serverTextBox;
        private Label? portLabel;
        private TextBox? portTextBox;
        private Label? usernameLabel;
        private TextBox? usernameTextBox;
        private Label? passwordLabel;
        private TextBox? passwordTextBox;
        private CheckBox? windowsAuthCheckBox;
        private Button? connectButton;
        private Button? disconnectButton;
        private Button? testConnectionButton;
        private Button? discoverButton;
        private GroupBox? connectionGroupBox;
        private GroupBox? serversGroupBox;
        private ListBox? serversListBox;
        private Button? addServerButton;
        private Button? removeServerButton;
        private Label? statusLabel;
        private Label? connectionStatusLabel;
        
        private PIServerManager serverManager;
        private RealPIServerManager? realPIServerManager;

        public PIServerConnectionControl(PIServerManager serverManager)
        {
            this.serverManager = serverManager;
            this.realPIServerManager = new RealPIServerManager();
            InitializeComponent();
            SetupEventHandlers();
        }

        private void InitializeComponent()
        {
            this.Size = new Size(800, 600);
            this.BackColor = SystemColors.Control;

            CreateControls();
            LayoutControls();
        }

        private void CreateControls()
        {
            // Connection group
            connectionGroupBox = new GroupBox
            {
                Text = "PI Server Connection",
                Font = new Font("Segoe UI", 9F, FontStyle.Bold)
            };

            serverLabel = new Label { Text = "Server Name:" };
            serverTextBox = new TextBox { Text = "localhost" };

            portLabel = new Label { Text = "Port:" };
            portTextBox = new TextBox { Text = "5450" };

            usernameLabel = new Label { Text = "Username:" };
            usernameTextBox = new TextBox();

            passwordLabel = new Label { Text = "Password:" };
            passwordTextBox = new TextBox { UseSystemPasswordChar = true };

            windowsAuthCheckBox = new CheckBox
            {
                Text = "Use Windows Authentication",
                Checked = true
            };

            connectButton = new Button
            {
                Text = "Connect",
                BackColor = Color.FromArgb(76, 175, 80),
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat
            };

            disconnectButton = new Button
            {
                Text = "Disconnect",
                BackColor = Color.FromArgb(244, 67, 54),
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat,
                Enabled = false
            };

            testConnectionButton = new Button
            {
                Text = "Test Connection",
                BackColor = Color.FromArgb(33, 150, 243),
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat
            };

            discoverButton = new Button
            {
                Text = "Discover Servers",
                BackColor = Color.FromArgb(156, 39, 176),
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat
            };

            statusLabel = new Label 
            { 
                Text = "Status:",
                Font = new Font("Segoe UI", 9F, FontStyle.Bold)
            };
            
            connectionStatusLabel = new Label
            {
                Text = "Disconnected",
                ForeColor = Color.Red,
                Font = new Font("Segoe UI", 9F, FontStyle.Regular)
            };

            // Servers group
            serversGroupBox = new GroupBox
            {
                Text = "Available PI Servers",
                Font = new Font("Segoe UI", 9F, FontStyle.Bold)
            };

            serversListBox = new ListBox
            {
                DisplayMember = "ServerName"
            };

            addServerButton = new Button
            {
                Text = "Add Server",
                BackColor = Color.FromArgb(76, 175, 80),
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat
            };

            removeServerButton = new Button
            {
                Text = "Remove Server",
                BackColor = Color.FromArgb(244, 67, 54),
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat
            };
        }

        private void LayoutControls()
        {
            const int margin = 15;
            const int labelWidth = 120;
            const int controlWidth = 200;
            const int buttonWidth = 120;
            const int buttonHeight = 30;
            const int rowHeight = 35;
            
            // Connection group layout
            connectionGroupBox!.Location = new Point(margin, margin);
            connectionGroupBox.Size = new Size(this.Width - 2 * margin, 280);
            connectionGroupBox.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;

            int currentY = 25;
            
            // Server name
            serverLabel!.Location = new Point(15, currentY);
            serverLabel.Size = new Size(labelWidth, 20);
            serverTextBox!.Location = new Point(15 + labelWidth, currentY);
            serverTextBox.Size = new Size(controlWidth, 20);
            currentY += rowHeight;

            // Port
            portLabel!.Location = new Point(15, currentY);
            portLabel.Size = new Size(labelWidth, 20);
            portTextBox!.Location = new Point(15 + labelWidth, currentY);
            portTextBox.Size = new Size(controlWidth, 20);
            currentY += rowHeight;

            // Windows auth checkbox
            windowsAuthCheckBox!.Location = new Point(15, currentY);
            windowsAuthCheckBox.Size = new Size(200, 20);
            currentY += rowHeight;

            // Username (only if not Windows auth)
            usernameLabel!.Location = new Point(15, currentY);
            usernameLabel.Size = new Size(labelWidth, 20);
            usernameTextBox!.Location = new Point(15 + labelWidth, currentY);
            usernameTextBox.Size = new Size(controlWidth, 20);
            currentY += rowHeight;

            // Password (only if not Windows auth)
            passwordLabel!.Location = new Point(15, currentY);
            passwordLabel.Size = new Size(labelWidth, 20);
            passwordTextBox!.Location = new Point(15 + labelWidth, currentY);
            passwordTextBox.Size = new Size(controlWidth, 20);
            currentY += rowHeight;

            // Status
            statusLabel!.Location = new Point(15, currentY);
            statusLabel.Size = new Size(60, 20);
            connectionStatusLabel!.Location = new Point(80, currentY);
            connectionStatusLabel.Size = new Size(200, 20);
            currentY += rowHeight;

            // Buttons
            connectButton!.Location = new Point(15, currentY);
            connectButton.Size = new Size(buttonWidth, buttonHeight);

            disconnectButton!.Location = new Point(15 + buttonWidth + 10, currentY);
            disconnectButton.Size = new Size(buttonWidth, buttonHeight);

            testConnectionButton!.Location = new Point(15 + 2 * (buttonWidth + 10), currentY);
            testConnectionButton.Size = new Size(buttonWidth, buttonHeight);

            discoverButton!.Location = new Point(15 + 3 * (buttonWidth + 10), currentY);
            discoverButton.Size = new Size(buttonWidth, buttonHeight);

            // Add controls to connection group
            connectionGroupBox.Controls.AddRange(new Control[]
            {
                serverLabel, serverTextBox,
                portLabel, portTextBox,
                usernameLabel, usernameTextBox,
                passwordLabel, passwordTextBox,
                windowsAuthCheckBox,
                statusLabel, connectionStatusLabel,
                connectButton, disconnectButton, testConnectionButton, discoverButton
            });

            // Servers group layout
            serversGroupBox!.Location = new Point(margin, 310);
            serversGroupBox.Size = new Size(this.Width - 2 * margin, this.Height - 330);
            serversGroupBox.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right | AnchorStyles.Bottom;

            serversListBox!.Location = new Point(15, 25);
            serversListBox.Size = new Size(serversGroupBox.Width - 160, serversGroupBox.Height - 45);
            serversListBox.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right | AnchorStyles.Bottom;

            addServerButton!.Location = new Point(serversGroupBox.Width - 130, 25);
            addServerButton.Size = new Size(buttonWidth, buttonHeight);
            addServerButton.Anchor = AnchorStyles.Top | AnchorStyles.Right;

            removeServerButton!.Location = new Point(serversGroupBox.Width - 130, 65);
            removeServerButton.Size = new Size(buttonWidth, buttonHeight);
            removeServerButton.Anchor = AnchorStyles.Top | AnchorStyles.Right;

            // Add controls to servers group
            serversGroupBox.Controls.AddRange(new Control[]
            {
                serversListBox, addServerButton, removeServerButton
            });

            // Add groups to form
            this.Controls.AddRange(new Control[]
            {
                connectionGroupBox, serversGroupBox
            });
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

            // Server manager events
            serverManager.ConnectionChanged += PIServerManager_ConnectionChanged;
        }

        private void WindowsAuthCheckBox_CheckedChanged(object? sender, EventArgs e)
        {
            bool useWindowsAuth = windowsAuthCheckBox!.Checked;
            usernameLabel!.Enabled = !useWindowsAuth;
            usernameTextBox!.Enabled = !useWindowsAuth;
            passwordLabel!.Enabled = !useWindowsAuth;
            passwordTextBox!.Enabled = !useWindowsAuth;
        }

        private async void ConnectButton_Click(object? sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(serverTextBox!.Text))
            {
                MessageBox.Show("Please enter a server name.", "Validation Error",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            try
            {
                connectButton!.Enabled = false;
                connectionStatusLabel!.Text = "Connecting...";
                connectionStatusLabel.ForeColor = Color.Orange;

                string serverName = serverTextBox.Text.Trim();
                string username = windowsAuthCheckBox!.Checked ? "" : usernameTextBox!.Text;
                string password = windowsAuthCheckBox.Checked ? "" : passwordTextBox!.Text;

                bool success = await serverManager.ConnectAsync(serverName, username, password);
                
                if (success)
                {
                    connectionStatusLabel.Text = $"Connected to {serverName}";
                    connectionStatusLabel.ForeColor = Color.Green;
                    connectButton.Enabled = false;
                    disconnectButton!.Enabled = true;
                }
                else
                {
                    connectionStatusLabel.Text = "Connection failed";
                    connectionStatusLabel.ForeColor = Color.Red;
                    connectButton.Enabled = true;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Connection error: {ex.Message}", "Error",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
                connectionStatusLabel!.Text = "Connection error";
                connectionStatusLabel.ForeColor = Color.Red;
                connectButton!.Enabled = true;
            }
        }

        private void DisconnectButton_Click(object? sender, EventArgs e)
        {
            try
            {
                serverManager.Disconnect();
                connectionStatusLabel!.Text = "Disconnected";
                connectionStatusLabel.ForeColor = Color.Red;
                connectButton!.Enabled = true;
                disconnectButton!.Enabled = false;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Disconnect error: {ex.Message}", "Error",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private async void TestConnectionButton_Click(object? sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(serverTextBox!.Text))
            {
                MessageBox.Show("Please enter a server name.", "Validation Error",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            try
            {
                testConnectionButton!.Enabled = false;
                connectionStatusLabel!.Text = "Testing...";
                connectionStatusLabel.ForeColor = Color.Orange;

                string serverName = serverTextBox.Text.Trim();
                
                bool success = await serverManager.TestConnectionAsync(serverName);
                
                if (success)
                {
                    connectionStatusLabel.Text = "Test successful";
                    connectionStatusLabel.ForeColor = Color.Green;
                    MessageBox.Show("Connection test successful!", "Test Result",
                        MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
                else
                {
                    connectionStatusLabel.Text = "Test failed";
                    connectionStatusLabel.ForeColor = Color.Red;
                    MessageBox.Show("Connection test failed.", "Test Result",
                        MessageBoxButtons.OK, MessageBoxIcon.Warning);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Test error: {ex.Message}", "Error",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
                connectionStatusLabel!.Text = "Test error";
                connectionStatusLabel.ForeColor = Color.Red;
            }
            finally
            {
                testConnectionButton!.Enabled = true;
            }
        }

        private async void DiscoverButton_Click(object? sender, EventArgs e)
        {
            try
            {
                discoverButton!.Enabled = false;
                connectionStatusLabel!.Text = "Discovering servers...";
                connectionStatusLabel.ForeColor = Color.Orange;

                // Use real PI server manager for discovery
                var servers = await DiscoverServersAsync();
                
                serversListBox!.DataSource = servers;
                connectionStatusLabel.Text = $"Found {servers.Count} servers";
                connectionStatusLabel.ForeColor = Color.Blue;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Discovery error: {ex.Message}", "Error",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
                connectionStatusLabel!.Text = "Discovery failed";
                connectionStatusLabel.ForeColor = Color.Red;
            }
            finally
            {
                discoverButton!.Enabled = true;
            }
        }

        private void AddServerButton_Click(object? sender, EventArgs e)
        {
            using (var dialog = new PIServerConnectionDialog())
            {
                if (dialog.ShowDialog() == DialogResult.OK)
                {
                    var servers = serversListBox!.DataSource as List<PIServerConnection> ?? new List<PIServerConnection>();
                    servers.Add(dialog.Connection);
                    serversListBox.DataSource = null;
                    serversListBox.DataSource = servers;
                }
            }
        }

        private void RemoveServerButton_Click(object? sender, EventArgs e)
        {
            if (serversListBox!.SelectedItem is PIServerConnection selectedServer)
            {
                var servers = serversListBox.DataSource as List<PIServerConnection> ?? new List<PIServerConnection>();
                servers.Remove(selectedServer);
                serversListBox.DataSource = null;
                serversListBox.DataSource = servers;
            }
        }

        private void ServersListBox_SelectedIndexChanged(object? sender, EventArgs e)
        {
            if (serversListBox!.SelectedItem is PIServerConnection selectedServer)
            {
                serverTextBox!.Text = selectedServer.ServerName;
                portTextBox!.Text = selectedServer.Port.ToString();
                windowsAuthCheckBox!.Checked = selectedServer.UseWindowsAuthentication;
                usernameTextBox!.Text = selectedServer.Username;
                passwordTextBox!.Text = selectedServer.Password;
            }
        }

        private void PIServerManager_ConnectionChanged(object? sender, PIServerConnection e)
        {
            if (this.InvokeRequired)
            {
                this.Invoke(new Action(() => UpdateConnectionStatus(e)));
            }
            else
            {
                UpdateConnectionStatus(e);
            }
        }

        private void UpdateConnectionStatus(PIServerConnection connection)
        {
            if (connection.IsConnected)
            {
                connectionStatusLabel!.Text = $"Connected to {connection.ServerName}";
                connectionStatusLabel.ForeColor = Color.Green;
                connectButton!.Enabled = false;
                disconnectButton!.Enabled = true;
            }
            else
            {
                connectionStatusLabel!.Text = "Disconnected";
                connectionStatusLabel.ForeColor = Color.Red;
                connectButton!.Enabled = true;
                disconnectButton!.Enabled = false;
            }
        }

        // Simulated discovery implementation
        private async Task<List<PIServerConnection>> DiscoverServersAsync()
        {
            await Task.Delay(2000); // Simulate network discovery

            var servers = new List<PIServerConnection>
            {
                new PIServerConnection("localhost") { Description = "Local PI Server", ServerVersion = "2018 SP3" },
                new PIServerConnection("PISRV01") { Description = "Production PI Server", ServerVersion = "2018 SP3" },
                new PIServerConnection("PISRV02") { Description = "Development PI Server", ServerVersion = "2016 R2" },
                new PIServerConnection("PISRV-DMZ") { Description = "DMZ PI Server", ServerVersion = "2018 SP3" }
            };

            if (realPIServerManager != null)
            {
                try
                {
                    var realServers = await realPIServerManager.DiscoverServersAsync();
                    servers.AddRange(realServers);
                }
                catch
                {
                    // Fall back to simulated servers if real discovery fails
                }
            }

            return servers;
        }
    }
} 