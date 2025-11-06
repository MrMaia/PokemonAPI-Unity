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
    public TextMeshProUGUI moveDetails_PP_Text; 
    public TextMeshProUGUI moveDetails_Type_Text; 

    [Header("UI do Inimigo")]
    public Image enemySpriteImage;
    public TextMeshProUGUI enemyNameText;
    public TextMeshProUGUI enemyLevelText; 
    public TextMeshProUGUI enemyHpText;    

    // --- Caching dos Movimentos ---
    private List<string> moveUrls = new List<string>();
    private Dictionary<int, MoveDetails> moveCache = new Dictionary<int, MoveDetails>();

    private string baseURL = "https://pokeapi.co/api/v2/pokemon/";

    void Start()
    {
        // --- SOLICITAÇÃO 1: Pokémon aleatório para Aliado e Inimigo ---
        int allyPokeId = Random.Range(1, 1026);
        int enemyPokeId = Random.Range(1, 1026);
        
        StartCoroutine(CarregarPokemonAliado(allyPokeId));
        StartCoroutine(CarregarPokemonInimigo(enemyPokeId));
    }

    // --- Coroutine dividida: Apenas para o ALIADO ---
    public IEnumerator CarregarPokemonAliado(int pokemonID)
    {
        //Limpa o cache e URLs antigas
        moveUrls.Clear();
        moveCache.Clear();

        string url = baseURL + pokemonID.ToString();
        UnityWebRequest request = UnityWebRequest.Get(url);
        yield return request.SendWebRequest();

        if (request.result != UnityWebRequest.Result.Success)
        {
            Debug.LogError("Erro API (Aliado): " + request.error);
            yield break; 
        }

        string jsonResponse = request.downloadHandler.text;
        PokemonData pokemon = JsonUtility.FromJson<PokemonData>(jsonResponse);

        // --- Nomes e Sprites (Apenas Aliado) ---
        allyNameText.text = CapitalizeFirstLetter(pokemon.name);
        StartCoroutine(CarregarSprite(pokemon.sprites.back_default, allySpriteImage));
        
        // --- SOLICITAÇÃO 2: Level Aleatório ---
        int currentLevel = Random.Range(1, 101); // Nível de 1 a 100
        allyLevelText.text = "Lvl " + currentLevel;

        // --- HP (Apenas Aliado) ---
        int maxHp = 0;
        foreach (PokemonStat stat in pokemon.stats) {
            if (stat.stat.name == "hp") { maxHp = stat.base_stat; break; }
        }
        allyHpText.text = maxHp + " / " + maxHp;
        
        // --- Processar os 4 Movimentos (Apenas Aliado) ---
        for (int i = 0; i < allyMoveTexts.Length; i++)
        {
            if (allyMoveTexts[i] != null)
            {
                if (i < pokemon.moves.Length)
                {
                    // --- SOLICITAÇÃO 4: Texto dos Moves em MAIÚSCULO ---
                    string moveName = CapitalizeFirstLetter(pokemon.moves[i].move.name).ToUpper();
                    allyMoveTexts[i].text = moveName;
                    
                    moveUrls.Add(pokemon.moves[i].move.url);
                }
                else
                {
                    allyMoveTexts[i].text = " - ";
                    moveUrls.Add(null);
                }
            }
        }
        
        ClearMoveDetailsUI();
    }

    // --- NOVA Coroutine: Apenas para o INIMIGO ---
    public IEnumerator CarregarPokemonInimigo(int pokemonID)
    {
        string url = baseURL + pokemonID.ToString();
        UnityWebRequest request = UnityWebRequest.Get(url);
        yield return request.SendWebRequest();

        if (request.result != UnityWebRequest.Result.Success)
        {
            Debug.LogError("Erro API (Inimigo): " + request.error);
            yield break; 
        }

        string jsonResponse = request.downloadHandler.text;
        PokemonData pokemon = JsonUtility.FromJson<PokemonData>(jsonResponse);

        // --- Nomes e Sprites (Apenas Inimigo) ---
        enemyNameText.text = CapitalizeFirstLetter(pokemon.name);
        StartCoroutine(CarregarSprite(pokemon.sprites.front_default, enemySpriteImage));
        
        // --- SOLICITAÇÃO 2: Level Aleatório ---
        int currentLevel = Random.Range(1, 101); // Nível de 1 a 100
        enemyLevelText.text = "Lvl " + currentLevel;

        // --- HP (Apenas Inimigo) ---
        int maxHp = 0;
        foreach (PokemonStat stat in pokemon.stats) {
            if (stat.stat.name == "hp") { maxHp = stat.base_stat; break; }
        }
        
        // --- SOLICITAÇÃO 3: Sem HP para o Inimigo ---
        enemyHpText.text = ""; // Simplesmente deixamos o texto vazio
        // (Se preferir, pode fazer: enemyHpText.gameObject.SetActive(false);)
    }


    // --- FUNÇÕES DE HOVER (Sem alteração, exceto pelo ToUpper()) ---
    public void OnMoveHover(int moveIndex)
    {
        if (moveIndex >= moveUrls.Count || moveUrls[moveIndex] == null)
        {
            ClearMoveDetailsUI();
            return;
        }

        if (moveCache.ContainsKey(moveIndex))
        {
            UpdateMoveDetailsUI(moveCache[moveIndex]);
        }
        else
        {
            StartCoroutine(CarregarDetalhesDoMovimento(moveIndex, moveUrls[moveIndex]));
            moveDetails_PP_Text.text = "--/--";
            moveDetails_Type_Text.text = "Carregando...";
        }
    }

    public void OnMoveHoverExit()
    {
        ClearMoveDetailsUI();
    }

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
        moveCache[moveIndex] = moveDetails;
        UpdateMoveDetailsUI(moveDetails);
    }

    private void UpdateMoveDetailsUI(MoveDetails details)
    {
        if (moveDetails_PP_Text != null)
        {
            moveDetails_PP_Text.text = details.pp + " / " + details.pp;
        }
        if (moveDetails_Type_Text != null)
        {
            // --- SOLICITAÇÃO 4: Texto do TIPO em MAIÚSCULO ---
            moveDetails_Type_Text.text = CapitalizeFirstLetter(details.type.name).ToUpper();
        }
    }

    private void ClearMoveDetailsUI()
    {
        if (moveDetails_PP_Text != null)
        {
            moveDetails_PP_Text.text = "--/--";
        }
        if (moveDetails_Type_Text != null)
        {
            moveDetails_Type_Text.text = "Tipo";
        }
    }

    // --- Coroutine de Sprite (Sem alterações) ---
    private IEnumerator CarregarSprite(string url, Image targetImage)
    {
        UnityWebRequest request = UnityWebRequestTexture.GetTexture(url);
        yield return request.SendWebRequest();
        if (request.result == UnityWebRequest.Result.Success) {
            Texture2D texture = ((DownloadHandlerTexture)request.downloadHandler).texture;
            
            // --- SOLICITAÇÃO 5: Qualidade da Imagem ---
            // A linha abaixo JÁ ESTÁ no seu script e é a correta para pixel art.
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