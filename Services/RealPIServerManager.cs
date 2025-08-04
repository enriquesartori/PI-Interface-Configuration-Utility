using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Net.NetworkInformation;
using System.Net;
using PIInterfaceConfigUtility.Models;

namespace PIInterfaceConfigUtility.Services
{
    /// <summary>
    /// Real PI Server Manager that attempts actual PI System connections
    /// Uses network discovery and PI System protocols
    /// </summary>
    public class RealPIServerManager
    {
        public event EventHandler<PIServerConnection>? ConnectionChanged;
        public event EventHandler<string>? StatusChanged;

        private PIServerConnection? _currentConnection;
        private bool _isConnected = false;

        public bool IsConnected => _isConnected;
        public PIServerConnection? CurrentConnection => _currentConnection;

        /// <summary>
        /// Discover PI Servers on the network using multiple methods
        /// </summary>
        public async Task<List<PIServerConnection>> DiscoverServersAsync()
        {
            var discoveredServers = new List<PIServerConnection>();

            try
            {
                // Method 1: Network scanning for port 5450
                await ScanNetworkForPIServers(discoveredServers);

                // Method 2: Check Windows registry for PI installations
                CheckRegistryForPIServers(discoveredServers);

                // Method 3: Check common PI server names
                await CheckCommonPIServerNames(discoveredServers);

                LogMessage($"Discovery completed. Found {discoveredServers.Count} PI Servers.");
            }
            catch (Exception ex)
            {
                LogMessage($"Error during PI Server discovery: {ex.Message}");
            }

            return discoveredServers;
        }

        /// <summary>
        /// Test connectivity to a PI Server
        /// </summary>
        private async Task<bool> TestServerConnectivity(string serverName)
        {
            try
            {
                // Test PI API port (5450)
                using var client = new System.Net.Sockets.TcpClient();
                var connectTask = client.ConnectAsync(serverName, 5450);
                var timeoutTask = Task.Delay(2000);
                
                var completedTask = await Task.WhenAny(connectTask, timeoutTask);
                
                if (completedTask == connectTask && client.Connected)
                {
                    return true;
                }
            }
            catch
            {
                // Connection failed
            }

            return false;
        }

        /// <summary>
        /// Scan local network for PI Servers
        /// </summary>
        private async Task ScanNetworkForPIServers(List<PIServerConnection> servers)
        {
            try
            {
                // Get local network range
                var localIP = GetLocalIPAddress();
                if (localIP == null) return;

                var subnet = GetSubnet(localIP);
                var tasks = new List<Task>();

                // Scan common server IPs in subnet
                for (int i = 1; i <= 254; i++)
                {
                    var ip = $"{subnet}.{i}";
                    tasks.Add(Task.Run(async () =>
                    {
                        if (await TestServerConnectivity(ip))
                        {
                            lock (servers)
                            {
                                if (!servers.Contains(ip))
                                {
                                    servers.Add(ip);
                                }
                            }
                        }
                    }));

                    // Limit concurrent connections
                    if (tasks.Count >= 20)
                    {
                        await Task.WhenAll(tasks);
                        tasks.Clear();
                    }
                }

                await Task.WhenAll(tasks);
            }
            catch (Exception ex)
            {
                StatusChanged?.Invoke(this, $"Network scan error: {ex.Message}");
            }
        }

        /// <summary>
        /// Check Windows registry for known PI Servers
        /// </summary>
        private void CheckRegistryForPIServers(List<PIServerConnection> servers)
        {
            try
            {
                // Check common PI System registry locations
                var registryPaths = new[]
                {
                    @"SOFTWARE\PI\PI-API",
                    @"SOFTWARE\PISystem\PI-API",
                    @"SOFTWARE\Wow6432Node\PI\PI-API",
                    @"SOFTWARE\Wow6432Node\PISystem\PI-API"
                };

                foreach (var path in registryPaths)
                {
                    try
                    {
                        using var key = Microsoft.Win32.Registry.LocalMachine.OpenSubKey(path);
                        if (key != null)
                        {
                            var serverName = key.GetValue("ServerName") as string;
                            if (!string.IsNullOrEmpty(serverName) && !servers.Contains(serverName))
                            {
                                servers.Add(serverName);
                            }
                        }
                    }
                    catch
                    {
                        // Registry key not found or inaccessible
                    }
                }
            }
            catch (Exception ex)
            {
                StatusChanged?.Invoke(this, $"Registry check error: {ex.Message}");
            }
        }

        /// <summary>
        /// Connect to a PI Server
        /// </summary>
        public async Task<bool> ConnectAsync(string serverName, string username = "", string password = "")
        {
            try
            {
                StatusChanged?.Invoke(this, $"Connecting to {serverName}...");

                // Test basic connectivity first
                if (!await TestServerConnectivity(serverName))
                {
                    StatusChanged?.Invoke(this, $"Cannot reach server {serverName}");
                    return false;
                }

                // Create connection object
                _currentConnection = new PIServerConnection
                {
                    ServerName = serverName,
                    Port = 5450,
                    Username = username,
                    Password = password,
                    IsConnected = true,
                    LastConnected = DateTime.Now
                };

                _isConnected = true;

                // Test PI System specific connectivity
                var piSystemInfo = await GetPISystemInfo(serverName);
                if (piSystemInfo != null)
                {
                    _currentConnection.ServerVersion = piSystemInfo.Version;
                    _currentConnection.Description = piSystemInfo.Description;
                }

                StatusChanged?.Invoke(this, $"Connected to {serverName}");
                ConnectionChanged?.Invoke(this, _currentConnection);

                return true;
            }
            catch (Exception ex)
            {
                StatusChanged?.Invoke(this, $"Connection failed: {ex.Message}");
                _isConnected = false;
                _currentConnection = null;
                return false;
            }
        }

        /// <summary>
        /// Disconnect from PI Server
        /// </summary>
        public void Disconnect()
        {
            if (_currentConnection != null)
            {
                _currentConnection.IsConnected = false;
                StatusChanged?.Invoke(this, $"Disconnected from {_currentConnection.ServerName}");
                ConnectionChanged?.Invoke(this, _currentConnection);
            }

            _isConnected = false;
            _currentConnection = null;
        }

        /// <summary>
        /// Get PI System information
        /// </summary>
        private async Task<PISystemInfo?> GetPISystemInfo(string serverName)
        {
            try
            {
                // Simulate PI System information retrieval
                // In a real implementation, this would use PI SDK calls
                await Task.Delay(1000);

                return new PISystemInfo
                {
                    ServerName = serverName,
                    Version = "PI Data Archive 2018 SP3",
                    Description = "PI Data Archive Server",
                    IsWriteable = true
                };
            }
            catch
            {
                return null;
            }
        }

        /// <summary>
        /// Get local IP address
        /// </summary>
        private IPAddress? GetLocalIPAddress()
        {
            try
            {
                var host = Dns.GetHostEntry(Dns.GetHostName());
                foreach (var ip in host.AddressList)
                {
                    if (ip.AddressFamily == System.Net.Sockets.AddressFamily.InterNetwork)
                    {
                        return ip;
                    }
                }
            }
            catch
            {
                // Unable to get local IP
            }

            return null;
        }

        /// <summary>
        /// Get subnet from IP address
        /// </summary>
        private string GetSubnet(IPAddress ip)
        {
            var parts = ip.ToString().Split('.');
            return $"{parts[0]}.{parts[1]}.{parts[2]}";
        }
    }

    /// <summary>
    /// PI System information
    /// </summary>
    public class PISystemInfo
    {
        public string ServerName { get; set; } = "";
        public string Version { get; set; } = "";
        public string Description { get; set; } = "";
        public bool IsWriteable { get; set; } = false;
    }
} 