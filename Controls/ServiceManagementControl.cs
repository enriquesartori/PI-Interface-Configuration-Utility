using System;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using PIInterfaceConfigUtility.Models;
using PIInterfaceConfigUtility.Services;
using System.Threading.Tasks; // Added missing import for Task

namespace PIInterfaceConfigUtility.Controls
{
    /// <summary>
    /// User control for managing Windows services related to PI interfaces
    /// </summary>
    public partial class ServiceManagementControl : UserControl
    {
        // Make all UI controls nullable
        private DataGridView? servicesGrid;
        private GroupBox? actionsGroupBox;
        private Button? startButton;
        private Button? stopButton;
        private Button? restartButton;
        private Button? refreshButton;
        
        private InterfaceManager interfaceManager;

        public ServiceManagementControl(InterfaceManager manager)
        {
            interfaceManager = manager;
            InitializeComponent();
            SetupEventHandlers();
            LoadServices();
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
            // Services grid
            servicesGrid = new DataGridView
            {
                AllowUserToAddRows = false,
                AllowUserToDeleteRows = false,
                SelectionMode = DataGridViewSelectionMode.FullRowSelect,
                ReadOnly = true,
                AutoGenerateColumns = false
            };

            // Add columns
            servicesGrid.Columns.AddRange(new DataGridViewColumn[]
            {
                new DataGridViewTextBoxColumn { Name = "Name", HeaderText = "Interface Name", Width = 150 },
                new DataGridViewTextBoxColumn { Name = "ServiceName", HeaderText = "Service Name", Width = 150 },
                new DataGridViewTextBoxColumn { Name = "Status", HeaderText = "Status", Width = 100 },
                new DataGridViewTextBoxColumn { Name = "Type", HeaderText = "Type", Width = 100 },
                new DataGridViewTextBoxColumn { Name = "LastStarted", HeaderText = "Last Started", Width = 150 },
                new DataGridViewTextBoxColumn { Name = "Uptime", HeaderText = "Uptime", Width = 100 }
            });

            // Actions group
            actionsGroupBox = new GroupBox
            {
                Text = "Service Actions",
                Font = new Font("Segoe UI", 9F, FontStyle.Bold)
            };

            startButton = new Button
            {
                Text = "Start Service",
                BackColor = Color.FromArgb(76, 175, 80),
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat
            };

            stopButton = new Button
            {
                Text = "Stop Service",
                BackColor = Color.FromArgb(244, 67, 54),
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat
            };

            restartButton = new Button
            {
                Text = "Restart Service",
                BackColor = Color.FromArgb(255, 152, 0),
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat
            };

            refreshButton = new Button
            {
                Text = "Refresh",
                BackColor = Color.FromArgb(33, 150, 243),
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat
            };
        }

        private void LayoutControls()
        {
            const int margin = 10;
            const int buttonWidth = 120;
            const int buttonHeight = 30;

            // Services grid
            servicesGrid!.Location = new Point(margin, margin);
            servicesGrid.Size = new Size(this.Width - 2 * margin, this.Height - 150);
            servicesGrid.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right | AnchorStyles.Bottom;

            // Actions group
            actionsGroupBox!.Location = new Point(margin, this.Height - 120);
            actionsGroupBox.Size = new Size(this.Width - 2 * margin, 100);
            actionsGroupBox.Anchor = AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;

            // Buttons inside group
            int buttonY = 25;
            startButton!.Location = new Point(15, buttonY);
            startButton.Size = new Size(buttonWidth, buttonHeight);

            stopButton!.Location = new Point(15 + buttonWidth + 10, buttonY);
            stopButton.Size = new Size(buttonWidth, buttonHeight);

            restartButton!.Location = new Point(15 + 2 * (buttonWidth + 10), buttonY);
            restartButton.Size = new Size(buttonWidth, buttonHeight);

            refreshButton!.Location = new Point(15 + 3 * (buttonWidth + 10), buttonY);
            refreshButton.Size = new Size(buttonWidth, buttonHeight);

            // Add controls to group
            actionsGroupBox.Controls.AddRange(new Control[]
            {
                startButton, stopButton, restartButton, refreshButton
            });

            // Add controls to form
            this.Controls.AddRange(new Control[]
            {
                servicesGrid, actionsGroupBox
            });
        }

        private void SetupEventHandlers()
        {
            startButton!.Click += StartButton_Click;
            stopButton!.Click += StopButton_Click;
            restartButton!.Click += RestartButton_Click;
            refreshButton!.Click += RefreshButton_Click;

            servicesGrid!.SelectionChanged += ServicesGrid_SelectionChanged;

            // Interface manager events
            interfaceManager.InterfaceStatusChanged += InterfaceManager_InterfaceStatusChanged;
        }

        private void LoadServices()
        {
            if (servicesGrid == null) return;

            var interfaces = interfaceManager.Interfaces.Select(i => new
            {
                Interface = i,
                Name = i.Name,
                ServiceName = i.ServiceName,
                Status = i.Status.ToString(),
                Type = i.Type.ToString(),
                LastStarted = i.LastStarted == DateTime.MinValue ? "Never" : i.LastStarted.ToString("yyyy-MM-dd HH:mm:ss"),
                Uptime = i.Status == InterfaceStatus.Running ? i.Uptime.ToString(@"dd\.hh\:mm\:ss") : "00.00:00:00"
            }).ToList();

            servicesGrid.DataSource = interfaces;
        }

        private void ServicesGrid_SelectionChanged(object? sender, EventArgs e)
        {
            bool hasSelection = servicesGrid!.SelectedRows.Count > 0;
            
            if (hasSelection)
            {
                var selectedRow = servicesGrid.SelectedRows[0];
                var interfaceItem = selectedRow.DataBoundItem?.GetType().GetProperty("Interface")?.GetValue(selectedRow.DataBoundItem) as PIInterface;
                
                if (interfaceItem != null)
                {
                    bool isRunning = interfaceItem.Status == InterfaceStatus.Running;
                    bool isStopped = interfaceItem.Status == InterfaceStatus.Stopped;
                    
                    startButton!.Enabled = isStopped;
                    stopButton!.Enabled = isRunning;
                    restartButton!.Enabled = isRunning;
                }
            }
            else
            {
                startButton!.Enabled = false;
                stopButton!.Enabled = false;
                restartButton!.Enabled = false;
            }
        }

        private async void StartButton_Click(object? sender, EventArgs e)
        {
            var selectedInterface = GetSelectedInterface();
            if (selectedInterface != null)
            {
                try
                {
                    await interfaceManager.StartInterfaceAsync(selectedInterface.Name);
                    LoadServices();
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Error starting interface: {ex.Message}", "Error",
                        MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }

        private async void StopButton_Click(object? sender, EventArgs e)
        {
            var selectedInterface = GetSelectedInterface();
            if (selectedInterface != null)
            {
                try
                {
                    await interfaceManager.StopInterfaceAsync(selectedInterface.Name);
                    LoadServices();
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Error stopping interface: {ex.Message}", "Error",
                        MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }

        private async void RestartButton_Click(object? sender, EventArgs e)
        {
            var selectedInterface = GetSelectedInterface();
            if (selectedInterface != null)
            {
                try
                {
                    await interfaceManager.StopInterfaceAsync(selectedInterface.Name);
                    await Task.Delay(2000); // Wait for clean shutdown
                    await interfaceManager.StartInterfaceAsync(selectedInterface.Name);
                    LoadServices();
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Error restarting interface: {ex.Message}", "Error",
                        MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }

        private void RefreshButton_Click(object? sender, EventArgs e)
        {
            LoadServices();
        }

        private PIInterface? GetSelectedInterface()
        {
            if (servicesGrid!.SelectedRows.Count == 0)
                return null;

            var selectedRow = servicesGrid.SelectedRows[0];
            return selectedRow.DataBoundItem?.GetType().GetProperty("Interface")?.GetValue(selectedRow.DataBoundItem) as PIInterface;
        }

        private void InterfaceManager_InterfaceStatusChanged(object? sender, PIInterface e)
        {
            // Update the grid when interface status changes
            if (this.InvokeRequired)
            {
                this.Invoke(new Action(() => LoadServices()));
            }
            else
            {
                LoadServices();
            }
        }
    }
} 