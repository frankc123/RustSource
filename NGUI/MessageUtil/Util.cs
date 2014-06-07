namespace NGUI.MessageUtil
{
    using System;
    using System.Runtime.CompilerServices;
    using UnityEngine;

    public static class Util
    {
        public static void AltClick(this GameObject recv)
        {
            MSG(recv, "OnAltClick", null, false);
        }

        public static void AltDoubleClick(this GameObject recv)
        {
            MSG(recv, "OnAltDoubleClick", null, false);
        }

        public static void AltPress(this GameObject recv, bool press)
        {
            MSG(recv, "OnAltPress", Boxed.Box(press), true);
        }

        public static void Click(this GameObject recv)
        {
            MSG(recv, "OnClick", null, false);
        }

        public static void DoubleClick(this GameObject recv)
        {
            MSG(recv, "OnDoubleClick", null, false);
        }

        public static void Drag(this GameObject recv, Vector2 delta)
        {
            MSG(recv, "OnDrag", Boxed.Box<Vector2>(delta), true);
        }

        public static void DragState(this GameObject recv, bool dragging)
        {
            MSG(recv, "OnDragState", Boxed.Box(dragging), true);
        }

        public static void Drop(this GameObject recv, GameObject obj)
        {
            MSG(recv, "OnDrop", Boxed.Box<GameObject>(obj), true);
        }

        public static void Hover(this GameObject recv, bool highlight)
        {
            MSG(recv, "OnHover", Boxed.Box(highlight), true);
        }

        public static void Input(this GameObject recv, string input)
        {
            MSG(recv, "OnInput", Boxed.Box<string>(input), true);
        }

        public static void Key(this GameObject recv, KeyCode key)
        {
            MSG(recv, "OnKey", Boxed.Box(key), true);
        }

        public static void MidClick(this GameObject recv)
        {
            MSG(recv, "OnMidClick", null, false);
        }

        public static void MidDoubleClick(this GameObject recv)
        {
            MSG(recv, "OnMidDoubleClick", null, false);
        }

        public static void MidPress(this GameObject recv, bool press)
        {
            MSG(recv, "OnMidPress", Boxed.Box(press), true);
        }

        private static void MSG(GameObject recv, string message, object value, bool withValue)
        {
            if (recv != null)
            {
                if (withValue)
                {
                    if (object.ReferenceEquals(value, null))
                    {
                        Debug.LogWarning(string.Format("((GameObject){2}).SendMessage(\"{0}\", SendMessageOptions.{1}, null ) was not called because of the null argument.", message, SendMessageOptions.DontRequireReceiver, recv), recv);
                    }
                    else
                    {
                        try
                        {
                            recv.SendMessage(message, value, SendMessageOptions.DontRequireReceiver);
                        }
                        catch (Exception exception)
                        {
                            object[] args = new object[] { message, SendMessageOptions.DontRequireReceiver, recv, exception, value, value.GetType() };
                            Debug.LogError(string.Format("((GameObject){2}).SendMessage(\"{0}\", {4}({5}), SendMessageOptions.{1}) threw the exception below\r\n{3}", args), recv);
                        }
                    }
                }
                else
                {
                    try
                    {
                        recv.SendMessage(message, SendMessageOptions.DontRequireReceiver);
                    }
                    catch (Exception exception2)
                    {
                        object[] objArray2 = new object[] { message, SendMessageOptions.DontRequireReceiver, recv, exception2 };
                        Debug.LogError(string.Format("((GameObject){2}).SendMessage(\"{0}\", SendMessageOptions.{1}) threw the exception below\r\n{3}", objArray2), recv);
                    }
                }
            }
            else if (!withValue)
            {
                Debug.LogWarning(string.Format("((GameObject)null).SendMessage(\"{0}\", SendMessageOptions.{1})", message, SendMessageOptions.DontRequireReceiver));
            }
            else
            {
                Debug.LogWarning(string.Format("((GameObject)null).SendMessage(\"{0}\", {1}, SendMessageOptions.{2})", message, value, SendMessageOptions.DontRequireReceiver));
            }
        }

        public static void NGUIMessage(this GameObject recv, string message)
        {
            MSG(recv, message, null, false);
        }

        public static void NGUIMessage(this GameObject recv, string message, bool value)
        {
            MSG(recv, message, Boxed.Box(value), true);
        }

        public static void NGUIMessage(this GameObject recv, string message, int value)
        {
            MSG(recv, message, Boxed.Box(value), true);
        }

        public static void NGUIMessage(this GameObject recv, string message, object value)
        {
            MSG(recv, message, value, true);
        }

        public static void NGUIMessage(this GameObject recv, string message, GameObject value)
        {
            MSG(recv, message, Boxed.Box<GameObject>(value), true);
        }

        public static void NGUIMessage(this GameObject recv, string message, KeyCode value)
        {
            MSG(recv, message, Boxed.Box(value), true);
        }

        public static void NGUIMessage<T>(this GameObject recv, string message, T value)
        {
            MSG(recv, message, Boxed.Box<T>(value), true);
        }

        public static void Press(this GameObject recv, bool press)
        {
            MSG(recv, "OnPress", Boxed.Box(press), true);
        }

        public static void Scroll(this GameObject recv, float y)
        {
            MSG(recv, "OnScroll", Boxed.Box<float>(y), true);
        }

        public static void ScrollX(this GameObject recv, float x)
        {
            MSG(recv, "OnScrollX", Boxed.Box<float>(x), true);
        }

        public static void Select(this GameObject recv, bool selected)
        {
            MSG(recv, "OnSelect", Boxed.Box(selected), true);
        }

        public static void Tooltip(this GameObject recv, bool show)
        {
            MSG(recv, "OnTooltip", Boxed.Box(show), true);
        }
    }
}

