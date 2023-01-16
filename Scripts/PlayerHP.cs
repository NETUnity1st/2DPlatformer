using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class PlayerHP : MonoBehaviour
{
    [SerializeField]
    public Slider hpBar;
    [SerializeField]
    public TextMeshProUGUI textHp;

    public static float maxHp = 100f;
    public static float curHp;
    

    // Start is called before the first frame update
    void Start()
    {
        curHp = maxHp;
    }

    // Update is called once per frame
    void Update()
    {
        hpBar.value = curHp / maxHp;
    }

    public void Onslider()
    {
        textHp.text = (hpBar.value * 100).ToString();
    }

}
