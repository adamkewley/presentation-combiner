using PPTCombiner.FS;
using System;
using System.Collections.ObjectModel;
using System.IO;
using System.Reactive.Subjects;
using System.Windows.Forms;
using System.Windows.Input;

namespace PPTCombiner.Commands
{
    class AddFile : ICommand
    {
        private readonly ObservableCollection<AddedPath> addedPaths;
        private readonly BehaviorSubject<AddedPath> appSelection;

        public AddFile(ObservableCollection<AddedPath> addedPaths, BehaviorSubject<AddedPath> appSelection)
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
            FileDialog dialog = new OpenFileDialog();
            DialogResult result = dialog.ShowDialog();

            if(result == DialogResult.OK)
            {
                if(File.Exists(dialog.FileName))
                {
                    var userSelection = PathHelpers.FindValidFilesInPath(dialog.FileName);
                    addedPaths.Add(userSelection);
                    appSelection.OnNext(userSelection);
                }
            }
        }
    }
}
