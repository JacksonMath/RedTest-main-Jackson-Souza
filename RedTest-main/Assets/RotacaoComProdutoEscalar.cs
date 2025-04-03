using UnityEngine;

public class RotacaoComProdutoEscalar : MonoBehaviour
{
    public Transform alvo;
    public float velocidadeRotacao = 2f;
    public float limiteAlinhamento = 0.99f; // Quanto mais próximo de 1, mais alinhado

    void Update()
    {
        Quaternion rotacaoDesejada = Quaternion.LookRotation(alvo.position - transform.position);
        Vector3 enemy = (alvo.position - transform.position).normalized;
        enemy.y = 0;

        // Calcula o produto escalar entre a rotação atual e a desejada
        float dot = Quaternion.Dot(transform.rotation, rotacaoDesejada);

        // Só gira se ainda não estiver alinhado o suficiente
        if (dot < limiteAlinhamento)
        {
            transform.rotation = Quaternion.Slerp(transform.rotation, rotacaoDesejada, Time.deltaTime * velocidadeRotacao);
        }
    }
}
