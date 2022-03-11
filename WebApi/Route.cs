using System;

namespace WebApi;

public class Route
{
    private String _path;
    private String _method;
    private Func<Response> _callback;
    
    public Route(string path, string method, Func<Response> callback)
    {
        this._path = path;
        this._method = method;
        this._callback = callback;
    }

    public String Getpath()
    {
        return _path;
    }

    public string GetMethod()
    {
        return _method;
    }

    public Func<Response> GetCallback()
    {
        return _callback;
    }
}