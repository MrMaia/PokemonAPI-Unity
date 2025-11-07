using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Networking;
using System.Collections;
using System.Collections.Generic; // Para List
using TMPro;

public class TeamUIController : MonoBehaviour
{
    [Header("Referências Principais")]
    // Referência ao PokeController para acessar a lista
    public PokeController pokeController; 

    [Header("Blocos da UI do Time")]
    // Lista dos 6 blocos da UI (via Inspector)
    public List<PokemonUIBlock> uiBlocks = new List<PokemonUIBlock>(6);

    // OnEnable() é chamado ao ativar o GameObject.
    // Usado para atualizar a UI ao abrir a tela.
    void OnEnable()
    {
        AtualizarTimeUI();
    }

    // Função principal que atualiza os 6 blocos
    public void AtualizarTimeUI()
    {
        // Valida se a referência ao PokeController existe
        if (pokeController == null)
        {
            Debug.LogError("PokeController não foi definido no TeamUIController!");
            return;
        }

        // Itera pelos 6 blocos da UI
        for (int i = 0; i < uiBlocks.Count; i++)
        {
            // Verifica se existe um Pokémon no time para este slot
            if (i < pokeController.timeAliado.Count)
            {
                // Se existe Pokémon, preenche o bloco
                PokeController.PokemonInstance pokemon = pokeController.timeAliado[i];
                PokemonUIBlock block = uiBlocks[i];

                // Preenche os dados
                block.nameText.text = CapitalizeFirstLetter(pokemon.data.name); 
                block.levelText.text = "Lvl " + pokemon.currentLevel;
                block.hpText.text = pokemon.maxHp + " / " + pokemon.maxHp;
                
                // Carrega o sprite
                StartCoroutine(CarregarSprite(pokemon.spriteUrl, block.pokemonSprite));
                
                // Ativa o bloco
                block.blockRoot.SetActive(true);
            }
            else
            {
                // Se não existe Pokémon, esconde o bloco
                uiBlocks[i].blockRoot.SetActive(false);
            }
        }
    }

    // --- Funções Auxiliares (copiadas do PokeController) ---

    private IEnumerator CarregarSprite(string url, Image targetImage)
    {
        UnityWebRequest request = UnityWebRequestTexture.GetTexture(url);
        yield return request.SendWebRequest();
        if (request.result == UnityWebRequest.Result.Success) {
            Texture2D texture = ((DownloadHandlerTexture)request.downloadHandler).texture;
            texture.filterMode = FilterMode.Point; // Mantém o pixel art
            Sprite newSprite = Sprite.Create(texture, new Rect(0, 0, texture.width, texture.height), new Vector2(0.5f, 0.5f));
            targetImage.sprite = newSprite;
        } else {
            Debug.LogError("Erro no Sprite: " + request.error);
        }
    }

    private string CapitalizeFirstLetter(string input)
    {
        if (string.IsNullOrEmpty(input)) return string.Empty;
        input = input.Replace('-', ' ');
        string[] parts = input.Split(' ');
        for(int i=0; i < parts.Length; i++) {
            if(parts[i].Length > 0)
                parts[i] = char.ToUpper(parts[i][0]) + parts[i].Substring(1);
        }
        return string.Join(" ", parts);
    }
}


// --- Classe Auxiliar ---
// Classe auxiliar (não-MonoBehaviour).
// Organiza as referências de UI de cada bloco no Inspector.
[System.Serializable]
public class PokemonUIBlock
{
    public GameObject blockRoot; // O objeto "pai" do bloco
    public Image pokemonSprite;
    public TextMeshProUGUI nameText;
    public TextMeshProUGUI levelText;
    public TextMeshProUGUI hpText;
}