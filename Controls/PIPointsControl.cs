using System;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using PIInterfaceConfigUtility.Models;
using PIInterfaceConfigUtility.Services;

namespace PIInterfaceConfigUtility
{
    public partial class PIPointsControl : UserControl
    {
        private readonly PIServerManager piServerManager;
        private ToolStrip? toolStrip;
        private DataGridView? pointsGrid;
        private GroupBox? propertiesGroupBox;
        private PropertyGrid? propertyGrid;
        private SplitContainer? splitContainer;
        private GroupBox? searchGroupBox;
        private TextBox? searchTextBox;
        private Button? searchButton;
        private Label? connectionStatusLabel;
        
        public PIPointsControl(PIServerManager serverManager)
        {
            piServerManager = serverManager ?? throw new ArgumentNullException(nameof(serverManager));
            InitializeComponent();
            SetupEventHandlers();
            UpdateConnectionStatus();
            LoadPIPoints();
        }
        
        private void InitializeComponent()
        {
            this.SuspendLayout();
            
            // Main control properties
            this.Size = new Size(800, 600);
            this.BackColor = Color.White;
            this.Dock = DockStyle.Fill;
            
            SetupSearchSection();
            SetupToolStrip();
            SetupSplitContainer();
            SetupPointsGrid();
            SetupPropertiesPanel();
            
            this.ResumeLayout(false);
        }
        
        private void SetupSearchSection()
        {
            searchGroupBox = new GroupBox
            {
                Text = "Search PI Points",
                Location = new Point(10, 10),
                Size = new Size(780, 60),
                Font = new Font("Segoe UI", 9F, FontStyle.Bold)
            };
            
            connectionStatusLabel = new Label
            {
                Location = new Point(15, 25),
                Size = new Size(100, 23),
                Text = "Not Connected",
                ForeColor = Color.Red
            };
            
            searchTextBox = new TextBox
            {
                Location = new Point(125, 22),
                Size = new Size(200, 23),
                PlaceholderText = "Enter search pattern (* for all)"
            };
            
            searchButton = new Button
            {
                Text = "Search",
                Location = new Point(335, 21),
                Size = new Size(80, 25),
                BackColor = Color.FromArgb(0, 120, 215),
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat,
                Enabled = false
            };
            
            searchGroupBox.Controls.AddRange(new Control[]
            {
                connectionStatusLabel, searchTextBox, searchButton
            });
            
            this.Controls.Add(searchGroupBox);
        }
        
        private void SetupToolStrip()
        {
            toolStrip = new ToolStrip();
            toolStrip.Location = new Point(0, 80);
            toolStrip.ImageScalingSize = new Size(24, 24);
            
            var addButton = new ToolStripButton("Add Point", null, AddPoint_Click)
            {
                DisplayStyle = ToolStripItemDisplayStyle.ImageAndText
            };
            
            var editButton = new ToolStripButton("Edit Point", null, EditPoint_Click)
            {
                DisplayStyle = ToolStripItemDisplayStyle.ImageAndText
            };
            
            var deleteButton = new ToolStripButton("Delete Point", null, DeletePoint_Click)
            {
                DisplayStyle = ToolStripItemDisplayStyle.ImageAndText
            };
            
            var readButton = new ToolStripButton("Read Value", null, ReadValue_Click)
            {
                DisplayStyle = ToolStripItemDisplayStyle.ImageAndText
            };
            
            var writeButton = new ToolStripButton("Write Value", null, WriteValue_Click)
            {
                DisplayStyle = ToolStripItemDisplayStyle.ImageAndText
            };
            
            var refreshButton = new ToolStripButton("Refresh", null, RefreshPoints_Click)
            {
                DisplayStyle = ToolStripItemDisplayStyle.ImageAndText
            };
            
            toolStrip.Items.AddRange(new ToolStripItem[]
            {
                addButton,
                editButton,
                deleteButton,
                new ToolStripSeparator(),
                readButton,
                writeButton,
                new ToolStripSeparator(),
                refreshButton
            });
            
            this.Controls.Add(toolStrip);
        }
        
        private void SetupSplitContainer()
        {
            splitContainer = new SplitContainer
            {
                Location = new Point(0, 110),
                Size = new Size(800, 490),
                Orientation = Orientation.Horizontal,
                SplitterDistance = 300,
                Panel1MinSize = 200,
                Panel2MinSize = 150
            };
            
            this.Controls.Add(splitContainer);
        }
        
        private void SetupPointsGrid()
        {
            pointsGrid = new DataGridView
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
            pointsGrid.Columns.AddRange(new DataGridViewColumn[]
            {
                new DataGridViewTextBoxColumn
                {
                    Name = "Name",
                    HeaderText = "Point Name",
                    DataPropertyName = "Name",
                    Width = 150
                },
                new DataGridViewTextBoxColumn
                {
                    Name = "Type",
                    HeaderText = "Type",
                    DataPropertyName = "Type",
                    Width = 80
                },
                new DataGridViewTextBoxColumn
                {
                    Name = "SourceAddress",
                    HeaderText = "Source Address",
                    DataPropertyName = "SourceAddress",
                    Width = 120
                },
                new DataGridViewTextBoxColumn
                {
                    Name = "CurrentValue",
                    HeaderText = "Current Value",
                    Width = 100
                },
                new DataGridViewTextBoxColumn
                {
                    Name = "Units",
                    HeaderText = "Units",
                    DataPropertyName = "Units",
                    Width = 60
                },
                new DataGridViewTextBoxColumn
                {
                    Name = "Status",
                    HeaderText = "Status",
                    DataPropertyName = "Status",
                    Width = 80
                },
                new DataGridViewTextBoxColumn
                {
                    Name = "LastUpdate",
                    HeaderText = "Last Update",
                    DataPropertyName = "LastUpdate",
                    Width = 130
                },
                new DataGridViewTextBoxColumn
                {
                    Name = "Description",
                    HeaderText = "Description",
                    DataPropertyName = "Description",
                    AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill
                }
            });
            
            // Custom formatting
            pointsGrid.CellFormatting += PointsGrid_CellFormatting;
            
            splitContainer.Panel1.Controls.Add(pointsGrid);
        }
        
        private void SetupPropertiesPanel()
        {
            propertiesGroupBox = new GroupBox
            {
                Text = "Point Properties",
                Dock = DockStyle.Fill,
                Font = new Font("Segoe UI", 9F, FontStyle.Bold)
            };
            
            propertyGrid = new PropertyGrid
            {
                Dock = DockStyle.Fill,
                PropertySort = PropertySort.Categorized,
                HelpVisible = true
            };
            
            propertiesGroupBox.Controls.Add(propertyGrid);
            splitContainer.Panel2.Controls.Add(propertiesGroupBox);
        }
        
        private void SetupEventHandlers()
        {
            searchButton.Click += SearchButton_Click;
            searchTextBox.KeyPress += SearchTextBox_KeyPress;
            pointsGrid.SelectionChanged += PointsGrid_SelectionChanged;
            piServerManager.StatusChanged += PIServerManager_StatusChanged;
            piServerManager.ConnectionChanged += PIServerManager_ConnectionChanged;
        }
        
        private void UpdateConnectionStatus()
        {
            if (piServerManager.IsConnected)
            {
                connectionStatusLabel.Text = "Connected";
                connectionStatusLabel.ForeColor = Color.Green;
                searchButton.Enabled = true;
            }
            else
            {
                connectionStatusLabel.Text = "Not Connected";
                connectionStatusLabel.ForeColor = Color.Red;
                searchButton.Enabled = false;
            }
        }
        
        private void LoadPIPoints()
        {
            if (!piServerManager.IsConnected)
            {
                pointsGrid.DataSource = null;
                return;
            }
            
            var pointList = piServerManager.PIPoints.Select(p => new
            {
                p.Name,
                p.Type,
                p.SourceAddress,
                CurrentValue = p.GetFormattedValue(),
                p.Units,
                p.Status,
                p.LastUpdate,
                p.Description,
                Point = p
            }).ToList();
            
            pointsGrid.DataSource = pointList;
        }
        
        private void PointsGrid_CellFormatting(object? sender, DataGridViewCellFormattingEventArgs e)
        {
            if (e.ColumnIndex == pointsGrid.Columns["Status"].Index && e.Value != null)
            {
                var status = (PIPointStatus)e.Value;
                e.CellStyle.ForeColor = status switch
                {
                    PIPointStatus.Good => Color.Green,
                    PIPointStatus.Bad => Color.Red,
                    PIPointStatus.Questionable => Color.Orange,
                    PIPointStatus.ConfigError => Color.DarkRed,
                    PIPointStatus.NotConnected => Color.Gray,
                    _ => Color.Gray
                };
                e.CellStyle.Font = new Font(e.CellStyle.Font, FontStyle.Bold);
            }
            
            if (e.ColumnIndex == pointsGrid.Columns["LastUpdate"].Index && e.Value != null)
            {
                if (e.Value is DateTime dateTime)
                {
                    e.Value = dateTime.ToString("yyyy-MM-dd HH:mm:ss");
                    e.FormattingApplied = true;
                }
            }
        }
        
        private void PointsGrid_SelectionChanged(object? sender, EventArgs e)
        {
            if (pointsGrid.SelectedRows.Count > 0)
            {
                var selectedRow = pointsGrid.SelectedRows[0];
                var point = selectedRow.DataBoundItem?.GetType().GetProperty("Point")?.GetValue(selectedRow.DataBoundItem) as PIPoint;
                
                if (point != null)
                {
                    propertyGrid.SelectedObject = point;
                }
            }
            else
            {
                propertyGrid.SelectedObject = null;
            }
        }
        
        private async void SearchButton_Click(object? sender, EventArgs e)
        {
            if (!piServerManager.IsConnected)
            {
                MessageBox.Show("Please connect to a PI Server first.", "Not Connected",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }
            
            try
            {
                searchButton.Enabled = false;
                searchButton.Text = "Searching...";
                
                var searchPattern = string.IsNullOrWhiteSpace(searchTextBox.Text) ? "*" : searchTextBox.Text;
                var points = await piServerManager.SearchPIPointsAsync(searchPattern);
                
                LoadPIPoints();
                
                MessageBox.Show($"Found {points.Count} PI points matching '{searchPattern}'", "Search Complete",
                    MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Search failed: {ex.Message}", "Search Error",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            finally
            {
                searchButton.Enabled = true;
                searchButton.Text = "Search";
            }
        }
        
        private void SearchTextBox_KeyPress(object? sender, KeyPressEventArgs e)
        {
            if (e.KeyChar == (char)Keys.Enter)
            {
                SearchButton_Click(sender, EventArgs.Empty);
                e.Handled = true;
            }
        }
        
        private void AddPoint_Click(object? sender, EventArgs e)
        {
            if (!piServerManager.IsConnected)
            {
                MessageBox.Show("Please connect to a PI Server first.", "Not Connected",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }
            
            var addDialog = new AddPIPointDialog();
            if (addDialog.ShowDialog() == DialogResult.OK)
            {
                try
                {
                    var newPoint = addDialog.PIPoint;
                    piServerManager.CreatePIPointAsync(newPoint);
                    LoadPIPoints();
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Error adding PI point: {ex.Message}", "Error",
                        MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }
        
        private void EditPoint_Click(object? sender, EventArgs e)
        {
            if (pointsGrid.SelectedRows.Count == 0)
            {
                MessageBox.Show("Please select a PI point to edit.", "No Selection",
                    MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }
            
            var selectedRow = pointsGrid.SelectedRows[0];
            var point = selectedRow.DataBoundItem?.GetType().GetProperty("Point")?.GetValue(selectedRow.DataBoundItem) as PIPoint;
            
            if (point != null)
            {
                var editDialog = new EditPIPointDialog(point);
                if (editDialog.ShowDialog() == DialogResult.OK)
                {
                    try
                    {
                        piServerManager.UpdatePIPointAsync(editDialog.PIPoint);
                        LoadPIPoints();
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show($"Error updating PI point: {ex.Message}", "Error",
                            MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                }
            }
        }
        
        private async void DeletePoint_Click(object? sender, EventArgs e)
        {
            if (pointsGrid.SelectedRows.Count == 0)
            {
                MessageBox.Show("Please select a PI point to delete.", "No Selection",
                    MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }
            
            var selectedRow = pointsGrid.SelectedRows[0];
            var pointName = selectedRow.Cells["Name"].Value?.ToString();
            
            if (string.IsNullOrEmpty(pointName))
                return;
                
            var result = MessageBox.Show(
                $"Are you sure you want to delete the PI point '{pointName}'?",
                "Confirm Delete",
                MessageBoxButtons.YesNo,
                MessageBoxIcon.Question);
                
            if (result == DialogResult.Yes)
            {
                try
                {
                    await piServerManager.DeletePIPointAsync(pointName);
                    LoadPIPoints();
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Error deleting PI point: {ex.Message}", "Error",
                        MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }
        
        private async void ReadValue_Click(object? sender, EventArgs e)
        {
            if (pointsGrid.SelectedRows.Count == 0)
            {
                MessageBox.Show("Please select a PI point to read.", "No Selection",
                    MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }
            
            var selectedRow = pointsGrid.SelectedRows[0];
            var pointName = selectedRow.Cells["Name"].Value?.ToString();
            
            if (string.IsNullOrEmpty(pointName))
                return;
                
            try
            {
                var value = await piServerManager.ReadPIPointValueAsync(pointName);
                LoadPIPoints();
                
                MessageBox.Show($"Current value for '{pointName}': {value}", "Read Value",
                    MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error reading PI point value: {ex.Message}", "Error",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
        
        private async void WriteValue_Click(object? sender, EventArgs e)
        {
            if (pointsGrid.SelectedRows.Count == 0)
            {
                MessageBox.Show("Please select a PI point to write to.", "No Selection",
                    MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }
            
            var selectedRow = pointsGrid.SelectedRows[0];
            var pointName = selectedRow.Cells["Name"].Value?.ToString();
            
            if (string.IsNullOrEmpty(pointName))
                return;
                
            var writeDialog = new WriteValueDialog(pointName);
            if (writeDialog.ShowDialog() == DialogResult.OK)
            {
                try
                {
                    await piServerManager.WritePIPointValueAsync(pointName, writeDialog.Value);
                    LoadPIPoints();
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Error writing PI point value: {ex.Message}", "Error",
                        MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }
        
        private void RefreshPoints_Click(object? sender, EventArgs e)
        {
            LoadPIPoints();
        }
        
        private void PIServerManager_StatusChanged(object? sender, string status)
        {
            // Could update status bar
        }
        
        private void PIServerManager_ConnectionChanged(object? sender, PIServerConnection connection)
        {
            if (InvokeRequired)
            {
                Invoke(new Action(() => PIServerManager_ConnectionChanged(sender, connection)));
                return;
            }
            
            UpdateConnectionStatus();
            LoadPIPoints();
        }
    }
} 