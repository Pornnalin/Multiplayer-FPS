using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class UIController : MonoBehaviour
{
    public static UIController instance;
    public TMP_Text overheatedMessage;
    public Slider weaponTempSlider;
    public TMP_Text deathText;
    public GameObject deathScreen;
    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
        else
        {
            Destroy(this.gameObject);
        }
    }
}