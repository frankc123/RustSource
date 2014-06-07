namespace MoPhoGames.USpeak.Interface
{
    using System;

    public interface ISpeechDataHandler
    {
        void USpeakInitializeSettings(int data);
        void USpeakOnSerializeAudio(byte[] data);
    }
}

