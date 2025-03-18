namespace Eyesolaris.DynamicLoading
{
    public abstract class RunnableInitializableObject<TOptions> : RunnableObject, IRunnableObject<TOptions>
    {
        protected RunnableInitializableObject(bool initializationNecessary)
            : base(initializationNecessary)
        {
        }

        public override sealed Type OptionsType => typeof(TOptions);

        public TOptions? Options { get; private set; }

        public void Initialize(TOptions? options)
        {
            Options = options;
            if (options is not null)
            {
                ParseOptions(options);
            }
            Initialized = true;
        }

        public override sealed void Initialize(object? options)
        {
            if (options is null)
            {
                Initialize((TOptions?)default);
                return;
            }
            if (options is TOptions opts)
            {
                Initialize(opts);
                return;
            }
            throw new InvalidOperationException($"Options object have invalid type {options?.GetType().FullName ?? "null"}, expected {typeof(TOptions)}");
        }

        protected virtual void ParseOptions(TOptions options) { }
    }
}
