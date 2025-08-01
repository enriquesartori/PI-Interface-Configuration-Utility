using System;
using System.Drawing;
using System.Windows.Forms;

namespace PIInterfaceConfigUtility
{
    public partial class LogsViewerControl : UserControl
    {
            private RichTextBox? logsTextBox;
    private ToolStrip? toolStrip;
    private ComboBox? logLevelComboBox;
    private Button? clearButton, saveButton;
    private CheckBox? autoScrollCheckBox;
        private System.Windows.Forms.Timer? logTimer;
        
        public LogsViewerControl()
        {
            InitializeComponent();
            SetupEventHandlers();
            StartLogSimulation();
        }
        
        private void InitializeComponent()
        {
            this.SuspendLayout();
            
            this.Size = new Size(800, 600);
            this.BackColor = Color.White;
            this.Dock = DockStyle.Fill;
            
            SetupToolStrip();
            SetupLogsTextBox();
            
            this.ResumeLayout(false);
        }
        
        private void SetupToolStrip()
        {
            toolStrip = new ToolStrip();
            toolStrip.ImageScalingSize = new Size(24, 24);
            
            // Log level filter
            var logLevelLabel = new ToolStripLabel("Log Level:");
            logLevelComboBox = new ComboBox
            {
                DropDownStyle = ComboBoxStyle.DropDownList,
                Width = 100
            };
            logLevelComboBox.Items.AddRange(new[] { "All", "Error", "Warning", "Info", "Debug" });
            logLevelComboBox.SelectedIndex = 0;
            
            var logLevelHost = new ToolStripControlHost(logLevelComboBox);
            
            // Clear button
            clearButton = new Button
            {
                Text = "Clear",
                Size = new Size(60, 23),
                BackColor = Color.FromArgb(108, 117, 125),
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat
            };
            var clearHost = new ToolStripControlHost(clearButton);
            
            // Save button
            saveButton = new Button
            {
                Text = "Save Logs",
                Size = new Size(80, 23),
                BackColor = Color.FromArgb(0, 120, 215),
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat
            };
            var saveHost = new ToolStripControlHost(saveButton);
            
            // Auto-scroll checkbox
            autoScrollCheckBox = new CheckBox
            {
                Text = "Auto Scroll",
                Checked = true,
                Size = new Size(90, 23)
            };
            var autoScrollHost = new ToolStripControlHost(autoScrollCheckBox);
            
            toolStrip.Items.AddRange(new ToolStripItem[]
            {
                logLevelLabel,
                logLevelHost,
                new ToolStripSeparator(),
                clearHost,
                saveHost,
                new ToolStripSeparator(),
                autoScrollHost
            });
            
            this.Controls.Add(toolStrip);
        }
        
        private void SetupLogsTextBox()
        {
            logsTextBox = new RichTextBox
            {
                Dock = DockStyle.Fill,
                ReadOnly = true,
                BackColor = Color.Black,
                ForeColor = Color.White,
                Font = new Font("Consolas", 9F),
                ScrollBars = RichTextBoxScrollBars.Vertical
            };
            
            this.Controls.Add(logsTextBox);
        }
        
        private void SetupEventHandlers()
        {
            clearButton!.Click += ClearButton_Click;
            saveButton!.Click += SaveButton_Click;
            logLevelComboBox!.SelectedIndexChanged += LogLevelComboBox_SelectedIndexChanged;
        }
        
        private void StartLogSimulation()
        {
            // Add initial welcome message
            AppendLog("INFO", "PI Interface Configuration Utility started");
            AppendLog("INFO", "Log viewer initialized");
            
            // Start timer for simulated log entries
            logTimer = new System.Windows.Forms.Timer();
            logTimer.Interval = 5000; // Add log entry every 5 seconds
            logTimer.Tick += LogTimer_Tick;
            logTimer.Start();
        }
        
        private void LogTimer_Tick(object? sender, EventArgs e)
        {
            // Simulate various log entries
            var random = new Random();
            var logTypes = new[] { "INFO", "DEBUG", "WARNING", "ERROR" };
            var messages = new[]
            {
                "Interface heartbeat received",
                "PI Point value updated",
                "Configuration saved successfully",
                "Connection established",
                "Data collection in progress",
                "Service status checked",
                "Memory usage within normal limits",
                "Periodic maintenance completed",
                "Interface statistics updated",
                "Buffer flush completed"
            };
            
            var logType = logTypes[random.Next(logTypes.Length)];
            var message = messages[random.Next(messages.Length)];
            
            // Occasionally add error or warning messages
            if (random.Next(10) == 0) // 10% chance
            {
                logType = "WARNING";
                message = "Interface response time above threshold";
            }
            else if (random.Next(20) == 0) // 5% chance
            {
                logType = "ERROR";
                message = "Failed to connect to data source";
            }
            
            AppendLog(logType, message);
        }
        
        private void AppendLog(string level, string message)
        {
            if (InvokeRequired)
            {
                Invoke(new Action(() => AppendLog(level, message)));
                return;
            }
            
            var timestamp = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
            var logEntry = $"[{timestamp}] {level.PadRight(7)} {message}";
            
            // Check if this log level should be displayed
            var selectedLevel = logLevelComboBox!.SelectedItem?.ToString();
            if (selectedLevel != "All" && selectedLevel != level)
                return;
            
            // Determine color based on log level
            var color = level switch
            {
                "ERROR" => Color.Red,
                "WARNING" => Color.Yellow,
                "INFO" => Color.LightBlue,
                "DEBUG" => Color.Gray,
                _ => Color.White
            };
            
            logsTextBox!.SelectionStart = logsTextBox.TextLength;
            logsTextBox.SelectionLength = 0;
            logsTextBox.SelectionColor = color;
            logsTextBox.AppendText(logEntry + Environment.NewLine);
            logsTextBox.SelectionColor = logsTextBox.ForeColor;
            
            // Auto-scroll if enabled
            if (autoScrollCheckBox!.Checked)
            {
                logsTextBox.SelectionStart = logsTextBox.Text.Length;
                logsTextBox.ScrollToCaret();
            }
            
            // Limit log size to prevent memory issues
            if (logsTextBox.Lines.Length > 1000)
            {
                var lines = logsTextBox.Lines;
                var newLines = new string[800];
                Array.Copy(lines, lines.Length - 800, newLines, 0, 800);
                logsTextBox.Lines = newLines;
            }
        }
        
        private void ClearButton_Click(object? sender, EventArgs e)
        {
            logsTextBox!.Clear();
            AppendLog("INFO", "Log viewer cleared");
        }
        
        private void SaveButton_Click(object? sender, EventArgs e)
        {
            using var saveDialog = new SaveFileDialog();
            saveDialog.Filter = "Text Files (*.txt)|*.txt|Log Files (*.log)|*.log|All Files (*.*)|*.*";
            saveDialog.DefaultExt = "txt";
            saveDialog.FileName = $"PIInterfaceUtility_Logs_{DateTime.Now:yyyyMMdd_HHmmss}.txt";
            
            if (saveDialog.ShowDialog() == DialogResult.OK)
            {
                try
                {
                    System.IO.File.WriteAllText(saveDialog.FileName, logsTextBox!.Text);
                    AppendLog("INFO", $"Logs saved to: {saveDialog.FileName}");
                    MessageBox.Show("Logs saved successfully!", "Save Complete",
                        MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
                catch (Exception ex)
                {
                    AppendLog("ERROR", $"Failed to save logs: {ex.Message}");
                    MessageBox.Show($"Error saving logs: {ex.Message}", "Save Error",
                        MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }
        
        private void LogLevelComboBox_SelectedIndexChanged(object? sender, EventArgs e)
        {
            // Filter would be implemented here in a full version
            // For now, it only affects new incoming logs
        }
        
        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                logTimer?.Stop();
                logTimer?.Dispose();
            }
            base.Dispose(disposing);
        }
    }
} 