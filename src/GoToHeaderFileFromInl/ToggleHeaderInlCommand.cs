using System;
using System.ComponentModel.Design;
using System.IO;
using System.Threading.Tasks;
using EnvDTE;
using EnvDTE80;
using Microsoft.VisualStudio.Shell;
using Microsoft.VisualStudio.Shell.Interop;
using Task = System.Threading.Tasks.Task;

namespace GoToHeaderFileFromInl
{
    /// <summary>
    /// Command that switches between a .h file and the .inl file with the same base name.
    /// </summary>
    internal sealed class ToggleHeaderInlCommand
    {
        public const int CommandId = 0x0100;

        public static readonly Guid CommandSet = new Guid("a3c1e5d7-2f48-4b91-8c6e-5d0a9b7e4c21");

        private const string HeaderExtension = ".h";
        private const string InlineExtension = ".inl";
        private const string MessageBoxTitle = "Go To Header/Inline File";

        private readonly AsyncPackage package;

        private ToggleHeaderInlCommand(AsyncPackage package, OleMenuCommandService commandService)
        {
            this.package = package ?? throw new ArgumentNullException(nameof(package));
            commandService = commandService ?? throw new ArgumentNullException(nameof(commandService));

            var menuCommandId = new CommandID(CommandSet, CommandId);
            commandService.AddCommand(new MenuCommand(this.Execute, menuCommandId));
        }

        public static async Task InitializeAsync(AsyncPackage package)
        {
            await ThreadHelper.JoinableTaskFactory.SwitchToMainThreadAsync(package.DisposalToken);

            object service = await package.GetServiceAsync(typeof(IMenuCommandService));
            if (service == null)
            {
                return;
            }

            new ToggleHeaderInlCommand(package, (OleMenuCommandService)service);
        }

        private void Execute(object sender, EventArgs e)
        {
            ThreadHelper.ThrowIfNotOnUIThread();

            var dte = ServiceProvider.GlobalProvider.GetService(typeof(DTE)) as DTE2;
            if (dte == null)
            {
                return;
            }

            string currentPath = dte.ActiveDocument?.FullName;

            if (string.IsNullOrEmpty(currentPath))
            {
                ShowMessage("当前没有打开的文档。请先打开一个 .h 或 .inl 文件。");
                return;
            }

            string extension = Path.GetExtension(currentPath);
            string targetExtension;

            if (string.Equals(extension, HeaderExtension, StringComparison.OrdinalIgnoreCase))
            {
                targetExtension = InlineExtension;
            }
            else if (string.Equals(extension, InlineExtension, StringComparison.OrdinalIgnoreCase))
            {
                targetExtension = HeaderExtension;
            }
            else
            {
                ShowMessage($"当前文件不是 .h 或 .inl 文件：\n{currentPath}");
                return;
            }

            string directory = Path.GetDirectoryName(currentPath) ?? string.Empty;
            if (!Directory.Exists(directory))
            {
                ShowMessage($"当前文档尚未保存到磁盘，无法查找同名文件：\n{currentPath}");
                return;
            }

            string targetPath = Path.Combine(directory, Path.GetFileNameWithoutExtension(currentPath) + targetExtension);

            if (!File.Exists(targetPath))
            {
                ShowMessage($"未找到同名文件：\n{targetPath}");
                return;
            }

            dte.ItemOperations.OpenFile(targetPath, EnvDTE.Constants.vsViewKindTextView);
        }

        private void ShowMessage(string message)
        {
            ThreadHelper.ThrowIfNotOnUIThread();

            VsShellUtilities.ShowMessageBox(
                this.package,
                message,
                MessageBoxTitle,
                OLEMSGICON.OLEMSGICON_INFO,
                OLEMSGBUTTON.OLEMSGBUTTON_OK,
                OLEMSGDEFBUTTON.OLEMSGDEFBUTTON_FIRST);
        }
    }
}
