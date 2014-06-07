using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using UnityEngine;

public abstract class HUDIndicator : MonoBehaviour
{
    private static double _lastTime;
    private static double _stepTime;
    private UIAnchor anchor;
    private static Target CenterAuto = new Target("HUD_INDICATOR_CENTER_AUTO", UIAnchor.Side.Center);
    private static Target CenterFixed3000Tall = new Target("HUD_INDICATOR_CENTER_3000", 0xbb8, UIAnchor.Side.Center);
    private int listIndex = -1;
    protected static Matrix4x4 worldToCameraLocalMatrix = Matrix4x4.identity;

    protected HUDIndicator()
    {
    }

    protected abstract bool Continue();
    protected Vector3 GetPoint(PlacementSpace space, Vector3 input)
    {
        switch (space)
        {
            case PlacementSpace.World:
            {
                Vector3? nullable = CameraFX.World2Screen(input);
                input = Target.camera.ScreenToWorldPoint(!nullable.HasValue ? Vector3.zero : nullable.Value);
                return input;
            }
            case PlacementSpace.Screen:
                input = Target.camera.ScreenToWorldPoint(input);
                return input;

            case PlacementSpace.Viewport:
                input = Target.camera.ViewportToWorldPoint(input);
                return input;

            case PlacementSpace.Anchor:
                input = this.anchor.transform.TransformPoint(input);
                return input;
        }
        return input;
    }

    protected static HUDIndicator InstantiateIndicator(ScratchTarget target, HUDIndicator prefab)
    {
        return InstantiateIndicator(target, prefab, PlacementSpace.Anchor, Vector3.zero, 0f);
    }

    protected static HUDIndicator InstantiateIndicator(ScratchTarget target, HUDIndicator prefab, PlacementSpace space, Vector3 position)
    {
        return InstantiateIndicator(target, prefab, space, position, 0f);
    }

    private static HUDIndicator InstantiateIndicator(ref Target target, HUDIndicator prefab, PlacementSpace space, Vector3 position, float rotation)
    {
        UIAnchor anchor = target.anchor;
        Quaternion quaternion = Quaternion.AngleAxis(rotation, Vector3.back);
        switch (space)
        {
            case PlacementSpace.World:
            {
                Vector3? nullable = CameraFX.World2Screen(position);
                position = Target.camera.ScreenToWorldPoint(!nullable.HasValue ? Vector3.zero : nullable.Value);
                break;
            }
            case PlacementSpace.Screen:
                position = Target.camera.ScreenToWorldPoint(position);
                break;

            case PlacementSpace.Viewport:
                position = Target.camera.ViewportToWorldPoint(position);
                break;

            case PlacementSpace.Anchor:
                position = anchor.transform.TransformPoint(position);
                quaternion = anchor.transform.rotation * quaternion;
                break;
        }
        position.z = anchor.transform.position.z;
        HUDIndicator indicator = (HUDIndicator) Object.Instantiate(prefab, position, quaternion);
        indicator.transform.parent = anchor.transform;
        indicator.transform.localScale = Vector3.one;
        indicator.anchor = target.anchor;
        return indicator;
    }

    protected static HUDIndicator InstantiateIndicator(ScratchTarget target, HUDIndicator prefab, PlacementSpace space, Vector3 position, float rotation)
    {
        ScratchTarget target2 = target;
        if (target2 == ScratchTarget.CenteredAuto)
        {
            return InstantiateIndicator(ref CenterAuto, prefab, space, position, rotation);
        }
        if (target2 != ScratchTarget.CenteredFixed3000Tall)
        {
            throw new ArgumentOutOfRangeException("target");
        }
        return InstantiateIndicator(ref CenterFixed3000Tall, prefab, space, position, rotation);
    }

    protected void OnDestroy()
    {
        if (this.listIndex != -1)
        {
            INDICATOR.Remove(this);
        }
    }

    protected void Start()
    {
        if (!this.Continue())
        {
            Object.Destroy(base.gameObject);
        }
        else
        {
            INDICATOR.Add(this);
        }
    }

    internal static void Step()
    {
        _stepTime = time;
        Camera main = Camera.main;
        if (main != null)
        {
            worldToCameraLocalMatrix = Matrix4x4.Scale(new Vector3(1f, 1f, -1f)) * main.worldToCameraMatrix;
        }
        int count = INDICATOR.activeIndicators.Count;
        for (int i = count - 1; i >= 0; i--)
        {
            if (INDICATOR.activeIndicators[i].Continue())
            {
                continue;
            }
            int num3 = i;
            do
            {
                HUDIndicator indicator = INDICATOR.activeIndicators[i];
                INDICATOR.activeIndicators.RemoveAt(i);
                indicator.listIndex = -1;
                Object.Destroy(indicator.gameObject);
                count--;
                if (--i >= 0)
                {
                    do
                    {
                        if (!INDICATOR.activeIndicators[i].Continue())
                        {
                            num3 = i;
                            break;
                        }
                    }
                    while (--i >= 0);
                }
            }
            while (num3 == i);
            while (num3 < count)
            {
                INDICATOR.activeIndicators[num3].listIndex = num3;
                num3++;
            }
            break;
        }
    }

    protected static double stepTime
    {
        get
        {
            return _stepTime;
        }
    }

    protected static double time
    {
        get
        {
            return (!DebugInput.GetKey(KeyCode.Period) ? (_lastTime = NetCull.time) : _lastTime);
        }
    }

    private static class INDICATOR
    {
        public static List<HUDIndicator> activeIndicators = new List<HUDIndicator>();

        public static void Add(HUDIndicator hud)
        {
            if (hud.listIndex == -1)
            {
                hud.listIndex = activeIndicators.Count;
                activeIndicators.Add(hud);
            }
        }

        public static void Remove(HUDIndicator hud)
        {
            if (hud.listIndex != -1)
            {
                try
                {
                    activeIndicators.RemoveAt(hud.listIndex);
                    int listIndex = hud.listIndex;
                    int count = activeIndicators.Count;
                    while (listIndex < count)
                    {
                        activeIndicators[listIndex].listIndex = listIndex;
                        listIndex++;
                    }
                }
                finally
                {
                    hud.listIndex = -1;
                }
            }
        }
    }

    protected enum PlacementSpace
    {
        World,
        Screen,
        Viewport,
        Anchor,
        DoNotModify
    }

    protected enum ScratchTarget
    {
        CenteredAuto,
        CenteredFixed3000Tall
    }

    [StructLayout(LayoutKind.Sequential)]
    private struct Target
    {
        private const int kDefaultManualSize = 0x3e8;
        private const UIAnchor.Side kDefaultSide = UIAnchor.Side.Center;
        private UIRoot _root;
        private UIAnchor _anchor;
        public readonly string name;
        public readonly bool automatic;
        public readonly int manualSize;
        public readonly UIAnchor.Side side;
        private static UICamera _uiCamera;
        public Target(string name) : this(name, true, 0x3e8, UIAnchor.Side.Center)
        {
        }

        public Target(string name, int manualSize) : this(name, false, manualSize, UIAnchor.Side.Center)
        {
        }

        public Target(string name, UIAnchor.Side side) : this(name, true, 0x3e8, side)
        {
        }

        public Target(string name, int manualSize, UIAnchor.Side side) : this(name, false, manualSize, side)
        {
        }

        private Target(string name, bool automatic, int manualSize, UIAnchor.Side side)
        {
            this.automatic = automatic;
            this.manualSize = manualSize;
            this.name = name;
            this.side = side;
            this._root = null;
            this._anchor = null;
        }

        public static UICamera uiCamera
        {
            get
            {
                if (_uiCamera == null)
                {
                    _uiCamera = UICamera.FindCameraForLayer(g.layer);
                }
                return _uiCamera;
            }
        }
        public static Camera camera
        {
            get
            {
                UICamera uiCamera = HUDIndicator.Target.uiCamera;
                return ((uiCamera == null) ? null : uiCamera.cachedCamera);
            }
        }
        public UIRoot root
        {
            get
            {
                if (this._root == null)
                {
                    Type[] components = new Type[] { typeof(UIRoot) };
                    this._root = new GameObject(this.name, components) { layer = g.layer }.GetComponent<UIRoot>();
                    this._root.automatic = this.automatic;
                    this._root.manualHeight = this.manualSize;
                }
                return this._root;
            }
        }
        public UIAnchor anchor
        {
            get
            {
                if (this._anchor == null)
                {
                    UIRoot root = this.root;
                    Type[] components = new Type[] { typeof(UIAnchor) };
                    this._anchor = new GameObject("ANCHOR", components) { layer = g.layer }.GetComponent<UIAnchor>();
                    this._anchor.transform.parent = root.transform;
                    this._anchor.side = this.side;
                    this._anchor.uiCamera = camera;
                }
                return this._anchor;
            }
        }
        private static class g
        {
            public static readonly int layer = LayerMask.NameToLayer("NGUILayer2D");
        }
    }
}

