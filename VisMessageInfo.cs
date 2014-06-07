using System;

public sealed class VisMessageInfo : IDisposable
{
    private Kind _kind;
    private VisNode _other;
    private VisReactor _self;
    private static VisMessageInfo dump;
    private VisMessageInfo next;

    private VisMessageInfo()
    {
    }

    public static VisMessageInfo Create(VisReactor issuer, Kind kind)
    {
        VisMessageInfo dump;
        if (VisMessageInfo.dump != null)
        {
            dump = VisMessageInfo.dump;
            VisMessageInfo.dump = dump.next;
            dump.next = null;
        }
        else
        {
            dump = new VisMessageInfo();
        }
        dump._self = issuer;
        dump._other = null;
        dump._kind = kind;
        return dump;
    }

    public static VisMessageInfo Create(VisReactor issuer, VisNode other, Kind kind)
    {
        VisMessageInfo dump;
        if (VisMessageInfo.dump != null)
        {
            dump = VisMessageInfo.dump;
            VisMessageInfo.dump = dump.next;
            dump.next = null;
        }
        else
        {
            dump = new VisMessageInfo();
        }
        dump._self = issuer;
        dump._other = other;
        dump._kind = kind;
        return dump;
    }

    void IDisposable.Dispose()
    {
        if (this._kind != ((Kind) 0))
        {
            this._kind = (Kind) 0;
            this.next = dump;
            dump = this;
            this._other = null;
            this._self = null;
        }
    }

    public bool isSeeEvent
    {
        get
        {
            return ((this._kind & Kind.SeeEnter) == Kind.SeeEnter);
        }
    }

    public bool isSpectatingEvent
    {
        get
        {
            return (((((int) this._kind) - 1) & ((int) Kind.SeeEnter)) == ((int) Kind.SeeEnter));
        }
    }

    public VisReactor issuer
    {
        get
        {
            return this._self;
        }
    }

    public bool isTwoNodeEvent
    {
        get
        {
            return (this._kind > Kind.SpectatorExit);
        }
    }

    public Kind kind
    {
        get
        {
            return this._kind;
        }
    }

    public VisNode other
    {
        get
        {
            return this._other;
        }
    }

    public VisNode seeer
    {
        get
        {
            return this.spectator;
        }
    }

    public VisNode seenNode
    {
        get
        {
            return this.spectated;
        }
    }

    public VisNode self
    {
        get
        {
            return this._self.node;
        }
    }

    public VisNode sender
    {
        get
        {
            return this._self.node;
        }
    }

    public VisNode spectated
    {
        get
        {
            return (!this.isSeeEvent ? this.self : this._other);
        }
    }

    public VisNode spectator
    {
        get
        {
            return (!this.isSpectatingEvent ? this.self : this._other);
        }
    }

    public VisNode target
    {
        get
        {
            return this._other;
        }
    }

    public enum Kind : byte
    {
        SeeAdd = 5,
        SeeEnter = 1,
        SeeExit = 3,
        SeeRemove = 7,
        SpectatedEnter = 2,
        SpectatorAdd = 8,
        SpectatorExit = 4,
        SpectatorRemove = 10
    }
}

