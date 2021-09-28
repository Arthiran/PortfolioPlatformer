using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HyperlinkButton : MonoBehaviour
{
    [SerializeField]
    private string URLtext;

    private SpriteRenderer sprite;
    private Color originalColour;
    private Color DarkColour = new Color(0.8f, 0.8f, 0.8f);

    private void Awake()
    {
        sprite = GetComponent<SpriteRenderer>();
        originalColour = sprite.color;
    }

    private void OnMouseEnter()
    {
        sprite.color = originalColour * DarkColour;
    }

    private void OnMouseDown()
    {
        OpenURL(URLtext);
    }

    private void OnMouseExit()
    {
        sprite.color = originalColour;
    }

    public void OpenURL(string _url)
    {
        Application.OpenURL(_url);
    }
}
