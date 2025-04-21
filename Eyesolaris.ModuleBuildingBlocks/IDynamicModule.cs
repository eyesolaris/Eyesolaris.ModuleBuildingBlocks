using System.Diagnostics.CodeAnalysis;

namespace Eyesolaris.DynamicLoading
{
    public interface IDynamicModule : IRunnableObject
    {
        DynamicEntityId ModuleId { get; }
        string Description { get; }
    }

    public interface IDynamicModule<TOptions> : IDynamicModule, IRunnableObject<TOptions>
    {
    }
}
