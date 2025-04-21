
namespace Eyesolaris.DynamicLoading
{
    public abstract class DynamicInitializableModule<TOptions> : RunnableInitializableObject<TOptions>, IDynamicModule<TOptions>
    {
        protected DynamicInitializableModule()
            : base(initializationNecessary: true)
        {
        }

        public abstract DynamicEntityId ModuleId { get; }
        public abstract string Description { get; }

        protected DynamicInitializableModule(bool initializationNecessary)
            : base(initializationNecessary)
        {
        }

        protected override sealed Task RunImpl(CancellationToken token)
        {
            if (!Initialized)
            {
                throw new InvalidOperationException("Module is not initialized");
            }
            return StartModule(token);
        }

        protected virtual Task StartModule(CancellationToken token)
        {
            return Task.CompletedTask;
        }
    }
}
