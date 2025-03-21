// Copyright © Kris Penner. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.


using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;

namespace KodeAid.Reflection
{
    public static class ReflectionHelper
    {
        public static bool IsNullable(Type t)
        {
            ArgCheck.NotNull(nameof(t), t);

            if (t.IsValueType)
            {
                return IsNullableType(t);
            }

            return true;
        }

        public static bool IsNullableType(Type t)
        {
            ArgCheck.NotNull(nameof(t), t);

            return (t.IsGenericType && t.GetGenericTypeDefinition() == typeof(Nullable<>));
        }

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
                        throw new NullReferenceException($"Path '{traversed}' is null.");
                    }

                    return null;
                }

                var index = propertyName.EndsWith("]") ? propertyName.Substring(0, propertyName.Length - 1) : null;
                var type = obj.GetType();

                if (index == null)  // get by property
                {
                    PropertyInfo property = null;
                    property = type.GetProperty(propertyName, BindingFlags.Instance | BindingFlags.Public | (ignoreCase ? BindingFlags.IgnoreCase : BindingFlags.Default));

                    if (property == null)
                    {
                        if (throwOnPathNotFound)
                        {
                            throw new InvalidOperationException($"Property {propertyName} not found on {type.FullName} at '{traversed}'.");
                        }

                        return null;
                    }

                    obj = property.GetValue(obj, null);
                }
                else  // get by index
                {
                    var indexed = false;
                    var interfaces = type.GetInterfaces().ToList();

                    if (type.IsInterface)
                    {
                        interfaces.Insert(0, type);
                    }

                    foreach (var indexerInterface in interfaces)
                    {
                        if (indexerInterface.IsGenericType && indexerInterface.GetGenericTypeDefinition() == typeof(IDictionary<,>))
                        {
                            indexed = true;
                            obj = typeof(ReflectionHelper).GetMethod(nameof(GetFromDictionary), BindingFlags.Static | BindingFlags.NonPublic)
                                .MakeGenericMethod(indexerInterface.GetGenericArguments())
                                .Invoke(null, [obj, index]);
                            break;
                        }

                        if (indexerInterface.IsGenericType && indexerInterface.GetGenericTypeDefinition() == typeof(IList<>))
                        {
                            indexed = true;
                            obj = typeof(ReflectionHelper).GetMethod(nameof(GetFromList), BindingFlags.Static | BindingFlags.NonPublic)
                                .MakeGenericMethod(indexerInterface.GetGenericArguments())
                                .Invoke(null, [obj, index, ignoreCase, throwOnPathNotFound, throwOnNullReference]);
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
                            throw new InvalidOperationException($"Index {index} not found on {type.FullName} at '{traversed}'.");
                        }

                        return null;
                    }
                }

                traversed.Append(index != null ? $"[{index}]" : (traversed.Length == 0 ? propertyName : $".{propertyName}"));
            }

            return obj;
        }

        public static bool ConfirmPropertyPath(Type type, string path, bool ignoreCase = false)
        {
            ArgCheck.NotNull(nameof(type), type);
            ArgCheck.NotNull(nameof(path), path);

            var traversed = new StringBuilder();

            foreach (var propertyName in path.Split('.', '['))
            {
                var index = propertyName.EndsWith("]") ? propertyName.Substring(0, propertyName.Length - 1) : null;

                if (index == null)  // get by property
                {
                    PropertyInfo property = null;
                    property = type.GetProperty(propertyName, BindingFlags.Instance | BindingFlags.Public | (ignoreCase ? BindingFlags.IgnoreCase : BindingFlags.Default));

                    if (property == null)
                    {
                        return false;
                    }

                    type = property.PropertyType;
                }
                else  // get by index
                {
                    var indexed = false;
                    var interfaces = type.GetInterfaces().ToList();

                    if (type.IsInterface)
                    {
                        interfaces.Insert(0, type);
                    }

                    foreach (var indexerInterface in interfaces)
                    {
                        if (indexerInterface.IsGenericType && indexerInterface.GetGenericTypeDefinition() == typeof(IDictionary<,>))
                        {
                            indexed = true;
                            type = indexerInterface.GenericTypeArguments[1];
                            break;
                        }

                        if (indexerInterface.IsGenericType && indexerInterface.GetGenericTypeDefinition() == typeof(IList<>))
                        {
                            indexed = true;
                            type = indexerInterface.GenericTypeArguments[0];
                            break;
                        }
                    }

                    if (!indexed && type.IsArray)
                    {
                        indexed = true;
                        type = type.GetElementType();
                    }

                    if (!indexed)
                    {
                        return false;
                    }
                }

                traversed.Append(index != null ? $"[{index}]" : (traversed.Length == 0 ? propertyName : $".{propertyName}"));
            }

            return true;
        }

        /// <summary>
        /// Find all defined types across all referenced assemblies including those in the execution paths.
        /// </summary>
        /// <typeparam name="T">Type which the results must be assignable to.</typeparam>
        /// <param name="startingPoint">Assemblies to start the search from, if not provided the entry assembly is used: Assembly.GetEntryAssembly().</param>
        /// <param name="typeFilter">Predicate to filter out returned types, or null for the default which is to include all public instance non-abstract and non-generic classes or value-types.</param>
        /// <param name="mustHavePublicDefaultCtor">True to ensure all returned types have a public default constructor defined, otherwise false.</param>
        /// <param name="assemblySearchOptions">How to search for additional assemblies to include.</param>
        /// <param name="throwOnError">True to throw on any assembly load errors, otherwise false.</param>
        /// <param name="assemblyNamePrefixes">Case insensitive prefixes of assembly names and file names (*.dlls) to include in search, null/empty to include all.</param>
        /// <returns></returns>
        public static IEnumerable<Type> FindAllTypes<T>(Assembly startingPoint = null, Predicate<Type> typeFilter = null, bool mustHavePublicDefaultCtor = false, AssemblySearchOptions assemblySearchOptions = AssemblySearchOptions.Default, bool throwOnError = false, IEnumerable<string> assemblyNamePrefixes = null)
        {
            return FindAllTypes(startingPoint, typeof(T), typeFilter, mustHavePublicDefaultCtor, assemblySearchOptions, throwOnError, assemblyNamePrefixes);
        }

        /// <summary>
        /// Find all defined types across all referenced assemblies including those in the execution paths.
        /// </summary>
        /// <typeparam name="T">Type which the results must be assignable to.</typeparam>
        /// <param name="startingPoint">Assemblies to start the search from, if not provided the entry assembly is used: Assembly.GetEntryAssembly().</param>
        /// <param name="typeFilter">Predicate to filter out returned types, or null for the default which is to include all public instance non-abstract and non-generic classes or value-types.</param>
        /// <param name="mustHavePublicDefaultCtor">True to ensure all returned types have a public default constructor defined, otherwise false.</param>
        /// <param name="assemblySearchOptions">How to search for additional assemblies to include.</param>
        /// <param name="throwOnError">True to throw on any assembly load errors, otherwise false.</param>
        /// <param name="assemblyNamePrefixes">Case insensitive prefixes of assembly names and file names (*.dlls) to include in search, null/empty to include all.</param>
        /// <returns></returns>
        public static IEnumerable<Type> FindAllTypes<T>(Assembly startingPoint = null, Predicate<Type> typeFilter = null, bool mustHavePublicDefaultCtor = false, AssemblySearchOptions assemblySearchOptions = AssemblySearchOptions.Default, bool throwOnError = false, params string[] assemblyNamePrefixes)
        {
            return FindAllTypes(startingPoint, typeof(T), typeFilter, mustHavePublicDefaultCtor, assemblySearchOptions, throwOnError, assemblyNamePrefixes);
        }

        /// <summary>
        /// Find all defined types across all referenced assemblies including those in the execution paths.
        /// </summary>
        /// <param name="startingPoint">Assemblies to start the search from, if not provided the entry assembly is used: Assembly.GetEntryAssembly().</param>
        /// <param name="ofType">Type which the results must be assignable to, null for no filter.</param>
        /// <param name="typeFilter">Predicate to filter out returned types, or null for the default which is to include all public instance non-abstract and non-generic classes or value-types.</param>
        /// <param name="mustHavePublicDefaultCtor">True to ensure all returned types have a public default constructor defined, otherwise false.</param>
        /// <param name="assemblySearchOptions">How to search for additional assemblies to include.</param>
        /// <param name="throwOnError">True to throw on any assembly load errors, otherwise false.</param>
        /// <param name="assemblyNamePrefixes">Case insensitive prefixes of assembly names and file names (*.dlls) to include in search, null/empty to include all.</param>
        /// <returns></returns>
        public static IEnumerable<Type> FindAllTypes(Assembly startingPoint = null, Type ofType = null, Predicate<Type> typeFilter = null, bool mustHavePublicDefaultCtor = false, AssemblySearchOptions assemblySearchOptions = AssemblySearchOptions.Default, bool throwOnError = false, params string[] assemblyNamePrefixes)
        {
            return FindAllTypes(startingPoint, ofType, typeFilter, mustHavePublicDefaultCtor, assemblySearchOptions, throwOnError, (IEnumerable<string>)assemblyNamePrefixes);
        }

        /// <summary>
        /// Find all defined types across all referenced assemblies including those in the execution paths.
        /// </summary>
        /// <param name="startingPoint">Assemblies to start the search from, if not provided the entry assembly is used: Assembly.GetEntryAssembly().</param>
        /// <param name="ofType">Type which the results must be assignable to, null for no filter.</param>
        /// <param name="typeFilter">Predicate to filter out returned types, or null for the default which is to include all public instance non-abstract and non-generic classes or value-types.</param>
        /// <param name="mustHavePublicDefaultCtor">True to ensure all returned types have a public default constructor defined, otherwise false.</param>
        /// <param name="assemblySearchOptions">How to search for additional assemblies to include.</param>
        /// <param name="throwOnError">True to throw on any assembly load errors, otherwise false.</param>
        /// <param name="assemblyNamePrefixes">Case insensitive prefixes of assembly names and file names (*.dlls) to include in search, null/empty to include all.</param>
        /// <returns></returns>
        public static IEnumerable<Type> FindAllTypes(Assembly startingPoint = null, Type ofType = null, Predicate<Type> typeFilter = null, bool mustHavePublicDefaultCtor = false, AssemblySearchOptions assemblySearchOptions = AssemblySearchOptions.Default, bool throwOnError = false, IEnumerable<string> assemblyNamePrefixes = null)
        {
            return FindAssemblies(startingPoint, assemblySearchOptions, throwOnError, assemblyNamePrefixes)
                .SelectMany(a => a.GetLoadableTypes())
                .Where(t => ofType == null || ofType.IsAssignableFrom(t))
                .Where(t => typeFilter?.Invoke(t) ?? ((t.IsClass || t.IsValueType) && t.IsPublic && !t.IsAbstract && !t.IsGenericType))
                .Where(t => !mustHavePublicDefaultCtor || t.IsValueType || t.GetConstructor(BindingFlags.Instance | BindingFlags.Public, null, Type.EmptyTypes, null) != null)
                .Distinct() // shouldn't be required
                .ToList();
        }

        public static IEnumerable<Assembly> FindAssemblies(Assembly startingPoint = null, AssemblySearchOptions assemblySearchOptions = AssemblySearchOptions.Default, bool throwOnError = false, IEnumerable<string> assemblyNamePrefixes = null)
        {
            var directorySearchOptions = assemblySearchOptions.HasFlagSet(AssemblySearchOptions.IncludeSubdirectories) ? SearchOption.AllDirectories : SearchOption.TopDirectoryOnly;
            var matchedAssemblies = new List<Assembly>();
            var assembliesSearched = new HashSet<string>(StringComparer.InvariantCultureIgnoreCase);

            var assembliesToSearch = new Queue<Assembly>();
            startingPoint ??= Assembly.GetEntryAssembly();
            assembliesToSearch.Enqueue(startingPoint);

            if (assemblySearchOptions.HasFlagSet(AssemblySearchOptions.StartingDirectory))
            {
                assembliesToSearch.EnqueueRange(FindAssemblies(startingPoint, assemblyNamePrefixes, directorySearchOptions, assembliesSearched, throwOnError));
            }

            while (assembliesToSearch.Count > 0)
            {
                var assembly = assembliesToSearch.Dequeue();

                if (assembliesSearched.Add(assembly.FullName))
                {
                    if (!assemblyNamePrefixes.Any() || assemblyNamePrefixes.Any(n => assembly.FullName.StartsWith(n, StringComparison.InvariantCultureIgnoreCase)))
                    {
                        matchedAssemblies.Add(assembly);

                        if (assemblySearchOptions.HasFlagSet(AssemblySearchOptions.ReferencedAssemblies))
                        {
                            assembliesToSearch.EnqueueRange(assembly.GetReferencedAssemblies().Select(a =>
                            {
                                try
                                {
                                    return Assembly.Load(a);
                                }
                                catch
                                {
                                    if (throwOnError)
                                    {
                                        throw;
                                    }

                                    return null;
                                }
                            }).WhereNotNull());
                        }

                        if (assembly != startingPoint && assemblySearchOptions.HasFlagSet(AssemblySearchOptions.AssemblyDirectories))
                        {
                            assembliesToSearch.EnqueueRange(FindAssemblies(assembly, assemblyNamePrefixes, directorySearchOptions, assembliesSearched, throwOnError));
                        }
                    }
                }
            }

            return matchedAssemblies
                .Distinct() // shouldn't be required
                .ToList();
        }

        private static IEnumerable<Assembly> FindAssemblies(Assembly assembly, IEnumerable<string> assemblyNamePrefixes, SearchOption directorySearchOptions, HashSet<string> assembliesSearched, bool throwOnError)
        {
            var assembliesFound = new List<Assembly>();
#if NET8_0_OR_GREATER
            var assemblyLocation = Path.GetDirectoryName(new Uri(assembly.Location).AbsolutePath);
#else
            var assemblyLocation = Path.GetDirectoryName(new Uri(assembly.CodeBase).AbsolutePath);
#endif

            if (assembliesSearched.Add(assemblyLocation))
            {
                var dllFiles = new List<string>();

                try
                {
                    if (!assemblyNamePrefixes.Any())
                    {
                        dllFiles.AddRange(Directory.EnumerateFiles(assemblyLocation, "*.dll", directorySearchOptions));
                    }
                    else
                    {
                        dllFiles.AddRange(assemblyNamePrefixes.SelectMany(n => Directory.EnumerateFiles(assemblyLocation, $"{n}*.dll", directorySearchOptions)));
                    }
                }
                catch
                {
                    if (throwOnError)
                    {
                        throw;
                    }
                }

                foreach (var dllFile in dllFiles.Distinct())
                {
                    try
                    {
                        assembliesFound.Add(Assembly.Load(AssemblyName.GetAssemblyName(dllFile)));
                    }
                    catch
                    {
                        try
                        {
                            assembliesFound.Add(Assembly.LoadFrom(dllFile));
                        }
                        catch
                        {
                            if (throwOnError)
                            {
                                throw;
                            }
                        }
                    }
                }
            }

            return assembliesFound;
        }

        private static TValue GetFromDictionary<TKey, TValue>(IDictionary<TKey, TValue> dict, object index)
        {
            var key = (TKey)Convert.ChangeType(index, typeof(TKey), null);
            return dict[key];
        }

        private static T GetFromList<T>(IList<T> list, object index, bool ignoreCase, bool throwOnPathNotFound, bool throwOnNullReference)
        {
            if (index is string s)
            {
                if (s.Contains('='))
                {
                    var parts = s.Split('=');

                    if (parts.Length == 2)
                    {
                        return list.FirstOrDefault(item =>
                        {
                            var value = FollowPropertyPath(item, parts[0], ignoreCase, throwOnPathNotFound, throwOnNullReference);

                            if (value != null)
                            {
                                return Equals(value, ParseHelper.Parse(parts[1], value.GetType(), ignoreCase));
                            }

                            return false;
                        });
                    }
                }
                else if (s.EndsWith(" -isnull"))
                {
                    s = s.Substring(0, s.Length - " -isnull".Length);

                    return list.FirstOrDefault(item =>
                    {
                        return FollowPropertyPath(item, s, ignoreCase, throwOnPathNotFound, throwOnNullReference) == null;
                    });
                }
                else if (s.EndsWith(" -notnull"))
                {
                    s = s.Substring(0, s.Length - " -notnull".Length);

                    return list.FirstOrDefault(item =>
                    {
                        return FollowPropertyPath(item, s, ignoreCase, throwOnPathNotFound, throwOnNullReference) != null;
                    });
                }
            }

            return list[Convert.ToInt32(index)];
        }
    }
}
