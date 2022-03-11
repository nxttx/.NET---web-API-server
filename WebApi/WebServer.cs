using System;
using System.Collections.Generic;
using System.Net;
using System.Threading;

namespace WebApi;

public class WebServer
{
    private Boolean IsRunning = false;
    private Action _InitializeComponent;
    
    private HttpListener _httpListener;
    private List<Route> _routes;
    private Func<Response> _notFoundCallback;

    /// <summary>
    /// Constructor without interface
    /// </summary>
    public WebServer()
    {
        _routes = new List<Route>();
        _notFoundCallback = () => new Response(404, "<html><body>404</body></html>");
    }
    
    /// <summary>
    /// Constructor with interface
    /// </summary>
    /// <param name="_InitializeComponent"></param>
    public WebServer(Action _InitializeComponent)
    {
        _routes = new List<Route>();
        _notFoundCallback = () => new Response(404, "<html><body>404</body></html>");
        this._InitializeComponent = _InitializeComponent;
    }

    /// <summary>
    /// Routes HTTP GET requests to the specified path with the specified callback functions.
    /// </summary>
    /// <param name="path"></param>
    /// <param name="callback"></param>
    public void Get(String path, Func<Response> callback)
    {
        var route = new Route(path, "GET", callback);
        _routes.Add(route);
    }

    /// <summary>
    /// Routes HTTP POST requests to the specified path with the specified callback functions.
    /// </summary>
    /// <param name="path"></param>
    /// <param name="callback"></param>
    public void Post(String path, Func<Response> callback)
    {
        var route = new Route(path, "POST", callback);
        _routes.Add(route);
    }
    
    /// <summary>
    /// Routes HTTP DELETE requests to the specified path with the specified callback functions
    /// </summary>
    /// <param name="path"></param>
    /// <param name="callback"></param>
    public void Delete(String path, Func<Response> callback)
    {
        var route = new Route(path, "DELETE", callback);
        _routes.Add(route);
    }

    /// <summary>
    /// Routes HTTP PUT requests to the specified path with the specified callback functions.
    /// </summary>
    /// <param name="path"></param>
    /// <param name="callback"></param>
    public void Put(String path, Func<Response> callback)
    {
        var route = new Route(path, "PUT", callback);
        _routes.Add(route);
    }

    /// <summary>
    /// This method is like the standard app.METHOD() methods, except it matches all HTTP verbs.
    /// </summary>
    /// <param name="path"></param>
    /// <param name="callback"></param>
    public void All(String path, Func<Response> callback)
    {
        Get(path, callback);
        Post(path, callback);
        Delete(path, callback);
        Put(path, callback);
    }

    /// <summary>
    /// Sets the 404 callback
    /// </summary>
    /// <param name="notFoundCallback"></param>
    public void NotFound(Func<Response> notFoundCallback)
    {
        _notFoundCallback = notFoundCallback;
    }

    /// <summary>
    /// Binds and listens for connections on the specified host and port.
    /// </summary>
    /// <param name="port"></param>
    public void Listen(int port)
    {
        IsRunning = true; 
        _httpListener = new HttpListener();
        _httpListener.Prefixes.Add("http://localhost" + ":" + port +"/");
        _httpListener.Start();
        Console.WriteLine("Listening...");
        while (true)
        {
            // Note: The GetContext method blocks while waiting for a request
            HttpListenerContext context = _httpListener.GetContext();
            // If connection has been found, set connection on a thread from the thread pool and handle the response. 
            ThreadPool.QueueUserWorkItem(state => { 
                HttpListenerRequest request = context.Request;
                // Obtain a response object.
                HttpListenerResponse response = context.Response;
                // Get user response 
                Response userResponse = (GetRoute(request.HttpMethod, request.RawUrl))();
                // Set user status code
                response.StatusCode = userResponse.GetStatusCode();
                // Set user response text
                string responseString = userResponse.GetResponseText();
                byte[] buffer = System.Text.Encoding.UTF8.GetBytes(responseString);
                // Get a response stream and write the response to it.
                response.ContentLength64 = buffer.Length;
                System.IO.Stream output = response.OutputStream;
                output.Write(buffer, 0, buffer.Length);
                // You must close the output stream.
                output.Close();
            });
        }
    }

    /// <summary>
    /// Stops listening to HTTP requests
    /// </summary>
    /// <exception cref="ListenerNotStartedException"></exception>
    public void Close()
    {
        try
        {
            _httpListener.Stop();
            _httpListener.Close();
            IsRunning = false;
        }
        catch (NullReferenceException e)
        {
            throw new ListenerNotStartedException(
                "Trying to close the webserver while the webserver is not active.");
        }
        catch (HttpListenerException e)
        {
            
        }
        catch (Exception e)
        {
            throw;
        }

    }

    /// <summary>
    /// Starts Webserver interface
    /// </summary>
    public void Start()
    {
        _InitializeComponent();
    }

    private Func<Response> GetRoute(String method, String path)
    {
        foreach (var route in _routes)
        {
            if (route.GetMethod() == method && route.Getpath() == path)
            {
                return route.GetCallback();
            }
        }
        
        return _notFoundCallback;
    }

    public Boolean GetIsRunning() { return IsRunning; }

}