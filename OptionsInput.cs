using System;
using UnityEngine;

public class OptionsInput : MonoBehaviour
{
    public GameObject lineObject;

    private void Start()
    {
        foreach (GameInput.GameButton button in GameInput.Buttons)
        {
            GameObject obj2 = (GameObject) Object.Instantiate(this.lineObject);
            obj2.transform.parent = base.transform;
            obj2.GetComponent<OptionsKeyBinding>().Setup(button);
        }
    }
}

