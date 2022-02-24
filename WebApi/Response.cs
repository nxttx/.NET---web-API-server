using System;

namespace WebApi;

public class Response
{
    private int _StatusCode;
    private String _ResponseText;

    public Response(int statusCode, string responseText)
    {
        _StatusCode = statusCode;
        _ResponseText = responseText;
    }

    public int GetStatusCode()
    {
        return _StatusCode;
    }
    public String GetResponseText()
    {
        return _ResponseText;
    }
}