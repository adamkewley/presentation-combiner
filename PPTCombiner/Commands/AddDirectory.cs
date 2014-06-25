using PPTCombiner.FS;
using System;
using System.Collections.ObjectModel;
using System.IO;
using System.Reactive.Subjects;
using System.Windows.Forms;
using System.Windows.Input;

namespace PPTCombiner.Commands
{
    class AddDirectory : ICommand
    {
        //TODO: Change to PPTCombiner.FS.AddedPath
        private readonly ObservableCollection<AddedPath> addedPaths;
        private readonly BehaviorSubject<AddedPath> appSelection;

        public AddDirectory(ObservableCollection<AddedPath> addedPaths, BehaviorSubject<AddedPath> appSelection)
        {
            this.addedPaths = addedPaths;
            this.appSelection = appSelection;
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
                    var selectedPath = PathHelpers.FindValidFilesInPath(dialog.SelectedPath);
                    addedPaths.Add(selectedPath);
                    appSelection.OnNext(selectedPath);
                }
            }
        }
    }
}
