
using Microsoft.VisualBasic;
using System;
using System.Collections;
using System.Collections.Generic;
//using System.Data;
using System.Diagnostics;
using SwinGameSDK;

/*The GameController is responsible for controlling the game, 
	managing user input, and displaying the current state of the game.
*/
public static class GameController
{

	private static BattleShipsGame _theGame;
	private static Player _human;

	private static AIPlayer _ai;

	private static Stack<GameState> _state = new Stack<GameState>();

	private static AIOption _aiSetting;
	/* Summary, returns the current state of the game, indicating which 
		screen is currently being used
	*/
	// value & returns, The current state
	public static GameState CurrentState {
		get { return _state.Peek(); }
	}

    // Summary, returns the human player.
    // value & returns, the human player
	public static Player HumanPlayer {
		get { return _human; }
	}

    // Summary, Returns the computer player.
    // value & returns, the conputer player
	public static Player ComputerPlayer {
		get { return _ai; }
	}

	static GameController()
	{
		//bottom state will be quitting. If player exits main menu then the game is over
		_state.Push(GameState.Quitting);

		//at the start the player is viewing the main menu
		_state.Push(GameState.ViewingMainMenu);
	}

    // Summary, Starts a new game.
    // remarks, Creates an AI player based upon the _aiSetting.
	public static void StartGame()
	{
		if (_theGame != null)
			EndGame();

		//Create the game
		_theGame = new BattleShipsGame();

		//create the players
		switch (_aiSetting) {
			case AIOption.Medium:
				_ai = new AIMediumPlayer(_theGame);
				break;
			case AIOption.Hard:
				_ai = new AIHardPlayer(_theGame);
				break;
			default:
				_ai = new AIEasyPlayer(_theGame);
				break;
		}

		_human = new Player(_theGame);

		//AddHandler _human.PlayerGrid.Changed, AddressOf GridChanged
		_ai.PlayerGrid.Changed += GridChanged;
		_theGame.AttackCompleted += AttackCompleted;

		AddNewState(GameState.Deploying);
	}

    // summary, Stops listening to the old game once a new game is started

	private static void EndGame()
	{
		//RemoveHandler _human.PlayerGrid.Changed, AddressOf GridChanged
		_ai.PlayerGrid.Changed -= GridChanged;
		_theGame.AttackCompleted -= AttackCompleted;
	}

    /* Summary, Listens to the game grids for any changes and redraws the screen
       when the grids change*/
    // <param name="sender">the grid that changed</param>
    // <param name="args">not used</param>
	private static void GridChanged(object sender, EventArgs args)
	{
		DrawScreen();
		SwinGame.RefreshScreen();
	}
	
	// Summary, add an explosion animation on specific cell when showAnimation is true and will play "Hit" sound effect
   
	private static void PlayHitSequence(int row, int column, bool showAnimation)
	{
		if (showAnimation) {
			UtilityFunctions.AddExplosion(row, column);
		}

		Audio.PlaySoundEffect(GameResources.GameSound("Hit"));

		UtilityFunctions.DrawAnimationSequence();
	}
	
	// Summary, add a water splash animation on specific cell when showAnimation is true and will play "water splash" sound effect
	private static void PlayMissSequence(int row, int column, bool showAnimation)
	{
		if (showAnimation) {
			UtilityFunctions.AddSplash(row, column);
		}

		Audio.PlaySoundEffect(GameResources.GameSound("Miss"));

		UtilityFunctions.DrawAnimationSequence();
	}

	// Summary, Listens for attacks to be completed.
    // <param name="sender">the game</param>
    // <param name="result">the result of the attack</param>
    // remarks, Displays a message, plays sound and redraws the screen
	private static void AttackCompleted(object sender, AttackResult result)
	{
		bool isHuman = false;
		isHuman = object.ReferenceEquals(_theGame.Player, HumanPlayer);

		if (isHuman) {
			UtilityFunctions.Message = "You " + result.ToString();
		} else {
			UtilityFunctions.Message = "The AI " + result.ToString();
		}

		switch (result.Value) {
			case ResultOfAttack.Destroyed:
				PlayHitSequence(result.Row, result.Column, isHuman);
				Audio.PlaySoundEffect(GameResources.GameSound("Sink"));

				break;
			case ResultOfAttack.GameOver:
				PlayHitSequence(result.Row, result.Column, isHuman);
				Audio.PlaySoundEffect(GameResources.GameSound("Sink"));

				while (Audio.SoundEffectPlaying(GameResources.GameSound("Sink"))) {
					SwinGame.Delay(10);
					SwinGame.RefreshScreen();
				}

				if (HumanPlayer.IsDestroyed) {
					Audio.PlaySoundEffect(GameResources.GameSound("Lose"));
				} else {
					Audio.PlaySoundEffect(GameResources.GameSound("Winner"));
				}

				break;
			case ResultOfAttack.Hit:
				PlayHitSequence(result.Row, result.Column, isHuman);
				break;
			case ResultOfAttack.Miss:
				PlayMissSequence(result.Row, result.Column, isHuman);
				break;
			case ResultOfAttack.ShotAlready:
				Audio.PlaySoundEffect(GameResources.GameSound("Error"));
				break;
		}
	}

	/* summary, Completes the deployment phase of the game and
       switches to the battle mode (Discovering state)
	*/   
    // remarks, This adds the players to the game before switching state.
	public static void EndDeployment()
	{
		//deploy the players
		_theGame.AddDeployedPlayer(_human);
		_theGame.AddDeployedPlayer(_ai);

		SwitchState(GameState.Discovering);
	}

	// summary, Gets the player to attack the indicated row and column.
    // <param name="row">the row to attack</param>
    // <param name="col">the column to attack</param>
    // remarks, Checks the attack result once the attack is complete
	public static void Attack(int row, int col)
	{
		AttackResult result = default(AttackResult);
		result = _theGame.Shoot(row, col);
		CheckAttackResult(result);
	}

	// summary, Gets the AI to attack.
    // remarks, Checks the attack result once the attack is complete.
	private static void AIAttack()
	{
		AttackResult result = default(AttackResult);
		result = _theGame.Player.Attack();
		CheckAttackResult(result);
	}

	/*summary,Checks the results of the attack and switches to Ending 
		the Game if the result was game over.
	*/
    // <param name="result">the result of the last attack</param>
    // remarks,Gets the AI to attack if the result switched to the AI player.
	private static void CheckAttackResult(AttackResult result)
	{
		switch (result.Value) {
			case ResultOfAttack.Miss:
				if (object.ReferenceEquals(_theGame.Player, ComputerPlayer))
					AIAttack();
				break;
			case ResultOfAttack.GameOver:
				SwitchState(GameState.EndingGame);
				break;
		}
	}

	// summary, Handles the user SwinGame.
    /* remarks, Reads key and mouse input and converts these into
		actions for the game to perform. The actions performed depend 
		upon the state of the game.
	*/
    
	public static void HandleUserInput()
	{
		//Read incoming input events
		SwinGame.ProcessEvents();

		switch (CurrentState) {
			case GameState.ViewingMainMenu:
				MenuController.HandleMainMenuInput();
				break;
			case GameState.ViewingGameMenu:
				MenuController.HandleGameMenuInput();
				break;
			case GameState.AlteringSettings:
				MenuController.HandleSetupMenuInput();
				break;
			case GameState.Deploying:
				DeploymentController.HandleDeploymentInput();
				break;
			case GameState.Discovering:
				DiscoveryController.HandleDiscoveryInput();
				break;
			case GameState.EndingGame:
				EndingGameController.HandleEndOfGameInput();
				break;
			case GameState.ViewingHighScores:
				HighScoreController.HandleHighScoreInput();
				break;
		}

		UtilityFunctions.UpdateAnimations();
	}

	// summary, Draws the current state of the game to the screen.
    // remarks, What is drawn depends upon the state of the game.
	public static void DrawScreen()
	{
		UtilityFunctions.DrawBackground();

		switch (CurrentState) {
			case GameState.ViewingMainMenu:
				MenuController.DrawMainMenu();
				break;
			case GameState.ViewingGameMenu:
				MenuController.DrawGameMenu();
				break;
			case GameState.AlteringSettings:
				MenuController.DrawSettings();
				break;
			case GameState.Deploying:
				DeploymentController.DrawDeployment();
				break;
			case GameState.Discovering:
				DiscoveryController.DrawDiscovery();
				break;
			case GameState.EndingGame:
				EndingGameController.DrawEndOfGame();
				break;
			case GameState.ViewingHighScores:
				HighScoreController.DrawHighScores();
				break;
		}

		UtilityFunctions.DrawAnimations();

		SwinGame.RefreshScreen();
	}

	/* summary, Move the game to a new state. The current state is maintained
		so that it can be returned to.
	 */
    // <param name="state">the new game state</param>
	public static void AddNewState(GameState state)
	{
		_state.Push(state);
		UtilityFunctions.Message = "";
	}

    // summary, End the current state and add in the new state.
    // <param name="newState">the new state of the game</param>
	public static void SwitchState(GameState newState)
	{
		EndCurrentState();
		AddNewState(newState);
	}

    // summary, Ends the current state, returning to the prior state
	public static void EndCurrentState()
	{
		_state.Pop();
	}

    // summary, Sets the difficulty for the next level of the game.
    // <param name="setting">the new difficulty level</param>
	public static void SetDifficulty(AIOption setting)
	{
		_aiSetting = setting;
	}

}