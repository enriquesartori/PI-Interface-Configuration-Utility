using System;
using System.Drawing;
using System.Windows.Forms;
using PIInterfaceConfigUtility.Services;
using System.Threading.Tasks; // Added missing import for Task
using System.Linq; // Added missing import for Count

namespace PIInterfaceConfigUtility
{
    public partial class DiagnosticsControl : UserControl
    {
        private readonly PIServerManager piServerManager;
        private readonly InterfaceManager interfaceManager;
            private RichTextBox? diagnosticsTextBox;
    private Button? runDiagnosticsButton, clearButton;
    private System.Windows.Forms.Timer? updateTimer;
        
        public DiagnosticsControl(PIServerManager serverManager, InterfaceManager manager)
        {
            piServerManager = serverManager ?? throw new ArgumentNullException(nameof(serverManager));
            interfaceManager = manager ?? throw new ArgumentNullException(nameof(manager));
            InitializeComponent();
            SetupEventHandlers();
        }
        
        private void InitializeComponent()
        {
            this.SuspendLayout();
            
            this.Size = new Size(800, 600);
            this.BackColor = Color.White;
            this.Dock = DockStyle.Fill;
            
            // Buttons
            runDiagnosticsButton = new Button
            {
                Text = "Run Diagnostics",
                Location = new Point(10, 10),
                Size = new Size(120, 30),
                BackColor = Color.FromArgb(0, 120, 215),
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat
            };
            
            clearButton = new Button
            {
                Text = "Clear",
                Location = new Point(140, 10),
                Size = new Size(80, 30),
                BackColor = Color.FromArgb(108, 117, 125),
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat
            };
            
            // Diagnostics text box
            diagnosticsTextBox = new RichTextBox
            {
                Location = new Point(10, 50),
                Size = new Size(780, 540),
                ReadOnly = true,
                BackColor = Color.Black,
                ForeColor = Color.LightGreen,
                Font = new Font("Consolas", 9F),
                ScrollBars = RichTextBoxScrollBars.Vertical
            };
            
            this.Controls.AddRange(new Control[]
            {
                runDiagnosticsButton, clearButton, diagnosticsTextBox
            });
            
            // Update timer
            updateTimer = new System.Windows.Forms.Timer();
            updateTimer.Interval = 10000; // Update every 10 seconds
            updateTimer.Tick += UpdateTimer_Tick;
            updateTimer.Start();
            
            this.ResumeLayout(false);
        }
        
        private void SetupEventHandlers()
        {
            runDiagnosticsButton!.Click += RunDiagnosticsButton_Click;
            clearButton!.Click += ClearButton_Click;
        }
        
        private async void RunDiagnosticsButton_Click(object? sender, EventArgs e)
        {
            runDiagnosticsButton!.Enabled = false;
            runDiagnosticsButton.Text = "Running...";
            
            try
            {
                ClearButton_Click(null, EventArgs.Empty);
                await RunDiagnostics();
            }
            finally
            {
                runDiagnosticsButton!.Enabled = true;
                runDiagnosticsButton.Text = "Run Diagnostics";
            }
        }
        
        private void ClearButton_Click(object? sender, EventArgs e)
        {
            diagnosticsTextBox!.Clear();
        }
        
        private async void UpdateTimer_Tick(object? sender, EventArgs e)
        {
            if (runDiagnosticsButton.Enabled) // Only auto-update when not manually running
            {
                await RunDiagnostics();
            }
        }
        
        private async Task RunDiagnostics()
        {
            AppendText("=== PI Interface Configuration Utility Diagnostics ===", Color.Yellow);
            AppendText($"Timestamp: {DateTime.Now:yyyy-MM-dd HH:mm:ss}", Color.White);
            AppendText("", Color.White);
            
            // System Information
            AppendText("--- System Information ---", Color.Cyan);
            AppendText($"OS Version: {Environment.OSVersion}", Color.White);
            AppendText($"Machine Name: {Environment.MachineName}", Color.White);
            AppendText($"User: {Environment.UserName}", Color.White);
            AppendText($".NET Version: {Environment.Version}", Color.White);
            AppendText($"Working Directory: {Environment.CurrentDirectory}", Color.White);
            AppendText("", Color.White);
            
            // PI Server Connection Status
            AppendText("--- PI Server Connection ---", Color.Cyan);
            if (piServerManager.IsConnected && piServerManager.CurrentConnection != null)
            {
                AppendText($"Status: Connected", Color.Green);
                AppendText($"Server: {piServerManager.CurrentConnection.ServerName}", Color.White);
                AppendText($"Port: {piServerManager.CurrentConnection.Port}", Color.White);
                AppendText($"Connected Since: {piServerManager.CurrentConnection.LastConnected:yyyy-MM-dd HH:mm:ss}", Color.White);
                AppendText($"PI Points Loaded: {piServerManager.PIPoints.Count}", Color.White);
            }
            else
            {
                AppendText("Status: Disconnected", Color.Red);
                AppendText("No PI Server connection available", Color.Yellow);
            }
            AppendText("", Color.White);
            
            // Interface Status
            AppendText("--- Interface Status ---", Color.Cyan);
            var interfaces = interfaceManager.Interfaces;
            if (interfaces.Count > 0)
            {
                AppendText($"Total Interfaces: {interfaces.Count}", Color.White);
                
                var runningCount = interfaces.Count(i => i.Status == Models.InterfaceStatus.Running);
                var stoppedCount = interfaces.Count(i => i.Status == Models.InterfaceStatus.Stopped);
                var errorCount = interfaces.Count(i => i.Status == Models.InterfaceStatus.Error);
                
                AppendText($"Running: {runningCount}", runningCount > 0 ? Color.Green : Color.Gray);
                AppendText($"Stopped: {stoppedCount}", stoppedCount > 0 ? Color.Yellow : Color.Gray);
                AppendText($"Errors: {errorCount}", errorCount > 0 ? Color.Red : Color.Gray);
                
                AppendText("", Color.White);
                AppendText("Interface Details:", Color.White);
                foreach (var iface in interfaces)
                {
                    var statusColor = iface.Status switch
                    {
                        Models.InterfaceStatus.Running => Color.Green,
                        Models.InterfaceStatus.Error => Color.Red,
                        Models.InterfaceStatus.Stopped => Color.Yellow,
                        _ => Color.Gray
                    };
                    AppendText($"  {iface.Name} ({iface.Type}): {iface.Status}", statusColor);
                }
            }
            else
            {
                AppendText("No interfaces configured", Color.Yellow);
            }
            AppendText("", Color.White);
            
            // Memory Usage
            AppendText("--- Memory Usage ---", Color.Cyan);
            var workingSet = Environment.WorkingSet / (1024 * 1024);
            AppendText($"Working Set: {workingSet:N0} MB", Color.White);
            
            GC.Collect();
            var totalMemory = GC.GetTotalMemory(false) / (1024 * 1024);
            AppendText($"Managed Memory: {totalMemory:N0} MB", Color.White);
            AppendText("", Color.White);
            
            // Network Connectivity Test
            AppendText("--- Network Connectivity ---", Color.Cyan);
            try
            {
                if (piServerManager.IsConnected && piServerManager.CurrentConnection != null)
                {
                    var testResult = await piServerManager.TestConnectionAsync(piServerManager.CurrentConnection.ServerName);
                    AppendText($"PI Server Connectivity: {(testResult ? "OK" : "Failed")}", testResult ? Color.Green : Color.Red);
                }
                else
                {
                    AppendText("PI Server Connectivity: Not Connected", Color.Yellow);
                }
            }
            catch (Exception ex)
            {
                AppendText($"PI Server Connectivity: Error - {ex.Message}", Color.Red);
            }
            
            AppendText("", Color.White);
            AppendText("=== Diagnostics Complete ===", Color.Yellow);
            AppendText("", Color.White);
            
            // Auto-scroll to bottom
            diagnosticsTextBox.SelectionStart = diagnosticsTextBox.Text.Length;
            diagnosticsTextBox.ScrollToCaret();
        }
        
        private void AppendText(string text, Color color)
        {
            if (InvokeRequired)
            {
                Invoke(new Action(() => AppendText(text, color)));
                return;
            }
            
            diagnosticsTextBox.SelectionStart = diagnosticsTextBox.TextLength;
            diagnosticsTextBox.SelectionLength = 0;
            diagnosticsTextBox.SelectionColor = color;
            diagnosticsTextBox.AppendText(text + Environment.NewLine);
            diagnosticsTextBox.SelectionColor = diagnosticsTextBox.ForeColor;
        }
        
        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                updateTimer?.Stop();
                updateTimer?.Dispose();
            }
            base.Dispose(disposing);
        }
    }
} 