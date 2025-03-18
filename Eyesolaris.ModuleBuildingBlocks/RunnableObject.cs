using Eyesolaris.Logging;
using Microsoft.Extensions.Logging;
using System.Diagnostics.CodeAnalysis;

namespace Eyesolaris.DynamicLoading
{
    public abstract class RunnableObject : IRunnableObject
    {
        public event EventHandler? DisposeInvoked;

        #region Public properties
        [MemberNotNullWhen(true, nameof(_outerToken), nameof(_cts), nameof(_tokenRegistration))]
        public bool IsRunning => _outerToken is not null;
        public bool IsDisposed => _disposedValue;
        public bool Initialized { get; protected set; }
        public abstract Task? WorkerTask { get; }
        public virtual Type? OptionsType => null;
        public virtual object? OptionsUntyped => null;
        #endregion

        #region Public methods
        public virtual void Initialize(object? options)
        {
            Initialized = true;
        }

        public async Task Run(CancellationToken cancellationToken)
        {
            ThrowIfDisposed();
            if (IsRunning)
                throw new InvalidOperationException("Object is already started");
            _semaphore.Wait(cancellationToken);
            try
            {
                _outerToken = cancellationToken;
                _cts = CancellationTokenSource.CreateLinkedTokenSource(cancellationToken);
                CancellationToken tok = _cts.Token;
                _tokenRegistration = tok.Register(_DisposeCtsIfNotNull);
                await RunImpl(tok);
            }
            finally
            {
                _semaphore.Release();
            }
        }

        public async Task Stop()
        {
            _semaphore.Wait();
            try
            {
                if (!IsRunning)
                    return;
                if (_cts is not null)
                {
                    _cts.Cancel();
                    _DisposeCts();
                }
                await StopImpl();
                _outerToken = null;
            }
            finally
            {
                _semaphore.Release();
            }
        }

        public void Dispose()
        {
            // Не изменяйте этот код. Разместите код очистки в методе "Dispose(bool disposing)".
            Dispose(disposing: true);
            GC.SuppressFinalize(this);
        }
        #endregion

        #region Protected properties
        protected SemaphoreSlim StateSemaphore => _semaphore;

        #endregion

        #region Protected abstract methods
        protected abstract Task RunImpl(CancellationToken cancellationToken);


        #endregion

        #region Protected virtual methods
        #region Start/Stop support
        protected virtual Task StopImpl() { return Task.CompletedTask; }
        #endregion

        #region Dispose support
        protected virtual void DisposeManagedResources() { }
        protected virtual void DisposeUnmanagedResources() { }
        protected virtual void NullifyLargeFields() { }
        #endregion
        #endregion

        #region Protected constructors
        protected RunnableObject(bool initializationNecessary)
        {
            Initialized = !initializationNecessary;
        }
        #endregion

        #region Protected methods
        #region Dispose support
        protected void Dispose(bool disposing)
        {
            if (!_disposedValue)
            {
                DisposeInvoked?.Invoke(this, EventArgs.Empty);
                if (disposing)
                {
                    // TODO: освободить управляемое состояние (управляемые объекты)
                    if (IsRunning)
                    {
                        try
                        {
                            Stop().Wait();
                        }
                        catch (Exception ex)
                        {
                            Logger.Global.LogWarning("An exception occured while stopping the {objType} object:\n{ex}", GetType(), ex);
                        }
                        _tokenRegistration.Value.Dispose();
                    }
                    DisposeManagedResources();
                    if (_semaphore.CurrentCount == 0)
                    {
                        _semaphore.Release();
                    }
                    _semaphore.Dispose();
                }
                // TODO: освободить неуправляемые ресурсы (неуправляемые объекты) и переопределить метод завершения
                DisposeUnmanagedResources();
                // TODO: установить значение NULL для больших полей
                NullifyLargeFields();
                _disposedValue = true;
            }
        }

        protected void ThrowIfDisposed()
        {
            if (_disposedValue)
            {
                throw new ObjectDisposedException(nameof(RunnableObject), "Object is already disposed");
            }
        }
        #endregion

        #region Finalizer
        // TODO: переопределить метод завершения, только если "Dispose(bool disposing)" содержит код для освобождения неуправляемых ресурсов
        ~RunnableObject()
        {
            // Не изменяйте этот код. Разместите код очистки в методе "Dispose(bool disposing)".
            Dispose(disposing: false);
        }
        #endregion
        #endregion

        #region Private fields
        private CancellationToken? _outerToken;
        private CancellationTokenSource? _cts;
        private bool _disposedValue;
        private CancellationTokenRegistration? _tokenRegistration;
        private readonly SemaphoreSlim _semaphore = new(1, 1);
        #endregion

        #region Private methods
        private void _DisposeCtsIfNotNull()
        {
            if (_cts is not null)
            {
                _DisposeCts();
            }
        }

        private void _DisposeCts()
        {
            _cts?.Dispose();
            _cts = null;
        }
        #endregion
    }
}
