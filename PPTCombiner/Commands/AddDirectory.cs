using PPTCombiner.FS;
using System;
using System.Collections.ObjectModel;
using System.IO;
using System.Windows.Forms;
using System.Windows.Input;

namespace PPTCombiner.Commands
{
    internal sealed class AddDirectory : ICommand
    {
        private readonly ObservableCollection<AddedPath> addedPaths;
        private readonly ObservableCollection<AddedPath> appSelection;
        private readonly ICommandInvoker commandInvoker;

        public AddDirectory(ObservableCollection<AddedPath> addedPaths, ObservableCollection<AddedPath> appSelection, ICommandInvoker commandInvoker)
        {
            this.addedPaths = addedPaths;
            this.appSelection = appSelection;
            this.commandInvoker = commandInvoker;
        }

        public bool CanExecute(object parameter)
        {
            return true;
        }

        public event EventHandler CanExecuteChanged;

        public void Execute(object parameter)
        {
            var dialog = new FolderBrowserDialog();
            DialogResult result = dialog.ShowDialog();

            if(result == DialogResult.OK)
            {
                if(Directory.Exists(dialog.SelectedPath))
                {
                    // ForwardCommand
                    var selectedPath = PathHelpers.PathToAddedPath(dialog.SelectedPath);

                    var command = PPTCombiner.FS.Commands.CreateReversibleCommand(
                        () =>
                        {
                            addedPaths.Add(selectedPath);
                            appSelection.Add(selectedPath);
                        },
                        () =>
                        {
                            addedPaths.Remove(selectedPath);
                            appSelection.Remove(selectedPath);
                        });

                    commandInvoker.InvokeCommand(command);
                }
            }
        }
    }
}
