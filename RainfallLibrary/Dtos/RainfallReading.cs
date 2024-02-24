namespace RainfallLibrary.Dtos
{
	/// <summary>
	/// Datails of rainfall readings
	/// </summary>
	public class RainfallReading
	{
		/// <summary>
		/// Date and time of when measurement was taken
		/// </summary>
		public string DateMeasured { get; set; }

		/// <summary>
		/// Amount of rainfall during the time of measurement
		/// </summary>
		public double AmountMeasured { get; set; }
	}
}