using System.Windows.Input;

namespace Buform;

internal static class CommandExtensions
{
    public static bool SafeCanExecute(this ICommand? command, object? parameter = null)
    {
        return command?.CanExecute(parameter) == true;
    }

    public static void SafeExecute(this ICommand? command, object? parameter = null)
    {
        if (!command.SafeCanExecute(parameter))
        {
            return;
        }

        command!.Execute(parameter);
    }
}
