# PI Interface Configuration Utility

A comprehensive desktop application for configuring and managing PI interfaces and data collection systems. This application replicates the functionality of the OSIsoft PI Interface Configuration Utility (ICU) with a modern Windows Forms interface.

## Features

### Core Functionality
- **PI Server Connection Management** - Connect to PI Servers with authentication support
- **Interface Configuration** - Configure and manage various types of PI interfaces (OPC DA/UA, Modbus, DNP3, BACnet, etc.)
- **PI Points Management** - Create, edit, and manage PI data points/tags
- **Service Management** - Start, stop, and monitor interface services
- **Real-time Diagnostics** - System health monitoring and connection diagnostics
- **Configuration Management** - Save, load, import, and export configurations
- **Logging and Troubleshooting** - Comprehensive logging with filtering capabilities

### Interface Types Supported
- OPC DA
- OPC UA
- Modbus TCP/RTU
- DNP3
- BACnet
- SNMP
- Database interfaces
- File system interfaces
- Custom interfaces

### Key Benefits
- **No Installation Required** - Standalone executable with no dependencies
- **Modern UI** - Clean, intuitive Windows Forms interface
- **Comprehensive Configuration** - All PI interface setup options in one place
- **Real-time Monitoring** - Live status updates and diagnostics
- **Import/Export** - Support for JSON, XML, and CSV configuration formats

## System Requirements

- Windows 10 or later (x64 or x86)
- No additional software requirements (self-contained executable)

## Building the Application

### Prerequisites
- .NET 6.0 SDK or later
- Windows development environment

### Build Instructions

1. **Clone or download the source code** to your local machine

2. **Open Command Prompt or PowerShell** in the project directory

3. **Run the build script:**
   ```cmd
   build.cmd
   ```

   Or manually build using .NET CLI:
   ```cmd
   dotnet restore
   dotnet build --configuration Release
   dotnet publish --configuration Release --runtime win-x64 --self-contained true --output "publish\win-x64" -p:PublishSingleFile=true
   ```

4. **Locate the executable:**
   - Windows x64: `publish\win-x64\PIInterfaceConfigUtility.exe`
   - Windows x86: `publish\win-x86\PIInterfaceConfigUtility.exe`

The build process creates standalone executables that include all required dependencies and do not require .NET installation on target machines.

## Usage Guide

### Getting Started

1. **Launch the Application**
   - Double-click `PIInterfaceConfigUtility.exe`
   - No installation required

2. **Connect to PI Server**
   - Go to the "PI Server Connection" tab
   - Enter your PI Server details (name, port, credentials)
   - Click "Connect" or use "Test" to verify connectivity

### Managing Interfaces

1. **Add New Interface**
   - Navigate to "Interface Configuration" tab
   - Click "Add Interface" in the toolbar
   - Select interface type and provide configuration details
   - Configure interface-specific properties

2. **Configure Interface Properties**
   - Select an interface from the list
   - Use the Properties panel to modify settings
   - Configure connection parameters, scan rates, and other options

3. **Start/Stop Interfaces**
   - Use individual interface controls or bulk actions
   - Monitor status in real-time
   - View statistics and error counts

### Managing PI Points

1. **Search and Browse PI Points**
   - Use the "PI Points" tab
   - Search for existing points using wildcards
   - View current values and status

2. **Create New PI Points**
   - Click "Add Point" toolbar button
   - Configure point properties (name, type, address, units)
   - Set scaling, filtering, and archiving options

3. **Read/Write Values**
   - Select points and use "Read Value" for current data
   - Use "Write Value" to send test data

### Service Management

1. **Monitor Interface Services**
   - "Service Management" tab shows all interface services
   - Real-time status updates every 5 seconds
   - View message counts, error rates, and uptime

2. **Bulk Operations**
   - Start/Stop all services at once
   - View detailed statistics for each service
   - Monitor system performance

### Diagnostics and Troubleshooting

1. **System Diagnostics**
   - "Diagnostics" tab provides comprehensive system health check
   - Monitor PI Server connectivity
   - Check interface status and performance
   - View memory usage and system information

2. **Log Viewing**
   - "Logs" tab shows real-time application logs
   - Filter by log level (Error, Warning, Info, Debug)
   - Save logs to file for analysis
   - Auto-scroll and log rotation features

### Configuration Management

1. **Save/Load Configurations**
   - File menu: New, Open, Save, Save As
   - Native `.piconfig` format for complete configurations

2. **Import/Export**
   - Import from JSON, XML, or CSV files
   - Export configurations in multiple formats
   - Migrate settings between environments

## File Formats

### Native Configuration (.piconfig)
Complete application state including:
- PI Server connections
- Interface configurations
- PI Point definitions
- Application settings

### Import/Export Formats

**JSON Format:**
```json
{
  "ServerConnection": {
    "ServerName": "PIServer01",
    "Port": 5450,
    "UseWindowsAuthentication": true
  },
  "Interfaces": [
    {
      "Name": "OPC_Interface_01",
      "Type": "OPC_DA",
      "Description": "Main OPC interface",
      "Properties": {
        "ProgID": "Matrikon.OPC.Server.1",
        "UpdateRate": "1000"
      }
    }
  ],
  "Points": [
    {
      "Name": "Tank01.Level",
      "Type": "Float32",
      "SourceAddress": "40001",
      "Units": "%",
      "Description": "Tank 1 Level"
    }
  ]
}
```

**XML Format:**
```xml
<PIConfiguration Version="1.0">
  <Interfaces>
    <Interface Name="OPC_Interface_01" Type="OPC_DA" Description="Main OPC interface"/>
  </Interfaces>
  <Points>
    <Point Name="Tank01.Level" Type="Float32" SourceAddress="40001" Units="%"/>
  </Points>
</PIConfiguration>
```

**CSV Format (PI Points):**
```csv
Name,Description,SourceAddress,Type,Units,InterfaceId,Enabled
Tank01.Level,Tank 1 Level,40001,Float32,%,OPC_Interface_01,True
Tank01.Temperature,Tank 1 Temperature,40002,Float32,°C,OPC_Interface_01,True
```

## Troubleshooting

### Common Issues

**Connection Problems:**
- Verify PI Server is running and accessible
- Check firewall settings (default port 5450)
- Ensure proper authentication credentials
- Use "Test Connection" button to diagnose

**Interface Issues:**
- Verify interface executable paths
- Check Windows service permissions
- Review interface-specific configuration
- Monitor error logs for details

**Performance Issues:**
- Adjust scan intervals for interfaces
- Monitor system memory usage in Diagnostics
- Reduce number of concurrent interfaces if needed
- Check network connectivity quality

### Error Codes and Messages

The application provides detailed error messages in the Logs tab. Common categories:

- **Connection Errors**: PI Server connectivity issues
- **Configuration Errors**: Invalid interface or point settings
- **Service Errors**: Windows service management problems
- **Permission Errors**: Insufficient user privileges

## Technical Architecture

### Key Components

**Models:**
- `PIServerConnection` - PI Server connection information
- `PIInterface` - Interface configuration and status
- `PIPoint` - PI data point definitions

**Services:**
- `PIServerManager` - PI Server connectivity and operations
- `InterfaceManager` - Interface lifecycle management
- `ConfigurationManager` - Configuration persistence

**UI Controls:**
- `PIServerConnectionControl` - Server connection management
- `InterfaceConfigurationControl` - Interface setup and monitoring
- `PIPointsControl` - PI point management
- `ServiceManagementControl` - Service operations
- `DiagnosticsControl` - System health monitoring
- `LogsViewerControl` - Application logging

### Technology Stack

- **.NET 6.0** - Modern cross-platform framework
- **Windows Forms** - Native Windows desktop UI
- **Newtonsoft.Json** - JSON serialization
- **System.ServiceProcess** - Windows service management
- **System.Management** - System information access

## License

This project is provided as-is for educational and development purposes. Please ensure compliance with your organization's policies regarding PI System integration tools.

## Support

For issues or questions:
1. Check the Diagnostics tab for system health information
2. Review the Logs tab for detailed error messages
3. Verify PI Server connectivity and permissions
4. Consult PI System documentation for interface-specific guidance

## Contributing

This is a demonstration project showing how to create a PI Interface Configuration Utility clone. You can extend it by:

- Adding new interface types
- Implementing additional PI System APIs
- Enhancing the user interface
- Adding more diagnostic capabilities
- Integrating with other PI System tools 