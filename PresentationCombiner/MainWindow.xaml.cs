using PresentationCombiner.FS;
using System;
using System.IO;
using System.Windows;

namespace PresentationCombiner
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private readonly MainWindowViewModel viewModel = new MainWindowViewModel();

        // Drag & drop watchers.
        private bool validDataDraggedIn = false;
        private string draggedPath = null;

        // Circular selection updating lock.
        private bool selectionLock = false;

        public MainWindow()
        {
            this.DataContext = this.viewModel;
            InitializeComponent();

            // Monitor changes in the selection from the viewmodel.
            this.viewModel.Selection.CollectionChanged += (s, e) =>
            {
                if (selectionLock) return;

                selectionLock = true;
                OcExtensions.MapChangesToList(
                    (AddedPathView x) => x, e, this.AddedPathsList.SelectedItems);
                selectionLock = false;

                this.AddedPathsList.Focus();
            };
        }

        #region Drag & Drop

        private void ListBox_Drop(object sender, DragEventArgs e)
        {            
            if(Directory.Exists(draggedPath))
            {
                this.viewModel.AddDirectory.Execute(draggedPath);
            }
            else if(File.Exists(draggedPath))
            {
                this.viewModel.AddFile.Execute(draggedPath);
            }

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

        #endregion

        #region Selection Logic

        /// <summary>
        /// Occurs when the user selects something in the UI.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void AddedPathsList_SelectionChanged(object sender, System.Windows.Controls.SelectionChangedEventArgs e)
        {
            // Prevent circular updating of selection collection.
            if (selectionLock) return;

            selectionLock = true;
            foreach(AddedPathView addedPath in e.AddedItems)
            {
                this.viewModel.Selection.Add(addedPath);
            }

            foreach(AddedPathView removedPath in e.RemovedItems)
            {
                this.viewModel.Selection.Remove(removedPath);
            }
            selectionLock = false;
        }

        #endregion
    }
}
