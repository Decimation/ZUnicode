using System.Globalization;
using System.Linq;
using System.Reflection;
using System.Text.Unicode;
using Kantan.Collections;
using Novus.Utilities;

namespace ZUnicode;
#nullable disable
public static class Program
{
	static T get<T>(this IList<T> rg, in int i)
	{
		if (rg.IsIndexWithinBounds(i)) {
			return rg[i];
		}

		return default;
	}

	static T2 get<T, T2>(this IList<T> rg, int i, Func<T> getDefault = null,
	                     Func<T, T2> conv = null) /*where T2 : IParsable<T2>*/
	{
		if (rg.IsIndexWithinBounds(i)) {
			if (typeof(T2) == typeof(IParsable<>) && typeof(T) == typeof(string)) {
				var fx = typeof(T2).GetMethod("Parse", BindingFlags.Public | BindingFlags.Static,
				                              types: new[] { typeof(string), typeof(IFormatProvider) });

				conv ??= (x) => (T2) fx.Invoke((string) (object) x, new Object[] { CultureInfo.InvariantCulture });
			}
			else { }

			getDefault = () => (T) (object) rg[i];
		}
		else {
			getDefault ??= () => default;
		}

		conv ??= (x) => (T2) (object) x;

		return conv(getDefault());
	}

	static T2 get<T2>(this IList<string> rg, int i, Func<string> getDefault,
	                  Func<string, T2> conv = null) /*where T2 : IParsable<T2>*/
	{
		var fx = typeof(T2).GetMethod("Parse", BindingFlags.Public | BindingFlags.Static,
		                              types: new[] { typeof(string), typeof(IFormatProvider) });

		if (fx!= null) {
			
			conv ??= (x) => (T2) fx.Invoke(null, new Object[] {x, CultureInfo.InvariantCulture });
		}
		else {
			conv ??= (x) => (T2) (object) x;

		}

		string inp = default;

		if (rg.IsIndexWithinBounds(i)) {
			inp = rg[i];
		}
		else {
			inp = getDefault();
		}

		return conv(inp);
	}

	static void Main(string[] args)
	{
		var ranges = new[]
		{
			UnicodeRanges.CombiningDiacriticalMarks,
			UnicodeRanges.CombiningDiacriticalMarksExtended,
			UnicodeRanges.CombiningDiacriticalMarksSupplement,
			UnicodeRanges.CombiningDiacriticalMarksforSymbols

		};

		string s;
		int    add = 10;

		s = args.get<string>(0, getDefault: () => Console.ReadLine());

		add = args.get<int>(1, () => Console.ReadLine());
		Console.WriteLine($"{s} {add}");
		static char[] rg(UnicodeRange r)
		{
			var rg = new char[r.Length];

			for (int i = 0; i < r.Length; i++) {
				rg[i] = (char) (r.FirstCodePoint + i);
			}

			return rg;
		}

		for (int j = 0; j < ranges.Length; j++) {
			UnicodeRange r   = ranges[j];
			var          rg2 = new char[r.Length];

			for (int i = 0; i < r.Length; i++) {
				rg2[i] = (char) (r.FirstCodePoint + i);
			}

			var chars = rg2.TakeRandom(Random.Shared.Next(0, 20)).ToArray();
			var rg1   = new string(chars, 0, chars.Length);
			Console.WriteLine(rg1);
		}
	}
}