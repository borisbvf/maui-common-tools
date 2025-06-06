﻿using CoreGraphics;
using CoreText;
using Foundation;
using UIKit;
using Microsoft.Maui.Platform;

namespace MauiCommonTools.Alerts;
/// <summary>
/// Toast for iOS + MacCatalyst
/// </summary>
public class PlatformToast : Alert, IDisposable
{
	internal const float DefaultPadding = 10;
	internal static Color DefaultTextColor = Colors.Black;

	readonly PaddedLabel messageLabel;

	bool isDisposed;

	/// <summary>
	/// Initialize <see cref="PlatformToast"/>
	/// </summary>
	/// <param name="message">Toast Message</param>
	/// <param name="backgroundColor">Toast Background Color</param>
	/// <param name="cornerRadius">Toast Border Corner Radius</param>
	/// <param name="textColor">Toast Text Color</param>
	/// <param name="font">Toast Font</param>
	/// <param name="characterSpacing">Toast Message Character Spacing</param>
	/// <param name="padding">Toast Padding</param>
	/// <param name="shouldFillAndExpandHorizontally">Should stretch container horizontally to fit the screen</param>
	public PlatformToast(
		string message,
		UIColor backgroundColor,
		CGRect cornerRadius,
		UIColor textColor,
		UIFont font,
		double characterSpacing,
		nfloat padding,
		bool shouldFillAndExpandHorizontally = false) : base(shouldFillAndExpandHorizontally)
	{
		padding += DefaultPadding;

		messageLabel = new PaddedLabel(padding, padding, padding, padding)
		{
			Lines = new IntPtr(0)
		};

		Message = message;
		TextColor = textColor;
		Font = font;
		CharacterSpacing = characterSpacing;
		AlertView.VisualOptions.BackgroundColor = backgroundColor;
		AlertView.VisualOptions.CornerRadius = cornerRadius;
		AlertView.AddChild(messageLabel);
	}

	/// <summary>
	/// Finalizer for <see cref="PlatformToast"/>
	/// </summary>
	~PlatformToast() => Dispose(false);

	/// <summary>
	/// Toast Message
	/// </summary>
	public string Message
	{
		get => messageLabel.Text ??= string.Empty;
		private init => messageLabel.Text = value;
	}

	/// <summary>
	/// Toast Text Color
	/// </summary>
	public UIColor TextColor
	{
		get => messageLabel.TextColor ??= DefaultTextColor.ToPlatform();
		private init => messageLabel.TextColor = value;
	}

	/// <summary>
	/// Toast Font
	/// </summary>
	public UIFont Font
	{
		get => messageLabel.Font;
		private init => messageLabel.Font = value;
	}

	/// <summary>
	/// Toast CharacterSpacing
	/// </summary>
	public double CharacterSpacing
	{
		init
		{
			var em = Font.PointSize > 0 ? GetEmFromPx(Font.PointSize, value) : 0;
			messageLabel.AttributedText = new NSAttributedString(Message, new CTStringAttributes() { KerningAdjustment = (float)em });
		}
	}

	/// <inheritdoc />
	public void Dispose()
	{
		Dispose(true);
		GC.SuppressFinalize(this);
	}

	/// <inheritdoc />
	protected virtual void Dispose(bool isDisposing)
	{
		if (!isDisposed)
		{
			if (isDisposing)
			{
				messageLabel.Dispose();
			}

			isDisposed = true;
		}
	}

	static nfloat GetEmFromPx(nfloat defaultFontSize, double currentValue) => 100 * (nfloat)currentValue / defaultFontSize;
}