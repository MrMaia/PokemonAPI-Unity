using UnityEngine;
using UnityEngine.EventSystems; // Necessário para as interfaces de evento

// Implementa interfaces para eventos de mouse (PointerEnter, PointerExit)
public class MoveHoverTrigger : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    // --- Variáveis de Configuração (via Inspector) ---
    
    // Índice do golpe (0, 1, 2 ou 3)
    public int moveIndex;
    
    // Referência ao PokeController
    public PokeController pokeController;

    
    // Chamado pela Unity quando o mouse entra
    public void OnPointerEnter(PointerEventData eventData)
    {
        if (pokeController != null)
        {
            // Avisa o PokeController qual golpe está em foco
            pokeController.OnMoveHover(moveIndex);
        }
    }

    // Chamado pela Unity quando o mouse sai
    public void OnPointerExit(PointerEventData eventData)
    {
        if (pokeController != null)
        {
            // Avisa o PokeController que o mouse saiu
            pokeController.OnMoveHoverExit();
        }
    }
}