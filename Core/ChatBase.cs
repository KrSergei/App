namespace Core
{
    public abstract class ChatBase
    {
        protected CancellationTokenSource CancellationTokenSource { get; set; } = new CancellationTokenSource();
        protected CancellationToken CancellationToken => CancellationTokenSource.Token;
        
        public abstract Task Start();
        protected abstract Task Listener();
    }
}
