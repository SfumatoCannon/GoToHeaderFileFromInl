using System;
using System.Runtime.InteropServices;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.VisualStudio.Shell;

namespace GoToHeaderFileFromInl
{
    /// <summary>
    /// VSIX package that registers the toggle command.
    /// </summary>
    [PackageRegistration(UseManagedResourcesOnly = true, AllowsBackgroundLoading = true)]
    [InstalledProductRegistration("Go To Header/Inline File", "Toggle between a .h file and its same-named .inl file.", "1.0")]
    [ProvideMenuResource("Menus.ctmenu", 1)]
    [Guid(PackageGuidString)]
    public sealed class ToggleHeaderInlPackage : AsyncPackage
    {
        public const string PackageGuidString = "8e4f2a31-6b7d-4c15-9e3a-71d2c4b5a6f0";

        protected override async Task InitializeAsync(CancellationToken cancellationToken, IProgress<ServiceProgressData> progress)
        {
            await this.JoinableTaskFactory.SwitchToMainThreadAsync(cancellationToken);
            await ToggleHeaderInlCommand.InitializeAsync(this);
        }
    }
}
