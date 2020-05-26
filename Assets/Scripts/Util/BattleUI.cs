using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BattleUI : MonoBehaviour
{
    public GameObject HealthBarObject;
    public GameObject DppBarObject;
    public GameObject UiBarsBackgroundObject;
    public GameObject HighlighterObject;
    public GameObject TurnOrderObject;
    public GameObject SelectableObject;

    private Image _healthBar;
    private Image _dppBar;

    public Image HealthBar
    {
        get { return _healthBar ?? (_healthBar = HealthBarObject.GetComponent<Image>()); }
        set { HealthBar = value; }
    }

    public Image DppBar
    {
        get { return _dppBar ?? (_dppBar = DppBarObject.GetComponent<Image>()); }
        set { DppBar = value; }
    }
}
