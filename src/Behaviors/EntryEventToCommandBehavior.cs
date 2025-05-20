using System.Windows.Input;

namespace Recipes.Behaviors;

public class EntryEventToCommandBehavior : Behavior<Entry>
{
	public static readonly BindableProperty TextChangedCommandProperty = BindableProperty.Create(
		nameof(TextChangedCommand), typeof(ICommand), typeof(EntryEventToCommandBehavior),
		propertyChanged: PropChanged
		);
	public ICommand TextChangedCommand
	{
		get => (ICommand)GetValue(TextChangedCommandProperty);
		set => SetValue(TextChangedCommandProperty, value);
	}

	private static void PropChanged(BindableObject view, object oldValue, object newValue)
	{
		//
	}

	protected override void OnAttachedTo(Entry bindable)
	{
		base.OnAttachedTo(bindable);
		bindable.TextChanged += OnEntryTextChanged;
	}
	protected override void OnDetachingFrom(Entry bindable)
	{
		base.OnDetachingFrom(bindable);
		bindable.TextChanged -= OnEntryTextChanged;
	}
	public void OnEntryTextChanged(object? sender, TextChangedEventArgs args)
	{
		var parameter = args;
		if (TextChangedCommand?.CanExecute(parameter) ?? false)
		{
			TextChangedCommand.Execute(parameter);
		}
	}
}
