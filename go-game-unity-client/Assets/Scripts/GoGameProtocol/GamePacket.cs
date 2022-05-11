namespace GoGameProtocol
{
    public class GamePck
    {
        public string PckName { get; set; }
        
    }

    public class HandShakePck : GamePck
    {
        public string SenderId { get; set; }

        public HandShakePck()
        {
            PckName = GamePckNames.HandShake;
        }
    }

    public class PlaceStonePck : GamePck
    {
        public int StoneType { get; set; }
        public int X { get; set; }
        public int Y { get; set; }

        public PlaceStonePck()
        {
            PckName = GamePckNames.PlaceStone;
        }
    }

    public static class GamePckNames
    {
        public const string HandShake = "HandShake";
        public const string PlaceStone = "PlaceStone";
    }
}