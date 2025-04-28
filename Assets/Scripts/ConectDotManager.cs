using UnityEngine;

public class Dot : MonoBehaviour
{
    public int pointNumber; // N�mero do ponto (ordem)
}

public class ConnectDotsManager : MonoBehaviour
{
    public int currentPoint = 1; // Come�a do ponto 1

    void Update()
    {
        if (Input.GetMouseButtonDown(0)) // ou usar Touch para mobile
        {
            Vector2 worldPoint = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            RaycastHit2D hit = Physics2D.Raycast(worldPoint, Vector2.zero);

            if (hit.collider != null)
            {
                Dot dot = hit.collider.GetComponent<Dot>();
                if (dot != null)
                {
                    if (dot.pointNumber == currentPoint)
                    {
                        Debug.Log("Correto!");
                        currentPoint++;

                        // Aqui voc� pode checar se todos os pontos j� foram tocados
                    }
                    else
                    {
                        Debug.Log("Tocou o ponto errado! Game Over.");
                        // Aqui chama derrota
                    }
                }
                else if (hit.collider.CompareTag("FailArea"))
                {
                    Debug.Log("Tocou na �rea errada! Game Over.");
                    // Aqui chama derrota
                }
            }
        }
    }
}
