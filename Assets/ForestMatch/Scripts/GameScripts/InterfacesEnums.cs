namespace Mkey
{
    public enum GameMode { Play, Edit }
    public enum MatchBoardState { ShowEstimate, Fill, Collect, Waiting, Iddle}
    public enum SpawnerStyle { AllEnabled, AllEnabledAlign, DisabledAligned }
    public enum FillType {Step, Fast}
    public enum BombDir { Vertical, Horizontal, Radial, Dinamit, Color, Butterfly }
    public enum BombType { StaticMatch, DynamicMatch, DynamicClick }
    public enum HardMode { Easy, Hard }
}