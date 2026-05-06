using System;

namespace MedGyn.MedForce.Common.SharedModels
{
	public class Package
	{
		public decimal Weight { get; set; }
		public int WeightUnits { get; set; }
		public decimal Length { get; set; }
		public decimal Width { get; set; }
		public decimal Height { get; set; }
		public int DimensionUnits { get; set; }
	}
}
