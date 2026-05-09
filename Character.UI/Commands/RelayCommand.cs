using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace Character.UI.Commands
{
    public class RelayCommand : ICommand
    {
        private readonly Func<object?, Task>? _executeAsync;
        private readonly Action<object?>? _execute;
        private readonly Func<object?, bool>? _canExecute;

        private bool _isExecuting;

        public RelayCommand(Func<object?, Task> executeAsync, Func<object?, bool>? canExecute = null)
        {
            _executeAsync = executeAsync;
            _canExecute = canExecute;
        }
        public RelayCommand(Action<object?> execute, Func<object?, bool>? canExecute = null)
        {
            _execute = execute;
            _canExecute = canExecute;
        }
        public RelayCommand(Action execute, Func<bool>? canExecute = null)
        {
            _execute = _ => execute();

            if (canExecute != null) _canExecute = _ => canExecute();
        }
        public RelayCommand(Func<Task> executeAsync, Func<bool>? canExecute = null)
        {
            _executeAsync = _ => executeAsync();

            if (canExecute != null) _canExecute = _ => canExecute();
        }

        public bool CanExecute(object? parameter)
        {
            if (_isExecuting) return false;

            return _canExecute?.Invoke(parameter) ?? true;
        }

        public async void Execute(object? parameter)
        {
            if (_executeAsync != null)
            {
                try
                {
                    _isExecuting = true;
                    RaiseCanExecuteChanged();

                    await _executeAsync(parameter);
                }
                catch (Exception ex)
                {
                    // TODO: log hoặc handle global
                    Debug.WriteLine($"Error executing command: {ex}");
                }
                finally
                {
                    _isExecuting = false;
                    RaiseCanExecuteChanged();
                }
            }
            else
            {
                _execute?.Invoke(parameter);
            }
        }

        public event EventHandler? CanExecuteChanged;

        public void RaiseCanExecuteChanged()
            => CanExecuteChanged?.Invoke(this, EventArgs.Empty);
    }
    public class RelayCommand<T> : ICommand
    {
        private readonly Func<T?, Task>? _executeAsync;
        private readonly Action<T?>? _execute;
        private readonly Func<T?, bool>? _canExecute;
        private bool _isExecuting;

        public RelayCommand(Action<T?> execute, Func<T?, bool>? canExecute = null)
        {
            _execute = execute;
            _canExecute = canExecute;
        }
        public RelayCommand(Func<T?, Task> executeAsync, Func<T?, bool>? canExecute = null)
        {
            _executeAsync = executeAsync;
            _canExecute = canExecute;
        }
        public bool CanExecute(object? parameter)
        {
            if (_isExecuting) return false;
            return _canExecute?.Invoke(parameter is T t ? t : default) ?? true;
        }
        public async void Execute(object? parameter)
        {
            var arg = parameter is T t ? t : default;

            if (_executeAsync != null)
            {
                try
                {
                    _isExecuting = true;
                    RaiseCanExecuteChanged();
                    await _executeAsync(arg);
                }
                catch (Exception ex)
                {
                    Debug.WriteLine($"Error executing command: {ex}");
                }
                finally
                {
                    _isExecuting = false;
                    RaiseCanExecuteChanged();
                }
            }
            else
            {
                _execute?.Invoke(arg);
            }
        }
        public event EventHandler? CanExecuteChanged;
        public void RaiseCanExecuteChanged()
            => CanExecuteChanged?.Invoke(this, EventArgs.Empty);
    }
}
