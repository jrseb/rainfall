using System.ComponentModel.DataAnnotations;

namespace RainfallLibrary.Entity
{
	internal class RainfallReadingRaw
	{
		[Display(Description = "Date and time of when measurement was taken")]
		[Required(ErrorMessage = "Date of Measurement is required")]
		public DateTime DateMeasured { get; set; }

		[Display(Description = "Amount of rainfall during the time of measurement")]
		[Range(minimum: 0, maximum: Double.MaxValue)]
		[Required(ErrorMessage = "Rainfall measurement is required")]
		public double AmountMeasured { get; set; }

		[Display(Description = "Station Id where the measurement was taken")]
		[Required(ErrorMessage = "Station Id must not be blank")]
		public string StationId { get; set; }
	}
}