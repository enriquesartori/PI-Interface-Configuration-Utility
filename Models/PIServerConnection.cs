using System;

namespace PIInterfaceConfigUtility.Models
{
    public class PIServerConnection
    {
        public string ServerName { get; set; } = string.Empty;
        public int Port { get; set; } = 5450;
        public string Username { get; set; } = string.Empty;
        public string Password { get; set; } = string.Empty;
        public bool UseWindowsAuthentication { get; set; } = true;
        public bool IsConnected { get; set; } = false;
        public DateTime? LastConnected { get; set; }
        public string ConnectionString => $"{ServerName}:{Port}";
        public int TimeoutSeconds { get; set; } = 30;
        
        public PIServerConnection() { }
        
        public PIServerConnection(string serverName, int port = 5450)
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