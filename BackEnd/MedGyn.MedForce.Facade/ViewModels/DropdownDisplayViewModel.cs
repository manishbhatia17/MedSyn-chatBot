using System.Collections.Generic;
using System.Linq;
using MedGyn.MedForce.Service.Contracts;

namespace MedGyn.MedForce.Facade.ViewModels
{
	public class DropdownDisplayViewModel
	{
		public DropdownDisplayViewModel(int value, string text, string altID = null, bool visible = true)
		{
			Value   = value;
			Text    = text;
			AltID   = altID;
			Visible = visible;
		}

		public int Value { get; set; }
		public string Text { get; set; }
		public string AltID { get; set; }
		public bool Visible { get; set; }
		public object Data { get; set; }
	}

	public static class DropdownHelpers
	{
		public static List<DropdownDisplayViewModel> ToDropdownList(this IEnumerable<CodeContract> list)
		{
			return list.Select(c => new DropdownDisplayViewModel(c.CodeID, c.CodeDescription, c.CodeName, !c.IsDeleted))
				.OrderBy(c => c.Text).ToList();
		}

		public static List<DropdownDisplayViewModel> ToDropdownList(this IEnumerable<VendorContract> list)
		{
			return list.Select(v => new DropdownDisplayViewModel(v.VendorID, $"{v.VendorCustomID} {v.VendorName}"))
				.OrderBy(v => v.Text).ToList();
		}
	}
}


