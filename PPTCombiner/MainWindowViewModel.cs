using PPTCombiner.Commands;
using PPTCombiner.FS;
using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Reactive.Subjects;
using System.Windows.Input;

namespace PPTCombiner
{
    sealed class MainWindowViewModel : INotifyPropertyChanged
    {
        public MainWindowViewModel()
        {
            this.Paths = new ObservableCollection<string>();
            this.selection = new BehaviorSubject<string>("");
            this.selection.Subscribe(_ => 
                this.PropertyChanged.Raise(this, "Selection"));

            this.buttonTextObservable = FHelpers.PathListToButtonText(this.Paths);
            this.buttonTextObservable.Subscribe(_ => 
                PropertyChanged.Raise(this, "ButtonText"));

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

        private readonly BehaviorSubject<string> buttonTextObservable;
        public string ButtonText
        {
            get { return buttonTextObservable.Value; }
        }

        public ICommand AddDirectory { get; private set; }
        public ICommand AddFile { get; private set; }
        public ICommand RemoveSelected { get; private set; }
        public ICommand PerformMerge { get; private set; }        

        public event PropertyChangedEventHandler PropertyChanged;
    }
}
