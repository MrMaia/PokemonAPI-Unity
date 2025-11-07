using System;

// [Serializable] permite ao JsonUtility da Unity converter esta classe
[Serializable]
public class PokemonData
{
    // Nomes de variáveis devem ser idênticos aos da API
    public string name;
    public PokemonSprites sprites;
    public PokemonStat[] stats; // Um array para a lista de stats
    public PokemonMove[] moves; // Um array para a lista de moves
}

[Serializable]
public class PokemonSprites
{
    // URLs das imagens para extrair
    public string front_default;
    public string back_default;
}

// --- Classes para Stats ---
// Estrutura aninhada do JSON de stats:
// stats -> [ { "base_stat": 45, "stat": { "name": "hp" } } ]
[Serializable]
public class PokemonStat
{
    public int base_stat; // Valor (ex: 45)
    public Stat stat;      // Objeto aninhado com o nome
}

[Serializable]
public class Stat
{
    public string name; // Nome (ex: "hp")
}

// --- Classes para Moves ---
// Estrutura aninhada do JSON de moves:
// moves -> [ { "move": { "name": "tackle", "url": "..." } } ]
[Serializable]
public class PokemonMove
{
    public Move move;
}

[Serializable]
public class Move
{
    public string name; // Nome do golpe
    public string url;  // URL para detalhes (PP, Tipo)
}

// --- Classes para Detalhes do Move ---
// Molde para o JSON da URL de detalhes do golpe.
[Serializable]
public class MoveDetails
{
    // Obtem apenas os campos que interessa (pp e type)
    public int pp;
    public MoveType type;
}

[Serializable]
public class MoveType
{
    public string name; // Nome do tipo (ex: "normal")
}