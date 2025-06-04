namespace MauiCommonTools.Alerts;
public interface IToast : IAlert, IDisposable
{
	ToastDuration Duration { get; }
	double TextSize { get; }
}

public enum ToastDuration
{
	Short,
	Long
}