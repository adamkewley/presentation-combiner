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
        private readonly ObservableCollection<string> addedPaths;
        private readonly BehaviorSubject<string> appSelection;

        public AddDirectory(ObservableCollection<string> addedPaths, BehaviorSubject<string> appSelection)
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
                    addedPaths.Add(dialog.SelectedPath);
                    appSelection.OnNext(dialog.SelectedPath);
                }
            }
        }
    }
}
