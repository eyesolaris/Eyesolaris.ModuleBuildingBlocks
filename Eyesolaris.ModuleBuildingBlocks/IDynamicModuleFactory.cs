using System.Reflection;

namespace Eyesolaris.DynamicLoading
{
    public interface IDynamicModuleFactory
    {
        string FactoryId { get; }

        DynamicEntityName FactoryDynamicId => new(FactoryId, _GetAssemblyVersion());
        IReadOnlyList<DynamicEntityName> SupportedModuleTypes { get; }
        IDynamicModule? CreateDynamicModule(DynamicEntityName id);

        private Version _GetAssemblyVersion()
        {
            Type implementerType = GetType();
            AssemblyName typeAssemblyName = implementerType.Assembly.GetName();
            return typeAssemblyName.Version ?? _defaultVersion;
        }

        private static readonly Version _defaultVersion = new(1, 0);
    }

    public interface IDynamicModuleFactory<TModule> : IDynamicModuleFactory
        where TModule : class, IDynamicModule
    {
        new TModule? CreateDynamicModule(DynamicEntityName id);

        IDynamicModule? IDynamicModuleFactory.CreateDynamicModule(DynamicEntityName id)
            => CreateDynamicModule(id);
    }
}
