using System;
using Microsoft.SharePoint.Administration;
using SPCommon.Interface;

namespace SPCommon.Infrastructure.Common
{
    public class SPLogger : ILogger
    {
        #region Singleton 

        private SPLogger() {}
        private static SPLogger _instance;
        public static SPLogger Instance
        {
            get { return _instance ?? (_instance = new SPLogger()); }
        }

        #endregion

        public void Log(string message)
        {
            SPDiagnosticsService.Local.WriteTrace(
                0,
                new SPDiagnosticsCategory(Constants.Logging.AreaName, TraceSeverity.Medium, EventSeverity.Information),
                TraceSeverity.Medium,
                message,
                null);
        }

        public void Log(Exception ex)
        {
            SPDiagnosticsService.Local.WriteTrace(
                0, 
                new SPDiagnosticsCategory(Constants.Logging.AreaName, TraceSeverity.Unexpected, EventSeverity.Error), 
                TraceSeverity.Unexpected, 
                ex.Message, 
                ex.StackTrace);
        }

    }
}
