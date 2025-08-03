using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.ServiceProcess;
using PIInterfaceConfigUtility.Models;

namespace PIInterfaceConfigUtility.Services
{
    public class InterfaceManager
    {
        private readonly List<PIInterface> interfaces = new();
        
        public event EventHandler<string>? StatusChanged;
        public event EventHandler<PIInterface>? InterfaceStatusChanged;
        
        public IReadOnlyList<PIInterface> Interfaces => interfaces.AsReadOnly();
        
        public void AddInterface(PIInterface piInterface)
        {
            if (interfaces.Any(i => i.Name == piInterface.Name))
                throw new InvalidOperationException($"Interface with name '{piInterface.Name}' already exists");
                
            interfaces.Add(piInterface);
            OnStatusChanged($"Interface added: {piInterface.Name}");
        }
        
        public void RemoveInterface(string interfaceName)
        {
            var interfaceToRemove = interfaces.FirstOrDefault(i => i.Name == interfaceName);
            if (interfaceToRemove != null)
            {
                interfaces.Remove(interfaceToRemove);
                OnStatusChanged($"Interface removed: {interfaceName}");
            }
        }
        
        public PIInterface? GetInterface(string interfaceName)
        {
            return interfaces.FirstOrDefault(i => i.Name == interfaceName);
        }
        
        public async Task<bool> StartInterfaceAsync(string interfaceName)
        {
            var piInterface = GetInterface(interfaceName);
            if (piInterface == null)
                throw new InvalidOperationException($"Interface not found: {interfaceName}");
                
            OnStatusChanged($"Starting interface: {interfaceName}");
            piInterface.Status = InterfaceStatus.Starting;
            OnInterfaceStatusChanged(piInterface);
            
            try
            {
                // Simulate interface startup
                await Task.Delay(2000);
                
                // In a real implementation, this would start the actual service
                if (!string.IsNullOrEmpty(piInterface.ServiceName))
                {
                    // Try to start Windows service
                    try
                    {
                        using var service = new ServiceController(piInterface.ServiceName);
                        if (service.Status == ServiceControllerStatus.Stopped)
                        {
                            service.Start();
                            service.WaitForStatus(ServiceControllerStatus.Running, TimeSpan.FromSeconds(30));
                        }
                    }
                    catch
                    {
                        // Service might not exist, continue with simulation
                    }
                }
                
                piInterface.Status = InterfaceStatus.Running;
                piInterface.LastStarted = DateTime.Now;
                OnInterfaceStatusChanged(piInterface);
                OnStatusChanged($"Interface started: {interfaceName}");
                
                return true;
            }
            catch (Exception ex)
            {
                piInterface.Status = InterfaceStatus.Error;
                OnInterfaceStatusChanged(piInterface);
                OnStatusChanged($"Failed to start interface {interfaceName}: {ex.Message}");
                return false;
            }
        }
        
        public async Task<bool> StopInterfaceAsync(string interfaceName)
        {
            var piInterface = GetInterface(interfaceName);
            if (piInterface == null)
                throw new InvalidOperationException($"Interface not found: {interfaceName}");
                
            OnStatusChanged($"Stopping interface: {interfaceName}");
            piInterface.Status = InterfaceStatus.Stopping;
            OnInterfaceStatusChanged(piInterface);
            
            try
            {
                // Simulate interface shutdown
                await Task.Delay(1500);
                
                // In a real implementation, this would stop the actual service
                if (!string.IsNullOrEmpty(piInterface.ServiceName))
                {
                    try
                    {
                        using var service = new ServiceController(piInterface.ServiceName);
                        if (service.Status == ServiceControllerStatus.Running)
                        {
                            service.Stop();
                            service.WaitForStatus(ServiceControllerStatus.Stopped, TimeSpan.FromSeconds(30));
                        }
                    }
                    catch
                    {
                        // Service might not exist, continue with simulation
                    }
                }
                
                piInterface.Status = InterfaceStatus.Stopped;
                piInterface.LastStopped = DateTime.Now;
                OnInterfaceStatusChanged(piInterface);
                OnStatusChanged($"Interface stopped: {interfaceName}");
                
                return true;
            }
            catch (Exception ex)
            {
                piInterface.Status = InterfaceStatus.Error;
                OnInterfaceStatusChanged(piInterface);
                OnStatusChanged($"Failed to stop interface {interfaceName}: {ex.Message}");
                return false;
            }
        }
        
        public async Task StartAllInterfacesAsync()
        {
            OnStatusChanged("Starting all interfaces...");
            
            var tasks = interfaces.Where(i => i.Status == InterfaceStatus.Stopped)
                                 .Select(i => StartInterfaceAsync(i.Name));
                                 
            await Task.WhenAll(tasks);
            
            var runningCount = interfaces.Count(i => i.Status == InterfaceStatus.Running);
            OnStatusChanged($"Started {runningCount} interfaces");
        }
        
        public async Task StopAllInterfacesAsync()
        {
            OnStatusChanged("Stopping all interfaces...");
            
            var tasks = interfaces.Where(i => i.Status == InterfaceStatus.Running)
                                 .Select(i => StopInterfaceAsync(i.Name));
                                 
            await Task.WhenAll(tasks);
            
            var stoppedCount = interfaces.Count(i => i.Status == InterfaceStatus.Stopped);
            OnStatusChanged($"Stopped {stoppedCount} interfaces");
        }
        
        public async Task<InterfaceStatus> GetInterfaceStatusAsync(string interfaceName)
        {
            var piInterface = GetInterface(interfaceName);
            if (piInterface == null)
                throw new InvalidOperationException($"Interface not found: {interfaceName}");
                
            // Simulate status check
            await Task.Delay(100);
            
            // In a real implementation, this would check the actual service status
            if (!string.IsNullOrEmpty(piInterface.ServiceName))
            {
                try
                {
                    using var service = new ServiceController(piInterface.ServiceName);
                    piInterface.Status = service.Status switch
                    {
                        ServiceControllerStatus.Running => InterfaceStatus.Running,
                        ServiceControllerStatus.Stopped => InterfaceStatus.Stopped,
                        ServiceControllerStatus.StartPending => InterfaceStatus.Starting,
                        ServiceControllerStatus.StopPending => InterfaceStatus.Stopping,
                        _ => InterfaceStatus.Unknown
                    };
                }
                catch
                {
                    // Service doesn't exist or access denied
                    piInterface.Status = InterfaceStatus.Unknown;
                }
            }
            
            return piInterface.Status;
        }
        
        public async Task RefreshAllInterfaceStatusAsync()
        {
            OnStatusChanged("Refreshing interface status...");
            
            foreach (var piInterface in interfaces)
            {
                var status = await GetInterfaceStatusAsync(piInterface.Name);
                if (piInterface.Status != status)
                {
                    piInterface.Status = status;
                    OnInterfaceStatusChanged(piInterface);
                }
            }
            
            OnStatusChanged("Interface status refreshed");
        }
        
        public PIInterface CreateInterface(string name, InterfaceType type)
        {
            var piInterface = new PIInterface(name, type);
            
            // Set default properties based on interface type
            switch (type)
            {
                case InterfaceType.OPCDA:
                    piInterface.AddProperty("OPCServer", "Matrikon.OPC.Simulation.1");
                    piInterface.AddProperty("GroupUpdateRate", 1000);
                    piInterface.AddProperty("ConnectionTimeout", 30);
                    break;
                case InterfaceType.OPCAE:
                    piInterface.AddProperty("OPCAEServer", "Matrikon.OPC.AlarmEvent.1");
                    piInterface.AddProperty("SubscriptionRate", 500);
                    break;
                case InterfaceType.PIPing:
                    piInterface.AddProperty("PointSource", "PING");
                    piInterface.AddProperty("ScanFrequency", 30000);
                    break;
                case InterfaceType.UFL:
                    piInterface.AddProperty("InputDirectory", @"C:\PI\Interfaces\UFL\Input");
                    piInterface.AddProperty("ProcessedDirectory", @"C:\PI\Interfaces\UFL\Processed");
                    break;
            }
            
            return piInterface;
        }
        
        public void UpdateInterface(PIInterface updatedInterface)
        {
            var existingInterface = interfaces.FirstOrDefault(i => i.Id == updatedInterface.Id);
            if (existingInterface != null)
            {
                var index = interfaces.IndexOf(existingInterface);
                interfaces[index] = updatedInterface;
                OnStatusChanged($"Interface updated: {updatedInterface.Name}");
                OnInterfaceStatusChanged(updatedInterface);
            }
        }
        
        public async Task<Dictionary<string, object>> GetInterfaceStatisticsAsync(string interfaceName)
        {
            var piInterface = GetInterface(interfaceName);
            if (piInterface == null)
                throw new InvalidOperationException($"Interface not found: {interfaceName}");
                
            // Simulate gathering statistics
            await Task.Delay(200);
            
            var stats = new Dictionary<string, object>
            {
                ["MessagesReceived"] = piInterface.MessagesReceived,
                ["MessagesSent"] = piInterface.MessagesSent,
                ["ErrorCount"] = piInterface.ErrorCount,
                ["LastStarted"] = piInterface.LastStarted?.ToString("yyyy-MM-dd HH:mm:ss") ?? "Never",
                ["LastStopped"] = piInterface.LastStopped?.ToString("yyyy-MM-dd HH:mm:ss") ?? "Never",
                ["PointCount"] = piInterface.Points.Count,
                ["ActivePoints"] = piInterface.Points.Count(p => p.Enabled),
                ["Uptime"] = piInterface.LastStarted.HasValue ? 
                    DateTime.Now - piInterface.LastStarted.Value : TimeSpan.Zero
            };
            
            return stats;
        }
        
        protected virtual void OnStatusChanged(string status)
        {
            StatusChanged?.Invoke(this, status);
        }
        
        protected virtual void OnInterfaceStatusChanged(PIInterface piInterface)
        {
            InterfaceStatusChanged?.Invoke(this, piInterface);
        }
    }
} 