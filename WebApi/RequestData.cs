using System;
using System.Net;

namespace WebApi;

public class RequestData
{
    private String[,] _parameters;
    private String _body;
    private HttpListenerRequest _request;
    
    public String[,] Parameters
    {
        get => _parameters;
    }

    public string Body
    {
        get => _body;
    }

    public HttpListenerRequest Request
    {
        get => _request;
    }
    
    public RequestData(String[,] parameters, string body, HttpListenerRequest request)
    {
        _parameters = parameters ?? throw new ArgumentNullException(nameof(parameters));
        _body = body ?? throw new ArgumentNullException(nameof(body));
        _request = request ?? throw new ArgumentNullException(nameof(request));
    }
}