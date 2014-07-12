using PresentationCombiner.FS;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows.Input;

namespace PresentationCombiner.Commands
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
            this.commandInvoker = commandInvoker;

            this.selectedPaths.CollectionChanged += (s, e) => CanExecuteChanged.Raise(this, EventArgs.Empty);
        }

        public bool CanExecute(object parameter)
        {
            return selectedPaths.Count > 0;
        }

        public event EventHandler CanExecuteChanged;

        public void Execute(object parameter)
        {
            var selection = new List<Tuple<int, AddedPath>>();

            foreach(AddedPath selectedPath in selectedPaths)
            {
                int index = addedPaths.IndexOf(selectedPath);
                selection.Add(new Tuple<int, AddedPath>(index, selectedPath));
            }

            var command = PresentationCombiner.FS.Commands.CreateReversibleCommand(
                    () =>
                    {
                        foreach (AddedPath selectedPath in selection.Select(x => x.Item2))
                        {
                            this.selectedPaths.Remove(selectedPath);
                            this.addedPaths.Remove(selectedPath);
                        }
                    },
                    () =>
                    {
                        // sort according to location
                        var orderedSelection = selection.OrderBy(x => x.Item1);

                        // re-add
                        foreach (Tuple<int, AddedPath> selectedPath in orderedSelection)
                        {
                            addedPaths.Insert(selectedPath.Item1, selectedPath.Item2);
                            this.selectedPaths.Add(selectedPath.Item2);
                        }
                    });

            this.commandInvoker.InvokeCommand(command);
        }
    }
}
