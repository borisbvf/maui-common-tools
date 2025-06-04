using CoreGraphics;
using Microsoft.Maui.Platform;
using UIKit;

namespace MauiCommonTools.Alerts;
public class AlertView(bool shouldFillAndExpandHorizontally) : UIView
{
	const int defaultSpacing = 10;

	readonly List<UIView> children = [];
	readonly bool shouldFillAndExpandHorizontally = shouldFillAndExpandHorizontally;

	/// <summary>
	/// Parent UIView
	/// </summary>
	public UIView ParentView { get; } = UIApplicationExtensions.GetKeyWindow(
		UIApplication.SharedApplication) ?? throw new InvalidOperationException("KeyWindow is not found.");

	/// <summary>
	/// PopupView Children
	/// </summary>
	public IReadOnlyList<UIView> Children => children;

	/// <summary>
	/// <see cref="AlertViewVisualOptions"/>
	/// </summary>
	public AlertViewVisualOptions VisualOptions { get; } = new();

	/// <summary>
	/// <see cref="UIView"/> on which Alert will appear. When null, <see cref="AlertView"/> will appear at bottom of screen.
	/// </summary>
	public UIView? AnchorView { get; set; }

	/// <summary>
	/// Container of <see cref="AlertView"/>
	/// </summary>
	protected UIStackView Container { get; } = new()
	{
		Alignment = UIStackViewAlignment.Fill,
		Distribution = UIStackViewDistribution.EqualSpacing,
		Axis = UILayoutConstraintAxis.Horizontal,
		TranslatesAutoresizingMaskIntoConstraints = false
	};

	/// <summary>
	/// Dismisses the Popup from the screen
	/// </summary>
	public void Dismiss() => RemoveFromSuperview();

	/// <summary>
	/// Adds a <see cref="UIView"/> to <see cref="Children"/>
	/// </summary>
	/// <param name="child"></param>
	public void AddChild(UIView child)
	{
		children.Add(child);
		Container.AddArrangedSubview(child);
	}

	/// <summary>
	/// Initializes <see cref="AlertView"/>
	/// </summary>
	public void Setup()
	{
		Initialize();
		SetParentConstraints();
	}

	/// <inheritdoc />
	public override void LayoutSubviews()
	{
		base.LayoutSubviews();

		if (AnchorView is null)
		{
			this.SafeBottomAnchor().ConstraintEqualTo(ParentView.SafeBottomAnchor(), -defaultSpacing).Active = true;
			this.SafeTopAnchor().ConstraintGreaterThanOrEqualTo(ParentView.SafeTopAnchor(), defaultSpacing).Active = true;
		}
		else if (AnchorView.Superview is not null
				 && AnchorView.Superview.ConvertRectToView(AnchorView.Frame, null).Top < Container.Frame.Height + SafeAreaLayoutGuide.LayoutFrame.Bottom)
		{
			var top = AnchorView.Superview.Frame.Top + AnchorView.Frame.Height + defaultSpacing;
			this.SafeTopAnchor().ConstraintEqualTo(ParentView.TopAnchor, top).Active = true;
		}
		else
		{
			this.SafeBottomAnchor().ConstraintEqualTo(AnchorView.SafeTopAnchor(), 0).Active = true;
		}
	}

	void SetParentConstraints()
	{
		if (shouldFillAndExpandHorizontally)
		{
			this.SafeLeadingAnchor().ConstraintEqualTo(ParentView.SafeLeadingAnchor(), defaultSpacing).Active = true;
			this.SafeTrailingAnchor().ConstraintEqualTo(ParentView.SafeTrailingAnchor(), -defaultSpacing).Active = true;
		}
		else
		{
			this.SafeLeadingAnchor().ConstraintGreaterThanOrEqualTo(ParentView.SafeLeadingAnchor(), defaultSpacing).Active = true;
			this.SafeTrailingAnchor().ConstraintLessThanOrEqualTo(ParentView.SafeTrailingAnchor(), -defaultSpacing).Active = true;
		}

		this.SafeCenterXAnchor().ConstraintEqualTo(ParentView.SafeCenterXAnchor()).Active = true;

		Container.SafeLeadingAnchor().ConstraintEqualTo(this.SafeLeadingAnchor(), defaultSpacing).Active = true;
		Container.SafeTrailingAnchor().ConstraintEqualTo(this.SafeTrailingAnchor(), -defaultSpacing).Active = true;
		Container.SafeBottomAnchor().ConstraintEqualTo(this.SafeBottomAnchor(), -defaultSpacing).Active = true;
		Container.SafeTopAnchor().ConstraintEqualTo(this.SafeTopAnchor(), defaultSpacing).Active = true;
	}

	void Initialize()
	{
		TranslatesAutoresizingMaskIntoConstraints = false;
		AddSubview(Container);

		var subView = new RoundedView(
			VisualOptions.CornerRadius.X,
			VisualOptions.CornerRadius.Y,
			VisualOptions.CornerRadius.Width,
			VisualOptions.CornerRadius.Height)
		{
			BackgroundColor = VisualOptions.BackgroundColor,
			AutoresizingMask = UIViewAutoresizing.FlexibleHeight | UIViewAutoresizing.FlexibleWidth
		};
		Subviews[0].InsertSubview(subView, atIndex: 0);
	}
}

/// <summary>
/// Visual Options for <see cref="AlertView"/>
/// </summary>
public class AlertViewVisualOptions
{
	/// <summary>
	/// Border Corner Radius
	/// </summary>
	public CGRect CornerRadius { get; set; }

	/// <summary>
	/// Background Color
	/// </summary>
	public UIColor BackgroundColor { get; set; } = UIColor.Gray;
}

internal static class UIApplicationExtensions
{
	public static UIWindow? GetKeyWindow(this UIApplication application)
	{
#pragma warning disable CA1416 // TODO: 'UIApplication.Windows' is unsupported on: 'ios' 15.0 and later.
#pragma warning disable CA1422 // Validate platform compatibility
		var windows = application.Windows;
#pragma warning restore CA1422 // Validate platform compatibility
#pragma warning restore CA1416

		for (int i = 0; i < windows.Length; i++)
		{
			var window = windows[i];
			if (window.IsKeyWindow)
				return window;
		}

		return null;
	}
}