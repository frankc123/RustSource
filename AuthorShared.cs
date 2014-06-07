using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Threading;
using UnityEngine;

public abstract class AuthorShared : MonoBehaviour
{
    private static readonly GUIContent AuthorPeiceContent = new GUIContent();
    private static readonly GenerateOptions authorPopupGenerate = new GenerateOptions(AuthorShared.AuthorPopupGenerate);
    private static Rect lastRect_popup;

    protected AuthorShared()
    {
    }

    public static TComponent AddComponent<TComponent>(GameObject target, string type) where TComponent: Component
    {
        Component component = target.AddComponent(type);
        if (component != null)
        {
            if (component is TComponent)
            {
                return (TComponent) component;
            }
            Debug.LogWarning("The string type \"" + type + "\" is a component class but does not inherit \"" + typeof(TComponent).AssemblyQualifiedName + "\"", target);
            Object.DestroyImmediate(component);
            return null;
        }
        Debug.LogWarning("The string type \"" + type + "\" evaluated to no component type. null returning", target);
        return null;
    }

    public static bool ArrayField<T>(Content content, ref T[] array, ArrayFieldFunctor<T> functor)
    {
        BeginHorizontal(new GUILayoutOption[0]);
        int num = (array != null) ? array.Length : 0;
        PrefixLabel(content);
        BeginVertical(new GUILayoutOption[0]);
        BeginHorizontal(new GUILayoutOption[0]);
        GUILayoutOption[] options = new GUILayoutOption[] { GUILayout.ExpandWidth(false) };
        Label("Size", options);
        int newSize = Mathf.Max(0, IntField(new Content(), num, new GUILayoutOption[0]));
        EndHorizontal();
        bool flag = num != newSize;
        if (num > 0)
        {
            for (int i = 0; i < num; i++)
            {
                flag |= functor(ref array[i]);
            }
        }
        EndVertical();
        EndHorizontal();
        if (flag)
        {
            Array.Resize<T>(ref array, newSize);
            return true;
        }
        return false;
    }

    public static bool? Ask(string Question)
    {
        return null;
    }

    private static bool AuthorPopupGenerate(object arg, ref int selected, out GUIContent[] options, out Array array)
    {
        AuthorPeice peice;
        options = null;
        array = null;
        AuthorOptionGenerate generate = (AuthorOptionGenerate) arg;
        List<AuthorPeice> list = new List<AuthorPeice>(generate.creation.EnumeratePeices(generate.selectedOnly));
        int count = list.Count;
        if (count == 0)
        {
            return false;
        }
        if (generate.type != null)
        {
            for (int j = 0; j < count; j++)
            {
                if (((peice = list[j]) == null) || !generate.type.IsAssignableFrom(peice.GetType()))
                {
                    list.RemoveAt(j--);
                    count--;
                }
            }
        }
        else
        {
            for (int k = 0; k < count; k++)
            {
                peice = list[k];
                if (peice == null)
                {
                    list.RemoveAt(k--);
                    count--;
                }
            }
        }
        if (count == 0)
        {
            return false;
        }
        if (!generate.allowSelf && (generate.self != null))
        {
            if (generate.peice != null)
            {
                for (int m = 0; m < count; m++)
                {
                    if ((peice = list[m]) == generate.self)
                    {
                        list.RemoveAt(m--);
                        count--;
                    }
                    else if (peice == generate.peice)
                    {
                        selected = m++;
                        while (m < count)
                        {
                            if ((peice = list[m]) == generate.self)
                            {
                                list.RemoveAt(m--);
                                count--;
                            }
                            m++;
                        }
                        break;
                    }
                }
            }
            else
            {
                for (int n = 0; n < count; n++)
                {
                    if ((peice = list[n]) == generate.self)
                    {
                        list.RemoveAt(n--);
                        count--;
                    }
                }
            }
        }
        else if (generate.peice != null)
        {
            for (int num6 = 0; num6 < count; num6++)
            {
                if ((peice = list[num6]) == generate.peice)
                {
                    selected = num6;
                    break;
                }
            }
        }
        if (count == 0)
        {
            return false;
        }
        AuthorPeice[] peiceArray = list.ToArray();
        list = null;
        options = new GUIContent[peiceArray.Length];
        for (int i = 0; i < peiceArray.Length; i++)
        {
            options[i] = new GUIContent(string.Format("{0:00}. {1} ({2})", i, peiceArray[i].peiceID, peiceArray[i].GetType().Name), peiceArray[i].ToString());
        }
        array = peiceArray;
        return true;
    }

    public static Rect BeginHorizontal(params GUILayoutOption[] options)
    {
        return new Rect(0f, 0f, 0f, 0f);
    }

    public static Rect BeginHorizontal(GUIStyle style, params GUILayoutOption[] options)
    {
        return new Rect(0f, 0f, 0f, 0f);
    }

    public static Vector2 BeginScrollView(Vector2 scroll, params GUILayoutOption[] options)
    {
        return scroll;
    }

    public static Rect BeginSubSection(Content title, params GUILayoutOption[] options)
    {
        Color backgroundColor = GUI.backgroundColor;
        GUI.backgroundColor = new Color(backgroundColor.r, backgroundColor.g, backgroundColor.b, backgroundColor.a * 0.4f);
        Rect rect = BeginVertical(Styles.subSection, new GUILayoutOption[0]);
        Label(title, Styles.subSectionTitle, new GUILayoutOption[0]);
        GUI.backgroundColor = backgroundColor;
        return rect;
    }

    public static Rect BeginSubSection(Content title, Content infoContent, params GUILayoutOption[] options)
    {
        return BeginSubSection(title, infoContent, Styles.infoLabel, options);
    }

    public static Rect BeginSubSection(Content title, Content infoContent, GUIStyle infoStyle, params GUILayoutOption[] options)
    {
        Rect rect = BeginSubSection(title, options);
        if ((infoContent.type != 0) && (Event.current.type == EventType.Repaint))
        {
            if (infoContent.type == 1)
            {
                GUI.Label(GUILayoutUtility.GetLastRect(), infoContent.text, infoStyle);
                return rect;
            }
            GUI.Label(GUILayoutUtility.GetLastRect(), infoContent.content, infoStyle);
        }
        return rect;
    }

    public static Rect BeginVertical(params GUILayoutOption[] options)
    {
        return new Rect(0f, 0f, 0f, 0f);
    }

    public static Rect BeginVertical(GUIStyle style, params GUILayoutOption[] options)
    {
        return new Rect(0f, 0f, 0f, 0f);
    }

    public static bool Button(Content content, params GUILayoutOption[] options)
    {
        switch (content.type)
        {
            case 1:
                return GUILayout.Button(content.text, options);

            case 2:
                return GUILayout.Button(content.content, options);
        }
        return GUILayout.Button(GUIContent.none, options);
    }

    public static bool Button(Texture image, params GUILayoutOption[] options)
    {
        return GUILayout.Button(image, options);
    }

    public static bool Button(Content content, GUIStyle style, params GUILayoutOption[] options)
    {
        switch (content.type)
        {
            case 1:
                return GUILayout.Button(content.text, style, options);

            case 2:
                return GUILayout.Button(content.content, style, options);
        }
        return GUILayout.Button(GUIContent.none, style, options);
    }

    public static bool Button(Texture image, GUIStyle style, params GUILayoutOption[] options)
    {
        return GUILayout.Button(image, style, options);
    }

    public static string CalculatePath(Transform targetTransform, Transform root)
    {
        return targetTransform.name;
    }

    public static bool Change(ref bool current, bool incoming)
    {
        if (current == incoming)
        {
            return false;
        }
        current = incoming;
        return true;
    }

    public static bool Change(ref int current, int incoming)
    {
        if (current == incoming)
        {
            return false;
        }
        current = incoming;
        return true;
    }

    public static bool Change<T>(ref T current, object incoming) where T: struct
    {
        if (current.Equals(incoming))
        {
            return false;
        }
        T local = current;
        try
        {
            current = (T) incoming;
            return true;
        }
        catch
        {
            current = local;
            return false;
        }
    }

    public static bool Change(ref float current, float incoming)
    {
        if (current == incoming)
        {
            return false;
        }
        current = incoming;
        return true;
    }

    public static bool Change(ref string current, string incoming)
    {
        if (current == incoming)
        {
            return false;
        }
        current = incoming;
        return true;
    }

    public static bool Change(ref Quaternion current, Quaternion incoming)
    {
        if (current == incoming)
        {
            return false;
        }
        current = incoming;
        return true;
    }

    public static bool Change(ref Vector2 current, Vector2 incoming)
    {
        if (current == incoming)
        {
            return false;
        }
        current = incoming;
        return true;
    }

    public static bool Change(ref Vector3 current, Vector3 incoming)
    {
        if (current == incoming)
        {
            return false;
        }
        current = incoming;
        return true;
    }

    public static bool Change(ref Vector4 current, Vector4 incoming)
    {
        if (current == incoming)
        {
            return false;
        }
        current = incoming;
        return true;
    }

    public static void CustomMenu(Rect position, GUIContent[] options, int selected, CustomMenuProc proc, object userData)
    {
        string[] strArray = new string[options.Length];
        for (int i = 0; i < strArray.Length; i++)
        {
            strArray[i] = options[i].text;
        }
        proc(userData, strArray, selected);
    }

    public static void EndHorizontal()
    {
    }

    public static void EndScrollView()
    {
    }

    public static void EndSubSection()
    {
        EndVertical();
    }

    public static void EndVertical()
    {
    }

    public static bool EnumField<T>(Content content, ref T value, params GUILayoutOption[] options) where T: struct
    {
        return Change<T>(ref value, EnumField(content, (Enum) Enum.ToObject(typeof(T), (T) value), options));
    }

    public static Enum EnumField(Content content, Enum value, params GUILayoutOption[] options)
    {
        return value;
    }

    public static bool EnumField<T>(Content content, ref T value, GUIStyle style, params GUILayoutOption[] options) where T: struct
    {
        return Change<T>(ref value, EnumField(content, (Enum) Enum.ToObject(typeof(T), (T) value), style, options));
    }

    public static Enum EnumField(Content content, Enum value, GUIStyle style, params GUILayoutOption[] options)
    {
        return value;
    }

    public static bool Exists(ObjectKind kind)
    {
        return (kind >= ObjectKind.LevelInstance);
    }

    public static GameObject FindPrefabRoot(GameObject prefab)
    {
        return prefab.transform.root.gameObject;
    }

    public static bool FloatField(Content content, ref float value, params GUILayoutOption[] options)
    {
        return Change(ref value, FloatField(content, (float) value, options));
    }

    public static float FloatField(Content content, float value, params GUILayoutOption[] options)
    {
        return value;
    }

    public static bool FloatField(Content content, ref float value, GUIStyle style, params GUILayoutOption[] options)
    {
        return Change(ref value, FloatField(content, (float) value, style, options));
    }

    public static float FloatField(Content content, float value, GUIStyle style, params GUILayoutOption[] options)
    {
        return value;
    }

    public static Object[] GetAllSelectedObjects()
    {
        return new Object[0];
    }

    public static string GetAssetPath(Object obj)
    {
        return string.Empty;
    }

    private static Rect GetControlRect(bool hasLabel, float height, GUIStyle style, params GUILayoutOption[] options)
    {
        return GUILayoutUtility.GetRect(0f, 100f, height, height, style, options);
    }

    public static ObjectKind GetObjectKind(Object value)
    {
        return ObjectKind.Null;
    }

    public static Transform GetRootBone(Component co)
    {
        if (co is SkinnedMeshRenderer)
        {
            return GetRootBone(co as SkinnedMeshRenderer);
        }
        return GetRootBone(co.gameObject);
    }

    public static Transform GetRootBone(GameObject go)
    {
        SkinnedMeshRenderer renderer;
        return GetRootBone(go, out renderer);
    }

    public static Transform GetRootBone(SkinnedMeshRenderer renderer)
    {
        if (renderer == null)
        {
            throw new ArgumentNullException("renderer");
        }
        return renderer.transform;
    }

    public static Transform GetRootBone(Component co, out SkinnedMeshRenderer renderer)
    {
        if (co is SkinnedMeshRenderer)
        {
            renderer = co as SkinnedMeshRenderer;
            return GetRootBone(renderer);
        }
        return GetRootBone(co.gameObject, out renderer);
    }

    public static Transform GetRootBone(GameObject go, out SkinnedMeshRenderer renderer)
    {
        if (go.renderer is SkinnedMeshRenderer)
        {
            renderer = go.renderer as SkinnedMeshRenderer;
        }
        else
        {
            renderer = null;
            foreach (Transform transform in go.transform.ListDecendantsByDepth())
            {
                if (transform.renderer is SkinnedMeshRenderer)
                {
                    renderer = transform.renderer as SkinnedMeshRenderer;
                    break;
                }
            }
            if (renderer == null)
            {
                return go.transform;
            }
        }
        return GetRootBone(renderer);
    }

    public static string GUIDToPath(string guid)
    {
        return string.Empty;
    }

    public static bool InAnimationMode()
    {
        return false;
    }

    public static GameObject InstantiatePrefab(GameObject prefab)
    {
        return (GameObject) Object.Instantiate(prefab);
    }

    public static T InstantiatePrefab<T>(T prefab) where T: Component
    {
        return (T) Object.Instantiate(prefab);
    }

    public static bool IntField(Content content, ref int value, params GUILayoutOption[] options)
    {
        return Change(ref value, IntField(content, (int) value, options));
    }

    public static int IntField(Content content, int value, params GUILayoutOption[] options)
    {
        return value;
    }

    public static bool IntField(Content content, ref int value, GUIStyle style, params GUILayoutOption[] options)
    {
        return Change(ref value, IntField(content, (int) value, style, options));
    }

    public static int IntField(Content content, int value, GUIStyle style, params GUILayoutOption[] options)
    {
        return value;
    }

    public static bool IsAsset(ObjectKind kind)
    {
        int num = (int) kind;
        return ((num >= 0) && ((num & 1) == 1));
    }

    public static bool IsInstance(ObjectKind kind)
    {
        int num = (int) kind;
        return ((num >= 0) && ((num & 1) == 0));
    }

    public static bool IsLevelInstance(ObjectKind kind)
    {
        return (((((kind == ObjectKind.LevelInstance) || (kind == ObjectKind.MissingPrefabInstance)) || ((kind == ObjectKind.PrefabInstance) || (kind == ObjectKind.ModelInstance))) || (kind == ObjectKind.DisconnectedPrefabInstance)) || (kind == ObjectKind.DisconnectedModelInstance));
    }

    public static bool IsModelAssetOrInstance(ObjectKind kind)
    {
        return (((kind == ObjectKind.Model) || (kind == ObjectKind.ModelInstance)) || (kind == ObjectKind.DisconnectedModelInstance));
    }

    public static bool IsNonModelPrefabAssetOrInstance(ObjectKind kind)
    {
        return (((kind == ObjectKind.Prefab) || (kind == ObjectKind.PrefabInstance)) || (kind == ObjectKind.DisconnectedPrefabInstance));
    }

    public static bool IsPrefabAssetOrInstance(ObjectKind kind)
    {
        return (((((kind == ObjectKind.Prefab) || (kind == ObjectKind.Model)) || ((kind == ObjectKind.PrefabInstance) || (kind == ObjectKind.ModelInstance))) || (kind == ObjectKind.DisconnectedPrefabInstance)) || (kind == ObjectKind.DisconnectedModelInstance));
    }

    public static bool IsScriptableObjectAssetOrInstance(ObjectKind kind)
    {
        return ((kind == ObjectKind.ScriptableObject) || (kind == ObjectKind.ScriptableObjectInstance));
    }

    public static void Label(Content content, params GUILayoutOption[] options)
    {
        switch (content.type)
        {
            case 1:
                GUILayout.Label(content.text, options);
                break;

            case 2:
                GUILayout.Label(content.content, options);
                break;

            default:
                GUILayout.Label(GUIContent.none, options);
                break;
        }
    }

    public static void Label(Texture content, params GUILayoutOption[] options)
    {
        GUILayout.Label(content, options);
    }

    public static void Label(Content content, GUIStyle style, params GUILayoutOption[] options)
    {
        switch (content.type)
        {
            case 1:
                GUILayout.Label(content.text, style, options);
                break;

            case 2:
                GUILayout.Label(content.content, style, options);
                break;

            default:
                GUILayout.Label(GUIContent.none, style, options);
                break;
        }
    }

    public static void Label(Texture content, GUIStyle style, params GUILayoutOption[] options)
    {
        GUILayout.Label(content, style, options);
    }

    public static bool MatchPrefab(Object a, Object b)
    {
        return (((!(a == b) && (a != null)) && (b != null)) && false);
    }

    public static Content ObjectContent<T>() where T: Object
    {
        return ObjectContentR(null, typeof(T));
    }

    public static Content ObjectContent(Type type)
    {
        return ObjectContentR(null, type);
    }

    public static Content ObjectContent<T>(T o) where T: Object
    {
        return ObjectContentR(o, typeof(T));
    }

    public static Content ObjectContent(Object o, Type type)
    {
        return ObjectContentR(o, type);
    }

    public static Content ObjectContent<T>(T o, Type type) where T: Object
    {
        Type expressionStack_18_0;
        object expressionStack_18_1;
        object expressionStack_D_1;
        if (type != null)
        {
            expressionStack_18_1 = o;
            expressionStack_18_0 = type;
            goto Label_0018;
        }
        else
        {
            expressionStack_D_1 = o;
            Type expressionStack_D_0 = type;
        }
        expressionStack_18_1 = expressionStack_D_1;
        expressionStack_18_0 = typeof(T);
    Label_0018:
        return ObjectContentR((Object) expressionStack_18_1, expressionStack_18_0);
    }

    private static Content ObjectContentR(Object o, Type type)
    {
        return GUIContent.none;
    }

    public static Object ObjectField(Object obj, Type type, params GUILayoutOption[] options)
    {
        return ObjectField(new Content(), obj, type, false, options);
    }

    public static bool ObjectField<T>(Content content, ref T reference, ObjectFieldFlags flags, params GUILayoutOption[] options) where T: Object
    {
        return ObjectField<T>(content, ref reference, typeof(T), flags, options);
    }

    public static Object ObjectField(Object obj, Type type, ObjectFieldFlags flags, params GUILayoutOption[] options)
    {
        return ObjectField(new Content(), obj, type, flags, options);
    }

    public static bool ObjectField<T>(Content content, ref T reference, Type type, ObjectFieldFlags flags, params GUILayoutOption[] options) where T: Object
    {
        Object obj2;
        Type expressionStack_1E_0;
        object expressionStack_1E_1;
        Content expressionStack_1E_2;
        object expressionStack_13_1;
        Content expressionStack_13_2;
        if (type != null)
        {
            expressionStack_1E_2 = content;
            expressionStack_1E_1 = (T) reference;
            expressionStack_1E_0 = type;
            goto Label_001E;
        }
        else
        {
            expressionStack_13_2 = content;
            expressionStack_13_1 = (T) reference;
            Type expressionStack_13_0 = type;
        }
        expressionStack_1E_2 = expressionStack_13_2;
        expressionStack_1E_1 = expressionStack_13_1;
        expressionStack_1E_0 = typeof(T);
    Label_001E:
        obj2 = ObjectField(expressionStack_1E_2, (Object) expressionStack_1E_1, expressionStack_1E_0, flags, options);
        if (GUI.changed)
        {
            reference = (T) obj2;
            return true;
        }
        return false;
    }

    public static Object ObjectField(Content label, Object value, Type type, ObjectFieldFlags flags, params GUILayoutOption[] options)
    {
        return ObjectField(label, value, type, (flags & ObjectFieldFlags.AllowScene) == ObjectFieldFlags.AllowScene, options);
    }

    public static Object ObjectField(Content label, Object value, Type type, bool allowScene, params GUILayoutOption[] options)
    {
        return value;
    }

    public static string PathToGUID(string path)
    {
        return string.Empty;
    }

    public static string PathToProjectPath(string path)
    {
        return path;
    }

    public static bool PeiceField<T>(Content content, AuthorCreation self, ref T peice, Type type, GUIStyle style, params GUILayoutOption[] options) where T: AuthorPeice
    {
        return PeiceFieldBase<T>(content, self, ref peice, type, true, style, options);
    }

    public static bool PeiceField<T>(Content content, AuthorPeice self, ref T peice, Type type, GUIStyle style, params GUILayoutOption[] options) where T: AuthorPeice
    {
        return PeiceFieldBase<T>(content, self, ref peice, type, false, style, options);
    }

    public static bool PeiceField<T>(Content content, AuthorPeice self, ref T peice, Type type, bool allowSelf, GUIStyle style, params GUILayoutOption[] options) where T: AuthorPeice
    {
        return PeiceFieldBase<T>(content, self, ref peice, type, allowSelf, style, options);
    }

    private static bool PeiceFieldBase<T>(Content content, AuthorShared self, ref T peice, Type type, bool allowSelf, GUIStyle style, params GUILayoutOption[] options) where T: AuthorPeice
    {
        return false;
    }

    public static void PingObject(int instanceID)
    {
    }

    public static void PingObject(Object o)
    {
    }

    public static bool Popup(ref int index, string[] displayedOptions, params GUILayoutOption[] options)
    {
        return Change(ref index, Popup((int) index, displayedOptions, options));
    }

    public static int Popup(int index, string[] displayedOptions, params GUILayoutOption[] options)
    {
        return index;
    }

    public static bool Popup(ref int index, GUIContent[] displayedOptions, params GUILayoutOption[] options)
    {
        return Change(ref index, Popup((int) index, displayedOptions, options));
    }

    public static int Popup(int index, GUIContent[] displayedOptions, params GUILayoutOption[] options)
    {
        return index;
    }

    public static bool Popup(ref int index, GUIContent[] displayedOptions, GUIStyle style, params GUILayoutOption[] options)
    {
        return Change(ref index, Popup((int) index, displayedOptions, style, options));
    }

    public static bool Popup(Content content, ref int index, string[] displayedOptions, params GUILayoutOption[] options)
    {
        return Change(ref index, Popup(content, (int) index, displayedOptions, options));
    }

    public static int Popup(Content content, int index, string[] displayedOptions, params GUILayoutOption[] options)
    {
        return index;
    }

    public static bool Popup(Content content, ref int index, GUIContent[] displayedOptions, params GUILayoutOption[] options)
    {
        return Change(ref index, Popup(content, (int) index, displayedOptions, options));
    }

    public static bool Popup(ref int index, string[] displayedOptions, GUIStyle style, params GUILayoutOption[] options)
    {
        return Change(ref index, Popup((int) index, displayedOptions, style, options));
    }

    public static int Popup(Content content, int index, GUIContent[] displayedOptions, params GUILayoutOption[] options)
    {
        return index;
    }

    public static int Popup(int index, string[] displayedOptions, GUIStyle style, params GUILayoutOption[] options)
    {
        return index;
    }

    public static int Popup(int index, GUIContent[] displayedOptions, GUIStyle style, params GUILayoutOption[] options)
    {
        return index;
    }

    public static bool Popup(Content content, ref int index, string[] displayedOptions, GUIStyle style, params GUILayoutOption[] options)
    {
        return Change(ref index, Popup(content, (int) index, displayedOptions, style, options));
    }

    public static int Popup(Content content, int index, string[] displayedOptions, GUIStyle style, params GUILayoutOption[] options)
    {
        return index;
    }

    public static bool Popup(Content content, ref int index, GUIContent[] displayedOptions, GUIStyle style, params GUILayoutOption[] options)
    {
        return Change(ref index, Popup(content, (int) index, displayedOptions, style, options));
    }

    public static int Popup(Content content, int index, GUIContent[] displayedOptions, GUIStyle style, params GUILayoutOption[] options)
    {
        return index;
    }

    private static bool PopupImmediate<T>(Content content, GenerateOptions generateOptions, T args, GUIStyle style, GUILayoutOption[] options, out object value)
    {
        value = null;
        return false;
    }

    public static void PrefixLabel(Content content)
    {
    }

    public static void PrefixLabel(Content content, GUIStyle followingStyle)
    {
    }

    public static void PrefixLabel(Content content, GUIStyle followingStyle, GUIStyle labelStyle)
    {
    }

    public static bool SelectionContains(int obj)
    {
        return false;
    }

    public static bool SelectionContains(Object obj)
    {
        return false;
    }

    public static void SetActiveSelection(Object o)
    {
    }

    public static void SetAllSelectedObjects(params Object[] objects)
    {
    }

    public static void SetDirty(Object obj)
    {
    }

    public static void SetSerializedProperty(Object objSet, string propertyPath, Object value)
    {
    }

    public static void StartAnimationMode(params Object[] objects)
    {
    }

    public static void StopAnimationMode()
    {
    }

    public static bool StringField(Content content, ref string value, params GUILayoutOption[] options)
    {
        return Change(ref value, StringField(content, (string) value, options));
    }

    public static string StringField(Content content, string value, params GUILayoutOption[] options)
    {
        return value;
    }

    public static bool StringField(Content content, ref string value, GUIStyle style, params GUILayoutOption[] options)
    {
        return Change(ref value, StringField(content, (string) value, style, options));
    }

    public static string StringField(Content content, string value, GUIStyle style, params GUILayoutOption[] options)
    {
        return value;
    }

    public static bool Toggle(Content content, bool state, params GUILayoutOption[] options)
    {
        switch (content.type)
        {
            case 1:
                return GUILayout.Toggle(state, content.text, options);

            case 2:
                return GUILayout.Toggle(state, content.content, options);
        }
        return GUILayout.Toggle(state, GUIContent.none, options);
    }

    public static bool Toggle(Content content, ref bool state, params GUILayoutOption[] options)
    {
        return Change(ref state, Toggle(content, (bool) state, options));
    }

    public static bool Toggle(Texture image, bool state, params GUILayoutOption[] options)
    {
        return GUILayout.Toggle(state, image, options);
    }

    public static bool Toggle(Content content, bool state, GUIStyle style, params GUILayoutOption[] options)
    {
        switch (content.type)
        {
            case 1:
                return GUILayout.Toggle(state, content.text, style, options);

            case 2:
                return GUILayout.Toggle(state, content.content, style, options);
        }
        return GUILayout.Toggle(state, GUIContent.none, style, options);
    }

    public static bool Toggle(Content content, ref bool state, GUIStyle style, params GUILayoutOption[] options)
    {
        return Change(ref state, Toggle(content, (bool) state, style, options));
    }

    public static bool Toggle(Texture image, bool state, GUIStyle style, params GUILayoutOption[] options)
    {
        return GUILayout.Toggle(state, image, style, options);
    }

    public static string TryPathToProjectPath(string path)
    {
        return path;
    }

    public static bool Vector3Field(Content content, ref Vector3 value, params GUILayoutOption[] options)
    {
        return Change(ref value, Vector3Field(content, (Vector3) value, options));
    }

    public static Vector3 Vector3Field(Content content, Vector3 value, params GUILayoutOption[] options)
    {
        return value;
    }

    private static bool VerifyArgs(GenerateOptions generateOptions, GUIContent[] options, Array array)
    {
        return ((((options != null) && (array != null)) && (options.Length == array.Length)) && (options.Length != 0));
    }

    public delegate bool ArrayFieldFunctor<T>(ref T value);

    protected class AttributeKeyValueList
    {
        private Dictionary<AuthTarg, ArrayList> dict;

        public AttributeKeyValueList(params object[] keysThenValues) : this((IEnumerable) keysThenValues)
        {
        }

        public AttributeKeyValueList(IEnumerable keysThenValues)
        {
            this.dict = new Dictionary<AuthTarg, ArrayList>();
            AuthTarg? nullable = null;
            using (IEnumerator enumerator = null)
            {
                enumerator = keysThenValues.GetEnumerator();
                if (enumerator != null)
                {
                    while (enumerator.MoveNext())
                    {
                        ArrayList list;
                        object current = enumerator.Current;
                        if (current is AuthTarg)
                        {
                            nullable = new AuthTarg?((AuthTarg) ((int) current));
                        }
                        else if (!nullable.HasValue || object.ReferenceEquals(current, null))
                        {
                            continue;
                        }
                        if (!this.dict.TryGetValue(nullable.Value, out list))
                        {
                            this.dict[nullable.Value] = list = new ArrayList();
                        }
                        list.Add(current);
                    }
                }
            }
        }

        [DebuggerHidden]
        private static IEnumerable<Component> GetComponentDown(GameObject go, Type type)
        {
            return new <GetComponentDown>c__Iterator1 { go = go, type = type, <$>go = go, <$>type = type, $PC = -2 };
        }

        [DebuggerHidden]
        private static IEnumerable<Component> GetComponentDown(GameObject go, Type type, Transform childSkip)
        {
            return new <GetComponentDown>c__Iterator0 { go = go, type = type, childSkip = childSkip, <$>go = go, <$>type = type, <$>childSkip = childSkip, $PC = -2 };
        }

        [DebuggerHidden]
        private static IEnumerable<Component> GetComponentUp(GameObject go, Type type, bool andThenDown)
        {
            return new <GetComponentUp>c__Iterator2 { go = go, type = type, andThenDown = andThenDown, <$>go = go, <$>type = type, <$>andThenDown = andThenDown, $PC = -2 };
        }

        public void Run(GameObject go)
        {
            if ((this.dict.Count > 0) && (go != null))
            {
                foreach (MonoBehaviour behaviour in go.GetComponentsInChildren<MonoBehaviour>(true))
                {
                    TypeRunner.Exec(behaviour, this);
                }
            }
        }

        public void Run(MonoBehaviour script)
        {
            if (this.dict.Count > 0)
            {
                TypeRunner.Exec(script, this);
            }
        }

        private static void RunInstance(MonoBehaviour instance, AuthField attribute, ArrayList args)
        {
            object obj2 = attribute.field.GetValue(instance);
            if (!(!(obj2 is Object) ? (obj2 != null) : ((bool) ((Object) obj2))))
            {
                Type fieldType = attribute.field.FieldType;
                bool isComponent = typeof(Component).IsAssignableFrom(fieldType);
                bool flag2 = !isComponent && typeof(GameObject).IsAssignableFrom(fieldType);
                if ((isComponent != flag2) && Search(instance, attribute, args, isComponent, ref obj2))
                {
                    attribute.field.SetValue(instance, obj2);
                }
            }
        }

        private static bool Search(MonoBehaviour instance, AuthField attribute, ArrayList args, bool isComponent, ref object value)
        {
            AuthOptions options = attribute.options & (AuthOptions.SearchUp | AuthOptions.SearchDown);
            bool flag = options != 0;
            if ((!flag || ((attribute.options & AuthOptions.SearchInclusive) == AuthOptions.SearchInclusive)) && SearchGameObject(instance.gameObject, attribute, args, isComponent, ref value))
            {
                return true;
            }
            if (flag)
            {
                if ((options & AuthOptions.SearchDown) == AuthOptions.SearchDown)
                {
                    if ((attribute.options & (AuthOptions.SearchReverse | AuthOptions.SearchUp)) == (AuthOptions.SearchReverse | AuthOptions.SearchUp))
                    {
                        if (SearchGameObjectUp(instance.gameObject, attribute, args, isComponent, ref value))
                        {
                            return true;
                        }
                        options &= ~AuthOptions.SearchUp;
                    }
                    if (SearchGameObjectDown(instance.gameObject, attribute, args, isComponent, ref value))
                    {
                        return true;
                    }
                }
                if (((options & AuthOptions.SearchUp) == AuthOptions.SearchUp) && SearchGameObjectUp(instance.gameObject, attribute, args, isComponent, ref value))
                {
                    return true;
                }
            }
            return false;
        }

        private static bool SearchGameObject(GameObject self, AuthField attribute, ArrayList options, bool isComponent, ref object value)
        {
            IEnumerator enumerator = options.GetEnumerator();
            try
            {
                while (enumerator.MoveNext())
                {
                    object current = enumerator.Current;
                    if (current is Object)
                    {
                        Object obj3 = (Object) current;
                        if ((obj3 != null) && (((attribute.options & 4) == 0) || (obj3.name == attribute.nameMask)))
                        {
                            if (isComponent)
                            {
                                Component component;
                                if (obj3 is GameObject)
                                {
                                    component = ((GameObject) obj3).GetComponent(attribute.field.FieldType);
                                }
                                else if (attribute.field.FieldType.IsAssignableFrom(obj3.GetType()))
                                {
                                    component = (Component) obj3;
                                }
                                else
                                {
                                    if (!(obj3 is Component))
                                    {
                                        continue;
                                    }
                                    component = ((Component) obj3).GetComponent(attribute.field.FieldType);
                                }
                                if (component != null)
                                {
                                    value = component;
                                    return true;
                                }
                            }
                            else if (obj3 is GameObject)
                            {
                                value = (GameObject) obj3;
                                return true;
                            }
                        }
                    }
                }
            }
            finally
            {
                IDisposable disposable = enumerator as IDisposable;
                if (disposable == null)
                {
                }
                disposable.Dispose();
            }
            return false;
        }

        private static bool SearchGameObjectDown(GameObject self, AuthField attribute, ArrayList options, bool isComponent, ref object value)
        {
            Type type = !isComponent ? typeof(Transform) : attribute.field.FieldType;
            IEnumerator enumerator = options.GetEnumerator();
            try
            {
                while (enumerator.MoveNext())
                {
                    object current = enumerator.Current;
                    if (current is Object)
                    {
                        Object obj3 = (Object) current;
                        if (obj3 != null)
                        {
                            GameObject gameObject;
                            if (obj3 is GameObject)
                            {
                                gameObject = (GameObject) obj3;
                            }
                            else
                            {
                                if (!(obj3 is Component))
                                {
                                    continue;
                                }
                                gameObject = ((Component) obj3).gameObject;
                            }
                            IEnumerator<Component> enumerator2 = GetComponentDown(gameObject, type).GetEnumerator();
                            try
                            {
                                while (enumerator2.MoveNext())
                                {
                                    Component component = enumerator2.Current;
                                    if (((attribute.options & 4) == 0) || (component.name == attribute.nameMask))
                                    {
                                        if (isComponent)
                                        {
                                            value = component;
                                            return true;
                                        }
                                        GameObject obj5 = component.gameObject;
                                        if (obj5 != null)
                                        {
                                            value = obj5;
                                            return true;
                                        }
                                    }
                                }
                                continue;
                            }
                            finally
                            {
                                if (enumerator2 == null)
                                {
                                }
                                enumerator2.Dispose();
                            }
                        }
                    }
                }
            }
            finally
            {
                IDisposable disposable = enumerator as IDisposable;
                if (disposable == null)
                {
                }
                disposable.Dispose();
            }
            return false;
        }

        private static bool SearchGameObjectUp(GameObject self, AuthField attribute, ArrayList options, bool isComponent, ref object value)
        {
            Type type = !isComponent ? typeof(Transform) : attribute.field.FieldType;
            IEnumerator enumerator = options.GetEnumerator();
            try
            {
                while (enumerator.MoveNext())
                {
                    object current = enumerator.Current;
                    if (current is Object)
                    {
                        Object obj3 = (Object) current;
                        if (obj3 != null)
                        {
                            GameObject gameObject;
                            if (obj3 is GameObject)
                            {
                                gameObject = (GameObject) obj3;
                            }
                            else
                            {
                                if (!(obj3 is Component))
                                {
                                    continue;
                                }
                                gameObject = ((Component) obj3).gameObject;
                            }
                            IEnumerator<Component> enumerator2 = GetComponentUp(gameObject, type, false).GetEnumerator();
                            try
                            {
                                while (enumerator2.MoveNext())
                                {
                                    Component component = enumerator2.Current;
                                    if (((attribute.options & 4) == 0) || (component.name == attribute.nameMask))
                                    {
                                        if (isComponent)
                                        {
                                            value = component;
                                            return true;
                                        }
                                        GameObject obj5 = component.gameObject;
                                        if (obj5 != null)
                                        {
                                            value = obj5;
                                            return true;
                                        }
                                    }
                                }
                                continue;
                            }
                            finally
                            {
                                if (enumerator2 == null)
                                {
                                }
                                enumerator2.Dispose();
                            }
                        }
                    }
                }
            }
            finally
            {
                IDisposable disposable = enumerator as IDisposable;
                if (disposable == null)
                {
                }
                disposable.Dispose();
            }
            return false;
        }

        [CompilerGenerated]
        private sealed class <GetComponentDown>c__Iterator0 : IEnumerable<Component>, IEnumerator<Component>, IDisposable, IEnumerator, IEnumerable
        {
            internal Component $current;
            internal int $PC;
            internal Transform <$>childSkip;
            internal GameObject <$>go;
            internal Type <$>type;
            internal IEnumerator <$s_3>__0;
            internal Component[] <$s_4>__2;
            internal int <$s_5>__3;
            internal object <child>__1;
            internal Component <component>__4;
            internal Transform childSkip;
            internal GameObject go;
            internal Type type;

            [DebuggerHidden]
            public void Dispose()
            {
                uint num = (uint) this.$PC;
                this.$PC = -1;
                switch (num)
                {
                    case 1:
                        try
                        {
                        }
                        finally
                        {
                            IDisposable disposable = this.<$s_3>__0 as IDisposable;
                            if (disposable == null)
                            {
                            }
                            disposable.Dispose();
                        }
                        break;
                }
            }

            public bool MoveNext()
            {
                uint num = (uint) this.$PC;
                this.$PC = -1;
                bool flag = false;
                switch (num)
                {
                    case 0:
                        if ((this.go == null) || !typeof(Component).IsAssignableFrom(this.type))
                        {
                            goto Label_0164;
                        }
                        this.<$s_3>__0 = this.go.transform.GetEnumerator();
                        num = 0xfffffffd;
                        break;

                    case 1:
                        break;

                    default:
                        goto Label_016B;
                }
                try
                {
                    switch (num)
                    {
                        case 1:
                            goto Label_0113;
                    }
                    while (this.<$s_3>__0.MoveNext())
                    {
                        this.<child>__1 = this.<$s_3>__0.Current;
                        if ((this.<child>__1 is Transform) && (((Transform) this.<child>__1) != this.childSkip))
                        {
                            this.<$s_4>__2 = ((Transform) this.<child>__1).gameObject.GetComponentsInChildren(this.type, true);
                            this.<$s_5>__3 = 0;
                            while (this.<$s_5>__3 < this.<$s_4>__2.Length)
                            {
                                this.<component>__4 = this.<$s_4>__2[this.<$s_5>__3];
                                this.$current = this.<component>__4;
                                this.$PC = 1;
                                flag = true;
                                return true;
                            Label_0113:
                                this.<$s_5>__3++;
                            }
                        }
                    }
                }
                finally
                {
                    if (!flag)
                    {
                    }
                    IDisposable disposable = this.<$s_3>__0 as IDisposable;
                    if (disposable == null)
                    {
                    }
                    disposable.Dispose();
                }
            Label_0164:
                this.$PC = -1;
            Label_016B:
                return false;
            }

            [DebuggerHidden]
            public void Reset()
            {
                throw new NotSupportedException();
            }

            [DebuggerHidden]
            IEnumerator<Component> IEnumerable<Component>.GetEnumerator()
            {
                if (Interlocked.CompareExchange(ref this.$PC, 0, -2) == -2)
                {
                    return this;
                }
                return new AuthorShared.AttributeKeyValueList.<GetComponentDown>c__Iterator0 { go = this.<$>go, type = this.<$>type, childSkip = this.<$>childSkip };
            }

            [DebuggerHidden]
            IEnumerator IEnumerable.GetEnumerator()
            {
                return this.System.Collections.Generic.IEnumerable<UnityEngine.Component>.GetEnumerator();
            }

            Component IEnumerator<Component>.Current
            {
                [DebuggerHidden]
                get
                {
                    return this.$current;
                }
            }

            object IEnumerator.Current
            {
                [DebuggerHidden]
                get
                {
                    return this.$current;
                }
            }
        }

        [CompilerGenerated]
        private sealed class <GetComponentDown>c__Iterator1 : IEnumerable<Component>, IEnumerator<Component>, IDisposable, IEnumerator, IEnumerable
        {
            internal Component $current;
            internal int $PC;
            internal GameObject <$>go;
            internal Type <$>type;
            internal IEnumerator <$s_6>__0;
            internal Component[] <$s_7>__2;
            internal int <$s_8>__3;
            internal object <child>__1;
            internal Component <component>__4;
            internal GameObject go;
            internal Type type;

            [DebuggerHidden]
            public void Dispose()
            {
                uint num = (uint) this.$PC;
                this.$PC = -1;
                switch (num)
                {
                    case 1:
                        try
                        {
                        }
                        finally
                        {
                            IDisposable disposable = this.<$s_6>__0 as IDisposable;
                            if (disposable == null)
                            {
                            }
                            disposable.Dispose();
                        }
                        break;
                }
            }

            public bool MoveNext()
            {
                uint num = (uint) this.$PC;
                this.$PC = -1;
                bool flag = false;
                switch (num)
                {
                    case 0:
                        if ((this.go == null) || !typeof(Component).IsAssignableFrom(this.type))
                        {
                            goto Label_0149;
                        }
                        this.<$s_6>__0 = this.go.transform.GetEnumerator();
                        num = 0xfffffffd;
                        break;

                    case 1:
                        break;

                    default:
                        goto Label_0150;
                }
                try
                {
                    switch (num)
                    {
                        case 1:
                            goto Label_00F8;
                    }
                    while (this.<$s_6>__0.MoveNext())
                    {
                        this.<child>__1 = this.<$s_6>__0.Current;
                        if (this.<child>__1 is Transform)
                        {
                            this.<$s_7>__2 = ((Transform) this.<child>__1).gameObject.GetComponentsInChildren(this.type, true);
                            this.<$s_8>__3 = 0;
                            while (this.<$s_8>__3 < this.<$s_7>__2.Length)
                            {
                                this.<component>__4 = this.<$s_7>__2[this.<$s_8>__3];
                                this.$current = this.<component>__4;
                                this.$PC = 1;
                                flag = true;
                                return true;
                            Label_00F8:
                                this.<$s_8>__3++;
                            }
                        }
                    }
                }
                finally
                {
                    if (!flag)
                    {
                    }
                    IDisposable disposable = this.<$s_6>__0 as IDisposable;
                    if (disposable == null)
                    {
                    }
                    disposable.Dispose();
                }
            Label_0149:
                this.$PC = -1;
            Label_0150:
                return false;
            }

            [DebuggerHidden]
            public void Reset()
            {
                throw new NotSupportedException();
            }

            [DebuggerHidden]
            IEnumerator<Component> IEnumerable<Component>.GetEnumerator()
            {
                if (Interlocked.CompareExchange(ref this.$PC, 0, -2) == -2)
                {
                    return this;
                }
                return new AuthorShared.AttributeKeyValueList.<GetComponentDown>c__Iterator1 { go = this.<$>go, type = this.<$>type };
            }

            [DebuggerHidden]
            IEnumerator IEnumerable.GetEnumerator()
            {
                return this.System.Collections.Generic.IEnumerable<UnityEngine.Component>.GetEnumerator();
            }

            Component IEnumerator<Component>.Current
            {
                [DebuggerHidden]
                get
                {
                    return this.$current;
                }
            }

            object IEnumerator.Current
            {
                [DebuggerHidden]
                get
                {
                    return this.$current;
                }
            }
        }

        [CompilerGenerated]
        private sealed class <GetComponentUp>c__Iterator2 : IEnumerable<Component>, IEnumerator<Component>, IDisposable, IEnumerator, IEnumerable
        {
            internal Component $current;
            internal int $PC;
            internal bool <$>andThenDown;
            internal GameObject <$>go;
            internal Type <$>type;
            internal int <$s_10>__3;
            internal IEnumerator<Component> <$s_11>__7;
            internal Component[] <$s_9>__2;
            internal Component <child>__8;
            internal Component <component>__4;
            internal int <i>__6;
            internal Transform <parent>__1;
            internal Transform <skip>__5;
            internal int <upCount>__0;
            internal bool andThenDown;
            internal GameObject go;
            internal Type type;

            [DebuggerHidden]
            public void Dispose()
            {
                uint num = (uint) this.$PC;
                this.$PC = -1;
                switch (num)
                {
                    case 2:
                        try
                        {
                        }
                        finally
                        {
                            if (this.<$s_11>__7 == null)
                            {
                            }
                            this.<$s_11>__7.Dispose();
                        }
                        break;
                }
            }

            public bool MoveNext()
            {
                uint num = (uint) this.$PC;
                this.$PC = -1;
                bool flag = false;
                switch (num)
                {
                    case 0:
                        if ((this.go != null) && typeof(Component).IsAssignableFrom(this.type))
                        {
                            this.<upCount>__0 = 0;
                            this.<parent>__1 = this.go.transform.parent;
                            if (this.<parent>__1 == null)
                            {
                                break;
                            }
                            do
                            {
                                this.<upCount>__0++;
                                this.<$s_9>__2 = this.<parent>__1.GetComponents(this.type);
                                this.<$s_10>__3 = 0;
                                while (this.<$s_10>__3 < this.<$s_9>__2.Length)
                                {
                                    this.<component>__4 = this.<$s_9>__2[this.<$s_10>__3];
                                    this.$current = this.<component>__4;
                                    this.$PC = 1;
                                    goto Label_025C;
                                Label_00DA:
                                    this.<$s_10>__3++;
                                }
                                this.<parent>__1 = this.<parent>__1.parent;
                            }
                            while (this.<parent>__1 != null);
                            if (!this.andThenDown)
                            {
                                goto Label_025A;
                            }
                            while (this.<upCount>__0 > 0)
                            {
                                this.<parent>__1 = this.go.transform.parent;
                                this.<skip>__5 = this.go.transform;
                                this.<upCount>__0--;
                                this.<i>__6 = 0;
                                while (this.<i>__6 < this.<upCount>__0)
                                {
                                    this.<parent>__1 = this.<parent>__1.parent;
                                    this.<skip>__5 = this.<skip>__5.parent;
                                    this.<i>__6++;
                                }
                                this.<$s_11>__7 = AuthorShared.AttributeKeyValueList.GetComponentDown(this.<parent>__1.gameObject, this.type, this.<skip>__5).GetEnumerator();
                                num = 0xfffffffd;
                            Label_01DD:
                                try
                                {
                                    while (this.<$s_11>__7.MoveNext())
                                    {
                                        this.<child>__8 = this.<$s_11>__7.Current;
                                        this.$current = this.<child>__8;
                                        this.$PC = 2;
                                        flag = true;
                                        goto Label_025C;
                                    }
                                    continue;
                                }
                                finally
                                {
                                    if (!flag)
                                    {
                                    }
                                    if (this.<$s_11>__7 == null)
                                    {
                                    }
                                    this.<$s_11>__7.Dispose();
                                }
                            }
                        }
                        break;

                    case 1:
                        goto Label_00DA;

                    case 2:
                        goto Label_01DD;

                    default:
                        goto Label_025A;
                }
                this.$PC = -1;
            Label_025A:
                return false;
            Label_025C:
                return true;
            }

            [DebuggerHidden]
            public void Reset()
            {
                throw new NotSupportedException();
            }

            [DebuggerHidden]
            IEnumerator<Component> IEnumerable<Component>.GetEnumerator()
            {
                if (Interlocked.CompareExchange(ref this.$PC, 0, -2) == -2)
                {
                    return this;
                }
                return new AuthorShared.AttributeKeyValueList.<GetComponentUp>c__Iterator2 { go = this.<$>go, type = this.<$>type, andThenDown = this.<$>andThenDown };
            }

            [DebuggerHidden]
            IEnumerator IEnumerable.GetEnumerator()
            {
                return this.System.Collections.Generic.IEnumerable<UnityEngine.Component>.GetEnumerator();
            }

            Component IEnumerator<Component>.Current
            {
                [DebuggerHidden]
                get
                {
                    return this.$current;
                }
            }

            object IEnumerator.Current
            {
                [DebuggerHidden]
                get
                {
                    return this.$current;
                }
            }
        }

        private class AuthField
        {
            public FieldInfo field;
            public string nameMask;
            public AuthOptions options;
        }

        private static class TypeRunner
        {
            private static readonly Dictionary<Type, AuthorShared.AttributeKeyValueList.TypeRunnerPlatform> platforms = new Dictionary<Type, AuthorShared.AttributeKeyValueList.TypeRunnerPlatform>();

            public static void Exec(MonoBehaviour monoBehaviour, AuthorShared.AttributeKeyValueList kv)
            {
                if (monoBehaviour != null)
                {
                    Type key = monoBehaviour.GetType();
                    if (key != typeof(MonoBehaviour))
                    {
                        AuthorShared.AttributeKeyValueList.TypeRunnerPlatform platform;
                        if (!platforms.TryGetValue(key, out platform))
                        {
                            GeneratePlatform(key, out platform);
                        }
                        platform.Exec(monoBehaviour, kv);
                    }
                }
            }

            private static void GeneratePlatform(Type type, out AuthorShared.AttributeKeyValueList.TypeRunnerPlatform platform)
            {
                if (type.BaseType == typeof(MonoBehaviour))
                {
                    platform = null;
                }
                else if (!platforms.TryGetValue(type.BaseType, out platform))
                {
                    GeneratePlatform(type.BaseType, out platform);
                }
                Type[] typeArguments = new Type[] { type };
                AuthorShared.AttributeKeyValueList.TypeRunnerExec a = (AuthorShared.AttributeKeyValueList.TypeRunnerExec) typeof(AuthorShared.AttributeKeyValueList.TypeRunner<>).MakeGenericType(typeArguments).GetField("exec", BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Static).GetValue(null);
                if (a != null)
                {
                    if ((platform != null) && (platform.exec != null))
                    {
                        a = (AuthorShared.AttributeKeyValueList.TypeRunnerExec) Delegate.Combine(a, platform.exec);
                    }
                }
                else if (platform != null)
                {
                    a = platform.exec;
                }
                AuthorShared.AttributeKeyValueList.TypeRunnerPlatform platform2 = new AuthorShared.AttributeKeyValueList.TypeRunnerPlatform {
                    @base = platform,
                    exec = a,
                    hasBase = platform != null,
                    hasDelegate = a != null,
                    tested = true
                };
                platforms[type] = platform = platform2;
            }

            public static bool TestAttribute<T>(FieldInfo field, out T[] attribs) where T: Attribute
            {
                if (Attribute.IsDefined(field, typeof(T)))
                {
                    Attribute[] attributeArray = Attribute.GetCustomAttributes(field, typeof(T), false);
                    if (attributeArray.Length > 0)
                    {
                        attribs = new T[attributeArray.Length];
                        for (int i = 0; i < attributeArray.Length; i++)
                        {
                            attribs[i] = (T) attributeArray[i];
                        }
                        return true;
                    }
                }
                attribs = null;
                return false;
            }
        }

        private static class TypeRunner<T> where T: MonoBehaviour
        {
            private static readonly AuthorShared.AttributeKeyValueList.TypeRunnerExec exec;
            private static readonly int fieldCount;
            private static readonly KeyValuePair<AuthTarg, AuthorShared.AttributeKeyValueList.AuthField>[] fields;

            static TypeRunner()
            {
                FieldInfo[] fields = typeof(T).GetFields(BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly);
                int length = fields.Length;
                for (int i = 0; i < length; i++)
                {
                    PostAuthAttribute[] attributeArray;
                    bool flag;
                    if (!AuthorShared.AttributeKeyValueList.TypeRunner.TestAttribute<PostAuthAttribute>(fields[i], out attributeArray))
                    {
                        continue;
                    }
                    List<KeyValuePair<AuthTarg, AuthorShared.AttributeKeyValueList.AuthField>> list = new List<KeyValuePair<AuthTarg, AuthorShared.AttributeKeyValueList.AuthField>>();
                    do
                    {
                        flag = false;
                        int index = 0;
                        int num4 = attributeArray.Length;
                        do
                        {
                            AuthorShared.AttributeKeyValueList.AuthField field = new AuthorShared.AttributeKeyValueList.AuthField {
                                field = fields[i],
                                options = attributeArray[index].options,
                                nameMask = attributeArray[index].nameMask
                            };
                            list.Add(new KeyValuePair<AuthTarg, AuthorShared.AttributeKeyValueList.AuthField>(attributeArray[index].target, field));
                        }
                        while (++index < num4);
                        while (++i < length)
                        {
                            if (flag = AuthorShared.AttributeKeyValueList.TypeRunner.TestAttribute<PostAuthAttribute>(fields[i], out attributeArray))
                            {
                                break;
                            }
                        }
                    }
                    while (flag);
                    AuthorShared.AttributeKeyValueList.TypeRunner<T>.fields = list.ToArray();
                    AuthorShared.AttributeKeyValueList.TypeRunner<T>.fieldCount = AuthorShared.AttributeKeyValueList.TypeRunner<T>.fields.Length;
                    AuthorShared.AttributeKeyValueList.TypeRunner<T>.exec = new AuthorShared.AttributeKeyValueList.TypeRunnerExec(AuthorShared.AttributeKeyValueList.TypeRunner<T>.Exec);
                    return;
                }
                AuthorShared.AttributeKeyValueList.TypeRunner<T>.exec = null;
                AuthorShared.AttributeKeyValueList.TypeRunner<T>.fieldCount = 0;
                AuthorShared.AttributeKeyValueList.TypeRunner<T>.fields = null;
            }

            private static void Exec(object instance, AuthorShared.AttributeKeyValueList list)
            {
                MonoBehaviour behaviour = (MonoBehaviour) instance;
                for (int i = 0; i < AuthorShared.AttributeKeyValueList.TypeRunner<T>.fieldCount; i++)
                {
                    ArrayList list2;
                    if (list.dict.TryGetValue(AuthorShared.AttributeKeyValueList.TypeRunner<T>.fields[i].Key, out list2))
                    {
                        AuthorShared.AttributeKeyValueList.RunInstance(behaviour, AuthorShared.AttributeKeyValueList.TypeRunner<T>.fields[i].Value, list2);
                    }
                }
            }
        }

        private delegate void TypeRunnerExec(object instance, AuthorShared.AttributeKeyValueList kv);

        private class TypeRunnerPlatform
        {
            public AuthorShared.AttributeKeyValueList.TypeRunnerPlatform @base;
            public AuthorShared.AttributeKeyValueList.TypeRunnerExec exec;
            public bool hasBase;
            public bool hasDelegate;
            public bool tested;

            public void Exec(object instance, AuthorShared.AttributeKeyValueList kv)
            {
                if (this.hasBase)
                {
                    this.@base.Exec(instance, kv);
                }
                if (this.hasDelegate)
                {
                    this.exec(instance, kv);
                }
            }
        }
    }

    [StructLayout(LayoutKind.Sequential)]
    private struct AuthorOptionGenerate
    {
        public AuthorCreation creation;
        public AuthorShared self;
        public AuthorPeice peice;
        public Type type;
        public bool allowSelf;
        public bool selectedOnly;
    }

    [StructLayout(LayoutKind.Sequential)]
    public struct Content
    {
        public readonly int type;
        public readonly string text;
        public readonly GUIContent content;
        private Content(GUIContent content)
        {
            this.content = content;
            if (content == null)
            {
            }
            this.text = GUIContent.none.text;
            this.type = 2;
        }

        private Content(string text)
        {
            this.content = null;
            this.text = text;
            this.type = (text != null) ? 1 : 0;
        }

        public Texture image
        {
            get
            {
                return ((this.type != 2) ? GUIContent.none.image : this.content.image);
            }
        }
        public string tooltip
        {
            get
            {
                return ((this.type != 2) ? GUIContent.none.tooltip : this.content.tooltip);
            }
        }
        public static implicit operator AuthorShared.Content(GUIContent content)
        {
            return new AuthorShared.Content(content);
        }

        public static implicit operator AuthorShared.Content(string content)
        {
            return new AuthorShared.Content(content);
        }

        public static implicit operator AuthorShared.Content(bool show)
        {
            if (show)
            {
                return new AuthorShared.Content(GUIContent.none);
            }
            return new AuthorShared.Content();
        }

        public static bool operator true(AuthorShared.Content content)
        {
            return (content.type != 0);
        }

        public static bool operator false(AuthorShared.Content content)
        {
            return (content.type == 0);
        }

        public static implicit operator GUIContent(AuthorShared.Content content)
        {
            return g.GetOrTemp(content);
        }

        public static explicit operator string(AuthorShared.Content content)
        {
            return content.text;
        }
        private static class g
        {
            public static readonly GUIContent[] bufContents = new GUIContent[] { new GUIContent(), new GUIContent(), new GUIContent(), new GUIContent(), new GUIContent(), new GUIContent(), new GUIContent(), new GUIContent(), new GUIContent(), new GUIContent(), new GUIContent(), new GUIContent(), new GUIContent(), new GUIContent(), new GUIContent(), new GUIContent() };
            private static int bufPos = 0;
            public static readonly GUIContent noneCopy = new GUIContent();

            public static GUIContent GetOrTemp(AuthorShared.Content content)
            {
                if (content.type == 2)
                {
                    return content.content;
                }
                if (content.type != 1)
                {
                    return noneCopy;
                }
                GUIContent content2 = bufContents[bufPos];
                if (++bufPos == bufContents.Length)
                {
                    bufPos = 0;
                }
                content2.text = content.text;
                content2.tooltip = noneCopy.tooltip;
                content2.image = noneCopy.image;
                return content2;
            }
        }
    }

    public delegate void CustomMenuProc(object userData, string[] options, int selected);

    private delegate bool GenerateOptions(object args, ref int selected, out GUIContent[] options, out Array values);

    private static class Hash
    {
        public static readonly int s_PopupHash = "EditorPopup".GetHashCode();
    }

    protected static class Icon
    {
        private static GUIContent _delete;
        private static GUIContent _solo;

        public static GUIContent delete
        {
            get
            {
                if (_delete == null)
                {
                }
                return (_delete = new GUIContent(texDelete, "Delete"));
            }
        }

        public static GUIContent solo
        {
            get
            {
                if (_solo == null)
                {
                }
                return (_solo = new GUIContent(texSolo, "Solo Select"));
            }
        }

        public static Texture texDelete
        {
            get
            {
                return null;
            }
        }

        public static Texture texSolo
        {
            get
            {
                return null;
            }
        }
    }

    public enum ObjectFieldFlags
    {
        AllowScene = 1,
        Asset = 0x100,
        ForbidNull = 2,
        Instance = 0x10,
        Model = 8,
        NotInstance = 0x80,
        NotModel = 0x40,
        NotPrefab = 0x20,
        Prefab = 4,
        Root = 0x200
    }

    public enum ObjectKind
    {
        DisconnectedModelInstance = 8,
        DisconnectedPrefabInstance = 6,
        LevelInstance = 0,
        MissingPrefabInstance = 5,
        Model = 2,
        ModelInstance = 4,
        Null = -2,
        OtherAsset = 9,
        OtherInstance = 10,
        Prefab = 1,
        PrefabInstance = 3,
        ScriptableObject = 7,
        ScriptableObjectInstance = 11
    }

    public enum PeiceAction
    {
        None,
        AddToSelection,
        RemoveFromSelection,
        SelectSolo,
        Delete,
        Dirty,
        Ping
    }

    [StructLayout(LayoutKind.Sequential)]
    public struct PeiceCommand
    {
        public AuthorPeice peice;
        public AuthorShared.PeiceAction action;
    }

    [StructLayout(LayoutKind.Sequential, Size=1)]
    public struct PropMod
    {
        public static AuthorShared.PropMod New()
        {
            return new AuthorShared.PropMod();
        }

        public static AuthorShared.PropMod[] Get(Object o)
        {
            return new AuthorShared.PropMod[0];
        }

        public string propertyPath
        {
            get
            {
                return string.Empty;
            }
        }
        public string value
        {
            get
            {
                return string.Empty;
            }
        }
        public Object objectReference
        {
            get
            {
                return null;
            }
        }
        public Object target
        {
            get
            {
                return null;
            }
        }
        public static void Set(Object o, AuthorShared.PropMod[] mod)
        {
        }
    }

    public static class Scene
    {
        private const string _BoneParameters = "_B4";
        private const string _Height = "_Hv";
        private const string _LightScale = "_Lv";
        private const string _Radius = "_Rv";
        private const string _Sides = "_S3";
        private const string _ToolColor = "_Tc";
        private const int kShapeCount = 8;
        private const int SHAPE_BONE = 2;
        private const int SHAPE_BOX = 3;
        private const int SHAPE_CAPSULE_X = 4;
        private const int SHAPE_CAPSULE_Y = 5;
        private const int SHAPE_CAPSULE_Z = 6;
        private const int SHAPE_DISH = 1;
        private const int SHAPE_MESH = 0;
        private const int SHAPE_SPHERE = 7;

        public static bool BoxDrag(ref Vector3 center, ref Vector3 size)
        {
            return false;
        }

        private static float CapRadius(float radius, float height, int axis, int heightAxis)
        {
            if (heightAxis == axis)
            {
                return (radius + (height / 2f));
            }
            return radius;
        }

        public static bool CapsuleDrag(ref Vector3 center, ref float radius, ref float height, ref int heightAxis)
        {
            return false;
        }

        private static Vector3 Direction(int i)
        {
            switch ((i % 3))
            {
                case 1:
                    return (((((i / 3) % 2) * ((i / 3) % 2)) != 1) ? Vector3.up : Vector3.down);

                case 2:
                    return (((((i / 3) % 2) * ((i / 3) % 2)) != 1) ? Vector3.forward : Vector3.back);
            }
            return (((((i / 3) % 2) * ((i / 3) % 2)) != 1) ? Vector3.right : Vector3.left);
        }

        public static void DrawBone(Vector3 origin, Quaternion rot, float length, float backLength, Vector3 size)
        {
        }

        private static void DrawBoneNow(Vector3 origin, Quaternion forward, float length, float backLength, Vector3 size)
        {
        }

        public static void DrawBox(Vector3 center, Vector3 size)
        {
        }

        private static void DrawBoxNow(Vector3 center, Vector3 size)
        {
        }

        public static void DrawCapsule(Vector3 center, float radius, float height, int axis)
        {
        }

        private static void DrawCapsuleNow(Vector3 center, float radius, float height, int axis)
        {
        }

        public static void DrawSphere(Vector3 center, float radius)
        {
        }

        private static void DrawSphereNow(Vector3 center, float radius)
        {
        }

        public static float? GetAxialAngleDifference(Quaternion a, Quaternion b)
        {
            Vector3 vector;
            Vector3 vector2;
            float num;
            float num2;
            a.ToAngleAxis(out num, out vector);
            b.ToAngleAxis(out num2, out vector2);
            float num3 = Vector3.Dot(vector, vector2);
            if (Mathf.Approximately(num3, 1f))
            {
                return new float?(Mathf.DeltaAngle(num, num2));
            }
            if (Mathf.Approximately(num3, -1f))
            {
                return new float?(Mathf.DeltaAngle(num, -num2));
            }
            return null;
        }

        public static void GetUpAndRight(ref Vector3 forward, out Vector3 right, out Vector3 up)
        {
            forward.Normalize();
            float num = Vector3.Dot(forward, Vector3.up);
            if ((num * num) > 0.8099999f)
            {
                if ((forward.x * forward.x) <= (forward.z * forward.z))
                {
                    up = Vector3.Cross(forward, Vector3.right);
                }
                else
                {
                    up = Vector3.Cross(forward, Vector3.forward);
                }
                up.Normalize();
                right = Vector3.Cross(forward, up);
                right.Normalize();
            }
            else
            {
                right = Vector3.Cross(forward, Vector3.up);
                right.Normalize();
                up = Vector3.Cross(forward, right);
                up.Normalize();
            }
            if (Vector3.Dot(Vector3.Cross(up, forward), right) < 0f)
            {
                right = -right;
            }
        }

        public static bool LimitDrag(Vector3 anchor, Vector3 axis, ref JointLimits limit)
        {
            float min = limit.min;
            float max = limit.max;
            if (LimitDrag(anchor, axis, ref min, ref max))
            {
                limit.min = min;
                limit.max = max;
                limit.min = min;
                return true;
            }
            return false;
        }

        public static bool LimitDrag(Vector3 anchor, Vector3 axis, ref SoftJointLimit bothWays)
        {
            float limit = bothWays.limit;
            if (LimitDragBothWays(anchor, axis, ref limit))
            {
                bothWays.limit = limit;
                return true;
            }
            return false;
        }

        public static bool LimitDrag(Vector3 anchor, Vector3 axis, ref float min, ref float max)
        {
            float offset = 0f;
            return (LimitDrag(anchor, axis, ref offset, ref min, ref max) && (offset == 0f));
        }

        public static bool LimitDrag(Vector3 anchor, Vector3 axis, ref float offset, ref JointLimits limit)
        {
            float min = limit.min;
            float max = limit.max;
            if (LimitDrag(anchor, axis, ref offset, ref min, ref max))
            {
                limit.min = min;
                limit.max = max;
                return true;
            }
            return false;
        }

        public static bool LimitDrag(Vector3 anchor, Vector3 axis, ref float offset, ref SoftJointLimit bothWays)
        {
            float limit = bothWays.limit;
            if (LimitDragBothWays(anchor, axis, ref offset, ref limit))
            {
                bothWays.limit = limit;
                return true;
            }
            return false;
        }

        public static bool LimitDrag(Vector3 anchor, Vector3 axis, ref SoftJointLimit low, ref SoftJointLimit high)
        {
            float limit = low.limit;
            float max = high.limit;
            if (LimitDrag(anchor, axis, ref limit, ref max))
            {
                if (limit != low.limit)
                {
                    limit = Mathf.Clamp(limit, -180f, 180f);
                    if (limit != low.limit)
                    {
                        low.limit = limit;
                        return true;
                    }
                    return false;
                }
                if (max != high.limit)
                {
                    max = Mathf.Clamp(max, -180f, 180f);
                    if (max != high.limit)
                    {
                        high.limit = max;
                        return true;
                    }
                }
                else
                {
                    return true;
                }
            }
            return false;
        }

        public static bool LimitDrag(Vector3 anchor, Vector3 axis, ref float offset, ref float min, ref float max)
        {
            return false;
        }

        public static bool LimitDrag(Vector3 anchor, Vector3 axis, ref float offset, ref SoftJointLimit low, ref SoftJointLimit high)
        {
            float limit = low.limit;
            float max = high.limit;
            if (LimitDrag(anchor, axis, ref offset, ref limit, ref max))
            {
                if (limit != low.limit)
                {
                    limit = Mathf.Clamp(limit, -180f, 180f);
                    if (limit != low.limit)
                    {
                        low.limit = limit;
                        return true;
                    }
                    return false;
                }
                if (max != high.limit)
                {
                    max = Mathf.Clamp(max, -180f, 180f);
                    if (max != high.limit)
                    {
                        high.limit = max;
                        return true;
                    }
                }
                else
                {
                    return true;
                }
            }
            return false;
        }

        public static bool LimitDragBothWays(Vector3 anchor, Vector3 axis, ref float angle)
        {
            float offset = 0f;
            return (LimitDragBothWays(anchor, axis, ref offset, ref angle) && (offset == 0f));
        }

        public static bool LimitDragBothWays(Vector3 anchor, Vector3 axis, ref float offset, ref float angle)
        {
            return false;
        }

        public static bool PivotDrag(ref Vector3 anchor, ref Vector3 axis)
        {
            return false;
        }

        public static bool PointDrag(ref Vector3 anchor)
        {
            return false;
        }

        public static bool PointDrag(ref Vector3 anchor, ref Vector3 axis)
        {
            return false;
        }

        public static bool SphereDrag(ref Vector3 center, ref float radius)
        {
            return false;
        }

        public static Color color
        {
            get
            {
                return Color.white;
            }
            set
            {
            }
        }

        public static bool lighting
        {
            get
            {
                return false;
            }
            set
            {
            }
        }

        public static Matrix4x4 matrix
        {
            get
            {
                return Matrix4x4.identity;
            }
            set
            {
            }
        }

        private static class Keyword
        {
            private static readonly string[] BIT_STRINGS = new string[] { "SBA", "SBB", "SBC" };
            private const int BIT_STRINGS_LENGTH = 3;
            public static readonly string[][] SHAPE = new string[8][];

            static Keyword()
            {
                for (int i = 0; i < 8; i++)
                {
                    int num2 = 0;
                    for (int j = 0; j < 3; j++)
                    {
                        if ((i & (((int) 1) << j)) == (((int) 1) << j))
                        {
                            num2++;
                        }
                    }
                    SHAPE[i] = new string[num2];
                    int num4 = 0;
                    for (int k = 0; k < 3; k++)
                    {
                        if ((i & (((int) 1) << k)) == (((int) 1) << k))
                        {
                            SHAPE[i][num4++] = BIT_STRINGS[k];
                        }
                    }
                }
            }
        }
    }

    public static class Styles
    {
        private static StyleMod _gradientInline = new StyleMod(new StyleModFunctor(AuthorShared.Styles.CreateGradientInline));
        private static StyleMod _gradientInlineFill = new StyleMod(new StyleModFunctor(AuthorShared.Styles.CreateGradientInlineFill));
        private static StyleMod _gradientOutline = new StyleMod(new StyleModFunctor(AuthorShared.Styles.CreateGradientOutline));
        private static StyleMod _gradientOutlineFill = new StyleMod(new StyleModFunctor(AuthorShared.Styles.CreateGradientOutlineFill));
        private static StyleMod _infoLabel = new StyleMod(new StyleModFunctor(AuthorShared.Styles.CreateInfoLabel));
        private static StyleMod _palletButton = new StyleMod(iconAbove);
        private static StyleMod _peiceButtonLeft = new StyleMod(leftAlignText);
        private static StyleMod _peiceButtonMid = new StyleMod(centerAlignText);
        private static StyleMod _peiceButtonRight = new StyleMod(rightAlignText);
        private static StyleMod _subSectionTitle = new StyleMod(new StyleModFunctor(AuthorShared.Styles.CreateSubSectionTitleFill));
        private static readonly StyleModFunctor centerAlignText = new StyleModFunctor(AuthorShared.Styles.CenterAlignText);
        private static readonly StyleModFunctor iconAbove = new StyleModFunctor(AuthorShared.Styles.IconAbove);
        private static readonly StyleModFunctor leftAlignText = new StyleModFunctor(AuthorShared.Styles.LeftAlignText);
        private static readonly StyleModFunctor rightAlignText = new StyleModFunctor(AuthorShared.Styles.RightAlignText);

        private static void CenterAlignText(GUIStyle original, ref GUIStyle mod)
        {
            switch (original.alignment)
            {
                case TextAnchor.UpperLeft:
                case TextAnchor.UpperRight:
                    mod.alignment = TextAnchor.UpperCenter;
                    break;

                case TextAnchor.MiddleLeft:
                case TextAnchor.MiddleRight:
                    mod.alignment = TextAnchor.MiddleCenter;
                    break;

                case TextAnchor.LowerLeft:
                case TextAnchor.LowerRight:
                    mod.alignment = TextAnchor.LowerCenter;
                    break;
            }
        }

        private static void CreateGradientInline(GUIStyle original, ref GUIStyle mod)
        {
            mod.border = new RectOffset(1, 1, 1, 1);
            mod.normal = new GUIStyleState();
            mod.normal.background = (Texture2D) Resources.LoadAssetAtPath("Assets/AuthorSuite/Editor Resources/Icons/GradientInline.png", typeof(Texture2D));
        }

        private static void CreateGradientInlineFill(GUIStyle original, ref GUIStyle mod)
        {
            mod.border = new RectOffset(1, 1, 1, 1);
            mod.normal = new GUIStyleState();
            mod.normal.background = (Texture2D) Resources.LoadAssetAtPath("Assets/AuthorSuite/Editor Resources/Icons/GradientInlineFill.png", typeof(Texture2D));
        }

        private static void CreateGradientOutline(GUIStyle original, ref GUIStyle mod)
        {
            mod.border = new RectOffset(1, 1, 1, 1);
            mod.normal = new GUIStyleState();
            mod.normal.background = (Texture2D) Resources.LoadAssetAtPath("Assets/AuthorSuite/Editor Resources/Icons/GradientOutline.png", typeof(Texture2D));
        }

        private static void CreateGradientOutlineFill(GUIStyle original, ref GUIStyle mod)
        {
            mod.border = new RectOffset(1, 1, 1, 1);
            mod.normal = new GUIStyleState();
            mod.normal.background = (Texture2D) Resources.LoadAssetAtPath("Assets/AuthorSuite/Editor Resources/Icons/GradientOutlineFill.png", typeof(Texture2D));
        }

        private static void CreateInfoLabel(GUIStyle original, ref GUIStyle mod)
        {
            mod.alignment = TextAnchor.LowerLeft;
            mod.normal.textColor = new Color(1f, 1f, 1f, 0.17f);
        }

        private static void CreateSubSectionTitleFill(GUIStyle original, ref GUIStyle mod)
        {
            CreateGradientOutlineFill(original, ref mod);
            mod.alignment = TextAnchor.UpperRight;
            mod.font = boldLabel.font;
            mod.normal.textColor = new Color(0.03f, 0.03f, 0.03f, 1f);
            mod.stretchWidth = true;
        }

        private static void IconAbove(GUIStyle original, ref GUIStyle mod)
        {
            mod.imagePosition = ImagePosition.ImageAbove;
        }

        private static void LeftAlignText(GUIStyle original, ref GUIStyle mod)
        {
            switch (original.alignment)
            {
                case TextAnchor.UpperCenter:
                case TextAnchor.UpperRight:
                    mod.alignment = TextAnchor.UpperLeft;
                    break;

                case TextAnchor.MiddleCenter:
                case TextAnchor.MiddleRight:
                    mod.alignment = TextAnchor.MiddleLeft;
                    break;

                case TextAnchor.LowerCenter:
                case TextAnchor.LowerRight:
                    mod.alignment = TextAnchor.LowerLeft;
                    break;
            }
        }

        private static void RightAlignText(GUIStyle original, ref GUIStyle mod)
        {
            switch (original.alignment)
            {
                case TextAnchor.UpperLeft:
                case TextAnchor.UpperCenter:
                    mod.alignment = TextAnchor.UpperRight;
                    break;

                case TextAnchor.MiddleLeft:
                case TextAnchor.MiddleCenter:
                    mod.alignment = TextAnchor.MiddleRight;
                    break;

                case TextAnchor.LowerLeft:
                case TextAnchor.LowerCenter:
                    mod.alignment = TextAnchor.LowerRight;
                    break;
            }
        }

        public static GUIStyle boldLabel
        {
            get
            {
                return label;
            }
        }

        public static GUIStyle box
        {
            get
            {
                return GUI.skin.box;
            }
        }

        public static GUIStyle button
        {
            get
            {
                return GUI.skin.button;
            }
        }

        public static GUIStyle gradientInline
        {
            get
            {
                return _gradientInline.GetStyle(box);
            }
        }

        public static GUIStyle gradientInlineFill
        {
            get
            {
                return _gradientInlineFill.GetStyle(box);
            }
        }

        public static GUIStyle gradientOutline
        {
            get
            {
                return _gradientOutline.GetStyle(box);
            }
        }

        public static GUIStyle gradientOutlineFill
        {
            get
            {
                return _gradientOutlineFill.GetStyle(box);
            }
        }

        public static GUIStyle infoLabel
        {
            get
            {
                return _infoLabel.GetStyle(miniLabel);
            }
        }

        public static GUIStyle label
        {
            get
            {
                return GUI.skin.label;
            }
        }

        public static GUIStyle largeLabel
        {
            get
            {
                return label;
            }
        }

        public static GUIStyle largeWhiteLabel
        {
            get
            {
                return label;
            }
        }

        public static GUIStyle miniBoldLabel
        {
            get
            {
                return label;
            }
        }

        public static GUIStyle miniButton
        {
            get
            {
                return button;
            }
        }

        public static GUIStyle miniButtonLeft
        {
            get
            {
                return button;
            }
        }

        public static GUIStyle miniButtonMid
        {
            get
            {
                return button;
            }
        }

        public static GUIStyle miniButtonRight
        {
            get
            {
                return button;
            }
        }

        public static GUIStyle miniLabel
        {
            get
            {
                return label;
            }
        }

        public static GUIStyle palletButton
        {
            get
            {
                return _palletButton.GetStyle(miniButton);
            }
        }

        public static GUIStyle peiceButtonLeft
        {
            get
            {
                return _peiceButtonLeft.GetStyle(miniButtonLeft);
            }
        }

        public static GUIStyle peiceButtonMid
        {
            get
            {
                return _peiceButtonMid.GetStyle(miniButtonMid);
            }
        }

        public static GUIStyle peiceButtonRight
        {
            get
            {
                return _peiceButtonRight.GetStyle(miniButtonRight);
            }
        }

        public static GUIStyle subSection
        {
            get
            {
                return gradientOutline;
            }
        }

        public static GUIStyle subSectionTitle
        {
            get
            {
                return _subSectionTitle.GetStyle(box);
            }
        }

        [StructLayout(LayoutKind.Sequential)]
        private struct StyleMod
        {
            public readonly AuthorShared.Styles.StyleModFunctor functor;
            private GUIStyle original;
            private GUIStyle modified;
            public StyleMod(AuthorShared.Styles.StyleModFunctor functor)
            {
                this.functor = functor;
                this.original = (GUIStyle) (this.modified = null);
            }

            public GUIStyle GetStyle(GUIStyle original)
            {
                if (original == null)
                {
                    return null;
                }
                if (this.original != original)
                {
                    this.original = original;
                    this.modified = new GUIStyle(original);
                    try
                    {
                        this.functor(original, ref this.modified);
                        if (this.modified == null)
                        {
                        }
                        this.modified = this.original;
                    }
                    catch (Exception exception)
                    {
                        Debug.LogError(exception);
                    }
                }
                return this.modified;
            }
        }

        private delegate void StyleModFunctor(GUIStyle original, ref GUIStyle mod);
    }
}

