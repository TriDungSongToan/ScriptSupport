using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace Character.UI.ViewModels
{
    public class BaseViewModel : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler? PropertyChanged;
        private bool _suppressNotify = false;
        private HashSet<string> _changedProperties = new();

        protected bool SetProperty<T>(ref T storage, T value, [CallerMemberName] string propertyName = "")
        {
            if (EqualityComparer<T>.Default.Equals(storage, value))
                return false;

            storage = value;

            if (_suppressNotify)
                _changedProperties.Add(propertyName);
            else
                OnPropertyChanged(propertyName);

            return true;
        }
        protected void OnPropertyChanged([CallerMemberName] string propertyName = "")
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        public void BeginUpdate()
        {
            _suppressNotify = true;
            _changedProperties.Clear();
        }
        public void EndUpdate()
        {
            _suppressNotify = false;
            foreach (var prop in _changedProperties)
                OnPropertyChanged(prop);
            _changedProperties.Clear();
        }
    }
}

