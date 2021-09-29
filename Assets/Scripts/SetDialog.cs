using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SetDialog : MonoBehaviour
{
    private string PlayerTag = "Player";

    public string[] DialogTextBlocks;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.tag == PlayerTag)
        {
            collision.gameObject.GetComponentInChildren<DialogController>().InitDialog(DialogTextBlocks);
            Destroy(gameObject);
        }
    }
}
