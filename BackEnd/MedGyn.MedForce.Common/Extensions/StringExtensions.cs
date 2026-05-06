using System;

public static class StringExtensions
{
	/// <summary>
	/// Converts a string value into an int32 safely.
	/// </summary>
	/// <param name="value">Value to be converted</param>
	/// <returns>The integer representation of the string, or 0 if the string is not an integer</returns>
	public static int ToInt(this string value)
	{
		int result = 0;

		Int32.TryParse(value, out result);

		return result;
	}

	public static bool IsNullOrEmpty(this string value)
	{
		return string.IsNullOrEmpty(value?.Trim() ?? "");
	}
}

