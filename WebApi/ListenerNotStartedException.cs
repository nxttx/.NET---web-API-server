using System;

namespace WebApi;

public class ListenerNotStartedException : NullReferenceException 
{
    public ListenerNotStartedException()
    {
    }

    public ListenerNotStartedException(string message)
        : base(message)
    {
    }

    public ListenerNotStartedException(string message, Exception inner)
        : base(message, inner)
    {
    } 
}