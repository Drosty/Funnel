using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Text;

namespace Funnel
{
    /// <summary>
    /// The data mapping extensions.
    /// </summary>
    public static class Extensions
    {
        #region Public Methods and Operators
        private static readonly Dictionary<PropertyInfo, Action<object, object>> CachedSetter = new Dictionary<PropertyInfo, Action<object, object>>();
        /// <summary>
        /// Add explicit column/property mapping to the IEnumerable of reflected mapping sets
        /// </summary>
        /// <param name="rInfos">Reflection infos to add the mapping to</param>
        /// <param name="columnNameSource">Source Column/Property Name</param>
        /// <param name="columnNameTarget">Target Column/Property Name</param>
        /// <param name="converter">IMappingConverter type to use when converting</param>
        /// <returns>ReflectionInfo Object</returns>
        public static IEnumerable<ReflectionInfo> AddExplicitMapping(this IEnumerable<ReflectionInfo> rInfos, string columnNameSource, string columnNameTarget, Type converter = null)
        {
            return rInfos.Select(rInfo => rInfo.AddExplicitMapping(columnNameSource, columnNameTarget, converter));
        }

        /// <summary>
        /// Add explicit column/property mapping to the reflected mapping set.
        /// </summary>
        /// <param name="rInfo">Reflection info to add the mapping to</param>
        /// <param name="columnNameSource">Source Column/Property Name</param>
        /// <param name="columnNameTarget">Target Column/Property Name</param>
        /// <param name="converter">IMappingConverter type to use when converting</param>
        /// <returns>ReflectionInfo Object</returns>
        public static ReflectionInfo AddExplicitMapping(this ReflectionInfo rInfo, string columnNameSource, string columnNameTarget, Type converter = null)
        {
            var mapCol = new MappedColumn
                {
                    Source = columnNameSource,
                    Target = columnNameTarget,
                    Converter = converter
                };

            List<MappedColumn> col = null;
            if (rInfo.MappedColumns.TryGetValue(columnNameSource, out col))
                col.Add(mapCol);
            else
                rInfo.MappedColumns[columnNameSource] = new List<MappedColumn>() { mapCol };

            return rInfo;
        }

        /// <summary>
        /// Uses each ReflectionInfo in the enumerable to generate an instance of the set type populating the data using reflection.
        /// </summary>
        /// <typeparam name="T">
        /// Object Type to create and populate
        /// </typeparam>
        /// <param name="reflectedArray">
        /// IEnumerable of ReflectionInfo Key/Value Set
        /// </param>
        /// <param name="ignoreCase">Ignore case on name matching if true</param>
        /// <param name="throwException">If populating a field match with a value fails throw an exception if true</param>
        /// <param name="removeSourceUnderscores">Remove underscores from source property names if true</param>
        /// <param name="bindingFlags">
        /// </param>
        /// <returns>
        /// Enumerable of newly created instances of T
        /// </returns>
        public static IEnumerable<T> Into<T>(
            this IEnumerable<ReflectionInfo> reflectedArray, bool ignoreCase = false, bool throwException = true, bool removeSourceUnderscores = false,
            BindingFlags bindingFlags = BindingFlags.Public | BindingFlags.Instance)
        {
            return reflectedArray.Select(reflect => reflect.IntoSingle<T>(ignoreCase, throwException, removeSourceUnderscores, bindingFlags));
        }

        /// <summary>
        /// Uses ReflectionInfo to generate an instance of the set type populating the data using reflection.
        /// </summary>
        /// <typeparam name="T">
        /// Object Type to create and populate
        /// </typeparam>
        /// <param name="reflectedArray">
        /// ReflectionInfo Key/Value Set
        /// </param>
        /// <param name="toUpdate">Object whose properties will be updated</param>
        /// <param name="ignoreCase">Ignore case on name matching if true</param>
        /// <param name="throwException">If populating a field match with a value fails throw an exception if true</param>
        /// <param name="removeSourceUnderscores">Remove underscores from property names in source if true</param>
        /// <param name="bindingFlags">
        /// BindingFlags in GetProperties Command Defaults to Public
        /// </param>
        /// <returns>
        /// New instance of T
        /// </returns>
        public static T UpdateSingle<T>(
            this ReflectionInfo reflectedArray, T toUpdate, bool ignoreCase = false, bool throwException = true, bool removeSourceUnderscores = false, BindingFlags bindingFlags = BindingFlags.Public | BindingFlags.Instance)
        {
            Dictionary<string, PropertyInfo> props;

            if (ignoreCase)
                props = typeof(T).GetProperties(bindingFlags).ToDictionary(x => x.Name.ToLower());
            else
                props = typeof(T).GetProperties(bindingFlags).ToDictionary(x => x.Name);

            var empty = toUpdate;
            foreach (var reflected in reflectedArray)
            {
                PropertyInfo prop = null;
                List<MappedColumn> explicitMapping = null;


                if (reflectedArray.MappedColumns.TryGetValue(reflected.Key.ToString(), out explicitMapping))
                {
                    foreach (MappedColumn eMapping in explicitMapping)
                    {
                        props.TryGetValue(ignoreCase ? eMapping.Target.ToLower() : eMapping.Target, out prop);
                        if (eMapping.Converter != null && prop != null)
                        {
                            if (typeof(IMappingConverter).IsAssignableFrom(eMapping.Converter))
                            {
                                var converter = Activator.CreateInstance(eMapping.Converter) as IMappingConverter;
                                prop.SetValue(empty, converter.ConversionMethod(reflected.Value), null);
                            }
                            else
                                throw new Exception("Converter Type not a valid IMappingConverter");
                        }
                        else
                            ParseSetValue(reflected, prop, empty, throwException);
                    }
                }
                else
                {
                    string keyName = reflected.Key.ToString().Replace(" ", "").Trim();
                    if (removeSourceUnderscores)
                        keyName = keyName.Replace("_", "");
                    if (ignoreCase)
                        props.TryGetValue(keyName.ToLower(), out prop);
                    else
                        props.TryGetValue(keyName, out prop);
                    if (prop != null)
                        ParseSetValue(reflected, prop, empty, throwException);
                }

            }

            return empty;
        }

        /// <summary>
        /// Uses ReflectionInfo to generate an instance of the set type populating the data using reflection.
        /// </summary>
        /// <typeparam name="T">
        /// Object Type to create and populate
        /// </typeparam>
        /// <param name="reflectedArray">
        /// ReflectionInfo Key/Value Set
        /// </param>
        /// <param name="ignoreCase">Ignore case on name matching if true</param>
        /// <param name="throwException">If populating a field match with a value fails throw an exception if true</param>
        /// <param name="removeSourceUnderscores">Remove underscores from source property names if true</param>
        /// <param name="bindingFlags">
        /// BindingFlags in GetProperties Command Defaults to Public
        /// </param>
        /// <returns>
        /// New instance of T
        /// </returns>
        public static T IntoSingle<T>(
            this ReflectionInfo reflectedArray, bool ignoreCase = false, bool throwException = true, bool removeSourceUnderscores = false, BindingFlags bindingFlags = BindingFlags.Public | BindingFlags.Instance)
        {
            var empty = Activator.CreateInstance<T>();
            return UpdateSingle(reflectedArray, empty, ignoreCase, throwException, removeSourceUnderscores, bindingFlags);
        }

        /// <summary>
        /// Prints out a reflected or mapped set of data into a single string for logging or debugging purposes.
        /// </summary>
        /// <param name="reflected">
        /// ReflectionInfo Key/Value Set
        /// </param>
        /// <returns>
        /// Formatted string
        /// </returns>
        public static string IntoSingleLineString(this ReflectionInfo reflected)
        {
            var sb = new StringBuilder();
            foreach (var pair in reflected)
                sb.AppendFormat("{0}: {1}, ", pair.Key, pair.Value);

            return sb.ToString().Trim(' ', ',');
        }

        /// <summary>
        /// Uses each ReflectionInfo in the enumerable to generate a Datatable using the Type name as the table name,
        /// the properties as columns and the set of property values as rows.
        /// </summary>
        /// <param name="reflectedArray">
        /// IEnumerable of ReflectionInfo Key/Value Set
        /// </param>
        /// <returns>
        /// DataTable populated with a row for each object
        /// </returns>
        public static DataTable IntoTable(this IEnumerable<ReflectionInfo> reflectedArray)
        {
            var firstItem = reflectedArray.First();
            var dt = new DataTable(firstItem.SourceType != null ? firstItem.SourceType.Name : "DataTable");
            foreach (var item in reflectedArray.First())
            {
                var dc = new DataColumn(item.Key.ToString(), item.Value != null ? item.Value.GetType() : typeof(object));
                dt.Columns.Add(dc);
            }

            foreach (ReflectionInfo rows in reflectedArray)
            {
                DataRow dr = dt.NewRow();
                foreach (var row in rows)
                    dr[row.Key.ToString()] = row.Value ?? DBNull.Value;

                dt.Rows.Add(dr);
            }

            return dt;
        }

        /// <summary>
        /// Maps an enumerable of paired values to a Key / Value to allow setting properties via reflection in another object.
        /// </summary>
        /// <typeparam name="TReflected">
        /// Enumerable of Enumerable of value pairs
        /// </typeparam>
        /// <typeparam name="TKey">
        /// The Property Name selector type
        /// </typeparam>
        /// <typeparam name="TValue">
        /// The Value selector type
        /// </typeparam>
        /// <param name="toMapArray">
        /// Enumerable of value pairs
        /// </param>
        /// <param name="keySelector">
        /// Selector for Property Name
        /// </param>
        /// <param name="valueSelector">
        /// Selector for Value
        /// </param>
        /// <returns>
        /// Enumerable of ReflectionInfo Key / Value Set
        /// </returns>
        public static IEnumerable<ReflectionInfo> Map<TReflected, TKey, TValue>(
            this IEnumerable<IEnumerable<TReflected>> toMapArray,
            Func<TReflected, TKey> keySelector,
            Func<TReflected, TValue> valueSelector)
        {
            return toMapArray.Select(map => MapSingle(map, keySelector, valueSelector));
        }

        /// <summary>
        /// The map csv using header.
        /// </summary>
        /// <param name="csvArray">
        /// The csv array.
        /// </param>
        /// <returns>
        /// An Enumerable of ReflectionInfos
        /// </returns>
        public static IEnumerable<ReflectionInfo> MapArrayUsingHeader(this IEnumerable<IEnumerable<string>> csvArray)
        {
            List<IEnumerable<string>> csvData = csvArray.ToList();
            string[] header = csvData.First().ToArray();
            var rInfos = new List<ReflectionInfo>();
            foreach (var item in csvData.Skip(1))
            {
                string[] itemA = item.ToArray();

                var rInfo = new ReflectionInfo { SourceType = item.GetType() };

                for (int i = 0; i < (header.Count() > itemA.Count() ? itemA.Count() : header.Count()); i++)
                    rInfo[header[i]] = itemA[i];

                rInfos.Add(rInfo);
            }

            return rInfos.AsEnumerable();
        }

        /// <summary>
        /// Maps 2D Enumerable of paired values to a Key / Value to allow setting properties via reflection in another object.
        /// </summary>
        /// <typeparam name="TReflected">
        /// Enumerable of Enumerable of value pairs
        /// </typeparam>
        /// <typeparam name="TKey">
        /// The Property Name selector type
        /// </typeparam>
        /// <typeparam name="TValue">
        /// The Value selector type
        /// </typeparam>
        /// <param name="toMap">
        /// Enumerable of value pairs
        /// </param>
        /// <param name="keySelector">
        /// Selector for Property Name
        /// </param>
        /// <param name="valueSelector">
        /// Selector for Value
        /// </param>
        /// <returns>
        /// The Mapping.Extensions+ReflectionInfo.
        /// </returns>
        public static ReflectionInfo MapSingle<TReflected, TKey, TValue>(
            this IEnumerable<TReflected> toMap,
            Func<TReflected, TKey> keySelector,
            Func<TReflected, TValue> valueSelector)
        {
            var reflectInfo = new ReflectionInfo
                {
                    SourceType = typeof(TReflected)
                };
            foreach (TReflected mapValue in toMap)
                reflectInfo.Add(keySelector(mapValue), valueSelector(mapValue));

            return reflectInfo;
        }

        /// <summary>
        /// Parse an enumerable of strings as a Csv using the given delimiter. The delimiter can exist between quotes
        /// it will not be seperated.
        /// </summary>
        /// <param name="csvArray">The IEnumerable of strings</param>
        /// <param name="delimiter">The delimeter</param>
        /// <returns>A 2D Enumerable of strings</returns>
        public static IEnumerable<IEnumerable<string>> ParseCsv(this IEnumerable<string> csvArray, char delimiter)
        {
            foreach (string line in csvArray)
                yield return ParseCsvLine(line, delimiter);
        }

        /// <summary>
        /// Parse an enumerable of strings as a Csv using the given delimiter. The delimiter can exist between quotes
        /// it will not be seperated.
        /// </summary>
        /// <param name="csvArray">The IEnumerable of strings</param>
        /// <param name="delimiter">The delimeter</param>
        /// <returns>A 2D Enumerable of strings</returns>
        public static IEnumerable<IEnumerable<string>> ParseFixedWidth(this IEnumerable<string> csvArray, params int[] columnWidths)
        {
            foreach (string line in csvArray)
                yield return ParseFixedColumnLine(line, columnWidths);
        }

        /// <summary>
        /// Iterates through each property of each object in the enumerable and generates an enumerable of enumerables of Key / Value pairs
        /// to allow setting properties via reflection in another object.
        /// </summary>
        /// <param name="toReflectArray">
        /// Enumerable of objects to reflect
        /// </param>
        /// <param name="bindingFlags">
        /// BindingFlags in GetProperties Command Defaults to Public
        /// </param>
        /// <returns>
        /// Enumerable of ReflectionInfo Key / Value Set
        /// </returns>
        public static IEnumerable<ReflectionInfo> Reflect(
            this IEnumerable<object> toReflectArray,
            BindingFlags bindingFlags = BindingFlags.Public | BindingFlags.Instance)
        {
            foreach (object b in toReflectArray)
                yield return b.ReflectSingle(bindingFlags);
        }

        /// <summary>
        /// Iterates through the properties in an object and creates a Key / Value set to allow setting properties via reflection in another object.
        /// </summary>
        /// <param name="toReflect">
        /// Object to reflect
        /// </param>
        /// <param name="bindingFlags">
        /// BindingFlags in GetProperties Command Defaults to Public
        /// </param>
        /// <returns>
        /// ReflectionInfo Key / Value Set
        /// </returns>
        public static ReflectionInfo ReflectSingle(
            this object toReflect, BindingFlags bindingFlags = BindingFlags.Public | BindingFlags.Instance)
        {
            ReflectionInfo rInfo = null;
            var reflectType = toReflect.GetType();
            PropertyInfo[] props = reflectType.GetProperties(bindingFlags);
            rInfo = new ReflectionInfo
                {
                    SourceType = reflectType
                };
            foreach (PropertyInfo prop in props)
            {
                rInfo.Add(prop.Name, prop.GetValue(toReflect, new object[]
                    {
                    }));
            }
            return rInfo;
        }

        #endregion

        #region Methods
        private static IEnumerable<string> ParseFixedColumnLine(string line, IEnumerable<int> columnWidths)
        {
            var lineArray = new List<string>();
            int lastIndex = 0;
            foreach (var w in columnWidths)
            {
                if(lastIndex+w>line.Length)
                    lineArray.Add(line.Substring(lastIndex).Trim(' '));
                else
                    lineArray.Add(line.Substring(lastIndex, w).Trim(' '));
                lastIndex += w;
            }
            return lineArray;
        }

        private static IEnumerable<string> ParseCsvLine(string line, char delimiter)
        {
            string csvLine = line.Trim();
            bool inQuote = false;
            var lineArray = new List<string>();
            var currentEntry = new List<char>();
            foreach (char c in csvLine)
            {
                if (c == '\"')
                {
                    inQuote = !inQuote;
                    continue;
                }
                if (c == delimiter && !inQuote)
                {
                    lineArray.Add(String.Concat(currentEntry));
                    currentEntry.Clear();
                }
                else
                    currentEntry.Add(c);
            }
            lineArray.Add(String.Concat(currentEntry));
            return lineArray;
        }

        /// <summary>
        /// Method to parse a value using a TypeConverter via reflection
        /// </summary>
        /// <param name="reflected">The Pair to use when setting</param>
        /// <param name="prop">The property to set</param>
        /// <param name="empty">Empty object that will have its property set.</param>
        /// <param name="throwException">Throw exception on a failed value set</param>
        private static void ParseSetValue(KeyValuePair<object, object> reflected, PropertyInfo prop, object empty, bool throwException)
        {
            try
            {
                if (reflected.Value != null)
                {
                    Action<object, object> cach;
                    if (!CachedSetter.TryGetValue(prop, out cach))
                    {
                        var method = prop.GetSetMethod();
                        cach = (Action<object, object>)BuildSetAccessor(method);
                        CachedSetter[prop] = cach;
                    }
                    if (reflected.Value.GetType() == prop.PropertyType)
                        cach(empty, reflected.Value);
                    else
                    {
                        TypeConverter converter = TypeDescriptor.GetConverter(prop.PropertyType);
                        if (reflected.Value is string)
                        {
                            if (String.IsNullOrEmpty(reflected.Value.ToString()))
                                return;

                        }
                        cach(empty, converter.ConvertFrom(reflected.Value));
                    }
                }
                //else
                //cach(prop.SetValue(empty, null, null);
            }
            catch (Exception ex)
            {
                if (throwException)
                {
                    throw new Exception(String.Format("An Error occured while parsing a value for {0}",prop.Name),ex);
                }
                Debug.WriteLine(ex.ToString());
            }
        }
        static Action<object, object> BuildSetAccessor(MethodInfo method)
        {
            var obj = Expression.Parameter(typeof(object), "o");
            var value = Expression.Parameter(typeof(object));

            Expression<Action<object, object>> expr =
                Expression.Lambda<Action<object, object>>(
                    Expression.Call(
                        Expression.Convert(obj, method.DeclaringType),
                        method,
                        Expression.Convert(value, method.GetParameters()[0].ParameterType)),
                    obj,
                    value);

            return expr.Compile();
        }


        /// <summary>
        /// Maps the data table to an enumartable of <see cref="ReflectionInfo"/> objects.
        /// </summary>
        /// <param name="toReflectTable">The DataTable to map.</param>
        /// <returns>Returns an enumerable of <see cref="ReflectionInfo"/> objects, populated with data from the rows.</returns>
        public static IEnumerable<ReflectionInfo> MapDataTable(this DataTable toReflectTable)
        {
            return from DataRow toReflect in toReflectTable.Rows select toReflect.MapDataRow();
        }

        /// <summary>
        /// Maps the data row to a <see cref="ReflectionInfo"/> object.
        /// </summary>
        /// <param name="toReflect">The DataRow to map.</param>
        /// <returns>Returns an enumerable of <see cref="ReflectionInfo"/> objects, populated with data from the row</returns>
        public static ReflectionInfo MapDataRow(this DataRow toReflect)
        {
            var rInfo = new ReflectionInfo
            {
                SourceType = toReflect.GetType()
            };
            for (int i = 0; i < toReflect.Table.Columns.Count; i++)
            {
                rInfo.Add(toReflect.Table.Columns[i].ColumnName, toReflect[i]);
            }
            return rInfo;
        }

        /// <summary>
        /// Creates a dictionary the keys set to the property/column names.
        /// </summary>
        /// <param name="reflected">The Reflection Info object to convert to a Dynamic object.</param>
        /// <returns>Dictionary</returns>
        public static Dictionary<string, object> IntoSingleDictionary(this ReflectionInfo reflected)
        {
            return reflected.ToDictionary(x => x.Key.ToString(), x => x.Value);
        }

        /// <summary>
        /// Creates an IEnumerable of dictionaries with the property/column names as keys.
        /// </summary>
        /// <param name="reflectionArray">The reflected array to convert.</param>
        /// <returns>IEnumerable of Dictionaries</returns>
        public static IEnumerable<Dictionary<string, object>> IntoDictionary(this IEnumerable<ReflectionInfo> reflectionArray)
        {
            foreach (var rInfo in reflectionArray)
            {
                yield return rInfo.IntoSingleDictionary();
            }
        }

        /// <summary>
        /// Converts some reflection info into an enumeration of a Dynamic object.
        /// </summary>
        /// <param name="reflectedArray">The reflected array to convert.</param>
        /// <returns>An enumerable of dynamic objects.</returns>
        public static IEnumerable<dynamic> IntoDynamic(this IEnumerable<ReflectionInfo> reflectedArray)
        {
            return reflectedArray.Select(reflected => reflected.IntoDynamicSingle());
        }

        /// <summary>
        /// Converts some reflection info into a Dynamic object.
        /// </summary>
        /// <param name="reflected">The Reflection Info object to convert to a Dynamic object.</param>
        /// <returns>A dynamic object with properties corresponding to the reflection info.</returns>
        public static dynamic IntoDynamicSingle(this ReflectionInfo reflected)
        {
            return new DynamicEntity(reflected.IntoSingleDictionary());
            //foreach (var keyValuePair in reflected)
            //{
            //    KeyValuePair<object, object> tempReflected = keyValuePair; // Needs to happen or it could produce race conditions.
            //    IEnumerable<MappedColumn> explicitMapping = reflected.MappedColumns.Where(x => x.Source == tempReflected.Key.ToString()).ToList();

            //    if (explicitMapping.Any())
            //    {
            //        foreach (MappedColumn eMapping in explicitMapping)
            //        {
            //            if (eMapping.Converter != null)
            //            {
            //                if (typeof(IMappingConverter).IsAssignableFrom(eMapping.Converter))
            //                {
            //                    var converter = (IMappingConverter)Activator.CreateInstance(eMapping.Converter);
            //                    dynamicObject[eMapping.Target] = converter.ConversionMethod(keyValuePair.Value.ToString());
            //                }
            //                else
            //                    throw new Exception("Converter Type not a valid IMappingConverter");
            //            }
            //            else
            //                dynamicObject[keyValuePair.Key.ToString()] = keyValuePair.Value;
            //        }
            //    }
            //    else
            //        dynamicObject[keyValuePair.Key.ToString()] = keyValuePair.Value;
            //}
            //return dynamicObject;
        }

        #endregion
    }
}