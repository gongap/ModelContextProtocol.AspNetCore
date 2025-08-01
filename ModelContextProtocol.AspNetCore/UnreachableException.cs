namespace System.Diagnostics
{
    public sealed class UnreachableException : Exception
    {
        public UnreachableException(string? message)
            : base(message)
        {
        }

        public UnreachableException(string? message, Exception? innerException)
            : base(message, innerException)
        {
        }
    }
}