using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Text.RegularExpressions;

namespace AddressUtility
{
    public class Validating : INotifyDataErrorInfo
    {
        private readonly Dictionary<string, List<string>> _errors = new();

        public event EventHandler<DataErrorsChangedEventArgs> ErrorsChanged;

        public bool HasErrors
            => _errors.Count > 0;

        protected void OnErrorsChanged([CallerMemberName] string propertyName = "")
            => ErrorsChanged?.Invoke(this, new(propertyName));

        public IEnumerable GetErrors(string propertyName)
        {
            if (_errors.ContainsKey(propertyName))
                return _errors[propertyName];
            return null;
        }
        public void ValidateName(string name, [CallerMemberName] string propertyName = "")
        {
            List<string> propertyErrors = new();

            if (string.IsNullOrEmpty(name))
                propertyErrors.Add("Название не может быть пустым");

            if (name.Length > 100)
                propertyErrors.Add("Название должно быть меньше 100 символов");

            if (name.StartsWith(' '))
                propertyErrors.Add("Название не должно начинаться с пробела");

            if (Regex.IsMatch(name, "[A-Za-z]"))
                propertyErrors.Add("Название не должно содержать латинские буквы");

            if (propertyErrors.Count > 0)
            {
                if (_errors.ContainsKey(propertyName))
                    _errors[propertyName] = propertyErrors;
                else
                    _errors.Add(propertyName, propertyErrors);

                return;
            }

            _errors.Remove(propertyName);
        }
    }
}
