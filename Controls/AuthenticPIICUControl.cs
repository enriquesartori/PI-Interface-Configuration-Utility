using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using PIInterfaceConfigUtility.Models;
using PIInterfaceConfigUtility.Services;

namespace PIInterfaceConfigUtility.Controls
{
    /// <summary>
    /// Authentic PI Interface Configuration Utility control that matches the real PI ICU
    /// </summary>
    public partial class AuthenticPIICUControl : UserControl
    {
        private TabControl? mainTabControl;
        private ComboBox? interfaceComboBox;
        private ComboBox? interfaceTypeComboBox;
        private Label? connectionStatusLabel;
        private Button? renameButton;
        private Button? closeButton;
        private Button? applyButton;
        
        // Tab Pages
        private TabPage? generalTabPage;
        private TabPage? interfaceSpecificTabPage; // PIPing, OPC DA, etc.
        private TabPage? serviceTabPage;
        private TabPage? uniIntTabPage;
        private TabPage? uniIntFailoverTabPage;
        private TabPage? uniIntDisconnectedTabPage;
        private TabPage? ioRateTabPage;

        // General Tab Controls
        private TextBox? pointSourceTextBox;
        private TextBox? interfaceIDTextBox;
        private DataGridView? scanClassesGrid;
        private Button? addScanClassButton;
        private Button? deleteScanClassButton;
        private Button? resetScanClassButton;
        private Label? piHostInfoLabel;
        private TextBox? serverCollectiveTextBox;
        private TextBox? sdkMemberTextBox;
        private TextBox? apiHostnameTextBox;
        private TextBox? userTextBox;
        private TextBox? typeTextBox;
        private TextBox? versionTextBox;
        private TextBox? portTextBox;

        // Interface-Specific Tab Controls (Dynamic based on interface type)
        private NumericUpDown? timeoutDurationNumeric;
        private NumericUpDown? threadCountNumeric;
        private TextBox? additionalParametersTextBox;

        // Service Tab Controls
        private TextBox? serviceNameTextBox;
        private TextBox? displayNameTextBox;
        private RadioButton? localSystemRadio;
        private RadioButton? domainUserRadio;
        private ListBox? dependenciesListBox;
        private RadioButton? autoStartupRadio;
        private RadioButton? manualStartupRadio;
        private RadioButton? disabledStartupRadio;
        private Button? createServiceButton;
        private Button? removeServiceButton;

        // UniInt Tab Controls
        private NumericUpDown? maxStopTimeNumeric;
        private NumericUpDown? startupDelayNumeric;
        private NumericUpDown? pointUpdateIntervalNumeric;
        private NumericUpDown? serviceEventsNumeric;
        private NumericUpDown? percentUpNumeric;
        private CheckBox? queueDataCheckBox;
        private CheckBox? bypassExceptionCheckBox;
        private CheckBox? writeStatusOnShutdownCheckBox;
        private ComboBox? shutdownStatusCombo;
        private CheckBox? disableAllOutputsCheckBox;
        private CheckBox? suppressInitialOutputsCheckBox;
        private CheckBox? useEventTimestampCheckBox;
        private CheckBox? useAlternateUTCCheckBox;

        // UniInt Failover Tab Controls
        private CheckBox? enableFailoverCheckBox;
        private RadioButton? phase1Radio;
        private RadioButton? phase2Radio;
        private TextBox? failoverIdThisTextBox;
        private TextBox? failoverIdOtherTextBox;
        private CheckBox? noFailoverCheckBox;
        private CheckBox? failoverControlLogsCheckBox;
        private NumericUpDown? heartbeatRateNumeric;
        private ComboBox? ufoTypeCombo;
        private TextBox? synchronizationFileTextBox;
        private Button? browseUFOButton;

        // UniInt Disconnected Startup Tab Controls
        private CheckBox? enableDisconnectedCheckBox;
        private NumericUpDown? cacheSyncPeriodNumeric;
        private TextBox? cachePathTextBox;
        private Button? browseCacheButton;
        private DataGridView? cacheFilesGrid;
        private Button? renameFilesButton;
        private Button? deleteFilesButton;
        private Button? refreshFilesButton;

        // IO Rate Tab Controls
        private CheckBox? enableIORatesCheckBox;
        private Button? createIORateButton;
        private Button? deleteIORateButton;
        private Button? resetIORateButton;
        private Button? renameIORateButton;
        private Button? addToFileButton;
        private TextBox? eventCounterTextBox;
        private TextBox? tagnameTextBox;
        private TextBox? tagStatusTextBox;
        private TextBox? inFileTextBox;
        private TextBox? snapshotTextBox;

        private PIInterfaceConfiguration currentConfiguration = new();
        private RealPIServerManager? piServerManager;

        public AuthenticPIICUControl()
        {
            InitializeComponent();
            InitializeAuthenticPIICU();
        }

        private void InitializeComponent()
        {
            this.Size = new Size(700, 530);
            this.BackColor = SystemColors.Control;
        }

        private void InitializeAuthenticPIICU()
        {
            this.SuspendLayout();

            // Create main header with interface selection
            CreateHeader();

            // Create main tab control
            CreateMainTabControl();

            // Create all tab pages
            CreateGeneralTab();
            CreateInterfaceSpecificTab();
            CreateServiceTab();
            CreateUniIntTab();
            CreateUniIntFailoverTab();
            CreateUniIntDisconnectedTab();
            CreateIORateTab();

            // Create footer with buttons
            CreateFooter();

            this.ResumeLayout(false);
        }

        private void CreateHeader()
        {
            // Interface dropdown
            var interfaceLabel = new Label
            {
                Text = "Interface:",
                Location = new Point(8, 12),
                Size = new Size(55, 20),
                TextAlign = ContentAlignment.MiddleLeft
            };
            this.Controls.Add(interfaceLabel);

            interfaceComboBox = new ComboBox
            {
                Location = new Point(70, 10),
                Size = new Size(200, 21),
                DropDownStyle = ComboBoxStyle.DropDownList
            };
            interfaceComboBox.Items.AddRange(new[] { "PIPng1 -> TOSDEMO-PISRV" });
            interfaceComboBox.SelectedIndex = 0;
            this.Controls.Add(interfaceComboBox);

            // Rename button
            renameButton = new Button
            {
                Text = "Rename",
                Location = new Point(275, 9),
                Size = new Size(60, 23)
            };
            this.Controls.Add(renameButton);

            // Type dropdown
            var typeLabel = new Label
            {
                Text = "Type:",
                Location = new Point(8, 40),
                Size = new Size(35, 20),
                TextAlign = ContentAlignment.MiddleLeft
            };
            this.Controls.Add(typeLabel);

            interfaceTypeComboBox = new ComboBox
            {
                Location = new Point(70, 38),
                Size = new Size(100, 21),
                DropDownStyle = ComboBoxStyle.DropDownList
            };
            interfaceTypeComboBox.Items.AddRange(new[] { "PIPing", "OPC DA", "Perfmon", "UFL", "RDBMS", "OPC AE" });
            interfaceTypeComboBox.SelectedIndex = 0;
            interfaceTypeComboBox.SelectedIndexChanged += InterfaceTypeComboBox_SelectedIndexChanged;
            this.Controls.Add(interfaceTypeComboBox);

            // PI Png label
            var piPngLabel = new Label
            {
                Text = "PI Png",
                Location = new Point(175, 40),
                Size = new Size(40, 20),
                TextAlign = ContentAlignment.MiddleLeft
            };
            this.Controls.Add(piPngLabel);

            // Connection status
            connectionStatusLabel = new Label
            {
                Text = "PI Data server Connection Status\n✓ TOSDEMO-PISRV\n   Writeable",
                Location = new Point(520, 10),
                Size = new Size(170, 50),
                TextAlign = ContentAlignment.TopLeft,
                BackColor = Color.LightGreen,
                BorderStyle = BorderStyle.FixedSingle
            };
            this.Controls.Add(connectionStatusLabel);
        }

        private void CreateMainTabControl()
        {
            mainTabControl = new TabControl
            {
                Location = new Point(8, 70),
                Size = new Size(680, 420),
                SelectedIndex = 0
            };
            this.Controls.Add(mainTabControl);
        }

        private void CreateGeneralTab()
        {
            generalTabPage = new TabPage("General");
            mainTabControl!.TabPages.Add(generalTabPage);

            // Point Source
            var pointSourceLabel = new Label
            {
                Text = "Point Source:",
                Location = new Point(10, 20),
                Size = new Size(80, 20)
            };
            generalTabPage.Controls.Add(pointSourceLabel);

            pointSourceTextBox = new TextBox
            {
                Text = "PING",
                Location = new Point(95, 18),
                Size = new Size(120, 20)
            };
            generalTabPage.Controls.Add(pointSourceTextBox);

            // Interface ID
            var interfaceIDLabel = new Label
            {
                Text = "Interface ID:",
                Location = new Point(10, 50),
                Size = new Size(80, 20)
            };
            generalTabPage.Controls.Add(interfaceIDLabel);

            interfaceIDTextBox = new TextBox
            {
                Text = "1",
                Location = new Point(95, 48),
                Size = new Size(40, 20)
            };
            generalTabPage.Controls.Add(interfaceIDTextBox);

            // Scan Classes section
            var scanClassesLabel = new Label
            {
                Text = "Scan Classes",
                Location = new Point(10, 80),
                Size = new Size(100, 20),
                Font = new Font(SystemFonts.DefaultFont, FontStyle.Bold)
            };
            generalTabPage.Controls.Add(scanClassesLabel);

            scanClassesGrid = new DataGridView
            {
                Location = new Point(10, 105),
                Size = new Size(300, 120),
                AllowUserToAddRows = false,
                AllowUserToDeleteRows = false,
                SelectionMode = DataGridViewSelectionMode.FullRowSelect
            };
            
            // Add scan classes columns
            scanClassesGrid.Columns.Add("ScanFrequency", "Scan Frequency");
            scanClassesGrid.Columns.Add("ClassNumber", "Scan Class #");
            
            // Add sample data
            scanClassesGrid.Rows.Add("00:30:00", "1");
            
            generalTabPage.Controls.Add(scanClassesGrid);

            // Scan class buttons
            addScanClassButton = new Button
            {
                Text = "➕",
                Location = new Point(320, 105),
                Size = new Size(25, 25)
            };
            generalTabPage.Controls.Add(addScanClassButton);

            deleteScanClassButton = new Button
            {
                Text = "❌",
                Location = new Point(320, 135),
                Size = new Size(25, 25)
            };
            generalTabPage.Controls.Add(deleteScanClassButton);

            resetScanClassButton = new Button
            {
                Text = "↻",
                Location = new Point(320, 165),
                Size = new Size(25, 25)
            };
            generalTabPage.Controls.Add(resetScanClassButton);

            // PI Host Information section
            var piHostLabel = new Label
            {
                Text = "PI Host Information",
                Location = new Point(360, 20),
                Size = new Size(120, 20),
                Font = new Font(SystemFonts.DefaultFont, FontStyle.Bold)
            };
            generalTabPage.Controls.Add(piHostLabel);

            CreatePIHostInfoControls(generalTabPage);
        }

        private void CreatePIHostInfoControls(TabPage parent)
        {
            var labels = new[]
            {
                ("Server/Collective:", 50),
                ("SDK Member:", 80),
                ("API Hostname:", 110),
                ("User:", 140),
                ("Type:", 170),
                ("Version:", 200),
                ("Port:", 230)
            };

            var textBoxes = new[]
            {
                (serverCollectiveTextBox = new TextBox(), ""),
                (sdkMemberTextBox = new TextBox(), ""),
                (apiHostnameTextBox = new TextBox(), "TOSDEMO-PISRV"),
                (userTextBox = new TextBox(), "piadmin"),
                (typeTextBox = new TextBox(), "Non-replicated - PI3"),
                (versionTextBox = new TextBox(), "PI 3.4.445.688"),
                (portTextBox = new TextBox(), "5450")
            };

            for (int i = 0; i < labels.Length; i++)
            {
                var label = new Label
                {
                    Text = labels[i].Item1,
                    Location = new Point(360, labels[i].Item2),
                    Size = new Size(100, 20)
                };
                parent.Controls.Add(label);

                textBoxes[i].Item1.Text = textBoxes[i].Item2;
                textBoxes[i].Item1.Location = new Point(470, labels[i].Item2 - 2);
                textBoxes[i].Item1.Size = new Size(150, 20);
                textBoxes[i].Item1.ReadOnly = true;
                textBoxes[i].Item1.BackColor = SystemColors.Control;
                parent.Controls.Add(textBoxes[i].Item1);
            }
        }

        private void CreateInterfaceSpecificTab()
        {
            // This will be dynamically named based on interface type
            interfaceSpecificTabPage = new TabPage("PIPing");
            mainTabControl!.TabPages.Add(interfaceSpecificTabPage);

            UpdateInterfaceSpecificTab();
        }

        private void UpdateInterfaceSpecificTab()
        {
            if (interfaceSpecificTabPage == null) return;

            interfaceSpecificTabPage.Controls.Clear();
            
            var interfaceType = interfaceTypeComboBox?.SelectedItem?.ToString() ?? "PIPing";
            interfaceSpecificTabPage.Text = interfaceType;

            switch (interfaceType)
            {
                case "PIPing":
                    CreatePIPingSpecificControls();
                    break;
                case "OPC DA":
                    CreateOPCDASpecificControls();
                    break;
                case "Perfmon":
                    CreatePerfmonSpecificControls();
                    break;
                case "UFL":
                    CreateUFLSpecificControls();
                    break;
                case "RDBMS":
                    CreateRDBMSSpecificControls();
                    break;
                case "OPC AE":
                    CreateOPCAESpecificControls();
                    break;
            }
        }

        private void CreatePIPingSpecificControls()
        {
            // PI Png Interface-Specific Parameters header
            var headerLabel = new Label
            {
                Text = "PI Png Interface-Specific Parameters (2.0.0.20)",
                Location = new Point(10, 20),
                Size = new Size(300, 20),
                Font = new Font(SystemFonts.DefaultFont, FontStyle.Bold)
            };
            interfaceSpecificTabPage!.Controls.Add(headerLabel);

            // Set Timeout Duration
            var timeoutLabel = new Label
            {
                Text = "Set Timeout Duration (seconds):",
                Location = new Point(30, 50),
                Size = new Size(180, 20)
            };
            interfaceSpecificTabPage.Controls.Add(timeoutLabel);

            timeoutDurationNumeric = new NumericUpDown
            {
                Minimum = 1,
                Maximum = 999,
                Value = 3,
                Location = new Point(220, 48),
                Size = new Size(60, 20)
            };
            interfaceSpecificTabPage.Controls.Add(timeoutDurationNumeric);

            // Set Thread Count
            var threadLabel = new Label
            {
                Text = "Set Thread Count:",
                Location = new Point(30, 80),
                Size = new Size(120, 20)
            };
            interfaceSpecificTabPage.Controls.Add(threadLabel);

            threadCountNumeric = new NumericUpDown
            {
                Minimum = 1,
                Maximum = 500,
                Value = 10,
                Location = new Point(220, 78),
                Size = new Size(60, 20)
            };
            interfaceSpecificTabPage.Controls.Add(threadCountNumeric);

            // Additional Parameters
            var additionalLabel = new Label
            {
                Text = "Additional Parameters",
                Location = new Point(10, 120),
                Size = new Size(150, 20),
                Font = new Font(SystemFonts.DefaultFont, FontStyle.Bold)
            };
            interfaceSpecificTabPage.Controls.Add(additionalLabel);

            additionalParametersTextBox = new TextBox
            {
                Location = new Point(10, 145),
                Size = new Size(650, 200),
                Multiline = true,
                ScrollBars = ScrollBars.Both
            };
            interfaceSpecificTabPage.Controls.Add(additionalParametersTextBox);
        }

        private void CreateOPCDASpecificControls()
        {
            // OPC DA specific parameters
            var headerLabel = new Label
            {
                Text = "OPC DA Interface-Specific Parameters",
                Location = new Point(10, 20),
                Size = new Size(300, 20),
                Font = new Font(SystemFonts.DefaultFont, FontStyle.Bold)
            };
            interfaceSpecificTabPage!.Controls.Add(headerLabel);

            // OPC Server Configuration
            var opcServerLabel = new Label
            {
                Text = "OPC Server:",
                Location = new Point(30, 50),
                Size = new Size(80, 20)
            };
            interfaceSpecificTabPage.Controls.Add(opcServerLabel);

            var opcServerTextBox = new TextBox
            {
                Location = new Point(120, 48),
                Size = new Size(200, 20),
                Text = "Matrikon.OPC.Simulation.1"
            };
            interfaceSpecificTabPage.Controls.Add(opcServerTextBox);

            // Group Settings
            var groupLabel = new Label
            {
                Text = "Group Update Rate (ms):",
                Location = new Point(30, 80),
                Size = new Size(140, 20)
            };
            interfaceSpecificTabPage.Controls.Add(groupLabel);

            var groupRateNumeric = new NumericUpDown
            {
                Value = 1000,
                Minimum = 100,
                Maximum = 60000,
                Location = new Point(180, 78),
                Size = new Size(80, 20)
            };
            interfaceSpecificTabPage.Controls.Add(groupRateNumeric);
        }

        private void CreatePerfmonSpecificControls()
        {
            var headerLabel = new Label
            {
                Text = "Performance Monitor Interface Parameters",
                Location = new Point(10, 20),
                Size = new Size(300, 20),
                Font = new Font(SystemFonts.DefaultFont, FontStyle.Bold)
            };
            interfaceSpecificTabPage!.Controls.Add(headerLabel);

            // Performance counters configuration
            var countersLabel = new Label
            {
                Text = "Performance Counters:",
                Location = new Point(30, 50),
                Size = new Size(130, 20)
            };
            interfaceSpecificTabPage.Controls.Add(countersLabel);

            var countersListBox = new ListBox
            {
                Location = new Point(30, 75),
                Size = new Size(400, 150)
            };
            countersListBox.Items.AddRange(new[] 
            {
                "\\Processor(_Total)\\% Processor Time",
                "\\Memory\\Available MBytes",
                "\\System\\Processes"
            });
            interfaceSpecificTabPage.Controls.Add(countersListBox);
        }

        private void CreateUFLSpecificControls()
        {
            var headerLabel = new Label
            {
                Text = "Universal File Loader Interface Parameters",
                Location = new Point(10, 20),
                Size = new Size(300, 20),
                Font = new Font(SystemFonts.DefaultFont, FontStyle.Bold)
            };
            interfaceSpecificTabPage!.Controls.Add(headerLabel);

            // File path configuration
            var filePathLabel = new Label
            {
                Text = "Input File Path:",
                Location = new Point(30, 50),
                Size = new Size(100, 20)
            };
            interfaceSpecificTabPage.Controls.Add(filePathLabel);

            var filePathTextBox = new TextBox
            {
                Location = new Point(140, 48),
                Size = new Size(300, 20),
                Text = "C:\\PI\\Interfaces\\UFL\\*.csv"
            };
            interfaceSpecificTabPage.Controls.Add(filePathTextBox);
        }

        private void CreateRDBMSSpecificControls()
        {
            var headerLabel = new Label
            {
                Text = "RDBMS Interface Parameters",
                Location = new Point(10, 20),
                Size = new Size(300, 20),
                Font = new Font(SystemFonts.DefaultFont, FontStyle.Bold)
            };
            interfaceSpecificTabPage!.Controls.Add(headerLabel);

            // Database connection
            var dbLabel = new Label
            {
                Text = "Connection String:",
                Location = new Point(30, 50),
                Size = new Size(120, 20)
            };
            interfaceSpecificTabPage.Controls.Add(dbLabel);

            var dbTextBox = new TextBox
            {
                Location = new Point(30, 75),
                Size = new Size(500, 20),
                Text = "Server=localhost;Database=ProcessData;Integrated Security=true;"
            };
            interfaceSpecificTabPage.Controls.Add(dbTextBox);
        }

        private void CreateOPCAESpecificControls()
        {
            var headerLabel = new Label
            {
                Text = "OPC Alarms & Events Interface Parameters",
                Location = new Point(10, 20),
                Size = new Size(300, 20),
                Font = new Font(SystemFonts.DefaultFont, FontStyle.Bold)
            };
            interfaceSpecificTabPage!.Controls.Add(headerLabel);

            // OPC AE Server
            var aeServerLabel = new Label
            {
                Text = "OPC AE Server:",
                Location = new Point(30, 50),
                Size = new Size(100, 20)
            };
            interfaceSpecificTabPage.Controls.Add(aeServerLabel);

            var aeServerTextBox = new TextBox
            {
                Location = new Point(140, 48),
                Size = new Size(200, 20),
                Text = "Matrikon.OPC.AlarmEvent.1"
            };
            interfaceSpecificTabPage.Controls.Add(aeServerTextBox);
        }

        private void CreateServiceTab()
        {
            serviceTabPage = new TabPage("Service");
            mainTabControl!.TabPages.Add(serviceTabPage);

            // Service Configuration header
            var serviceConfigLabel = new Label
            {
                Text = "Service Configuration",
                Location = new Point(10, 20),
                Size = new Size(150, 20),
                Font = new Font(SystemFonts.DefaultFont, FontStyle.Bold)
            };
            serviceTabPage.Controls.Add(serviceConfigLabel);

            // Service name
            var serviceNameLabel = new Label
            {
                Text = "Service name:",
                Location = new Point(10, 50),
                Size = new Size(80, 20)
            };
            serviceTabPage.Controls.Add(serviceNameLabel);

            serviceNameTextBox = new TextBox
            {
                Text = "PIPng1",
                Location = new Point(100, 48),
                Size = new Size(120, 20)
            };
            serviceTabPage.Controls.Add(serviceNameTextBox);

            // Display name
            var displayNameLabel = new Label
            {
                Text = "Display name:",
                Location = new Point(10, 80),
                Size = new Size(80, 20)
            };
            serviceTabPage.Controls.Add(displayNameLabel);

            displayNameTextBox = new TextBox
            {
                Text = "PI-PIPng1",
                Location = new Point(100, 78),
                Size = new Size(120, 20)
            };
            serviceTabPage.Controls.Add(displayNameTextBox);

            // Log on as
            var logonLabel = new Label
            {
                Text = "Log on as:",
                Location = new Point(10, 110),
                Size = new Size(80, 20)
            };
            serviceTabPage.Controls.Add(logonLabel);

            localSystemRadio = new RadioButton
            {
                Text = "LocalSystem",
                Location = new Point(100, 110),
                Size = new Size(100, 20),
                Checked = true
            };
            serviceTabPage.Controls.Add(localSystemRadio);

            domainUserRadio = new RadioButton
            {
                Text = "Domain\\UserName",
                Location = new Point(100, 135),
                Size = new Size(130, 20)
            };
            serviceTabPage.Controls.Add(domainUserRadio);

            // Dependencies
            var dependenciesLabel = new Label
            {
                Text = "Dependencies:",
                Location = new Point(10, 170),
                Size = new Size(80, 20)
            };
            serviceTabPage.Controls.Add(dependenciesLabel);

            dependenciesListBox = new ListBox
            {
                Location = new Point(100, 170),
                Size = new Size(200, 80)
            };
            dependenciesListBox.Items.Add("tcpip");
            serviceTabPage.Controls.Add(dependenciesListBox);

            // Startup Type section
            var startupTypeLabel = new Label
            {
                Text = "Startup Type",
                Location = new Point(350, 20),
                Size = new Size(100, 20),
                Font = new Font(SystemFonts.DefaultFont, FontStyle.Bold)
            };
            serviceTabPage.Controls.Add(startupTypeLabel);

            autoStartupRadio = new RadioButton
            {
                Text = "Auto",
                Location = new Point(350, 50),
                Size = new Size(60, 20)
            };
            serviceTabPage.Controls.Add(autoStartupRadio);

            manualStartupRadio = new RadioButton
            {
                Text = "Manual",
                Location = new Point(350, 75),
                Size = new Size(70, 20),
                Checked = true
            };
            serviceTabPage.Controls.Add(manualStartupRadio);

            disabledStartupRadio = new RadioButton
            {
                Text = "Disabled",
                Location = new Point(350, 100),
                Size = new Size(80, 20)
            };
            serviceTabPage.Controls.Add(disabledStartupRadio);

            // Create/Remove buttons
            var createRemoveLabel = new Label
            {
                Text = "Create / Remove",
                Location = new Point(500, 20),
                Size = new Size(100, 20),
                Font = new Font(SystemFonts.DefaultFont, FontStyle.Bold)
            };
            serviceTabPage.Controls.Add(createRemoveLabel);

            createServiceButton = new Button
            {
                Text = "Create",
                Location = new Point(500, 50),
                Size = new Size(70, 25)
            };
            serviceTabPage.Controls.Add(createServiceButton);

            removeServiceButton = new Button
            {
                Text = "Remove",
                Location = new Point(500, 80),
                Size = new Size(70, 25)
            };
            serviceTabPage.Controls.Add(removeServiceButton);

            // Installed services section
            var installedLabel = new Label
            {
                Text = "Installed services:",
                Location = new Point(350, 140),
                Size = new Size(100, 20)
            };
            serviceTabPage.Controls.Add(installedLabel);

            var installedServicesListBox = new ListBox
            {
                Location = new Point(350, 165),
                Size = new Size(300, 150)
            };
            installedServicesListBox.Items.AddRange(new[] 
            {
                "AFService",
                "AJRouter", 
                "ALG",
                "AppHostSvc",
                "AppIDSvc",
                "PI AF Application Service"
            });
            serviceTabPage.Controls.Add(installedServicesListBox);
        }

        private void CreateUniIntTab()
        {
            uniIntTabPage = new TabPage("UniInt");
            mainTabControl!.TabPages.Add(uniIntTabPage);

            // Performance and Behavior section
            var perfBehaviorLabel = new Label
            {
                Text = "Performance and Behavior",
                Location = new Point(10, 20),
                Size = new Size(170, 20),
                Font = new Font(SystemFonts.DefaultFont, FontStyle.Bold)
            };
            uniIntTabPage.Controls.Add(perfBehaviorLabel);

            CreatePerformanceBehaviorControls();
            CreateDataHandlingControls();
            CreateOutputsControls();
            CreateTimestampsControls();
        }

        private void CreatePerformanceBehaviorControls()
        {
            var controls = new[]
            {
                ("Maximum stop time(sec.):", maxStopTimeNumeric = new NumericUpDown { Maximum = 9999, Value = 120 }, new Button { Text = "Reset" }),
                ("Startup delay (sec.):", startupDelayNumeric = new NumericUpDown { Maximum = 9999, Value = 30 }, new Button { Text = "Reset" }),
                ("Point update interval (sec.):", pointUpdateIntervalNumeric = new NumericUpDown { Maximum = 9999, Value = 120 }, new Button { Text = "Reset" }),
                ("Service events per second (ms):", serviceEventsNumeric = new NumericUpDown { Maximum = 9999, Value = 500 }, new Button { Text = "Reset" }),
                ("Percent Up:", percentUpNumeric = new NumericUpDown { Maximum = 100, Value = 100 }, new Button { Text = "Reset" })
            };

            for (int i = 0; i < controls.Length; i++)
            {
                var label = new Label
                {
                    Text = controls[i].Item1,
                    Location = new Point(10, 50 + i * 30),
                    Size = new Size(180, 20)
                };
                uniIntTabPage!.Controls.Add(label);

                controls[i].Item2.Location = new Point(200, 48 + i * 30);
                controls[i].Item2.Size = new Size(80, 20);
                uniIntTabPage.Controls.Add(controls[i].Item2);

                controls[i].Item3.Location = new Point(290, 47 + i * 30);
                controls[i].Item3.Size = new Size(50, 22);
                uniIntTabPage.Controls.Add(controls[i].Item3);
            }

            // Additional checkboxes
            var checkboxes = new[]
            {
                ("API Connection name:", new CheckBox()),
                ("Disable UniInt performance counters", new CheckBox()),
                ("Include Point Source in the header of log messages", new CheckBox()),
                ("Include UFO ID in log messages", new CheckBox())
            };

            for (int i = 0; i < checkboxes.Length; i++)
            {
                var checkbox = checkboxes[i].Item2;
                checkbox.Text = checkboxes[i].Item1;
                checkbox.Location = new Point(10, 200 + i * 25);
                checkbox.Size = new Size(300, 20);
                uniIntTabPage!.Controls.Add(checkbox);
            }
        }

        private void CreateDataHandlingControls()
        {
            var dataHandlingLabel = new Label
            {
                Text = "Data Handling",
                Location = new Point(350, 20),
                Size = new Size(100, 20),
                Font = new Font(SystemFonts.DefaultFont, FontStyle.Bold)
            };
            uniIntTabPage!.Controls.Add(dataHandlingLabel);

            queueDataCheckBox = new CheckBox
            {
                Text = "Queue data (for active interfaces)",
                Location = new Point(350, 50),
                Size = new Size(200, 20)
            };
            uniIntTabPage.Controls.Add(queueDataCheckBox);

            bypassExceptionCheckBox = new CheckBox
            {
                Text = "Bypass exception",
                Location = new Point(350, 75),
                Size = new Size(150, 20)
            };
            uniIntTabPage.Controls.Add(bypassExceptionCheckBox);

            writeStatusOnShutdownCheckBox = new CheckBox
            {
                Text = "Write status to tags on shutdown:",
                Location = new Point(350, 100),
                Size = new Size(200, 20),
                Checked = true
            };
            uniIntTabPage.Controls.Add(writeStatusOnShutdownCheckBox);

            shutdownStatusCombo = new ComboBox
            {
                Location = new Point(350, 125),
                Size = new Size(100, 20),
                DropDownStyle = ComboBoxStyle.DropDownList
            };
            shutdownStatusCombo.Items.AddRange(new[] { "Int Shut", "Comm Fail", "I/O Timeout" });
            shutdownStatusCombo.SelectedIndex = 0;
            uniIntTabPage.Controls.Add(shutdownStatusCombo);
        }

        private void CreateOutputsControls()
        {
            var outputsLabel = new Label
            {
                Text = "Outputs",
                Location = new Point(350, 160),
                Size = new Size(60, 20),
                Font = new Font(SystemFonts.DefaultFont, FontStyle.Bold)
            };
            uniIntTabPage!.Controls.Add(outputsLabel);

            disableAllOutputsCheckBox = new CheckBox
            {
                Text = "Disable all outputs from PI",
                Location = new Point(350, 185),
                Size = new Size(180, 20)
            };
            uniIntTabPage.Controls.Add(disableAllOutputsCheckBox);

            suppressInitialOutputsCheckBox = new CheckBox
            {
                Text = "Suppress initial outputs from PI",
                Location = new Point(350, 210),
                Size = new Size(200, 20)
            };
            uniIntTabPage.Controls.Add(suppressInitialOutputsCheckBox);

            useEventTimestampCheckBox = new CheckBox
            {
                Text = "Use event timestamp instead of\ncurrent timestamp for digital outputs",
                Location = new Point(350, 235),
                Size = new Size(200, 40)
            };
            uniIntTabPage.Controls.Add(useEventTimestampCheckBox);
        }

        private void CreateTimestampsControls()
        {
            var timestampsLabel = new Label
            {
                Text = "Timestamps",
                Location = new Point(10, 320),
                Size = new Size(80, 20),
                Font = new Font(SystemFonts.DefaultFont, FontStyle.Bold)
            };
            uniIntTabPage!.Controls.Add(timestampsLabel);

            var timestampInfoLabel = new Label
            {
                Text = "If the interface computer is in a time zone that is different from the\n" +
                       "PI Server's time zone by an increment of 30 minutes instead of an\n" +
                       "hour or if the interface computer's time settings are incorrect for\n" +
                       "any reason, this option should be specified.",
                Location = new Point(10, 345),
                Size = new Size(400, 60)
            };
            uniIntTabPage.Controls.Add(timestampInfoLabel);

            useAlternateUTCCheckBox = new CheckBox
            {
                Text = "Use alternate method of determining UTC seconds",
                Location = new Point(10, 410),
                Size = new Size(280, 20)
            };
            uniIntTabPage.Controls.Add(useAlternateUTCCheckBox);
        }

        private void CreateUniIntFailoverTab()
        {
            uniIntFailoverTabPage = new TabPage("UniInt Failover");
            mainTabControl!.TabPages.Add(uniIntFailoverTabPage);

            // Enable UniInt Failover
            enableFailoverCheckBox = new CheckBox
            {
                Text = "Enable UniInt Failover",
                Location = new Point(10, 20),
                Size = new Size(150, 20),
                Font = new Font(SystemFonts.DefaultFont, FontStyle.Bold)
            };
            uniIntFailoverTabPage.Controls.Add(enableFailoverCheckBox);

            // Phase selection
            phase1Radio = new RadioButton
            {
                Text = "Phase 1",
                Location = new Point(30, 50),
                Size = new Size(70, 20),
                Checked = true
            };
            uniIntFailoverTabPage.Controls.Add(phase1Radio);

            phase2Radio = new RadioButton
            {
                Text = "Phase 2",
                Location = new Point(110, 50),
                Size = new Size(70, 20)
            };
            uniIntFailoverTabPage.Controls.Add(phase2Radio);

            // Failover IDs
            var failoverIdThisLabel = new Label
            {
                Text = "Failover ID# for this instance:",
                Location = new Point(30, 80),
                Size = new Size(150, 20)
            };
            uniIntFailoverTabPage.Controls.Add(failoverIdThisLabel);

            failoverIdThisTextBox = new TextBox
            {
                Location = new Point(190, 78),
                Size = new Size(100, 20)
            };
            uniIntFailoverTabPage.Controls.Add(failoverIdThisTextBox);

            var failoverIdOtherLabel = new Label
            {
                Text = "Failover ID# of the other instance:",
                Location = new Point(30, 110),
                Size = new Size(170, 20)
            };
            uniIntFailoverTabPage.Controls.Add(failoverIdOtherLabel);

            failoverIdOtherTextBox = new TextBox
            {
                Location = new Point(210, 108),
                Size = new Size(100, 20)
            };
            uniIntFailoverTabPage.Controls.Add(failoverIdOtherTextBox);

            // Additional options
            noFailoverCheckBox = new CheckBox
            {
                Text = "Do not failover when both interfaces lose connection to PI",
                Location = new Point(30, 140),
                Size = new Size(350, 20)
            };
            uniIntFailoverTabPage.Controls.Add(noFailoverCheckBox);

            failoverControlLogsCheckBox = new CheckBox
            {
                Text = "Failover control logs are unsolicited (not scan based)",
                Location = new Point(30, 165),
                Size = new Size(300, 20)
            };
            uniIntFailoverTabPage.Controls.Add(failoverControlLogsCheckBox);

            var heartbeatLabel = new Label
            {
                Text = "Rate at which the heartbeat point is updated/checked:",
                Location = new Point(30, 195),
                Size = new Size(250, 20)
            };
            uniIntFailoverTabPage.Controls.Add(heartbeatLabel);

            heartbeatRateNumeric = new NumericUpDown
            {
                Minimum = 100,
                Maximum = 60000,
                Value = 1000,
                Location = new Point(220, 138),
                Size = new Size(80, 20)
            };
            uniIntFailoverTabPage.Controls.Add(heartbeatRateNumeric);

            var millisecondsLabel = new Label
            {
                Text = "milliseconds",
                Location = new Point(380, 195),
                Size = new Size(80, 20)
            };
            uniIntFailoverTabPage.Controls.Add(millisecondsLabel);

            var resetButton = new Button
            {
                Text = "Reset",
                Location = new Point(470, 192),
                Size = new Size(50, 22)
            };
            uniIntFailoverTabPage.Controls.Add(resetButton);

            // UFO Type
            var ufoTypeLabel = new Label
            {
                Text = "UFO Type:",
                Location = new Point(30, 230),
                Size = new Size(70, 20)
            };
            uniIntFailoverTabPage.Controls.Add(ufoTypeLabel);

            ufoTypeCombo = new ComboBox
            {
                Location = new Point(110, 228),
                Size = new Size(80, 20),
                DropDownStyle = ComboBoxStyle.DropDownList
            };
            ufoTypeCombo.Items.AddRange(new[] { "COLD", "WARM", "HOT" });
            ufoTypeCombo.SelectedIndex = 0;
            uniIntFailoverTabPage.Controls.Add(ufoTypeCombo);

            var syncPathLabel = new Label
            {
                Text = "Synchronization File Path:",
                Location = new Point(30, 260),
                Size = new Size(150, 20)
            };
            uniIntFailoverTabPage.Controls.Add(syncPathLabel);

            synchronizationFileTextBox = new TextBox
            {
                Location = new Point(30, 285),
                Size = new Size(400, 20)
            };
            uniIntFailoverTabPage.Controls.Add(synchronizationFileTextBox);

            browseUFOButton = new Button
            {
                Text = "Browse",
                Location = new Point(440, 284),
                Size = new Size(70, 22)
            };
            uniIntFailoverTabPage.Controls.Add(browseUFOButton);

            // Status display area (table view)
            var statusGrid = new DataGridView
            {
                Location = new Point(30, 320),
                Size = new Size(600, 60),
                AllowUserToAddRows = false,
                AllowUserToDeleteRows = false,
                ColumnHeadersHeight = 25
            };

            statusGrid.Columns.Add("Status", "Status");
            statusGrid.Columns.Add("Tag", "Tag");
            statusGrid.Columns.Add("ExtDesc", "ExtDesc");
            statusGrid.Columns.Add("PointSource", "PointSource");
            statusGrid.Columns.Add("ID", "ID");
            statusGrid.Columns.Add("PointType", "PointType");
            statusGrid.Columns.Add("DigitalSet", "DigitalSet");
            statusGrid.Columns.Add("Compressing", "Compressing");
            statusGrid.Columns.Add("CompDev", "CompDev");
            statusGrid.Columns.Add("CompMax", "CompMax");

            uniIntFailoverTabPage.Controls.Add(statusGrid);
        }

        private void CreateUniIntDisconnectedTab()
        {
            uniIntDisconnectedTabPage = new TabPage("UniInt Disconnected Startup");
            mainTabControl!.TabPages.Add(uniIntDisconnectedTabPage);

            // Enable disconnected startup
            enableDisconnectedCheckBox = new CheckBox
            {
                Text = "Enable disconnected startup (with point caching)",
                Location = new Point(10, 20),
                Size = new Size(280, 20),
                Font = new Font(SystemFonts.DefaultFont, FontStyle.Bold)
            };
            uniIntDisconnectedTabPage.Controls.Add(enableDisconnectedCheckBox);

            // Cache synchronization period
            var cacheSyncLabel = new Label
            {
                Text = "Cache synchronization period:",
                Location = new Point(30, 50),
                Size = new Size(170, 20)
            };
            uniIntDisconnectedTabPage.Controls.Add(cacheSyncLabel);

            cacheSyncPeriodNumeric = new NumericUpDown
            {
                Minimum = 100,
                Maximum = 9999,
                Value = 250,
                Location = new Point(220, 78),
                Size = new Size(80, 20)
            };
            uniIntDisconnectedTabPage.Controls.Add(cacheSyncPeriodNumeric);

            var millisecondsLabel2 = new Label
            {
                Text = "milliseconds",
                Location = new Point(300, 50),
                Size = new Size(80, 20)
            };
            uniIntDisconnectedTabPage.Controls.Add(millisecondsLabel2);

            var resetButton2 = new Button
            {
                Text = "Reset",
                Location = new Point(390, 47),
                Size = new Size(50, 22)
            };
            uniIntDisconnectedTabPage.Controls.Add(resetButton2);

            // Cache Path
            var cachePathLabel = new Label
            {
                Text = "Cache Path:",
                Location = new Point(30, 80),
                Size = new Size(80, 20)
            };
            uniIntDisconnectedTabPage.Controls.Add(cachePathLabel);

            cachePathTextBox = new TextBox
            {
                Location = new Point(30, 105),
                Size = new Size(400, 20)
            };
            uniIntDisconnectedTabPage.Controls.Add(cachePathTextBox);

            browseCacheButton = new Button
            {
                Text = "Browse",
                Location = new Point(440, 104),
                Size = new Size(70, 22)
            };
            uniIntDisconnectedTabPage.Controls.Add(browseCacheButton);

            // Cache files section
            var cacheFilesLabel = new Label
            {
                Text = "Cache files:",
                Location = new Point(30, 140),
                Size = new Size(80, 20)
            };
            uniIntDisconnectedTabPage.Controls.Add(cacheFilesLabel);

            renameFilesButton = new Button
            {
                Text = "Rename Files",
                Location = new Point(120, 138),
                Size = new Size(90, 22)
            };
            uniIntDisconnectedTabPage.Controls.Add(renameFilesButton);

            deleteFilesButton = new Button
            {
                Text = "Delete Files",
                Location = new Point(220, 138),
                Size = new Size(80, 22)
            };
            uniIntDisconnectedTabPage.Controls.Add(deleteFilesButton);

            refreshFilesButton = new Button
            {
                Text = "Refresh",
                Location = new Point(310, 138),
                Size = new Size(70, 22)
            };
            uniIntDisconnectedTabPage.Controls.Add(refreshFilesButton);

            // Cache files grid
            cacheFilesGrid = new DataGridView
            {
                Location = new Point(30, 170),
                Size = new Size(600, 200),
                AllowUserToAddRows = false,
                AllowUserToDeleteRows = false,
                SelectionMode = DataGridViewSelectionMode.FullRowSelect
            };

            cacheFilesGrid.Columns.Add("Type", "Type");
            cacheFilesGrid.Columns.Add("Name", "Name");
            cacheFilesGrid.Columns.Add("Modified", "Modified");
            cacheFilesGrid.Columns.Add("Created", "Created");
            cacheFilesGrid.Columns.Add("SizeKB", "Size (KB)");
            cacheFilesGrid.Columns.Add("FullPath", "Full Path");

            uniIntDisconnectedTabPage.Controls.Add(cacheFilesGrid);
        }

        private void CreateIORateTab()
        {
            ioRateTabPage = new TabPage("IO Rate");
            mainTabControl!.TabPages.Add(ioRateTabPage);

            // Input/Output Rates Tag header
            var ioRateLabel = new Label
            {
                Text = "Input/Output Rates Tag",
                Location = new Point(10, 20),
                Size = new Size(150, 20),
                Font = new Font(SystemFonts.DefaultFont, FontStyle.Bold)
            };
            ioRateTabPage.Controls.Add(ioRateLabel);

            // Enable IO Rates
            enableIORatesCheckBox = new CheckBox
            {
                Text = "Enable IORates for this interface",
                Location = new Point(30, 50),
                Size = new Size(200, 20)
            };
            ioRateTabPage.Controls.Add(enableIORatesCheckBox);

            // IO Rate buttons
            var buttonY = 80;
            var buttons = new[]
            {
                (createIORateButton = new Button { Text = "Create" }, "Create"),
                (deleteIORateButton = new Button { Text = "Delete" }, "Delete"),
                (resetIORateButton = new Button { Text = "Reset" }, "Reset"),
                (renameIORateButton = new Button { Text = "Rename" }, "Rename"),
                (addToFileButton = new Button { Text = "Add to File" }, "Add to File")
            };

            for (int i = 0; i < buttons.Length; i++)
            {
                buttons[i].Item1.Location = new Point(30 + i * 80, buttonY);
                buttons[i].Item1.Size = new Size(70, 25);
                ioRateTabPage.Controls.Add(buttons[i].Item1);
            }

            // Tag configuration fields
            var fields = new[]
            {
                ("Event Counter:", eventCounterTextBox = new TextBox(), 120),
                ("Tagname:", tagnameTextBox = new TextBox(), 150),
                ("Tag Status:", tagStatusTextBox = new TextBox(), 180),
                ("In File:", inFileTextBox = new TextBox(), 210),
                ("Snapshot:", snapshotTextBox = new TextBox(), 240)
            };

            foreach (var field in fields)
            {
                var label = new Label
                {
                    Text = field.Item1,
                    Location = new Point(30, field.Item3),
                    Size = new Size(100, 20)
                };
                ioRateTabPage.Controls.Add(label);

                field.Item2.Location = new Point(140, field.Item3 - 2);
                field.Item2.Size = new Size(300, 20);
                ioRateTabPage.Controls.Add(field.Item2);

                // Add suggest icon button for some fields
                if (field.Item1 == "Tagname:" || field.Item1 == "Event Counter:")
                {
                    var suggestButton = new Button
                    {
                        Text = "📝",
                        Location = new Point(450, field.Item3 - 2),
                        Size = new Size(25, 22)
                    };
                    ioRateTabPage.Controls.Add(suggestButton);
                }
            }

            // Large grid area for IO rate data
            var ioRateGrid = new DataGridView
            {
                Location = new Point(30, 280),
                Size = new Size(600, 100),
                AllowUserToAddRows = false,
                AllowUserToDeleteRows = false
            };
            ioRateTabPage.Controls.Add(ioRateGrid);
        }

        private void CreateFooter()
        {
            // Footer buttons
            closeButton = new Button
            {
                Text = "Close",
                Location = new Point(530, 500),
                Size = new Size(75, 25)
            };
            closeButton.Click += (s, e) => this.FindForm()?.Close();
            this.Controls.Add(closeButton);

            applyButton = new Button
            {
                Text = "Apply",
                Location = new Point(615, 500),
                Size = new Size(75, 25)
            };
            applyButton.Click += ApplyButton_Click;
            this.Controls.Add(applyButton);
        }

        private void InterfaceTypeComboBox_SelectedIndexChanged(object? sender, EventArgs e)
        {
            UpdateInterfaceSpecificTab();
        }

        private void ApplyButton_Click(object? sender, EventArgs e)
        {
            try
            {
                // Save current configuration
                SaveCurrentConfiguration();
                MessageBox.Show("Configuration applied successfully!", "Apply", 
                    MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error applying configuration: {ex.Message}", "Error",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void SaveCurrentConfiguration()
        {
            // Update configuration object with current form values
            currentConfiguration.InterfaceName = interfaceComboBox?.SelectedItem?.ToString() ?? "";
            currentConfiguration.InterfaceType = Enum.Parse<PIInterfaceType>(interfaceTypeComboBox?.SelectedItem?.ToString() ?? "PIPing");
            currentConfiguration.PointSource = pointSourceTextBox?.Text ?? "";
            
            if (int.TryParse(interfaceIDTextBox?.Text, out int interfaceId))
                currentConfiguration.InterfaceID = interfaceId;

            currentConfiguration.TimeoutDuration = (int)(timeoutDurationNumeric?.Value ?? 3);
            currentConfiguration.ThreadCount = (int)(threadCountNumeric?.Value ?? 10);
            currentConfiguration.AdditionalParameters = additionalParametersTextBox?.Text ?? "";

            // Service configuration
            currentConfiguration.ServiceName = serviceNameTextBox?.Text ?? "";
            currentConfiguration.DisplayName = displayNameTextBox?.Text ?? "";
            currentConfiguration.LogonType = localSystemRadio?.Checked == true ? 
                ServiceLogonType.LocalSystem : ServiceLogonType.DomainUserName;

            if (autoStartupRadio?.Checked == true)
                currentConfiguration.StartupType = ServiceStartupType.Auto;
            else if (disabledStartupRadio?.Checked == true)
                currentConfiguration.StartupType = ServiceStartupType.Disabled;
            else
                currentConfiguration.StartupType = ServiceStartupType.Manual;

            // UniInt configuration
            currentConfiguration.MaximumStopTime = (int)(maxStopTimeNumeric?.Value ?? 120);
            currentConfiguration.StartupDelay = (int)(startupDelayNumeric?.Value ?? 30);
            currentConfiguration.PointUpdateInterval = (int)(pointUpdateIntervalNumeric?.Value ?? 120);
            currentConfiguration.ServiceEventsPerSecond = (int)(serviceEventsNumeric?.Value ?? 500);
            currentConfiguration.PercentUp = (int)(percentUpNumeric?.Value ?? 100);

            currentConfiguration.QueueDataForActiveInterfaces = queueDataCheckBox?.Checked ?? false;
            currentConfiguration.BypassException = bypassExceptionCheckBox?.Checked ?? false;
            currentConfiguration.WriteStatusToTagsOnShutdown = writeStatusOnShutdownCheckBox?.Checked ?? true;
            currentConfiguration.ShutdownStatus = shutdownStatusCombo?.SelectedItem?.ToString() ?? "Int Shut";

            currentConfiguration.DisableAllOutputsFromPI = disableAllOutputsCheckBox?.Checked ?? false;
            currentConfiguration.SuppressInitialOutputsFromPI = suppressInitialOutputsCheckBox?.Checked ?? false;
            currentConfiguration.UseEventTimestamp = useEventTimestampCheckBox?.Checked ?? false;
            currentConfiguration.UseAlternateUTCMethod = useAlternateUTCCheckBox?.Checked ?? false;

            // Failover configuration
            currentConfiguration.EnableUniIntFailover = enableFailoverCheckBox?.Checked ?? false;
            currentConfiguration.FailoverPhase = phase1Radio?.Checked == true ? FailoverPhase.Phase1 : FailoverPhase.Phase2;
            currentConfiguration.FailoverIDThisInstance = failoverIdThisTextBox?.Text ?? "";
            currentConfiguration.FailoverIDOtherInstance = failoverIdOtherTextBox?.Text ?? "";
            currentConfiguration.NoFailoverWhenBothInterfacesLoseConnection = noFailoverCheckBox?.Checked ?? false;
            currentConfiguration.FailoverControlLogs = failoverControlLogsCheckBox?.Checked ?? false;
            currentConfiguration.HeartbeatUpdateRate = (int)(heartbeatRateNumeric?.Value ?? 1000);
            
            if (Enum.TryParse<UFOType>(ufoTypeCombo?.SelectedItem?.ToString(), out UFOType ufoType))
                currentConfiguration.UFOType = ufoType;
            
            currentConfiguration.SynchronizationFilePath = synchronizationFileTextBox?.Text ?? "";

            // Disconnected startup configuration  
            currentConfiguration.EnableDisconnectedStartup = enableDisconnectedCheckBox?.Checked ?? false;
            currentConfiguration.CacheSynchronizationPeriod = (int)(cacheSyncPeriodNumeric?.Value ?? 250);
            currentConfiguration.CachePath = cachePathTextBox?.Text ?? "";

            // IO Rate configuration
            currentConfiguration.EnableIORates = enableIORatesCheckBox?.Checked ?? false;
            currentConfiguration.EventCounter = eventCounterTextBox?.Text ?? "";
            currentConfiguration.Tagname = tagnameTextBox?.Text ?? "";
            currentConfiguration.TagStatus = tagStatusTextBox?.Text ?? "";
            currentConfiguration.InFile = inFileTextBox?.Text ?? "";
            currentConfiguration.Snapshot = snapshotTextBox?.Text ?? "";
        }

        public PIInterfaceConfiguration CurrentConfiguration => currentConfiguration;

        public void SetPIServerManager(RealPIServerManager manager)
        {
            piServerManager = manager;
            if (manager.IsConnected && manager.CurrentConnection != null)
            {
                UpdateConnectionStatus(manager.CurrentConnection);
            }
        }

        private void UpdateConnectionStatus(PIServerConnection connection)
        {
            if (connectionStatusLabel != null)
            {
                connectionStatusLabel.Text = $"PI Data server Connection Status\n✓ {connection.ServerName}\n   {(connection.IsConnected ? "Writeable" : "Disconnected")}";
                connectionStatusLabel.BackColor = connection.IsConnected ? Color.LightGreen : Color.LightCoral;
            }
        }
    }
} 