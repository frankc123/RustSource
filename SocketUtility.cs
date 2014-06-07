using System;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using UnityEngine;

public static class SocketUtility
{
    public static bool ContainsSocket(this Socket.Map map, int index)
    {
        Socket socket;
        return map.FindSocket(index, out socket);
    }

    public static bool ContainsSocket<TSocket>(this Socket.Map map, int index) where TSocket: Socket, new()
    {
        TSocket local;
        return map.FindSocket<TSocket>(index, out local);
    }

    public static bool ContainsSocket(this Socket.Map map, string name)
    {
        Socket socket;
        return map.FindSocket(name, out socket);
    }

    public static bool ContainsSocket<TSocket>(this Socket.Map map, string name) where TSocket: Socket, new()
    {
        TSocket local;
        return map.FindSocket<TSocket>(name, out local);
    }

    public static bool ContainsSocket(this Socket.Mapped mapped, int index)
    {
        Socket socket;
        return mapped.GetSocketMapOrNull().FindSocket(index, out socket);
    }

    public static bool ContainsSocket<TSocket>(this Socket.Mapped mapped, int index) where TSocket: Socket, new()
    {
        TSocket local;
        return mapped.GetSocketMapOrNull().FindSocket<TSocket>(index, out local);
    }

    public static bool ContainsSocket(this Socket.Mapped mapped, string name)
    {
        Socket socket;
        return mapped.GetSocketMapOrNull().FindSocket(name, out socket);
    }

    public static bool ContainsSocket<TSocket>(this Socket.Mapped mapped, string name) where TSocket: Socket, new()
    {
        TSocket local;
        return mapped.GetSocketMapOrNull().FindSocket<TSocket>(name, out local);
    }

    public static bool FindSocket(this Socket.Map map, int index, out Socket socket)
    {
        Socket.Map.Reference reference = map;
        return reference.Socket(index, out socket);
    }

    public static bool FindSocket<TSocket>(this Socket.Map map, int index, out TSocket socket) where TSocket: Socket, new()
    {
        Socket.Map.Reference reference = map;
        return reference.Socket<TSocket>(index, out socket);
    }

    public static bool FindSocket(this Socket.Map map, string name, out Socket socket)
    {
        Socket.Map.Reference reference = map;
        return reference.Socket(name, out socket);
    }

    public static bool FindSocket<TSocket>(this Socket.Map map, string name, out TSocket socket) where TSocket: Socket, new()
    {
        Socket.Map.Reference reference = map;
        return reference.Socket<TSocket>(name, out socket);
    }

    public static bool FindSocket(this Socket.Mapped mapped, int index, out Socket socket)
    {
        return mapped.GetSocketMapOrNull().Socket(index, out socket);
    }

    public static bool FindSocket<TSocket>(this Socket.Mapped mapped, int index, out TSocket socket) where TSocket: Socket, new()
    {
        return mapped.GetSocketMapOrNull().Socket<TSocket>(index, out socket);
    }

    public static bool FindSocket(this Socket.Mapped mapped, string name, out Socket socket)
    {
        return mapped.GetSocketMapOrNull().Socket(name, out socket);
    }

    public static bool FindSocket<TSocket>(this Socket.Mapped mapped, string name, out TSocket socket) where TSocket: Socket, new()
    {
        return mapped.GetSocketMapOrNull().Socket<TSocket>(name, out socket);
    }

    public static Socket.Map GetSocketMapOrNull(this Socket.Mapped mapped)
    {
        return ((!object.ReferenceEquals(mapped, null) && (mapped is Object)) ? mapped.socketMap : null);
    }

    public static bool GetSocketMapOrNull(this Socket.Mapped mapped, out Socket.Map map)
    {
        if (object.ReferenceEquals(mapped, null) || !(mapped is Object))
        {
            map = null;
            return false;
        }
        map = mapped.socketMap;
        return !object.ReferenceEquals(map, null);
    }

    public static void Play(this AudioClip clip, Socket socket, bool parent, float volume, float pitch, AudioRolloffMode rolloffMode, float minDistance, float maxDistance, float doppler, float spread, bool bypassEffects)
    {
        if (socket == null)
        {
            if (parent)
            {
                socket.Snap();
                clip.Play(socket.attachParent, socket.position, socket.rotation, volume, pitch, rolloffMode, minDistance, maxDistance, doppler, spread, bypassEffects);
            }
            else
            {
                clip.Play(socket.position, socket.rotation, volume, pitch, rolloffMode, minDistance, maxDistance, doppler, spread, bypassEffects);
            }
        }
    }

    public static int SocketIndex(this Socket.Map map, string name)
    {
        int num;
        map.SocketIndex(name, out num);
        return num;
    }

    public static int SocketIndex(this Socket.Mapped mapped, string name)
    {
        int num;
        mapped.GetSocketMapOrNull().SocketIndex(name, out num);
        return num;
    }
}

