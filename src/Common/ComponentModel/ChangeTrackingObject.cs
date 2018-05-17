// Copyright (c) Kris Penner. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.


using System;
using System.ComponentModel;

namespace KodeAid.ComponentModel
{
    public abstract class ChangeTrackingObject : EditableObjectBase, IChangeTracking, IRevertibleChangeTracking
    {
        private bool _ignoreChanges = false;
        private bool _isChanged = false;

        protected ChangeTrackingObject()
        {
            BeginEdit();
        }

        public bool IsChanged
        {
            get { return _isChanged; }
            private set { ChangeProperty(nameof(IsChanged), ref _isChanged, value); }
        }

        public void AcceptChanges()
        {
            CheckOnlyOnePendingEdit();
            _ignoreChanges = true;
            EndEdit();
            BeginEdit();
            _ignoreChanges = false;
            IsChanged = false;
        }

        public void RejectChanges()
        {
            CheckOnlyOnePendingEdit();
            _ignoreChanges = true;
            CancelEdit();
            BeginEdit();
            _ignoreChanges = false;
            IsChanged = false;
        }

        protected override void ChangeTrackingOverride(string propertyName, object oldValue, object newValue)
        {
            base.ChangeTrackingOverride(propertyName, oldValue, newValue);
            if (!_ignoreChanges)
            {
                CheckOnlyOnePendingEdit();
                IsChanged = base.HasChangesInPendingEdit;
            }
        }

        private void CheckOnlyOnePendingEdit()
        {
            if (PendingEditCount != 1)
                throw new InvalidOperationException("Only a single pending edit transaction scope is supported.");
        }
    }
}
