using Microsoft.AspNetCore.Mvc;
using RainfallLibrary.Common;
using RainfallLibrary.Dtos;
using RainfallLibrary.Interfaces;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace RainfallApi.Controllers
{
	[Route("[controller]/id")]
	[ApiController]
	public class Rainfall : ControllerBase
	{
		private IWeatherReport _weatherService;
		public Rainfall(IWeatherReport service)
		{
			_weatherService = service;
		}

		/// <summary>
		/// Get rainfall readings by station id
		/// </summary>
		/// <param name="stationId">The Id of the reading station</param>
		/// <param name="count">Number of readings to return</param>
		/// <response code="200">A list of rainfall readings successfully retrieved</response>
		/// <response code="400">Invalid request</response>
		/// <response code="404">No readings found for the specified stationId</response>
		/// <response code="500">Internal server error</response>
		[ProducesResponseType(StatusCodes.Status200OK, Type = typeof(List<RainfallReading>))]
		[ProducesResponseType(StatusCodes.Status400BadRequest)]
		[ProducesResponseType(StatusCodes.Status404NotFound)]
		[ProducesResponseType(StatusCodes.Status500InternalServerError)]
		[HttpGet("{stationId}/readings")]
		public IActionResult GetRainfall(string stationId, [FromQuery] int count = 10)
		{
			var ret = default(List<RainfallReading>);
			try
			{
				ret = _weatherService.GetRainfallReading(stationId, count)?.ToList();
			}
			catch (RainfallReadingException e)
			{
				switch (e.Code)
				{
					case StatusCodes.Status404NotFound:
						return NotFound(e.Message);

					case StatusCodes.Status400BadRequest:
						return BadRequest(e.Message);
				}
			}
			catch (Exception e)
			{
				return Problem(e.Message);
			}
			return Ok(ret);
		}
	}
}