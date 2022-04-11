using Microsoft.Extensions.Logging;
using System.Text.RegularExpressions;

namespace MiniJob.Dapr.Processes;

internal static class LoggerExtensions
{
    public const string DebugLevel = "debug";
    public const string InfoLevel = "info";
    public const string WarningLevel = "warning";
    public const string ErrorLevel = "error";
    public const string FatalLevel = "fatal";

    public static void LogData(this ILogger logger, string data, IDaprProcessUpdater processUpdater)
    {
        if (string.IsNullOrEmpty(data))
        {
            return;
        }

        var message = data;
        var properties = new Dictionary<string, object>();
        var logLevel = LogLevel.Information;
        try
        {
            // Parse the log json text using System.Text.Json
            var logRecord = System.Text.Json.JsonSerializer.Deserialize<DaprProcessLogRecord>(
                data,
                new System.Text.Json.JsonSerializerOptions
                {
                    PropertyNameCaseInsensitive = true
                });

            // Extract the message.
            if (!logRecord.Msg.IsNullOrEmpty())
            {
                message = logRecord.Msg;
            }

            // Log Level
            if (!logRecord.Level.IsNullOrEmpty())
            {
                logLevel = ToDaprLogLevel(logRecord.Level);
            }

            // Extract the version
            if (!string.IsNullOrEmpty(logRecord.Ver))
            {
                AddProperty(properties, "DaprVersion", logRecord.Ver);
                processUpdater.UpdateVersion(logRecord.Ver);
            }

            // Extract known additional properties
            AddProperty(properties, "DaprAppId", logRecord.App_id);
            AddProperty(properties, "DaprInstance", logRecord.Instance);
            AddProperty(properties, "DaprScope", logRecord.Scope);
            AddProperty(properties, "DaprTime", logRecord.Time);
            AddProperty(properties, "DaprType", logRecord.Type);
        }
        catch
        {
            // Cannot parse, will just be stored as a string
        }

        // Log it out
        if (properties.Count > 0)
        {
            // Include additional properties as a scope.
            using (logger.BeginScope(properties))
            {
                logger.Log(logLevel, message);
            }
        }
        else
        {
            logger.Log(logLevel, message);
        }

        // Now interpret the message
        if (processUpdater != null)
        {
            InterpretMessage(message, processUpdater);
        }
    }

    private static void AddProperty(Dictionary<string, object> values, string key, string value)
    {
        if (!value.IsNullOrWhiteSpace())
        {
            values.Add(key, value);
        }
    }

    private static void InterpretMessage(string message, IDaprProcessUpdater processUpdater)
    {
        if (Regex.Match(message, "([Dd]apr).+([Ii]nitialized).+([Rr]unning)").Success)
        {
            // Dapr Sidecar
            // "dapr initialized. Status: Running. Init Elapsed 243.003ms"
            processUpdater.UpdateStatus(DaprProcessStatus.Started);
        }
        else if (Regex.Match(message, "([Pp]lacement).+([Ss]tarted).+([Pp]ort)").Success)
        {
            // Dapr Placement
            // "placement service started on port 50005"
            processUpdater.UpdateStatus(DaprProcessStatus.Started);
        }
        else if (Regex.Match(message, "([Ss]entry).+([Rr]unning).+([Pp]rotecting)").Success)
        {
            // Dapr Sentry
            // "sentry certificate authority is running, protecting ya'll"
            processUpdater.UpdateStatus(DaprProcessStatus.Started);
        }
        else if (Regex.Match(message, "([Ss]top).+([Ss]hutting).+([Dd]own)").Success)
        {
            // Dapr Placement
            // "stop command issued. Shutting down all operations"
            processUpdater.UpdateStatus(DaprProcessStatus.Stopping);
        }
    }

    internal static LogLevel ToDaprLogLevel(string daprDaprLogLevel) =>
        daprDaprLogLevel switch
        {
            FatalLevel => LogLevel.Critical,
            DebugLevel => LogLevel.Debug,
            ErrorLevel => LogLevel.Error,
            WarningLevel => LogLevel.Warning,
            _ => LogLevel.Information,
        };
}
