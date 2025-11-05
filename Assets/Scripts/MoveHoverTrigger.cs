using UnityEngine;
using UnityEngine.EventSystems; // Necessário para detectar o mouse

public class MoveHoverTrigger : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    // Crie estes campos públicos para configurar no Inspector
    public int moveIndex;
    public PokeController pokeController;

    // Esta função é chamada quando o mouse ENTRA na área do objeto
    public void OnPointerEnter(PointerEventData eventData)
    {
        // Diz ao PokeController QUAL movimento está em foco
        if (pokeController != null)
        {
            pokeController.OnMoveHover(moveIndex);
        }
    }

    // Esta função é chamada quando o mouse SAI da área do objeto
    public void OnPointerExit(PointerEventData eventData)
    {
        // Diz ao PokeController que o mouse saiu
        if (pokeController != null)
        {
            pokeController.OnMoveHoverExit();
        }
    }
}