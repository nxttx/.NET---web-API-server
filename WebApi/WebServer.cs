using System;
using System.Collections.Generic;
using System.Net;

namespace WebApi;

public class WebServer
{
    private HttpListener _httpListener;
    private List<Route> _routes;

    public WebServer()
    {
        _routes = new List<Route>();
    }

    public void Get(String slug, Func<Response> callback)
    {
        var route = new Route(slug, "GET", callback);
        _routes.Add(route);
    }

    public void Post(String slug, Func<Response> callback)
    {
        var route = new Route(slug, "POST", callback);
        _routes.Add(route);
    }

    public void Delete(String slug, Func<Response> callback)
    {
        var route = new Route(slug, "DELETE", callback);
        _routes.Add(route);
    }

    public void Put(String slug, Func<Response> callback)
    {
        var route = new Route(slug, "PUT", callback);
        _routes.Add(route);
    }

    public void All(String slug, Func<Response> callback)
    {
        Get(slug, callback);
        Post(slug, callback);
        Delete(slug, callback);
        Put(slug, callback);
    }

    public void Listen(int port)
    {
        using (_httpListener = new HttpListener())
        {
            _httpListener.Prefixes.Add("http://localhost" + ":" + port +"/");
            _httpListener.Start();
            Console.WriteLine("Listening...");
            while (true)
            {
                // Note: The GetContext method blocks while waiting for a request
                HttpListenerContext context = _httpListener.GetContext();
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
            }
        }
    }

    public void Stop()
    {
        throw new NotImplementedException();
    }

    public void Start()
    {
        throw new NotImplementedException();
    }

    private Func<Response> GetRoute(String method, String slug)
    {
        foreach (var route in _routes)
        {
            if (route.GetMethod() == method && route.GetSlug() == slug)
            {
                return route.GetCallback();
            }
        }

        // throw new Exception("404 - not found");
        return ()=>new Response(404, "<html><body>404</body></html>");
    }
}