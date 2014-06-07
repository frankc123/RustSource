using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using UnityEngine;

[StructLayout(LayoutKind.Sequential)]
public struct RPOSWindowMessageCenter
{
    public const RPOSWindowMessage kBegin = RPOSWindowMessage.WillShow;
    public const RPOSWindowMessage kLast = RPOSWindowMessage.DidHide;
    public const RPOSWindowMessage kEnd = RPOSWindowMessage.WillClose;
    public const int kMessageCount = 4;
    private RPOSWindowMessageResponder[] responders;
    private bool init;
    private static readonly RPOSWindowMessageHandler[] none;
    static RPOSWindowMessageCenter()
    {
        none = new RPOSWindowMessageHandler[0];
    }

    public void Fire(RPOSWindow window, RPOSWindowMessage message)
    {
        if ((this.init && (message >= RPOSWindowMessage.WillShow)) && (message <= RPOSWindowMessage.DidHide))
        {
            this.responders[((int) message) - 2].Invoke(window, message);
        }
    }

    public bool Add(RPOSWindowMessage message, RPOSWindowMessageHandler handler)
    {
        if (((message < RPOSWindowMessage.WillShow) || (message > RPOSWindowMessage.DidHide)) || (handler == null))
        {
            return false;
        }
        if (!this.init)
        {
            this.responders = new RPOSWindowMessageResponder[4];
            this.init = true;
        }
        return this.responders[((int) message) - 2].Add(handler);
    }

    public bool Remove(RPOSWindowMessage message, RPOSWindowMessageHandler handler)
    {
        return (((this.init && (message >= RPOSWindowMessage.WillShow)) && ((message <= RPOSWindowMessage.DidHide) && (handler != null))) && this.responders[((int) message) - 2].Remove(handler));
    }

    public IEnumerable<RPOSWindowMessageHandler> EnumerateHandlers(RPOSWindowMessage message)
    {
        if ((this.init && (message >= RPOSWindowMessage.WillShow)) && (message <= RPOSWindowMessage.DidHide))
        {
            int index = ((int) message) - 2;
            if (this.responders[index].init && (this.responders[index].count != 0))
            {
                return this.responders[index].handlers;
            }
        }
        return none;
    }

    public int CountHandlers(RPOSWindowMessage message)
    {
        return (((this.init && (message >= RPOSWindowMessage.WillShow)) && (message <= RPOSWindowMessage.DidHide)) ? this.responders[((int) message) - 2].count : 0);
    }
    [StructLayout(LayoutKind.Sequential)]
    private struct RPOSWindowMessageResponder
    {
        public HashSet<RPOSWindowMessageHandler> handlerGate;
        public List<RPOSWindowMessageHandler> handlers;
        public int count;
        public bool init;
        public bool Add(RPOSWindowMessageHandler handler)
        {
            if (handler == null)
            {
                return false;
            }
            if (!this.init)
            {
                this.handlerGate = new HashSet<RPOSWindowMessageHandler>();
                this.handlers = new List<RPOSWindowMessageHandler>();
                this.init = true;
                this.handlerGate.Add(handler);
            }
            else if (!this.handlerGate.Add(handler))
            {
                return false;
            }
            this.handlers.Add(handler);
            this.count++;
            return true;
        }

        public bool Remove(RPOSWindowMessageHandler handler)
        {
            if ((!this.init || (handler == null)) || !this.handlerGate.Remove(handler))
            {
                return false;
            }
            this.handlers.Remove(handler);
            this.count--;
            return true;
        }

        private bool TryInvoke(RPOSWindow window, RPOSWindowMessage message, int i)
        {
            RPOSWindowMessageHandler handler = this.handlers[i];
            try
            {
                return handler(window, message);
            }
            catch (Exception exception)
            {
                Debug.LogError(string.Concat(new object[] { "handler ", handler, " threw exception with message ", message, " on window ", window, " and will no longer execute. The exception is below\r\n", exception }), window);
                return false;
            }
        }

        public void Invoke(RPOSWindow window, RPOSWindowMessage message)
        {
            if (this.init && (this.count != 0))
            {
                if (((message - 2) & RPOSWindowMessage.DidOpen) == RPOSWindowMessage.DidOpen)
                {
                    for (int i = this.count - 1; i >= 0; i--)
                    {
                        if (!this.TryInvoke(window, message, i))
                        {
                            this.handlerGate.Remove(this.handlers[i]);
                            this.handlers.RemoveAt(i);
                            this.count--;
                        }
                    }
                }
                else
                {
                    for (int j = 0; j < this.count; j++)
                    {
                        if (!this.TryInvoke(window, message, j))
                        {
                            this.handlerGate.Remove(this.handlers[j]);
                            this.handlers.RemoveAt(j--);
                            this.count--;
                        }
                    }
                }
            }
        }
    }
}

