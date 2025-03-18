namespace Eyesolaris.DynamicLoading
{
    public interface IDynamicModuleFactory
    {
        string FactoryId { get; }
        IReadOnlyList<DynamicEntityName> SupportedModuleTypes { get; }
        IDynamicModule? CreateDynamicModule(DynamicEntityName id);
    }

    public interface IDynamicModuleFactory<TModule> : IDynamicModuleFactory
        where TModule : class, IDynamicModule
    {
        new TModule? CreateDynamicModule(DynamicEntityName id);

        IDynamicModule? IDynamicModuleFactory.CreateDynamicModule(DynamicEntityName id)
            => CreateDynamicModule(id);
    }
}
