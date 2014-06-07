using Facepunch;
using System;
using System.Runtime.InteropServices;
using uLink;

[StructLayout(LayoutKind.Sequential)]
public struct DamageBeing
{
    public IDBase id;
    public IDMain idMain
    {
        get
        {
            return ((this.id == null) ? null : this.id.idMain);
        }
    }
    public Character character
    {
        get
        {
            return (this.idOwnerMain as Character);
        }
    }
    public IDMain idOwnerMain
    {
        get
        {
            IDMain main = (this.id == null) ? null : this.id.idMain;
            if (main != null)
            {
                if (main is RigidObj)
                {
                    NetworkView ownerView = ((RigidObj) main).ownerView;
                    if (ownerView != null)
                    {
                        return ownerView.GetComponent<IDMain>();
                    }
                    return null;
                }
                if (main is IDeployedObjectMain)
                {
                    DeployedObjectInfo deployedObjectInfo = ((IDeployedObjectMain) main).DeployedObjectInfo;
                    if (deployedObjectInfo.valid)
                    {
                        return deployedObjectInfo.playerCharacter;
                    }
                }
            }
            return main;
        }
    }
    public Controllable controllable
    {
        get
        {
            if (this.id != null)
            {
                IDMain idOwnerMain = this.idOwnerMain;
                if (idOwnerMain == null)
                {
                    return null;
                }
                if (idOwnerMain is Character)
                {
                    return ((Character) idOwnerMain).controllable;
                }
                if (idOwnerMain is IDeployedObjectMain)
                {
                    DeployedObjectInfo deployedObjectInfo = ((IDeployedObjectMain) idOwnerMain).DeployedObjectInfo;
                    if (deployedObjectInfo.valid)
                    {
                        return deployedObjectInfo.playerControllable;
                    }
                }
            }
            return null;
        }
    }
    public PlayerClient client
    {
        get
        {
            if (this.id == null)
            {
                return null;
            }
            IDMain idOwnerMain = this.idOwnerMain;
            if (idOwnerMain == null)
            {
                return null;
            }
            if (idOwnerMain is Character)
            {
                return ((Character) idOwnerMain).playerClient;
            }
            if (idOwnerMain is IDeployedObjectMain)
            {
                DeployedObjectInfo deployedObjectInfo = ((IDeployedObjectMain) idOwnerMain).DeployedObjectInfo;
                if (deployedObjectInfo.valid)
                {
                    return deployedObjectInfo.playerClient;
                }
            }
            Controllable component = idOwnerMain.GetComponent<Controllable>();
            if (component == null)
            {
                return null;
            }
            PlayerClient playerClient = component.playerClient;
            if (playerClient == null)
            {
                NetworkView networkView = component.networkView;
                if (networkView != null)
                {
                    PlayerClient.Find(networkView.owner, out playerClient);
                }
            }
            return playerClient;
        }
    }
    public NetworkView networkView
    {
        get
        {
            if (this.id == null)
            {
                return null;
            }
            IDMain idMain = this.id.idMain;
            if (idMain != null)
            {
                return idMain.networkView;
            }
            return this.id.networkView;
        }
    }
    public NetworkView ownerView
    {
        get
        {
            IDMain main = (this.id == null) ? null : this.id.idMain;
            if (main is RigidObj)
            {
                return ((RigidObj) main).ownerView;
            }
            return this.networkView;
        }
    }
    public NetworkViewID networkViewID
    {
        get
        {
            NetworkView networkView = this.networkView;
            if (networkView != null)
            {
                return networkView.viewID;
            }
            return NetworkViewID.unassigned;
        }
    }
    public NetworkViewID ownerViewID
    {
        get
        {
            NetworkView ownerView = this.ownerView;
            if (ownerView != null)
            {
                return ownerView.viewID;
            }
            return NetworkViewID.unassigned;
        }
    }
    public BodyPart bodyPart
    {
        get
        {
            if ((this.id is IDRemoteBodyPart) && (this.id != null))
            {
                return ((IDRemoteBodyPart) this.id).bodyPart;
            }
            return BodyPart.Undefined;
        }
    }
    public bool Equals(DamageBeing other)
    {
        return (this.id == other.id);
    }

    public override bool Equals(object obj)
    {
        return object.Equals(this.id, obj);
    }

    public override int GetHashCode()
    {
        return ((this.id == null) ? 0 : this.id.GetHashCode());
    }

    public override string ToString()
    {
        if (this.id != null)
        {
            return string.Format("{{id=({0}),idMain=({1})}}", this.id, this.id.idMain);
        }
        return "{{null}}";
    }

    public bool IsDifferentPlayer(PlayerClient exclude)
    {
        if (this.id != null)
        {
            IDMain idOwnerMain = this.idOwnerMain;
            if (idOwnerMain == null)
            {
                idOwnerMain = this.id.idMain;
                if (idOwnerMain == null)
                {
                    return false;
                }
            }
            if (idOwnerMain is Character)
            {
                PlayerClient playerClient = ((Character) idOwnerMain).playerClient;
                return ((playerClient != null) && (playerClient != exclude));
            }
            if (idOwnerMain is IDeployedObjectMain)
            {
                DeployedObjectInfo deployedObjectInfo = ((IDeployedObjectMain) idOwnerMain).DeployedObjectInfo;
                if (deployedObjectInfo.valid)
                {
                    PlayerClient client2 = deployedObjectInfo.playerClient;
                    return ((client2 != null) && (client2 != exclude));
                }
            }
            Controllable component = idOwnerMain.GetComponent<Controllable>();
            if (component != null)
            {
                PlayerClient client3 = component.playerClient;
                return ((client3 != null) && (client3 != exclude));
            }
        }
        return false;
    }

    public ulong userID
    {
        get
        {
            PlayerClient client = this.client;
            if (client != null)
            {
                return client.userID;
            }
            return 0L;
        }
    }
    public static implicit operator IDBase(DamageBeing being)
    {
        return being.id;
    }

    public static bool operator true(DamageBeing being)
    {
        return (being.id == 1);
    }

    public static bool operator false(DamageBeing being)
    {
        return (being.id == 0);
    }
}

