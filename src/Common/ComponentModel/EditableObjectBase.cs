// Copyright © Kris Penner. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.


using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace KodeAid.ComponentModel
{
    public abstract class EditableObjectBase : ChangeNotificationObject
    {
        private static readonly ConcurrentDictionary<Type, Dictionary<string, PropertyInfo>> _propertyCache = new ConcurrentDictionary<Type, Dictionary<string, PropertyInfo>>();
        private Dictionary<string, PropertyInfo> _trackedProperties;
        private readonly Stack<Dictionary<string, object>> _states = new Stack<Dictionary<string, object>>();

        /// <summary>
        /// The number of edit transaction scopes pending.
        /// </summary>
        protected int PendingEditCount
        {
            get { return _states.Count; }
        }

        /// <summary>
        /// True if there are any changes pending in the current or ancestral (parent) edit transaction scopes.
        /// </summary>
        protected bool HasChangesInPendingEdit
        {
            get { return PendingEditCount > 0 && _states.Any(s => s.Any()); }
        }

        /// <summary>
        /// True if there are any changes pending in the current edit transaction scope only.
        /// </summary>
        protected bool HasChangesInCurrentEdit
        {
            get { return PendingEditCount > 0 && _states.Last().Any(); }
        }

        protected void BeginEdit()
        {
            _states.Push(new Dictionary<string, object>());
        }

        protected bool CancelEdit()
        {
            if (PendingEditCount == 0)
                return false;
            foreach (var entry in _states.Pop())
            {
                _trackedProperties[entry.Key].SetValue(this, entry.Value);
                foreach (var parentState in _states.Reverse().TakeWhile(s => Equals(s[entry.Key], entry.Value)))
                    parentState.Remove(entry.Key);
            }
            return true;
        }

        protected bool EndEdit()
        {
            if (PendingEditCount == 0)
                return false;
            _states.Pop();
            return true;
        }

        protected virtual bool IsPropertyTracked(PropertyInfo p)
        {
            if (!p.CanRead || !p.CanWrite || p.GetMethod == null || !p.GetMethod.IsPublic || p.SetMethod == null || !p.SetMethod.IsPublic)
                return false;
            var pt = p.PropertyType;
            if (pt == typeof(string))
                return true;
            var pti = pt.GetTypeInfo();
            return pti.IsPrimitive || pti.IsEnum || pti.IsValueType;
        }

        protected override void ChangeTrackingOverride(string propertyName, object oldValue, object newValue)
        {
            base.ChangeTrackingOverride(propertyName, oldValue, newValue);
            if (_states.Count > 0 && IsPropertyTracked(propertyName))
            {
                if (!_states.Peek().ContainsKey(propertyName))
                {
                    // save original value in this state and parent states (working backwards) if they didn't already have the property saved,
                    // once a parent state has the property already saved we know all other parent levels will too (TakeWhile) to their own saved original value for this property
                    foreach (var state in _states.Reverse().TakeWhile(s => !s.ContainsKey(propertyName)))
                        state.Add(propertyName, oldValue);
                }
                else
                {
                    // remove property from this state and parent states (working backwards) as long as their original value is also equal to the new value,
                    // once a parent state's value has a different original value than the new value we need to stop (TakeWhile) as they have a different saved state for this property
                    foreach (var state in _states.Reverse().TakeWhile(s => Equals(s[propertyName], newValue)))
                        state.Remove(propertyName);
                }
            }
        }

        private bool IsPropertyTracked(string propertyName)
        {
            if (string.IsNullOrEmpty(propertyName))
                return false;
            EnsureLocalPropertyCacheIsSet();
            return _trackedProperties.ContainsKey(propertyName);
        }

        private void EnsureLocalPropertyCacheIsSet()
        {
            if (_trackedProperties == null)
                _trackedProperties = _propertyCache.GetOrAdd(GetType(),
                  t => t.GetRuntimeProperties().Where(p => IsPropertyTracked(p)).ToDictionary(p => p.Name));
        }
    }
}
