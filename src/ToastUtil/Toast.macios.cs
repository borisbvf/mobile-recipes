using CoreGraphics;
using Microsoft.Maui.Animations;
using Microsoft.Maui.Platform;
using System.Runtime.InteropServices;
using UIKit;

namespace Recipes.ToastUtil;
public partial class Toast : IToast
{
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
											AlertDefaults.BackgroundColor.ToPlatform(),
											cornerRadius,
											AlertDefaults.TextColor.ToPlatform(),
											UIFont.SystemFontOfSize((NFloat)TextSize),
											AlertDefaults.CharacterSpacing,
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
