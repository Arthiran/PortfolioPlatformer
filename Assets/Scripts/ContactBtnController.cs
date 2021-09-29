using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ContactBtnController : MonoBehaviour
{
    public GameObject IDCard;

    public void OpenID()
    {
        IDCard.SetActive(IDCard.activeSelf ? false : true);
    }
}
