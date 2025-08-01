using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using PIInterfaceConfigUtility.Models;
using System.Linq;

namespace PIInterfaceConfigUtility.Services
{
    public class PIServerManager
    {
        private PIServerConnection? currentConnection;
        private readonly List<PIPoint> piPoints = new();
        
        public event EventHandler<string>? StatusChanged;
        public event EventHandler<PIServerConnection>? ConnectionChanged;
        
        public bool IsConnected => currentConnection?.IsConnected == true;
        public PIServerConnection? CurrentConnection => currentConnection;
        public IReadOnlyList<PIPoint> PIPoints => piPoints.AsReadOnly();
        
        public async Task<bool> ConnectAsync(string serverName, string username = "", string password = "")
        {
            try
            {
                OnStatusChanged("Connecting to PI Server...");
                
                // Simulate connection process
                await Task.Delay(2000);
                
                currentConnection = new PIServerConnection(serverName)
                {
                    Username = username,
                    Password = password,
                    IsConnected = true,
                    LastConnected = DateTime.Now
                };
                
                OnStatusChanged($"Connected to PI Server: {serverName}");
                OnConnectionChanged(currentConnection);
                
                // Load PI Points after successful connection
                await LoadPIPointsAsync();
                
                return true;
            }
            catch (Exception ex)
            {
                OnStatusChanged($"Connection failed: {ex.Message}");
                return false;
            }
        }
        
        public void Disconnect()
        {
            if (currentConnection != null)
            {
                currentConnection.IsConnected = false;
                OnStatusChanged($"Disconnected from PI Server: {currentConnection.ServerName}");
                OnConnectionChanged(currentConnection);
            }
            
            piPoints.Clear();
            currentConnection = null;
        }
        
        public async Task<List<PIPoint>> SearchPIPointsAsync(string searchPattern = "*")
        {
            if (!IsConnected)
                throw new InvalidOperationException("Not connected to PI Server");
                
            OnStatusChanged("Searching PI Points...");
            
            // Simulate PI point search
            await Task.Delay(1000);
            
            var results = new List<PIPoint>();
            
            // Add some sample PI points for demonstration
            results.AddRange(new[]
            {
                new PIPoint("Tank01.Level", "40001", PIPointType.Float32) { Description = "Tank 1 Level", Units = "%" },
                new PIPoint("Tank01.Temperature", "40002", PIPointType.Float32) { Description = "Tank 1 Temperature", Units = "°C" },
                new PIPoint("Tank01.Pressure", "40003", PIPointType.Float32) { Description = "Tank 1 Pressure", Units = "bar" },
                new PIPoint("Pump01.Status", "40004", PIPointType.Digital) { Description = "Pump 1 Status", DigitalStates = "Off,On,Fault" },
                new PIPoint("Valve01.Position", "40005", PIPointType.Float32) { Description = "Valve 1 Position", Units = "%" }
            });
            
            OnStatusChanged($"Found {results.Count} PI Points");
            return results;
        }
        
        public async Task<bool> CreatePIPointAsync(PIPoint point)
        {
            if (!IsConnected)
                throw new InvalidOperationException("Not connected to PI Server");
                
            OnStatusChanged($"Creating PI Point: {point.Name}");
            
            // Simulate PI point creation
            await Task.Delay(500);
            
            if (!piPoints.Any(p => p.Name == point.Name))
            {
                piPoints.Add(point);
                OnStatusChanged($"PI Point created: {point.Name}");
                return true;
            }
            
            throw new InvalidOperationException($"PI Point already exists: {point.Name}");
        }
        
        public async Task<bool> UpdatePIPointAsync(PIPoint point)
        {
            if (!IsConnected)
                throw new InvalidOperationException("Not connected to PI Server");
                
            OnStatusChanged($"Updating PI Point: {point.Name}");
            
            // Simulate PI point update
            await Task.Delay(500);
            
            var existingPoint = piPoints.FirstOrDefault(p => p.Id == point.Id);
            if (existingPoint != null)
            {
                var index = piPoints.IndexOf(existingPoint);
                piPoints[index] = point;
                OnStatusChanged($"PI Point updated: {point.Name}");
                return true;
            }
            
            throw new InvalidOperationException($"PI Point not found: {point.Name}");
        }
        
        public async Task<bool> DeletePIPointAsync(string pointName)
        {
            if (!IsConnected)
                throw new InvalidOperationException("Not connected to PI Server");
                
            OnStatusChanged($"Deleting PI Point: {pointName}");
            
            // Simulate PI point deletion
            await Task.Delay(500);
            
            var point = piPoints.FirstOrDefault(p => p.Name == pointName);
            if (point != null)
            {
                piPoints.Remove(point);
                OnStatusChanged($"PI Point deleted: {pointName}");
                return true;
            }
            
            throw new InvalidOperationException($"PI Point not found: {pointName}");
        }
        
        public async Task<object?> ReadPIPointValueAsync(string pointName)
        {
            if (!IsConnected)
                throw new InvalidOperationException("Not connected to PI Server");
                
            var point = piPoints.FirstOrDefault(p => p.Name == pointName);
            if (point == null)
                throw new InvalidOperationException($"PI Point not found: {pointName}");
                
            // Simulate reading current value
            await Task.Delay(100);
            
            // Generate random values for demonstration
            var random = new Random();
            object value = point.Type switch
            {
                PIPointType.Float32 or PIPointType.Float64 => random.NextDouble() * 100,
                PIPointType.Int16 or PIPointType.Int32 => random.Next(0, 100),
                PIPointType.Digital => random.Next(0, 2),
                PIPointType.String => $"Status_{random.Next(1, 10)}",
                _ => 0
            };
            
            point.UpdateValue(value, DateTime.Now);
            return value;
        }
        
        public async Task<bool> WritePIPointValueAsync(string pointName, object value)
        {
            if (!IsConnected)
                throw new InvalidOperationException("Not connected to PI Server");
                
            var point = piPoints.FirstOrDefault(p => p.Name == pointName);
            if (point == null)
                throw new InvalidOperationException($"PI Point not found: {pointName}");
                
            OnStatusChanged($"Writing value to PI Point: {pointName}");
            
            // Simulate writing value
            await Task.Delay(200);
            
            point.UpdateValue(value, DateTime.Now);
            OnStatusChanged($"Value written to PI Point: {pointName} = {value}");
            
            return true;
        }
        
        public async Task<List<string>> GetPIServerListAsync()
        {
            OnStatusChanged("Discovering PI Servers...");
            
            // Simulate server discovery
            await Task.Delay(1500);
            
            var servers = new List<string>
            {
                "PISERVER01",
                "PISERVER02",
                "PIARCHIVE",
                "localhost",
                "127.0.0.1"
            };
            
            OnStatusChanged($"Found {servers.Count} PI Servers");
            return servers;
        }
        
        public async Task<bool> TestConnectionAsync(string serverName, int timeout = 5000)
        {
            OnStatusChanged($"Testing connection to: {serverName}");
            
            // Simulate connection test
            await Task.Delay(Math.Min(timeout, 2000));
            
            // Simulate success for demonstration (in real implementation, this would ping the server)
            var success = !string.IsNullOrEmpty(serverName);
            
            OnStatusChanged(success ? 
                $"Connection test successful: {serverName}" : 
                $"Connection test failed: {serverName}");
                
            return success;
        }
        
        private async Task LoadPIPointsAsync()
        {
            OnStatusChanged("Loading PI Points...");
            
            // Simulate loading existing PI points
            await Task.Delay(1000);
            
            piPoints.Clear();
            var samplePoints = await SearchPIPointsAsync();
            piPoints.AddRange(samplePoints);
            
            OnStatusChanged($"Loaded {piPoints.Count} PI Points");
        }
        
        protected virtual void OnStatusChanged(string status)
        {
            StatusChanged?.Invoke(this, status);
        }
        
        protected virtual void OnConnectionChanged(PIServerConnection connection)
        {
            ConnectionChanged?.Invoke(this, connection);
        }
    }
} 