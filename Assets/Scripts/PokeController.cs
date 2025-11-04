using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Networking;
using System.Collections;
using TMPro; // Usando TextMeshPro

public class PokeController : MonoBehaviour
{
    // --- Referências da UI ---
    [Header("UI do Aliado")]
    public Image allySpriteImage;
    public TextMeshProUGUI allyNameText;
    public TextMeshProUGUI allyLevelText; // NOVO
    public TextMeshProUGUI allyHpText;    // NOVO
    public TextMeshProUGUI[] allyMoveTexts; // NOVO (Deve ter 4 elementos)

    [Header("UI do Inimigo")]
    public Image enemySpriteImage;
    public TextMeshProUGUI enemyNameText;
    public TextMeshProUGUI enemyLevelText; // NOVO
    public TextMeshProUGUI enemyHpText;    // NOVO
    // (Vamos deixar os moves do inimigo ocultos por enquanto, como no jogo)

    // --- URL Base da API ---
    private string baseURL = "https://pokeapi.co/api/v2/pokemon/";

    // --- Método de Exemplo ---
    void Start()
    {
        StartCoroutine(CarregarPokemon(4)); // Charmander (ID 4)
    }

    // --- Coroutine Principal para buscar dados do Pokémon ---
    public IEnumerator CarregarPokemon(int pokemonID)
    {
        string url = baseURL + pokemonID.ToString();

        UnityWebRequest request = UnityWebRequest.Get(url);
        yield return request.SendWebRequest();

        if (request.result == UnityWebRequest.Result.ConnectionError || request.result == UnityWebRequest.Result.ProtocolError)
        {
            Debug.LogError("Erro na API: " + request.error);
            yield break; 
        }

        string jsonResponse = request.downloadHandler.text;
        PokemonData pokemon = JsonUtility.FromJson<PokemonData>(jsonResponse);

        // --- Nomes e Sprites (Como antes) ---
        string pokeName = CapitalizeFirstLetter(pokemon.name);
        allyNameText.text = pokeName;
        enemyNameText.text = pokeName;

        StartCoroutine(CarregarSprite(pokemon.sprites.back_default, allySpriteImage));
        StartCoroutine(CarregarSprite(pokemon.sprites.front_default, enemySpriteImage));

        // --- NOVO: Processar Level e HP ---

        // 1. Level: O Level NÃO vem da API, é uma mecânica do jogo.
        // Vamos usar um valor fixo (placeholder) por enquanto.
        int currentLevel = 50; 
        allyLevelText.text = "Lvl " + currentLevel;
        enemyLevelText.text = "Lvl " + currentLevel;

        // 2. HP: A API nos dá o "base_stat" de HP.
        int maxHp = 0;
        foreach (PokemonStat stat in pokemon.stats)
        {
            if (stat.stat.name == "hp")
            {
                maxHp = stat.base_stat;
                break; // Achamos o HP, podemos parar o loop
            }
        }

        // O "96/96" do seu exemplo era um placeholder.
        // O HP base do Charmander é 39. Vamos exibir "39 / 39".
        // (O cálculo real do HP (96) depende do Level, IVs, EVs, etc.)
        
        // Vamos assumir que a vida está cheia (HP atual = HP máximo)
        allyHpText.text = maxHp + " / " + maxHp;
        enemyHpText.text = maxHp + " / " + maxHp;
        
        Debug.Log("HP Base: " + maxHp);

        // --- NOVO: Processar os 4 Movimentos ---
        
        // Vamos preencher os 4 textos de movimentos
        for (int i = 0; i < allyMoveTexts.Length; i++)
        {
            // Verifique se 'allyMoveTexts[i]' não é nulo antes de usar
            if (allyMoveTexts[i] != null)
            {
                if (i < pokemon.moves.Length)
                {
                    // Se o Pokémon tem um movimento para este slot
                    string moveName = CapitalizeFirstLetter(pokemon.moves[i].move.name);
                    allyMoveTexts[i].text = moveName;
                }
                else
                {
                    // Se não tem movimento (ex: Pokémon só tem 2), preenche com "-"
                    allyMoveTexts[i].text = " - ";
                }
            }
        }
    }


    // --- Coroutine para baixar a IMAGEM (Sprite) ---
    private IEnumerator CarregarSprite(string url, Image targetImage)
    {
        UnityWebRequest request = UnityWebRequestTexture.GetTexture(url);
        yield return request.SendWebRequest();

        if (request.result == UnityWebRequest.Result.ConnectionError || request.result == UnityWebRequest.Result.ProtocolError)
        {
            Debug.LogError("Erro no Sprite: " + request.error);
        }
        else
        {
            Texture2D texture = ((DownloadHandlerTexture)request.downloadHandler).texture;
            texture.filterMode = FilterMode.Point; // Mantém o visual pixelado
            
            Sprite newSprite = Sprite.Create(texture, new Rect(0, 0, texture.width, texture.height), new Vector2(0.5f, 0.5f));
            targetImage.sprite = newSprite;
        }
    }

    // --- Função Bônus: Para capitalizar nomes ---
    private string CapitalizeFirstLetter(string input)
    {
        if (string.IsNullOrEmpty(input))
            return string.Empty;
        
        // Substitui hífens por espaços (ex: "double-edge" vira "Double Edge")
        input = input.Replace('-', ' ');

        // Capitaliza cada palavra (ex: "double edge" vira "Double Edge")
        string[] parts = input.Split(' ');
        for(int i=0; i < parts.Length; i++)
        {
            if(parts[i].Length > 0)
                parts[i] = char.ToUpper(parts[i][0]) + parts[i].Substring(1);
        }
        return string.Join(" ", parts);
    }
}