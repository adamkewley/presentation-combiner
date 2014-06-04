using System;
using System.Collections.ObjectModel;
using System.Reactive.Subjects;
using System.Windows.Input;

namespace PPTCombiner.Commands
{
    class RemoveSelected : ICommand
    {
        private readonly ObservableCollection<string> addedPaths;
        private readonly BehaviorSubject<string> selectedPath;

        public RemoveSelected(ObservableCollection<string> addedPaths, BehaviorSubject<string> selectedPath)
        {
            this.addedPaths = addedPaths;
            this.selectedPath = selectedPath;
            selectedPath.Subscribe(_ => CanExecuteChanged.Raise(this, EventArgs.Empty));
        }

        public bool CanExecute(object parameter)
        {
            return !String.IsNullOrWhiteSpace(selectedPath.Value);
        }

        public event EventHandler CanExecuteChanged;

        public void Execute(object parameter)
        {
            addedPaths.Remove(selectedPath.Value);
            selectedPath.OnNext("");
        }
    }
}
