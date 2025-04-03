using UnityEngine;

public class HitTrigger : MonoBehaviour
{
    private void OnTriggerEnter(Collider enemy)
    {
        // Procura pelo primeiro Pai que tenha o script "Hit"
        Transform pai = transform;
        while (pai.parent != null)
        {
            pai = pai.parent;
            Hit ParentScript = pai.GetComponent<Hit>();
            if (ParentScript != null)
            {
                ParentScript.HitEnemy(enemy);
                break; // Encontra o primeiro pai com o script e para a busca
            }
        }
    }
}