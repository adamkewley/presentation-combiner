using PresentationCombiner.FS;
using System;
using System.Windows.Input;

namespace PresentationCombiner.Commands
{
    internal sealed class Undo : ICommand
    {
        private readonly ICommandController commandController;

        public Undo(ICommandController commandController)
        {
            this.commandController = commandController;

            commandController.CanUndoChanged.Subscribe(_ =>
                this.CanExecuteChanged.Raise(this, EventArgs.Empty));
        }

        public bool CanExecute(object parameter)
        {
            return this.commandController.CanUndo();
        }

        public event EventHandler CanExecuteChanged;

        public void Execute(object parameter)
        {
            this.commandController.Undo();
        }
    }
}
