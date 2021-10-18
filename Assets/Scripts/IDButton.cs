using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IDButton : MonoBehaviour
{
    public void OpenURL(string _url)
    {
        Application.OpenURL(_url);
    }
}
