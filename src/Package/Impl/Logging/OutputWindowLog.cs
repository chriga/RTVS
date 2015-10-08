﻿using System;
using System.Diagnostics;
using System.Threading.Tasks;
using EnvDTE;
using Microsoft.R.Actions.Logging;
using Microsoft.VisualStudio.R.Package.Shell;
using Microsoft.VisualStudio.Shell.Interop;

namespace Microsoft.VisualStudio.R.Package.Logging
{
    public abstract class OutputWindowLog: LinesLog
    {
        private IVsOutputWindowPane _pane;
        private Guid _paneGuid;
        private string _windowName;

        protected OutputWindowLog(Guid paneGuid, string windowName)
        {
            _paneGuid = paneGuid;
            _windowName = windowName;
        }

        private void EnsurePaneVisible()
        {
            if (_pane == null)
            {
                // TODO: consider using IVsOutputWindow3.CreatePane2 and colorize the output
                IVsOutputWindow outputWindow = AppShell.Current.GetGlobalService<IVsOutputWindow>(typeof(SVsOutputWindow)) as IVsOutputWindow;
                outputWindow.GetPane(ref _paneGuid, out _pane);
                if (_pane == null)
                {
                    outputWindow.CreatePane(ref _paneGuid, _windowName, fInitVisible: 1, fClearWithSolution: 1);
                    outputWindow.GetPane(ref _paneGuid, out _pane);

                    Debug.Assert(_pane != null, "Cannot create output window pane " + _windowName);
                }
            }

            _pane.Activate();

            DTE dte = AppShell.Current.GetGlobalService<DTE>();
            Window window = dte.Windows.Item(EnvDTE.Constants.vsWindowKindOutput);
            if (window != null)
            {
                window.Activate();
            }
        }

        #region IActionLog
        public override Task WriteAsync(MessageCategory category, string message)
        {
            base.WriteAsync(category, message);

            EnsurePaneVisible();
            _pane.OutputStringThreadSafe(message);

            return Task.CompletedTask;
        }
        #endregion
    }
}
