using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace PairMatching.Tools
{
    /// <summary>
    /// Copy tool functions
    /// </summary>
    public static class CopyTools
    {
        /// <summary>
        /// Copy the values of properties that have the same name 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <typeparam name="S"></typeparam>
        /// <param name="from">Copy from the object</param>
        /// <param name="to">Copy to this object</param>
        /// <returns>The object with the value of the 'from' properties</returns>
        public static T CopyPropertiesTo<T, S>(this S from, T to, params string[] ignoreProps)
        {
            var properties = to.GetType().GetProperties();
            foreach (var propTo in properties)
            {
                if (!propTo.CanWrite || ignoreProps.Contains(propTo.Name))
                {
                    continue;
                }
                var propFrom = from.GetType().GetProperty(propTo.Name);
                if (!CanCopy(propFrom, propTo))
                {
                    continue;
                }
                
                
                var value = propFrom.GetValue(from, null);
                if (value != null)
                {
                    //if (propFrom.PropertyType.IsClass && !propFrom.PropertyType.IsArray)
                    //{
                    //    value.CopyPropertiesTo(propTo.GetValue(to));
                    //}
                    propTo.SetValue(to, value);
                }
            }
            return to;
        }

        private static bool CanCopy(PropertyInfo propFrom, PropertyInfo propTo)
        {
            return propFrom != null
                    && propFrom.CanRead
                    && propFrom.PropertyType == propTo.PropertyType;
        }

        /// <summary>
        /// Copy properties to new instance
        /// </summary>
        /// <typeparam name="S"></typeparam>
        /// <param name="from"></param>
        /// <param name="type"></param>
        /// <returns>New instance with the value of the 'from' properties</returns>
        public static object CopyPropertiesToNew<S>(this S from, Type type)
        {
            object to = Activator.CreateInstance(type); // new object of Type
            return from.CopyPropertiesTo(to);
        }

        /// <summary>
        /// Copy properties to new instance
        /// </summary>
        /// <typeparam name="S"></typeparam>
        /// <param name="from"></param>
        /// <param name="type"></param>
        /// <returns>New instance with the value of the 'from' properties</returns>
        public static TType CopyPropertiesToNew<S, TType>(this S from, params string[] ignoreProps) where TType : class
        {
            object to = Activator.CreateInstance(typeof(TType)); // new object of Type
            return from.CopyPropertiesTo(to as TType, ignoreProps);
        }

        /// <summary>
        /// Clone any object. Get a copy
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="original">the original object</param>
        /// <returns>The new copy of the original object</returns>
        public static T Clone<T>(this T original) where T : new()
        {
            T copyToObject = new();

            foreach (PropertyInfo propertyInfo in typeof(T).GetProperties())
            {
                if (propertyInfo.CanWrite)
                {
                    propertyInfo.SetValue(copyToObject, propertyInfo.GetValue(original, null), null);
                }
            }

            return copyToObject;
        }
    }
}
