<div align="center">
  <h1>⚡ PokemonAPI-Unity</h1>
  <p><strong>Simulador de batalha Pokémon em Unity, com dados, golpes e sprites consumidos ao vivo da PokéAPI</strong></p>

  <img src="https://img.shields.io/badge/Unity-6-black?style=for-the-badge&logo=unity&logoColor=white"/>
  <img src="https://img.shields.io/badge/c%23-%23239120.svg?style=for-the-badge&logo=c-sharp&logoColor=white"/>
  <img src="https://img.shields.io/badge/API-PokeAPI-red?style=for-the-badge"/>
  <img src="https://img.shields.io/badge/status-prot%C3%B3tipo-yellow?style=for-the-badge"/>
</div>

---

## 🚀 Sobre o Projeto

Projeto pessoal desenvolvido durante meu período como trainee na **BSA Tech**, empresa de jogos
de cassino, para treinar consumo de APIs REST, corrotinas e UI dentro da Unity. A ideia foi
recriar, em miniatura, uma tela de batalha Pokémon: um Pokémon aliado enfrenta um inimigo
aleatório, ambos com dados reais (stats, golpes, sprites) buscados direto da PokéAPI, além de uma
tela de time com os 6 membros do jogador.

---

## ✨ Funcionalidades

- ⚔️ Batalha 1x1 contra um Pokémon inimigo sorteado aleatoriamente
- 🎒 Time de até 6 Pokémon, sorteados e carregados de forma assíncrona
- 📊 Cálculo de HP máximo a partir do stat base e do nível (fórmula estilo jogo oficial)
- 🕹️ 4 golpes por Pokémon, com PP e tipo exibidos ao passar o mouse (com cache local)
- 🖼️ Sprites baixados dinamicamente da API, com filtro *Point* para preservar o pixel art
- 🔄 Botão de reiniciar, que recarrega a cena e sorteia um novo confronto

---

## 🛠️ Tecnologias

| Tecnologia | Uso |
|---|---|
| Unity 6 (6000.2.8f1) | Engine e runtime do jogo |
| C# | Lógica de gameplay e integração com a API |
| [PokéAPI](https://pokeapi.co/) | Fonte dos dados de Pokémon, golpes e sprites |
| UnityWebRequest / Coroutines | Requisições HTTP assíncronas sem travar o jogo |
| TextMesh Pro | Renderização de texto da UI |
| Universal Render Pipeline (URP) | Pipeline de renderização |
| Emerald UI Pack | Assets visuais no estilo Pokémon Emerald |

---

## 📁 Estrutura do Projeto

```
PokemonAPI-Unity/
├── Assets/
│   ├── Scripts/
│   │   ├── PokeController.cs      # Batalha, requisições à API e lista do time
│   │   ├── PokemonData.cs         # Modelos de dados (JSON da PokéAPI)
│   │   ├── TeamUIController.cs    # Preenche a tela de time a partir do PokeController
│   │   └── MoveHoverTrigger.cs    # Dispara o hover de detalhes de golpe
│   ├── Scenes/
│   ├── Emerald UI Pack 1.2/       # Assets visuais estilo Pokémon Emerald
│   └── TextMesh Pro/
├── ProjectSettings/
├── Packages/
└── LICENSE
```

---

## ⚙️ Como Executar Localmente

```bash
git clone https://github.com/MrMaia/PokemonAPI-Unity.git
```

1. Abra o **Unity Hub** e adicione a pasta clonada como projeto.
2. Use a versão **6000.2.8f1** (ou compatível) do Unity para abrir o projeto.
3. Abra a cena principal em `Assets/Scenes`.
4. Dê Play — é necessário estar conectado à internet, pois os dados vêm da PokéAPI em tempo real.

---

## 🔒 Privacidade / Aviso

Este projeto consome a [PokéAPI](https://pokeapi.co/), uma API pública e gratuita, apenas para
fins de estudo e demonstração. Pokémon e todos os nomes, sprites e dados relacionados são marcas
registradas da Nintendo, Game Freak e The Pokémon Company — este projeto não tem qualquer
afiliação com essas empresas.

---

<div align="center">
  <p>Feito com ❤️ por <strong>Allan Maia</strong></p>
</div>
