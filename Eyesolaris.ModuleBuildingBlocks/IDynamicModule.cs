using System.Diagnostics.CodeAnalysis;

namespace Eyesolaris.DynamicLoading
{
    public interface IRunnableObject : IDisposable
    {
        bool IsRunning { get; }
        bool IsDisposed { get; }
        bool Initialized { get; }
        Type? OptionsType { get; }
        object? OptionsUntyped { get; }

        Task? WorkerTask { get; }

        void Initialize(object? options);

        /// <summary>
        /// Runs an initialized module
        /// </summary>
        /// <param name="token"></param>
        /// <exception cref="InvalidOperationException">Module is not initialized</exception>
        Task Run(CancellationToken token);
    }

    public interface IRunnableObject<TOptions> : IRunnableObject
    {
        TOptions? Options { get; }

        void Initialize(TOptions? options);
    }

    public interface IDynamicModule : IRunnableObject
    {
        DynamicEntityName ModuleId { get; }
        string Description { get; }
    }

    public interface IDynamicModule<TOptions> : IDynamicModule, IRunnableObject<TOptions>
    {
    }
}
