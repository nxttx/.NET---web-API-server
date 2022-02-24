using System;

namespace WebApi;

public class Route
{
    private String _slug;
    private String _method;
    private Func<Response> _callback;
    
    public Route(string slug, string method, Func<Response> callback)
    {
        this._slug = slug;
        this._method = method;
        this._callback = callback;
    }

    public String GetSlug()
    {
        return _slug;
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