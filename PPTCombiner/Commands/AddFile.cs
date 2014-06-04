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
        private readonly ObservableCollection<string> addedPaths;
        private readonly BehaviorSubject<string> appSelection;

        public AddFile(ObservableCollection<string> addedPaths, BehaviorSubject<string> appSelection)
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
                    addedPaths.Add(dialog.FileName);
                    appSelection.OnNext(dialog.FileName);
                }
            }
        }
    }
}
