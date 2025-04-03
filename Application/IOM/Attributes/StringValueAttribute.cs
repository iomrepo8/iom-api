using System;

namespace IOM.Attributes
{
    public class StringValueAttribute : Attribute
	{
		private string _value;

		/// <summary>
		/// Creates a new <see cref="StringValueAttribute"/> instance.
		/// </summary>
		/// <param name="value">Value.</param>
		public StringValueAttribute(string value)
		{
			_value = value;
		}

		/// <summary>
		/// Gets the value.
		/// </summary>
		/// <value></value>
		public string Value
		{
			get { return _value; }
		}
	}
}