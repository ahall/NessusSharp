using System;
using System.Xml.Linq;
using System.Collections.Generic;

namespace NessusSharp
{
    public class ParseReportHostItem
    {
        public int Port { get; set; }
        public string SvcName { get; set; }
        public string Protocol { get; set; }
        public string RiskFactor { get; set; }
        public string Description { get; set; }
        public string Synopsis { get; set; }
        public string Solution { get; set; }
        public string PluginType { get; set; }
        public string PluginName { get; set; }
        public int PluginId { get; set; }
        public string PluginOutput { get; set; }
    }

    public class ParseReportHost
    {
        public string Name { get; set; }
        public List<ParseReportHostItem> Items { get; set; }
        public ParseHostProperties Properties { get; set; }

        public int NumOpenPorts { get; set; }
        public int NumVulnHigh { get; set; }
        public int NumVulnMedium { get; set; }
        public int NumVulnLow { get; set; }
    }

    public class ParseReport
    {
        public List<ParseReportHost> Hosts { get; set; }
    }

    public class ParseHostProperties
    {
        public string HostStart { get; set; }
        public string HostEnd { get; set; }
        public string OperatingSystem { get; set; }
        public string HostIp { get; set; }
        public string HostFqdn { get; set; }
        public string NetBiosName { get; set; }
        public string MacAddress { get; set; }
    }

    /// <summary>
    /// Parses .nessus files.
    /// </summary>
    public class NessusParser
    {
        private string path;

        public NessusParser(string path)
        {
            this.path = path;
        }

        public ParseReport Run()
        {
            var parseReport = new ParseReport();
            parseReport.Hosts = new List<ParseReportHost>();

            var element = XElement.Load(path);
            var report = element.Element("Report");
            foreach (var host in report.Elements("ReportHost"))
            {
                var parseHost = new ParseReportHost();
                parseHost.Items = new List<ParseReportHostItem>();

                parseHost.Name = (string)host.Attribute("name");
                if (string.IsNullOrEmpty(parseHost.Name))
                {
                    continue;
                }

                foreach (var item in host.Elements("ReportItem"))
                {
                    var parseReportHostItem = new ParseReportHostItem();
                    parseReportHostItem.Port = (int)item.Attribute("port");
                    parseReportHostItem.SvcName = (string)item.Attribute("svc_name");
                    parseReportHostItem.Protocol = (string)item.Attribute("protocol");
                    parseReportHostItem.PluginId = (int)item.Attribute("pluginID");
                    parseReportHostItem.PluginName = (string)item.Attribute("pluginName");

                    parseReportHostItem.Synopsis = (string)item.Element("synopsis");
                    parseReportHostItem.Description = (string)item.Element("description");

                    parseReportHostItem.RiskFactor = (string)item.Element("risk_factor");
                    switch (parseReportHostItem.RiskFactor)
                    {
                        case "High":
                            parseHost.NumVulnHigh++;
                            break;
                        case "Medium":
                            parseHost.NumVulnMedium++;
                            break;
                        case "None":
                        case "Low":
                            parseHost.NumVulnLow++;
                            break;
                        default:
                            break;
                    }

                    parseReportHostItem.Solution = (string)item.Element("solution");
                    parseReportHostItem.PluginType = (string)item.Element("plugin_type");
                    parseReportHostItem.PluginOutput = (string)item.Element("plugin_output");
                    parseHost.Items.Add(parseReportHostItem);
                }

                var parseHostProps = new ParseHostProperties();
                foreach (var hostProp in host.Elements("HostProperties"))
                {
                    foreach (var prop in hostProp.Elements())
                    {
                        string propName = (string)prop.Attribute("name");
                        if (propName == "HOST_START")
                        {
                            parseHostProps.HostStart = prop.Value;
                        }
                        else if (propName == "HOST_END")
                        {
                            parseHostProps.HostEnd = prop.Value;
                        }
                        else if (propName == "operating-system")
                        {
                            parseHostProps.OperatingSystem = prop.Value;
                        }
                        else if (propName == "host-ip")
                        {
                            parseHostProps.HostIp = prop.Value;
                        }
                        else if (propName == "host-fqdn")
                        {
                            parseHostProps.HostFqdn = prop.Value;
                        }
                        else if (propName == "netbios-name")
                        {
                            parseHostProps.NetBiosName = prop.Value;
                        }
                        else if (propName == "mac-address")
                        {
                            parseHostProps.MacAddress = prop.Value;
                        }
                    }

                    parseHost.Properties = parseHostProps;
                }

                // TODO: Need to check a special plugin to get this, leaving as 0 for now.
                parseHost.NumOpenPorts = 0;

                parseReport.Hosts.Add(parseHost);
            }

            return parseReport;
        }
    }
}

