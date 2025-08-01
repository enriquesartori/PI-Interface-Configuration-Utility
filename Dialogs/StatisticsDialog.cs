using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;

namespace PIInterfaceConfigUtility
{
    public partial class StatisticsDialog : Form
    {
        private readonly string serviceName;
        private readonly Dictionary<string, object> statistics;
        private ListView statisticsListView;
        private Button refreshButton, closeButton;
        
        public StatisticsDialog(string serviceName, Dictionary<string, object> stats)
        {
            this.serviceName = serviceName ?? throw new ArgumentNullException(nameof(serviceName));
            this.statistics = stats ?? throw new ArgumentNullException(nameof(stats));
            InitializeComponent();
            PopulateStatistics();
        }
        
        private void InitializeComponent()
        {
            this.SuspendLayout();
            
            this.Size = new Size(500, 600);
            this.Text = $"Statistics - {serviceName}";
            this.FormBorderStyle = FormBorderStyle.Sizable;
            this.MinimumSize = new Size(400, 400);
            this.StartPosition = FormStartPosition.CenterParent;
            this.ShowIcon = false;
            
            // Title
            var titleLabel = new Label
            {
                Text = $"Interface Statistics: {serviceName}",
                Location = new Point(15, 15),
                Size = new Size(450, 25),
                Font = new Font("Segoe UI", 12F, FontStyle.Bold),
                ForeColor = Color.FromArgb(0, 120, 215)
            };
            
            // Statistics ListView
            statisticsListView = new ListView
            {
                Location = new Point(15, 50),
                Size = new Size(450, 480),
                View = View.Details,
                FullRowSelect = true,
                GridLines = true,
                HeaderStyle = ColumnHeaderStyle.Nonclickable
            };
            
            // Add columns
            statisticsListView.Columns.Add("Property", 200);
            statisticsListView.Columns.Add("Value", 240);
            
            // Buttons
            refreshButton = new Button
            {
                Text = "Refresh",
                Location = new Point(300, 540),
                Size = new Size(80, 30),
                BackColor = Color.FromArgb(0, 120, 215),
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat
            };
            
            closeButton = new Button
            {
                Text = "Close",
                Location = new Point(390, 540),
                Size = new Size(80, 30),
                BackColor = Color.FromArgb(108, 117, 125),
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat,
                DialogResult = DialogResult.OK
            };
            
            this.Controls.AddRange(new Control[]
            {
                titleLabel, statisticsListView, refreshButton, closeButton
            });
            
            this.AcceptButton = closeButton;
            
            refreshButton.Click += RefreshButton_Click;
            this.Resize += StatisticsDialog_Resize;
            
            this.ResumeLayout(false);
        }
        
        private void PopulateStatistics()
        {
            statisticsListView.Items.Clear();
            
            // General statistics
            AddStatistic("Service Name", serviceName);
            AddStatistic("Last Updated", DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"));
            
            // Add all provided statistics
            foreach (var stat in statistics)
            {
                var displayValue = FormatStatisticValue(stat.Value);
                AddStatistic(FormatPropertyName(stat.Key), displayValue);
            }
            
            // Performance metrics (calculated)
            if (statistics.ContainsKey("MessagesReceived") && statistics.ContainsKey("MessagesSent"))
            {
                var received = Convert.ToInt64(statistics["MessagesReceived"]);
                var sent = Convert.ToInt64(statistics["MessagesSent"]);
                var total = received + sent;
                
                AddStatistic("Total Messages", total.ToString("N0"));
                
                if (total > 0 && statistics.ContainsKey("ErrorCount"))
                {
                    var errors = Convert.ToInt64(statistics["ErrorCount"]);
                    var successRate = ((double)(total - errors) / total) * 100;
                    AddStatistic("Success Rate", $"{successRate:F2}%");
                }
            }
            
            // Uptime calculation
            if (statistics.ContainsKey("Uptime") && statistics["Uptime"] is TimeSpan uptime)
            {
                AddStatistic("Uptime", FormatTimeSpan(uptime));
            }
        }
        
        private void AddStatistic(string property, string value)
        {
            var item = new ListViewItem(property);
            item.SubItems.Add(value);
            
            // Color coding for certain values
            if (property.Contains("Error") && !value.Equals("0"))
            {
                item.ForeColor = Color.Red;
            }
            else if (property.Contains("Success Rate"))
            {
                if (double.TryParse(value.Replace("%", ""), out double rate))
                {
                    item.ForeColor = rate >= 95 ? Color.Green : rate >= 90 ? Color.Orange : Color.Red;
                }
            }
            else if (property.Contains("Messages") && value != "0")
            {
                item.ForeColor = Color.Blue;
            }
            
            statisticsListView.Items.Add(item);
        }
        
        private string FormatPropertyName(string key)
        {
            // Convert camelCase to readable format
            var result = "";
            for (int i = 0; i < key.Length; i++)
            {
                if (i > 0 && char.IsUpper(key[i]))
                    result += " ";
                result += key[i];
            }
            return result;
        }
        
        private string FormatStatisticValue(object value)
        {
            return value switch
            {
                null => "N/A",
                DateTime dt => dt.ToString("yyyy-MM-dd HH:mm:ss"),
                TimeSpan ts => FormatTimeSpan(ts),
                long l => l.ToString("N0"),
                int i => i.ToString("N0"),
                double d => d.ToString("N2"),
                bool b => b ? "Yes" : "No",
                _ => value.ToString() ?? "N/A"
            };
        }
        
        private string FormatTimeSpan(TimeSpan timeSpan)
        {
            if (timeSpan.TotalDays >= 1)
                return $"{timeSpan.Days}d {timeSpan.Hours}h {timeSpan.Minutes}m";
            else if (timeSpan.TotalHours >= 1)
                return $"{timeSpan.Hours}h {timeSpan.Minutes}m {timeSpan.Seconds}s";
            else if (timeSpan.TotalMinutes >= 1)
                return $"{timeSpan.Minutes}m {timeSpan.Seconds}s";
            else
                return $"{timeSpan.Seconds}s";
        }
        
        private void RefreshButton_Click(object? sender, EventArgs e)
        {
            // In a real implementation, this would fetch fresh statistics
            PopulateStatistics();
            
            MessageBox.Show("Statistics refreshed.", "Refresh Complete",
                MessageBoxButtons.OK, MessageBoxIcon.Information);
        }
        
        private void StatisticsDialog_Resize(object? sender, EventArgs e)
        {
            // Adjust controls when dialog is resized
            if (statisticsListView != null && refreshButton != null && closeButton != null)
            {
                statisticsListView.Size = new Size(this.Width - 45, this.Height - 130);
                
                refreshButton.Location = new Point(this.Width - 190, this.Height - 70);
                closeButton.Location = new Point(this.Width - 100, this.Height - 70);
                
                // Adjust column widths
                if (statisticsListView.Columns.Count >= 2)
                {
                    statisticsListView.Columns[0].Width = (int)(statisticsListView.Width * 0.4);
                    statisticsListView.Columns[1].Width = (int)(statisticsListView.Width * 0.55);
                }
            }
        }
    }
} 