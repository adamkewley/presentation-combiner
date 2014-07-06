using PresentationCombiner.FS;
using System;
using System.Collections.ObjectModel;
using System.IO;
using System.Windows.Forms;
using System.Windows.Input;

namespace PresentationCombiner.Commands
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
            // if a path was passed as a parameter
            if(parameter is string)
            {
                this.addDirectory(parameter as string);
            }
            else
            {
                var dialog = new FolderBrowserDialog();
                DialogResult result = dialog.ShowDialog();

                if (result == DialogResult.OK)
                {
                    this.addDirectory(dialog.SelectedPath);
                }
            }
        }

        private void addDirectory(string directoryPath)
        {
            if(Directory.Exists(directoryPath))
            {
                AddedPath path = PathHelpers.PathToAddedPath(directoryPath);
                ReversibleCommand command =
                    PresentationCombiner.FS.Commands.CreateReversibleCommand(
                        () =>
                        {
                            addedPaths.Add(path);
                            appSelection.Add(path);
                        },
                        () =>
                        {
                            addedPaths.Remove(path);
                            appSelection.Remove(path);
                        });

                commandInvoker.InvokeCommand(command);
            }
        }
    }
}
