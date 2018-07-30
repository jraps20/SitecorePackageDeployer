using Hhogdev.SitecorePackageDeployer.Tasks;
using System;
using System.Threading;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.Serialization;
using System.Xml.XPath;
using Newtonsoft.Json;

namespace Hhogdev.SitecorePackageDeployer.Web.sitecore.admin
{
    public partial class StartSitecorePackageDeployer : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            var force = bool.Parse(Request.QueryString["force"] ?? "false");
            var synchronous = bool.Parse(Request.QueryString["synchronous"] ?? "false");
            var pauseIndexing = bool.Parse(Request.QueryString["pauseIndexing"] ?? "true");
            var pauseEvents = bool.Parse(Request.QueryString["pauseEvents"] ?? "true");
            var responseType = Request.QueryString["response"] ?? "";
            if (force)
            {
                InstallPackage.ResetInstallState();
            }

            if (pauseIndexing)
            {
                InstallPackage.PauseIndexing = true;
            }

            if (pauseEvents)
            {
                InstallPackage.PauseEvents = true;
            }

            if (synchronous)
            {
                var task = Task.Factory.StartNew(Runner);
                Task.WaitAll(task);
            }
            else
            {
                ThreadPool.QueueUserWorkItem((ctx) =>
                {
                    Runner();
                });
            }
            var response = new InstallerResponse();
            response.Status = InstallPackage.GetInstallerState().ToString();
            response.Messages = InstallPackage.GetInstallerLog();
            if (response.Status == null && response.Messages == null)
            {
                return;
            }

            if (responseType.Equals("json", StringComparison.InvariantCultureIgnoreCase))
            {

                string output = JsonConvert.SerializeObject(response);
                Response.Clear();
                Response.ContentType = "application/json; charset=utf-8";
                Response.Write(output);
                Response.End();
            }
            else if (responseType.Equals("xml", StringComparison.InvariantCultureIgnoreCase))
            {
                XmlDocument doc = new XmlDocument();
                XmlElement messages = doc.CreateElement("messages");
                messages.SetAttribute("status", response.Status);
                doc.AppendChild(messages);
                foreach (var responseMessage in response.Messages)
                {
                    XmlElement message = doc.CreateElement("message");
                    message.InnerText = responseMessage;
                    messages.AppendChild(message);
                }
                Response.Clear();
                Response.ContentType = "application/xml; charset=utf-8";
                Response.Write(doc.OuterXml);
                Response.End();
            }
            else
            {
                State.InnerText = response.Status;
                foreach (var logEntry in response.Messages)
                {
                    Logs.InnerText += logEntry;
                }
            }
        }

        private static void Runner()
        {
            var installer = new InstallPackage();
            installer.Run();
        }

        public class InstallerResponse
        {
            public string Status;
            public string[] Messages;
        }
    }
}