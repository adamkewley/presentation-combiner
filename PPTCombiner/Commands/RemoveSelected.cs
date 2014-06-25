using PPTCombiner.FS;
using System;
using System.Collections.ObjectModel;
using System.Reactive.Subjects;
using System.Windows.Input;

namespace PPTCombiner.Commands
{
    class RemoveSelected : ICommand
    {
        //TODO: Change to PPTCombiner.FS.AddedPath
        private readonly ObservableCollection<AddedPath> addedPaths;
        private readonly BehaviorSubject<AddedPath> selectedPath;

        public RemoveSelected(ObservableCollection<AddedPath> addedPaths, BehaviorSubject<AddedPath> selectedPath)
        {
            this.addedPaths = addedPaths;
            this.selectedPath = selectedPath;
            selectedPath.Subscribe(_ => CanExecuteChanged.Raise(this, EventArgs.Empty));
        }

        public bool CanExecute(object parameter)
        {
            return selectedPath.Value != null;
        }

        public event EventHandler CanExecuteChanged;

        public void Execute(object parameter)
        {
            addedPaths.Remove(selectedPath.Value);
            selectedPath.OnNext(null);
        }
    }
}
