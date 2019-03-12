using System;

namespace PubSubWeb.Model
{
    [Serializable]
    public class EventArgs<T> : EventArgs
    {
        public T Argument;

        public EventArgs() : this(default(T))
        {
        }

        public EventArgs(T argument)
        {
            Argument = argument;
        }
    }
}
