using Microsoft.Extensions.Logging;
using System.Text;
using System.Text.Json;

namespace MiniJob.Processors;

public class HttpProcessor : ProcessorBase
{
    protected override async Task<ProcessorResult> DoWorkAsync(ProcessorContext context)
    {
        HttpArgs httpArgs;
        try
        {
            httpArgs = JsonSerializer.Deserialize<HttpArgs>(context.GetArgs());
        }
        catch (Exception ex)
        {
            string message = $"http params deserialize failed, please check jobParam configuration, args: {context.GetArgs()}, HttpParams PropertyInfo: {typeof(HttpArgs).GetProperties()}";
            Logger.LogError(ex, message);
            return ProcessorResult.ErrorMessage(message);
        }

        if (httpArgs.Url.IsNullOrEmpty())
            return ProcessorResult.ErrorMessage("url can't be empty!");

        if (!httpArgs.Url.StartsWith("http"))
            httpArgs.Url = "http://" + httpArgs.Url;
        Logger.LogInformation("request url: {Url}", httpArgs.Url);

        // set default method
        if (httpArgs.Method.IsNullOrEmpty())
        {
            httpArgs.Method = "GET";
            Logger.LogInformation("using default request method: GET");
        }
        else
        {
            httpArgs.Method = httpArgs.Method.ToUpper();
            Logger.LogInformation("request httpmethod: {HttpMethod}", httpArgs.Method);
        }

        // set default mediaType
        if (httpArgs.Method != "GET" && httpArgs.MediaType.IsNullOrEmpty())
        {
            httpArgs.MediaType = "application/json";
            Logger.LogInformation("try to use 'application/json' as media type");
        }

        Logger.LogInformation("request timeout: {Timeout} seconds", httpArgs.Timeout);

        var httpClientFactory = LazyServiceProvider.LazyGetRequiredService<IHttpClientFactory>();
        var httpClient = httpClientFactory.CreateClient();
        httpClient.Timeout = TimeSpan.FromSeconds(httpArgs.Timeout);

        HttpRequestMessage httpRequest = new(GetHttpMethod(httpArgs.Method), httpArgs.Url);
        if (!httpArgs.Headers.IsNullOrEmpty())
        {
            foreach (var header in httpArgs.Headers)
            {
                httpRequest.Headers.Add(header.Key, header.Value);
                Logger.LogInformation("add header {Key}:{Value}", header.Key, header.Value);
            }
        }

        if (httpArgs.Method != "GET")
        {
            httpRequest.Content = new StringContent(httpArgs.Body, Encoding.UTF8, httpArgs.MediaType);
        }

        // todo: 失败重试
        var response = await httpClient.SendAsync(httpRequest);
        var responseMsg = await response.Content.ReadAsStringAsync();

        if (!response.IsSuccessStatusCode)
        {
            var message = $"{httpArgs} failed, response code is {response.StatusCode}, response body is {responseMsg}";
            Logger.LogWarning(message);
            return ProcessorResult.ErrorMessage(message);
        }

        Logger.LogInformation("{HttpArgs} Response {StatusCode}, response body: {Response}", httpArgs, response.StatusCode, responseMsg);

        // 自定义结果校验
        if (!httpArgs.CheckKey.IsNullOrWhiteSpace() && !httpArgs.CheckValue.IsNullOrWhiteSpace())
        {
            using var jsonDocument = JsonDocument.Parse(responseMsg);
            if (jsonDocument.RootElement.TryGetProperty(httpArgs.CheckKey, out JsonElement jsonElement))
            {
                var checkValue = jsonElement.GetString();
                if (httpArgs.CheckValue != checkValue)
                {
                    var message = $"check {httpArgs.CheckKey}={httpArgs.CheckValue} failed, response code is {response.StatusCode}";
                    Logger.LogWarning(message);
                    return ProcessorResult.ErrorMessage(message);
                }
                else
                {
                    var message = $"check {httpArgs.CheckKey}={httpArgs.CheckValue} success, response code is {response.StatusCode}";
                    Logger.LogInformation(message);
                    return ProcessorResult.OkMessage(message);
                }
            }
            else
            {
                var message = $"check {httpArgs.CheckKey}={httpArgs.CheckValue} failed, response not exists checkkey, please check jobargs configuration, response code is {response.StatusCode}";
                Logger.LogWarning(message);
                return ProcessorResult.OkMessage(message);
            }
        }

        var okMessage = $"response statuscode: {response.StatusCode}";
        Logger.LogInformation(okMessage);
        return ProcessorResult.OkMessage(okMessage);
    }

    private static HttpMethod GetHttpMethod(string method)
    {
        switch (method)
        {
            case "GET": return HttpMethod.Get;
            case "POST": return HttpMethod.Post;
            case "PUT": return HttpMethod.Put;
            case "DELETE": return HttpMethod.Delete;
        }

        return HttpMethod.Get;
    }
}
