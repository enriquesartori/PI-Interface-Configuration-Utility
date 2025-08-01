using System;
using System.IO;
using System.Text.Json;
using System.Xml;
using System.Collections.Generic;
using PIInterfaceConfigUtility.Models;
using Newtonsoft.Json;

namespace PIInterfaceConfigUtility.Services
{
    public class ConfigurationData
    {
        public PIServerConnection? ServerConnection { get; set; }
        public List<PIInterface> Interfaces { get; set; } = new();
        public List<PIPoint> Points { get; set; } = new();
        public Dictionary<string, string> Settings { get; set; } = new();
        public DateTime Created { get; set; } = DateTime.Now;
        public DateTime LastModified { get; set; } = DateTime.Now;
        public string Version { get; set; } = "1.0";
    }
    
    public class ConfigurationManager
    {
        private ConfigurationData currentConfiguration = new();
        private string? currentFilePath;
        
        public event EventHandler<string>? StatusChanged;
        
        public ConfigurationData CurrentConfiguration => currentConfiguration;
        public bool HasUnsavedChanges { get; private set; }
        public string? CurrentFilePath => currentFilePath;
        
        public void NewConfiguration()
        {
            currentConfiguration = new ConfigurationData();
            currentFilePath = null;
            HasUnsavedChanges = false;
            OnStatusChanged("New configuration created");
        }
        
        public void LoadConfiguration(string filePath)
        {
            if (!File.Exists(filePath))
                throw new FileNotFoundException($"Configuration file not found: {filePath}");
                
            try
            {
                var json = File.ReadAllText(filePath);
                var config = JsonConvert.DeserializeObject<ConfigurationData>(json);
                
                if (config == null)
                    throw new InvalidOperationException("Invalid configuration file format");
                    
                currentConfiguration = config;
                currentFilePath = filePath;
                HasUnsavedChanges = false;
                
                OnStatusChanged($"Configuration loaded from: {filePath}");
            }
            catch (Exception ex)
            {
                throw new InvalidOperationException($"Failed to load configuration: {ex.Message}", ex);
            }
        }
        
        public void SaveConfiguration()
        {
            if (string.IsNullOrEmpty(currentFilePath))
                throw new InvalidOperationException("No file path specified. Use SaveConfigurationAs instead.");
                
            SaveConfigurationAs(currentFilePath);
        }
        
        public void SaveConfigurationAs(string filePath)
        {
            try
            {
                currentConfiguration.LastModified = DateTime.Now;
                
                var settings = new JsonSerializerSettings
                {
                    Formatting = Formatting.Indented,
                    NullValueHandling = NullValueHandling.Ignore,
                    DateFormatHandling = DateFormatHandling.IsoDateFormat
                };
                
                var json = JsonConvert.SerializeObject(currentConfiguration, settings);
                
                // Ensure directory exists
                var directory = Path.GetDirectoryName(filePath);
                if (!string.IsNullOrEmpty(directory) && !Directory.Exists(directory))
                    Directory.CreateDirectory(directory);
                    
                File.WriteAllText(filePath, json);
                
                currentFilePath = filePath;
                HasUnsavedChanges = false;
                
                OnStatusChanged($"Configuration saved to: {filePath}");
            }
            catch (Exception ex)
            {
                throw new InvalidOperationException($"Failed to save configuration: {ex.Message}", ex);
            }
        }
        
        public void ImportConfiguration(string filePath)
        {
            if (!File.Exists(filePath))
                throw new FileNotFoundException($"Import file not found: {filePath}");
                
            var extension = Path.GetExtension(filePath).ToLower();
            
            try
            {
                switch (extension)
                {
                    case ".json":
                        ImportFromJson(filePath);
                        break;
                    case ".xml":
                        ImportFromXml(filePath);
                        break;
                    case ".csv":
                        ImportFromCsv(filePath);
                        break;
                    default:
                        throw new NotSupportedException($"Import format not supported: {extension}");
                }
                
                HasUnsavedChanges = true;
                OnStatusChanged($"Configuration imported from: {filePath}");
            }
            catch (Exception ex)
            {
                throw new InvalidOperationException($"Failed to import configuration: {ex.Message}", ex);
            }
        }
        
        public void ExportConfiguration(string filePath)
        {
            var extension = Path.GetExtension(filePath).ToLower();
            
            try
            {
                // Ensure directory exists
                var directory = Path.GetDirectoryName(filePath);
                if (!string.IsNullOrEmpty(directory) && !Directory.Exists(directory))
                    Directory.CreateDirectory(directory);
                    
                switch (extension)
                {
                    case ".json":
                        ExportToJson(filePath);
                        break;
                    case ".xml":
                        ExportToXml(filePath);
                        break;
                    case ".csv":
                        ExportToCsv(filePath);
                        break;
                    default:
                        throw new NotSupportedException($"Export format not supported: {extension}");
                }
                
                OnStatusChanged($"Configuration exported to: {filePath}");
            }
            catch (Exception ex)
            {
                throw new InvalidOperationException($"Failed to export configuration: {ex.Message}", ex);
            }
        }
        
        private void ImportFromJson(string filePath)
        {
            var json = File.ReadAllText(filePath);
            var importedConfig = JsonConvert.DeserializeObject<ConfigurationData>(json);
            
            if (importedConfig == null)
                throw new InvalidOperationException("Invalid JSON configuration format");
                
            // Merge with current configuration
            if (importedConfig.ServerConnection != null)
                currentConfiguration.ServerConnection = importedConfig.ServerConnection;
                
            currentConfiguration.Interfaces.AddRange(importedConfig.Interfaces);
            currentConfiguration.Points.AddRange(importedConfig.Points);
            
            foreach (var setting in importedConfig.Settings)
                currentConfiguration.Settings[setting.Key] = setting.Value;
        }
        
        private void ImportFromXml(string filePath)
        {
            var doc = new XmlDocument();
            doc.Load(filePath);
            
            var root = doc.DocumentElement;
            if (root?.Name != "PIConfiguration")
                throw new InvalidOperationException("Invalid XML configuration format");
                
            // Import interfaces
            var interfacesNode = root.SelectSingleNode("Interfaces");
            if (interfacesNode != null)
            {
                foreach (XmlNode interfaceNode in interfacesNode.ChildNodes)
                {
                    if (interfaceNode.Name == "Interface")
                    {
                        var piInterface = new PIInterface
                        {
                            Name = interfaceNode.Attributes?["Name"]?.Value ?? "",
                            Description = interfaceNode.Attributes?["Description"]?.Value ?? "",
                            Type = Enum.TryParse<InterfaceType>(interfaceNode.Attributes?["Type"]?.Value, out var type) ? type : InterfaceType.OPC_DA
                        };
                        
                        currentConfiguration.Interfaces.Add(piInterface);
                    }
                }
            }
            
            // Import PI points
            var pointsNode = root.SelectSingleNode("Points");
            if (pointsNode != null)
            {
                foreach (XmlNode pointNode in pointsNode.ChildNodes)
                {
                    if (pointNode.Name == "Point")
                    {
                        var point = new PIPoint
                        {
                            Name = pointNode.Attributes?["Name"]?.Value ?? "",
                            Description = pointNode.Attributes?["Description"]?.Value ?? "",
                            SourceAddress = pointNode.Attributes?["SourceAddress"]?.Value ?? "",
                            Type = Enum.TryParse<PIPointType>(pointNode.Attributes?["Type"]?.Value, out var pointType) ? pointType : PIPointType.Float32
                        };
                        
                        currentConfiguration.Points.Add(point);
                    }
                }
            }
        }
        
        private void ImportFromCsv(string filePath)
        {
            var lines = File.ReadAllLines(filePath);
            if (lines.Length < 2)
                throw new InvalidOperationException("CSV file must contain header and at least one data row");
                
            var headers = lines[0].Split(',');
            
            for (int i = 1; i < lines.Length; i++)
            {
                var values = lines[i].Split(',');
                if (values.Length >= headers.Length)
                {
                    var point = new PIPoint();
                    
                    for (int j = 0; j < headers.Length; j++)
                    {
                        var header = headers[j].Trim();
                        var value = values[j].Trim();
                        
                        switch (header.ToLower())
                        {
                            case "name":
                                point.Name = value;
                                break;
                            case "description":
                                point.Description = value;
                                break;
                            case "sourceaddress":
                                point.SourceAddress = value;
                                break;
                            case "type":
                                if (Enum.TryParse<PIPointType>(value, out var pointType))
                                    point.Type = pointType;
                                break;
                            case "units":
                                point.Units = value;
                                break;
                        }
                    }
                    
                    if (!string.IsNullOrEmpty(point.Name))
                        currentConfiguration.Points.Add(point);
                }
            }
        }
        
        private void ExportToJson(string filePath)
        {
            var settings = new JsonSerializerSettings
            {
                Formatting = Formatting.Indented,
                NullValueHandling = NullValueHandling.Ignore,
                DateFormatHandling = DateFormatHandling.IsoDateFormat
            };
            
            var json = JsonConvert.SerializeObject(currentConfiguration, settings);
            File.WriteAllText(filePath, json);
        }
        
        private void ExportToXml(string filePath)
        {
            var doc = new XmlDocument();
            var declaration = doc.CreateXmlDeclaration("1.0", "UTF-8", null);
            doc.AppendChild(declaration);
            
            var root = doc.CreateElement("PIConfiguration");
            root.SetAttribute("Version", currentConfiguration.Version);
            root.SetAttribute("Created", currentConfiguration.Created.ToString("yyyy-MM-ddTHH:mm:ss"));
            doc.AppendChild(root);
            
            // Export interfaces
            var interfacesElement = doc.CreateElement("Interfaces");
            foreach (var piInterface in currentConfiguration.Interfaces)
            {
                var interfaceElement = doc.CreateElement("Interface");
                interfaceElement.SetAttribute("Name", piInterface.Name);
                interfaceElement.SetAttribute("Description", piInterface.Description);
                interfaceElement.SetAttribute("Type", piInterface.Type.ToString());
                interfacesElement.AppendChild(interfaceElement);
            }
            root.AppendChild(interfacesElement);
            
            // Export PI points
            var pointsElement = doc.CreateElement("Points");
            foreach (var point in currentConfiguration.Points)
            {
                var pointElement = doc.CreateElement("Point");
                pointElement.SetAttribute("Name", point.Name);
                pointElement.SetAttribute("Description", point.Description);
                pointElement.SetAttribute("SourceAddress", point.SourceAddress);
                pointElement.SetAttribute("Type", point.Type.ToString());
                pointElement.SetAttribute("Units", point.Units);
                pointsElement.AppendChild(pointElement);
            }
            root.AppendChild(pointsElement);
            
            doc.Save(filePath);
        }
        
        private void ExportToCsv(string filePath)
        {
            using var writer = new StreamWriter(filePath);
            
            // Write header
            writer.WriteLine("Name,Description,SourceAddress,Type,Units,InterfaceId,Enabled");
            
            // Write PI points
            foreach (var point in currentConfiguration.Points)
            {
                writer.WriteLine($"{EscapeCsvValue(point.Name)},{EscapeCsvValue(point.Description)},{EscapeCsvValue(point.SourceAddress)},{point.Type},{EscapeCsvValue(point.Units)},{EscapeCsvValue(point.InterfaceId)},{point.Enabled}");
            }
        }
        
        private string EscapeCsvValue(string value)
        {
            if (string.IsNullOrEmpty(value))
                return "";
                
            if (value.Contains(",") || value.Contains("\"") || value.Contains("\n"))
                return $"\"{value.Replace("\"", "\"\"")}\"";
                
            return value;
        }
        
        public void SetServerConnection(PIServerConnection connection)
        {
            currentConfiguration.ServerConnection = connection;
            HasUnsavedChanges = true;
        }
        
        public void AddInterface(PIInterface piInterface)
        {
            currentConfiguration.Interfaces.Add(piInterface);
            HasUnsavedChanges = true;
        }
        
        public void RemoveInterface(string interfaceId)
        {
            currentConfiguration.Interfaces.RemoveAll(i => i.Id == interfaceId);
            HasUnsavedChanges = true;
        }
        
        public void AddPoint(PIPoint point)
        {
            currentConfiguration.Points.Add(point);
            HasUnsavedChanges = true;
        }
        
        public void RemovePoint(string pointId)
        {
            currentConfiguration.Points.RemoveAll(p => p.Id == pointId);
            HasUnsavedChanges = true;
        }
        
        public void SetSetting(string key, string value)
        {
            currentConfiguration.Settings[key] = value;
            HasUnsavedChanges = true;
        }
        
        protected virtual void OnStatusChanged(string status)
        {
            StatusChanged?.Invoke(this, status);
        }
    }
} 