using System;

namespace WebApi;

public class Response
{
    private int _statusCode;
    private String _responseText;

    public Response(int statusCode, string responseText)
    {
        _statusCode = statusCode;
        _responseText = responseText;
    }

    public int GetStatusCode()
    {
        return _statusCode;
    }
    public String GetResponseText()
    {
        return _responseText;
    }
}