using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Eyesolaris.DynamicLoading
{
    public abstract class DynamicModule : RunnableObject, IDynamicModule
    {
        public abstract DynamicEntityName ModuleId { get; }
        public abstract string Description { get; }

        protected DynamicModule(bool initializationNecessary)
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
