using PPTCombiner.Commands;
using PPTCombiner.FS;
using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Windows;
using System.Windows.Input;

namespace PPTCombiner
{
    sealed class MainWindowViewModel : INotifyPropertyChanged
    {
        private readonly ObservableCollection<AddedPath> paths = new ObservableCollection<AddedPath>();
        private readonly ObservableCollection<AddedPath> selection = new ObservableCollection<AddedPath>();

        public MainWindowViewModel()
        {
            this.PathsView = 
                this.paths.TwoWayDynamicMap(
                    pathModel => pathModel.AddedPathtoAddedPathView(),
                    pathView => pathView.AddedPath).Item1;
            
            this.SelectionView = 
                this.selection.TwoWayDynamicMap(
                    pathModel => pathModel.AddedPathtoAddedPathView(),
                    pathView => pathView.AddedPath).Item1;

            FHelpers.PathListToButtonText(this.paths)
                .Subscribe(newButtonText =>
                {
                    this.ButtonText = newButtonText;
                    PropertyChanged.Raise(this, "ButtonText");
                });

            // File list visibility
            this.ShowFileList = Visibility.Hidden;
            PathsView.CollectionChanged += (s, e) =>
            {
                var newVisibility = PathsView.Count > 0 ? Visibility.Visible : Visibility.Hidden;
                this.ShowFileList = newVisibility;
                PropertyChanged.Raise(this, "ShowFileList");
            };

            // Commands.
            this.AddDirectory = new AddDirectory(this.paths, this.selection);
            this.AddFile = new AddFile(this.paths, this.selection);
            this.PerformMerge = new PerformMerge(this.paths);
            this.RemoveSelected = new RemoveSelected(this.paths, this.selection);
        }

        public ObservableCollection<AddedPathView> PathsView { get; private set; }
        public ObservableCollection<AddedPathView> SelectionView { get; private set; }

        public string ButtonText { get; private set; }

        public Visibility ShowFileList { get; private set; }

        public ICommand AddDirectory { get; private set; }
        public ICommand AddFile { get; private set; }
        public ICommand RemoveSelected { get; private set; }
        public ICommand PerformMerge { get; private set; }

        public event PropertyChangedEventHandler PropertyChanged;
    }
}
