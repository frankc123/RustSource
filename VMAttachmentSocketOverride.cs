using System;

public class VMAttachmentSocketOverride : ViewModelAttachment
{
    public Socket.CameraSpace socketOverride;

    private void OnDrawGizmosSelected()
    {
        this.socketOverride.DrawGizmos("socketOverride");
    }
}

