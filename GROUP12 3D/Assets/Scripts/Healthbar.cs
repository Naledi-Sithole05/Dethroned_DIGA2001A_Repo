using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;

public class Healthbar : MonoBehaviour
{
    
    public Slider Slider;

    public void SetMaxHealth(int health)
    {
        Slider.value = health;  
        Slider.value= health;
    }
    public void SetHealth(int  health)
    {
        Slider.value = health;
    }
}
