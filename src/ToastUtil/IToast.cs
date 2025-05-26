namespace Recipes.ToastUtil;
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
