using System;

// [Serializable] permite que a Unity entenda essas classes
// para o JsonUtility.

[Serializable]
public class PokemonData
{
    // O nome DEVE ser idêntico ao da API ("name")
    public string name;
    
    // O nome DEVE ser idêntico ao da API ("sprites")
    public PokemonSprites sprites;

    // --- NOVO: Adicionando Stats ---
    public PokemonStat[] stats;

    // --- NOVO: Adicionando Moves ---
    public PokemonMove[] moves;
}

[Serializable]
public class PokemonSprites
{
    public string front_default;
    public string back_default;
}

// --- NOVO: Classe para Stats ---
[Serializable]
public class PokemonStat
{
    public int base_stat;
    public Stat stat; // "stat" é um sub-objeto
}

// --- NOVO: Classe para o nome do Stat (ex: "hp") ---
[Serializable]
public class Stat
{
    public string name; 
}

// --- NOVO: Classe para Moves ---
[Serializable]
public class PokemonMove
{
    public Move move; // "move" é um sub-objeto
}

// --- NOVO: Classe para o nome do Move ---
[Serializable]
public class Move
{
    public string name;
    // Vamos adicionar a URL aqui no próximo passo para buscar o PP
}