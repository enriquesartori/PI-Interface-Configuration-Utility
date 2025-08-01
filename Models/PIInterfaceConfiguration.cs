using System;
using System.Collections.Generic;
using System.ComponentModel;

namespace PIInterfaceConfigUtility.Models
{
    /// <summary>
    /// Comprehensive PI Interface configuration matching real PI ICU functionality
    /// </summary>
    public class PIInterfaceConfiguration : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler? PropertyChanged;

        // General Tab Properties
        public string InterfaceName { get; set; } = "";
        public PIInterfaceType InterfaceType { get; set; } = PIInterfaceType.PIPing;
        public string PointSource { get; set; } = "";
        public int InterfaceID { get; set; } = 1;
        public List<ScanClass> ScanClasses { get; set; } = new();
        public string PIHostInformation { get; set; } = "";
        public string ServerCollective { get; set; } = "";
        public string SDKMember { get; set; } = "";
        public string APIHostname { get; set; } = "";
        public string User { get; set; } = "";
        public string Type { get; set; } = "";
        public string Version { get; set; } = "";
        public int Port { get; set; } = 5450;

        // Service Tab Properties
        public string ServiceName { get; set; } = "";
        public string DisplayName { get; set; } = "";
        public ServiceLogonType LogonType { get; set; } = ServiceLogonType.LocalSystem;
        public string LogonUser { get; set; } = "";
        public string LogonPassword { get; set; } = "";
        public List<string> Dependencies { get; set; } = new();
        public ServiceStartupType StartupType { get; set; } = ServiceStartupType.Manual;

        // Interface-Specific Parameters (PIPing, OPC DA, etc.)
        public int TimeoutDuration { get; set; } = 3;
        public int ThreadCount { get; set; } = 10;
        public string AdditionalParameters { get; set; } = "";

        // UniInt Tab Properties (Performance and Behavior)
        public int MaximumStopTime { get; set; } = 120;
        public int StartupDelay { get; set; } = 30;
        public int PointUpdateInterval { get; set; } = 120;
        public int ServiceEventsPerSecond { get; set; } = 500;
        public int PercentUp { get; set; } = 100;
        public bool APIConnectionName { get; set; } = false;
        public bool DisableUniIntPerformanceCounters { get; set; } = false;
        public bool IncludePointSourceInHeader { get; set; } = false;
        public bool IncludeUFOIDInLogMessages { get; set; } = false;

        // Data Handling
        public bool QueueDataForActiveInterfaces { get; set; } = false;
        public bool BypassException { get; set; } = false;
        public bool WriteStatusToTagsOnShutdown { get; set; } = true;
        public string ShutdownStatus { get; set; } = "Int Shut";

        // Outputs
        public bool DisableAllOutputsFromPI { get; set; } = false;
        public bool SuppressInitialOutputsFromPI { get; set; } = false;
        public bool UseEventTimestamp { get; set; } = false;

        // Timestamps
        public bool UseAlternateUTCMethod { get; set; } = false;

        // UniInt Failover Properties
        public bool EnableUniIntFailover { get; set; } = false;
        public FailoverPhase FailoverPhase { get; set; } = FailoverPhase.Phase1;
        public string FailoverIDThisInstance { get; set; } = "";
        public string FailoverIDOtherInstance { get; set; } = "";
        public bool NoFailoverWhenBothInterfacesLoseConnection { get; set; } = false;
        public bool FailoverControlLogs { get; set; } = false;
        public int HeartbeatUpdateRate { get; set; } = 1000;
        public UFOType UFOType { get; set; } = UFOType.COLD;
        public string SynchronizationFilePath { get; set; } = "";

        // UniInt Disconnected Startup Properties
        public bool EnableDisconnectedStartup { get; set; } = false;
        public int CacheSynchronizationPeriod { get; set; } = 250;
        public string CachePath { get; set; } = "";
        public List<CacheFile> CacheFiles { get; set; } = new();

        // IO Rate Properties
        public bool EnableIORates { get; set; } = false;
        public string EventCounter { get; set; } = "";
        public string Tagname { get; set; } = "";
        public string TagStatus { get; set; } = "";
        public string InFile { get; set; } = "";
        public string Snapshot { get; set; } = "";

        protected virtual void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }

    /// <summary>
    /// Scan class configuration
    /// </summary>
    public class ScanClass
    {
        public int ClassNumber { get; set; }
        public TimeSpan ScanFrequency { get; set; }
        public bool Enabled { get; set; } = true;
    }

    /// <summary>
    /// Cache file information for disconnected startup
    /// </summary>
    public class CacheFile
    {
        public string Type { get; set; } = "";
        public string Name { get; set; } = "";
        public DateTime Modified { get; set; }
        public DateTime Created { get; set; }
        public long SizeKB { get; set; }
        public string FullPath { get; set; } = "";
    }

    /// <summary>
    /// PI Interface types matching real PI ICU
    /// </summary>
    public enum PIInterfaceType
    {
        PIPing,
        OPCDA,
        Perfmon,
        UFL,
        RDBMS,
        OPCAE,
        UniInt,
        Custom
    }

    /// <summary>
    /// Service logon types
    /// </summary>
    public enum ServiceLogonType
    {
        LocalSystem,
        DomainUserName
    }

    /// <summary>
    /// Service startup types
    /// </summary>
    public enum ServiceStartupType
    {
        Auto,
        Manual,
        Disabled
    }

    /// <summary>
    /// Failover phases
    /// </summary>
    public enum FailoverPhase
    {
        Phase1,
        Phase2
    }

    /// <summary>
    /// UFO (UniInt Failover Object) types
    /// </summary>
    public enum UFOType
    {
        COLD,
        WARM,
        HOT
    }
} 