public enum GameState
{
    // The player is viewing the main menu.
    ViewingMainMenu,

    // The player is viewing the game menu
    ViewingGameMenu,

    // The player is looking at the high scores
    ViewingHighScores,

    // The player is altering the game settings
    AlteringSettings,
    
    // Players are deploying their ships
    Deploying,

    // Players are attempting to locate each others ships
    Discovering,

    // One player has won, showing the victory screen
    EndingGame,

    // The player has quit. Show ending credits and terminate the game
    Quitting
}

