﻿using System.ComponentModel;

namespace MauiCommonTools.Alerts;
public partial class Toast : IToast
{
	public const double DefaultFontSize = 14;

	bool isDisposed;

	public string Text { get; set; } = string.Empty;

	public ToastDuration Duration { get; set; } = ToastDuration.Short;

	public double TextSize { get; set; } = DefaultFontSize;

	/// <summary>
	/// Create new Toast
	/// </summary>
	/// <param name="message">Toast message</param>
	/// <param name="duration">Toast duration</param>
	/// <param name="textSize">Toast font size</param>
	/// <returns>New instance of Toast</returns>
	public static IToast Make(
		string message,
		ToastDuration duration = ToastDuration.Short,
		double textSize = DefaultFontSize)
	{
		return new Toast
		{
			Text = message,
			Duration = duration,
			TextSize = textSize
		};
	}

	/// <summary>
	/// Show Toast
	/// </summary>
	public virtual Task Show(CancellationToken token = default)
	{
#if WINDOWS
		return ShowPlatform(token);
#else
		ShowPlatform(token);
		return Task.CompletedTask;
#endif
	}

	/// <summary>
	/// Dismiss Toast
	/// </summary>
	public virtual Task Dismiss(CancellationToken token = default)
	{
#if WINDOWS
		return DismissPlatform(token);
#else
		DismissPlatform(token);
		return Task.CompletedTask;
#endif
	}

	/// <summary>
	/// Dispose Toast
	/// </summary>
	public void Dispose()
	{
		Dispose(true);
		GC.SuppressFinalize(this);
	}

	static TimeSpan GetDuration(ToastDuration duration) => duration switch
	{
		ToastDuration.Short => TimeSpan.FromSeconds(2),
		ToastDuration.Long => TimeSpan.FromSeconds(3.5),
		_ => throw new InvalidEnumArgumentException(nameof(Duration), (int)duration, typeof(ToastDuration))
	};
}
