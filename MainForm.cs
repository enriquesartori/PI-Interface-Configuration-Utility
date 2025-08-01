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
            serverConnectionControl = new PIServerConnectionControl();
            serverConnectionControl.Dock = DockStyle.Fill;
            serverConnectionTab.Controls.Add(serverConnectionControl);
            mainTabControl.TabPages.Add(serverConnectionTab);

            // PI Points Tab
            piPointsTab = new TabPage("PI Points Management");
            piPointsControl = new PIPointsControl();
            piPointsControl.Dock = DockStyle.Fill;
            piPointsTab.Controls.Add(piPointsControl);
            mainTabControl.TabPages.Add(piPointsTab);

            // Service Management Tab
            serviceManagementTab = new TabPage("Interface Services");
            serviceManagementControl = new ServiceManagementControl();
            serviceManagementControl.Dock = DockStyle.Fill;
            serviceManagementTab.Controls.Add(serviceManagementControl);
            mainTabControl.TabPages.Add(serviceManagementTab);

            // Diagnostics Tab
            diagnosticsTab = new TabPage("System Diagnostics");
            diagnosticsControl = new DiagnosticsControl();
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
            statusLabel = new ToolStripStatusLabel("Ready");
            progressBar = new ToolStripProgressBar();
            progressBar.Visible = false;
            
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
            
            // Also update server connection control to use real manager
            if (serverConnectionControl != null && realPIServerManager != null)
            {
                // Server connection control will use both real and simulated managers
            }
        }
        
        private void ApplyModernTheme()
        {
            this.BackColor = Color.FromArgb(240, 240, 240);
            menuStrip.BackColor = Color.White;
            mainTabControl.BackColor = Color.White;
            statusStrip.BackColor = Color.FromArgb(240, 240, 240);
        }
        
        private void UpdateStatus(string message)
        {
            statusLabel.Text = message;
            statusStrip.Refresh();
        }
        
        private void ShowProgress(bool show)
        {
            progressBar!.Visible = show;
            if (show)
            {
                progressBar.Style = ProgressBarStyle.Marquee;
            }
        }
        
        // Event Handlers
        private void NewConfiguration_Click(object sender, EventArgs e)
        {
            if (MessageBox.Show("Create a new configuration? Any unsaved changes will be lost.", 
                "New Configuration", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
            {
                configurationManager.NewConfiguration();
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
                        configurationManager.LoadConfiguration(openDialog.FileName);
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
                configurationManager.SaveConfiguration();
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
                        configurationManager.SaveConfigurationAs(saveDialog.FileName);
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
                        configurationManager.ImportConfiguration(importDialog.FileName);
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
                        configurationManager.ExportConfiguration(exportDialog.FileName);
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