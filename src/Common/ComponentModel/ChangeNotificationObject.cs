// Copyright © Kris Penner. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.


using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace KodeAid.ComponentModel
{
    public abstract class ChangeNotificationObject : INotifyPropertyChanging, INotifyPropertyChanged
    {
        public event PropertyChangingEventHandler PropertyChanging;
        public event PropertyChangedEventHandler PropertyChanged;

        protected virtual void OnPropertyChanging(PropertyChangingEventArgs e)
        {
            PropertyChanging?.Invoke(this, e);
        }

        protected virtual void OnPropertyChanged(PropertyChangedEventArgs e)
        {
            PropertyChanged?.Invoke(this, e);
        }

        protected void NotifyAllPropertiesChanging()
        {
            NotifyPropertyChanging(null);
        }

        protected void NotifyPropertiesChanging(params string[] propertyNames)
        {
            NotifyPropertiesChanging((IEnumerable<string>)propertyNames);
        }

        protected void NotifyPropertiesChanging(IEnumerable<string> propertyNames)
        {
            if (propertyNames != null)
            {
                foreach (var propertyName in propertyNames)
                {
                    NotifyPropertyChanging(propertyName);
                }
            }
        }

        protected virtual void NotifyPropertyChanging(string propertyName)
        {
            OnPropertyChanging(new PropertyChangingEventArgs(propertyName));
        }

        protected void NotifyAllPropertiesChanged()
        {
            NotifyPropertyChanged(null);
        }

        protected void NotifyPropertiesChanged(params string[] propertyNames)
        {
            NotifyPropertiesChanged((IEnumerable<string>)propertyNames);
        }

        protected void NotifyPropertiesChanged(IEnumerable<string> propertyNames)
        {
            if (propertyNames != null)
            {
                foreach (var propertyName in propertyNames)
                {
                    NotifyPropertyChanged(propertyName);
                }
            }
        }

        protected virtual void NotifyPropertyChanged(string propertyName)
        {
            OnPropertyChanged(new PropertyChangedEventArgs(propertyName));
        }

        /// <summary>
        /// Changes the specified property's backing field if they are considered unequal and raises any notifications as required.
        /// If both values are considered to be equal, then no assignement or notifications occurs.
        /// This is an overload to accomodate the most common use-case utilizing the <seealso cref="CallerMemberNameAttribute"/> for <see cref="propertyName"/>.
        /// </summary>
        /// <typeparam name="T">The type of the property.</typeparam>
        /// <param name="propertyField">The backing field of the property passed in by reference and currently assigned the old value.</param>
        /// <param name="value">The new value for the property to be assigned to the backing field <paramref name="propertyField"/> if they are considered to be unequal.</param>
        /// <param name="propertyName">The name of the property, defaults to null which will be populated by the calling member's name.</param>
        /// <returns>True if the property was changed; otherwise, false.</returns>
        protected bool ChangeProperty<T>(ref T propertyField, T value, [CallerMemberName]string propertyName = null)
        {
            return ChangeProperty(propertyName, ref propertyField, value);
        }

        /// <summary>
        /// Changes the specified property's backing field if they are considered unequal and raises any notifications as required.
        /// If both values are considered to be equal, then no assignement or notifications occurs.
        /// </summary>
        /// <typeparam name="T">The type of the property.</typeparam>
        /// <param name="propertyName">The name of the property.</param>
        /// <param name="propertyField">The backing field of the property passed in by reference and currently assigned the old value.</param>
        /// <param name="value">The new value for the property to be assigned to the backing field <paramref name="propertyField"/> if they are considered to be unequal.</param>
        /// <returns>True if the property was changed; otherwise, false.</returns>
        protected bool ChangeProperty<T>(string propertyName, ref T propertyField, T value)
        {
            return ChangeProperty(propertyName, ref propertyField, value, null, null, null, false, null);
        }

        /// <summary>
        /// Changes the specified property's backing field if they are considered unequal and raises any notifications as required.
        /// If both values are considered to be equal, then no assignement or notifications occurs.
        /// </summary>
        /// <typeparam name="T">The type of the property.</typeparam>
        /// <param name="propertyName">The name of the property.</param>
        /// <param name="propertyField">The backing field of the property passed in by reference and currently assigned the old value.</param>
        /// <param name="value">The new value for the property to be assigned to the backing field <paramref name="propertyField"/> if they are considered to be unequal.</param>
        /// <param name="affectedProperties">Any additional properties which should be considered affected by this change.</param>
        /// <returns>True if the property was changed; otherwise, false.</returns>
        protected bool ChangeProperty<T>(string propertyName, ref T propertyField, T value, params string[] affectedProperties)
        {
            return ChangeProperty(propertyName, ref propertyField, value, null, null, null, false, affectedProperties);
        }

        /// <summary>
        /// Changes the specified property's backing field if they are considered unequal and raises any notifications as required.
        /// If both values are considered to be equal, then no assignement or notifications occurs.
        /// </summary>
        /// <typeparam name="T">The type of the property.</typeparam>
        /// <param name="propertyName">The name of the property.</param>
        /// <param name="propertyField">The backing field of the property passed in by reference and currently assigned the old value.</param>
        /// <param name="value">The new value for the property to be assigned to the backing field <paramref name="propertyField"/> if they are considered to be unequal.</param>
        /// <param name="allPropertiesAffected">True if all properties should be considered affected by this change; otherwise, false.</param>
        /// <returns>True if the property was changed; otherwise, false.</returns>
        protected bool ChangeProperty<T>(string propertyName, ref T propertyField, T value, bool allPropertiesAffected)
        {
            return ChangeProperty(propertyName, ref propertyField, value, null, null, null, allPropertiesAffected, null);
        }

        /// <summary>
        /// Changes the specified property's backing field if they are considered unequal and raises any notifications as required.
        /// If both values are considered to be equal, then no assignement or notifications occurs.
        /// </summary>
        /// <typeparam name="T">The type of the property.</typeparam>
        /// <param name="propertyName">The name of the property.</param>
        /// <param name="propertyField">The backing field of the property passed in by reference and currently assigned the old value.</param>
        /// <param name="value">The new value for the property to be assigned to the backing field <paramref name="propertyField"/> if they are considered to be unequal.</param>
        /// <param name="onChange">An optional action to invoke after the change and prior to notifications.</param>
        /// <returns>True if the property was changed; otherwise, false.</returns>
        protected bool ChangeProperty<T>(string propertyName, ref T propertyField, T value, Action onChange)
        {
            return ChangeProperty(propertyName, ref propertyField, value, null, null, onChange, false, null);
        }

        /// <summary>
        /// Changes the specified property's backing field if they are considered unequal and raises any notifications as required.
        /// If both values are considered to be equal, then no assignement or notifications occurs.
        /// </summary>
        /// <typeparam name="T">The type of the property.</typeparam>
        /// <param name="propertyName">The name of the property.</param>
        /// <param name="propertyField">The backing field of the property passed in by reference and currently assigned the old value.</param>
        /// <param name="value">The new value for the property to be assigned to the backing field <paramref name="propertyField"/> if they are considered to be unequal.</param>
        /// <param name="onChange">An optional action to invoke after the change and prior to notifications.</param>
        /// <param name="affectedProperties">Any additional properties which should be considered affected by this change.</param>
        /// <returns>True if the property was changed; otherwise, false.</returns>
        protected bool ChangeProperty<T>(string propertyName, ref T propertyField, T value, Action onChange, params string[] affectedProperties)
        {
            return ChangeProperty(propertyName, ref propertyField, value, null, null, onChange, false, affectedProperties);
        }

        /// <summary>
        /// Changes the specified property's backing field if they are considered unequal and raises any notifications as required.
        /// If both values are considered to be equal, then no assignement or notifications occurs.
        /// </summary>
        /// <typeparam name="T">The type of the property.</typeparam>
        /// <param name="propertyName">The name of the property.</param>
        /// <param name="propertyField">The backing field of the property passed in by reference and currently assigned the old value.</param>
        /// <param name="value">The new value for the property to be assigned to the backing field <paramref name="propertyField"/> if they are considered to be unequal.</param>
        /// <param name="onChange">An optional action to invoke after the change and prior to notifications.</param>
        /// <param name="allPropertiesAffected">True if all properties should be considered affected by this change; otherwise, false.</param>
        /// <returns>True if the property was changed; otherwise, false.</returns>
        protected bool ChangeProperty<T>(string propertyName, ref T propertyField, T value, Action onChange, bool allPropertiesAffected)
        {
            return ChangeProperty(propertyName, ref propertyField, value, null, null, onChange, allPropertiesAffected, null);
        }

        /// <summary>
        /// Changes the specified property's backing field if they are considered unequal and raises any notifications as required.
        /// If both values are considered to be equal, then no assignement or notifications or initialization occurs.
        /// </summary>
        /// <typeparam name="T">The type of the property.</typeparam>
        /// <param name="propertyName">The name of the property.</param>
        /// <param name="propertyField">The backing field of the property passed in by reference and currently assigned the old value.</param>
        /// <param name="value">The new value for the property to be assigned to the backing field <paramref name="propertyField"/> if they are considered to be unequal.</param>
        /// <param name="initialize">An optional action to invoke on the new value after assignment, invoked only if the new value is not null.</param>
        /// <param name="uninitialize">An optional action to invoke on the old value prior to assignment, invoked only if the old value is not null.</param>
        /// <returns>True if the property was changed; otherwise, false.</returns>
        protected bool ChangeProperty<T>(string propertyName, ref T propertyField, T value, Action<T> initialize, Action<T> uninitialize)
        {
            return ChangeProperty(propertyName, ref propertyField, value, initialize, uninitialize, null, false, null);
        }

        /// <summary>
        /// Changes the specified property's backing field if they are considered unequal and raises any notifications as required.
        /// If both values are considered to be equal, then no assignement or notifications or initialization occurs.
        /// </summary>
        /// <typeparam name="T">The type of the property.</typeparam>
        /// <param name="propertyName">The name of the property.</param>
        /// <param name="propertyField">The backing field of the property passed in by reference and currently assigned the old value.</param>
        /// <param name="value">The new value for the property to be assigned to the backing field <paramref name="propertyField"/> if they are considered to be unequal.</param>
        /// <param name="initialize">An optional action to invoke on the new value after assignment, invoked only if the new value is not null.</param>
        /// <param name="uninitialize">An optional action to invoke on the old value prior to assignment, invoked only if the old value is not null.</param>
        /// <param name="affectedProperties">Any additional properties which should be considered affected by this change.</param>
        /// <returns>True if the property was changed; otherwise, false.</returns>
        protected bool ChangeProperty<T>(string propertyName, ref T propertyField, T value, Action<T> initialize, Action<T> uninitialize, params string[] affectedProperties)
        {
            return ChangeProperty(propertyName, ref propertyField, value, initialize, uninitialize, null, false, affectedProperties);
        }

        /// <summary>
        /// Changes the specified property's backing field if they are considered unequal and raises any notifications as required.
        /// If both values are considered to be equal, then no assignement or notifications or initialization occurs.
        /// </summary>
        /// <typeparam name="T">The type of the property.</typeparam>
        /// <param name="propertyName">The name of the property.</param>
        /// <param name="propertyField">The backing field of the property passed in by reference and currently assigned the old value.</param>
        /// <param name="value">The new value for the property to be assigned to the backing field <paramref name="propertyField"/> if they are considered to be unequal.</param>
        /// <param name="initialize">An optional action to invoke on the new value after assignment, invoked only if the new value is not null.</param>
        /// <param name="uninitialize">An optional action to invoke on the old value prior to assignment, invoked only if the old value is not null.</param>
        /// <param name="allPropertiesAffected">True if all properties should be considered affected by this change; otherwise, false.</param>
        /// <returns>True if the property was changed; otherwise, false.</returns>
        protected bool ChangeProperty<T>(string propertyName, ref T propertyField, T value, Action<T> initialize, Action<T> uninitialize, bool allPropertiesAffected)
        {
            return ChangeProperty(propertyName, ref propertyField, value, initialize, uninitialize, null, allPropertiesAffected, null);
        }

        /// <summary>
        /// Changes the specified property's backing field if they are considered unequal and raises any notifications as required.
        /// If both values are considered to be equal, then no assignement or notifications or initialization occurs.
        /// </summary>
        /// <typeparam name="T">The type of the property.</typeparam>
        /// <param name="propertyName">The name of the property.</param>
        /// <param name="propertyField">The backing field of the property passed in by reference and currently assigned the old value.</param>
        /// <param name="value">The new value for the property to be assigned to the backing field <paramref name="propertyField"/> if they are considered to be unequal.</param>
        /// <param name="initialize">An optional action to invoke on the new value after assignment, invoked only if the new value is not null.</param>
        /// <param name="uninitialize">An optional action to invoke on the old value prior to assignment, invoked only if the old value is not null.</param>
        /// <param name="onChange">An optional action to invoke after the change and prior to notifications.</param>
        /// <returns>True if the property was changed; otherwise, false.</returns>
        protected bool ChangeProperty<T>(string propertyName, ref T propertyField, T value, Action<T> initialize, Action<T> uninitialize, Action onChange)
        {
            return ChangeProperty(propertyName, ref propertyField, value, initialize, uninitialize, onChange, false, null);
        }

        /// <summary>
        /// Changes the specified property's backing field if they are considered unequal and raises any notifications as required.
        /// If both values are considered to be equal, then no assignement or notifications or initialization occurs.
        /// </summary>
        /// <typeparam name="T">The type of the property.</typeparam>
        /// <param name="propertyName">The name of the property.</param>
        /// <param name="propertyField">The backing field of the property passed in by reference and currently assigned the old value.</param>
        /// <param name="value">The new value for the property to be assigned to the backing field <paramref name="propertyField"/> if they are considered to be unequal.</param>
        /// <param name="initialize">An optional action to invoke on the new value after assignment, invoked only if the new value is not null.</param>
        /// <param name="uninitialize">An optional action to invoke on the old value prior to assignment, invoked only if the old value is not null.</param>
        /// <param name="onChange">An optional action to invoke after the change and prior to notifications.</param>
        /// <param name="affectedProperties">Any additional properties which should be considered affected by this change.</param>
        /// <returns>True if the property was changed; otherwise, false.</returns>
        protected bool ChangeProperty<T>(string propertyName, ref T propertyField, T value, Action<T> initialize, Action<T> uninitialize, Action onChange, params string[] affectedProperties)
        {
            return ChangeProperty(propertyName, ref propertyField, value, initialize, uninitialize, onChange, false, affectedProperties);
        }

        /// <summary>
        /// Changes the specified property's backing field if they are considered unequal and raises any notifications as required.
        /// If both values are considered to be equal, then no assignement or notifications or initialization occurs.
        /// </summary>
        /// <typeparam name="T">The type of the property.</typeparam>
        /// <param name="propertyName">The name of the property.</param>
        /// <param name="propertyField">The backing field of the property passed in by reference and currently assigned the old value.</param>
        /// <param name="value">The new value for the property to be assigned to the backing field <paramref name="propertyField"/> if they are considered to be unequal.</param>
        /// <param name="initialize">An optional action to invoke on the new value after assignment, invoked only if the new value is not null.</param>
        /// <param name="uninitialize">An optional action to invoke on the old value prior to assignment, invoked only if the old value is not null.</param>
        /// <param name="onChange">An optional action to invoke after the change and prior to notifications.</param>
        /// <param name="allPropertiesAffected">True if all properties should be considered affected by this change; otherwise, false.</param>
        /// <returns>True if the property was changed; otherwise, false.</returns>
        protected bool ChangeProperty<T>(string propertyName, ref T propertyField, T value, Action<T> initialize, Action<T> uninitialize, Action onChange, bool allPropertiesAffected)
        {
            return ChangeProperty(propertyName, ref propertyField, value, initialize, uninitialize, onChange, allPropertiesAffected, null);
        }

        /// <summary>
        /// Changes the specified property's backing field if they are considered unequal and raises any notifications as required.
        /// If both values are considered to be equal, then no assignement or notifications or initialization occurs.
        /// </summary>
        /// <typeparam name="T">The type of the property.</typeparam>
        /// <param name="propertyName">The name of the property.</param>
        /// <param name="propertyField">The backing field of the property passed in by reference and currently assigned the old value.</param>
        /// <param name="value">The new value for the property to be assigned to the backing field <paramref name="propertyField"/> if they are considered to be unequal.</param>
        /// <param name="initialize">An optional action to invoke on the new value after assignment, invoked only if the new value is not null.</param>
        /// <param name="uninitialize">An optional action to invoke on the old value prior to assignment, invoked only if the old value is not null.</param>
        /// <param name="onChange">An optional action to invoke after the change and prior to notifications.</param>
        /// <param name="allPropertiesAffected">True if all properties should be considered affected by this change; otherwise, false.</param>
        /// <param name="affectedProperties">Any additional properties which should be considered affected by this change.</param>
        /// <returns>True if the property was changed; otherwise, false.</returns>
        private bool ChangeProperty<T>(string propertyName, ref T propertyField, T value, Action<T> initialize, Action<T> uninitialize, Action onChange, bool allPropertiesAffected, string[] affectedProperties)
        {
            // if not changing, then do nothing and return false
            if (EqualityComparer<T>.Default.Equals(propertyField, value))
                return false;

            // capture for change tracking support
            var oldValue = propertyField;

            // changing notification
            NotifyPropertyChanging(propertyName);
            if (affectedProperties != null && affectedProperties.Length > 0)
                NotifyPropertiesChanging(affectedProperties);
            if (allPropertiesAffected)
                NotifyAllPropertiesChanging();

            // change property
            if (propertyField != null && uninitialize != null)
                uninitialize(propertyField);
            propertyField = value;
            if (propertyField != null && initialize != null)
                initialize(propertyField);
            onChange?.Invoke();

            // changed notification
            NotifyPropertyChanged(propertyName);
            if (affectedProperties != null && affectedProperties.Length > 0)
                NotifyPropertiesChanged(affectedProperties);
            if (allPropertiesAffected)
                NotifyAllPropertiesChanged();

            // support for dervied classes to implement change tracking
            ChangeTrackingOverride(propertyName, oldValue, value);
            return true;
        }

        protected virtual void ChangeTrackingOverride(string propertyName, object oldValue, object newValue)
        {
        }
    }
}
