using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Reflection;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Scriban;

namespace SkanksAIO.Web;

[AttributeUsage(AttributeTargets.Class, Inherited = false)]
public class ControllerAttribute : Attribute { }

[AttributeUsage(AttributeTargets.Method, Inherited = false)]
public class RouteAttribute : Attribute
{
    public string Path { get; private set; } = "";
    public string[] Methods { get; private set; } = { "GET" };

    public RouteAttribute(string Path, string[]? Methods = null)
    {
        this.Path = Path;
        if (Methods != null)
            this.Methods = Methods;
    }
}

[AttributeUsage(AttributeTargets.Method, Inherited = false)]
public class TemplateAttribute : Attribute
{
    public string Name { get; private set; } = "";

    public TemplateAttribute(string Name)
    {
        this.Name = Name;
    }
}

internal class Node
{
    internal List<Node> nodes = new List<Node>();
    internal string key;
    internal MethodInfo? handler;

    internal Node(string key)
    {
        this.key = key;
    }

    private static bool IsSet(string str)
    {
        return str != null && str.Trim().Length > 0;
    }

    internal void Insert(string uri, MethodInfo handler)
    {
        // Radix Trie Insert
        var parts = uri.Split('/', 2);
        var key = parts[0];
        var rest = parts.Length > 1 ? parts[1] : "";
        if (nodes.Count == 0)
            nodes.Add(new Node(key));
        var node = nodes.FirstOrDefault(n => n.key == key);
        if (node == null)
            nodes.Add(node = new Node(key));
        if (IsSet(rest))
            node.Insert(rest, handler);
        else
            node.handler = handler;
    }

    internal MethodInfo? Get(string uri)
    {
        // Radix Trie Get
        var parts = uri.Split('/', 2);
        var key = parts[0];
        var rest = parts.Length > 1 ? parts[1] : "";
        var node = nodes.FirstOrDefault(n => n.key == key);
        if (node == null)
            return null;
        if (IsSet(rest))
            return node.Get(rest);
        else
            return node.handler;
    }
}

internal class Router
{
    private Dictionary<string, Node> routes = new Dictionary<string, Node>();

    internal void Insert(string method, string path, MethodInfo handler)
    {
        if (!this.routes.ContainsKey(method.ToUpper()))
        {
            this.routes.Add(method.ToUpper(), new Node(""));
        }
        this.routes[method.ToUpper()].Insert(path, handler);
    }

    internal async Task<string> RouteRequest(HttpListenerRequest request, HttpListenerResponse response)
    {
        response.ContentType = "text/plain";

        var path = request.Url?.AbsolutePath;
        if (path == null) return "404";

        var method = request.HttpMethod;
        if (!this.routes.ContainsKey(method.ToUpper()))
        {
            return "404";
        }

        var node = this.routes[method.ToUpper()];
        var handler = node.Get(path);

        if (handler == null || handler.DeclaringType == null)
        {
            response.StatusCode = 404;
            return "404";
        }

        var container = Activator.CreateInstance(handler.DeclaringType);

        // collect query parameters from request for the handler arguments
        var query = request.QueryString;
        var args = new List<object>();

        foreach (var arg in handler.GetParameters())
        {
            if (arg.ParameterType == typeof(HttpListenerRequest))
            {
                args.Add(request);
            }
            else if (arg.ParameterType == typeof(string))
            {
                if (query[arg.Name] != null)
                    args.Add(query[arg.Name]!);
                else
                    args.Add(arg.HasDefaultValue ? arg.DefaultValue! : "");
            }
            else if (arg.ParameterType == typeof(int))
            {
                if (query[arg.Name] != null)
                    args.Add(int.Parse(query[arg.Name]!));
                else
                    args.Add(arg.HasDefaultValue ? arg.DefaultValue! : 0);
            }
            else if (arg.ParameterType == typeof(bool))
            {
                if (query[arg.Name] != null)
                    args.Add(bool.Parse(query[arg.Name]!));
                else
                    args.Add(arg.HasDefaultValue ? arg.DefaultValue! : false);
            }
            else if (arg.ParameterType == typeof(double))
            {
                if (query[arg.Name] != null)
                    args.Add(double.Parse(query[arg.Name]!));
                else
                    args.Add(arg.HasDefaultValue ? arg.DefaultValue! : 0.0);
            }
            else if (arg.ParameterType == typeof(float))
            {
                if (query[arg.Name] != null)
                    args.Add(float.Parse(query[arg.Name]!));
                else
                    args.Add(arg.HasDefaultValue ? arg.DefaultValue! : 0.0f);
            }
            else if (arg.ParameterType == typeof(long))
            {
                if (query[arg.Name] != null)
                    args.Add(long.Parse(query[arg.Name]!));
                else
                    args.Add(arg.HasDefaultValue ? arg.DefaultValue! : 0L);
            }
            else if (arg.ParameterType == typeof(short))
            {
                if (query[arg.Name] != null)
                    args.Add(short.Parse(query[arg.Name]!));
                else
                    args.Add(arg.HasDefaultValue ? arg.DefaultValue! : 0);
            }
            else if (arg.ParameterType == typeof(byte))
            {
                if (query[arg.Name] != null)
                    args.Add(byte.Parse(query[arg.Name]!));
                else
                    args.Add(arg.HasDefaultValue ? arg.DefaultValue! : 0);
            }
            else if (arg.ParameterType == typeof(char))
            {
                if (query[arg.Name] != null)
                    args.Add(char.Parse(query[arg.Name]!));
                else
                    args.Add(arg.HasDefaultValue ? arg.DefaultValue! : '\0');
            }
            else if (arg.ParameterType == typeof(decimal))
            {
                if (query[arg.Name] != null)
                    args.Add(decimal.Parse(query[arg.Name]!));
                else
                    args.Add(arg.HasDefaultValue ? arg.DefaultValue! : 0.0m);
            }
            else if (arg.ParameterType == typeof(uint))
            {
                if (query[arg.Name] != null)
                    args.Add(uint.Parse(query[arg.Name]!));
                else
                    args.Add(arg.HasDefaultValue ? arg.DefaultValue! : 0);
            }
            else if (arg.ParameterType == typeof(ushort))
            {
                if (query[arg.Name] != null)
                    args.Add(ushort.Parse(query[arg.Name]!));
                else
                    args.Add(arg.HasDefaultValue ? arg.DefaultValue! : 0);
            }
            else if (arg.ParameterType == typeof(ulong))
            {
                if (query[arg.Name] != null)
                    args.Add(ulong.Parse(query[arg.Name]!));
                else
                    args.Add(arg.HasDefaultValue ? arg.DefaultValue! : 0);
            }
            else if (arg.ParameterType == typeof(sbyte))
            {
                if (query[arg.Name] != null)
                    args.Add(sbyte.Parse(query[arg.Name]!));
                else
                    args.Add(arg.HasDefaultValue ? arg.DefaultValue! : 0);
            }
            else if (arg.ParameterType == typeof(char[]))
            {
                if (query[arg.Name] != null)
                    args.Add(query[arg.Name]!.ToCharArray());
                else
                    args.Add(arg.HasDefaultValue ? arg.DefaultValue! : new char[0]);
            }
            else if (arg.ParameterType == typeof(DateTime))
            {
                if (query[arg.Name] != null)
                    args.Add(DateTime.Parse(query[arg.Name]!));
                else
                    args.Add(arg.HasDefaultValue ? arg.DefaultValue! : DateTime.MinValue);
            }
            else if (arg.ParameterType == typeof(TimeSpan))
            {
                if (query[arg.Name] != null)
                    args.Add(TimeSpan.Parse(query[arg.Name]!));
                else
                    args.Add(arg.HasDefaultValue ? arg.DefaultValue! : TimeSpan.MinValue);
            }
            else if (arg.ParameterType == typeof(Guid))
            {
                if (query[arg.Name] != null)
                    args.Add(Guid.Parse(query[arg.Name]!));
                else
                    args.Add(arg.HasDefaultValue ? arg.DefaultValue! : Guid.Empty);
            }
            else if (arg.ParameterType == typeof(Uri))
            {
                if (query[arg.Name] != null)
                    args.Add(new Uri(query[arg.Name]!));
                else
                    args.Add(arg.HasDefaultValue ? arg.DefaultValue! : new Uri(""));
            }
            else if (arg.ParameterType == typeof(object))
            {
                if (query[arg.Name] != null)
                    args.Add(query[arg.Name]!);
                else
                    args.Add(arg.HasDefaultValue ? arg.DefaultValue! : default!);
            }
            else
            {
                if (query[arg.Name] != null)
                    args.Add(query[arg.Name]!);
                else
                    args.Add(arg.HasDefaultValue ? arg.DefaultValue! : "");
            }
        }

        dynamic? result;
        if (handler.ReturnType.BaseType == typeof(Task))
        {
            result = await (Task<dynamic>)handler.Invoke(container, args.ToArray())!;
        }
        else
        {
            result = handler.Invoke(container, args.ToArray())!;
        }

        if (result == null)
        {
            response.StatusCode = 404;
            return "404";
        }

        var template = handler.GetCustomAttribute<TemplateAttribute>();
        if (template == null)
        {
            var str_result = result.ToString();
            if (str_result == null)
            {
                response.StatusCode = 500;
                return "500";
            }

            str_result = str_result.Trim();

            // check if str_result starts with { or [ for json serialization
            if (str_result.StartsWith("{") || str_result.StartsWith("["))
            {
                response.ContentType = "application/json";
                return JsonConvert.SerializeObject(result);
            }
            else
            {
                response.ContentType = "text/plain";
                return str_result;
            }
        }

        var templatePath = template.Name;
        if (templatePath.StartsWith("/"))
        {
            templatePath = templatePath.Substring(1);
        }

        var basepath = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
        var templatePathFull = Path.Combine(basepath!, "templates", templatePath);

        if (!File.Exists(templatePathFull))
        {
            response.StatusCode = 404;
            return $"Template [{templatePathFull}] not found";
        }

        var templateContent = File.ReadAllText(templatePathFull);

        try
        {
            var t = Template.Parse(templateContent);
            var output = t.Render(result);
            
            response.ContentType = "text/html";
            return output;
        }
        catch (Exception e)
        {
            response.StatusCode = 500;
            return $"Error rendering template [{templatePathFull}]: {e.Message}";
        }
    }
}

internal class WebServer
{
    private int port;
    private HttpListener _listener;

    private Router router = new Router();

    private WebBehaviour? behaviour;

    public WebServer(int port)
    {
        _listener = new HttpListener();
        this.port = port;
    }

    public void Start()
    {
        behaviour = Plugin.Instance!.AddComponent<WebBehaviour>();
        behaviour.OnRequestReceived += OnRequestReceived;
        // find all Controllers
        var controllers = Assembly.GetExecutingAssembly().GetTypes()
            .Where(t => t.IsDefined(typeof(ControllerAttribute), false));

        // register all controllers
        foreach (var controller in controllers)
        {
            var methods = controller.GetMethods();
            foreach (var method in methods)
            {
                var attr2 = method.GetCustomAttribute<RouteAttribute>();
                if (attr2 != null)
                {
                    foreach (var m in attr2.Methods)
                    {
                        router.Insert(m, attr2.Path, method);
                    }
                }
            }
        }

        _listener.Prefixes.Add($"http://*:{port.ToString()}/");
        _listener.Start();
        accept();
        Plugin.Logger?.LogMessage($"[Webserver] Listening on port {port}");
    }

    public void Stop()
    {
        _listener.Stop();
    }

    private void accept()
    {
        _listener.BeginGetContext(new AsyncCallback(ListenerCallback), _listener);
    }

    public void OnRequestReceived(HttpListenerContext context)
    {
        HttpListenerRequest request = context.Request;
        HttpListenerResponse response = context.Response;

        var responseString = router.RouteRequest(request, response).GetAwaiter().GetResult();

        byte[] buffer = System.Text.Encoding.UTF8.GetBytes(responseString);
        response.ContentLength64 = buffer.Length;
        System.IO.Stream output = response.OutputStream;
        output.Write(buffer, 0, buffer.Length);
        output.Close();

        if (_listener.IsListening)
        {
            accept();
        }
    }

    private void ListenerCallback(IAsyncResult result)
    {
        HttpListenerContext context = _listener.EndGetContext(result);
        this.behaviour!.QueueRequest(context);
    }
}
