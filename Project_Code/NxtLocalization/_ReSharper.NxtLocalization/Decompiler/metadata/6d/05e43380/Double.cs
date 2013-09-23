// Type: System.Double
// Assembly: mscorlib, Version=2.0.5.0, Culture=neutral, PublicKeyToken=7cec85d7bea7798e
// Assembly location: C:\Program Files (x86)\Reference Assemblies\Microsoft\Framework\Silverlight\v4.0\Profile\WindowsPhone71\mscorlib.dll

using System.Globalization;
using System.Security;

namespace System
{
  /// <summary>
  /// Represents a double-precision floating-point number.
  /// </summary>
  public struct Double : IComparable, IFormattable, IConvertible, IComparable<double>, IEquatable<double>
  {
    /// <summary>
    /// Represents the smallest possible value of a <see cref="T:System.Double"/>. This field is constant.
    /// </summary>
    public const double MinValue = -1.7976931348623157E+308;
    /// <summary>
    /// Represents the largest possible value of a <see cref="T:System.Double"/>. This field is constant.
    /// </summary>
    public const double MaxValue = 1.7976931348623157E+308;
    /// <summary>
    /// Represents the smallest positive <see cref="T:System.Double"/> value greater than zero. This field is constant.
    /// </summary>
    public const double Epsilon = 4.94065645841247E-324;
    /// <summary>
    /// Represents negative infinity. This field is constant.
    /// </summary>
    public const double NegativeInfinity = double.NegativeInfinity;
    /// <summary>
    /// Represents positive infinity. This field is constant.
    /// </summary>
    public const double PositiveInfinity = double.PositiveInfinity;
    /// <summary>
    /// Represents a value that is not a number (NaN). This field is constant.
    /// </summary>
    public const double NaN = double.NaN;
    /// <summary>
    /// Returns a value indicating whether the specified number evaluates to negative or positive infinity
    /// </summary>
    /// 
    /// <returns>
    /// true if <paramref name="d"/> evaluates to <see cref="F:System.Double.PositiveInfinity"/> or <see cref="F:System.Double.NegativeInfinity"/>; otherwise, false.
    /// </returns>
    /// <param name="d">A double-precision floating-point number. </param>
    [SecuritySafeCritical]
    public static bool IsInfinity(double d);
    /// <summary>
    /// Returns a value indicating whether the specified number evaluates to positive infinity.
    /// </summary>
    /// 
    /// <returns>
    /// true if <paramref name="d"/> evaluates to <see cref="F:System.Double.PositiveInfinity"/>; otherwise, false.
    /// </returns>
    /// <param name="d">A double-precision floating-point number. </param>
    [SecuritySafeCritical]
    public static bool IsPositiveInfinity(double d);
    /// <summary>
    /// Returns a value indicating whether the specified number evaluates to negative infinity.
    /// </summary>
    /// 
    /// <returns>
    /// true if <paramref name="d"/> evaluates to <see cref="F:System.Double.NegativeInfinity"/>; otherwise, false.
    /// </returns>
    /// <param name="d">A double-precision floating-point number. </param>
    [SecuritySafeCritical]
    public static bool IsNegativeInfinity(double d);
    /// <summary>
    /// Returns a value indicating whether the specified number evaluates to a value that is not a number (<see cref="F:System.Double.NaN"/>).
    /// </summary>
    /// 
    /// <returns>
    /// true if <paramref name="d"/> evaluates to <see cref="F:System.Double.NaN"/>; otherwise, false.
    /// </returns>
    /// <param name="d">A double-precision floating-point number. </param>
    public static bool IsNaN(double d);
    /// <summary>
    /// Compares this instance to a specified object and returns an integer that indicates whether the value of this instance is less than, equal to, or greater than the value of the specified object.
    /// </summary>
    /// 
    /// <returns>
    /// A signed number indicating the relative values of this instance and <paramref name="value"/>.Value Description A negative integer This instance is less than <paramref name="value"/>.-or- This instance is not a number (<see cref="F:System.Double.NaN"/>) and <paramref name="value"/> is a number. Zero This instance is equal to <paramref name="value"/>.-or- This instance and <paramref name="value"/> are both Double.NaN, <see cref="F:System.Double.PositiveInfinity"/>, or <see cref="F:System.Double.NegativeInfinity"/>A positive integer This instance is greater than <paramref name="value"/>.-or- This instance is a number and <paramref name="value"/> is not a number (<see cref="F:System.Double.NaN"/>).-or- <paramref name="value"/> is null.
    /// </returns>
    /// <param name="value">An object to compare, or null. </param><exception cref="T:System.ArgumentException"><paramref name="value"/> is not a <see cref="T:System.Double"/>. </exception>
    public int CompareTo(object value);
    /// <summary>
    /// Compares this instance to a specified double-precision floating-point number and returns an integer that indicates whether the value of this instance is less than, equal to, or greater than the value of the specified double-precision floating-point number.
    /// </summary>
    /// 
    /// <returns>
    /// A signed number indicating the relative values of this instance and <paramref name="value"/>.Return Value Description Less than zero This instance is less than <paramref name="value"/>.-or- This instance is not a number (<see cref="F:System.Double.NaN"/>) and <paramref name="value"/> is a number. Zero This instance is equal to <paramref name="value"/>.-or- Both this instance and <paramref name="value"/> are not a number (<see cref="F:System.Double.NaN"/>), <see cref="F:System.Double.PositiveInfinity"/>, or <see cref="F:System.Double.NegativeInfinity"/>. Greater than zero This instance is greater than <paramref name="value"/>.-or- This instance is a number and <paramref name="value"/> is not a number (<see cref="F:System.Double.NaN"/>).
    /// </returns>
    /// <param name="value">A double-precision floating-point number to compare. </param>
    public int CompareTo(double value);
    /// <summary>
    /// Returns a value indicating whether this instance is equal to a specified object.
    /// </summary>
    /// 
    /// <returns>
    /// true if <paramref name="obj"/> is an instance of <see cref="T:System.Double"/> and equals the value of this instance; otherwise, false.
    /// </returns>
    /// <param name="obj">An object to compare with this instance. </param>
    public override bool Equals(object obj);
    /// <summary>
    /// Returns a value indicating whether this instance and a specified <see cref="T:System.Double"/> object represent the same value.
    /// </summary>
    /// 
    /// <returns>
    /// true if <paramref name="obj"/> is equal to this instance; otherwise, false.
    /// </returns>
    /// <param name="obj">A <see cref="T:System.Double"/> object to compare to this instance.</param>
    public bool Equals(double obj);
    /// <summary>
    /// Returns the hash code for this instance.
    /// </summary>
    /// 
    /// <returns>
    /// A 32-bit signed integer hash code.
    /// </returns>
    [SecuritySafeCritical]
    public override int GetHashCode();
    /// <summary>
    /// Converts the numeric value of this instance to its equivalent string representation.
    /// </summary>
    /// 
    /// <returns>
    /// The string representation of the value of this instance.
    /// </returns>
    public override string ToString();
    /// <summary>
    /// Converts the numeric value of this instance to its equivalent string representation, using the specified format.
    /// </summary>
    /// 
    /// <returns>
    /// The string representation of the value of this instance as specified by <paramref name="format"/>.
    /// </returns>
    /// <param name="format">A standard or custom numeric format string (see Remarks).</param><exception cref="T:System.FormatException"><paramref name="format"/> is invalid. </exception>
    public string ToString(string format);
    /// <summary>
    /// Converts the numeric value of this instance to its equivalent string representation using the specified format and culture-specific format information.
    /// </summary>
    /// 
    /// <returns>
    /// The string representation of the value of this instance as specified by <paramref name="format"/> and <paramref name="provider"/>.
    /// </returns>
    /// <param name="format">A standard or custom numeric format string (see Remarks).</param><param name="provider">An <see cref="T:System.IFormatProvider"/> that supplies culture-specific formatting information. </param>
    [SecuritySafeCritical]
    public string ToString(string format, IFormatProvider provider);
    /// <summary>
    /// Converts the string representation of a number to its double-precision floating-point number equivalent.
    /// </summary>
    /// 
    /// <returns>
    /// A double-precision floating-point number that is equivalent to the numeric value or symbol specified in <paramref name="s"/>.
    /// </returns>
    /// <param name="s">A string that contains a number to convert. </param><exception cref="T:System.ArgumentNullException"><paramref name="s"/> is null. </exception><exception cref="T:System.FormatException"><paramref name="s"/> does not represent a number in a valid format. </exception><exception cref="T:System.OverflowException"><paramref name="s"/> represents a number that is less than <see cref="F:System.Double.MinValue"/> or greater than <see cref="F:System.Double.MaxValue"/>. </exception>
    public static double Parse(string s);
    /// <summary>
    /// Converts the string representation of a number in a specified style to its double-precision floating-point number equivalent.
    /// </summary>
    /// 
    /// <returns>
    /// A double-precision floating-point number that is equivalent to the numeric value or symbol specified in <paramref name="s"/>.
    /// </returns>
    /// <param name="s">A string that contains a number to convert. </param><param name="style">A bitwise combination of enumeration values that indicate the style elements that can be present in <paramref name="s"/>. A typical value to specify is a combination of <see cref="F:System.Globalization.NumberStyles.Float"/> combined with <see cref="F:System.Globalization.NumberStyles.AllowThousands"/>.</param><exception cref="T:System.ArgumentNullException"><paramref name="s"/> is null. </exception><exception cref="T:System.FormatException"><paramref name="s"/> does not represent a number in a valid format. </exception><exception cref="T:System.OverflowException"><paramref name="s"/> represents a number that is less than <see cref="F:System.Double.MinValue"/> or greater than <see cref="F:System.Double.MaxValue"/>. </exception><exception cref="T:System.ArgumentException"><paramref name="style"/> is not a <see cref="T:System.Globalization.NumberStyles"/> value. -or-<paramref name="style"/> includes the <see cref="F:System.Globalization.NumberStyles.AllowHexSpecifier"/> value. </exception>
    public static double Parse(string s, NumberStyles style);
    /// <summary>
    /// Converts the string representation of a number in a specified culture-specific format to its double-precision floating-point number equivalent.
    /// </summary>
    /// 
    /// <returns>
    /// A double-precision floating-point number that is equivalent to the numeric value or symbol specified in <paramref name="s"/>.
    /// </returns>
    /// <param name="s">A string that contains a number to convert. </param><param name="provider">An object that supplies culture-specific formatting information about <paramref name="s"/>. </param><exception cref="T:System.ArgumentNullException"><paramref name="s"/> is null. </exception><exception cref="T:System.FormatException"><paramref name="s"/> does not represent a number in a valid format. </exception><exception cref="T:System.OverflowException"><paramref name="s"/> represents a number that is less than <see cref="F:System.Double.MinValue"/> or greater than <see cref="F:System.Double.MaxValue"/>. </exception>
    public static double Parse(string s, IFormatProvider provider);
    /// <summary>
    /// Converts the string representation of a number in a specified style and culture-specific format to its double-precision floating-point number equivalent.
    /// </summary>
    /// 
    /// <returns>
    /// A double-precision floating-point number that is equivalent to the numeric value or symbol specified in <paramref name="s"/>.
    /// </returns>
    /// <param name="s">A string that contains a number to convert. </param><param name="style">A bitwise combination of enumeration values that indicate the style elements that can be present in <paramref name="s"/>. A typical value to specify is <see cref="F:System.Globalization.NumberStyles.Float"/> combined with <see cref="F:System.Globalization.NumberStyles.AllowThousands"/>.</param><param name="provider">An object that supplies culture-specific formatting information about <paramref name="s"/>. </param><exception cref="T:System.ArgumentNullException"><paramref name="s"/> is null. </exception><exception cref="T:System.FormatException"><paramref name="s"/> does not represent a numeric value. </exception><exception cref="T:System.ArgumentException"><paramref name="style"/> is not a <see cref="T:System.Globalization.NumberStyles"/> value. -or-<paramref name="style"/> is the <see cref="F:System.Globalization.NumberStyles.AllowHexSpecifier"/> value.</exception><exception cref="T:System.OverflowException"><paramref name="s"/> represents a number that is less than <see cref="F:System.Double.MinValue"/> or greater than <see cref="F:System.Double.MaxValue"/>. </exception>
    [SecuritySafeCritical]
    public static double Parse(string s, NumberStyles style, IFormatProvider provider);
    /// <summary>
    /// Converts the numeric value of this instance to its equivalent string representation using the specified culture-specific format information.
    /// </summary>
    /// 
    /// <returns>
    /// The string representation of the value of this instance as specified by <paramref name="provider"/>.
    /// </returns>
    /// <param name="provider">An <see cref="T:System.IFormatProvider"/> that supplies culture-specific formatting information. </param>
    public string ToString(IFormatProvider provider);
    /// <summary>
    /// Returns the <see cref="T:System.TypeCode"/> for value type <see cref="T:System.Double"/>.
    /// </summary>
    /// 
    /// <returns>
    /// The enumerated constant, <see cref="F:System.TypeCode.Double"/>.
    /// </returns>
    public TypeCode GetTypeCode();
    /// <summary>
    /// For a description of this member, see <see cref="M:System.IConvertible.ToBoolean(System.IFormatProvider)"/>.
    /// </summary>
    /// 
    /// <returns>
    /// true if the value of the current instance is not zero; otherwise, false.
    /// </returns>
    /// <param name="provider">This parameter is ignored.</param>
    bool IConvertible.ToBoolean(IFormatProvider provider);
    /// <summary>
    /// This conversion is not supported. Attempting to use this method throws an <see cref="T:System.InvalidCastException"/>.
    /// </summary>
    /// 
    /// <returns>
    /// This conversion is not supported. No value is returned.
    /// </returns>
    /// <param name="provider">This parameter is ignored.</param><exception cref="T:System.InvalidCastException">In all cases.</exception>
    char IConvertible.ToChar(IFormatProvider provider);
    /// <summary>
    /// For a description of this member, see <see cref="M:System.IConvertible.ToSByte(System.IFormatProvider)"/>.
    /// </summary>
    /// 
    /// <returns>
    /// The value of the current instance, converted to an <see cref="T:System.SByte"/>.
    /// </returns>
    /// <param name="provider">This parameter is ignored.</param>
    sbyte IConvertible.ToSByte(IFormatProvider provider);
    /// <summary>
    /// For a description of this member, see <see cref="M:System.IConvertible.ToByte(System.IFormatProvider)"/>.
    /// </summary>
    /// 
    /// <returns>
    /// The value of the current instance, converted to a <see cref="T:System.Byte"/>.
    /// </returns>
    /// <param name="provider">This parameter is ignored.</param>
    byte IConvertible.ToByte(IFormatProvider provider);
    /// <summary>
    /// For a description of this member, see <see cref="M:System.IConvertible.ToInt16(System.IFormatProvider)"/>.
    /// </summary>
    /// 
    /// <returns>
    /// The value of the current instance, converted to an <see cref="T:System.Int16"/>.
    /// </returns>
    /// <param name="provider">This parameter is ignored.</param>
    short IConvertible.ToInt16(IFormatProvider provider);
    /// <summary>
    /// For a description of this member, see <see cref="M:System.IConvertible.ToUInt16(System.IFormatProvider)"/>.
    /// </summary>
    /// 
    /// <returns>
    /// The value of the current instance, converted to a <see cref="T:System.UInt16"/>.
    /// </returns>
    /// <param name="provider">This parameter is ignored. </param>
    ushort IConvertible.ToUInt16(IFormatProvider provider);
    /// <summary>
    /// For a description of this member, see <see cref="M:System.IConvertible.ToInt32(System.IFormatProvider)"/>.
    /// </summary>
    /// 
    /// <returns>
    /// The value of the current instance, converted to an <see cref="T:System.Int32"/>.
    /// </returns>
    /// <param name="provider">This parameter is ignored.</param>
    int IConvertible.ToInt32(IFormatProvider provider);
    /// <summary>
    /// For a description of this member, see <see cref="M:System.IConvertible.ToUInt32(System.IFormatProvider)"/>.
    /// </summary>
    /// 
    /// <returns>
    /// The value of the current instance, converted to a <see cref="T:System.UInt32"/>.
    /// </returns>
    /// <param name="provider">This parameter is ignored.   </param>
    uint IConvertible.ToUInt32(IFormatProvider provider);
    /// <summary>
    /// For a description of this member, see <see cref="M:System.IConvertible.ToInt64(System.IFormatProvider)"/>.
    /// </summary>
    /// 
    /// <returns>
    /// The value of the current instance, converted to an <see cref="T:System.Int64"/>.
    /// </returns>
    /// <param name="provider">This parameter is ignored.</param>
    long IConvertible.ToInt64(IFormatProvider provider);
    /// <summary>
    /// For a description of this member, see <see cref="M:System.IConvertible.ToUInt64(System.IFormatProvider)"/>.
    /// </summary>
    /// 
    /// <returns>
    /// The value of the current instance, converted to a <see cref="T:System.UInt64"/>.
    /// </returns>
    /// <param name="provider">This parameter is ignored.</param>
    ulong IConvertible.ToUInt64(IFormatProvider provider);
    /// <summary>
    /// For a description of this member, see <see cref="M:System.IConvertible.ToSingle(System.IFormatProvider)"/>.
    /// </summary>
    /// 
    /// <returns>
    /// The value of the current instance, converted to a <see cref="T:System.Single"/>.
    /// </returns>
    /// <param name="provider">This parameter is ignored.</param>
    float IConvertible.ToSingle(IFormatProvider provider);
    /// <summary>
    /// For a description of this member, see <see cref="M:System.IConvertible.ToDouble(System.IFormatProvider)"/>.
    /// </summary>
    /// 
    /// <returns>
    /// The value of the current instance, unchanged.
    /// </returns>
    /// <param name="provider">This parameter is ignored.</param>
    double IConvertible.ToDouble(IFormatProvider provider);
    /// <summary>
    /// For a description of this member, see <see cref="M:System.IConvertible.ToDecimal(System.IFormatProvider)"/>.
    /// </summary>
    /// 
    /// <returns>
    /// The value of the current instance, converted to a <see cref="T:System.Decimal"/>.
    /// </returns>
    /// <param name="provider">This parameter is ignored.</param>
    decimal IConvertible.ToDecimal(IFormatProvider provider);
    /// <summary>
    /// This conversion is not supported. Attempting to use this method throws an <see cref="T:System.InvalidCastException"/>
    /// </summary>
    /// 
    /// <returns>
    /// This conversion is not supported. No value is returned.
    /// </returns>
    /// <param name="provider">This parameter is ignored.</param><exception cref="T:System.InvalidCastException">In all cases.</exception>
    DateTime IConvertible.ToDateTime(IFormatProvider provider);
    /// <summary>
    /// For a description of this member, see <see cref="M:System.IConvertible.ToType(System.Type,System.IFormatProvider)"/>.
    /// </summary>
    /// 
    /// <returns>
    /// The value of the current instance, converted to <paramref name="type"/>.
    /// </returns>
    /// <param name="type">The type to which to convert this <see cref="T:System.Double"/> value.</param><param name="provider">An <see cref="T:System.IFormatProvider"/> implementation that supplies culture-specific information about the format of the returned value.</param>
    object IConvertible.ToType(Type type, IFormatProvider provider);
    /// <summary>
    /// Converts the string representation of a number to its double-precision floating-point number equivalent. A return value indicates whether the conversion succeeded or failed.
    /// </summary>
    /// 
    /// <returns>
    /// true if <paramref name="s"/> was converted successfully; otherwise, false.
    /// </returns>
    /// <param name="s">A string containing a number to convert. </param><param name="result">When this method returns, contains the double-precision floating-point number equivalent to the <paramref name="s"/> parameter, if the conversion succeeded, or zero if the conversion failed. The conversion fails if the <paramref name="s"/> parameter is null, is not a number in a valid format, or represents a number less than <see cref="F:System.Double.MinValue"/> or greater than <see cref="F:System.Double.MaxValue"/>. This parameter is passed uninitialized. </param>
    [SecuritySafeCritical]
    public static bool TryParse(string s, out double result);
    /// <summary>
    /// Converts the string representation of a number in a specified style and culture-specific format to its double-precision floating-point number equivalent. A return value indicates whether the conversion succeeded or failed.
    /// </summary>
    /// 
    /// <returns>
    /// true if <paramref name="s"/> was converted successfully; otherwise, false.
    /// </returns>
    /// <param name="s">A string containing a number to convert. </param><param name="style">A bitwise combination of <see cref="T:System.Globalization.NumberStyles"/> values that indicates the permitted format of <paramref name="s"/>. A typical value to specify is <see cref="F:System.Globalization.NumberStyles.Float"/> combined with <see cref="F:System.Globalization.NumberStyles.AllowThousands"/>.</param><param name="provider">An <see cref="T:System.IFormatProvider"/> that supplies culture-specific formatting information about <paramref name="s"/>. </param><param name="result">When this method returns, contains a double-precision floating-point number equivalent to the numeric value or symbol contained in <paramref name="s"/>, if the conversion succeeded, or zero if the conversion failed. The conversion fails if the <paramref name="s"/> parameter is null, is not in a format compliant with <paramref name="style"/>, represents a number less than <see cref="F:System.SByte.MinValue"/> or greater than <see cref="F:System.SByte.MaxValue"/>, or if <paramref name="style"/> is not a valid combination of <see cref="T:System.Globalization.NumberStyles"/> enumerated constants. This parameter is passed uninitialized. </param><exception cref="T:System.ArgumentException"><paramref name="style"/> is not a <see cref="T:System.Globalization.NumberStyles"/> value. -or-<paramref name="style"/> includes the <see cref="F:System.Globalization.NumberStyles.AllowHexSpecifier"/> value.</exception>
    [SecuritySafeCritical]
    public static bool TryParse(string s, NumberStyles style, IFormatProvider provider, out double result);
  }
}
