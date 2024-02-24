namespace RainfallLibrary.Common
{
	/// <summary>
	/// Custom Error code to make things simple to identify in api
	/// </summary>
	/// <param name="statusCode">Http valid status code</param>
	/// <param name="message">Message you want to send</param>
	public class RainfallReadingException(int statusCode, string message) : Exception(message)
	{
		/// <summary>
		/// HTTP status code to return
		/// </summary>
		public int Code = statusCode;
	}
}