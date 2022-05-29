namespace Gameplay
{
    /// <summary>
    /// Holder for all event type defines
    /// </summary>
    public static class EventTypes
    {
        public const string SESSION_P = "SESSION_";
        
        public const string SESSION_STARTED = SESSION_P + "STARTED";
        public const string SESSION_ENDED = SESSION_P + "ENDED";

        public const string GAME_P = "GAME_";
        public const string GAME_STARTED = GAME_P + "STARTED";
        
        public const string QUESTION_P = "QUESTION_";
        
        public const string QUESTION_ANSWERED = QUESTION_P + "ANSWERED";
    }
}