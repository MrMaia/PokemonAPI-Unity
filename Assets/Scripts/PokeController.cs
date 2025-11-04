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
    public TextMeshProUGUI allyLevelText; 
    public TextMeshProUGUI allyHpText;    
    public TextMeshProUGUI[] allyMoveTexts; // (Array de 4 textos, como antes)
    
    // --- NOVO: UI para detalhes do primeiro movimento ---
    [Header("UI Detalhes Movimentos")]
    public TextMeshProUGUI move1_PP_Text; // Texto para o PP (ex: "PP 35/35")
    public TextMeshProUGUI move1_Type_Text; // Texto para o Tipo (ex: "NORMAL")
    // (Você pode adicionar mais para move2, move3, move4 depois)


    [Header("UI do Inimigo")]
    public Image enemySpriteImage;
    public TextMeshProUGUI enemyNameText;
    public TextMeshProUGUI enemyLevelText; 
    public TextMeshProUGUI enemyHpText;    

    // --- URL Base da API ---
    private string baseURL = "https://pokeapi.co/api/v2/pokemon/";

    // --- Método de Exemplo ---
    void Start()
    {
        // Charmander (ID 4) ou Bulbasaur (ID 1) são bons testes
        StartCoroutine(CarregarPokemon(3)); 
    }

    // --- Coroutine Principal para buscar dados do Pokémon ---
    public IEnumerator CarregarPokemon(int pokemonID)
    {
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

        // --- Nomes e Sprites ---
        string pokeName = CapitalizeFirstLetter(pokemon.name);
        allyNameText.text = pokeName;
        enemyNameText.text = pokeName;

        StartCoroutine(CarregarSprite(pokemon.sprites.back_default, allySpriteImage));
        StartCoroutine(CarregarSprite(pokemon.sprites.front_default, enemySpriteImage));

        // --- Level e HP ---
        int currentLevel = 50; 
        allyLevelText.text = "Lvl " + currentLevel;
        enemyLevelText.text = "Lvl " + currentLevel;

        int maxHp = 0;
        foreach (PokemonStat stat in pokemon.stats)
        {
            if (stat.stat.name == "hp")
            {
                maxHp = stat.base_stat;
                break; 
            }
        }
        
        allyHpText.text = maxHp + " / " + maxHp;
        enemyHpText.text = maxHp + " / " + maxHp;
        
        // --- Processar os 4 Movimentos ---
        for (int i = 0; i < allyMoveTexts.Length; i++)
        {
            if (allyMoveTexts[i] != null)
            {
                if (i < pokemon.moves.Length)
                {
                    string moveName = CapitalizeFirstLetter(pokemon.moves[i].move.name);
                    allyMoveTexts[i].text = moveName;

                    // --- NOVO: Buscar detalhes do PRIMEIRO movimento ---
                    if (i == 0)
                    {
                        // Pegamos a URL do movimento e iniciamos a nova coroutine
                        string moveURL = pokemon.moves[i].move.url;
                        StartCoroutine(CarregarDetalhesDoMovimento(moveURL));
                    }
                }
                else
                {
                    allyMoveTexts[i].text = " - ";
                }
            }
        }
    }

    // --- NOVO: Coroutine para buscar PP e TIPO do movimento ---
    private IEnumerator CarregarDetalhesDoMovimento(string moveUrl)
    {
        UnityWebRequest request = UnityWebRequest.Get(moveUrl);
        yield return request.SendWebRequest();

        if (request.result != UnityWebRequest.Result.Success)
        {
            Debug.LogError("Erro na API (Move): " + request.error);
            yield break;
        }

        // 1. Pegar o JSON do movimento
        string jsonResponse = request.downloadHandler.text;

        // 2. Converter usando nossas NOVAS classes
        MoveDetails moveDetails = JsonUtility.FromJson<MoveDetails>(jsonResponse);

        // 3. Extrair os dados
        int pp = moveDetails.pp;
        string typeName = CapitalizeFirstLetter(moveDetails.type.name);

        Debug.Log("Detalhes do Movimento: " + typeName + ", PP: " + pp);

        // 4. Atualizar a UI
        if (move1_PP_Text != null)
        {
            // (Assumindo que o PP atual é igual ao máximo no início)
            move1_PP_Text.text = "PP " + pp + " / " + pp;
        }

        if (move1_Type_Text != null)
        {
            move1_Type_Text.text = typeName;
        }
    }


    // --- Coroutine para baixar a IMAGEM (Sprite) ---
    private IEnumerator CarregarSprite(string url, Image targetImage)
    {
        UnityWebRequest request = UnityWebRequestTexture.GetTexture(url);
        yield return request.SendWebRequest();

        if (request.result == UnityWebRequest.Result.Success)
        {
            Texture2D texture = ((DownloadHandlerTexture)request.downloadHandler).texture;
            texture.filterMode = FilterMode.Point; 
            
            Sprite newSprite = Sprite.Create(texture, new Rect(0, 0, texture.width, texture.height), new Vector2(0.5f, 0.5f));
            targetImage.sprite = newSprite;
        }
        else
        {
            Debug.LogError("Erro no Sprite: " + request.error);
        }
    }

    // --- Função Bônus: Para capitalizar nomes ---
    private string CapitalizeFirstLetter(string input)
    {
        if (string.IsNullOrEmpty(input))
            return string.Empty;
        
        input = input.Replace('-', ' ');
        string[] parts = input.Split(' ');
        for(int i=0; i < parts.Length; i++)
        {
            if(parts[i].Length > 0)
                parts[i] = char.ToUpper(parts[i][0]) + parts[i].Substring(1);
        }
        return string.Join(" ", parts);
    }
}