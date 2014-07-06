using PresentationCombiner.FS;
using System;
using System.Windows.Input;

namespace PresentationCombiner.Commands
{
    internal sealed class Redo : ICommand
    {
        private readonly ICommandController commandController;

        public Redo(ICommandController commandController)
        {
            this.commandController = commandController;

            this.commandController.CanRedoChanged.Subscribe(_ =>
                this.CanExecuteChanged.Raise(this, EventArgs.Empty));
        }

        public bool CanExecute(object parameter)
        {
            return this.commandController.CanRedo();
        }

        public event EventHandler CanExecuteChanged;

        public void Execute(object parameter)
        {
            this.commandController.Redo();
        }
    }
}
