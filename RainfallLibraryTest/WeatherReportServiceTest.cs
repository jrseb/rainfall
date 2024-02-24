using Microsoft.AspNetCore.Http;
using RainfallLibrary.Common;
using RainfallLibrary.Dtos;
using RainfallLibrary.Interfaces;
using RainfallLibrary.Services;

namespace RainfallLibraryTest;

public class Tests
{
	private IWeatherReport _weatherReport;
	private const string TEST_STATION_NAME = @"Station1";

	[SetUp]
	public void Setup()
	{
		_weatherReport = new WeatherReportService();
	}

	//-- 200: test success
	[Test]
	[TestCase(TEST_STATION_NAME, 10, 10, Description = "Code: 200 Must at least have more than 1 record")]
	public void SuccessTest(string stationId, int count, int expectedCount)
	{
		//-- get readings
		var rainReadings = _weatherReport.GetRainfallReading(stationId, count);

		//-- find an null reading
		var rainNullReading = rainReadings?.Where(a => a == default(RainfallReading)).FirstOrDefault();

		//-- find an valid reading
		var rainReading = rainReadings?.FirstOrDefault();

		//-- this is for testing non station1
		Assert.IsTrue(rainReadings?.Count >= expectedCount, "Other station expected Reading list count does not meet test criteria");

		//-- this is for station1 testing
		Assert.IsTrue(rainReadings?.Count == expectedCount && stationId == TEST_STATION_NAME, "Station1 expected Reading list count does not meet test criteria");

		//-- only if we expect records
		if (expectedCount != 0)
		{
			//-- check for null reading
			Assert.IsTrue(rainNullReading == default(RainfallReading), "Reading list content have null values");

			//-- check for content
			Assert.IsFalse(rainReadings?.Any(a => a.DateMeasured.Trim().Length == 0), "Reading list contains invalid DateMeasured");
			Assert.IsFalse(rainReadings?.Any(a => a.AmountMeasured <= 0), "Reading list contains invalid AmountMeasured");
		}
		else
			Assert.Pass();
	}

	//-- 400: Invalid Request
	[Test]
	[TestCase("", 10, Description = "Code: 400 Invalid request due to bad stationId")]
	[TestCase("Station1", -1, Description = "Code: 400 Invalid request due to bad stationId")]
	public void InvalidRequestTest(string stationId, int count)
	{
		//-- get readings
		var error = Assert.Throws<RainfallReadingException>(() => _weatherReport.GetRainfallReading(stationId, count),
			"Service failed to catch INVALID REQUEST due to invalid argument");
		Assert.IsTrue(error.Code == StatusCodes.Status400BadRequest, $"Service failed to send Status Code: 400");
	}

	//-- 404: No readings found
	[Test]
	[TestCase("SomeOtherName", 10, Description = "Code: 404 No readings found for the specified stationId")]
	public void NoReadingTest(string stationId, int count)
	{
		//-- get readings
		var error = Assert.Throws<RainfallReadingException>(() => _weatherReport.GetRainfallReading(stationId, count),
						"Service failed to catch NO RECORDS exception");
		Assert.IsTrue(error.Code == StatusCodes.Status404NotFound, $"Service failed to send Status Code: 404");
	}
}