using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Threading;

namespace WebApi;

public class WebServer
{
    private Boolean _isRunning;
    private Action _initializeComponent;
    
    private HttpListener _httpListener;
    private List<Route> _routes;
    private Func<RequestData, Response> _notFoundCallback;

    /// <summary>
    /// Constructor without interface
    /// </summary>
    public WebServer()
    {
        _isRunning = false;
        _routes = new List<Route>();
        _notFoundCallback = (_) => new Response(404, "<html><body>404</body></html>");
        _initializeComponent = null;
    }
    
    /// <summary>
    /// Constructor with interface
    /// </summary>
    /// <param name="initializeComponent"></param>
    public WebServer(Action initializeComponent)
    {
        _isRunning = false;
        _routes = new List<Route>();
        _notFoundCallback = (_) => new Response(404, "<html><body>404</body></html>");
        _initializeComponent = initializeComponent;
    }

    /// <summary>
    /// Routes HTTP GET requests to the specified path with the specified callback functions.
    /// </summary>
    /// <param name="path"></param>
    /// <param name="callback"></param>
    public void Get(String path, Func<RequestData, Response> callback)
    {
        var route = new Route(path, "GET", callback);
        _routes.Add(route);
    }

    /// <summary>
    /// Routes HTTP POST requests to the specified path with the specified callback functions.
    /// </summary>
    /// <param name="path"></param>
    /// <param name="callback"></param>
    public void Post(String path, Func<RequestData, Response> callback)
    {
        var route = new Route(path, "POST", callback);
        _routes.Add(route);
    }
    
    /// <summary>
    /// Routes HTTP DELETE requests to the specified path with the specified callback functions
    /// </summary>
    /// <param name="path"></param>
    /// <param name="callback"></param>
    public void Delete(String path, Func<RequestData, Response> callback)
    {
        var route = new Route(path, "DELETE", callback);
        _routes.Add(route);
    }

    /// <summary>
    /// Routes HTTP PUT requests to the specified path with the specified callback functions.
    /// </summary>
    /// <param name="path"></param>
    /// <param name="callback"></param>
    public void Put(String path, Func<RequestData, Response> callback)
    {
        var route = new Route(path, "PUT", callback);
        _routes.Add(route);
    }

    /// <summary>
    /// This method is like the standard app.METHOD() methods, except it matches all HTTP verbs.
    /// </summary>
    /// <param name="path"></param>
    /// <param name="callback"></param>
    public void All(String path, Func<RequestData, Response> callback)
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
    public void NotFound(Func<RequestData, Response> notFoundCallback)
    {
        _notFoundCallback = notFoundCallback;
    }

    /// <summary>
    /// Binds and listens for connections on the specified host and port.
    /// </summary>
    /// <param name="port"></param>
    public void Listen(int port)
    {
        _isRunning = true; 
        _httpListener = new HttpListener();
        _httpListener.Prefixes.Add("http://localhost" + ":" + port +"/");
        _httpListener.Start();
        Console.WriteLine("Listening...");
        while (true)
        {
            // Note: The GetContext method blocks while waiting for a request
            HttpListenerContext context = _httpListener.GetContext();
            // If connection has been found, set connection on a thread from the thread pool and handle the response. 
            ThreadPool.QueueUserWorkItem(_ => { 
                HttpListenerRequest request = context.Request;
                // Obtain a response object.
                HttpListenerResponse response = context.Response;
                
                // Split url for var and raw path;
                String[] splitUrl = request.RawUrl?.Split("?");
                String path = splitUrl[0];
                // Create get parameters
                String[,] getParameters = new String[0,0];
                if (splitUrl.Length > 1)
                {
                    getParameters = CreateParameters(splitUrl[1]);
                }
                // Get request body
                String body = GetBodyContent(request.InputStream);
                // Create requestData for callback
                RequestData requestDataForCallback = new RequestData(getParameters, body, request);
                
                // Get user response 
                Response userResponse = (GetRouteCallback(request.HttpMethod,path ))(requestDataForCallback);
                // Set user status code
                response.StatusCode = userResponse.GetStatusCode();
                // Set user response text
                string responseString = userResponse.GetResponseText();
                byte[] buffer = System.Text.Encoding.UTF8.GetBytes(responseString);
                // Get a response stream and write the response to it.
                response.ContentLength64 = buffer.Length;
                Stream output = response.OutputStream;
                output.Write(buffer, 0, buffer.Length);
                // You must close the output stream.
                output.Close();
            });
        }
    }

    /// <summary>
    /// Stops listening to HTTP requests.
    ///
    /// TODO: CLosing application results in an exception, find a solution so that doesnt happen. 
    /// </summary>
    /// <exception cref="ListenerNotStartedException"></exception>
    public void Close()
    {
        try
        {
            _httpListener.Stop();
            _httpListener.Close();
            _isRunning = false;
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
        if (_initializeComponent != null)
        {
            _initializeComponent();
        }
        else
        {
            throw new WebserverNotInitializedWithInterfaceException("Trying to start the interface when the interface was nog given to the webserver.");
        }
        
    }
    
    /// <summary>
    /// Searches for the specified route and returns that routes callback, if no route has been found,
    /// then the 404 callback will be returned.
    ///
    /// TODO: Look if forloop can be changed to LINQ or PLINQ. 
    /// </summary>
    /// <param name="method"></param>
    /// <param name="path"></param>
    /// <returns></returns>
    private Func<RequestData, Response> GetRouteCallback(String method, String path)
    {
        
        IEnumerable<Func<RequestData, Response>> scoreQuery =
            from route in _routes
            where route.GetMethod() == method && route.Getpath() == path
            select route.GetCallback();

        try
        {
            return scoreQuery.First();
        }
        catch (InvalidOperationException e)
        {
            return _notFoundCallback;
        }
    }
    
    /// <summary>
    /// Getter for isRunning.
    /// </summary>
    /// <returns>isRunning</returns>
    public Boolean GetIsRunning() { return _isRunning; }

    /// <summary>
    /// creates get parameters from an string
    /// </summary>
    /// <param name="parameters"></param>
    /// <returns>Arraylist with all parameters</returns>
    private String[,] CreateParameters(String parameters)
    {
        String[] parametersSplitted = parameters.Split('&');
        
        String[,] returnValue = new String [parametersSplitted.Length, 2];

        for (int i = 0; i < parametersSplitted.Length; i++)
        {
            String[] varsSplitted = parametersSplitted[i].Split('=');
            returnValue[i, 0] = varsSplitted[0];
            returnValue[i, 1] = varsSplitted[1];
        }

        return returnValue; 
    }

    /// <summary>
    /// Reads the requestInputStream and returns it in a String format. 
    /// </summary>
    /// <param name="requestInputStream"></param>
    /// <returns>The content of the request input stream</returns>
    /// <exception cref="NotImplementedException"></exception>
    private string GetBodyContent(Stream requestInputStream)
    {
        using (var reader = new StreamReader(requestInputStream))
        {
            return reader.ReadToEnd();
        }
    }
}