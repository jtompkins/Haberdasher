using System;
using System.Data.Entity.Design.PluralizationServices;
using System.Globalization;

namespace Haberdasher.Support.Helpers
{
	public static class PluralizationHelper
	{
		public static string Pluralize(string name) {
			if (String.IsNullOrEmpty(name))
				return null;

			var service = PluralizationService.CreateService(new CultureInfo("en-US"));

			return service.IsPlural(name) 
				? name 
				: service.Pluralize(name);
		}
	}
}
