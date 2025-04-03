using UnityEngine;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{
    public Image powerBar; //barra de especial

    public void UpdateSpecialBar(float fillAmount)
    {
        if (powerBar != null)
        {
            Debug.Log($"Atualizando barra de especial: {fillAmount}");
            powerBar.fillAmount = fillAmount; //atualiza a barra de special
        }
    }
}
