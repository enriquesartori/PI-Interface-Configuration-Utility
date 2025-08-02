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
        private DateTime _createdTime = DateTime.Now;
        private DateTime _lastUpdateTime = DateTime.Now;
        private string _lastUpdateTimeString = "";
        private int _scanIntervalMs = 5000; // Store as milliseconds
        private object? _currentValue;
        private PIPointStatus _status = PIPointStatus.Unknown;
        private string _interfaceName = "";
        private string _digitalStates = "";
        private int _id;
        private string _interfaceId = "";
        private bool _archive = true;
        private double _minValue = double.MinValue;
        private double _maxValue = double.MaxValue;
        private long _updateCount = 0;
        private double _conversionFactor = 1.0;
        private double _conversionOffset = 0.0;
        private string _conversionFormula = "";
        private bool _filterDuplicates = false;
        private double _compressionDeviation = 0.0;
        private double _compressionTimeDeadband = 0.0;
        private Dictionary<string, object> _attributes = new();

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
        /// When the point was created
        /// </summary>
        public DateTime CreatedTime
        {
            get => _createdTime;
            set
            {
                if (_createdTime != value)
                {
                    _createdTime = value;
                    OnPropertyChanged(nameof(CreatedTime));
                }
            }
        }

        /// <summary>
        /// Last update timestamp
        /// </summary>
        public DateTime LastUpdateTime
        {
            get => _lastUpdateTime;
            set
            {
                if (_lastUpdateTime != value)
                {
                    _lastUpdateTime = value;
                    _lastUpdateTimeString = value.ToString("yyyy-MM-dd HH:mm:ss");
                    OnPropertyChanged(nameof(LastUpdateTime));
                    OnPropertyChanged(nameof(LastUpdateTimeString));
                }
            }
        }

        /// <summary>
        /// Last update time as formatted string
        /// </summary>
        public string LastUpdateTimeString
        {
            get => _lastUpdateTimeString;
        }

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

        /// <summary>
        /// Interface ID this point belongs to
        /// </summary>
        public string InterfaceId
        {
            get => _interfaceId;
            set
            {
                if (_interfaceId != value)
                {
                    _interfaceId = value;
                    OnPropertyChanged(nameof(InterfaceId));
                }
            }
        }

        /// <summary>
        /// Whether the point is archived (alias for IsArchiving)
        /// </summary>
        public bool Archive
        {
            get => _archive;
            set
            {
                if (_archive != value)
                {
                    _archive = value;
                    OnPropertyChanged(nameof(Archive));
                }
            }
        }

        /// <summary>
        /// Minimum valid value for the point
        /// </summary>
        public double MinValue
        {
            get => _minValue;
            set
            {
                if (_minValue != value)
                {
                    _minValue = value;
                    OnPropertyChanged(nameof(MinValue));
                }
            }
        }

        /// <summary>
        /// Maximum valid value for the point
        /// </summary>
        public double MaxValue
        {
            get => _maxValue;
            set
            {
                if (_maxValue != value)
                {
                    _maxValue = value;
                    OnPropertyChanged(nameof(MaxValue));
                }
            }
        }

        /// <summary>
        /// Number of times this point has been updated
        /// </summary>
        public long UpdateCount
        {
            get => _updateCount;
            set
            {
                if (_updateCount != value)
                {
                    _updateCount = value;
                    OnPropertyChanged(nameof(UpdateCount));
                }
            }
        }

        /// <summary>
        /// Conversion factor for scaling values
        /// </summary>
        public double ConversionFactor
        {
            get => _conversionFactor;
            set
            {
                if (_conversionFactor != value)
                {
                    _conversionFactor = value;
                    OnPropertyChanged(nameof(ConversionFactor));
                }
            }
        }

        /// <summary>
        /// Conversion offset for adjusting values
        /// </summary>
        public double ConversionOffset
        {
            get => _conversionOffset;
            set
            {
                if (_conversionOffset != value)
                {
                    _conversionOffset = value;
                    OnPropertyChanged(nameof(ConversionOffset));
                }
            }
        }

        /// <summary>
        /// Custom conversion formula for complex calculations
        /// </summary>
        public string ConversionFormula
        {
            get => _conversionFormula;
            set
            {
                if (_conversionFormula != value)
                {
                    _conversionFormula = value;
                    OnPropertyChanged(nameof(ConversionFormula));
                }
            }
        }

        /// <summary>
        /// Whether to filter duplicate values
        /// </summary>
        public bool FilterDuplicates
        {
            get => _filterDuplicates;
            set
            {
                if (_filterDuplicates != value)
                {
                    _filterDuplicates = value;
                    OnPropertyChanged(nameof(FilterDuplicates));
                }
            }
        }

        /// <summary>
        /// Compression deviation threshold for data compression
        /// </summary>
        public double CompressionDeviation
        {
            get => _compressionDeviation;
            set
            {
                if (_compressionDeviation != value)
                {
                    _compressionDeviation = value;
                    OnPropertyChanged(nameof(CompressionDeviation));
                }
            }
        }

        /// <summary>
        /// Time deadband for compression (seconds)
        /// </summary>
        public double CompressionTimeDeadband
        {
            get => _compressionTimeDeadband;
            set
            {
                if (_compressionTimeDeadband != value)
                {
                    _compressionTimeDeadband = value;
                    OnPropertyChanged(nameof(CompressionTimeDeadband));
                }
            }
        }

        /// <summary>
        /// Custom attributes dictionary for additional properties
        /// </summary>
        public Dictionary<string, object> Attributes
        {
            get => _attributes;
            set
            {
                if (_attributes != value)
                {
                    _attributes = value ?? new Dictionary<string, object>();
                    OnPropertyChanged(nameof(Attributes));
                }
            }
        }

        /// <summary>
        /// Scan interval in milliseconds (for backward compatibility)
        /// </summary>
        public int ScanInterval
        {
            get => _scanIntervalMs;
            set
            {
                if (_scanIntervalMs != value)
                {
                    _scanIntervalMs = value;
                    OnPropertyChanged(nameof(ScanInterval));
                    OnPropertyChanged(nameof(ScanIntervalTimeSpan));
                }
            }
        }

        /// <summary>
        /// Scan interval as TimeSpan (for modern usage)
        /// </summary>
        public TimeSpan ScanIntervalTimeSpan
        {
            get => TimeSpan.FromMilliseconds(_scanIntervalMs);
            set
            {
                var ms = (int)value.TotalMilliseconds;
                if (_scanIntervalMs != ms)
                {
                    _scanIntervalMs = ms;
                    OnPropertyChanged(nameof(ScanInterval));
                    OnPropertyChanged(nameof(ScanIntervalTimeSpan));
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
            UpdateCount++;
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