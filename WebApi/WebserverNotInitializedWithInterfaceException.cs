using System;

namespace WebApi;

public class WebserverNotInitializedWithInterfaceException: NullReferenceException 
{
    public WebserverNotInitializedWithInterfaceException()
    {
    }

    public WebserverNotInitializedWithInterfaceException(string message)
        : base(message)
    {
    }

    public WebserverNotInitializedWithInterfaceException(string message, Exception inner)
        : base(message, inner)
    {
    } 
}