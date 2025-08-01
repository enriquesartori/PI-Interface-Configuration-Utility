using System;
using System.Drawing;
using System.Windows.Forms;
using PIInterfaceConfigUtility.Controls;
using PIInterfaceConfigUtility.Services;
using PIInterfaceConfigUtility.Dialogs;

namespace PIInterfaceConfigUtility
{
    public partial class MainForm : Form
    {
        private MenuStrip? menuStrip;
        private ToolStripMenuItem? fileMenuItem;
        private ToolStripMenuItem? helpMenuItem;
        
        private TabControl? mainTabControl;
        
        // Tab pages - emphasizing the authentic PI ICU as main interface
        private TabPage? authenticPIICUTab;
        private TabPage? serverConnectionTab;
        private TabPage? piPointsTab;
        private TabPage? serviceManagementTab;
        private TabPage? diagnosticsTab;
        private TabPage? logsTab;
        
        // Controls for each tab
        private AuthenticPIICUControl? authenticPIICUControl;
        private PIServerConnectionControl? serverConnectionControl;
        private PIPointsControl? piPointsControl;
        private ServiceManagementControl? serviceManagementControl;
        private DiagnosticsControl? diagnosticsControl;
        private LogsViewerControl? logsViewerControl;
        
        // Services - real PI connectivity
        private RealPIServerManager? realPIServerManager;
        private PIServerManager? piServerManager; // Keep for backward compatibility
        private InterfaceManager? interfaceManager;
        private ConfigurationManager? configurationManager;

        // Status and progress
        private StatusStrip? statusStrip;
        private ToolStripStatusLabel? statusLabel;
        private ToolStripProgressBar? progressBar;

        public MainForm()
        {
            InitializeComponent();
            InitializeServices();
            CreateMenuStrip();
            CreateTabControl();
            CreateTabPages();
            CreateStatusStrip();
            WireUpEventHandlers();
        }

        private void InitializeComponent()
        {
            this.Text = "PI Interface Configuration Utility";
            this.Size = new Size(1000, 700);
            this.StartPosition = FormStartPosition.CenterScreen;
            this.Icon = SystemIcons.Application;
        }

        private void InitializeServices()
        {
            // Initialize real PI server manager for authentic connectivity
            realPIServerManager = new RealPIServerManager();
            piServerManager = new PIServerManager(); // Keep for compatibility with existing controls
            interfaceManager = new InterfaceManager();
            configurationManager = new ConfigurationManager();
            
            // Set up real PI server manager events for main form feedback
            realPIServerManager.ConnectionChanged += (s, conn) =>
            {
                if (statusLabel != null)
                {
                    statusLabel.Text = conn.IsConnected ? 
                        $"✓ Connected to {conn.ServerName} (Real PI System)" : 
                        $"Disconnected from {conn.ServerName}";
                }
            };
            
            realPIServerManager.StatusChanged += (s, status) =>
            {
                if (statusLabel != null)
                {
                    statusLabel.Text = status;
                }
            };
        }

        private void CreateMenuStrip()
        {
            menuStrip = new MenuStrip();
            
            // File Menu
            fileMenuItem = new ToolStripMenuItem("&File");
            fileMenuItem.DropDownItems.AddRange(new ToolStripItem[]
            {
                new ToolStripMenuItem("&New Configuration", null, NewConfiguration_Click),
                new ToolStripMenuItem("&Open Configuration...", null, OpenConfiguration_Click),
                new ToolStripMenuItem("&Save Configuration", null, SaveConfiguration_Click),
                new ToolStripMenuItem("Save Configuration &As...", null, SaveConfigurationAs_Click),
                new ToolStripSeparator(),
                new ToolStripMenuItem("&Import Configuration...", null, ImportConfiguration_Click),
                new ToolStripMenuItem("&Export Configuration...", null, ExportConfiguration_Click),
                new ToolStripSeparator(),
                new ToolStripMenuItem("E&xit", null, (s, e) => this.Close())
            });
            
            var toolsMenu = new ToolStripMenuItem("&Tools");
            toolsMenu.DropDownItems.AddRange(new ToolStripItem[]
            {
                new ToolStripMenuItem("&Connect to PI Server", null, (s, e) => ConnectToPIServer()),
                new ToolStripMenuItem("&Disconnect from PI Server", null, (s, e) => DisconnectFromPIServer()),
                new ToolStripSeparator(),
                new ToolStripMenuItem("&Start All Interfaces", null, (s, e) => StartAllInterfaces()),
                new ToolStripMenuItem("S&top All Interfaces", null, (s, e) => StopAllInterfaces())
            });
            
            // Help Menu
            helpMenuItem = new ToolStripMenuItem("&Help");
            helpMenuItem.DropDownItems.AddRange(new ToolStripItem[]
            {
                new ToolStripMenuItem("&About", null, ShowAbout_Click)
            });
            
            menuStrip.Items.AddRange(new ToolStripItem[] { fileMenuItem, toolsMenu, helpMenuItem });
            this.MainMenuStrip = menuStrip;
            this.Controls.Add(menuStrip);
        }
        
        private void CreateTabControl()
        {
            mainTabControl = new TabControl();
            mainTabControl.Dock = DockStyle.Fill;
            mainTabControl.Font = new Font("Segoe UI", 9F);
            
            this.Controls.Add(mainTabControl);
        }
        
        private void CreateTabPages()
        {
            // Authentic PI ICU Tab - Main Interface (matching real PI ICU)
            authenticPIICUTab = new TabPage("PI Interface Configuration Utility");
            authenticPIICUControl = new AuthenticPIICUControl();
            authenticPIICUControl.Dock = DockStyle.Fill;
            authenticPIICUTab.Controls.Add(authenticPIICUControl);
            mainTabControl!.TabPages.Add(authenticPIICUTab);

            // Server Connection Tab
            serverConnectionTab = new TabPage("Server Connection & Discovery");
            serverConnectionControl = new PIServerConnectionControl(piServerManager!);
            serverConnectionControl.Dock = DockStyle.Fill;
            serverConnectionTab.Controls.Add(serverConnectionControl);
            mainTabControl.TabPages.Add(serverConnectionTab);

            // PI Points Tab
            piPointsTab = new TabPage("PI Points Management");
            piPointsControl = new PIPointsControl(piServerManager!);
            piPointsControl.Dock = DockStyle.Fill;
            piPointsTab.Controls.Add(piPointsControl);
            mainTabControl.TabPages.Add(piPointsTab);

            // Service Management Tab
            serviceManagementTab = new TabPage("Interface Services");
            serviceManagementControl = new ServiceManagementControl(interfaceManager!);
            serviceManagementControl.Dock = DockStyle.Fill;
            serviceManagementTab.Controls.Add(serviceManagementControl);
            mainTabControl.TabPages.Add(serviceManagementTab);

            // Diagnostics Tab
            diagnosticsTab = new TabPage("System Diagnostics");
            diagnosticsControl = new DiagnosticsControl(piServerManager!, interfaceManager!);
            diagnosticsControl.Dock = DockStyle.Fill;
            diagnosticsTab.Controls.Add(diagnosticsControl);
            mainTabControl.TabPages.Add(diagnosticsTab);

            // Logs Tab
            logsTab = new TabPage("Interface Logs");
            logsViewerControl = new LogsViewerControl();
            logsViewerControl.Dock = DockStyle.Fill;
            logsTab.Controls.Add(logsViewerControl);
            mainTabControl.TabPages.Add(logsTab);
        }
        
        private void CreateStatusStrip()
        {
            statusStrip = new StatusStrip();
            
            statusLabel = new ToolStripStatusLabel("Ready")
            {
                Spring = true,
                TextAlign = ContentAlignment.MiddleLeft
            };
            
            progressBar = new ToolStripProgressBar()
            {
                Style = ProgressBarStyle.Marquee,
                Visible = false
            };
            
            statusStrip.Items.AddRange(new ToolStripItem[] { statusLabel, progressBar });
            this.Controls.Add(statusStrip);
        }
        
        private void WireUpEventHandlers()
        {
            // Wire up compatibility service events
            if (piServerManager != null)
            {
                piServerManager.ConnectionChanged += (s, e) =>
                {
                    if (statusLabel != null && !realPIServerManager!.IsConnected)
                    {
                        statusLabel.Text = piServerManager.IsConnected ? 
                            "⚠ Connected (Simulation Mode)" : "Disconnected";
                    }
                };
            }

            if (interfaceManager != null)
            {
                interfaceManager.InterfaceStatusChanged += (s, e) =>
                {
                    if (statusLabel != null)
                    {
                        statusLabel.Text = $"Interface {e.Name} status: {e.Status}";
                    }
                };
            }
            
            // Connect the authentic PI ICU control to the real PI server manager
            if (authenticPIICUControl != null && realPIServerManager != null)
            {
                authenticPIICUControl.SetPIServerManager(realPIServerManager);
            }
        }

        // PI Server Connection Methods
        private async void ConnectToPIServer()
        {
            var connectionDialog = new PIServerConnectionDialog();
            if (connectionDialog.ShowDialog() == DialogResult.OK)
            {
                ShowProgress(true);
                UpdateStatus("Connecting to PI Server...");
                
                try
                {
                    // Try real PI server connection first
                    bool realSuccess = await realPIServerManager!.ConnectAsync(connectionDialog.Connection.ServerName, 
                        connectionDialog.Connection.Username, connectionDialog.Connection.Password);
                        
                    // Also connect with simulated manager for compatibility
                    await piServerManager!.ConnectAsync(connectionDialog.Connection.ServerName, 
                        connectionDialog.Connection.Username, connectionDialog.Connection.Password);
                    
                    if (realSuccess)
                    {
                        UpdateStatus($"✓ Connected to PI Server: {connectionDialog.Connection.ServerName} (Real PI System)");
                    }
                    else
                    {
                        UpdateStatus($"⚠ Connected to PI Server: {connectionDialog.Connection.ServerName} (Simulation Mode)");
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Connection failed: {ex.Message}", "Connection Error", 
                        MessageBoxButtons.OK, MessageBoxIcon.Error);
                    UpdateStatus("Connection failed.");
                }
                finally
                {
                    ShowProgress(false);
                }
            }
        }

        private void DisconnectFromPIServer()
        {
            realPIServerManager?.Disconnect();
            piServerManager?.Disconnect();
            UpdateStatus("Disconnected from PI Server.");
        }

        private async void StartAllInterfaces()
        {
            ShowProgress(true);
            UpdateStatus("Starting all interfaces...");
            
            try
            {
                await interfaceManager!.StartAllInterfacesAsync();
                UpdateStatus("All interfaces started.");
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error starting interfaces: {ex.Message}", "Error", 
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
                UpdateStatus("Failed to start all interfaces.");
            }
            finally
            {
                ShowProgress(false);
            }
        }

        private async void StopAllInterfaces()
        {
            ShowProgress(true);
            UpdateStatus("Stopping all interfaces...");
            
            try
            {
                await interfaceManager!.StopAllInterfacesAsync();
                UpdateStatus("All interfaces stopped.");
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error stopping interfaces: {ex.Message}", "Error", 
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
                UpdateStatus("Failed to stop all interfaces.");
            }
            finally
            {
                ShowProgress(false);
            }
        }

        // Utility Methods
        private void UpdateStatus(string message)
        {
            if (statusLabel != null)
            {
                statusLabel.Text = message;
                Application.DoEvents();
            }
        }

        private void ShowProgress(bool show)
        {
            if (progressBar != null)
            {
                progressBar.Visible = show;
            }
        }

        // File Menu Event Handlers
        private void NewConfiguration_Click(object? sender, EventArgs e)
        {
            if (MessageBox.Show("Are you sure you want to create a new configuration? Any unsaved changes will be lost.",
                "New Configuration", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
            {
                configurationManager!.NewConfiguration();
                UpdateStatus("New configuration created.");
            }
        }

        private void OpenConfiguration_Click(object? sender, EventArgs e)
        {
            using var openDialog = new OpenFileDialog
            {
                Filter = "Configuration Files (*.json)|*.json|All Files (*.*)|*.*",
                Title = "Open Configuration"
            };

            if (openDialog.ShowDialog() == DialogResult.OK)
            {
                try
                {
                    configurationManager!.LoadConfiguration(openDialog.FileName);
                    UpdateStatus($"Configuration loaded: {openDialog.FileName}");
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Error loading configuration: {ex.Message}", "Load Error",
                        MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }

        private void SaveConfiguration_Click(object? sender, EventArgs e)
        {
            try
            {
                configurationManager!.SaveConfiguration();
                UpdateStatus("Configuration saved.");
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error saving configuration: {ex.Message}", "Save Error",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void SaveConfigurationAs_Click(object? sender, EventArgs e)
        {
            using var saveDialog = new SaveFileDialog
            {
                Filter = "Configuration Files (*.json)|*.json|All Files (*.*)|*.*",
                Title = "Save Configuration As"
            };

            if (saveDialog.ShowDialog() == DialogResult.OK)
            {
                try
                {
                    configurationManager!.SaveConfigurationAs(saveDialog.FileName);
                    UpdateStatus($"Configuration saved as: {saveDialog.FileName}");
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Error saving configuration: {ex.Message}", "Save Error",
                        MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }

        private void ImportConfiguration_Click(object? sender, EventArgs e)
        {
            using var importDialog = new OpenFileDialog
            {
                Filter = "All Supported Files (*.json;*.xml;*.csv)|*.json;*.xml;*.csv|JSON Files (*.json)|*.json|XML Files (*.xml)|*.xml|CSV Files (*.csv)|*.csv|All Files (*.*)|*.*",
                Title = "Import Configuration"
            };

            if (importDialog.ShowDialog() == DialogResult.OK)
            {
                try
                {
                    configurationManager!.ImportConfiguration(importDialog.FileName);
                    UpdateStatus($"Configuration imported: {importDialog.FileName}");
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Error importing configuration: {ex.Message}", "Import Error",
                        MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }

        private void ExportConfiguration_Click(object? sender, EventArgs e)
        {
            using var exportDialog = new SaveFileDialog
            {
                Filter = "JSON Files (*.json)|*.json|XML Files (*.xml)|*.xml|CSV Files (*.csv)|*.csv|All Files (*.*)|*.*",
                Title = "Export Configuration"
            };

            if (exportDialog.ShowDialog() == DialogResult.OK)
            {
                try
                {
                    configurationManager!.ExportConfiguration(exportDialog.FileName);
                    UpdateStatus($"Configuration exported: {exportDialog.FileName}");
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Error exporting configuration: {ex.Message}", "Export Error",
                        MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }

        private void ShowAbout_Click(object? sender, EventArgs e)
        {
            using var aboutDialog = new AboutDialog();
            aboutDialog.ShowDialog();
        }
    }
} 