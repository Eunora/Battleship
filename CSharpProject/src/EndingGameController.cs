
using Microsoft.VisualBasic;
using System;
using System.Collections;
using System.Collections.Generic;
//using System.Data;
using System.Diagnostics;
using SwinGameSDK;

<<<<<<< HEAD
/// <summary>
/// The EndingGameController is responsible for managing the interactions at the end
/// of a game.
/// </summary>

static class EndingGameController
{

	/// <summary>
	/// Draw the end of the game screen, shows the win/lose state
	/// </summary>
	public static void DrawEndOfGame()
	{
		UtilityFunctions.DrawField(GameController.ComputerPlayer.PlayerGrid, GameController.ComputerPlayer, true);
		UtilityFunctions.DrawSmallField(GameController.HumanPlayer.PlayerGrid, GameController.HumanPlayer);

		if (GameController.HumanPlayer.IsDestroyed) {
			SwinGame.DrawTextLines("YOU LOSE!", Color.White, Color.Transparent, GameResources.GameFont("ArialLarge"), FontAlignment.AlignCenter, 0, 250, SwinGame.ScreenWidth(), SwinGame.ScreenHeight());
		} else {
			SwinGame.DrawTextLines("-- WINNER --", Color.White, Color.Transparent, GameResources.GameFont("ArialLarge"), FontAlignment.AlignCenter, 0, 250, SwinGame.ScreenWidth(), SwinGame.ScreenHeight());
		}
	}

	/// <summary>
	/// Handle the input during the end of the game. Any interaction
	/// will result in it reading in the highsSwinGame.
	/// </summary>
	public static void HandleEndOfGameInput()
	{
		if (SwinGame.MouseClicked(MouseButton.LeftButton) || SwinGame.KeyTyped(KeyCode.vk_RETURN) || SwinGame.KeyTyped(KeyCode.vk_ESCAPE)) {
			HighScoreController.ReadHighScore(GameController.HumanPlayer.Score);
			GameController.EndCurrentState();
		}
	}

=======
/* summary, The EndingGameController is responsible for managing the
	interactions at the end of a game.
*/
static class EndingGameController
{

    // summary, Draw the end of the game screen, shows the win/lose state
    public static void DrawEndOfGame()
    {
        Rectangle toDraw;
        string whatShouldIPrint;

        DrawField(ComputerPlayer.PlayerGrid, ComputerPlayer, true);
        DrawSmallField(HumanPlayer.PlayerGrid, HumanPlayer);

        toDraw.X = 0;
        toDraw.Y = 250;
        toDraw.Width = SwinGame.ScreenWidth();
        toDraw.Height = SwinGame.ScreenHeight();

        if (HumanPlayer.IsDestroyed)
            whatShouldIPrint = "YOU LOSE!";
        else
            whatShouldIPrint = "-- WINNER --";

        SwinGame.DrawTextLines(whatShouldIPrint, Color.White, Color.Transparent, GameResources.GameFont("ArialLarge"), FontAlignment.AlignCenter, toDraw);
    }

    /* summary, Handle the input during the end of the game. Any interaction 
		will result in it reading in the highsSwinGame.
	*/
    public static void HandleEndOfGameInput()
    {
        if (SwinGame.MouseClicked(MouseButton.LeftButton) || SwinGame.KeyTyped(KeyCode.VK_RETURN) || SwinGame.KeyTyped(KeyCode.VK_ESCAPE))
        {
            ReadHighScore(HumanPlayer.Score);
            EndCurrentState();
        }
    }
>>>>>>> change-comment
}
