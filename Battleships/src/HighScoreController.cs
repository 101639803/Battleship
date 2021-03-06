using System.IO;
using SwinGameSDK;
using System.Collections.Generic;
using System;
//Controls displaying and collecting high score dat
//Data is saved to a file
public static class HighScoreController
{
    private const int NAME_WIDTH = 3;
    private const int SCORES_LEFT = 490;

    //The score structure is used to keep the name and
    //score of the top players together.
    private struct Score : IComparable
    {
        public string Name;
        public int Value;

        //Allows scores to be compared to facilitate sorting
        //<param name="obj">the object to compare to</param>
        //<returns>a value that indicates the sort order</returns>
        public int CompareTo(object obj)
        {
            if (obj is Score)
            {
                Score other = ((Score)(obj));
                return (other.Value - Value);
            }
            else
            {
                return 0;
            }
        }
    }

    private static List<Score> _scores = new List<Score>();

    private static void LoadScores()
    {
        string filename = SwinGame.PathToResource("highscores.txt");
        StreamReader input = new StreamReader(filename);


        int numScores = Convert.ToInt32(input.ReadLine());

        _scores.Clear();
        for (int i = 1; (i <= numScores); i++)
        {
            string line = input.ReadLine();

            Score s;
            s.Name = line.Substring(0, NAME_WIDTH);
            s.Value = Convert.ToInt32(line.Substring(NAME_WIDTH));
            _scores.Add(s);
        }

        input.Close();
    }

    //Saves the scores back to the highscores text file.
    //The format is
    //# of scores
    //NNNSSS
    //
    //Where NNN is the name and SSS is the score
    //Note: TODO - fix the bug of saving highscores
    private static void SaveScores()
    {
        string filename = SwinGame.PathToResource("highscores.txt");

        StreamWriter output = new StreamWriter(filename);

        output.WriteLine(_scores.Count);
        foreach (Score s in _scores)
        {
            output.WriteLine((s.Name + s.Value));
        }

        output.Close();
    }

    //Draws the high scores to the screen.
    public static void DrawHighScores()
    {
        const int SCORES_HEADING = 40;
        const int SCORES_TOP = 80;
        const int SCORE_GAP = 30;

        if (_scores.Count == 0)
        {
            LoadScores();
        }

        SwinGame.DrawText("   High Scores   ", Color.White, GameResources.GameFont("Courier"), SCORES_LEFT, SCORES_HEADING);

        // For all of the scores:
        for (int i = 0; i <= _scores.Count - 1; i++)
        {
            Score s = _scores[i];

            // for scores 1 - 9 use 01 - 09
            if (i < 9)
            {
                SwinGame.DrawText((" " + ((i + 1) + (":   " + (s.Name + ("   " + s.Value))))), Color.White, GameResources.GameFont("Courier"), SCORES_LEFT, (SCORES_TOP + (i * SCORE_GAP)));
            }
            else
            {
                SwinGame.DrawText(((i + 1) + (":   " + (s.Name + ("   " + s.Value)))), Color.White, GameResources.GameFont("Courier"), SCORES_LEFT, (SCORES_TOP + (i * SCORE_GAP)));
            }
        }
    }

    //Handles the user input during the top score screen.
    public static void HandleHighScoreInput()
    {
        if (SwinGame.MouseClicked(MouseButton.LeftButton) || SwinGame.KeyTyped(KeyCode.EscapeKey) || SwinGame.KeyTyped(KeyCode.ReturnKey))
        {
            GameController.EndCurrentState();
        }
    }

    //Read the user's name for their highsSwinGame.
    //<param name="value">the player's sSwinGame.</param>
    //This verifies if the score is a highsSwinGame.
    public static void ReadHighScore(int value)
    {
        const int ENTRY_TOP = 500;

        if (_scores.Count == 0)
        {
            LoadScores();
        }

        // Is it a high score:
        if ((value > _scores[(_scores.Count - 1)].Value))
        {
            Score s = new Score();
            s.Value = value;

            GameController.AddNewState(GameState.ViewingHighScores);

            int x = (SCORES_LEFT + SwinGame.TextWidth(GameResources.GameFont("Courier"), "Name: "));
            SwinGame.StartReadingText(Color.White, NAME_WIDTH, GameResources.GameFont("Courier"), x, ENTRY_TOP);

            // Read the text from the user
            while (SwinGame.ReadingText())
            {
                SwinGame.ProcessEvents();
                UtilityFunctions.DrawBackground();
                HighScoreController.DrawHighScores();
                SwinGame.DrawText("Name: ", Color.White, GameResources.GameFont("Courier"), SCORES_LEFT, ENTRY_TOP);
                SwinGame.RefreshScreen();
            }

            s.Name = SwinGame.TextReadAsASCII();
            if ((s.Name.Length < 3))
            {
                s.Name = (s.Name + new string(' ', 3 - s.Name.Length));
            }

            _scores.RemoveAt((_scores.Count - 1));
            _scores.Add(s);
            _scores.Sort();
            SaveScores();
            GameController.EndCurrentState();
        }
    }

    //clear highscores in the stupidest way imaginable     
    public static void ClearHighScore()
    {
        string filename = SwinGame.PathToResource("fish.txt");
        StreamReader input = new StreamReader(filename);


        int numScores = Convert.ToInt32(input.ReadLine());

        _scores.Clear();
        for (int i = 1; (i <= numScores); i++)
        {
            string line = input.ReadLine();

            Score p;
            p.Name = line.Substring(0, NAME_WIDTH);
            p.Value = Convert.ToInt32(line.Substring(NAME_WIDTH));
            _scores.Add(p);
        }
        
        input.Close();
        SaveScores();
    }
}