using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Networking;
using System.Collections;
using System.Collections.Generic; // Necessário para Dicionário (cache)
using TMPro;

public class PokeController : MonoBehaviour
{
    // --- Referências da UI ---
    [Header("UI do Aliado")]
    public Image allySpriteImage;
    public TextMeshProUGUI allyNameText;
    public TextMeshProUGUI allyLevelText; 
    public TextMeshProUGUI allyHpText;    
    public TextMeshProUGUI[] allyMoveTexts;
    
    [Header("UI Detalhes Movimentos")]
    // RENOMEEI: Estes campos agora são genéricos
    public TextMeshProUGUI moveDetails_PP_Text; 
    public TextMeshProUGUI moveDetails_Type_Text; 

    [Header("UI do Inimigo")]
    public Image enemySpriteImage;
    public TextMeshProUGUI enemyNameText;
    public TextMeshProUGUI enemyLevelText; 
    public TextMeshProUGUI enemyHpText;    

    // --- Caching dos Movimentos ---
    // Armazena as URLs dos 4 movimentos
    private List<string> moveUrls = new List<string>();
    // Armazena os detalhes (PP, Tipo) já buscados
    private Dictionary<int, MoveDetails> moveCache = new Dictionary<int, MoveDetails>();

    private string baseURL = "https://pokeapi.co/api/v2/pokemon/";

    void Start()
    {
        int randomPoke = Random.Range(1, 1026);
        StartCoroutine(CarregarPokemon(randomPoke));
    }

    public IEnumerator CarregarPokemon(int pokemonID)
    {
        //Limpa o cache e URLs antigas se estiver recarregando
        moveUrls.Clear();
        moveCache.Clear();

        string url = baseURL + pokemonID.ToString();
        UnityWebRequest request = UnityWebRequest.Get(url);
        yield return request.SendWebRequest();

        if (request.result != UnityWebRequest.Result.Success)
        {
            Debug.LogError("Erro na API (Pokemon): " + request.error);
            yield break; 
        }

        string jsonResponse = request.downloadHandler.text;
        PokemonData pokemon = JsonUtility.FromJson<PokemonData>(jsonResponse);

        //Nomes, Sprites, Level, HP
        string pokeName = CapitalizeFirstLetter(pokemon.name);
        allyNameText.text = pokeName;
        enemyNameText.text = pokeName;
        StartCoroutine(CarregarSprite(pokemon.sprites.back_default, allySpriteImage));
        StartCoroutine(CarregarSprite(pokemon.sprites.front_default, enemySpriteImage));
        
        int currentLevel = 50; 
        allyLevelText.text = "Lvl " + currentLevel;
        enemyLevelText.text = "Lvl " + currentLevel;
        int maxHp = 0;
        foreach (PokemonStat stat in pokemon.stats) {
            if (stat.stat.name == "hp") { maxHp = stat.base_stat; break; }
        }
        allyHpText.text = maxHp + " / " + maxHp;
        enemyHpText.text = maxHp + " / " + maxHp;
        
        //Processar os 4 Movimentos
        for (int i = 0; i < allyMoveTexts.Length; i++)
        {
            if (allyMoveTexts[i] != null)
            {
                if (i < pokemon.moves.Length)
                {
                    // Preenche o nome do movimento
                    string moveName = CapitalizeFirstLetter(pokemon.moves[i].move.name);
                    allyMoveTexts[i].text = moveName;
                    
                    // Salva a URL para buscar DEPOIS (no hover)
                    moveUrls.Add(pokemon.moves[i].move.url);
                }
                else
                {
                    allyMoveTexts[i].text = " - ";
                    moveUrls.Add(null); // Adiciona null para slots vazios
                }
            }
        }
        
        // Limpa a UI de detalhes ao carregar
        ClearMoveDetailsUI();
    }

    // --- FUNÇÕES DE HOVER (Chamadas pelo MoveHoverTrigger.cs) ---

    // Chamada quando o mouse entra em um texto de movimento
    public void OnMoveHover(int moveIndex)
    {
        // Verifica se é um slot de movimento válido (não é "-")
        if (moveIndex >= moveUrls.Count || moveUrls[moveIndex] == null)
        {
            ClearMoveDetailsUI(); // É um slot "-", então limpa a UI
            return;
        }

        // 1. Verificar se já temos no cache
        if (moveCache.ContainsKey(moveIndex))
        {
            // Se sim, usa os dados do cache (rápido!)
            UpdateMoveDetailsUI(moveCache[moveIndex]);
        }
        else
        {
            // 2. Se não, busca na API (Lazy Loading)
            StartCoroutine(CarregarDetalhesDoMovimento(moveIndex, moveUrls[moveIndex]));
            
            // Mostra um "Loading" temporário
            moveDetails_PP_Text.text = "--/--";
            moveDetails_Type_Text.text = "Carregando...";
        }
    }

    // Chamada quando o mouse sai do texto
    public void OnMoveHoverExit()
    {
        ClearMoveDetailsUI();
    }

    // --- Coroutine para buscar PP e TIPO (Atualizada) ---
    private IEnumerator CarregarDetalhesDoMovimento(int moveIndex, string moveUrl)
    {
        UnityWebRequest request = UnityWebRequest.Get(moveUrl);
        yield return request.SendWebRequest();

        if (request.result != UnityWebRequest.Result.Success)
        {
            Debug.LogError("Erro na API (Move): " + request.error);
            yield break;
        }

        string jsonResponse = request.downloadHandler.text;
        MoveDetails moveDetails = JsonUtility.FromJson<MoveDetails>(jsonResponse);

        // Salva os dados no cache
        moveCache[moveIndex] = moveDetails;

        // Atualiza a UI com os dados recém-baixados
        UpdateMoveDetailsUI(moveDetails);
    }

    // --- Funções Auxiliares de UI ---

    // Atualiza a UI de PP e Tipo
    private void UpdateMoveDetailsUI(MoveDetails details)
    {
        if (moveDetails_PP_Text != null)
        {
            moveDetails_PP_Text.text = details.pp + " / " + details.pp;
        }
        if (moveDetails_Type_Text != null)
        {
            moveDetails_Type_Text.text = CapitalizeFirstLetter(details.type.name);
        }
    }

    // Limpa a UI de PP e Tipo (quando o mouse sai)
    private void ClearMoveDetailsUI()
    {
        if (moveDetails_PP_Text != null)
        {
            moveDetails_PP_Text.text = "--/--";
        }
        if (moveDetails_Type_Text != null)
        {
            moveDetails_Type_Text.text = "Tipo"; // Ou deixe "" (vazio)
        }
    }

    // --- Coroutine de Sprite (Sem alterações) ---
    private IEnumerator CarregarSprite(string url, Image targetImage)
    {
        UnityWebRequest request = UnityWebRequestTexture.GetTexture(url);
        yield return request.SendWebRequest();
        if (request.result == UnityWebRequest.Result.Success) {
            Texture2D texture = ((DownloadHandlerTexture)request.downloadHandler).texture;
            texture.filterMode = FilterMode.Point; 
            Sprite newSprite = Sprite.Create(texture, new Rect(0, 0, texture.width, texture.height), new Vector2(0.5f, 0.5f));
            targetImage.sprite = newSprite;
        } else {
            Debug.LogError("Erro no Sprite: " + request.error);
        }
    }

    // --- Função de Capitalizar (Sem alterações) ---
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