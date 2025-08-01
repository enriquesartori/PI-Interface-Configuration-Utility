using System;
using System.ComponentModel;

namespace PIInterfaceConfigUtility.Models
{
    /// <summary>
    /// Represents a PI Point (tag) in the PI System
    /// </summary>
    public class PIPoint : INotifyPropertyChanged
    {
        private string _name = "";
        private string _description = "";
        private PIPointDataType _dataType = PIPointDataType.Float32;
        private string _sourceAddress = "";
        private string _units = "";
        private bool _isEnabled = true;
        private bool _isArchiving = true;
        private int _scanInterval = 1000;
        private object? _currentValue;
        private DateTime _lastUpdateTime = DateTime.MinValue;
        private PIPointStatus _status = PIPointStatus.Unknown;
        private string _interfaceName = "";
        private string _digitalStates = "";
        private int _id;

        public event PropertyChangedEventHandler? PropertyChanged;

        /// <summary>
        /// Constructor for compatibility with legacy code
        /// </summary>
        public PIPoint(string name, string sourceAddress, PIPointType pointType)
        {
            _name = name;
            _sourceAddress = sourceAddress;
            _dataType = (PIPointDataType)pointType;
        }

        /// <summary>
        /// Constructor for compatibility with PIPointDataType
        /// </summary>
        public PIPoint(string name, string sourceAddress, PIPointDataType dataType)
        {
            _name = name;
            _sourceAddress = sourceAddress;
            _dataType = dataType;
        }

        /// <summary>
        /// Default parameterless constructor
        /// </summary>
        public PIPoint()
        {
        }

        /// <summary>
        /// Unique identifier for the PI Point
        /// </summary>
        public int Id
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
        /// PI Point name/tag name
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
        /// Description of the PI Point
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
        /// Data type of the PI Point
        /// </summary>
        public PIPointDataType DataType
        {
            get => _dataType;
            set
            {
                if (_dataType != value)
                {
                    _dataType = value;
                    OnPropertyChanged(nameof(DataType));
                }
            }
        }

        /// <summary>
        /// Legacy Type property for compatibility
        /// </summary>
        public PIPointType Type
        {
            get => (PIPointType)_dataType;
            set => DataType = (PIPointDataType)value;
        }

        /// <summary>
        /// Source address or location identifier
        /// </summary>
        public string SourceAddress
        {
            get => _sourceAddress;
            set
            {
                if (_sourceAddress != value)
                {
                    _sourceAddress = value;
                    OnPropertyChanged(nameof(SourceAddress));
                }
            }
        }

        /// <summary>
        /// Engineering units
        /// </summary>
        public string Units
        {
            get => _units;
            set
            {
                if (_units != value)
                {
                    _units = value;
                    OnPropertyChanged(nameof(Units));
                }
            }
        }

        /// <summary>
        /// Whether the point is enabled for data collection
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
        /// Legacy Enabled property for compatibility
        /// </summary>
        public bool Enabled
        {
            get => IsEnabled;
            set => IsEnabled = value;
        }

        /// <summary>
        /// Whether the point is set up for archiving
        /// </summary>
        public bool IsArchiving
        {
            get => _isArchiving;
            set
            {
                if (_isArchiving != value)
                {
                    _isArchiving = value;
                    OnPropertyChanged(nameof(IsArchiving));
                }
            }
        }

        /// <summary>
        /// Scan interval in milliseconds
        /// </summary>
        public int ScanInterval
        {
            get => _scanInterval;
            set
            {
                if (_scanInterval != value)
                {
                    _scanInterval = value;
                    OnPropertyChanged(nameof(ScanInterval));
                }
            }
        }

        /// <summary>
        /// Current value of the PI Point
        /// </summary>
        public object? CurrentValue
        {
            get => _currentValue;
            set
            {
                if (!Equals(_currentValue, value))
                {
                    _currentValue = value;
                    OnPropertyChanged(nameof(CurrentValue));
                    OnPropertyChanged(nameof(CurrentValueString));
                }
            }
        }

        /// <summary>
        /// Current value as a string for display
        /// </summary>
        public string CurrentValueString
        {
            get
            {
                if (CurrentValue == null) return "N/A";
                return DataType switch
                {
                    PIPointDataType.Boolean => CurrentValue.ToString() ?? "false",
                    PIPointDataType.Float32 or PIPointDataType.Float64 => 
                        string.Format("{0:F2} {1}", CurrentValue, Units).Trim(),
                    PIPointDataType.Int32 => $"{CurrentValue} {Units}".Trim(),
                    PIPointDataType.String => CurrentValue.ToString() ?? "",
                    _ => CurrentValue.ToString() ?? "N/A"
                };
            }
        }

        /// <summary>
        /// Timestamp of last update
        /// </summary>
        public DateTime LastUpdateTime
        {
            get => _lastUpdateTime;
            set
            {
                if (_lastUpdateTime != value)
                {
                    _lastUpdateTime = value;
                    OnPropertyChanged(nameof(LastUpdateTime));
                    OnPropertyChanged(nameof(LastUpdateTimeString));
                }
            }
        }

        /// <summary>
        /// Last update time as a formatted string
        /// </summary>
        public string LastUpdateTimeString => 
            LastUpdateTime == DateTime.MinValue ? "Never" : LastUpdateTime.ToString("yyyy-MM-dd HH:mm:ss");

        /// <summary>
        /// Legacy LastUpdate property for compatibility
        /// </summary>
        public DateTime LastUpdate
        {
            get => LastUpdateTime;
            set => LastUpdateTime = value;
        }

        /// <summary>
        /// Current status of the PI Point
        /// </summary>
        public PIPointStatus Status
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
        /// Name of the interface this point belongs to
        /// </summary>
        public string InterfaceName
        {
            get => _interfaceName;
            set
            {
                if (_interfaceName != value)
                {
                    _interfaceName = value;
                    OnPropertyChanged(nameof(InterfaceName));
                }
            }
        }

        /// <summary>
        /// Digital states for digital/boolean points (comma-separated)
        /// </summary>
        public string DigitalStates
        {
            get => _digitalStates;
            set
            {
                if (_digitalStates != value)
                {
                    _digitalStates = value;
                    OnPropertyChanged(nameof(DigitalStates));
                }
            }
        }

        protected virtual void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        /// <summary>
        /// Updates the current value and timestamp
        /// </summary>
        public void UpdateValue(object value, DateTime? timestamp = null)
        {
            CurrentValue = value;
            LastUpdateTime = timestamp ?? DateTime.Now;
        }

        /// <summary>
        /// Gets the formatted value for display
        /// </summary>
        public string GetFormattedValue()
        {
            return CurrentValueString;
        }

        public override string ToString()
        {
            return $"{Name} ({DataType}) = {CurrentValueString}";
        }
    }

    /// <summary>
    /// Supported PI Point data types
    /// </summary>
    public enum PIPointDataType
    {
        Boolean,
        Int16,
        Int32,
        Float32,
        Float64,
        String,
        Digital = Boolean // Alias for compatibility
    }

    /// <summary>
    /// Legacy alias for PIPointDataType (for compatibility)
    /// </summary>
    public enum PIPointType
    {
        Boolean,
        Int16,
        Int32,
        Float32,
        Float64,
        String,
        Digital = Boolean
    }

    /// <summary>
    /// PI Point status enumeration
    /// </summary>
    public enum PIPointStatus
    {
        Unknown,
        Good,
        Bad,
        Questionable,
        Substitute,
        NoData,
        ConfigError,
        NotConnected
    }
} 