// Copyright © Kris Penner. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.


using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;

namespace KodeAid.Reflection
{
    public static class ReflectionHelper
    {
        /// <summary>
        /// Traverses a path down an object including properties, dictionaries, lists and arrays.
        /// "Employees[John].Addresses[0].StreetName"
        /// </summary>
        /// <param name="obj">The root target object.</param>
        /// <param name="path">The path to follow, eg: "Employees[John].Addresses[0].StreetName"</param>
        /// <param name="ignoreCase">True to ignore case on property names, otherwise false.</param>
        /// <param name="throwOnPathNotFound">True to throw InvalidOperationException when a property or index was not found, otherwise false.</param>
        /// <param name="throwOnNullReference">True to throw NullReferenceException when path cannot be fully traversed due to a null segment along the way, otherwise false.</param>
        /// <returns></returns>
        public static object FollowPropertyPath(object obj, string path, bool ignoreCase = false, bool throwOnPathNotFound = false, bool throwOnNullReference = false)
        {
            ArgCheck.NotNull(nameof(path), path);

            var traversed = new StringBuilder();

            foreach (var propertyName in path.Split('.', '['))
            {
                if (obj == null)
                {
                    if (throwOnNullReference)
                    {
                        throw new NullReferenceException($"Path '{traversed.ToString()}' is null.");
                    }
                    return null;
                }

                var index = propertyName.EndsWith("]") ? propertyName.Substring(0, propertyName.Length - 1) : null;
                var type = obj.GetType();

                if (index == null)  // get by property
                {
                    PropertyInfo property = null;
                    property = type.GetProperty(propertyName, (BindingFlags.Instance | BindingFlags.Public) | (ignoreCase ? BindingFlags.IgnoreCase : BindingFlags.Default));
                    if (property == null)
                    {
                        if (throwOnPathNotFound)
                        {
                            throw new InvalidOperationException($"Property {propertyName} not found on {type.FullName} at '{traversed.ToString()}'.");
                        }
                        return null;
                    }
                    obj = property.GetValue(obj, null);
                }
                else  // get by index
                {
                    var indexed = false;
                    foreach (var indexerInterface in obj.GetType().GetInterfaces())
                    {
                        if (indexerInterface.IsGenericType && indexerInterface.GetGenericTypeDefinition() == typeof(IDictionary<,>))
                        {
                            indexed = true;
                            obj = typeof(ReflectionHelper).GetMethod(nameof(GetFromDictionary), BindingFlags.Static | BindingFlags.NonPublic)
                                .MakeGenericMethod(indexerInterface.GetGenericArguments())
                                .Invoke(null, new object[] { obj, index });
                            break;
                        }
                        if (indexerInterface.IsGenericType && indexerInterface.GetGenericTypeDefinition() == typeof(IList<>))
                        {
                            indexed = true;
                            obj = typeof(ReflectionHelper).GetMethod(nameof(GetFromList), BindingFlags.Static | BindingFlags.NonPublic)
                                .MakeGenericMethod(indexerInterface.GetGenericArguments())
                                .Invoke(null, new object[] { obj, index });
                            break;
                        }
                    }
                    if (!indexed && obj is Array array)
                    {
                        indexed = true;
                        obj = array.GetValue(Convert.ToInt32(index));
                    }
                    if (!indexed)
                    {
                        if (throwOnPathNotFound)
                        {
                            throw new InvalidOperationException($"Index {index} not found on {type.FullName} at '{traversed.ToString()}'.");
                        }
                        return null;
                    }
                }

                traversed.Append(index != null ? $"[{index}]" : (traversed.Length == 0 ? propertyName : $".{propertyName}"));
            }

            return obj;
        }

        private static TValue GetFromDictionary<TKey, TValue>(IDictionary<TKey, TValue> dict, object index)
        {
            var key = (TKey)Convert.ChangeType(index, typeof(TKey), null);
            return dict[key];
        }

        private static T GetFromList<T>(IList<T> list, object index)
        {
            return list[Convert.ToInt32(index)];
        }
    }
}
