using System.Collections;
using System.Collections.Generic;
using Jujutsu_Kaisen_Game_Proyect.Assets.BackEnd;
using UnityEngine;

public class InfoGame : MonoBehaviour
{
    public GameObject panel;
    private bool isActive;

    public void Active()
    {
        if (isActive)
        {
            isActive = false;
            panel.SetActive(false);
        }
        else
        {
            isActive = true;
            panel.SetActive(true);
        }
    }
}
