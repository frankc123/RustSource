namespace MoPhoGames.USpeak.Interface
{
    using System;

    public interface IUSpeakTalkController
    {
        void OnInspectorGUI();
        bool ShouldSend();
    }
}

