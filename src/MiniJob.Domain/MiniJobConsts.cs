namespace MiniJob;

public static class MiniJobConsts
{
    public const string DbTablePrefix = "MiniJob";

    public const string DbSchema = null;

    public static TimeSpan WorkerTimeout = TimeSpan.FromSeconds(30);
}