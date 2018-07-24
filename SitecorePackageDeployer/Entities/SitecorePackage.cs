using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Hhogdev.SitecorePackageDeployer.Entities
{
    public class SitecorePackage
    {
        public string Name { get; set; }

        public SitecorePackageStatus Status { get; set; }

    }

    public enum SitecorePackageStatus
    {
        Ready,
        Installed,
        Failed,
        Aborted,
        Shutdown
    }
}
