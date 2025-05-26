using CoreGraphics;
using UIKit;

namespace Recipes.ToastUtil;
/// <summary>
/// Visual Options for <see cref="CommunityToolkit.Maui.Core.Views.AlertView"/>
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