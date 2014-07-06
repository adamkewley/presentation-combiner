using PresentationCombiner.Commands;
using PresentationCombiner.FS;
using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Windows;
using System.Windows.Input;

namespace PresentationCombiner
{
    sealed class MainWindowViewModel : INotifyPropertyChanged
    {
        public MainWindowViewModel()
        {
            // Underlying models (pointless to DI these for such a small application).
            var paths = new ObservableCollection<AddedPath>();
            var selection = new ObservableCollection<AddedPath>();
            var commandController = new CommandController();

            // Wireup models to commands and views. Ignoring IDisposible cleanup due to app
            // size.
            this.Paths = 
                paths.TwoWayDynamicMap(
                    pathModel => pathModel.AddedPathtoAddedPathView(),
                    pathView => pathView.AddedPath).Item1;
            
            this.Selection = 
                selection.TwoWayDynamicMap(
                    pathModel => pathModel.AddedPathtoAddedPathView(),
                    pathView => pathView.AddedPath).Item1;

            FHelpers.PathListToButtonText(paths)
                .Subscribe(newButtonText => this.ButtonText = newButtonText);

            Paths.CollectionChanged += (s, e) =>
            {
                this.ShowHelpOverlay = Paths.Count == 0 ? Visibility.Visible : Visibility.Hidden;
            };

            // Commands Model.
            this.Undo = new Undo(commandController);
            this.Redo = new Redo(commandController);
            this.AddDirectory = new AddDirectory(paths, selection, commandController);
            this.AddFile = new AddFile(paths, selection, commandController);
            this.PerformMerge = new PerformMerge(paths);
            this.RemoveSelected = new RemoveSelected(paths, selection, commandController);
        }

        public ObservableCollection<AddedPathView> Paths { get; private set; }
        public ObservableCollection<AddedPathView> Selection { get; private set; }

        private string buttonText = "";
        public string ButtonText
        {
            get { return this.buttonText; }
            set
            {
                this.buttonText = value;
                PropertyChanged.Raise(this, "ButtonText");
            }
        }

        private Visibility showHelpOverlay = Visibility.Visible;
        public Visibility ShowHelpOverlay
        {
            get { return this.showHelpOverlay; }
            set
            {
                this.showHelpOverlay = value;
                PropertyChanged.Raise(this, "ShowHelpOverlay");
            }
        }

        public ICommand Undo { get; private set; }
        public ICommand Redo { get; private set; }
        public ICommand AddDirectory { get; private set; }
        public ICommand AddFile { get; private set; }
        public ICommand RemoveSelected { get; private set; }
        public ICommand PerformMerge { get; private set; }

        public event PropertyChangedEventHandler PropertyChanged;
    }
}
