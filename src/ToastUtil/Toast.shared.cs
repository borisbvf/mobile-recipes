using System.ComponentModel;

namespace Recipes.ToastUtil;
public partial class Toast : IToast
{
	bool isDisposed;

	public string Text { get; set; } = string.Empty;

	public ToastDuration Duration { get; set; } = ToastDuration.Short;

	public double TextSize { get; set; } = AlertDefaults.FontSize;

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
		double textSize = AlertDefaults.FontSize)
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
}
