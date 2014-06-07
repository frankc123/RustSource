using System;
using UnityEngine;

public class HelloScript : MonoBehaviour
{
    public string helloString;

    private void Start()
    {
        Debug.Log("HELLO!:" + this.helloString + "from object: " + base.gameObject.name);
    }
}

