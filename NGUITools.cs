using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Text;
using UnityEngine;

public static class NGUITools
{
    public const char kFormattingOffDisableCharacter = '\x00ab';
    public const string kFormattingOffDisableSymbol = "[\x00ab]";
    public const char kFormattingOffEnableCharacter = '\x00bb';
    public const string kFormattingOffEnableSymbol = "[\x00bb]";
    private static readonly string[] kFormattingOffSymbols = new string[] { "[\x00bb]", "[\x00ab]" };
    public const float kMaximumNegativeAlpha = -0.001960784f;
    public const float kMinimumAlpha = 0.001960784f;
    private static float mGlobalVolume = 1f;
    private static AudioListener mListener;
    private static bool mLoaded = false;

    private static void Activate(Transform t)
    {
        t.gameObject.SetActive(true);
    }

    public static GameObject AddChild(GameObject parent)
    {
        GameObject obj2 = new GameObject();
        if (parent != null)
        {
            Transform transform = obj2.transform;
            transform.parent = parent.transform;
            transform.localPosition = Vector3.zero;
            transform.localRotation = Quaternion.identity;
            transform.localScale = Vector3.one;
            obj2.layer = parent.layer;
        }
        return obj2;
    }

    public static T AddChild<T>(GameObject parent) where T: Component
    {
        GameObject obj2 = AddChild(parent);
        obj2.name = GetName<T>();
        return obj2.AddComponent<T>();
    }

    public static GameObject AddChild(GameObject parent, GameObject prefab)
    {
        GameObject obj2 = Object.Instantiate(prefab) as GameObject;
        if ((obj2 != null) && (parent != null))
        {
            Transform transform = obj2.transform;
            transform.parent = parent.transform;
            transform.localPosition = Vector3.zero;
            transform.localRotation = Quaternion.identity;
            transform.localScale = Vector3.one;
            obj2.layer = parent.layer;
        }
        return obj2;
    }

    public static UISprite AddSprite(GameObject go, UIAtlas atlas, string spriteName)
    {
        UIAtlas.Sprite sprite = (atlas == null) ? null : atlas.GetSprite(spriteName);
        UISprite sprite2 = ((sprite != null) && !(sprite.inner == sprite.outer)) ? ((UISprite) AddWidget<UISlicedSprite>(go)) : AddWidget<UISprite>(go);
        sprite2.atlas = atlas;
        sprite2.spriteName = spriteName;
        return sprite2;
    }

    public static T AddWidget<T>(GameObject go) where T: UIWidget
    {
        int num = CalculateNextDepth(go);
        T local = AddChild<T>(go);
        local.depth = num;
        Transform transform = local.transform;
        transform.localPosition = Vector3.zero;
        transform.localRotation = Quaternion.identity;
        transform.localScale = new Vector3(100f, 100f, 1f);
        local.gameObject.layer = go.layer;
        return local;
    }

    [Obsolete("Use AddWidgetHotSpot")]
    public static BoxCollider AddWidgetCollider(GameObject go)
    {
        if (go == null)
        {
            return null;
        }
        Collider component = go.GetComponent<Collider>();
        BoxCollider collider2 = component as BoxCollider;
        if (collider2 == null)
        {
            if (component != null)
            {
                if (Application.isPlaying)
                {
                    Object.Destroy(component);
                }
                else
                {
                    Object.DestroyImmediate(component);
                }
            }
            collider2 = go.AddComponent<BoxCollider>();
        }
        int num = CalculateNextDepth(go);
        AABBox box = NGUIMath.CalculateRelativeWidgetBounds(go.transform);
        collider2.isTrigger = true;
        collider2.center = box.center + ((Vector3) (Vector3.back * (num * 0.25f)));
        collider2.size = new Vector3(box.size.x, box.size.y, 0f);
        return collider2;
    }

    public static UIHotSpot AddWidgetHotSpot(GameObject go)
    {
        int num;
        AABBox box;
        if (go == null)
        {
            return null;
        }
        Collider collider = go.collider;
        if (collider != null)
        {
            UIHotSpot spot = ColliderToHotSpot(collider, true);
            if (spot == null)
            {
                return null;
            }
            return spot;
        }
        UIHotSpot component = go.GetComponent<UIHotSpot>();
        if (component != null)
        {
            if (component.isRect)
            {
                UIRectHotSpot asRect = component.asRect;
                num = CalculateNextDepth(go);
                box = NGUIMath.CalculateRelativeWidgetBounds(go.transform);
                asRect.size = box.size;
                asRect.center = box.center + ((Vector3) (Vector3.back * (num * 0.25f)));
                return asRect;
            }
            if (Application.isPlaying)
            {
                Object.Destroy(component);
            }
            else
            {
                Object.DestroyImmediate(component);
            }
        }
        num = CalculateNextDepth(go);
        box = NGUIMath.CalculateRelativeWidgetBounds(go.transform);
        UIRectHotSpot spot4 = go.AddComponent<UIRectHotSpot>();
        spot4.size = box.size;
        spot4.center = box.center + ((Vector3) (Vector3.back * (num * 0.25f)));
        return spot4;
    }

    public static void Broadcast(string funcName)
    {
        GameObject[] objArray = Object.FindObjectsOfType(typeof(GameObject)) as GameObject[];
        int index = 0;
        int length = objArray.Length;
        while (index < length)
        {
            objArray[index].SendMessage(funcName, SendMessageOptions.DontRequireReceiver);
            index++;
        }
    }

    public static void Broadcast(string funcName, object param)
    {
        GameObject[] objArray = Object.FindObjectsOfType(typeof(GameObject)) as GameObject[];
        int index = 0;
        int length = objArray.Length;
        while (index < length)
        {
            objArray[index].SendMessage(funcName, param, SendMessageOptions.DontRequireReceiver);
            index++;
        }
    }

    public static int CalculateNextDepth(GameObject go)
    {
        int a = -1;
        UIWidget[] componentsInChildren = go.GetComponentsInChildren<UIWidget>();
        int index = 0;
        int length = componentsInChildren.Length;
        while (index < length)
        {
            a = Mathf.Max(a, componentsInChildren[index].depth);
            index++;
        }
        return (a + 1);
    }

    private static void ColliderDestroy(Collider component)
    {
        if (Application.isPlaying)
        {
            Object.Destroy(component);
        }
        else
        {
            Object.DestroyImmediate(component);
        }
    }

    public static UIHotSpot ColliderToHotSpot(BoxCollider collider)
    {
        return ColliderToHotSpot(collider, false);
    }

    public static UIHotSpot ColliderToHotSpot(Collider collider)
    {
        return ColliderToHotSpot(collider, false);
    }

    private static UIHotSpot ColliderToHotSpot(BoxCollider collider, bool nullChecked)
    {
        if (!nullChecked && (collider == null))
        {
            return null;
        }
        Vector3 center = collider.center;
        Vector3 size = collider.size;
        GameObject gameObject = collider.gameObject;
        bool enabled = collider.enabled;
        ColliderDestroy(collider);
        if (size.z <= 0.001f)
        {
            UIRectHotSpot spot = gameObject.AddComponent<UIRectHotSpot>();
            spot.center = center;
            spot.size = size;
            spot.enabled = enabled;
            return spot;
        }
        UIBoxHotSpot spot2 = gameObject.AddComponent<UIBoxHotSpot>();
        spot2.center = center;
        spot2.size = size;
        spot2.enabled = enabled;
        return spot2;
    }

    private static UIHotSpot ColliderToHotSpot(Collider collider, bool nullChecked)
    {
        Bounds bounds;
        if (!nullChecked && (collider == null))
        {
            return null;
        }
        if (collider is BoxCollider)
        {
            return ColliderToHotSpot((BoxCollider) collider);
        }
        if (collider is SphereCollider)
        {
            return ColliderToHotSpot((SphereCollider) collider);
        }
        if (collider is TerrainCollider)
        {
            Debug.Log("Sorry not going to convert a terrain collider.. that sounds destructive.", collider);
            return null;
        }
        Bounds boundsSrc = collider.bounds;
        Matrix4x4 worldToLocalMatrix = collider.transform.worldToLocalMatrix;
        AABBox.Transform3x4(ref boundsSrc, ref worldToLocalMatrix, out bounds);
        bool enabled = collider.enabled;
        GameObject gameObject = collider.gameObject;
        ColliderDestroy(collider);
        Vector3 size = bounds.size;
        if (size.z <= 0.001f)
        {
            UIRectHotSpot spot = gameObject.AddComponent<UIRectHotSpot>();
            spot.size = size;
            spot.center = bounds.center;
            spot.enabled = enabled;
            return spot;
        }
        UIBoxHotSpot spot2 = gameObject.AddComponent<UIBoxHotSpot>();
        spot2.size = size;
        spot2.center = bounds.center;
        spot2.enabled = enabled;
        return spot2;
    }

    public static UIBoxHotSpot ColliderToHotSpotBox(BoxCollider collider)
    {
        return ColliderToHotSpotBox(collider, false);
    }

    private static UIBoxHotSpot ColliderToHotSpotBox(BoxCollider collider, bool nullChecked)
    {
        if (!nullChecked && (collider == null))
        {
            return null;
        }
        Vector3 center = collider.center;
        Vector3 size = collider.size;
        GameObject gameObject = collider.gameObject;
        bool enabled = collider.enabled;
        ColliderDestroy(collider);
        UIBoxHotSpot spot = gameObject.AddComponent<UIBoxHotSpot>();
        spot.center = center;
        spot.size = size;
        spot.enabled = enabled;
        return spot;
    }

    public static UIRectHotSpot ColliderToHotSpotRect(BoxCollider collider)
    {
        return ColliderToHotSpotRect(collider, false);
    }

    private static UIRectHotSpot ColliderToHotSpotRect(BoxCollider collider, bool nullChecked)
    {
        if (!nullChecked && (collider == null))
        {
            return null;
        }
        Vector3 center = collider.center;
        Vector2 size = collider.size;
        GameObject gameObject = collider.gameObject;
        bool enabled = collider.enabled;
        ColliderDestroy(collider);
        UIRectHotSpot spot = gameObject.AddComponent<UIRectHotSpot>();
        spot.center = center;
        spot.size = size;
        spot.enabled = enabled;
        return spot;
    }

    private static void Deactivate(Transform t)
    {
        t.gameObject.SetActive(false);
    }

    public static void Destroy(Object obj)
    {
        if (obj != null)
        {
            if (Application.isPlaying)
            {
                Object.Destroy(obj);
            }
            else
            {
                Object.DestroyImmediate(obj);
            }
        }
    }

    public static void DestroyImmediate(Object obj)
    {
        if (obj != null)
        {
            if (Application.isEditor)
            {
                Object.DestroyImmediate(obj);
            }
            else
            {
                Object.Destroy(obj);
            }
        }
    }

    public static string EncodeColor(Color c)
    {
        int num = 0xffffff & (NGUIMath.ColorToInt(c) >> 8);
        return num.ToString("X6");
    }

    public static T[] FindActive<T>() where T: Component
    {
        return (Object.FindObjectsOfType(typeof(T)) as T[]);
    }

    public static Camera FindCameraForLayer(int layer)
    {
        int num = ((int) 1) << layer;
        Camera[] cameraArray = FindActive<Camera>();
        int index = 0;
        int length = cameraArray.Length;
        while (index < length)
        {
            Camera camera = cameraArray[index];
            if ((camera.cullingMask & num) != 0)
            {
                return camera;
            }
            index++;
        }
        return null;
    }

    public static T FindInParents<T>(GameObject go) where T: Component
    {
        if (go == null)
        {
            return null;
        }
        object component = go.GetComponent<T>();
        if (component == null)
        {
            for (Transform transform = go.transform.parent; (transform != null) && (component == null); transform = transform.parent)
            {
                component = transform.gameObject.GetComponent<T>();
            }
        }
        return (T) component;
    }

    public static bool GetAllowClick(MonoBehaviour self)
    {
        bool flag;
        return GetAllowClick(self, out flag);
    }

    public static bool GetAllowClick(MonoBehaviour self, out bool possible)
    {
        Collider collider = self.collider;
        if (collider != null)
        {
            possible = true;
            return collider.enabled;
        }
        UIHotSpot component = self.GetComponent<UIHotSpot>();
        if (component != null)
        {
            possible = true;
            return component.enabled;
        }
        possible = false;
        return false;
    }

    public static bool GetCentroid(Component cell, out Vector3 centroid)
    {
        if (cell is Collider)
        {
            centroid = ((Collider) cell).bounds.center;
        }
        else if (cell is UIHotSpot)
        {
            centroid = ((UIHotSpot) cell).worldCenter;
        }
        else
        {
            UIHotSpot component = cell.GetComponent<UIHotSpot>();
            if (component != null)
            {
                centroid = component.worldCenter;
                return true;
            }
            Collider collider = cell.collider;
            if (collider != null)
            {
                centroid = collider.bounds.center;
                return true;
            }
            centroid = Vector3.zero;
            return false;
        }
        return true;
    }

    public static string GetHierarchy(GameObject obj)
    {
        string name = obj.name;
        while (obj.transform.parent != null)
        {
            obj = obj.transform.parent.gameObject;
            name = obj.name + "/" + name;
        }
        return ("\"" + name + "\"");
    }

    public static string GetName<T>() where T: Component
    {
        string str = typeof(T).ToString();
        if (str.StartsWith("UI"))
        {
            return str.Substring(2);
        }
        if (str.StartsWith("UnityEngine."))
        {
            str = str.Substring(12);
        }
        return str;
    }

    public static TComponent GetOrAddComponent<TComponent>(Component component) where TComponent: Component
    {
        if (component is TComponent)
        {
            return (TComponent) component;
        }
        return GetOrAddComponent<TComponent>(component.gameObject);
    }

    public static TComponent GetOrAddComponent<TComponent>(GameObject gameObject) where TComponent: Component
    {
        TComponent local = QuickGet<TComponent>(gameObject);
        return ((local == null) ? gameObject.AddComponent<TComponent>() : local);
    }

    public static bool GetOrAddComponent<TComponent>(Component component, ref TComponent value) where TComponent: Component
    {
        TComponent local;
        return ((((TComponent) value) == null) ? ((bool) local) : ((bool) value));
    }

    public static bool GetOrAddComponent<TComponent>(GameObject gameObject, ref TComponent value) where TComponent: Component
    {
        TComponent local;
        return ((((TComponent) value) == null) ? ((bool) local) : ((bool) value));
    }

    public static bool HasMeansOfClicking(Component self)
    {
        return ((self.collider != null) || ((bool) self.GetComponent<UIHotSpot>()));
    }

    public static bool IsChild(Transform parent, Transform child)
    {
        if ((parent != null) && (child != null))
        {
            while (child != null)
            {
                if (child == parent)
                {
                    return true;
                }
                child = child.parent;
            }
        }
        return false;
    }

    public static void MakePixelPerfect(Transform t)
    {
        UIWidget component = t.GetComponent<UIWidget>();
        if (component != null)
        {
            component.MakePixelPerfect();
        }
        else
        {
            t.localPosition = Round(t.localPosition);
            t.localScale = Round(t.localScale);
            int index = 0;
            int childCount = t.childCount;
            while (index < childCount)
            {
                MakePixelPerfect(t.GetChild(index));
                index++;
            }
        }
    }

    public static WWW OpenURL(string url)
    {
        WWW www = null;
        try
        {
            www = new WWW(url);
        }
        catch (Exception exception)
        {
            Debug.LogError(exception.Message);
        }
        return www;
    }

    public static Color ParseColor(string text, int offset)
    {
        int num = (NGUIMath.HexToDecimal(text[offset]) << 4) | NGUIMath.HexToDecimal(text[offset + 1]);
        int num2 = (NGUIMath.HexToDecimal(text[offset + 2]) << 4) | NGUIMath.HexToDecimal(text[offset + 3]);
        int num3 = (NGUIMath.HexToDecimal(text[offset + 4]) << 4) | NGUIMath.HexToDecimal(text[offset + 5]);
        float num4 = 0.003921569f;
        return new Color(num4 * num, num4 * num2, num4 * num3);
    }

    public static int ParseSymbol(string text, int index, List<Color> colors, ref int symbolSkipCount)
    {
        int length = text.Length;
        if ((index + 2) < length)
        {
            if (text[index + 2] == ']')
            {
                if (text[index + 1] != '-')
                {
                    if (text[index + 1] != '\x00bb')
                    {
                        if ((text[index + 1] == '\x00ab') && (--symbolSkipCount == 0))
                        {
                            return 3;
                        }
                    }
                    else if (symbolSkipCount++ == 0)
                    {
                        return 3;
                    }
                }
                else if (symbolSkipCount == 0)
                {
                    if ((colors != null) && (colors.Count > 1))
                    {
                        colors.RemoveAt(colors.Count - 1);
                    }
                    return 3;
                }
            }
            else if ((((index + 7) < length) && (text[index + 7] == ']')) && (symbolSkipCount == 0))
            {
                if (colors != null)
                {
                    Color item = ParseColor(text, index + 1);
                    Color color2 = colors[colors.Count - 1];
                    item.a = color2.a;
                    colors.Add(item);
                }
                return 8;
            }
        }
        return 0;
    }

    public static AudioSource PlaySound(AudioClip clip)
    {
        return PlaySound(clip, 1f, 1f);
    }

    public static AudioSource PlaySound(AudioClip clip, float volume)
    {
        return PlaySound(clip, volume, 1f);
    }

    public static AudioSource PlaySound(AudioClip clip, float volume, float pitch)
    {
        volume *= soundVolume;
        if ((clip != null) && (volume > 0.01f))
        {
            if (mListener == null)
            {
                mListener = Object.FindObjectOfType(typeof(AudioListener)) as AudioListener;
                if (mListener == null)
                {
                    Camera main = Camera.main;
                    if (main == null)
                    {
                        main = Object.FindObjectOfType(typeof(Camera)) as Camera;
                    }
                    if (main != null)
                    {
                        mListener = main.gameObject.AddComponent<AudioListener>();
                    }
                }
            }
            if (mListener != null)
            {
                AudioSource audio = mListener.audio;
                if (audio == null)
                {
                    audio = mListener.gameObject.AddComponent<AudioSource>();
                }
                audio.pitch = pitch;
                audio.PlayOneShot(clip, volume);
                return audio;
            }
        }
        return null;
    }

    public static TComponent QuickGet<TComponent>(GameObject gameObject) where TComponent: Component
    {
        switch (SG<TComponent>.V)
        {
            case SlipGate.Renderer:
                return (gameObject.renderer as TComponent);

            case SlipGate.Collider:
                return (gameObject.collider as TComponent);

            case SlipGate.Transform:
                return (gameObject.transform as TComponent);
        }
        return gameObject.GetComponent<TComponent>();
    }

    public static int RandomRange(int min, int max)
    {
        if (min == max)
        {
            return min;
        }
        return Random.Range(min, max + 1);
    }

    [Obsolete("Use UIAtlas.replacement instead")]
    public static void ReplaceAtlas(UIAtlas before, UIAtlas after)
    {
        UISprite[] spriteArray = FindActive<UISprite>();
        int index = 0;
        int length = spriteArray.Length;
        while (index < length)
        {
            UISprite sprite = spriteArray[index];
            if (sprite.atlas == before)
            {
                sprite.atlas = after;
            }
            index++;
        }
        UILabel[] labelArray = FindActive<UILabel>();
        int num3 = 0;
        int num4 = labelArray.Length;
        while (num3 < num4)
        {
            UILabel label = labelArray[num3];
            if ((label.font != null) && (label.font.atlas == before))
            {
                label.font.atlas = after;
            }
            num3++;
        }
    }

    [Obsolete("Use UIFont.replacement instead")]
    public static void ReplaceFont(UIFont before, UIFont after)
    {
        UILabel[] labelArray = FindActive<UILabel>();
        int index = 0;
        int length = labelArray.Length;
        while (index < length)
        {
            UILabel label = labelArray[index];
            if (label.font == before)
            {
                label.font = after;
            }
            index++;
        }
    }

    public static Vector3 Round(Vector3 v)
    {
        v.x = Mathf.Round(v.x);
        v.y = Mathf.Round(v.y);
        v.z = Mathf.Round(v.z);
        return v;
    }

    public static void SetActive(GameObject go, bool state)
    {
        if (state)
        {
            Activate(go.transform);
        }
        else
        {
            Deactivate(go.transform);
        }
    }

    public static bool SetAllowClick(Component self, bool allow)
    {
        Collider collider = self.collider;
        if (collider != null)
        {
            collider.enabled = allow;
            return true;
        }
        UIHotSpot component = self.GetComponent<UIHotSpot>();
        if (component != null)
        {
            component.enabled = allow;
            return true;
        }
        return false;
    }

    public static void SetAllowClickChildren(GameObject mChild, bool par1)
    {
        Collider[] componentsInChildren = mChild.GetComponentsInChildren<Collider>();
        int index = 0;
        int length = componentsInChildren.Length;
        while (index < length)
        {
            componentsInChildren[index].enabled = false;
            index++;
        }
        UIHotSpot[] spotArray = mChild.GetComponentsInChildren<UIHotSpot>();
        int num3 = 0;
        int num4 = spotArray.Length;
        while (num3 < num4)
        {
            spotArray[num3].enabled = false;
            num3++;
        }
    }

    public static string StripSymbols(string text)
    {
        if (text != null)
        {
            text = text.Replace(@"\n", "\n");
            int symbolSkipCount = 0;
            int index = 0;
            int length = text.Length;
            while (index < length)
            {
                char ch = text[index];
                if (ch == '[')
                {
                    int count = ParseSymbol(text, index, null, ref symbolSkipCount);
                    if (count > 0)
                    {
                        text = text.Remove(index, count);
                        length = text.Length;
                        continue;
                    }
                }
                index++;
            }
        }
        return text;
    }

    public static string UnformattedString(string str)
    {
        int num4;
        int index = str.IndexOf("[\x00bb]");
        int startIndex = str.IndexOf("[\x00ab]");
        if (index != -1)
        {
            if (startIndex != -1)
            {
                List<int> list = new List<int>();
                List<bool> list2 = new List<bool> {
                    index,
                    startIndex,
                    true,
                    false
                };
                while (++index < str.Length)
                {
                    index = str.IndexOf("[\x00ab]", index);
                    if (index == -1)
                    {
                        break;
                    }
                    list.Add(index);
                    list2.Add(true);
                }
                while (++startIndex < str.Length)
                {
                    startIndex = str.IndexOf("[\x00ab]", startIndex);
                    if (startIndex == -1)
                    {
                        break;
                    }
                    list.Add(startIndex);
                    list2.Add(false);
                }
                bool[] items = list2.ToArray();
                Array.Sort<int, bool>(list.ToArray(), items);
                int num5 = 0;
                int num6 = 0;
                for (int i = 0; i < items.Length; i++)
                {
                    if (items[i])
                    {
                        num5++;
                        while (++i < items.Length)
                        {
                            if (items[i])
                            {
                                num5++;
                            }
                            else if (--num5 == 0)
                            {
                                break;
                            }
                        }
                        continue;
                    }
                    num6++;
                    while (++i < items.Length)
                    {
                        if (items[i])
                        {
                            if (--num6 != 0)
                            {
                                continue;
                            }
                            break;
                        }
                        num6++;
                    }
                }
                StringBuilder builder3 = new StringBuilder();
                builder3.Append("[\x00bb]");
                for (int j = 0; j < num6; j++)
                {
                    builder3.Append("[\x00bb]");
                }
                builder3.Append(str);
                for (int k = 0; k < num5; k++)
                {
                    builder3.Append("[\x00ab]");
                }
                builder3.Append("[\x00ab]");
                return builder3.ToString();
            }
            num4 = 1;
            while (++index < str.Length)
            {
                index = str.IndexOf("[\x00ab]", index);
                if (index == -1)
                {
                    break;
                }
                num4++;
            }
        }
        else
        {
            if (startIndex == -1)
            {
                return ("[\x00bb]" + str + "[\x00ab]");
            }
            int num3 = 1;
            while (++startIndex < str.Length)
            {
                startIndex = str.IndexOf("[\x00ab]", startIndex);
                if (startIndex == -1)
                {
                    break;
                }
                num3++;
            }
            if (num3 == 1)
            {
                return ("[\x00bb]" + "[\x00bb]" + str + "[\x00ab]");
            }
            StringBuilder builder = new StringBuilder();
            builder.Append("[\x00bb]");
            while (num3-- > 0)
            {
                builder.Append("[\x00bb]");
            }
            builder.Append(str);
            builder.Append("[\x00ab]");
            return builder.ToString();
        }
        if (num4 == 1)
        {
            return ("[\x00bb]" + str + "[\x00ab]" + "[\x00ab]");
        }
        StringBuilder builder2 = new StringBuilder();
        builder2.Append("[\x00bb]");
        builder2.Append(str);
        while (num4-- > 0)
        {
            builder2.Append("[\x00ab]");
        }
        builder2.Append("[\x00ab]");
        return builder2.ToString();
    }

    public static bool ZeroAlpha(float alpha)
    {
        return ((alpha >= 0f) ? (alpha < 0.001960784f) : (alpha > -0.001960784f));
    }

    public static float soundVolume
    {
        get
        {
            if (!mLoaded)
            {
                mLoaded = true;
                mGlobalVolume = PlayerPrefs.GetFloat("Sound", 1f);
            }
            return mGlobalVolume;
        }
        set
        {
            if (mGlobalVolume != value)
            {
                mLoaded = true;
                mGlobalVolume = value;
                PlayerPrefs.SetFloat("Sound", value);
            }
        }
    }

    private static class SG<T> where T: Component
    {
        public static readonly NGUITools.SlipGate V;

        static SG()
        {
            if (typeof(Renderer).IsAssignableFrom(typeof(T)))
            {
                NGUITools.SG<T>.V = NGUITools.SlipGate.Renderer;
            }
            else if (typeof(Collider).IsAssignableFrom(typeof(T)))
            {
                NGUITools.SG<T>.V = NGUITools.SlipGate.Collider;
            }
            else if (typeof(Behaviour).IsAssignableFrom(typeof(T)))
            {
                NGUITools.SG<T>.V = NGUITools.SlipGate.Behaviour;
            }
            else if (typeof(Transform).IsAssignableFrom(typeof(T)))
            {
                NGUITools.SG<T>.V = NGUITools.SlipGate.Transform;
            }
            else
            {
                NGUITools.SG<T>.V = NGUITools.SlipGate.Component;
            }
        }
    }

    private enum SlipGate
    {
        Renderer,
        Collider,
        Behaviour,
        Transform,
        Component
    }
}

