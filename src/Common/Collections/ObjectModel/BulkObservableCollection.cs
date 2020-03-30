// Copyright © Kris Penner. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.


using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Linq;

namespace KodeAid.Collections.ObjectModel
{
    public class BulkObservableCollection<T> : ObservableCollection<T>
    {
        private int _bulkOperationDepth = 0;
        private bool _hasChanged = false;
        private bool _raiseResetChangedAction = false;
        private HashSet<string> _propertiesChanged = new HashSet<string>();

        public BulkObservableCollection()
        {
        }

        public BulkObservableCollection(IEnumerable<T> collection)
          : base(collection)
        {
        }

        public IDisposable BulkOperation()
        {
            BeginBulkOperation();
            return new Disposable(EndBulkOperation);
        }

        public void BeginBulkOperation()
        {
            ++_bulkOperationDepth;
            if (_bulkOperationDepth > 1)
                _raiseResetChangedAction = true;
        }

        public void EndBulkOperation()
        {
            EndBulkOperation(new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Reset));
        }

        public void AddRange(IEnumerable<T> items)
        {
            ArgCheck.NotNull(nameof(items), items);
            try
            {
                BeginBulkOperation();
                foreach (T item in items)
                    Add(item);
            }
            finally
            {
                EndBulkOperation(new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Add, items.ToList(), Count - items.Count()));
            }
        }

        protected override void OnPropertyChanged(PropertyChangedEventArgs e)
        {
            if (SuppressNotification)
                _propertiesChanged.Add(e.PropertyName);
            else
                base.OnPropertyChanged(e);
        }

        protected override void OnCollectionChanged(NotifyCollectionChangedEventArgs e)
        {
            if (SuppressNotification)
            {
                _hasChanged = true;
                if (e.Action != NotifyCollectionChangedAction.Add)
                    _raiseResetChangedAction = true;
            }
            else
                base.OnCollectionChanged(e);
        }

        private bool SuppressNotification { get { return _bulkOperationDepth > 0; } }

        private void EndBulkOperation(NotifyCollectionChangedEventArgs e)
        {
            if (_bulkOperationDepth == 0)
                throw new InvalidOperationException("Must call BeginBulkOperation() on collection before calling EndBulkOperation().");

            --_bulkOperationDepth;
            if (_bulkOperationDepth != 0)
                return;

            var hasChanged = _hasChanged;
            var raiseResetChangedAction = _raiseResetChangedAction;
            var propertiesChanged = _propertiesChanged.ToArray();
            _hasChanged = false;
            _raiseResetChangedAction = false;
            _propertiesChanged.Clear();

            if (!hasChanged)
                return;
            if (raiseResetChangedAction)
                OnCollectionChanged(new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Reset));
            else
                OnCollectionChanged(e);
            foreach (var propertyName in propertiesChanged)
                OnPropertyChanged(new PropertyChangedEventArgs(propertyName));
        }
    }
}
