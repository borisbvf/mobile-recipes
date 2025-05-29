using Microsoft.Windows.AppNotifications;
using Microsoft.Windows.AppNotifications.Builder;
using System.ComponentModel;

namespace Recipes.ToastUtil;
public partial class Toast : IToast
{
	static AppNotification? PlatformToast { get; set; }

	/// <summary>
	/// Dispose Toast
	/// </summary>
	protected virtual void Dispose(bool isDisposing)
	{
		if (isDisposed)
		{
			return;
		}

		isDisposed = true;
	}

	static async Task DismissPlatform(CancellationToken token)
	{
		if (PlatformToast is null)
		{
			return;
		}

		token.ThrowIfCancellationRequested();
		await AppNotificationManager.Default.RemoveAllAsync();

		// Verify PlatformToast is not null again after `await`
		if (PlatformToast is not null)
		{
			PlatformToast.Expiration = DateTimeOffset.Now;
			PlatformToast = null;
		}
	}

	async Task ShowPlatform(CancellationToken token)
	{
		await DismissPlatform(token);
		token.ThrowIfCancellationRequested();
		PlatformToast = new AppNotificationBuilder()
			.AddText(Text)
			.SetDuration(GetAppNotificationDuration(Duration))
			.BuildNotification();
		PlatformToast.Expiration = DateTimeOffset.Now.Add(GetDuration(Duration));
		AppNotificationManager.Default.Register();
		AppNotificationManager.Default.Show(PlatformToast);
		AppNotificationManager.Default.Unregister();
	}

	static AppNotificationDuration GetAppNotificationDuration(ToastDuration duration) => duration switch
	{
		ToastDuration.Short => AppNotificationDuration.Default,
		ToastDuration.Long => AppNotificationDuration.Long,
		_ => throw new InvalidEnumArgumentException(nameof(Duration), (int)duration, typeof(ToastDuration))
	};
}
