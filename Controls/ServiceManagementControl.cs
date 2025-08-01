using System;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using PIInterfaceConfigUtility.Services;
using System.Threading.Tasks; // Added missing import for Task

namespace PIInterfaceConfigUtility
{
    /// <summary>
    /// User control for managing Windows services related to PI interfaces
    /// </summary>
    public partial class ServiceManagementControl : UserControl
    {
        private readonly InterfaceManager interfaceManager;
        
        private DataGridView servicesGrid;
        private GroupBox actionsGroupBox;
        private Button startButton, stopButton, restartButton, refreshButton;
        private Button startAllButton, stopAllButton;
        private Label statusLabel;
        private ProgressBar operationProgressBar;

        public ServiceManagementControl(InterfaceManager manager)
        {
            interfaceManager = manager ?? throw new ArgumentNullException(nameof(manager));
            InitializeComponent();
            SetupEventHandlers();
            RefreshServices();
        }

        private void InitializeComponent()
        {
            SuspendLayout();

            // Main layout
            Dock = DockStyle.Fill;
            BackColor = Color.White;

            // Services grid
            servicesGrid = new DataGridView
            {
                Location = new Point(12, 12),
                Size = new Size(600, 300),
                AutoGenerateColumns = false,
                SelectionMode = DataGridViewSelectionMode.FullRowSelect,
                MultiSelect = false,
                ReadOnly = true,
                AllowUserToAddRows = false,
                AllowUserToDeleteRows = false,
                BorderStyle = BorderStyle.Fixed3D,
                BackgroundColor = Color.White
            };

            // Setup columns
            servicesGrid.Columns.AddRange(new DataGridViewColumn[]
            {
                new DataGridViewTextBoxColumn { Name = "ServiceName", HeaderText = "Service Name", Width = 150, DataPropertyName = "ServiceName" },
                new DataGridViewTextBoxColumn { Name = "DisplayName", HeaderText = "Display Name", Width = 200, DataPropertyName = "Name" },
                new DataGridViewTextBoxColumn { Name = "Status", HeaderText = "Status", Width = 100, DataPropertyName = "Status" },
                new DataGridViewTextBoxColumn { Name = "StartType", HeaderText = "Start Type", Width = 100, DataPropertyName = "AutoStart" },
                new DataGridViewTextBoxColumn { Name = "Description", HeaderText = "Description", Width = 200, DataPropertyName = "Description" }
            });

            // Actions group box
            actionsGroupBox = new GroupBox
            {
                Text = "Service Actions",
                Location = new Point(630, 12),
                Size = new Size(150, 300),
                Font = new Font("Segoe UI", 9F, FontStyle.Bold)
            };

            // Individual service buttons
            startButton = new Button
            {
                Text = "Start Service",
                Location = new Point(15, 30),
                Size = new Size(120, 30),
                BackColor = Color.LightGreen,
                Enabled = false
            };

            stopButton = new Button
            {
                Text = "Stop Service",
                Location = new Point(15, 70),
                Size = new Size(120, 30),
                BackColor = Color.LightCoral,
                Enabled = false
            };

            restartButton = new Button
            {
                Text = "Restart Service",
                Location = new Point(15, 110),
                Size = new Size(120, 30),
                BackColor = Color.LightBlue,
                Enabled = false
            };

            // Batch operations
            startAllButton = new Button
            {
                Text = "Start All",
                Location = new Point(15, 160),
                Size = new Size(120, 30),
                BackColor = Color.PaleGreen
            };

            stopAllButton = new Button
            {
                Text = "Stop All",
                Location = new Point(15, 200),
                Size = new Size(120, 30),
                BackColor = Color.MistyRose
            };

            refreshButton = new Button
            {
                Text = "Refresh",
                Location = new Point(15, 250),
                Size = new Size(120, 30),
                BackColor = Color.LightYellow
            };

            actionsGroupBox.Controls.AddRange(new Control[] {
                startButton, stopButton, restartButton,
                startAllButton, stopAllButton, refreshButton
            });

            // Status area
            statusLabel = new Label
            {
                Text = "Ready",
                Location = new Point(12, 330),
                Size = new Size(400, 20),
                Font = new Font("Segoe UI", 9F, FontStyle.Bold)
            };

            operationProgressBar = new ProgressBar
            {
                Location = new Point(12, 360),
                Size = new Size(600, 20),
                Style = ProgressBarStyle.Continuous,
                Visible = false
            };

            // Add main controls
            Controls.AddRange(new Control[] {
                servicesGrid, actionsGroupBox, statusLabel, operationProgressBar
            });

            ResumeLayout(false);
        }

        private void SetupEventHandlers()
        {
            startButton.Click += StartButton_Click;
            stopButton.Click += StopButton_Click;
            restartButton.Click += RestartButton_Click;
            refreshButton.Click += RefreshButton_Click;
            startAllButton.Click += StartAllButton_Click;
            stopAllButton.Click += StopAllButton_Click;
            
            servicesGrid.SelectionChanged += ServicesGrid_SelectionChanged;
            servicesGrid.CellFormatting += ServicesGrid_CellFormatting;
            
            interfaceManager.InterfaceStatusChanged += InterfaceManager_InterfaceStatusChanged;
        }

        private async void StartButton_Click(object? sender, EventArgs e)
        {
            if (GetSelectedInterface() is var selectedInterface && selectedInterface != null)
            {
                await PerformServiceOperation($"Starting {selectedInterface.Name}...", async () =>
                {
                    await interfaceManager.StartInterfaceAsync(selectedInterface.Id);
                });
            }
        }

        private async void StopButton_Click(object? sender, EventArgs e)
        {
            if (GetSelectedInterface() is var selectedInterface && selectedInterface != null)
            {
                await PerformServiceOperation($"Stopping {selectedInterface.Name}...", async () =>
                {
                    await interfaceManager.StopInterfaceAsync(selectedInterface.Id);
                });
            }
        }

        private async void RestartButton_Click(object? sender, EventArgs e)
        {
            if (GetSelectedInterface() is var selectedInterface && selectedInterface != null)
            {
                await PerformServiceOperation($"Restarting {selectedInterface.Name}...", async () =>
                {
                    await interfaceManager.StopInterfaceAsync(selectedInterface.Id);
                    await Task.Delay(2000); // Wait 2 seconds
                    await interfaceManager.StartInterfaceAsync(selectedInterface.Id);
                });
            }
        }

        private void RefreshButton_Click(object? sender, EventArgs e)
        {
            RefreshServices();
        }

        private async void StartAllButton_Click(object? sender, EventArgs e)
        {
            var interfaces = interfaceManager.GetInterfaces()
                .Where(i => i.Status != Models.InterfaceStatus.Running)
                .ToList();

            if (interfaces.Any())
            {
                await PerformBatchOperation("Starting all interfaces...", interfaces, 
                    async (iface) => await interfaceManager.StartInterfaceAsync(iface.Id));
            }
        }

        private async void StopAllButton_Click(object? sender, EventArgs e)
        {
            var interfaces = interfaceManager.GetInterfaces()
                .Where(i => i.Status == Models.InterfaceStatus.Running)
                .ToList();

            if (interfaces.Any())
            {
                await PerformBatchOperation("Stopping all interfaces...", interfaces,
                    async (iface) => await interfaceManager.StopInterfaceAsync(iface.Id));
            }
        }

        private void ServicesGrid_SelectionChanged(object? sender, EventArgs e)
        {
            var selectedInterface = GetSelectedInterface();
            bool hasSelection = selectedInterface != null;
            
            startButton.Enabled = hasSelection && selectedInterface?.Status != Models.InterfaceStatus.Running;
            stopButton.Enabled = hasSelection && selectedInterface?.Status == Models.InterfaceStatus.Running;
            restartButton.Enabled = hasSelection;
        }

        private void ServicesGrid_CellFormatting(object? sender, DataGridViewCellFormattingEventArgs e)
        {
            if (e.ColumnIndex == servicesGrid.Columns["Status"].Index && e.Value != null)
            {
                var status = e.Value.ToString();
                e.CellStyle.ForeColor = status switch
                {
                    "Running" => Color.Green,
                    "Stopped" => Color.Red,
                    "Starting" => Color.Orange,
                    "Stopping" => Color.Orange,
                    "Error" => Color.DarkRed,
                    _ => Color.Black
                };
                e.CellStyle.Font = new Font(e.CellStyle.Font!, FontStyle.Bold);
            }
        }

        private void InterfaceManager_InterfaceStatusChanged(object? sender, EventArgs e)
        {
            if (InvokeRequired)
            {
                Invoke(new Action(() => RefreshServices()));
                return;
            }
            
            RefreshServices();
        }

        private Models.PIInterface? GetSelectedInterface()
        {
            if (servicesGrid.SelectedRows.Count > 0)
            {
                return servicesGrid.SelectedRows[0].DataBoundItem as Models.PIInterface;
            }
            return null;
        }

        private void RefreshServices()
        {
            var interfaces = interfaceManager.GetInterfaces().ToList();
            
            servicesGrid.DataSource = interfaces;
            
            statusLabel.Text = $"Found {interfaces.Count} PI interfaces";
            statusLabel.ForeColor = Color.Black;
        }

        private async Task PerformServiceOperation(string operationText, Func<Task> operation)
        {
            try
            {
                statusLabel.Text = operationText;
                statusLabel.ForeColor = Color.Blue;
                
                SetButtonsEnabled(false);
                
                await operation();
                
                statusLabel.Text = "Operation completed successfully";
                statusLabel.ForeColor = Color.Green;
                
                RefreshServices();
            }
            catch (Exception ex)
            {
                statusLabel.Text = $"Operation failed: {ex.Message}";
                statusLabel.ForeColor = Color.Red;
                
                MessageBox.Show($"Service operation failed: {ex.Message}", "Service Error",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            finally
            {
                SetButtonsEnabled(true);
            }
        }

        private async Task PerformBatchOperation(string operationText, 
            System.Collections.Generic.List<Models.PIInterface> interfaces, 
            Func<Models.PIInterface, Task> operation)
        {
            try
            {
                statusLabel.Text = operationText;
                statusLabel.ForeColor = Color.Blue;
                
                SetButtonsEnabled(false);
                operationProgressBar.Visible = true;
                operationProgressBar.Maximum = interfaces.Count;
                operationProgressBar.Value = 0;
                
                for (int i = 0; i < interfaces.Count; i++)
                {
                    var iface = interfaces[i];
                    statusLabel.Text = $"{operationText} ({i + 1}/{interfaces.Count}) - {iface.Name}";
                    
                    try
                    {
                        await operation(iface);
                        await Task.Delay(500); // Small delay between operations
                    }
                    catch (Exception ex)
                    {
                        // Log individual failures but continue with batch
                        System.Diagnostics.Debug.WriteLine($"Failed to process {iface.Name}: {ex.Message}");
                    }
                    
                    operationProgressBar.Value = i + 1;
                }
                
                statusLabel.Text = "Batch operation completed";
                statusLabel.ForeColor = Color.Green;
                
                RefreshServices();
            }
            catch (Exception ex)
            {
                statusLabel.Text = $"Batch operation failed: {ex.Message}";
                statusLabel.ForeColor = Color.Red;
                
                MessageBox.Show($"Batch operation failed: {ex.Message}", "Batch Operation Error",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            finally
            {
                SetButtonsEnabled(true);
                operationProgressBar.Visible = false;
            }
        }

        private void SetButtonsEnabled(bool enabled)
        {
            startButton.Enabled = enabled;
            stopButton.Enabled = enabled;
            restartButton.Enabled = enabled;
            startAllButton.Enabled = enabled;
            stopAllButton.Enabled = enabled;
            refreshButton.Enabled = enabled;
        }
    }
} 