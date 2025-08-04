using System;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using PIInterfaceConfigUtility.Models;
using PIInterfaceConfigUtility.Services;
using PIInterfaceConfigUtility.Dialogs;

namespace PIInterfaceConfigUtility
{
    public partial class InterfaceConfigurationControl : UserControl
    {
        private readonly InterfaceManager interfaceManager;
        private ToolStrip? toolStrip;
            private DataGridView? interfacesGrid;
    private GroupBox? propertiesGroupBox;
    private PropertyGrid? propertyGrid;
    private SplitContainer? splitContainer;
        
        public InterfaceConfigurationControl(InterfaceManager manager)
        {
            interfaceManager = manager ?? throw new ArgumentNullException(nameof(manager));
            InitializeComponent();
            SetupEventHandlers();
            LoadInterfaces();
        }
        
        private void InitializeComponent()
        {
            this.SuspendLayout();
            
            // Main control properties
            this.Size = new Size(800, 600);
            this.BackColor = Color.White;
            this.Dock = DockStyle.Fill;
            
            SetupToolStrip();
            SetupSplitContainer();
            SetupInterfacesGrid();
            SetupPropertiesPanel();
            
            this.ResumeLayout(false);
        }
        
        private void SetupToolStrip()
        {
            toolStrip = new ToolStrip();
            toolStrip.ImageScalingSize = new Size(24, 24);
            
            var addButton = new ToolStripButton("Add Interface", null, AddInterface_Click)
            {
                DisplayStyle = ToolStripItemDisplayStyle.ImageAndText
            };
            
            var editButton = new ToolStripButton("Edit Interface", null, EditInterface_Click)
            {
                DisplayStyle = ToolStripItemDisplayStyle.ImageAndText
            };
            
            var deleteButton = new ToolStripButton("Delete Interface", null, DeleteInterface_Click)
            {
                DisplayStyle = ToolStripItemDisplayStyle.ImageAndText
            };
            
            var startButton = new ToolStripButton("Start Interface", null, StartInterface_Click)
            {
                DisplayStyle = ToolStripItemDisplayStyle.ImageAndText
            };
            
            var stopButton = new ToolStripButton("Stop Interface", null, StopInterface_Click)
            {
                DisplayStyle = ToolStripItemDisplayStyle.ImageAndText
            };
            
            var refreshButton = new ToolStripButton("Refresh Status", null, RefreshStatus_Click)
            {
                DisplayStyle = ToolStripItemDisplayStyle.ImageAndText
            };
            
            toolStrip.Items.AddRange(new ToolStripItem[]
            {
                addButton,
                editButton,
                deleteButton,
                new ToolStripSeparator(),
                startButton,
                stopButton,
                new ToolStripSeparator(),
                refreshButton
            });
            
            this.Controls.Add(toolStrip);
        }
        
        private void SetupSplitContainer()
        {
            splitContainer = new SplitContainer
            {
                Dock = DockStyle.Fill,
                Orientation = Orientation.Horizontal,
                SplitterDistance = 350,
                Panel1MinSize = 200,
                Panel2MinSize = 150
            };
            
            this.Controls.Add(splitContainer);
        }
        
        private void SetupInterfacesGrid()
        {
            interfacesGrid = new DataGridView
            {
                Dock = DockStyle.Fill,
                AutoGenerateColumns = false,
                SelectionMode = DataGridViewSelectionMode.FullRowSelect,
                MultiSelect = false,
                AllowUserToAddRows = false,
                AllowUserToDeleteRows = false,
                ReadOnly = true,
                BackgroundColor = Color.White,
                BorderStyle = BorderStyle.None
            };
            
            // Add columns
            interfacesGrid.Columns.AddRange(new DataGridViewColumn[]
            {
                new DataGridViewTextBoxColumn
                {
                    Name = "Name",
                    HeaderText = "Interface Name",
                    DataPropertyName = "Name",
                    Width = 150
                },
                new DataGridViewTextBoxColumn
                {
                    Name = "Type",
                    HeaderText = "Type",
                    DataPropertyName = "Type",
                    Width = 100
                },
                new DataGridViewTextBoxColumn
                {
                    Name = "Status",
                    HeaderText = "Status",
                    DataPropertyName = "Status",
                    Width = 100
                },
                new DataGridViewTextBoxColumn
                {
                    Name = "Description",
                    HeaderText = "Description",
                    DataPropertyName = "Description",
                    Width = 200,
                    AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill
                },
                new DataGridViewTextBoxColumn
                {
                    Name = "LastStarted",
                    HeaderText = "Last Started",
                    DataPropertyName = "LastStarted",
                    Width = 130
                },
                new DataGridViewTextBoxColumn
                {
                    Name = "PointCount",
                    HeaderText = "Points",
                    Width = 70
                }
            });
            
            // Custom formatting for status column
            interfacesGrid!.CellFormatting += InterfacesGrid_CellFormatting;
            
            splitContainer!.Panel1.Controls.Add(interfacesGrid);
        }
        
        private void SetupPropertiesPanel()
        {
            propertiesGroupBox = new GroupBox
            {
                Text = "Interface Properties",
                Dock = DockStyle.Fill,
                Font = new Font("Segoe UI", 9F, FontStyle.Bold)
            };
            
            propertyGrid = new PropertyGrid
            {
                Dock = DockStyle.Fill,
                PropertySort = PropertySort.Categorized,
                HelpVisible = true
            };
            
            propertiesGroupBox!.Controls.Add(propertyGrid!);
            splitContainer!.Panel2.Controls.Add(propertiesGroupBox);
        }
        
        private void SetupEventHandlers()
        {
            interfacesGrid!.SelectionChanged += InterfacesGrid_SelectionChanged;
            interfaceManager.StatusChanged += InterfaceManager_StatusChanged;
            interfaceManager.InterfaceStatusChanged += InterfaceManager_InterfaceStatusChanged;
        }
        
        private void LoadInterfaces()
        {
            var interfaceList = interfaceManager.Interfaces.Select(i => new
            {
                i.Name,
                i.Type,
                i.Status,
                i.Description,
                i.LastStarted,
                PointCount = i.Points.Count,
                Interface = i
            }).ToList();
            
            interfacesGrid!.DataSource = interfaceList;
        }
        
        private void InterfacesGrid_CellFormatting(object? sender, DataGridViewCellFormattingEventArgs e)
        {
            if (e.ColumnIndex == interfacesGrid!.Columns["Status"].Index && e.Value != null)
            {
                var status = (InterfaceStatus)e.Value;
                e.CellStyle.ForeColor = status switch
                {
                    InterfaceStatus.Running => Color.Green,
                    InterfaceStatus.Stopped => Color.Red,
                    InterfaceStatus.Starting => Color.Orange,
                    InterfaceStatus.Stopping => Color.Orange,
                    InterfaceStatus.Error => Color.DarkRed,
                    _ => Color.Gray
                };
                e.CellStyle.Font = new Font(e.CellStyle.Font, FontStyle.Bold);
            }
            
            if (e.ColumnIndex == interfacesGrid.Columns["LastStarted"].Index && e.Value != null)
            {
                if (e.Value is DateTime dateTime)
                {
                    e.Value = dateTime.ToString("yyyy-MM-dd HH:mm:ss");
                    e.FormattingApplied = true;
                }
            }
        }
        
        private void InterfacesGrid_SelectionChanged(object? sender, EventArgs e)
        {
            if (interfacesGrid!.SelectedRows.Count > 0)
            {
                var selectedRow = interfacesGrid.SelectedRows[0];
                var piInterface = selectedRow.DataBoundItem?.GetType().GetProperty("Interface")?.GetValue(selectedRow.DataBoundItem) as PIInterface;
                
                if (piInterface != null)
                {
                    propertyGrid!.SelectedObject = piInterface;
                }
            }
            else
            {
                propertyGrid!.SelectedObject = null;
            }
        }
        
        private void AddInterface_Click(object? sender, EventArgs e)
        {
            using (var dialog = new AddInterfaceDialog())
            {
                if (dialog.ShowDialog() == DialogResult.OK)
                {
                    try
                    {
                        // Use the Interface property from the dialog
                        var newInterface = dialog.Interface;
                        
                        interfaceManager!.AddInterface(newInterface);
                        LoadInterfaces();
                        
                        MessageBox.Show($"Interface '{newInterface.Name}' added successfully!", "Success",
                            MessageBoxButtons.OK, MessageBoxIcon.Information);
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show($"Error adding interface: {ex.Message}", "Error",
                            MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                }
            }
        }
        
        private void EditInterface_Click(object? sender, EventArgs e)
        {
            if (interfacesGrid!.SelectedRows.Count == 0)
            {
                MessageBox.Show("Please select an interface to edit.", "No Selection",
                    MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }
            
            var selectedRow = interfacesGrid.SelectedRows[0];
            var piInterface = selectedRow.DataBoundItem?.GetType().GetProperty("Interface")?.GetValue(selectedRow.DataBoundItem) as PIInterface;
            
            if (piInterface != null)
            {
                var editDialog = new EditInterfaceDialog(piInterface);
                if (editDialog.ShowDialog() == DialogResult.OK)
                {
                    interfaceManager.UpdateInterface(editDialog.Interface);
                    LoadInterfaces();
                }
            }
        }
        
        private void DeleteInterface_Click(object? sender, EventArgs e)
        {
            if (interfacesGrid!.SelectedRows.Count == 0)
            {
                MessageBox.Show("Please select an interface to delete.", "No Selection",
                    MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }
            
            var selectedRow = interfacesGrid.SelectedRows[0];
            var interfaceName = selectedRow.Cells["Name"].Value?.ToString();
            
            if (string.IsNullOrEmpty(interfaceName))
                return;
                
            var result = MessageBox.Show(
                $"Are you sure you want to delete the interface '{interfaceName}'?",
                "Confirm Delete",
                MessageBoxButtons.YesNo,
                MessageBoxIcon.Question);
                
            if (result == DialogResult.Yes)
            {
                interfaceManager.RemoveInterface(interfaceName);
                LoadInterfaces();
            }
        }
        
        private async void StartInterface_Click(object? sender, EventArgs e)
        {
            if (interfacesGrid!.SelectedRows.Count == 0)
            {
                MessageBox.Show("Please select an interface to start.", "No Selection",
                    MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }
            
            var selectedRow = interfacesGrid.SelectedRows[0];
            var interfaceName = selectedRow.Cells["Name"].Value?.ToString();
            
            if (string.IsNullOrEmpty(interfaceName))
                return;
                
            try
            {
                await interfaceManager.StartInterfaceAsync(interfaceName);
                LoadInterfaces();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error starting interface: {ex.Message}", "Error",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
        
        private async void StopInterface_Click(object? sender, EventArgs e)
        {
            if (interfacesGrid!.SelectedRows.Count == 0)
            {
                MessageBox.Show("Please select an interface to stop.", "No Selection",
                    MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }
            
            var selectedRow = interfacesGrid.SelectedRows[0];
            var interfaceName = selectedRow.Cells["Name"].Value?.ToString();
            
            if (string.IsNullOrEmpty(interfaceName))
                return;
                
            try
            {
                await interfaceManager.StopInterfaceAsync(interfaceName);
                LoadInterfaces();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error stopping interface: {ex.Message}", "Error",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
        
        private async void RefreshStatus_Click(object? sender, EventArgs e)
        {
            try
            {
                await interfaceManager.RefreshAllInterfaceStatusAsync();
                LoadInterfaces();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error refreshing status: {ex.Message}", "Error",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
        
        private void InterfaceManager_StatusChanged(object? sender, string status)
        {
            // This could be used to update a status bar or log
        }
        
        private void InterfaceManager_InterfaceStatusChanged(object? sender, PIInterface piInterface)
        {
            if (InvokeRequired)
            {
                Invoke(new Action(() => InterfaceManager_InterfaceStatusChanged(sender, piInterface)));
                return;
            }
            
            LoadInterfaces();
        }
    }
} 