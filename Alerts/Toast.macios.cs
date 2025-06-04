using CoreGraphics;
using System.Runtime.InteropServices;
using UIKit;
using Microsoft.Maui.Platform;

namespace MauiCommonTools.Alerts;
public partial class Toast : IToast
{
	internal static Color DefaultBackgroundColor = Colors.LightGray;
	internal static Color DefaultTextColor = Colors.Black;
	internal const double DefaultCharacterSpacing = 0.0d;

	static PlatformToast? PlatformToast { get; set; }

	/// <summary>
	/// Dispose Toast
	/// </summary>
	protected virtual void Dispose(bool isDisposing)
	{
		if (isDisposed)
		{
			return;
		}

		if (isDisposing)
		{
			PlatformToast?.Dispose();
		}

		isDisposed = true;
	}

	static void DismissPlatform(CancellationToken token)
	{
		if (PlatformToast is null)
		{
			return;
		}

		token.ThrowIfCancellationRequested();
		PlatformToast.Dismiss();
	}

	/// <summary>
	/// Show Toast
	/// </summary>
	void ShowPlatform(CancellationToken token)
	{
		DismissPlatform(token);
		token.ThrowIfCancellationRequested();

		var cornerRadius = CreateCornerRadius();
		NFloat[] nums = [cornerRadius.X, cornerRadius.Y, cornerRadius.Width, cornerRadius.Height];
		var padding = nums.Max();

		PlatformToast = new PlatformToast(Text,
			DefaultBackgroundColor.ToPlatform(),
			cornerRadius,
			DefaultTextColor.ToPlatform(),
			UIFont.SystemFontOfSize((NFloat)TextSize),
			DefaultCharacterSpacing,
			padding)
		{
			Duration = GetDuration(Duration)
		};

		PlatformToast.Show();
	}

	static CGRect CreateCornerRadius(int radius = 4)
	{
		return new CGRect(radius, radius, radius, radius);
	}
}
