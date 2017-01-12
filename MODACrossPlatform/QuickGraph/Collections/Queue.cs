namespace QuickGraph.Collections
{
    using System;

#if !SILVERLIGHT
    
#endif
    public sealed class Queue<T> : 
        System.Collections.Generic.Queue<T>,
        IQueue<T>
    {
    }
}
