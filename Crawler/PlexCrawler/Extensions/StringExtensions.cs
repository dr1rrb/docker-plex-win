using System;
using System.Linq;

namespace Crawler.Extensions
{
	internal static class StringExtensions
	{
		public static string TrimStart(this string text, string value, StringComparison comparison)
			=> text.StartsWith(value, comparison) 
				? text.Substring(value.Length) 
				: text;

		public static bool IsNullOrWhiteSpace(this string text)
			=> string.IsNullOrWhiteSpace(text);
	}
}