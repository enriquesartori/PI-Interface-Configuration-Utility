using System;
using System.Collections.Generic;

namespace PIInterfaceConfigUtility.Models
{
    public enum InterfaceType
    {
        OPC_DA,
        OPC_UA,
        Modbus,
        DNP3,
        BACnet,
        SNMP,
        Database,
        FileSystem,
        Custom
    }
    
    public enum InterfaceStatus
    {
        Stopped,
        Starting,
        Running,
        Stopping,
        Error,
        Unknown
    }
    
    public class PIInterface
    {
        public string Id { get; set; } = Guid.NewGuid().ToString();
        public string Name { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public InterfaceType Type { get; set; } = InterfaceType.OPC_DA;
        public InterfaceStatus Status { get; set; } = InterfaceStatus.Stopped;
        public string ServiceName { get; set; } = string.Empty;
        public string ExecutablePath { get; set; } = string.Empty;
        public string ConfigurationPath { get; set; } = string.Empty;
        public string ConfigFilePath { get; set; } = string.Empty;
        public string LogFilePath { get; set; } = string.Empty;
        public bool IsEnabled { get; set; } = true;
        public bool AutoStart { get; set; } = false;
        public int ScanInterval { get; set; } = 1000; // milliseconds
        public DateTime? LastStarted { get; set; }
        public DateTime? LastStopped { get; set; }
        public long MessagesReceived { get; set; } = 0;
        public long MessagesSent { get; set; } = 0;
        public long ErrorCount { get; set; } = 0;
        public Dictionary<string, string> Properties { get; set; } = new();
        public List<PIPoint> Points { get; set; } = new();
        
        // Connection settings
        public string SourceAddress { get; set; } = string.Empty;
        public int SourcePort { get; set; } = 0;
        public string SourceUsername { get; set; } = string.Empty;
        public string SourcePassword { get; set; } = string.Empty;
        
        // PI Server settings
        public string PIServerName { get; set; } = string.Empty;
        public string PIPointPrefix { get; set; } = string.Empty;
        
        public PIInterface() { }
        
        public PIInterface(string name, InterfaceType type)
        {
            Name = name;
            Type = type;
            ServiceName = $"PI_{Name.Replace(" ", "_")}";
        }
        
        public void AddProperty(string key, string value)
        {
            Properties[key] = value;
        }
        
        public string GetProperty(string key, string defaultValue = "")
        {
            return Properties.TryGetValue(key, out var value) ? value : defaultValue;
        }
        
        public override string ToString()
        {
            return $"{Name} ({Type}) - {Status}";
        }
    }
} 