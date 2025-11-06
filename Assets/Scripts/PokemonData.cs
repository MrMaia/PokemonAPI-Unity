using System;



// [Serializable] permite que a Unity entenda essas classes para o JsonUtility.



[Serializable]

public class PokemonData

{

//O nome DEVE ser idêntico ao da API ("name")

    public string name;


//O nome DEVE ser idêntico ao da API ("sprites")

    public PokemonSprites sprites;



//NOVO: Adicionando Stats

    public PokemonStat[] stats;



//Adicionando Moves

    public PokemonMove[] moves;

}



[Serializable]

public class PokemonSprites

{

    public string front_default;

    public string back_default;

}



//Classe para Stats

[Serializable]

public class PokemonStat

{

    public int base_stat;

    public Stat stat;

}



//Classe para o nome do Stat (ex: "hp")

[Serializable]

public class Stat

{

    public string name;

}



//NOVO: Classe para Moves

[Serializable]

public class PokemonMove

{

    public Move move;

}



//NOVO: Classe para os detalhes do Move

[Serializable]

public class MoveDetails

{

    public int pp; // O PP que queremos

    public MoveType type; // O Tipo que queremos

}



//Classe para o nome do Tipo

[Serializable]

public class MoveType

{

    public string name;

}



//Classe para o nome do Move

[Serializable]

public class Move

{

    public string name;

    public string url;

}