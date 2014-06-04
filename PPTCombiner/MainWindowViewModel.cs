using PPTCombiner.Commands;
using PPTCombiner.FS;
using ReactiveUI;
using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Reactive.Linq;
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
            this.selectionProperty =
                new ObservableAsPropertyHelper<string>(selection,
                                                       _ => PropertyChanged.Raise(this, "Selection"));

            this.buttonTextObservable = FHelpers.PathListToButtonText(Paths);
            this.buttonTextProperty =
                new ObservableAsPropertyHelper<string>(buttonTextObservable, 
                                                       _ => PropertyChanged.Raise(this, "ButtonText"), 
                                                       initialValue: "Nothing to merge.");

            this.AddDirectory = new AddDirectory(Paths, selection);
            this.AddFile = new AddFile(Paths, selection);
            this.PerformMerge = new PerformMerge(Paths);
            this.RemoveSelected = new RemoveSelected(Paths, selection);

        }

        public ObservableCollection<string> Paths { get; private set; }

        private readonly BehaviorSubject<string> selection;
        private readonly ObservableAsPropertyHelper<string> selectionProperty;
        public string Selection
        {
            get { return selection.Value; }
            set { selection.OnNext(value); }
        }

        private readonly IObservable<string> buttonTextObservable;
        private readonly ObservableAsPropertyHelper<string> buttonTextProperty;
        public string ButtonText
        {
            get { return buttonTextProperty.Value; }
        }

        public ICommand AddDirectory { get; private set; }
        public ICommand AddFile { get; private set; }
        public ICommand RemoveSelected { get; private set; }
        public ICommand PerformMerge { get; private set; }        

        public event PropertyChangedEventHandler PropertyChanged;
    }
}
