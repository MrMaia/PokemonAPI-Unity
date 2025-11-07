using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Networking;   // Para UnityWebRequest (chamadas de API)
using System.Collections;
using System.Collections.Generic; // Para List e Dictionary
using TMPro;                    // Para TextMeshPro
using UnityEngine.SceneManagement;  // Para reiniciar a cena

public class PokeController : MonoBehaviour
{
    // --- Referências da UI (via Inspector) ---
    [Header("UI do Aliado")]
    public Image allySpriteImage;
    public TextMeshProUGUI allyNameText;
    public TextMeshProUGUI allyLevelText; 
    public TextMeshProUGUI allyHpText;    
    public TextMeshProUGUI[] allyMoveTexts; // Array para os 4 textos de golpes
    
    [Header("UI Detalhes Movimentos")]
    public TextMeshProUGUI moveDetails_PP_Text; 
    public TextMeshProUGUI moveDetails_Type_Text; 

    [Header("UI do Inimigo")]
    public Image enemySpriteImage;
    public TextMeshProUGUI enemyNameText;
    public TextMeshProUGUI enemyLevelText; 
    public TextMeshProUGUI enemyHpText;    

    // --- Controle Interno ---
    private List<string> moveUrls = new List<string>(); // Armazena as URLs dos 4 golpes
    private Dictionary<int, MoveDetails> moveCache = new Dictionary<int, MoveDetails>(); // Cache para detalhes de golpes
    private string baseURL = "https://pokeapi.co/api/v2/pokemon/";

    // --- Estrutura de Jogo ---
    // Struct para guardar os dados da API junto com os dados de jogo (nível, HP).
    public struct PokemonInstance
    {
        public PokemonData data;
        public int currentLevel;
        public int maxHp;
        public string spriteUrl; // URL do sprite para a tela de time

        // Construtor para facilitar a criação
        public PokemonInstance(PokemonData data, int level, int hp, string spriteUrl)
        {
            this.data = data;
            this.currentLevel = level;
            this.maxHp = hp;
            this.spriteUrl = spriteUrl;
        }
    }

    // Lista pública para armazenar o time. O TeamUIController lê esta lista.
    public List<PokemonInstance> timeAliado = new List<PokemonInstance>();


    void Start()
    {
        // Carrega um inimigo aleatório
        int enemyPokeId = Random.Range(1, 1026);
        StartCoroutine(CarregarPokemonInimigo(enemyPokeId));

        // Limpa o time (para recarregar a cena)
        timeAliado.Clear(); 

        // Carrega o primeiro aliado (que vai para a batalha)
        int primeiroAliadoId = Random.Range(1, 1026);
        StartCoroutine(CarregarPokemonAliado(primeiroAliadoId));

        // Carrega os 5 membros restantes da equipe
        for (int i = 0; i < 5; i++)
        {
            int aliadoAdicionalId = Random.Range(1, 1026);
            StartCoroutine(CarregarPokemonTimeAdicional(aliadoAdicionalId));
        }
    }

    // --- Coroutines de Carregamento (API) ---
    // Coroutines (IEnumerator) para chamadas de API, evitando travar o jogo.

    // Carrega o Pokémon do Aliado (que luta)
    public IEnumerator CarregarPokemonAliado(int pokemonID)
    {
        // Limpa dados de golpes anteriores
        moveUrls.Clear();
        moveCache.Clear();

        string url = baseURL + pokemonID.ToString();
        UnityWebRequest request = UnityWebRequest.Get(url);
        yield return request.SendWebRequest(); // Pausa a execução e aguarda a API

        if (request.result != UnityWebRequest.Result.Success)
        {
            Debug.LogError("Erro API (Aliado): " + request.error);
            yield break; // Encerra a coroutine em caso de erro
        }

        string jsonResponse = request.downloadHandler.text;
        PokemonData pokemon = JsonUtility.FromJson<PokemonData>(jsonResponse);

        // --- Atualiza a UI da Batalha ---
        allyNameText.text = CapitalizeFirstLetter(pokemon.name);
        StartCoroutine(CarregarSprite(pokemon.sprites.back_default, allySpriteImage));
        
        int currentLevel = Random.Range(1, 101);
        allyLevelText.text = "Lvl " + currentLevel;

        // --- Cálculo de HP ---
        int baseHp = 0;
        foreach (PokemonStat stat in pokemon.stats) {
            if (stat.stat.name == "hp") { 
                baseHp = stat.base_stat; 
                break; 
            }
        }
        float hpCalculado = ((2 * baseHp) * currentLevel) / 100.0f;
        int maxHp = Mathf.FloorToInt(hpCalculado) + 10 + currentLevel; 
        allyHpText.text = maxHp + " / " + maxHp;

        // --- Adiciona ao Time ---
        string spriteUrlParaTime = pokemon.sprites.front_default; 
        PokemonInstance aliadoAtual = new PokemonInstance(pokemon, currentLevel, maxHp, spriteUrlParaTime);
        timeAliado.Insert(0, aliadoAtual); // Insere na posição 0 (primeiro do time)
        
        // --- Processa os 4 Movimentos ---
        for (int i = 0; i < allyMoveTexts.Length; i++)
        {
            if (i < pokemon.moves.Length) // Se o Pokémon tiver o golpe
            {
                string moveName = CapitalizeFirstLetter(pokemon.moves[i].move.name).ToUpper();
                allyMoveTexts[i].text = moveName;
                moveUrls.Add(pokemon.moves[i].move.url); // Salva a URL para o hover
            }
            else
            {
                allyMoveTexts[i].text = " - ";
                moveUrls.Add(null); // Adiciona null para manter a lista com 4 itens
            }
        }
        
        ClearMoveDetailsUI(); // Limpa a UI de detalhes
    }

    // Carrega o Pokémon Inimigo
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

        // Atualiza a UI do inimigo
        enemyNameText.text = CapitalizeFirstLetter(pokemon.name);
        StartCoroutine(CarregarSprite(pokemon.sprites.front_default, enemySpriteImage));
        
        int currentLevel = Random.Range(1, 101);
        enemyLevelText.text = "Lvl " + currentLevel;
        enemyHpText.text = ""; // HP do inimigo não é exibido
    }

    // Carrega os 5 Pokémon restantes do time (sem atualizar a UI da batalha)
    public IEnumerator CarregarPokemonTimeAdicional(int pokemonID)
    {
        string url = baseURL + pokemonID.ToString();
        UnityWebRequest request = UnityWebRequest.Get(url);
        yield return request.SendWebRequest();

        if (request.result != UnityWebRequest.Result.Success)
        {
            Debug.LogError("Erro API (Time Adicional): " + request.error);
            yield break; 
        }

        string jsonResponse = request.downloadHandler.text;
        PokemonData pokemon = JsonUtility.FromJson<PokemonData>(jsonResponse);

        // Calcula Nível e HP
        int currentLevel = Random.Range(1, 101); 

        int baseHp = 0;
        foreach (PokemonStat stat in pokemon.stats) {
            if (stat.stat.name == "hp") { 
                baseHp = stat.base_stat; 
                break; 
            }
        }
        float hpCalculado = ((2 * baseHp) * currentLevel) / 100.0f;
        int maxHp = Mathf.FloorToInt(hpCalculado) + 10 + currentLevel; 

        // Adiciona ao final da lista do time
        string spriteUrlParaTime = pokemon.sprites.front_default; 
        PokemonInstance novoMembro = new PokemonInstance(pokemon, currentLevel, maxHp, spriteUrlParaTime);
        timeAliado.Add(novoMembro);
    }


    // --- Funções de Hover (Mouse) ---
    // Chamadas pelo script MoveHoverTrigger

    public void OnMoveHover(int moveIndex)
    {
        // Verifica se o golpe existe
        if (moveIndex >= moveUrls.Count || moveUrls[moveIndex] == null)
        {
            ClearMoveDetailsUI();
            return;
        }

        // Verifica o cache
        if (moveCache.ContainsKey(moveIndex))
        {
            UpdateMoveDetailsUI(moveCache[moveIndex]); // Usa o dado do cache
        }
        else
        {
            // Busca na API e exibe "Carregando..."
            StartCoroutine(CarregarDetalhesDoMovimento(moveIndex, moveUrls[moveIndex]));
            moveDetails_PP_Text.text = "--/--";
            moveDetails_Type_Text.text = "Carregando...";
        }
    }

    public void OnMoveHoverExit()
    {
        ClearMoveDetailsUI();
    }

    // Busca os detalhes do golpe (PP e Tipo)
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
        
        moveCache[moveIndex] = moveDetails; // Salva no cache
        UpdateMoveDetailsUI(moveDetails); // Atualiza a UI
    }

    // --- Funções Auxiliares de UI (Helpers) ---

    private void UpdateMoveDetailsUI(MoveDetails details)
    {
        moveDetails_PP_Text.text = details.pp + " / " + details.pp;
        moveDetails_Type_Text.text = CapitalizeFirstLetter(details.type.name).ToUpper();
    }

    private void ClearMoveDetailsUI()
    {
        moveDetails_PP_Text.text = "--/--";
        moveDetails_Type_Text.text = "Tipo";
    }

    // --- Funções Auxiliares Gerais (Helpers) ---

    // Coroutine para baixar e aplicar uma imagem (sprite)
    private IEnumerator CarregarSprite(string url, Image targetImage)
    {
        UnityWebRequest request = UnityWebRequestTexture.GetTexture(url);
        yield return request.SendWebRequest();

        if (request.result == UnityWebRequest.Result.Success) 
        {
            Texture2D texture = ((DownloadHandlerTexture)request.downloadHandler).texture;
            
            // Filtro Point para manter o estilo pixel art
            texture.filterMode = FilterMode.Point; 
            
            // Cria o Sprite a partir da Textura
            Sprite newSprite = Sprite.Create(texture, new Rect(0, 0, texture.width, texture.height), new Vector2(0.5f, 0.5f));
            targetImage.sprite = newSprite;
        } 
        else 
        {
            Debug.LogError("Erro no Sprite: " + request.error);
        }
    }

    // Formata o texto (ex: "mr-mime" -> "Mr Mime")
    private string CapitalizeFirstLetter(string input)
    {
        if (string.IsNullOrEmpty(input)) return string.Empty;

        input = input.Replace('-', ' '); // Troca traços por espaços
        string[] parts = input.Split(' '); // Separa palavras
        
        for(int i=0; i < parts.Length; i++) {
            if(parts[i].Length > 0)
                // Converte a primeira letra para Maiúscula e junta com o resto
                parts[i] = char.ToUpper(parts[i][0]) + parts[i].Substring(1);
        }
        return string.Join(" ", parts); // Junta as palavras novamente
    }
    
    // Função para o botão de Reiniciar (chamada pela UI)
    public void ReiniciarOJogo()
    {
        int cenaAtualIndex = SceneManager.GetActiveScene().buildIndex;
        SceneManager.LoadScene(cenaAtualIndex);
    }
}