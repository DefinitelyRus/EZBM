using System.Runtime.CompilerServices;

namespace Backend.Core.Common;

public static class Esc
{
	public static T New<T>(
		string? message,
		object? value = null,
		[CallerArgumentExpression(nameof(value))] string? key = null
		) where T : Exception
	{
		T exception = (T)Activator.CreateInstance(typeof(T), message)!;

		if (!string.IsNullOrEmpty(key)) exception.Data[key] = value;

		return exception;
	}

	public static T New<T>(string? message, Dictionary<string, object?> data) where T : Exception
	{
		T exception = (T)Activator.CreateInstance(typeof(T), message)!;

		if (data != null && data.Count > 0)
		{
			foreach (KeyValuePair<string, object?> pair in data)
			{
				exception.Data[pair.Key] = pair.Value;
			}
		}

		return exception;
	}

	/// <summary>
	/// Adds data to an <see cref="Exception"/> object. 
	/// </summary>
	/// <typeparam name="T">The <see cref="Exception"/> subtype.</typeparam>
	/// <param name="exception">The <see cref="Exception"/> instance.</param>
	/// <param name="key">The string used to identify the value.
	/// Usually uses the name of its value source.</param>
	/// <param name="value"></param>
	/// <returns></returns>
	public static T AddData<T>(
		this T exception,
		object? value,
		[CallerArgumentExpression(nameof(value))] string key = ""
		) where T : Exception
	{
		exception.Data[key] = value;
		return exception;
	}

	/// <summary>
	/// Return the full formatted message and attaches all the data into the exception
	/// message.
	/// </summary>
	/// <param name="exception"></param>
	/// <returns></returns>
	public static string ExportMessage(this Exception exception, string messageSeparator = " || ", string infoSeparator = " ")
	{
		if (exception.Data.Count == 0)
		{
			return exception.Message;
		}

		List<string> entries = [..exception.Data.Keys
			.Cast<object>()
			.Select(key => $"{key}={exception.Data[key]}")];

		string info = string.Join(infoSeparator, entries);

		return $"{exception.Message}{messageSeparator}{info}";
	}
}