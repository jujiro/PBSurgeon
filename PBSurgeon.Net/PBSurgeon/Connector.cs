using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Text.RegularExpressions;
using Tab = Microsoft.AnalysisServices.Tabular;

namespace PBSurgeon
{
    /// <summary>
    /// Finds live connections to Power BI Tabular Services
    /// </summary>
    public static class Connector
    {
        const string pbiWorkspacePath = @"%LocalAppData%\Microsoft\Power BI Desktop\AnalysisServicesWorkspaces";
        const string portFile = @"data\msmdsrv.port.txt";
        const string flightRecorderTrace = @"data\FlightRecorderCurrent.trc";
        const string pbixFileXmlPattern = @"<ddl700_700.*>(.*\.pbix)</ddl700_700.*>";
        const string connStringTemplate = "localhost:{0}";
        static Connector()
        {
            Connections = new Dictionary<string, string>();
        }
        public static Dictionary<string, string> Connections { get; set; }        
        public static void GetActiveConnections()
        {            
            var _connections = new Dictionary<string, string>();
            var pbiPath = System.Environment.ExpandEnvironmentVariables(pbiWorkspacePath);
            var folders = Directory.GetDirectories(pbiPath, "*", SearchOption.TopDirectoryOnly);
            foreach (var folder in folders)
            {
                if (!File.GetAttributes(folder).HasFlag(FileAttributes.Directory)) continue;
                if (!File.Exists(Path.Combine(folder, portFile))) continue;
                string port = File.ReadAllText(Path.Combine(folder, portFile), Encoding.Unicode).Replace("\r\n", "").Replace("\n", "");
                if (string.IsNullOrWhiteSpace(port)) continue;
                string pbixFileName = "Unknown so far";
                if (File.Exists(Path.Combine(folder, flightRecorderTrace)))
                {
                    var fs = new FileStream(Path.Combine(folder, flightRecorderTrace), FileMode.Open, FileAccess.Read, FileShare.ReadWrite);
                    var sr = new StreamReader(fs, Encoding.UTF8);
                    string temp = sr.ReadToEnd();
                    var regex = new Regex(pbixFileXmlPattern, RegexOptions.IgnoreCase);
                    var matches = regex.Match(temp);
                    var fileName = "";
                    if (matches.Groups.Count > 1) fileName = matches.Groups[1].Value;
                    if ((!String.IsNullOrWhiteSpace(fileName)) && (File.Exists(fileName))) pbixFileName = fileName;
                }
                _connections.Add(string.Format(connStringTemplate, port), pbixFileName);
            }
            Connections = new Dictionary<string, string>();
            ValidateConnections(_connections);
        }
        private static void ValidateConnections(Dictionary<string, string> conns)
        {
            foreach (var conn in conns)
            {
                var srv = new Tab.Server();
                try
                {
                    srv.Connect(conn.Key);
                    srv.Disconnect();
                    Connections.Add(conn.Key, conn.Value);
                }
                catch { }
            }
        }
    }
}
