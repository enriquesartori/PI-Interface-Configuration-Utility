using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;
using PIInterfaceConfigUtility.Models;

namespace PIInterfaceConfigUtility.Dialogs
{
    public partial class StatisticsDialog : Form
    {
        private ListView? statisticsListView;
        private Button? refreshButton;
        private Button? closeButton;
        
        private PIInterface interfaceInstance;

        public StatisticsDialog(PIInterface piInterface)
        {
            interfaceInstance = piInterface;
            InitializeComponent();
            LoadStatistics();
        }

        private void InitializeComponent()
        {
            this.Text = $"Statistics - {interfaceInstance.Name}";
            this.Size = new Size(600, 400);
            this.StartPosition = FormStartPosition.CenterParent;
            this.FormBorderStyle = FormBorderStyle.Sizable;

            // Statistics ListView
            statisticsListView = new ListView
            {
                Location = new Point(12, 12),
                Size = new Size(560, 300),
                View = View.Details,
                FullRowSelect = true,
                GridLines = true
            };

            statisticsListView.Columns.Add("Property", 200);
            statisticsListView.Columns.Add("Value", 350);

            // Refresh Button
            refreshButton = new Button
            {
                Text = "Refresh",
                Location = new Point(400, 325),
                Size = new Size(75, 25)
            };
            refreshButton.Click += RefreshButton_Click;

            // Close Button
            closeButton = new Button
            {
                Text = "Close",
                Location = new Point(485, 325),
                Size = new Size(75, 25),
                DialogResult = DialogResult.OK
            };

            this.Controls.AddRange(new Control[]
            {
                statisticsListView,
                refreshButton,
                closeButton
            });

            this.AcceptButton = closeButton;
        }

        private void LoadStatistics()
        {
            if (statisticsListView == null) return;

            statisticsListView.Items.Clear();

            // Add statistics with color coding
            AddStatistic("Interface Name", interfaceInstance.Name, Color.Blue);
            AddStatistic("Type", interfaceInstance.Type.ToString(), Color.Blue);
            AddStatistic("Status", interfaceInstance.Status.ToString(), 
                interfaceInstance.Status == InterfaceStatus.Running ? Color.Green : Color.Red);
            AddStatistic("Service Name", interfaceInstance.ServiceName, Color.Black);
            AddStatistic("Points Count", interfaceInstance.PointsCount.ToString(), Color.Black);
            AddStatistic("Update Rate", $"{interfaceInstance.UpdateRate:F2} updates/sec", Color.Black);
            AddStatistic("Last Update", interfaceInstance.LastUpdate.ToString("yyyy-MM-dd HH:mm:ss"), Color.Black);
            AddStatistic("Uptime", interfaceInstance.Uptime.ToString(@"dd\.hh\:mm\:ss"), Color.Black);
            AddStatistic("CPU Usage", $"{interfaceInstance.CpuUsage:F1}%", Color.Black);
            AddStatistic("Memory Usage", $"{interfaceInstance.MemoryUsage:F1} MB", Color.Black);
            AddStatistic("Good Points", interfaceInstance.GoodPoints.ToString(), Color.Green);
            AddStatistic("Bad Points", interfaceInstance.BadPoints.ToString(), Color.Red);
            AddStatistic("Total Events", interfaceInstance.TotalEvents.ToString(), Color.Black);
            AddStatistic("Events Per Second", $"{interfaceInstance.EventsPerSecond:F2}", Color.Black);
        }

        private void AddStatistic(string property, string value, Color color)
        {
            var item = new ListViewItem(property);
            item.SubItems.Add(value);
            item.ForeColor = color;
            statisticsListView!.Items.Add(item);
        }

        private void RefreshButton_Click(object? sender, EventArgs e)
        {
            LoadStatistics();
        }
    }
} 