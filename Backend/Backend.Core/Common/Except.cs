namespace Backend.Core.Common;

public static class Except
{
	public static T Me<T>(string message) where T : Exception
	{
		return null;
	}
}

public class ExceptionWrapper<T> where T : Exception
{

}