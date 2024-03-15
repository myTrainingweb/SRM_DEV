using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using System.Linq;
using System.Web;
using System.Data;
using PlusCP.ActionFilters;

namespace PlusCP.Extensions
{
    public static class CollectionExtensions
    {
        public static IEnumerable<T> ToList<T>(this DataTable dt) where T : class, new()
        {
            try
            {
                Type objectType = typeof(T);
                List<T> listResult = new List<T>();
                var properties = objectType.GetProperties().Where(p => p.CanWrite);
                foreach (var dataRow in dt.AsEnumerable())
                {
                    T newItem = new T();
                    foreach (var property in properties)
                    {
                        try
                        {
                            var propType = Nullable.GetUnderlyingType(property.PropertyType) ?? property.PropertyType;
                            var value =
                                (dataRow[property.Name] == null || dataRow[property.Name] == DBNull.Value)
                                ? null
                                : Convert.ChangeType(dataRow[property.Name], propType);
                            property.SetValue(newItem, value, null);
                        }
                        catch { }
                    }
                    listResult.Add(newItem);
                }
                return listResult;
            }
            catch (Exception)
            {

                return Enumerable.Empty<T>();
            }

        }

        public static List<ArrayList> ToValueArrayList<T>(this IEnumerable<T> enumerable)
        {
            List<ArrayList> result = new List<ArrayList>();
            foreach (var val in enumerable)
            {
                ArrayList al = new ArrayList();
                foreach (var property in val.GetType().GetProperties(BindingFlags.Public | BindingFlags.Instance))
                {

                    var value = property.GetValue(val, null);
                    Type pt = property.PropertyType;

                    if (pt == typeof(string))
                    {
                        al.Add(value);
                    }
                    if (pt == typeof(DateTime) || pt == typeof(DateTime?))
                    {
                        al.Add(((DateTime?)value)?.ToString("yyyy.MM.dd") ?? string.Empty);
                    }
                    if (pt == typeof(TimeSpan) || pt == typeof(TimeSpan?))
                    {
                        al.Add(((TimeSpan?)value)?.ToString(@"hh\:mm") ?? string.Empty);
                    }
                    if (pt == typeof(int) || pt == typeof(int?))
                    {
                        bool isIntTime = property.GetCustomAttributes(false)
                            .Any(ca => ca.GetType() == typeof(IntTimeAttribute));
                        if (isIntTime && value != null)
                        {
                            al.Add(((int)value).ToErpTime());
                        }
                        else
                        {
                            al.Add(value);
                        }
                    }
                    if (pt == typeof(bool) || pt == typeof(bool?))
                    {
                        if (value == null)
                            al.Add(string.Empty);
                        else
                            al.Add(value.ToString());
                    }
                    if (pt == typeof(decimal) || pt == typeof(decimal?))
                    {
                        if (value == null)
                            al.Add(0);
                        else
                            al.Add((decimal)value);
                    }
                    if (pt == typeof(double) || pt == typeof(double?))
                    {
                        if (value == null)
                            al.Add(0);
                        else
                            al.Add((double)value);
                    }
                    if (pt == typeof(byte[]) || pt == typeof(byte[]))
                    {

                    }
                    if (pt == typeof(short) || pt == typeof(short?))
                    {
                        al.Add(value);
                    }
                }
                result.Add(al);
            }
            return result;
        }

        public static DataTable ConvertToDataTable<T>(this IEnumerable<T> entityList)
        {
            if (entityList == null) throw new ArgumentException("entityList");

            Type t = typeof(T);
            DataTable dtResult = new DataTable(t.Name);
            var propertyList = t.GetProperties();
            foreach (PropertyInfo property in propertyList)
            {
                Type columnType = property.PropertyType.IsGenericType &&
                                  property.PropertyType.GetGenericTypeDefinition() == typeof(Nullable<>)
                    ? Nullable.GetUnderlyingType(property.PropertyType)
                    : property.PropertyType;

                dtResult.Columns.Add(property.Name, columnType);
            }
            foreach (T entity in entityList)
            {
                DataRow dr = dtResult.NewRow();
                foreach (PropertyInfo propertyInfo in propertyList)
                {
                    object propertyValue = propertyInfo.GetValue(entity, null);
                    dr[propertyInfo.Name] = propertyValue ?? DBNull.Value;
                }
                dtResult.Rows.Add(dr);
            }

            return dtResult;
        }
    }
}