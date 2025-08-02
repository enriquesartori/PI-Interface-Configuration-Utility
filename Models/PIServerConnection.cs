using System;
using System.ComponentModel;

namespace PIInterfaceConfigUtility.Models
{
    /// <summary>
    /// Represents a connection to a PI Server
    /// </summary>
    public class PIServerConnection : INotifyPropertyChanged
    {
        private string _serverName = "";
        private int _port = 5450;
        private string _username = "";
        private string _password = "";
        private bool _isConnected = false;
        private DateTime _lastConnected = DateTime.MinValue;
        private string _description = "";
        private string _serverVersion = "";
        private bool _useWindowsAuthentication = true;

        public event PropertyChangedEventHandler? PropertyChanged;

        /// <summary>
        /// PI Server name or IP address
        /// </summary>
        public string ServerName
        {
            get => _serverName;
            set
            {
                if (_serverName != value)
                {
                    _serverName = value;
                    OnPropertyChanged(nameof(ServerName));
                }
            }
        }

        /// <summary>
        /// PI Server port (typically 5450)
        /// </summary>
        public int Port
        {
            get => _port;
            set
            {
                if (_port != value)
                {
                    _port = value;
                    OnPropertyChanged(nameof(Port));
                }
            }
        }

        /// <summary>
        /// Username for PI Server authentication
        /// </summary>
        public string Username
        {
            get => _username;
            set
            {
                if (_username != value)
                {
                    _username = value;
                    OnPropertyChanged(nameof(Username));
                }
            }
        }

        /// <summary>
        /// Password for PI Server authentication
        /// </summary>
        public string Password
        {
            get => _password;
            set
            {
                if (_password != value)
                {
                    _password = value;
                    OnPropertyChanged(nameof(Password));
                }
            }
        }

        /// <summary>
        /// Whether currently connected to the PI Server
        /// </summary>
        public bool IsConnected
        {
            get => _isConnected;
            set
            {
                if (_isConnected != value)
                {
                    _isConnected = value;
                    OnPropertyChanged(nameof(IsConnected));
                }
            }
        }

        /// <summary>
        /// Last connection timestamp
        /// </summary>
        public DateTime LastConnected
        {
            get => _lastConnected;
            set
            {
                if (_lastConnected != value)
                {
                    _lastConnected = value;
                    OnPropertyChanged(nameof(LastConnected));
                }
            }
        }

        /// <summary>
        /// PI Server description
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
        /// PI Server version information
        /// </summary>
        public string ServerVersion
        {
            get => _serverVersion;
            set
            {
                if (_serverVersion != value)
                {
                    _serverVersion = value;
                    OnPropertyChanged(nameof(ServerVersion));
                }
            }
        }

        /// <summary>
        /// Whether to use Windows Authentication
        /// </summary>
        public bool UseWindowsAuthentication
        {
            get => _useWindowsAuthentication;
            set
            {
                if (_useWindowsAuthentication != value)
                {
                    _useWindowsAuthentication = value;
                    OnPropertyChanged(nameof(UseWindowsAuthentication));
                }
            }
        }

        protected virtual void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        // Constructors for compatibility
        public PIServerConnection()
        {
        }

        public PIServerConnection(string serverName)
        {
            ServerName = serverName;
        }

        public PIServerConnection(string serverName, int port)
        {
            ServerName = serverName;
            Port = port;
        }

        public override string ToString()
        {
            return $"{ServerName}:{Port} ({(IsConnected ? "Connected" : "Disconnected")})";
        }
    }
} 