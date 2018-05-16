// Copyright (c) Kris Penner. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.


using System;
using System.Collections;
using System.ComponentModel;
using System.Linq;
using System.Reflection;
using KodeAid.Linq;

namespace KodeAid.ComponentModel
{
    public abstract class DataErrorNotificationObject : ChangeNotificationObject, INotifyDataErrorInfo, IDataErrorInfo
    {
        private const string _errorPropertyName = "Error";
        private const string _hasErrorsPropertyName = "HasErrors";
        private string _error;

        private event EventHandler<DataErrorsChangedEventArgs> ErrorsChanged;

        protected bool ValidateObject()
        {
            foreach (var propertyName in GetType().GetRuntimeProperties().Select(p => p.Name))
            {
                var error = ValidateProperty(propertyName);
                if (error != null)
                {
                    SetObjectError(error, propertyName);
                    return false;
                }
            }
            SetObjectError(null, null);
            return true;
        }

        protected virtual string ValidateProperty(string propertyName)
        {
            return null;
        }

        protected virtual void OnErrorsChanged(DataErrorsChangedEventArgs e)
        {
            ErrorsChanged?.Invoke(this, e);
        }

        private void SetObjectError(string error, string propertyName = null)
        {
            if (ChangeProperty(_errorPropertyName, ref _error, error, _hasErrorsPropertyName) && propertyName != null)
                OnErrorsChanged(new DataErrorsChangedEventArgs(propertyName));
        }

        bool INotifyDataErrorInfo.HasErrors
        {
            get { return !string.IsNullOrEmpty(_error); }
        }

        IEnumerable INotifyDataErrorInfo.GetErrors(string propertyName)
        {
            return EnumerableHelper.From(ValidateProperty(propertyName));
        }

        event EventHandler<DataErrorsChangedEventArgs> INotifyDataErrorInfo.ErrorsChanged
        {
            add { ErrorsChanged += value; }
            remove { ErrorsChanged -= value; }
        }

        string IDataErrorInfo.Error
        {
            get { return _error; }
        }

        string IDataErrorInfo.this[string columnName]
        {
            get { return ValidateProperty(columnName) ?? string.Empty; }
        }
    }
}
