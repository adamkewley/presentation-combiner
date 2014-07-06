using PresentationCombiner.FS;
using System;
using System.Collections.ObjectModel;
using System.IO;
using System.Windows.Forms;
using System.Windows.Input;

namespace PresentationCombiner.Commands
{
    internal sealed class AddFile : ICommand
    {
        private readonly ObservableCollection<AddedPath> addedPaths;
        private readonly ObservableCollection<AddedPath> appSelection;
        private readonly ICommandInvoker commandInvoker;

        public AddFile(ObservableCollection<AddedPath> addedPaths, ObservableCollection<AddedPath> appSelection, ICommandInvoker commandInvoker)
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
            if(parameter is string)
            {
                this.addFile(parameter as string);
            }
            else
            {
                FileDialog dialog = new OpenFileDialog();
                dialog.Filter = FHelpers.DialogPaths;
                DialogResult result = dialog.ShowDialog();

                if(result == DialogResult.OK)
                {
                    this.addFile(dialog.FileName);
                }
            }
        }

        private void addFile(string filePath)
        {
            if(File.Exists(filePath))
            {
                AddedPath path = PathHelpers.PathToAddedPath(filePath);
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
