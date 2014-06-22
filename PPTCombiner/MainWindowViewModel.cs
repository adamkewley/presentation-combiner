using PPTCombiner.Commands;
using PPTCombiner.FS;
using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Reactive.Subjects;
using System.Windows;
using System.Windows.Input;

namespace PPTCombiner
{
    sealed class MainWindowViewModel : INotifyPropertyChanged
    {
        public MainWindowViewModel()
        {
            this.Paths = new ObservableCollection<string>();

            // Current selection.
            this.selection = new BehaviorSubject<string>("");
            this.selection.Subscribe(_ =>
                this.PropertyChanged.Raise(this, "Selection"));

            // Merge button text.
            this.buttonText = FHelpers.PathListToButtonText(this.Paths);
            this.buttonText.Subscribe(_ =>
                PropertyChanged.Raise(this, "ButtonText"));

            // File list visibility
            this.showFileList = new BehaviorSubject<Visibility>(Visibility.Hidden);

            Paths.CollectionChanged += (s, e) =>
            {
                var newVisibility = Paths.Count > 0 ? Visibility.Visible : Visibility.Hidden;
                this.showFileList.OnNext(newVisibility);
            };

            this.showFileList.Subscribe(_ =>
                PropertyChanged.Raise(this, "ShowFileList"));

            // Commands
            this.AddDirectory = new AddDirectory(Paths, selection);
            this.AddFile = new AddFile(Paths, selection);
            this.PerformMerge = new PerformMerge(Paths);
            this.RemoveSelected = new RemoveSelected(Paths, selection);
        }

        public ObservableCollection<string> Paths { get; private set; }

        private readonly BehaviorSubject<string> selection;
        public string Selection
        {
            get { return selection.Value; }
            set { selection.OnNext(value); }
        }

        private readonly BehaviorSubject<string> buttonText;
        public string ButtonText
        {
            get { return buttonText.Value; }
        }

        private readonly BehaviorSubject<Visibility> showFileList;
        public Visibility ShowFileList
        {
            get { return showFileList.Value; }
        }

        public ICommand AddDirectory { get; private set; }
        public ICommand AddFile { get; private set; }
        public ICommand RemoveSelected { get; private set; }
        public ICommand PerformMerge { get; private set; }        

        public event PropertyChangedEventHandler PropertyChanged;
    }
}
