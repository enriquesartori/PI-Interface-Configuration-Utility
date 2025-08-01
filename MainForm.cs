using System;
using System.Drawing;
using System.Windows.Forms;
using PIInterfaceConfigUtility.Models;
using PIInterfaceConfigUtility.Services;

namespace PIInterfaceConfigUtility
{
    public partial class MainForm : Form
    {
        private MenuStrip? menuStrip;
        private ToolStrip? toolStrip;
        private StatusStrip? statusStrip;
        private TabControl? mainTabControl;
        private ToolStripStatusLabel? statusLabel;
        private ToolStripProgressBar? progressBar;
        
        private PIServerManager? piServerManager;
        private InterfaceManager? interfaceManager;
        private ConfigurationManager? configManager;
        
        public MainForm()
        {
            InitializeComponent();
            InitializeServices();
            SetupUI();
        }
        
        private void InitializeServices()
        {
            piServerManager = new PIServerManager();
            interfaceManager = new InterfaceManager();
            configManager = new ConfigurationManager();
        }
        
        private void InitializeComponent()
        {
            this.SuspendLayout();
            
            // Form properties
            this.AutoScaleDimensions = new SizeF(7F, 15F);
            this.AutoScaleMode = AutoScaleMode.Font;
            this.ClientSize = new Size(1200, 800);
            this.Name = "MainForm";
            this.Text = "PI Interface Configuration Utility";
            this.WindowState = FormWindowState.Maximized;
            this.StartPosition = FormStartPosition.CenterScreen;
            this.Icon = SystemIcons.Application;
            
            this.ResumeLayout(false);
        }
        
        private void SetupUI()
        {
            SetupMenuStrip();
            SetupToolStrip();
            SetupStatusStrip();
            SetupMainTabControl();
            ApplyModernTheme();
        }
        
        private void SetupMenuStrip()
        {
            menuStrip = new MenuStrip();
            
            // File Menu
            var fileMenu = new ToolStripMenuItem("&File");
            fileMenu.DropDownItems.AddRange(new ToolStripItem[]
            {
                new ToolStripMenuItem("&New Configuration", null, NewConfiguration_Click),
                new ToolStripMenuItem("&Open Configuration", null, OpenConfiguration_Click),
                new ToolStripMenuItem("&Save Configuration", null, SaveConfiguration_Click),
                new ToolStripMenuItem("Save Configuration &As...", null, SaveConfigurationAs_Click),
                new ToolStripSeparator(),
                new ToolStripMenuItem("&Import Configuration", null, ImportConfiguration_Click),
                new ToolStripMenuItem("&Export Configuration", null, ExportConfiguration_Click),
                new ToolStripSeparator(),
                new ToolStripMenuItem("E&xit", null, (s, e) => Application.Exit())
            });
            
            // Tools Menu
            var toolsMenu = new ToolStripMenuItem("&Tools");
            toolsMenu.DropDownItems.AddRange(new ToolStripItem[]
            {
                new ToolStripMenuItem("&Connect to PI Server", null, ConnectToPIServer_Click),
                new ToolStripMenuItem("&Disconnect from PI Server", null, DisconnectFromPIServer_Click),
                new ToolStripSeparator(),
                new ToolStripMenuItem("&Start All Interfaces", null, StartAllInterfaces_Click),
                new ToolStripMenuItem("S&top All Interfaces", null, StopAllInterfaces_Click),
                new ToolStripSeparator(),
                new ToolStripMenuItem("&Diagnostics", null, ShowDiagnostics_Click),
                new ToolStripMenuItem("&Logs Viewer", null, ShowLogsViewer_Click)
            });
            
            // Help Menu
            var helpMenu = new ToolStripMenuItem("&Help");
            helpMenu.DropDownItems.AddRange(new ToolStripItem[]
            {
                new ToolStripMenuItem("&About", null, ShowAbout_Click)
            });
            
            menuStrip.Items.AddRange(new ToolStripItem[] { fileMenu, toolsMenu, helpMenu });
            this.MainMenuStrip = menuStrip;
            this.Controls.Add(menuStrip);
        }
        
        private void SetupToolStrip()
        {
            toolStrip = new ToolStrip();
            toolStrip.ImageScalingSize = new Size(24, 24);
            
            var newButton = new ToolStripButton("New", null, NewConfiguration_Click) { DisplayStyle = ToolStripItemDisplayStyle.ImageAndText };
            var openButton = new ToolStripButton("Open", null, OpenConfiguration_Click) { DisplayStyle = ToolStripItemDisplayStyle.ImageAndText };
            var saveButton = new ToolStripButton("Save", null, SaveConfiguration_Click) { DisplayStyle = ToolStripItemDisplayStyle.ImageAndText };
            var connectButton = new ToolStripButton("Connect", null, ConnectToPIServer_Click) { DisplayStyle = ToolStripItemDisplayStyle.ImageAndText };
            var startButton = new ToolStripButton("Start All", null, StartAllInterfaces_Click) { DisplayStyle = ToolStripItemDisplayStyle.ImageAndText };
            var stopButton = new ToolStripButton("Stop All", null, StopAllInterfaces_Click) { DisplayStyle = ToolStripItemDisplayStyle.ImageAndText };
            
            toolStrip.Items.AddRange(new ToolStripItem[]
            {
                newButton,
                openButton,
                saveButton,
                new ToolStripSeparator(),
                connectButton,
                new ToolStripSeparator(),
                startButton,
                stopButton
            });
            
            this.Controls.Add(toolStrip);
        }
        
        private void SetupStatusStrip()
        {
            statusStrip = new StatusStrip();
            statusLabel = new ToolStripStatusLabel("Ready");
            progressBar = new ToolStripProgressBar();
            progressBar.Visible = false;
            
            statusStrip.Items.AddRange(new ToolStripItem[] { statusLabel, progressBar });
            this.Controls.Add(statusStrip);
        }
        
        private void SetupMainTabControl()
        {
            mainTabControl = new TabControl();
            mainTabControl.Dock = DockStyle.Fill;
            mainTabControl.Font = new Font("Segoe UI", 9F);
            
            // PI Server Connection Tab
            var serverTab = new TabPage("PI Server Connection");
            serverTab.Controls.Add(new PIServerConnectionControl(piServerManager));
            
            // Interface Configuration Tab
            var interfaceTab = new TabPage("Interface Configuration");
            interfaceTab.Controls.Add(new InterfaceConfigurationControl(interfaceManager));
            
            // PI Points Tab
            var pointsTab = new TabPage("PI Points");
            pointsTab.Controls.Add(new PIPointsControl(piServerManager));
            
            // Service Management Tab
            var serviceTab = new TabPage("Service Management");
            serviceTab.Controls.Add(new ServiceManagementControl(interfaceManager));
            
            // Diagnostics Tab
            var diagnosticsTab = new TabPage("Diagnostics");
            diagnosticsTab.Controls.Add(new DiagnosticsControl(piServerManager, interfaceManager));
            
            // Logs Tab
            var logsTab = new TabPage("Logs");
            logsTab.Controls.Add(new LogsViewerControl());
            
            mainTabControl.TabPages.AddRange(new TabPage[]
            {
                serverTab,
                interfaceTab,
                pointsTab,
                serviceTab,
                diagnosticsTab,
                logsTab
            });
            
            this.Controls.Add(mainTabControl);
        }
        
        private void ApplyModernTheme()
        {
            this.BackColor = Color.FromArgb(240, 240, 240);
            menuStrip.BackColor = Color.White;
            toolStrip.BackColor = Color.White;
            statusStrip.BackColor = Color.FromArgb(240, 240, 240);
            mainTabControl.BackColor = Color.White;
        }
        
        private void UpdateStatus(string message)
        {
            statusLabel.Text = message;
            statusStrip.Refresh();
        }
        
        private void ShowProgress(bool show)
        {
            progressBar.Visible = show;
            if (show)
            {
                progressBar.Style = ToolStripProgressBarStyle.Marquee;
            }
        }
        
        // Event Handlers
        private void NewConfiguration_Click(object sender, EventArgs e)
        {
            if (MessageBox.Show("Create a new configuration? Any unsaved changes will be lost.", 
                "New Configuration", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
            {
                configManager.NewConfiguration();
                UpdateStatus("New configuration created.");
            }
        }
        
        private void OpenConfiguration_Click(object sender, EventArgs e)
        {
            using (var openDialog = new OpenFileDialog())
            {
                openDialog.Filter = "PI Config Files (*.piconfig)|*.piconfig|All Files (*.*)|*.*";
                openDialog.Title = "Open Configuration";
                
                if (openDialog.ShowDialog() == DialogResult.OK)
                {
                    try
                    {
                        configManager.LoadConfiguration(openDialog.FileName);
                        UpdateStatus($"Configuration loaded: {openDialog.FileName}");
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show($"Error loading configuration: {ex.Message}", "Error", 
                            MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                }
            }
        }
        
        private void SaveConfiguration_Click(object sender, EventArgs e)
        {
            try
            {
                configManager.SaveConfiguration();
                UpdateStatus("Configuration saved.");
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error saving configuration: {ex.Message}", "Error", 
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
        
        private void SaveConfigurationAs_Click(object sender, EventArgs e)
        {
            using (var saveDialog = new SaveFileDialog())
            {
                saveDialog.Filter = "PI Config Files (*.piconfig)|*.piconfig|All Files (*.*)|*.*";
                saveDialog.Title = "Save Configuration As";
                
                if (saveDialog.ShowDialog() == DialogResult.OK)
                {
                    try
                    {
                        configManager.SaveConfigurationAs(saveDialog.FileName);
                        UpdateStatus($"Configuration saved as: {saveDialog.FileName}");
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show($"Error saving configuration: {ex.Message}", "Error", 
                            MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                }
            }
        }
        
        private void ImportConfiguration_Click(object sender, EventArgs e)
        {
            using (var importDialog = new OpenFileDialog())
            {
                importDialog.Filter = "JSON Files (*.json)|*.json|XML Files (*.xml)|*.xml|All Files (*.*)|*.*";
                importDialog.Title = "Import Configuration";
                
                if (importDialog.ShowDialog() == DialogResult.OK)
                {
                    try
                    {
                        configManager.ImportConfiguration(importDialog.FileName);
                        UpdateStatus($"Configuration imported: {importDialog.FileName}");
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show($"Error importing configuration: {ex.Message}", "Error", 
                            MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                }
            }
        }
        
        private void ExportConfiguration_Click(object sender, EventArgs e)
        {
            using (var exportDialog = new SaveFileDialog())
            {
                exportDialog.Filter = "JSON Files (*.json)|*.json|XML Files (*.xml)|*.xml";
                exportDialog.Title = "Export Configuration";
                
                if (exportDialog.ShowDialog() == DialogResult.OK)
                {
                    try
                    {
                        configManager.ExportConfiguration(exportDialog.FileName);
                        UpdateStatus($"Configuration exported: {exportDialog.FileName}");
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show($"Error exporting configuration: {ex.Message}", "Error", 
                            MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                }
            }
        }
        
        private async void ConnectToPIServer_Click(object sender, EventArgs e)
        {
            var connectionDialog = new PIServerConnectionDialog();
            if (connectionDialog.ShowDialog() == DialogResult.OK)
            {
                ShowProgress(true);
                UpdateStatus("Connecting to PI Server...");
                
                try
                {
                    await piServerManager.ConnectAsync(connectionDialog.ServerName, 
                        connectionDialog.Username, connectionDialog.Password);
                    UpdateStatus($"Connected to PI Server: {connectionDialog.ServerName}");
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
        
        private void DisconnectFromPIServer_Click(object sender, EventArgs e)
        {
            piServerManager.Disconnect();
            UpdateStatus("Disconnected from PI Server.");
        }
        
        private async void StartAllInterfaces_Click(object sender, EventArgs e)
        {
            ShowProgress(true);
            UpdateStatus("Starting all interfaces...");
            
            try
            {
                await interfaceManager.StartAllInterfacesAsync();
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
        
        private async void StopAllInterfaces_Click(object sender, EventArgs e)
        {
            ShowProgress(true);
            UpdateStatus("Stopping all interfaces...");
            
            try
            {
                await interfaceManager.StopAllInterfacesAsync();
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
        
        private void ShowDiagnostics_Click(object sender, EventArgs e)
        {
            mainTabControl.SelectedIndex = 4; // Diagnostics tab
        }
        
        private void ShowLogsViewer_Click(object sender, EventArgs e)
        {
            mainTabControl.SelectedIndex = 5; // Logs tab
        }
        
        private void ShowAbout_Click(object sender, EventArgs e)
        {
            var aboutDialog = new AboutDialog();
            aboutDialog.ShowDialog();
        }
    }
} 