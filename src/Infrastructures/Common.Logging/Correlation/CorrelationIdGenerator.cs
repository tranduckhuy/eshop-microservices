namespace Common.Logging.Correlation
{
    public class CorrelationIdGenerator : ICorrelationIdGenerator
    {
        private readonly object _lock = new object();
        private string? _correlationId;

        public string Get()
        {
            if (string.IsNullOrEmpty(_correlationId))
            {
                lock (_lock)
                {
                    _correlationId ??= Guid.NewGuid().ToString();
                }
            }
            return _correlationId;
        }

        public void Set(string correlationId)
        {
            lock (_lock)
            {
                _correlationId = correlationId;
            }
        }
    }
}
