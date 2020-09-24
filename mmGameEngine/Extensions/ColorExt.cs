using System;
using System.Collections.Generic;
using System.Text;
using Raylib_cs;

namespace mmGameEngine
{
	public static class ColorExt
	{
		private const string HEX = "0123456789ABCDEF";


		public static byte HexToByte(char c) => (byte)HEX.IndexOf(char.ToUpper(c));

		public static Color Invert(this Color color) => new Color(255 - color.r, 255 - color.g, 255 - color.b, color.a);

		public static Color HexToColor(string hex)
		{
			float r = (HexToByte(hex[0]) * 16 + HexToByte(hex[1])) / 255.0f;
			float g = (HexToByte(hex[2]) * 16 + HexToByte(hex[3])) / 255.0f;
			float b = (HexToByte(hex[4]) * 16 + HexToByte(hex[5])) / 255.0f;

			return new Color((byte)r, (byte)g, (byte)b, (byte)255);
		}

		public static Color HexToColor(int hex)
		{
			var r = (byte)(hex >> 16);
			var g = (byte)(hex >> 8);
			var b = (byte)(hex >> 0);

			return new Color((byte)r, (byte)g, (byte)b, (byte)255);
		}

		public static Color Create(Color color, int alpha)
		{
			var newColor = new Color();
			//newColor.PackedValue = 0;

			newColor.r = color.r;
			newColor.g = color.g;
			newColor.b = color.b;
			newColor.a = (byte)MathHelper.Clamp(alpha, Byte.MinValue, Byte.MaxValue);
			return newColor;
		}

		public static Color Create(Color color, float alpha)
		{
			var newColor = new Color();
			//newColor.PackedValue = 0;

			newColor.r = color.r;
			newColor.g = color.g;
			newColor.b = color.b;
			newColor.a = (byte)MathHelper.Clamp(alpha * 255, Byte.MinValue, Byte.MaxValue);
			return newColor;
		}

		public static Color Grayscale(this Color color)
		{
			return new Color((int)(color.r * 0.3 + color.g * 0.59 + color.b * 0.11),
				(int)(color.r * 0.3 + color.g * 0.59 + color.b * 0.11),
				(int)(color.r * 0.3 + color.g * 0.59 + color.b * 0.11),
				color.a);
		}

		public static Color Add(this Color color, Color second)
		{
			return new Color(color.r + second.r, color.g + second.g, color.b + second.b, color.a + second.a);
		}

		/// <summary>
		/// first - second
		/// </summary>
		public static Color Subtract(this Color color, Color second)
		{
			return new Color(color.r - second.r, color.g - second.g, color.b - second.b, color.a - second.a);
		}

		public static Color Multiply(this Color self, Color second)
		{
			return new Color
			{
				r = (byte)(self.r * second.r / 255),
				g = (byte)(self.g * second.g / 255),
				b = (byte)(self.b * second.b / 255),
				a = (byte)(self.a * second.a / 255)
			};
		}

		/// <summary>
		/// linearly interpolates Color from - to
		/// </summary>
		/// <param name="from">From.</param>
		/// <param name="to">To.</param>
		/// <param name="t">T.</param>
		public static Color Lerp(Color from, Color to, float t)
		{
			var t255 = (int)(t * 255);
			return new Color(from.r + (to.r - from.r) * t255 / 255, from.g + (to.g - from.g) * t255 / 255,
				from.b + (to.b - from.b) * t255 / 255, from.a + (to.a - from.a) * t255 / 255);
		}

		/// <summary>
		/// linearly interpolates Color from - to
		/// </summary>
		/// <param name="from">From.</param>
		/// <param name="to">To.</param>
		/// <param name="t">T.</param>
		public static void Lerp(ref Color from, ref Color to, out Color result, float t)
		{
			result = new Color();
			var t255 = (int)(t * 255);
			result.r = (byte)(from.r + (to.r - from.r) * t255 / 255);
			result.g = (byte)(from.g + (to.g - from.g) * t255 / 255);
			result.b = (byte)(from.b + (to.b - from.b) * t255 / 255);
			result.a = (byte)(from.a + (to.a - from.a) * t255 / 255);
		}

		public static (float, float, float) RgbToHsl(Color color)
		{
			var h = 0f;
			var s = 0f;
			var l = 0f;

			var r = color.r / 255f;
			var g = color.g / 255f;
			var b = color.b / 255f;
			var min = MathHelper.Min(MathHelper.Min(r, g), b);
			var max = MathHelper.Max(MathHelper.Max(r, g), b);
			var delta = max - min;

			// luminance is the ave of max and min
			l = (max + min) / 2f;

			if (delta > 0)
			{
				if (l < 0.5f)
					s = delta / (max + min);
				else
					s = delta / (2 - max - min);

				var deltaR = (((max - r) / 6f) + (delta / 2f)) / delta;
				var deltaG = (((max - g) / 6f) + (delta / 2f)) / delta;
				var deltaB = (((max - b) / 6f) + (delta / 2f)) / delta;

				if (r == max)
					h = deltaB - deltaG;
				else if (g == max)
					h = (1f / 3f) + deltaR - deltaB;
				else if (b == max)
					h = (2f / 3f) + deltaG - deltaR;

				if (h < 0)
					h += 1;

				if (h > 1)
					h -= 1;
			}

			return (h, s, l);
		}

		public static Color HslToRgb(float h, float s, float l)
		{
			float HueToRgb(float v1, float v2, float vH)
			{
				vH += (vH < 0) ? 1 : 0;
				vH -= (vH > 1) ? 1 : 0;
				var ret = v1;

				if ((6 * vH) < 1)
					ret = (v1 + (v2 - v1) * 6 * vH);
				else if ((2 * vH) < 1)
					ret = v2;

				else if ((3 * vH) < 2)
					ret = v1 + (v2 - v1) * ((2f / 3f) - vH) * 6f;

				return Mathf.Clamp01(ret);
			}

			var c = new Color();
			c.a = 255;

			if (s == 0)
			{
				c.r = (byte)(l * 255f);
				c.g = (byte)(l * 255f);
				c.b = (byte)(l * 255f);
			}
			else
			{
				var v2 = (l + s) - (s * l);
				if (l < 0.5f)
					v2 = l * (1 + s);

				var v1 = 2f * l - v2;

				c.r = (byte)(255f * HueToRgb(v1, v2, h + (1f / 3f)));
				c.g = (byte)(255f * HueToRgb(v1, v2, h));
				c.b = (byte)(255f * HueToRgb(v1, v2, h - (1f / 3f)));
			}

			return c;
		}
	}
}
