using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows;
using System.Windows.Input;

namespace Hive.IO.Forms
{
    /// <summary>
    ///     I'm using this link:
    ///     https://www.codeproject.com/Questions/1255014/Wpf-MVVM-dynamic-binding-of-usercontrols-and-show
    ///     as a basis for setting up the MVVM design pattern.
    /// </summary>
    public abstract class ObservableBase : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        public void Set<TValue>(ref TValue field,
            TValue newValue,
            [CallerMemberName] string propertyName = "")
        {
            if (EqualityComparer<TValue>.Default.Equals(field, default)
                || !field.Equals(newValue))
            {
                field = newValue;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
            }
        }

        public virtual void RaisePropertyChangedEvent([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }

    public abstract class ViewModelBase : ObservableBase
    {
        public bool IsInDesignMode()
        {
            return Application.Current.MainWindow == null ||
                   DesignerProperties.GetIsInDesignMode(Application.Current.MainWindow);
        }
    }

    public class RelayCommand<T> : ICommand
    {
        private readonly Predicate<T> canExecute;
        private readonly Action<T> execute;

        public RelayCommand(Action<T> execute, Predicate<T> canExecute = null)
        {
            this.execute = execute ?? throw new ArgumentNullException("execute");
            this.canExecute = canExecute;
        }

        public bool CanExecute(object parameter)
        {
            return canExecute == null || canExecute((T) parameter);
        }

        public event EventHandler CanExecuteChanged
        {
            add => CommandManager.RequerySuggested += value;
            remove => CommandManager.RequerySuggested -= value;
        }

        public void Execute(object parameter)
        {
            execute(parameter == null
                ? default
                : (T) Convert.ChangeType(parameter, typeof(T)));
        }
    }
}