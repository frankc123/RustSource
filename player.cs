using System;
using uLink;

public class Player : IDLocalCharacter
{
    private void uLink_OnNetworkInstantiate(NetworkMessageInfo info)
    {
        if (base.networkView.isMine)
        {
            GameTip componentInChildren = base.GetComponentInChildren<GameTip>();
            if (componentInChildren != null)
            {
                componentInChildren.enabled = false;
            }
        }
        if (!base.networkView.isMine)
        {
            GameTip tip2 = base.GetComponentInChildren<GameTip>();
            if ((tip2 != null) && (base.playerClient != null))
            {
                tip2.text = base.playerClient.userName;
            }
        }
    }
}

