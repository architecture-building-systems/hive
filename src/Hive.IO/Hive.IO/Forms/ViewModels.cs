using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows;

namespace Hive.IO.Forms
{
    /// <summary>
    /// I'm using this link: https://www.codeproject.com/Questions/1255014/Wpf-MVVM-dynamic-binding-of-usercontrols-and-show
    /// as a basis for setting up the MVVM design pattern. 
    /// </summary>
    public abstract class ObservableBase : INotifyPropertyChanged
    {
        public void Set<TValue>(ref TValue field,
            TValue newValue,
            [CallerMemberName] string propertyName = "")
        {
            if (EqualityComparer<TValue>.Default.Equals(field, default(TValue))
                || !field.Equals(newValue))
            {
                field = newValue;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;
    }

    public abstract class ViewModelBase : ObservableBase
    {
        public bool IsInDesignMode()
        {
            return Application.Current.MainWindow == null || DesignerProperties.GetIsInDesignMode(Application.Current.MainWindow);
        }
    }
}
