﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ClientUseEffectButtonController : MonoBehaviour
{
    public const string EffDefaultUIString = "Use Effect";

    public TMPro.TMP_Text buttonText;

    private Effect eff;
    private ClientUIController clientUICtrl;

    public void Initialize(Effect eff, ClientUIController clientUICtrl)
    {
        this.eff = eff;
        buttonText.text = string.IsNullOrEmpty(eff.Blurb) ? EffDefaultUIString : eff.Blurb;
        this.clientUICtrl = clientUICtrl;
    }

    public void UseEffect()
    {
        clientUICtrl.ActivateSelectedCardEff(eff.EffectIndex);
    }
}