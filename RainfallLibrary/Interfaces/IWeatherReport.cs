using RainfallLibrary.Dtos;
using System.Collections.Immutable;

namespace RainfallLibrary.Interfaces
{
	/// <summary>
	/// Calls that can be made to an assume external source
	/// </summary>
	public interface IWeatherReport
	{
		/// <summary>
		/// Fetch the rainfall reading from external service
		/// </summary>
		/// <param name="stationId">StationId to fetch</param>
		/// <param name="count">No of records to return</param>
		/// <returns></returns>
		public ImmutableList<RainfallReading>? GetRainfallReading(string stationId, int count);
	}
}