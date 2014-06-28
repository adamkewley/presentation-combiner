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
            this.Paths = new ObservableCollection<AddedPath>();
            this.PathsView = this.Paths.DynamicMap(pathModel => pathModel.AddedPathtoAddedPathView());

            // Current selection.
            this.Selection = new BehaviorSubject<AddedPath>(null);

            // Merge button text.
            FHelpers.PathListToButtonText(this.Paths)
                .Subscribe(newButtonText =>
                {
                    this.ButtonText = newButtonText;
                    PropertyChanged.Raise(this, "ButtonText");
                });

            // File list visibility
            this.showFileList = new BehaviorSubject<Visibility>(Visibility.Hidden);

            PathsView.CollectionChanged += (s, e) =>
            {
                var newVisibility = PathsView.Count > 0 ? Visibility.Visible : Visibility.Hidden;
                this.showFileList.OnNext(newVisibility);
            };

            this.showFileList.Subscribe(_ =>
                PropertyChanged.Raise(this, "ShowFileList"));

            // Commands
            this.AddDirectory = new AddDirectory(this.Paths, this.Selection);
            this.AddFile = new AddFile(this.Paths, this.Selection);
            this.PerformMerge = new PerformMerge(this.Paths);
            this.RemoveSelected = new RemoveSelected(this.Paths, this.Selection);
        }


        public ObservableCollection<AddedPath> Paths { get; private set; }
        public ObservableCollection<AddedPathView> PathsView { get; private set; }
        public BehaviorSubject<AddedPath> Selection { get; private set; }

        public string ButtonText { get; private set; }

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
