using SwinGameSDK;
using System.Collections.Generic;

//This includes a number of utility methods for
//drawing and interacting with the Mouse.
public static class UtilityFunctions
{

    public const int FIELD_TOP = 122;
    public const int FIELD_LEFT = 349;
    public const int FIELD_WIDTH = 418;
    public const int FIELD_HEIGHT = 418;
    public const int MESSAGE_TOP = 548;
    public const int CELL_WIDTH = 40;
    public const int CELL_HEIGHT = 40;
    public const int CELL_GAP = 2;
    public const int SHIP_GAP = 3;
    private readonly static Color SMALL_SEA = SwinGame.RGBAColor(6, 60, 94, 255);
    //private readonly static Color SMALL_SHIP = Color.Gray;
    private readonly static Color SMALL_MISS = SwinGame.RGBAColor(1, 147, 220, 255);
    private readonly static Color SMALL_HIT = SwinGame.RGBAColor(169, 24, 37, 255);
    //private readonly static Color LARGE_SEA = SwinGame.RGBAColor(6, 60, 94, 255);
    //private readonly static Color LARGE_SHIP = Color.Gray;
    private readonly static Color LARGE_MISS = SwinGame.RGBAColor(1, 147, 220, 255);
    private readonly static Color LARGE_HIT = SwinGame.RGBAColor(252, 2, 3, 255);
    private readonly static Color OUTLINE_COLOR = SwinGame.RGBAColor(5, 55, 88, 255);
    private readonly static Color SHIP_FILL_COLOR = Color.Gray;
    private readonly static Color SHIP_OUTLINE_COLOR = Color.White;
    private readonly static Color MESSAGE_COLOR = SwinGame.RGBAColor(2, 167, 252, 255);
    public const int ANIMATION_CELLS = 7;
    public const int FRAMES_PER_CELL = 8;

    //Determines if the mouse is in a given rectangle.
    //<param name="x">the x location to check</param>
    //<param name="y">the y location to check</param>
    //<param name="w">the width to check</param>
    //<param name="h">the height to check</param>
    //<returns>true if the mouse is in the area checked</returns>
    public static bool IsMouseInRectangle(int x, int y, int w, int h)
    {
        Point2D mouse = SwinGame.MousePosition();
        bool result = false;
        GameResources.MouseIcon();
        //Note: check horizontal position
        if (((mouse.X >= x) && (mouse.X <= (x + w))))
        {
            //Note: Check vertical position
            if (((mouse.Y >= y) && (mouse.Y <= (y + h))))
            {
                result = true;
            }
        }

        return result;
    }


    //Draws a large field using the grid and the indicated player's ships.
    //<param name="grid">the grid to draw</param>
    //<param name="thePlayer">the players ships to show</param>
    //<param name="showShips">indicates if the ships should be shown</param>
    public static void DrawField(ISeaGrid grid, Player thePlayer, bool showShips)
    {
        DrawCustomField(grid, thePlayer, false, showShips, FIELD_LEFT, FIELD_TOP, FIELD_WIDTH, FIELD_HEIGHT, CELL_WIDTH, CELL_HEIGHT, CELL_GAP);
    }

    //Draws a small field, showing the attacks made and the locations of the player's ships
    //<param name="grid">the grid to show</param>
    //<param name="thePlayer">the player to show the ships of</param>
    public static void DrawSmallField(ISeaGrid grid, Player thePlayer)
    {
        const int SMALL_FIELD_TOP = 373;
        const int SMALL_FIELD_LEFT = 39;
        const int SMALL_FIELD_HEIGHT = 166;
        const int SMALL_FIELD_WIDTH = 166;
        const int SMALL_FIELD_CELL_HEIGHT = 13;
        const int SMALL_FIELD_CELL_WIDTH = 13;
        const int SMALL_FIELD_CELL_GAP = 4;

        DrawCustomField(grid, thePlayer, true, true, SMALL_FIELD_LEFT, SMALL_FIELD_TOP, SMALL_FIELD_WIDTH, SMALL_FIELD_HEIGHT, SMALL_FIELD_CELL_WIDTH, SMALL_FIELD_CELL_HEIGHT, SMALL_FIELD_CELL_GAP);
    }

    //Draws the player's grid and ships.
    //<param name="grid">the grid to show</param>
    //<param name="thePlayer">the player to show the ships of</param>
    //<param name="small">true if the small grid is shown</param>
    //<param name="showShips">true if ships are to be shown</param>
    //<param name="left">the left side of the grid</param>
    //<param name="top">the top of the grid</param>
    //<param name="width">the width of the grid</param>
    //<param name="height">the height of the grid</param>
    //<param name="cellWidth">the width of each cell</param>
    //<param name="cellHeight">the height of each cell</param>
    //<param name="cellGap">the gap between the cells</param>
    private static void DrawCustomField(ISeaGrid grid, Player thePlayer, bool small, bool showShips, int left, int top, int width, int height, int cellWidth, int cellHeight, int cellGap)
    {
        //Note: not use this value
        // SwinGame.FillRectangle(Color.Blue, left, top, width, height)
        int rowTop;
        int colLeft;

        //Note: Draw the grid
        for (int row = 0; row <= 9; row++)
        {
            rowTop = (top + ((cellGap + cellHeight) * row));
            for (int col = 0; col <= 9; col++)
            {
                colLeft = (left + ((cellGap + cellWidth) * col));
                Color fillColor = SMALL_MISS;
                bool draw = true;
                switch (grid[row, col])
                {
                    case TileView.Ship:
                        draw = false;
                        break;
                    case TileView.Miss:
                        if (small)
                        {
                            fillColor = SMALL_MISS;
                        }
                        else
                        {
                            fillColor = LARGE_MISS;
                        }

                        break;
                    case TileView.Hit:
                        if (small)
                        {
                            fillColor = SMALL_HIT;
                        }
                        else
                        {
                            fillColor = LARGE_HIT;
                        }

                        break;
                    case TileView.Sea:
                        if (small)
                        {
                            fillColor = SMALL_SEA;
                        }
                        else
                        {
                            draw = false;
                        }

                        break;
                }
                if (draw)
                {
                    SwinGame.FillRectangle(fillColor, colLeft, rowTop, cellWidth, cellHeight);
                    if (!small)
                    {
                        SwinGame.DrawRectangle(OUTLINE_COLOR, colLeft, rowTop, cellWidth, cellHeight);
                    }

                }

            }

        }

        if (!showShips)
        {
            return;
        }

        int shipHeight;
        int shipWidth;
        string shipName;

        //Note: Draw the ships

		foreach (Ship s in thePlayer)
		{
			if (s == null || !s.IsDeployed)
			{
				continue;
			}

			rowTop = (top + (((cellGap + cellHeight) * s.Row) + SHIP_GAP));
			colLeft = (left + (((cellGap + cellWidth) * s.Column) + SHIP_GAP));
			if ((s.Direction == Direction.LeftRight))
			{
				shipName = ("ShipLR" + s.Size);
				shipHeight = (cellHeight - (SHIP_GAP * 2));
				shipWidth = (((cellWidth + cellGap) * s.Size) - ((SHIP_GAP * 2) - cellGap));
			}
			else
			{
				// Up down
				shipName = ("ShipUD" + s.Size);
				shipHeight = (((cellHeight + cellGap) * s.Size) - ((SHIP_GAP * 2) - cellGap));
				shipWidth = (cellWidth - (SHIP_GAP * 2));
			}

			if (!small)
			{
				SwinGame.DrawBitmap(GameResources.GameImage(shipName), colLeft, rowTop);
			}
			else
			{
				SwinGame.FillRectangle(SHIP_FILL_COLOR, colLeft, rowTop, shipWidth, shipHeight);
				SwinGame.DrawRectangle(SHIP_OUTLINE_COLOR, colLeft, rowTop, shipWidth, shipHeight);
			}

		}

	}

	private static string _message;

	//The message to display
	//<value>The message to display</value>
	//<returns>The message to display</returns>
	public static string Message
	{
		get
		{
			return _message;
		}
		set
		{
			_message = value;
		}
	}

	public static void DrawMessage()
	{
		SwinGame.DrawText(Message, MESSAGE_COLOR, GameResources.GameFont("Courier"), FIELD_LEFT, MESSAGE_TOP);
	}

	//Draws the background for the current state of the game
	public static void DrawBackground()
	{
		switch (GameController.CurrentState)
		{
			case GameState.ViewingMainMenu:
			case GameState.ViewingGameMenu:
			case GameState.AlteringSettings:
			case GameState.ViewingHighScores:
				SwinGame.DrawBitmap(GameResources.GameImage("Menu"), 0, 0);
				break;
			case GameState.Discovering:
			case GameState.EndingGame:
				SwinGame.DrawBitmap(GameResources.GameImage("Discovery"), 0, 0);
				break;
			case GameState.Deploying:
				SwinGame.DrawBitmap(GameResources.GameImage("Deploy"), 0, 0);
				break;
            case GameState.Instructions:
                SwinGame.DrawBitmap(GameResources.GameImage("InstructionsPage"), 0, 0);
                SwinGame.DrawText("--Instructions for Battleship--", Color.Blue, GameResources.GameFont("Courier"), 250, 10);

                SwinGame.DrawText("--BASIC STUFF--", Color.Yellow, GameResources.GameFont("Courier"), 100, 40);
                SwinGame.DrawText("--DIFFICULTY--", Color.Yellow, GameResources.GameFont("Courier"), 100, 60);
                SwinGame.DrawText("Choose between 3 Difficulties; Easy, Medium and Hard", Color.White, GameResources.GameFont("Courier"), 100, 80);

                SwinGame.DrawText("--HIGH SCORES--", Color.Yellow, GameResources.GameFont("Courier"), 100, 100);
                SwinGame.DrawText("Click on the scores button to view the Player's high score", Color.White, GameResources.GameFont("Courier"), 100, 120);

                SwinGame.DrawText("--AIM OF THE GAME--", Color.Yellow, GameResources.GameFont("Courier"), 100, 140);
                SwinGame.DrawText("Destroy all the opponent's ships in order to win", Color.White, GameResources.GameFont("Courier"), 100, 160);

                SwinGame.DrawText("--SCORING--", Color.Yellow, GameResources.GameFont("Courier"), 100, 190);
                SwinGame.DrawText("Each time the player hits the opponent's ship, 1 point will be given", Color.White, GameResources.GameFont("Courier"), 100, 210);

                SwinGame.DrawText("--GAME INSTRUCTIONS--", Color.Yellow, GameResources.GameFont("Courier"), 100, 300);
                SwinGame.DrawText("1. During Ship deployment stage, you can either place the ship on the grid", Color.White, GameResources.GameFont("Courier"), 100, 320);
                SwinGame.DrawText("horizontally or vertically. Or you can select the random shuffle which deploys", Color.White, GameResources.GameFont("Courier"), 100, 340);
                SwinGame.DrawText("all the ship on the grid for you. Note: All ship must be deployed before", Color.White, GameResources.GameFont("Courier"), 100, 360);
                SwinGame.DrawText("starting the game.", Color.White, GameResources.GameFont("Courier"), 100, 380);
                SwinGame.DrawText("2.During the attack stage, you will just need to Click on the grid", Color.White, GameResources.GameFont("Courier"), 100, 430);
                SwinGame.DrawText("in order to attack enemy's ship.", Color.White, GameResources.GameFont("Courier"), 100, 450);
                break;
            default:
				SwinGame.ClearScreen();
				break;
		}
		SwinGame.DrawFramerate(675, 585);
	}

	public static void AddExplosion(int row, int col)
	{
		AddAnimation(row, col, "Splash");
	}

	public static void AddSplash(int row, int col)
	{
		AddAnimation(row, col, "Splash");
	}

	private static List<Sprite> _animations = new List<Sprite>();

	private static void AddAnimation(int row, int col, string image)
	{
		Bitmap imgObj = GameResources.GameImage(image);
		imgObj.SetCellDetails(40, 40, 3, 3, 7);

		AnimationScript animation = SwinGame.LoadAnimationScript("splash.txt");

		Sprite s = SwinGame.CreateSprite(imgObj, animation);
		s.X = (FIELD_LEFT + (col * (CELL_WIDTH + CELL_GAP)));
		s.Y = (FIELD_TOP + (row * (CELL_HEIGHT + CELL_GAP)));
		s.StartAnimation("splash");

		_animations.Add(s);
	}

	public static void UpdateAnimations()
	{
		List<Sprite> ended = new List<Sprite>();
		foreach (Sprite s in _animations)
		{
			SwinGame.UpdateSprite(s);
			if (s.AnimationHasEnded)
			{
				ended.Add(s);
			}
		}

		foreach (Sprite s in ended)
		{
			_animations.Remove(s);
			SwinGame.FreeSprite(s);
		}
	}

	public static void DrawAnimations()
	{
		foreach (Sprite s in _animations)
		{
			SwinGame.DrawSprite(s);
		}
	}

	public static void DrawAnimationSequence()
	{
		for (int i = 1; (i <= (ANIMATION_CELLS * FRAMES_PER_CELL)); i++)
		{
			UpdateAnimations();
			GameController.DrawScreen();
		}
	}
}