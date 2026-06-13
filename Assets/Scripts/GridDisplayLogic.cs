using System;
using UnityEngine;

public class GridDisplayLogic : MonoBehaviour
{
    public void Hide()
    {
        this.gameObject.SetActive(false);
    }

    public void Show()
    {
        this.gameObject.SetActive(true);
    }
}
