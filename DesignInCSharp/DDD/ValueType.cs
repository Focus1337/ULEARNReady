using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Ddd.Taxi.Domain;
using System.Security.Cryptography;
using System.Text;

namespace Ddd.Infrastructure
{
    public class MatchingPair
    {
        public string PropertyName = string.Empty;
        public bool PropertyMatch = false;
    }

    public class ValueType<T>
    {
        public bool Equals(PersonName name) => false;

        private static int counter = 500;

        public override int GetHashCode()
        {
            var sSourceData = this.ToString();
            var tmpSource = Encoding.ASCII.GetBytes(sSourceData);

            var tmpHash = new MD5CryptoServiceProvider().ComputeHash(tmpSource);

            var a = BitConverter.ToInt32(tmpHash, 0);
            var b = BitConverter.ToInt32(tmpHash, 4);
            var c = BitConverter.ToInt32(tmpHash, 8);
            var d = BitConverter.ToInt32(tmpHash, 12);

            return a;
        }

        public override string ToString()
        {
            var a = this.GetType();

            var result = a.Name + "(";

            var propertiesNames =
                a.GetProperties(BindingFlags.Public | BindingFlags.Instance)
                    .Select(property => property.Name)
                    .ToList();

            propertiesNames.Sort();

            for (var i = 0; i < propertiesNames.Count; i++)
            {
                var aInfo = a.GetProperty(propertiesNames[i]);
                var aValue = aInfo?.GetValue(this);

                result += aValue != null ? propertiesNames[i] + ": " + aValue : propertiesNames[i] + ": ";

                if (i != propertiesNames.Count - 1) result += "; ";
            }

            result += ")";
            return result;
        }

        public override bool Equals(object obj)
        {
            if (obj == null)
                return false;

            var a = this.GetType();
            var b = obj.GetType();

            if (a != b)
                return false;

            var countA = a.GetProperties().ToList<PropertyInfo>().Count;
            var countB = b.GetProperties().ToList<PropertyInfo>().Count;

            if (countA != countB)
                return false;

            var matching = new List<MatchingPair>();

            // Проверим свойства объектов
            foreach (var propertyA in a.GetProperties())
            {
                var aInfo = a.GetProperty(propertyA.Name);
                var aValue = aInfo?.GetValue(this);

                var bInfo = b.GetProperty(propertyA.Name);

                // Если во втором объекте не найдено свойство, которое есть в первом, то он нам не подходит
                if (bInfo == null)
                    return false;

                var bValue = bInfo.GetValue(obj);

                if (aValue == null && bValue != null)
                    return false;
                if (aValue != null && bValue == null)
                    return false;
                if (aValue == null && bValue == null)
                    continue;

                try
                {
                    var aValueC = (IComparable) aValue;
                    var bValueC = (IComparable) bValue;

                    if (aValueC.CompareTo(bValueC) != 0)
                        return false;
                }
                catch (Exception e)
                {
                    var aValueType = aValue.GetType();
                    var bValueType = bValue.GetType();

                    if (aValueType != bValueType)
                        return false;

                    if (aValueType == typeof(string))
                        if (string.CompareOrdinal((string) aValue, (string) bValue) != 0)
                            return false;

                    if (aValueType == typeof(PersonName))
                        if (!aValue.Equals(bValue))
                            return false;

                    if (aValueType == typeof(DateTime))
                        if ((DateTime) aValue != (DateTime) bValue)
                            return false;
                }
            }

            return true;
        }
    }
}