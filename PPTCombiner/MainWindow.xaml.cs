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
        }

        private void ListBox_Drop(object sender, DragEventArgs e)
        {
            viewModel.Paths.Add(draggedPath);
            viewModel.Selection = draggedPath;
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
    }
}
