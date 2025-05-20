using System.Windows.Input;

namespace Recipes.Behaviors;

public class RadioEventToCommandBehavior : Behavior<RadioButton>
{
	public static readonly BindableProperty ValueChangedCommandProperty = BindableProperty.Create(
		nameof(ValueChangedCommand), typeof(ICommand), typeof(EntryEventToCommandBehavior),
		propertyChanged: PropChanged
		);
	public ICommand ValueChangedCommand
	{
		get => (ICommand)GetValue(ValueChangedCommandProperty);
		set => SetValue(ValueChangedCommandProperty, value);
	}

	private static void PropChanged(BindableObject view, object oldValue, object newValue)
	{
		//
	}

	protected override void OnAttachedTo(RadioButton bindable)
	{
		base.OnAttachedTo(bindable);
		bindable.CheckedChanged += OnRadioCheckedChanged;
	}
	protected override void OnDetachingFrom(RadioButton bindable)
	{
		base.OnDetachingFrom(bindable);
		bindable.CheckedChanged -= OnRadioCheckedChanged;
	}
	public void OnRadioCheckedChanged(object? sender, CheckedChangedEventArgs args)
	{
		var parameter = args;
		if (args.Value && (ValueChangedCommand?.CanExecute(parameter) ?? false))
		{
			ValueChangedCommand.Execute(parameter);
		}
	}
}
