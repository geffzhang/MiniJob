namespace MiniJob.Cron;

public sealed class CronExpressionException : Exception
{
    public CronExpressionException()
        : base()
    {
    }

    public CronExpressionException(string message)
        : base(message)
    {
    }

    public CronExpressionException(string message, Exception innerException)
        : base(message, innerException)
    {
    }
}
