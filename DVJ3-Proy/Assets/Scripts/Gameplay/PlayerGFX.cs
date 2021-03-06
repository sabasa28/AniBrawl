﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerGFX : MonoBehaviour
{
    public GameObject swingPivot;
    public GameObject punch;
    [SerializeField] string stepsSound = null;

    public void MakeStepSound()
    {
        AkSoundEngine.PostEvent(stepsSound, gameObject);
    }
}
