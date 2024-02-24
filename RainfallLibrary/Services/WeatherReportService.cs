using Microsoft.AspNetCore.Http;
using RainfallLibrary.Common;
using RainfallLibrary.Dtos;
using RainfallLibrary.Entity;
using RainfallLibrary.Interfaces;
using System.Collections.Immutable;

namespace RainfallLibrary.Services
{
	/// <summary>
	/// Controller that handles external Weather Report service
	/// </summary>
	public class WeatherReportService : IWeatherReport
	{
		private const string SURE_VALID_STATION_ID = "Station1";

		/// <summary>
		/// Fetch the needed rainfall reading from source
		/// </summary>
		/// <param name="stationId">Station to check</param>
		/// <param name="count">number of record to fetch</param>
		/// <returns></returns>
		public ImmutableList<RainfallReading>? GetRainfallReading(string stationId, int count)
		{
			// assuming this gets data from somewhere else and we have to fetch it.  but just doing ordinary list to return
			List<RainfallReadingRaw> rawReading = new List<RainfallReadingRaw>();
			ImmutableList<RainfallReading>? dto = default;

			string dummyId = string.Empty;

			//-- to handle error 400
			if (stationId.Trim().Length == 0) Throw400Error<String>("stationId", stationId);
			if (count <= 0) Throw400Error<int>("count", count);

			for (int ctr = 0; ctr <= (count + 20); ctr++)
			{
				// generate some other station if odd
				dummyId = (ctr % 2) == 0 ? SURE_VALID_STATION_ID : stationId + ctr.ToString().Trim();

				rawReading.Add(new RainfallReadingRaw()
				{
					StationId = dummyId,
					AmountMeasured = ctr + 1,
					DateMeasured = DateTime.Now.AddDays(-ctr)
				});
			}

			//-- only continue if we do have entries for the station
			if (rawReading.Any(a => a.StationId == stationId))
			{
				dto = rawReading.Where(a => a.StationId == stationId)
							.Take(count)
							.Select(a => new RainfallReading()
							{
								DateMeasured = a.DateMeasured.ToString("f"),
								AmountMeasured = a.AmountMeasured
							}).ToImmutableList<RainfallReading>();
			}
			else
			{
				Throw404Error(stationId);
			}
			return dto;
		}

		//-- CUSTOM ERRORS
		/// <summary>
		/// Custom throw procedure to specify correct status code and identify parameter issues
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <param name="paraName"></param>
		/// <param name="value"></param>
		/// <exception cref="RainfallReadingException"></exception>
		private void Throw400Error<T>(string paraName, T? value)
		{
			var message = $"Invalid request: Invalid {paraName} value ({value ?? default(T)})";
			throw new RainfallReadingException(StatusCodes.Status400BadRequest, message);
		}

		private void Throw404Error(string stationId)
		{
			var message = $"No readings found for the specified stationId ({stationId})";
			throw new RainfallReadingException(StatusCodes.Status404NotFound, message);
		}
	}
}