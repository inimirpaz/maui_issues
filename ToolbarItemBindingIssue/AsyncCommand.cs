// Credits: https://montemagno.com/asynccommand-more-come-to-mvvmhelpers/

using System.Reflection;
using System.Runtime.CompilerServices;
using System.Windows.Input;
using static System.String;

namespace Helpers;

public interface IAsyncCommand : ICommand
{
    /// <summary>
    /// Execute the command async.
    /// </summary>
    /// <returns>Task to be awaited on.</returns>
    Task ExecuteAsync();
}

/// <summary>
/// Interface for Async Command with parameter
/// </summary>
public interface IAsyncCommand<T> : ICommand
{
    /// <summary>
    /// Execute the command async.
    /// </summary>
    /// <param name="parameter">Parameter to pass to command</param>
    /// <returns>Task to be awaited on.</returns>
    Task ExecuteAsync(T parameter);
}
#nullable enable
public class AsyncCommand : IAsyncCommand
{
    readonly Func<Task> execute;
    readonly Func<object, bool>? canExecute;
    readonly Action<Exception>? onException;
    readonly bool continueOnCapturedContext;
    readonly WeakEventManager weakEventManager = new WeakEventManager();

    /// <summary>
    /// Create a new AsyncCommand
    /// </summary>
    /// <param name="execute">Function to execute</param>
    /// <param name="canExecute">Function to call to determine if it can be executed</param>
    /// <param name="onException">Action callback when an exception occurs</param>
    /// <param name="continueOnCapturedContext">If the context should be captured on exception</param>
    public AsyncCommand(Func<Task> execute,
                        Func<object, bool>? canExecute = null,
                        Action<Exception>? onException = null,
                        bool continueOnCapturedContext = false)
    {
        this.execute = execute ?? throw new ArgumentNullException(nameof(execute));
        this.canExecute = canExecute;
        this.onException = onException;
        this.continueOnCapturedContext = continueOnCapturedContext;
    }

    /// <summary>
    /// Event triggered when Can Excecute changes.
    /// </summary>
    public event EventHandler CanExecuteChanged
    {
        add { weakEventManager.AddEventHandler(value); }
        remove { weakEventManager.RemoveEventHandler(value); }
    }

    /// <summary>
    /// Invoke the CanExecute method and return if it can be executed.
    /// </summary>
    /// <param name="parameter">Parameter to pass to CanExecute.</param>
    /// <returns>If it can be executed.</returns>
    public bool CanExecute(object parameter) => canExecute?.Invoke(parameter) ?? true;

    /// <summary>
    /// Execute the command async.
    /// </summary>
    /// <returns>Task of action being executed that can be awaited.</returns>
    public Task ExecuteAsync() => execute();

    /// <summary>
    /// Raise a CanExecute change event.
    /// </summary>
    public void RaiseCanExecuteChanged() => weakEventManager.HandleEvent(this, EventArgs.Empty, nameof(CanExecuteChanged));

    #region Explicit implementations
    void ICommand.Execute(object parameter) => ExecuteAsync().SafeFireAndForget(onException, continueOnCapturedContext);
    #endregion
}

/// <summary>
/// Implementation of a generic Async Command
/// </summary>
public class AsyncCommand<T> : IAsyncCommand<T>
{

    readonly Func<T, Task> execute;
    readonly Func<object, bool>? canExecute;
    readonly Action<Exception>? onException;
    readonly bool continueOnCapturedContext;
    readonly WeakEventManager weakEventManager = new WeakEventManager();

    /// <summary>
    /// Create a new AsyncCommand
    /// </summary>
    /// <param name="execute">Function to execute</param>
    /// <param name="canExecute">Function to call to determine if it can be executed</param>
    /// <param name="onException">Action callback when an exception occurs</param>
    /// <param name="continueOnCapturedContext">If the context should be captured on exception</param>
    public AsyncCommand(Func<T, Task> execute,
                        Func<object, bool>? canExecute = null,
                        Action<Exception>? onException = null,
                        bool continueOnCapturedContext = false)
    {
        this.execute = execute ?? throw new ArgumentNullException(nameof(execute));
        this.canExecute = canExecute;
        this.onException = onException;
        this.continueOnCapturedContext = continueOnCapturedContext;
    }

    /// <summary>
    /// Event triggered when Can Excecute changes.
    /// </summary>
    public event EventHandler CanExecuteChanged
    {
        add { weakEventManager.AddEventHandler(value); }
        remove { weakEventManager.RemoveEventHandler(value); }
    }

    /// <summary>
    /// Invoke the CanExecute method and return if it can be executed.
    /// </summary>
    /// <param name="parameter">Parameter to pass to CanExecute.</param>
    /// <returns>If it can be executed</returns>
    public bool CanExecute(object parameter) => canExecute?.Invoke(parameter) ?? true;

    /// <summary>
    /// Execute the command async.
    /// </summary>
    /// <returns>Task that is executing and can be awaited.</returns>
    public Task ExecuteAsync(T parameter) => execute(parameter);

    /// <summary>
    /// Raise a CanExecute change event.
    /// </summary>
    public void RaiseCanExecuteChanged() => weakEventManager.HandleEvent(this, EventArgs.Empty, nameof(CanExecuteChanged));

    #region Explicit implementations

    void ICommand.Execute(object parameter)
    {
        if (CommandUtils.IsValidCommandParameter<T>(parameter))
            ExecuteAsync((T)parameter).SafeFireAndForget(onException, continueOnCapturedContext);

    }
    #endregion
}

public class WeakEventManager
{
    readonly Dictionary<string, List<Subscription>> eventHandlers = new Dictionary<string, List<Subscription>>();

    /// <summary>
    /// Add an event handler to the manager.
    /// </summary>
    /// <typeparam name="TEventArgs">Event handler of T</typeparam>
    /// <param name="handler">Handler of the event</param>
    /// <param name="eventName">Name to use in the dictionary. Should be unique.</param>
    public void AddEventHandler<TEventArgs>(EventHandler<TEventArgs> handler, [CallerMemberName] string eventName = "")
        where TEventArgs : EventArgs
    {
        if (IsNullOrEmpty(eventName))
            throw new ArgumentNullException(nameof(eventName));

        if (handler is null)
            throw new ArgumentNullException(nameof(handler));

        AddEventHandler(eventName, handler.Target, handler.GetMethodInfo());
    }

    /// <summary>
    /// Add an event handler to the manager.
    /// </summary>
    /// <param name="handler">Handler of the event</param>
    /// <param name="eventName">Name to use in the dictionary. Should be unique.</param>
    public void AddEventHandler(EventHandler handler, [CallerMemberName] string eventName = "")
    {
        if (IsNullOrEmpty(eventName))
            throw new ArgumentNullException(nameof(eventName));

        if (handler == null)
            throw new ArgumentNullException(nameof(handler));

        AddEventHandler(eventName, handler.Target, handler.GetMethodInfo());
    }

    /// <summary>
    /// Handle an event
    /// </summary>
    /// <param name="sender">Sender of the event</param>
    /// <param name="args">Arguments for the event</param>
    /// <param name="eventName">Name of the event.</param>
    public void HandleEvent(object sender, object args, string eventName)
    {
        List<(object? subscriber, MethodInfo handler)>? toRaise = new List<(object? subscriber, MethodInfo handler)>();
        List<Subscription>? toRemove = new List<Subscription>();

        if (eventHandlers.TryGetValue(eventName, out List<Subscription>? target))
        {
            for (int i = 0; i < target.Count; i++)
            {
                Subscription subscription = target[i];
                bool isStatic = subscription.Subscriber == null;
                if (isStatic)
                {
                    // For a static method, we'll just pass null as the first parameter of MethodInfo.Invoke
                    toRaise.Add((null, subscription.Handler));
                    continue;
                }

                object? subscriber = subscription.Subscriber?.Target;

                if (subscriber is null)
                    // The subscriber was collected, so there's no need to keep this subscription around
                    toRemove.Add(subscription);
                else
                    toRaise.Add((subscriber, subscription.Handler));
            }

            for (int i = 0; i < toRemove.Count; i++)
            {
                Subscription subscription = toRemove[i];
                target.Remove(subscription);
            }
        }

        for (int i = 0; i < toRaise.Count; i++)
        {
            (object? subscriber, MethodInfo? handler) = toRaise[i];
            handler.Invoke(subscriber, new[] { sender, args });
        }
    }

    /// <summary>
    /// Remove an event handler.
    /// </summary>
    /// <typeparam name="TEventArgs">Type of the EventArgs</typeparam>
    /// <param name="handler">Handler to remove</param>
    /// <param name="eventName">Event name to remove</param>
    public void RemoveEventHandler<TEventArgs>(EventHandler<TEventArgs> handler, [CallerMemberName] string eventName = "")
        where TEventArgs : EventArgs
    {
        if (IsNullOrEmpty(eventName))
            throw new ArgumentNullException(nameof(eventName));

        if (handler is null)
            throw new ArgumentNullException(nameof(handler));

        RemoveEventHandler(eventName, handler.Target, handler.GetMethodInfo());
    }

    /// <summary>
    /// Remove an event handler.
    /// </summary>
    /// <param name="handler">Handler to remove</param>
    /// <param name="eventName">Event name to remove</param>
    public void RemoveEventHandler(EventHandler handler, [CallerMemberName] string eventName = "")
    {
        if (IsNullOrEmpty(eventName))
            throw new ArgumentNullException(nameof(eventName));

        if (handler is null)
            throw new ArgumentNullException(nameof(handler));

        RemoveEventHandler(eventName, handler.Target, handler.GetMethodInfo());
    }

    void AddEventHandler(string eventName, object handlerTarget, MethodInfo methodInfo)
    {
        if (!eventHandlers.TryGetValue(eventName, out List<Subscription>? targets))
        {
            targets = new List<Subscription>();
            eventHandlers.Add(eventName, targets);
        }

        if (handlerTarget is null)
        {
            // This event handler is a static method
            targets.Add(new Subscription(null, methodInfo));
            return;
        }

        targets.Add(new Subscription(new WeakReference(handlerTarget), methodInfo));
    }

    void RemoveEventHandler(string eventName, object handlerTarget, MemberInfo methodInfo)
    {
        if (!eventHandlers.TryGetValue(eventName, out List<Subscription>? subscriptions))
            return;

        for (int n = subscriptions.Count; n > 0; n--)
        {
            Subscription current = subscriptions[n - 1];

            if (current.Subscriber?.Target != handlerTarget || current.Handler.Name != methodInfo.Name)
                continue;

            subscriptions.Remove(current);
            break;
        }
    }

    struct Subscription
    {
        public Subscription(WeakReference? subscriber, MethodInfo handler)
        {
            Subscriber = subscriber;
            Handler = handler ?? throw new ArgumentNullException(nameof(handler));
        }

        public readonly WeakReference? Subscriber;
        public readonly MethodInfo Handler;
    }
}

internal static class CommandUtils
{
    internal static bool IsValidCommandParameter<T>(object o)
    {
        bool valid;
        if (o != null)
        {
            // The parameter isn't null, so we don't have to worry whether null is a valid option
            valid = o is T;

            if (!valid)
                throw new InvalidCommandParameterException(typeof(T), o.GetType());

            return valid;
        }

        Type? t = typeof(T);

        // The parameter is null. Is T Nullable?
        if (Nullable.GetUnderlyingType(t) != null)
        {
            return true;
        }

        // Not a Nullable, if it's a value type then null is not valid
        valid = !t.GetTypeInfo().IsValueType;

        if (!valid)
            throw new InvalidCommandParameterException(typeof(T));

        return valid;
    }
}

/// <summary>
/// Represents errors that occur during IAsyncCommand execution.
/// </summary>
public class InvalidCommandParameterException : Exception
{
    /// <summary>
    /// Initializes a new instance of the <see cref="T:MvvmHelpersInvalidCommandParameterException"/> class.
    /// </summary>
    /// <param name="expectedType">Expected parameter type for AsyncCommand.Execute.</param>
    /// <param name="actualType">Actual parameter type for AsyncCommand.Execute.</param>
    /// <param name="innerException">Inner Exception</param>
    public InvalidCommandParameterException(Type expectedType, Type actualType, Exception innerException) : base(CreateErrorMessage(expectedType, actualType), innerException)
    {

    }

    /// <summary>
    /// Initializes a new instance of the <see cref="T:TaskExtensions.MVVM.InvalidCommandParameterException"/> class.
    /// </summary>
    /// <param name="expectedType">Expected parameter type for AsyncCommand.Execute.</param>
    /// <param name="innerException">Inner Exception</param>
    public InvalidCommandParameterException(Type expectedType, Exception innerException) : base(CreateErrorMessage(expectedType), innerException)
    {

    }

    /// <summary>
    /// Initializes a new instance of the <see cref="T:MvvmHelpers.InvalidCommandParameterException"/> class.
    /// </summary>
    /// <param name="expectedType">Expected parameter type for AsyncCommand.Execute.</param>
    /// <param name="actualType">Actual parameter type for AsyncCommand.Execute.</param>
    public InvalidCommandParameterException(Type expectedType, Type actualType) : base(CreateErrorMessage(expectedType, actualType))
    {

    }

    /// <summary>
    /// Initializes a new instance of the <see cref="T:TaskExtensions.MVVM.InvalidCommandParameterException"/> class.
    /// </summary>
    /// <param name="expectedType">Expected parameter type for AsyncCommand.Execute.</param>
    public InvalidCommandParameterException(Type expectedType) : base(CreateErrorMessage(expectedType))
    {

    }

    static string CreateErrorMessage(Type expectedType, Type actualType) =>
        $"Invalid type for parameter. Expected Type: {expectedType}, but received Type: {actualType}";

    static string CreateErrorMessage(Type expectedType) =>
        $"Invalid type for parameter. Expected Type {expectedType}";
}
public static class Utils
{
    /// <summary>
    /// Task extension to add a timeout.
    /// </summary>
    /// <returns>The task with timeout.</returns>
    /// <param name="task">Task.</param>
    /// <param name="timeoutInMilliseconds">Timeout duration in Milliseconds.</param>
    /// <typeparam name="T">The 1st type parameter.</typeparam>
    public async static Task<T> WithTimeout<T>(this Task<T> task, int timeoutInMilliseconds)
    {
        Task? retTask = await Task.WhenAny(task, Task.Delay(timeoutInMilliseconds))
            .ConfigureAwait(false);

#pragma warning disable CS8603 // Possible null reference return.
        return retTask is Task<T> ? task.Result : default;
#pragma warning restore CS8603 // Possible null reference return.
    }

    /// <summary>
    /// Task extension to add a timeout.
    /// </summary>
    /// <returns>The task with timeout.</returns>
    /// <param name="task">Task.</param>
    /// <param name="timeout">Timeout Duration.</param>
    /// <typeparam name="T">The 1st type parameter.</typeparam>
    public static Task<T> WithTimeout<T>(this Task<T> task, TimeSpan timeout) =>
        WithTimeout(task, (int)timeout.TotalMilliseconds);

#pragma warning disable RECS0165 // Asynchronous methods should return a Task instead of void
    /// <summary>
    /// Attempts to await on the task and catches exception
    /// </summary>
    /// <param name="task">Task to execute</param>
    /// <param name="onException">What to do when method has an exception</param>
    /// <param name="continueOnCapturedContext">If the context should be captured.</param>
    public static async void SafeFireAndForget(this Task task, Action<Exception>? onException = null, bool continueOnCapturedContext = false)
#pragma warning restore RECS0165 // Asynchronous methods should return a Task instead of void
    {
        try
        {
            await task.ConfigureAwait(continueOnCapturedContext);
        }
        catch (Exception ex) when (onException != null)
        {
            onException.Invoke(ex);
        }
    }
#nullable disable
}