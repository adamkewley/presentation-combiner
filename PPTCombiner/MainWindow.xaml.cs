using PPTCombiner.FS;
using System;
using System.Windows;

namespace PPTCombiner
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private readonly MainWindowViewModel viewModel;

        private bool validDataDraggedIn = false;
        private string draggedPath = null;

        public MainWindow()
        {
            this.viewModel = new MainWindowViewModel();
            this.DataContext = this.viewModel;
            InitializeComponent();
            this.viewModel.Selection.Subscribe(viewModelSelectionChanged);
        }

        private void ListBox_Drop(object sender, DragEventArgs e)
        {
            var addedPath = PathHelpers.FindValidFilesInPath(draggedPath);
            viewModel.Paths.Add(addedPath);
            viewModel.Selection.OnNext(addedPath);
            e.Handled = true;
        }

        private void ListBox_DragEnter(object sender, DragEventArgs e)
        {
            Array data = e.Data.GetData("FileName") as Array;
            if (data != null)
            {
                if ((data.Length == 1) && (data.GetValue(0) is String))
                {
                    string fileName = ((string[])data)[0];
                    e.Effects = DragDropEffects.Copy;
                    this.validDataDraggedIn = true;
                    this.draggedPath = fileName;
                }
                else
                {
                    // Wrong data type in Clipboard.
                    e.Effects = DragDropEffects.None;
                    this.validDataDraggedIn = false;
                    this.draggedPath = null;
                }
            }
            else
            {
                // Wrong data type in clipboard.
                e.Effects = DragDropEffects.None;
                this.validDataDraggedIn = false;
                this.draggedPath = null;
            }

            e.Handled = true;
        }

        private void ListBox_DragOver(object sender, DragEventArgs e)
        {            
            e.Effects = validDataDraggedIn ? DragDropEffects.Copy : DragDropEffects.None;
            e.Handled = true;
        }

        /// <summary>
        /// Occurs when the user selects something in the UI.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void AddedPathsList_SelectionChanged(object sender, System.Windows.Controls.SelectionChangedEventArgs e)
        {
            AddedPathView newSelection = this.AddedPathsList.SelectedItem as AddedPathView;
            var vmSelectionSubject = this.viewModel.Selection;

            if(newSelection == null)
            {
                if (vmSelectionSubject.Value == null) return;
                else vmSelectionSubject.OnNext(null);
            }
            else
            {
                if (newSelection.AddedPath == vmSelectionSubject.Value) return;
                else vmSelectionSubject.OnNext(newSelection.AddedPath);
            }

            e.Handled = true;
        }

        /// <summary>
        /// Occurs when the viewmodel changes the selection.
        /// </summary>
        /// <param name="newSelection"></param>
        private void viewModelSelectionChanged(AddedPath newSelection)
        {
            AddedPathView uiSelection = this.AddedPathsList.SelectedItem as AddedPathView;

            if (uiSelection == null)
            {
                if (newSelection == null) return;
                else this.AddedPathsList.SelectedItem = newSelection.AddedPathtoAddedPathView();
            }
            else
            {
                if (uiSelection.AddedPath == newSelection) return;
                else if(newSelection == null)
                {
                    this.AddedPathsList.SelectedIndex = -1;
                }
                else
                {
                    this.AddedPathsList.SelectedItem = newSelection.AddedPathtoAddedPathView();
                }
            }
        }
    }
}
