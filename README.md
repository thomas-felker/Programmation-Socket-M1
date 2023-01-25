# SocketProgrammingProject

![](banner.png)

# **<u>TP2 - PROGRAMMATION SOCKET</u>**

Dans le cadre d'un projet qui s'est déroulé d'Octobre 2022 à Décembre 2022, notre binôme, composé de Thomas FELKER et Jules THERET, a tenté de développer une application de jeu du pendu via la programmation socket.

## **Partie 1 :** Jeu entre un joueur et le serveur

Il nous a fallu dans un premier temps définir les règles du jeu ainsi que le déroulement d’une partie, puis établir un protocole permettant un dialogue entre le client et le serveur.

### 1. Quel service est rendu par le serveur ?

    Le serveur contient pour ainsi dire la "logique" du jeu du pendu.
    Il se charge de vérifier la proposition de lettre pour le mot à deviner effectuée par le client.
    Si la proposition en question est correcte, il le lui fait savoir par un message et dévoile ensuite la nouvelle lettre précédemment trouvée.
    Si tel n'est pas le cas, il le lui fait également savoir par un message puis affiche l'état du pendu.

### 2. Que demande le client ?

    Le client demande au serveur de vérifier sa proposition de lettre pour le mot à deviner.

### 3. Que répondra le serveur à la demande du client ?

    Si la proposition de lettre pour le mot à deviner est correcte, il le lui fait savoir par un message et dévoile ensuite la nouvelle lettre précédemment trouvée.
    Si tel n'est pas le cas, il le lui fait également savoir par un message puis affiche l'état du pendu.

### 4. Qu'est-ce qui est donc à la charge du serveur ?

    La logique du jeu est à la charge du serveur.
    C'est également à lui d'avertir le joueur de l'état de la fin de partie (victoire ou défaite).

### 5. Qui du serveur ou du client fixe les règles du jeu ?

    C'est le rôle du serveur de fixer les règles du jeu, du fait qu'il en contient la logique.

### 6. Définir la structure et les champs d'un message envoyé par le client

    Un message envoyé par le client est composé de ...

### 7. Définir la structure et les champs d'un message envoyé par le serveur

    Un message envoyé par le serveur est composé de ...

### 8. Élaborer la séquencement des messages échangés entre le client et le serveur

    ?

### 9. Définir précisément les diagrammes qui décrivent : une partie gagnée, perdue, abandonnée par le client

    ?

Les programmes demandés ont été rédigés dans le langage de programmation [C#](https://fr.wikipedia.org/wiki/C_sharp).

## **Partie 2 :** Extensions

- Le joueur propose le mot masqué et le serveur devine.
- Deux joueurs jouent le jeu en se connectant sur le même serveur.
- Le nombre de tentatives maximum varie en fonction de la difficulté du mot masqué ou le niveau du joueur.
- Un temps limite pour découvrir le mot masqué
- La possibilité de demander de jouer une nouvelle partie

## Modes de jeu :

### CLASSIQUE :

    L'ordinateur vous propose un mot, vous avez 6 essais pour le deviner.

### CLASSIQUE - CHRONO :

    La partie est chronométrée.

### PVP :

    Deux joueurs jouent l'un contre l'autre en se connectant au même serveur.
    L'un des deux joueurs propose un mot à l'autre et ce dernier tente de le deviner.

### PVP - SERVEUR :

    Deux joueurs s'affrontent et le mot à deviner est fixé par le serveur.
    À voir si le vainqueur est celui qui a trouvé le plus de lettres ou bien celui qui a deviné le mot entier le plus rapidement.
    (Victoire de l'un et défaite de l'autre)

### INVERSE :

    Le joueur propose un mot au serveur et ce dernier tente de le deviner.

### 2 VS 1 :

    Deux joueurs jouent contre le serveur en coopération.
    (Victoire des deux ou défaite des deux)
