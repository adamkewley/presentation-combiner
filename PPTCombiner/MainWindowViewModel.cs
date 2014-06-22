using PPTCombiner.Commands;
using PPTCombiner.FS;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Reactive.Linq;
using System.Reactive.Subjects;
using System.Windows;
using System.Windows.Input;

namespace PPTCombiner
{
    sealed class MainWindowViewModel : INotifyPropertyChanged, IDisposable
    {
        private readonly List<IDisposable> subscriptions;

        public MainWindowViewModel()
        {
            this.subscriptions = new List<IDisposable>();

            this.Paths = new ObservableCollection<string>();

            this.selection = new BehaviorSubject<string>("");
            this.subscriptions.Add(this.selection.Subscribe(_ =>
                this.PropertyChanged.Raise(this, "Selection")));

            this.buttonText = FHelpers.PathListToButtonText(this.Paths);
            this.subscriptions.Add(this.buttonText.Subscribe(_ =>
                PropertyChanged.Raise(this, "ButtonText")));

            this.showFileList = new BehaviorSubject<Visibility>(Visibility.Hidden);
            this.subscriptions.Add(
                Observable.FromEventPattern<NotifyCollectionChangedEventHandler, NotifyCollectionChangedEventArgs>(
                    ev => Paths.CollectionChanged += ev,
                    ev => Paths.CollectionChanged -= ev)
                .Select(_ => Paths.Count > 0 ? Visibility.Visible : Visibility.Hidden)
                .Subscribe(showFileList));
            this.subscriptions.Add(this.showFileList.Subscribe(_ =>
                PropertyChanged.Raise(this, "ShowFileList")));

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

        public void Dispose()
        {
            foreach(IDisposable subscription in subscriptions)
            {
                subscription.Dispose();
            }
        }
    }
}
