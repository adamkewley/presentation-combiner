using PPTCombiner.FS;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Windows.Input;

namespace PPTCombiner.Commands
{
    internal sealed class RemoveSelected : ICommand
    {
        private readonly ObservableCollection<AddedPath> addedPaths;
        private readonly ObservableCollection<AddedPath> selectedPaths;
        private readonly ICommandInvoker commandInvoker;

        public RemoveSelected(ObservableCollection<AddedPath> addedPaths, ObservableCollection<AddedPath> selectedPaths, ICommandInvoker commandInvoker)
        {
            this.addedPaths = addedPaths;
            this.selectedPaths = selectedPaths;
            this.selectedPaths.CollectionChanged += (s, e) => CanExecuteChanged.Raise(this, EventArgs.Empty);
            this.commandInvoker = commandInvoker;
        }

        public bool CanExecute(object parameter)
        {
            return selectedPaths.Count > 0;
        }

        public event EventHandler CanExecuteChanged;

        public void Execute(object parameter)
        {
            var selectedBuffer = new List<AddedPath>(selectedPaths);

            var command = PPTCombiner.FS.Commands.CreateReversibleCommand(
                    () =>
                    {
                        foreach (AddedPath selectedPath in selectedBuffer)
                        {
                            this.selectedPaths.Remove(selectedPath);
                            this.addedPaths.Remove(selectedPath);
                        }
                    },
                    () =>
                    {
                        foreach (AddedPath selectedPath in selectedBuffer)
                        {
                            this.addedPaths.Add(selectedPath);
                            this.selectedPaths.Add(selectedPath);
                        }
                    });

            this.commandInvoker.InvokeCommand(command);
        }
    }
}
