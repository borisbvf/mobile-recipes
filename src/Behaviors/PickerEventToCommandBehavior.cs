using System.Windows.Input;

namespace Recipes.Behaviors;
public class PickerEventToCommandBehavior : Behavior<Picker>
{
	public static readonly BindableProperty ValueChangedCommandProperty = BindableProperty.Create(
		nameof(ValueChangedCommand), typeof(ICommand), typeof(EntryEventToCommandBehavior)
		);
	public ICommand ValueChangedCommand
	{
		get => (ICommand)GetValue(ValueChangedCommandProperty);
		set => SetValue(ValueChangedCommandProperty, value);
	}
	protected override void OnAttachedTo(Picker bindable)
	{
		base.OnAttachedTo(bindable);
		bindable.SelectedIndexChanged += OnPickerValueChanged;
	}
	protected override void OnDetachingFrom(Picker bindable)
	{
		base.OnDetachingFrom(bindable);
		bindable.SelectedIndexChanged -= OnPickerValueChanged;
	}
	public void OnPickerValueChanged(object? sender, EventArgs e)
	{
		var parameter = e;
		if (ValueChangedCommand?.CanExecute(parameter) ?? false)
		{
			ValueChangedCommand.Execute(parameter);
		}
	}
}
