namespace MiniJob.Dapr.Security;

public interface IDaprApiTokenProvider
{
    string GetDaprApiToken();

    string GetAppApiToken();
}
