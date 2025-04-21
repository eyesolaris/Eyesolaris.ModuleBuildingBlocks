using System.Reflection;

namespace Eyesolaris.DynamicLoading
{
    public interface IDynamicModuleFactory
    {
        string FactoryId { get; }

        DynamicEntityId FactoryDynamicId => new(FactoryId, GetType().GetTypeAssemblyVersion());
        IReadOnlyList<DynamicEntityId> SupportedModuleTypes { get; }
        IDynamicModule? CreateDynamicModule(DynamicEntityIdTemplate id);
    }

    public interface IDynamicModuleFactory<TModule> : IDynamicModuleFactory
        where TModule : class, IDynamicModule
    {
        new TModule? CreateDynamicModule(DynamicEntityIdTemplate id);

        IDynamicModule? IDynamicModuleFactory.CreateDynamicModule(DynamicEntityIdTemplate id)
            => CreateDynamicModule(id);
    }
}
