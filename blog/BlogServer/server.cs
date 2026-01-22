using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

var urlPrefix = "http://localhost:5000/";
var root = Directory.GetCurrentDirectory();
var fallbackRoot = Directory.GetParent(root)?.FullName;

if (args.Length > 0)
{
    urlPrefix = args[0].EndsWith("/") ? args[0] : args[0] + "/";
}

Console.WriteLine($"Serving {root} on {urlPrefix}");
Console.WriteLine("Press Ctrl+C to stop.");

using var listener = new HttpListener();
listener.Prefixes.Add(urlPrefix);
listener.Start();

var cts = new CancellationTokenSource();
Console.CancelKeyPress += (_, e) =>
{
    e.Cancel = true;
    cts.Cancel();
    try
    {
        listener.Stop();
    }
    catch
    {
        // Ignore errors on shutdown.
    }
};

var contentTypes = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase)
{
    [".html"] = "text/html; charset=utf-8",
    [".htm"] = "text/html; charset=utf-8",
    [".css"] = "text/css; charset=utf-8",
    [".js"] = "application/javascript; charset=utf-8",
    [".json"] = "application/json; charset=utf-8",
    [".svg"] = "image/svg+xml",
    [".png"] = "image/png",
    [".jpg"] = "image/jpeg",
    [".jpeg"] = "image/jpeg",
    [".gif"] = "image/gif",
    [".ico"] = "image/x-icon"
};

async Task HandleRequestAsync(HttpListenerContext context)
{
    var request = context.Request;
    var response = context.Response;

    var relativePath = Uri.UnescapeDataString(request.Url?.AbsolutePath ?? "/");
    if (string.IsNullOrWhiteSpace(relativePath) || relativePath == "/")
    {
        relativePath = "/index.html";
    }

    var localPath = Path.Combine(root, relativePath.TrimStart('/'));
    if (!File.Exists(localPath) && !string.IsNullOrWhiteSpace(fallbackRoot))
    {
        var fallbackPath = Path.Combine(fallbackRoot, relativePath.TrimStart('/'));
        if (File.Exists(fallbackPath))
        {
            localPath = fallbackPath;
        }
    }

    if (!File.Exists(localPath))
    {
        response.StatusCode = (int)HttpStatusCode.NotFound;
        var bytes = Encoding.UTF8.GetBytes("Not Found");
        response.ContentType = "text/plain; charset=utf-8";
        response.ContentLength64 = bytes.Length;
        await response.OutputStream.WriteAsync(bytes, 0, bytes.Length);
        response.Close();
        return;
    }

    var extension = Path.GetExtension(localPath);
    if (!contentTypes.TryGetValue(extension, out var contentType))
    {
        contentType = "application/octet-stream";
    }

    response.StatusCode = (int)HttpStatusCode.OK;
    response.ContentType = contentType;

    await using var fileStream = File.OpenRead(localPath);
    response.ContentLength64 = fileStream.Length;
    await fileStream.CopyToAsync(response.OutputStream);
    response.Close();
}

try
{
    while (!cts.IsCancellationRequested)
    {
        var context = await listener.GetContextAsync();
        _ = Task.Run(() => HandleRequestAsync(context), cts.Token);
    }
}
catch (HttpListenerException) when (cts.IsCancellationRequested)
{
    // Listener stopped due to Ctrl+C.
}
finally
{
    listener.Close();
}
