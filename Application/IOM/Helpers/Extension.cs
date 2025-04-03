using IOM.Attributes;
using IOM.Utilities;
using System;
using System.Collections;
using System.Reflection;

namespace IOM.Helpers
{
    public static class IOMExtensions
    {
        public static string GetStringValue(this EODAction value)
        {
            Hashtable _stringValues = new Hashtable();

            string output = null;
            Type type = value.GetType();

            if (_stringValues.ContainsKey(value))
            {
                output = (_stringValues[value] as StringValueAttribute).Value;
            }
            else
            {
                //Look for our 'StringValueAttribute' in the field's custom attributes
                FieldInfo fi = type.GetField(value.ToString());
                StringValueAttribute[] attrs = fi.GetCustomAttributes(typeof(StringValueAttribute), false) 
                    as StringValueAttribute[];
                if (attrs.Length > 0)
                {
                    _stringValues.Add(value, attrs[0]);
                    output = attrs[0].Value;
                }
            }
            return output;
        }
    }
}