using System;
using System.Reflection;
using System.Runtime.InteropServices;
using UnityEngine;

public static class LocalPrefs
{
    public static CameraModes CameraMode
    {
        get
        {
            return h.cameraModes;
        }
    }

    public static class BoatCameraMode
    {
        public static void Reset()
        {
            LocalPrefs.g.s.boatCameraMode.Reset();
        }

        public static BoatCameraMode Current
        {
            get
            {
                return (BoatCameraMode) LocalPrefs.g.GetValue(ref LocalPrefs.g.s.boatCameraMode);
            }
            set
            {
                LocalPrefs.g.SetValue(ref LocalPrefs.g.s.boatCameraMode, new int?((int) value));
            }
        }
    }

    public class CameraModes
    {
        public SharedCameraMode this[KindOfCamera kind]
        {
            get
            {
                switch (kind)
                {
                    case KindOfCamera.Foot:
                        return (SharedCameraMode) LocalPrefs.FootCameraMode.Current;

                    case KindOfCamera.MountedWeapon:
                        return (SharedCameraMode) LocalPrefs.MountedWeaponCameraMode.Current;

                    case KindOfCamera.Car:
                        return (SharedCameraMode) LocalPrefs.CarCameraMode.Current;

                    case KindOfCamera.Boat:
                        return (SharedCameraMode) LocalPrefs.BoatCameraMode.Current;

                    case KindOfCamera.Jet:
                        return (SharedCameraMode) LocalPrefs.JetCameraMode.Current;

                    case KindOfCamera.Heli:
                        return (SharedCameraMode) LocalPrefs.HeliCameraMode.Current;
                }
                return SharedCameraMode.Undefined;
            }
            set
            {
                switch (kind)
                {
                    case KindOfCamera.Foot:
                        LocalPrefs.FootCameraMode.Current = (FootCameraMode) value;
                        break;

                    case KindOfCamera.MountedWeapon:
                        LocalPrefs.MountedWeaponCameraMode.Current = (MountedWeaponCameraMode) value;
                        break;

                    case KindOfCamera.Car:
                        LocalPrefs.CarCameraMode.Current = (CarCameraMode) value;
                        break;

                    case KindOfCamera.Boat:
                        LocalPrefs.BoatCameraMode.Current = (BoatCameraMode) value;
                        break;

                    case KindOfCamera.Jet:
                        LocalPrefs.JetCameraMode.Current = (JetCameraMode) value;
                        break;

                    case KindOfCamera.Heli:
                        LocalPrefs.HeliCameraMode.Current = (HeliCameraMode) value;
                        break;
                }
            }
        }
    }

    public static class CarCameraMode
    {
        public static void Reset()
        {
            LocalPrefs.g.s.carCameraMode.Reset();
        }

        public static CarCameraMode Current
        {
            get
            {
                return (CarCameraMode) LocalPrefs.g.GetValue(ref LocalPrefs.g.s.carCameraMode);
            }
            set
            {
                LocalPrefs.g.SetValue(ref LocalPrefs.g.s.carCameraMode, new int?((int) value));
            }
        }
    }

    public static class FootCameraMode
    {
        public static void Reset()
        {
            LocalPrefs.g.s.footCameraMode.Reset();
        }

        public static FootCameraMode Current
        {
            get
            {
                return (FootCameraMode) LocalPrefs.g.GetValue(ref LocalPrefs.g.s.footCameraMode);
            }
            set
            {
                LocalPrefs.g.SetValue(ref LocalPrefs.g.s.footCameraMode, new int?((int) value));
            }
        }
    }

    private static class g
    {
        public static bool GetValue(ref KeyDefault<bool> k)
        {
            if (k.Init())
            {
                k.value = PlayerPrefs.GetInt(k.key) != 0;
            }
            return k.value;
        }

        public static int GetValue(ref KeyDefault<int> k)
        {
            if (k.Init())
            {
                k.value = PlayerPrefs.GetInt(k.key);
            }
            return k.value;
        }

        public static float GetValue(ref KeyDefault<float> k)
        {
            if (k.Init())
            {
                k.value = PlayerPrefs.GetFloat(k.key);
            }
            return k.value;
        }

        public static string GetValue(ref KeyDefault<string> k)
        {
            if (k.Init())
            {
                k.value = PlayerPrefs.GetString(k.key);
            }
            return k.value;
        }

        private static bool NeedSetValue<T>(ref KeyDefault<T> k, bool isNull)
        {
            if (isNull)
            {
                if (k.set || k.Init())
                {
                    k.Reset();
                }
                return false;
            }
            k.Init();
            return true;
        }

        private static bool NeedSetValue<T>(ref KeyDefault<T> k, T? value) where T: struct
        {
            if (NeedSetValue<T>(ref k, !value.HasValue))
            {
                k.value = value.Value;
                return true;
            }
            return false;
        }

        private static bool NeedSetValue<T>(ref KeyDefault<T> k, T value) where T: class
        {
            if (NeedSetValue<T>(ref k, value == null))
            {
                k.value = value;
                return true;
            }
            return false;
        }

        private static KeyDefault<int> New(string key, Enum @default)
        {
            return new KeyDefault<int>(key, Convert.ToInt32(@default));
        }

        private static KeyDefault<T> New<T>(string key, T @default)
        {
            return new KeyDefault<T>(key, @default);
        }

        public static void SetValue(ref KeyDefault<bool> k, bool? value)
        {
            if ((NeedSetValue<bool>(ref k, value) && !k.set) || (k.value != value.Value))
            {
                PlayerPrefs.SetInt(k.key, !value.Value ? 0 : 1);
            }
        }

        public static void SetValue(ref KeyDefault<int> k, int? value)
        {
            if ((NeedSetValue<int>(ref k, value) && !k.set) || (k.value != value.Value))
            {
                PlayerPrefs.SetInt(k.key, value.Value);
            }
        }

        public static void SetValue(ref KeyDefault<float> k, float? value)
        {
            if ((NeedSetValue<float>(ref k, value) && !k.set) || (k.value != value.Value))
            {
                PlayerPrefs.SetFloat(k.key, value.Value);
            }
        }

        public static void SetValue(ref KeyDefault<string> k, string value)
        {
            if ((NeedSetValue<string>(ref k, value) && !k.set) || (k.value != value))
            {
                PlayerPrefs.SetString(k.key, value);
            }
        }

        [StructLayout(LayoutKind.Sequential)]
        public struct KeyDefault<T>
        {
            public readonly string key;
            public readonly T @default;
            public T value;
            public bool uninit;
            public bool set;
            public KeyDefault(string key, T value)
            {
                this.key = key;
                this.@default = value;
                this.value = this.@default;
                this.uninit = false;
                this.set = false;
            }

            public bool Init()
            {
                if (this.uninit)
                {
                    this.set = PlayerPrefs.HasKey(this.key);
                    this.uninit = false;
                    if (this.set)
                    {
                        return true;
                    }
                    this.value = this.@default;
                }
                return false;
            }

            public void Reset()
            {
                if (this.set || this.Init())
                {
                    PlayerPrefs.DeleteKey(this.key);
                    this.set = false;
                    this.value = this.@default;
                }
            }
        }

        public static class s
        {
            public static LocalPrefs.g.KeyDefault<int> boatCameraMode = LocalPrefs.g.New<int>("boat_cam", 2);
            public static LocalPrefs.g.KeyDefault<int> carCameraMode = LocalPrefs.g.New<int>("car_cam", 2);
            public static LocalPrefs.g.KeyDefault<int> footCameraMode = LocalPrefs.g.New<int>("foot_cam", 1);
            public static LocalPrefs.g.KeyDefault<int> heliCameraMode = LocalPrefs.g.New<int>("heli_cam", 2);
            public static LocalPrefs.g.KeyDefault<int> jetCameraMode = LocalPrefs.g.New<int>("jet_cam", 2);
            public static LocalPrefs.g.KeyDefault<int> mountedWeaponCameraMode = LocalPrefs.g.New<int>("mwep_cam", 1);
            public static LocalPrefs.g.KeyDefault<int> passengerCameraMode = LocalPrefs.g.New<int>("pass_cam", 1);
        }
    }

    private static class h
    {
        public static readonly LocalPrefs.CameraModes cameraModes = new LocalPrefs.CameraModes();
    }

    public static class HeliCameraMode
    {
        public static void Reset()
        {
            LocalPrefs.g.s.heliCameraMode.Reset();
        }

        public static HeliCameraMode Current
        {
            get
            {
                return (HeliCameraMode) LocalPrefs.g.GetValue(ref LocalPrefs.g.s.heliCameraMode);
            }
            set
            {
                LocalPrefs.g.SetValue(ref LocalPrefs.g.s.heliCameraMode, new int?((int) value));
            }
        }
    }

    public static class JetCameraMode
    {
        public static void Reset()
        {
            LocalPrefs.g.s.jetCameraMode.Reset();
        }

        public static JetCameraMode Current
        {
            get
            {
                return (JetCameraMode) LocalPrefs.g.GetValue(ref LocalPrefs.g.s.jetCameraMode);
            }
            set
            {
                LocalPrefs.g.SetValue(ref LocalPrefs.g.s.jetCameraMode, new int?((int) value));
            }
        }
    }

    public static class MountedWeaponCameraMode
    {
        public static void Reset()
        {
            LocalPrefs.g.s.mountedWeaponCameraMode.Reset();
        }

        public static MountedWeaponCameraMode Current
        {
            get
            {
                return (MountedWeaponCameraMode) LocalPrefs.g.GetValue(ref LocalPrefs.g.s.mountedWeaponCameraMode);
            }
            set
            {
                LocalPrefs.g.SetValue(ref LocalPrefs.g.s.mountedWeaponCameraMode, new int?((int) value));
            }
        }
    }
}

