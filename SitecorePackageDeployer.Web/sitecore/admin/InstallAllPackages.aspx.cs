using Hhogdev.SitecorePackageDeployer.Tasks;
using System;
using System.Threading;
using System.Threading.Tasks;
using Hhogdev.SitecorePackageDeployer.Entities;

namespace Hhogdev.SitecorePackageDeployer.Web.sitecore.admin
{
    public partial class InstallAllPackages : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (Request.QueryString["force"] == "1")
            {
                InstallPackage.ResetInstallState();
            }

            var installer = new InstallPackage();
            var packages = installer.Run();

            foreach (var package in packages)
            {
                Response.Write($"{package.Name} - {package.Status.ToString()}\r\n");
            }

            if (packages.FindAll(p => p.Status == SitecorePackageStatus.Failed).Count > 0)
                Response.StatusCode = 500;
        }
    }
}