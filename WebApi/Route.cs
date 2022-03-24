using System;
using System.Collections.Generic;

namespace WebApi;

public class Route
{
    private String _path;
    private String _method;
    private Func<RequestData, Response> _callback;
    
    public Route(string path, string method, Func<RequestData, Response> callback)
    {
        _path = path;
        _method = method;
        _callback = callback;
    }

    public String Getpath()
    {
        return _path;
    }

    public string GetMethod()
    {
        return _method;
    }

    public Func<RequestData, Response> GetCallback()
    {
        return _callback;
    }
}