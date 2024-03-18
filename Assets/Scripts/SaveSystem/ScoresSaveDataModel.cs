/// <summary>
/// 
///     Data Model for saving high scores
/// 
/// </summary>
public class ScoresSaveDataModel
{
    public int Level1Score { get; set; }
    public int Level2Score { get; set; }
    public int Level3Score { get; set; }
    public int Level4Score { get; set; }
    public int Level5Score { get; set; }

    public ScoresSaveDataModel()
    {
        Level1Score = 0;
        Level2Score = 0;
        Level3Score = 0;
        Level4Score = 0;
        Level5Score = 0;
    }

    public ScoresSaveDataModel(int l1, int l2, int l3, int l4, int l5)
    {
        Level1Score = l1;
        Level2Score = l2;
        Level3Score = l3;
        Level4Score = l4;
        Level5Score = l5;
    }
    
}
