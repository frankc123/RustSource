using System;
using System.Collections.Generic;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using UnityEngine;

public static class GameConstant
{
    public static Tag GetTag(this Component component)
    {
        return (Tag) component;
    }

    public static Tag GetTag(this GameObject gameObject)
    {
        return (Tag) gameObject;
    }

    public static class Layer
    {
        public const int kMask_BlocksSprite = 0x80401;
        public const int kMask_BloodSplatter = 0x80401;
        public const int kMask_BulletImpact = 0x183e1411;
        public const int kMask_BulletImpactCharacter = 0x18020000;
        public const int kMask_BulletImpactWorld = 0x1c1411;
        public const int kMask_ClientExplosion = 0x8000000;
        public const int kMask_Deployable = -472317957;
        public const int kMask_InfoLabel = -67174405;
        public const int kMask_Melee = 0x183e1411;
        public const int kMask_PlayerMovement = 0x20180403;
        public const int kMask_PlayerPusher = 0x140000;
        public const int kMask_ServerExplosion = 0x10360401;
        public const int kMask_SpawnLand = 0x80401;
        public const int kMask_Use = -201523205;
        public const int kMask_WildlifeMove = -472317957;

        public static class CharacterCollision
        {
            public const int index = 0x10;
            public const int mask = 0x10000;
            public const string name = "Character Collision";
        }

        public static class CullStatic
        {
            public const int index = 12;
            public const int mask = 0x1000;
            public const string name = "CullStatic";
        }

        public static class Debris
        {
            public const int index = 0x12;
            public const int mask = 0x40000;
            public const string name = "Debris";
        }

        public static class Default
        {
            public const int index = 0;
            public const int mask = 1;
            public const string name = "Default";
        }

        public static class GameUI
        {
            public const int index = 0x1f;
            public const int mask = -2147483648;
            public const string name = "GameUI";
        }

        public static class Hitbox
        {
            public const int index = 0x11;
            public const int mask = 0x20000;
            public const string name = "Hitbox";
        }

        public static class HitOnly
        {
            public const int index = 0x15;
            public const int mask = 0x200000;
            public const string name = "HitOnly";
        }

        public static class IgnoreRaycast
        {
            public const int index = 2;
            public const int mask = 4;
            public const string name = "Ignore Raycast";
        }

        public static class Mechanical
        {
            public const int index = 20;
            public const int mask = 0x100000;
            public const string name = "Mechanical";
        }

        public static class MeshBatched
        {
            public const int index = 0x16;
            public const int mask = 0x400000;
            public const string name = "MeshBatched";
        }

        public static class NGUILayer
        {
            public const int index = 8;
            public const int mask = 0x100;
            public const string name = "NGUILayer";
        }

        public static class NGUILayer2D
        {
            public const int index = 9;
            public const int mask = 0x200;
            public const string name = "NGUILayer2D";
        }

        public static class PlayerClip
        {
            public const int index = 0x1d;
            public const int mask = 0x20000000;
            public const string name = "PlayerClip";
        }

        public static class Ragdoll
        {
            public const int index = 0x1b;
            public const int mask = 0x8000000;
            public const string name = "Ragdoll";
        }

        public static class Skybox
        {
            public const int index = 0x17;
            public const int mask = 0x800000;
            public const string name = "Skybox";
        }

        public static class Sprite
        {
            public const int index = 11;
            public const int mask = 0x800;
            public const string name = "Sprite";
        }

        public static class Static
        {
            public const int index = 10;
            public const int mask = 0x400;
            public const string name = "Static";
        }

        public static class Terrain
        {
            public const int index = 0x13;
            public const int mask = 0x80000;
            public const string name = "Terrain";
        }

        public static class TransparentFX
        {
            public const int index = 1;
            public const int mask = 2;
            public const string name = "TransparentFX";
        }

        public static class Vehicle
        {
            public const int index = 0x1c;
            public const int mask = 0x10000000;
            public const string name = "Vehicle";
        }

        public static class ViewModel
        {
            public const int index = 13;
            public const int mask = 0x2000;
            public const string name = "View Model";
        }

        public static class Water
        {
            public const int index = 4;
            public const int mask = 0x10;
            public const string name = "Water";
        }

        public static class Zone
        {
            public const int index = 0x1a;
            public const int mask = 0x4000000;
            public const string name = "Zone";
        }
    }

    [StructLayout(LayoutKind.Sequential)]
    public struct Tag
    {
        private const int kBuiltinTagCount = 7;
        public const int kTagCount = 0x17;
        public const int kCustomTagCount = 0x10;
        public readonly int tagNumber;
        private static readonly GameConstant.TagInfo[] Info;
        private static readonly Dictionary<string, int> Dictionary;
        private Tag(int tagNumber)
        {
            this.tagNumber = tagNumber;
        }

        static Tag()
        {
            Info = new GameConstant.TagInfo[0x17];
            Dictionary = new Dictionary<string, int>(0x17);
            foreach (Type type in typeof(GameConstant.Tag).GetNestedTypes())
            {
                FieldInfo field = type.GetField("tag", BindingFlags.Public | BindingFlags.Static);
                FieldInfo info2 = type.GetField("tagNumber", BindingFlags.Public | BindingFlags.Static);
                FieldInfo info3 = type.GetField("builtin", BindingFlags.Public | BindingFlags.Static);
                if (((field != null) && (info2 != null)) && (info3 != null))
                {
                    try
                    {
                        int index = (int) info2.GetValue(null);
                        string tag = (string) field.GetValue(null);
                        bool builtin = (bool) info3.GetValue(null);
                        Info[index] = new GameConstant.TagInfo(tag, index, builtin);
                    }
                    catch (Exception exception)
                    {
                        Debug.LogError(exception);
                    }
                }
            }
            for (int i = 0; i < 0x17; i++)
            {
                if (!Info[i].valid)
                {
                    Debug.LogWarning(string.Format("Theres no tag specified for index {0}", i));
                }
                else
                {
                    int num4;
                    if (Dictionary.TryGetValue(Info[i].tag, out num4))
                    {
                        Debug.LogWarning(string.Format("Duplicate tag at index {0} will be overriden by predicessor at index {1}", i, num4));
                    }
                    else
                    {
                        Dictionary.Add(Info[i].tag, i);
                    }
                }
            }
        }

        public string tag
        {
            get
            {
                return Info[this.tagNumber].tag;
            }
        }
        public bool builtin
        {
            get
            {
                return Info[this.tagNumber].builtin;
            }
        }
        public bool Contains(GameObject gameObject)
        {
            return ((gameObject != null) && gameObject.CompareTag(Info[this.tagNumber].tag));
        }

        public bool Contains(Component component)
        {
            return ((component != null) && component.CompareTag(Info[this.tagNumber].tag));
        }

        public static int Index(GameObject gameObject)
        {
            for (int i = 0; i < 0x17; i++)
            {
                if (gameObject.CompareTag(Info[i].tag))
                {
                    return i;
                }
            }
            throw new InvalidProgramException(string.Format("There is a tag missing in this class for \"{0}\"", gameObject.tag));
        }

        public static int Index(Component component)
        {
            GameObject gameObject = component.gameObject;
            for (int i = 0; i < 0x17; i++)
            {
                if (gameObject.CompareTag(Info[i].tag))
                {
                    return i;
                }
            }
            throw new InvalidProgramException(string.Format("There is a tag missing in this class for \"{0}\"", gameObject.tag));
        }

        public static int Index(string tag)
        {
            int num;
            if (!Dictionary.TryGetValue(tag, out num))
            {
                throw new InvalidProgramException(string.Format("There is a tag missing in this class for \"{0}\"", tag));
            }
            return num;
        }

        public static explicit operator GameConstant.Tag(GameObject gameObject)
        {
            return new GameConstant.Tag(Index(gameObject));
        }

        public static explicit operator GameConstant.Tag(Component component)
        {
            return new GameConstant.Tag(Index(component));
        }
        public static class Barricade
        {
            public const bool builtin = false;
            public const string tag = "Barricade";
            public const int tagNumber = 13;
            public static readonly GameConstant.Tag value = new GameConstant.Tag(13);
        }

        public static class ClientFolder
        {
            public const bool builtin = false;
            public const string tag = "Client Folder";
            public const int tagNumber = 0x16;
            public static readonly GameConstant.Tag value = new GameConstant.Tag(0x16);
        }

        public static class ClientOnly
        {
            public const bool builtin = false;
            public const string tag = "RPOS Camera";
            public const int tagNumber = 0x13;
            public static readonly GameConstant.Tag value = new GameConstant.Tag(0x13);
        }

        public static class Door
        {
            public const bool builtin = false;
            public const string tag = "Door";
            public const int tagNumber = 12;
            public static readonly GameConstant.Tag value = new GameConstant.Tag(12);
        }

        public static class EditorOnly
        {
            public const bool builtin = true;
            public const string tag = "EditorOnly";
            public const int tagNumber = 3;
            public static readonly GameConstant.Tag value = new GameConstant.Tag(3);
        }

        public static class Finish
        {
            public const bool builtin = true;
            public const string tag = "Finish";
            public const int tagNumber = 2;
            public static readonly GameConstant.Tag value = new GameConstant.Tag(2);
        }

        public static class Folder
        {
            public const bool builtin = false;
            public const string tag = "Folder";
            public const int tagNumber = 20;
            public static readonly GameConstant.Tag value = new GameConstant.Tag(20);
        }

        public static class FPGrass
        {
            public const bool builtin = false;
            public const string tag = "FPGrass";
            public const int tagNumber = 0x11;
            public static readonly GameConstant.Tag value = new GameConstant.Tag(0x11);
        }

        public static class GameController
        {
            public const bool builtin = true;
            public const string tag = "GameController";
            public const int tagNumber = 6;
            public static readonly GameConstant.Tag value = new GameConstant.Tag(6);
        }

        public static class MainCamera
        {
            public const bool builtin = true;
            public const string tag = "MainCamera";
            public const int tagNumber = 4;
            public static readonly GameConstant.Tag value = new GameConstant.Tag(4);
        }

        public static class MainTerrain
        {
            public const bool builtin = false;
            public const string tag = "Main Terrain";
            public const int tagNumber = 8;
            public static readonly GameConstant.Tag value = new GameConstant.Tag(8);
        }

        public static class Meat
        {
            public const bool builtin = false;
            public const string tag = "Meat";
            public const int tagNumber = 10;
            public static readonly GameConstant.Tag value = new GameConstant.Tag(10);
        }

        public static class MeshBatched
        {
            public const bool builtin = false;
            public const string tag = "mBC";
            public const int tagNumber = 15;
            public static readonly GameConstant.Tag value = new GameConstant.Tag(15);
        }

        public static class Player
        {
            public const bool builtin = true;
            public const string tag = "Player";
            public const int tagNumber = 5;
            public static readonly GameConstant.Tag value = new GameConstant.Tag(5);
        }

        public static class Respawn
        {
            public const bool builtin = true;
            public const string tag = "Respawn";
            public const int tagNumber = 1;
            public static readonly GameConstant.Tag value = new GameConstant.Tag(1);
        }

        public static class RPOSCamera
        {
            public const bool builtin = false;
            public const string tag = "RPOS Camera";
            public const int tagNumber = 0x10;
        }

        public static class ServerFolder
        {
            public const bool builtin = false;
            public const string tag = "Server Folder";
            public const int tagNumber = 0x15;
            public static readonly GameConstant.Tag value = new GameConstant.Tag(0x15);
        }

        public static class ServerOnly
        {
            public const bool builtin = false;
            public const string tag = "Server Only";
            public const int tagNumber = 0x12;
            public static readonly GameConstant.Tag value = new GameConstant.Tag(0x12);
        }

        public static class Shelter
        {
            public const bool builtin = false;
            public const string tag = "Shelter";
            public const int tagNumber = 11;
            public static readonly GameConstant.Tag value = new GameConstant.Tag(11);
        }

        public static class SkyboxCamera
        {
            public const bool builtin = false;
            public const string tag = "Skybox Camera";
            public const int tagNumber = 7;
            public static readonly GameConstant.Tag value = new GameConstant.Tag(7);
        }

        public static class StorageBox
        {
            public const bool builtin = false;
            public const string tag = "StorageBox";
            public const int tagNumber = 14;
            public static readonly GameConstant.Tag value = new GameConstant.Tag(14);
        }

        public static class TreeCollider
        {
            public const bool builtin = false;
            public const string tag = "Tree Collider";
            public const int tagNumber = 9;
            public static readonly GameConstant.Tag value = new GameConstant.Tag(9);
        }

        public static class Untagged
        {
            public const bool builtin = true;
            public const string tag = "Untagged";
            public const int tagNumber = 0;
            public static readonly GameConstant.Tag value = new GameConstant.Tag(0);
        }
    }

    [StructLayout(LayoutKind.Sequential)]
    private struct TagInfo
    {
        public readonly string tag;
        public readonly int tagNumber;
        public readonly bool builtin;
        public readonly bool valid;
        public TagInfo(string tag, int tagNumber, bool builtin)
        {
            this.tag = tag;
            this.tagNumber = tagNumber;
            this.builtin = builtin;
            this.valid = true;
        }
    }
}

