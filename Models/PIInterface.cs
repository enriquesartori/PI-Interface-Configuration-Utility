using System;
using System.Collections.Generic;
using System.ComponentModel;
using PIInterfaceConfigUtility.Models;

namespace PIInterfaceConfigUtility.Models
{
    /// <summary>
    /// Represents a PI Interface with its configuration and status
    /// </summary>
    public class PIInterface : INotifyPropertyChanged
    {
        private string _name = "";
        private InterfaceType _type = InterfaceType.PIPing;
        private InterfaceStatus _status = InterfaceStatus.Stopped;
        private string _serviceName = "";
        private string _description = "";
        private string _version = "";
        private string _configFilePath = "";
        private string _logFilePath = "";
        private bool _isEnabled = true;
        private int _pointsCount = 0;
        private double _updateRate = 0.0;
        private DateTime _lastUpdate = DateTime.Now;
        private DateTime _lastStarted = DateTime.MinValue;
        private DateTime _lastStopped = DateTime.MinValue;
        private TimeSpan _uptime = TimeSpan.Zero;
        private double _cpuUsage = 0.0;
        private double _memoryUsage = 0.0;
        private int _goodPoints = 0;
        private int _badPoints = 0;
        private long _totalEvents = 0;
        private double _eventsPerSecond = 0.0;

        // Collections
        private List<PIPoint> _points = new();
        private Dictionary<string, object> _properties = new();

        // Additional tracking properties
        private string _id = Guid.NewGuid().ToString();
        private long _messagesReceived = 0;
        private long _messagesSent = 0;
        private int _errorCount = 0;

        public event PropertyChangedEventHandler? PropertyChanged;

        // Constructors
        public PIInterface()
        {
        }

        public PIInterface(string name, InterfaceType type)
        {
            _name = name;
            _type = type;
            _serviceName = $"PI-{name}";
        }

        /// <summary>
        /// Unique interface identifier
        /// </summary>
        public string Id
        {
            get => _id;
            set
            {
                if (_id != value)
                {
                    _id = value;
                    OnPropertyChanged(nameof(Id));
                }
            }
        }

        /// <summary>
        /// Interface name
        /// </summary>
        public string Name
        {
            get => _name;
            set
            {
                if (_name != value)
                {
                    _name = value;
                    OnPropertyChanged(nameof(Name));
                }
            }
        }

        /// <summary>
        /// Interface type
        /// </summary>
        public InterfaceType Type
        {
            get => _type;
            set
            {
                if (_type != value)
                {
                    _type = value;
                    OnPropertyChanged(nameof(Type));
                }
            }
        }

        /// <summary>
        /// Current interface status
        /// </summary>
        public InterfaceStatus Status
        {
            get => _status;
            set
            {
                if (_status != value)
                {
                    _status = value;
                    OnPropertyChanged(nameof(Status));
                }
            }
        }

        /// <summary>
        /// Windows service name
        /// </summary>
        public string ServiceName
        {
            get => _serviceName;
            set
            {
                if (_serviceName != value)
                {
                    _serviceName = value;
                    OnPropertyChanged(nameof(ServiceName));
                }
            }
        }

        /// <summary>
        /// Interface description
        /// </summary>
        public string Description
        {
            get => _description;
            set
            {
                if (_description != value)
                {
                    _description = value;
                    OnPropertyChanged(nameof(Description));
                }
            }
        }

        /// <summary>
        /// Interface version
        /// </summary>
        public string Version
        {
            get => _version;
            set
            {
                if (_version != value)
                {
                    _version = value;
                    OnPropertyChanged(nameof(Version));
                }
            }
        }

        /// <summary>
        /// Configuration file path
        /// </summary>
        public string ConfigFilePath
        {
            get => _configFilePath;
            set
            {
                if (_configFilePath != value)
                {
                    _configFilePath = value;
                    OnPropertyChanged(nameof(ConfigFilePath));
                }
            }
        }

        /// <summary>
        /// Log file path
        /// </summary>
        public string LogFilePath
        {
            get => _logFilePath;
            set
            {
                if (_logFilePath != value)
                {
                    _logFilePath = value;
                    OnPropertyChanged(nameof(LogFilePath));
                }
            }
        }

        /// <summary>
        /// Whether the interface is enabled
        /// </summary>
        public bool IsEnabled
        {
            get => _isEnabled;
            set
            {
                if (_isEnabled != value)
                {
                    _isEnabled = value;
                    OnPropertyChanged(nameof(IsEnabled));
                }
            }
        }

        /// <summary>
        /// Number of points managed by this interface
        /// </summary>
        public int PointsCount
        {
            get => _pointsCount;
            set
            {
                if (_pointsCount != value)
                {
                    _pointsCount = value;
                    OnPropertyChanged(nameof(PointsCount));
                }
            }
        }

        /// <summary>
        /// Update rate in updates per second
        /// </summary>
        public double UpdateRate
        {
            get => _updateRate;
            set
            {
                if (Math.Abs(_updateRate - value) > 0.001)
                {
                    _updateRate = value;
                    OnPropertyChanged(nameof(UpdateRate));
                }
            }
        }

        /// <summary>
        /// Last update timestamp
        /// </summary>
        public DateTime LastUpdate
        {
            get => _lastUpdate;
            set
            {
                if (_lastUpdate != value)
                {
                    _lastUpdate = value;
                    OnPropertyChanged(nameof(LastUpdate));
                }
            }
        }

        /// <summary>
        /// When the interface was last started
        /// </summary>
        public DateTime LastStarted
        {
            get => _lastStarted;
            set
            {
                if (_lastStarted != value)
                {
                    _lastStarted = value;
                    OnPropertyChanged(nameof(LastStarted));
                }
            }
        }

        /// <summary>
        /// When the interface was last stopped
        /// </summary>
        public DateTime LastStopped
        {
            get => _lastStopped;
            set
            {
                if (_lastStopped != value)
                {
                    _lastStopped = value;
                    OnPropertyChanged(nameof(LastStopped));
                }
            }
        }

        /// <summary>
        /// Interface uptime
        /// </summary>
        public TimeSpan Uptime
        {
            get => _uptime;
            set
            {
                if (_uptime != value)
                {
                    _uptime = value;
                    OnPropertyChanged(nameof(Uptime));
                }
            }
        }

        /// <summary>
        /// CPU usage percentage
        /// </summary>
        public double CpuUsage
        {
            get => _cpuUsage;
            set
            {
                if (Math.Abs(_cpuUsage - value) > 0.001)
                {
                    _cpuUsage = value;
                    OnPropertyChanged(nameof(CpuUsage));
                }
            }
        }

        /// <summary>
        /// Memory usage in MB
        /// </summary>
        public double MemoryUsage
        {
            get => _memoryUsage;
            set
            {
                if (Math.Abs(_memoryUsage - value) > 0.001)
                {
                    _memoryUsage = value;
                    OnPropertyChanged(nameof(MemoryUsage));
                }
            }
        }

        /// <summary>
        /// Number of points with good status
        /// </summary>
        public int GoodPoints
        {
            get => _goodPoints;
            set
            {
                if (_goodPoints != value)
                {
                    _goodPoints = value;
                    OnPropertyChanged(nameof(GoodPoints));
                }
            }
        }

        /// <summary>
        /// Number of points with bad status
        /// </summary>
        public int BadPoints
        {
            get => _badPoints;
            set
            {
                if (_badPoints != value)
                {
                    _badPoints = value;
                    OnPropertyChanged(nameof(BadPoints));
                }
            }
        }

        /// <summary>
        /// Total number of events processed
        /// </summary>
        public long TotalEvents
        {
            get => _totalEvents;
            set
            {
                if (_totalEvents != value)
                {
                    _totalEvents = value;
                    OnPropertyChanged(nameof(TotalEvents));
                }
            }
        }

        /// <summary>
        /// Events per second rate
        /// </summary>
        public double EventsPerSecond
        {
            get => _eventsPerSecond;
            set
            {
                if (Math.Abs(_eventsPerSecond - value) > 0.001)
                {
                    _eventsPerSecond = value;
                    OnPropertyChanged(nameof(EventsPerSecond));
                }
            }
        }

        /// <summary>
        /// Number of messages received by the interface
        /// </summary>
        public long MessagesReceived
        {
            get => _messagesReceived;
            set
            {
                if (_messagesReceived != value)
                {
                    _messagesReceived = value;
                    OnPropertyChanged(nameof(MessagesReceived));
                }
            }
        }

        /// <summary>
        /// Number of messages sent by the interface
        /// </summary>
        public long MessagesSent
        {
            get => _messagesSent;
            set
            {
                if (_messagesSent != value)
                {
                    _messagesSent = value;
                    OnPropertyChanged(nameof(MessagesSent));
                }
            }
        }

        /// <summary>
        /// Number of errors encountered
        /// </summary>
        public int ErrorCount
        {
            get => _errorCount;
            set
            {
                if (_errorCount != value)
                {
                    _errorCount = value;
                    OnPropertyChanged(nameof(ErrorCount));
                }
            }
        }

        /// <summary>
        /// Collection of PI Points managed by this interface
        /// </summary>
        public List<PIPoint> Points
        {
            get => _points;
            set
            {
                if (_points != value)
                {
                    _points = value ?? new List<PIPoint>();
                    _pointsCount = _points.Count;
                    OnPropertyChanged(nameof(Points));
                    OnPropertyChanged(nameof(PointsCount));
                }
            }
        }

        /// <summary>
        /// Add a custom property to the interface
        /// </summary>
        public void AddProperty(string name, object value)
        {
            _properties[name] = value;
            OnPropertyChanged($"Property_{name}");
        }

        /// <summary>
        /// Get a custom property value
        /// </summary>
        public T? GetProperty<T>(string name)
        {
            if (_properties.TryGetValue(name, out var value) && value is T typedValue)
            {
                return typedValue;
            }
            return default(T);
        }

        /// <summary>
        /// Get all custom properties
        /// </summary>
        public Dictionary<string, object> GetAllProperties()
        {
            return new Dictionary<string, object>(_properties);
        }

        protected virtual void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        public override string ToString()
        {
            return $"{Name} ({Type}) - {Status}";
        }
    }

    /// <summary>
    /// PI Interface types
    /// </summary>
    public enum InterfaceType
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
    /// PI Interface status
    /// </summary>
    public enum InterfaceStatus
    {
        Stopped,
        Starting,
        Running,
        Stopping,
        Error,
        Unknown
    }
} 