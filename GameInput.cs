using System;
using UnityEngine;

public class GameInput : MonoBehaviour
{
    public static GameButton[] Buttons = new GameButton[] { new GameButton("Left"), new GameButton("Right"), new GameButton("Up"), new GameButton("Down"), new GameButton("Jump"), new GameButton("Duck"), new GameButton("Sprint"), new GameButton("Fire"), new GameButton("AltFire"), new GameButton("Reload"), new GameButton("Use"), new GameButton("Inventory"), new GameButton("Flashlight"), new GameButton("Laser"), new GameButton("Voice"), new GameButton("Chat") };

    public static GameButton GetButton(string strName)
    {
        foreach (GameButton button in Buttons)
        {
            if (button.Name == strName)
            {
                return button;
            }
        }
        return null;
    }

    public static string GetConfig()
    {
        string str = string.Empty;
        foreach (GameButton button in Buttons)
        {
            string str2 = str;
            string[] textArray1 = new string[] { str2, "input.bind ", button.Name, " ", button.bindingOne.ToString(), " ", button.bindingTwo.ToString(), "\n" };
            str = string.Concat(textArray1);
        }
        return str;
    }

    public static Vector2 mouseDelta
    {
        get
        {
            Vector2 vector;
            vector.x = mouseDeltaX;
            vector.y = mouseDeltaY;
            return vector;
        }
    }

    public static float mouseDeltaX
    {
        get
        {
            return (Input.GetAxis("Mouse X") * mouseSensitivity);
        }
    }

    public static float mouseDeltaY
    {
        get
        {
            return (Input.GetAxis("Mouse Y") * mouseSensitivity);
        }
    }

    public static float mouseSensitivity
    {
        get
        {
            return input.mousespeed;
        }
    }

    public class GameButton
    {
        public KeyCode bindingOne;
        public KeyCode bindingTwo;
        public readonly string Name;

        internal GameButton(string NiceName)
        {
            this.Name = NiceName;
        }

        public void Bind(string A, string B)
        {
            SetKeyCode(ref this.bindingOne, A);
            SetKeyCode(ref this.bindingTwo, B);
        }

        public bool IsDown()
        {
            return (IsKeyHeld(this.bindingOne) || ((this.bindingOne != this.bindingTwo) && IsKeyHeld(this.bindingTwo)));
        }

        private static bool IsKeyHeld(KeyCode key)
        {
            return ((key != KeyCode.None) && Input.GetKey(key));
        }

        public bool IsPressed()
        {
            if (WasKeyPressed(this.bindingOne))
            {
                return (((this.bindingTwo == this.bindingOne) || WasKeyPressed(this.bindingTwo)) || !IsKeyHeld(this.bindingTwo));
            }
            return (((this.bindingTwo != this.bindingOne) && WasKeyPressed(this.bindingTwo)) && !IsKeyHeld(this.bindingOne));
        }

        public bool IsReleased()
        {
            if (WasKeyReleased(this.bindingOne))
            {
                return (((this.bindingTwo == this.bindingOne) || WasKeyReleased(this.bindingTwo)) || !IsKeyHeld(this.bindingTwo));
            }
            return (((this.bindingTwo != this.bindingOne) && WasKeyReleased(this.bindingTwo)) && !IsKeyHeld(this.bindingOne));
        }

        public static bool operator false(GameInput.GameButton gameButton)
        {
            return (!object.ReferenceEquals(gameButton, null) && !gameButton.IsDown());
        }

        public static bool operator true(GameInput.GameButton gameButton)
        {
            return (!object.ReferenceEquals(gameButton, null) && gameButton.IsDown());
        }

        private static KeyCode? ParseKeyCode(string name)
        {
            try
            {
                return new KeyCode?((KeyCode) ((int) Enum.Parse(typeof(KeyCode), name, true)));
            }
            catch (Exception exception)
            {
                Debug.LogException(exception);
                return null;
            }
        }

        private static void SetKeyCode(ref KeyCode value, string name)
        {
            KeyCode? nullable = ParseKeyCode(name);
            value = !nullable.HasValue ? value : nullable.Value;
        }

        public override string ToString()
        {
            if (this.Name == null)
            {
            }
            return string.Empty;
        }

        private static bool WasKeyPressed(KeyCode key)
        {
            return ((key != KeyCode.None) && Input.GetKeyDown(key));
        }

        private static bool WasKeyReleased(KeyCode key)
        {
            return ((key != KeyCode.None) && Input.GetKeyUp(key));
        }
    }
}

