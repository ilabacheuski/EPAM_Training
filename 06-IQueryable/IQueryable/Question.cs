namespace IQueryableTask
{

    public class Place
    {
        public string Woeid { get; set; }
        public string PlaceTypeName { get; set; }
        public string Name { get; set; }
        public string Country { get; set; }
        public string Postal { get; set; }
        public string Latitude { get; set; }
        public string Longitude { get; set; }
        public string Timezone { get; set; }
    }

    public class Question
    {
        public string Subject { get; set; }
        public string Content { get; set; }
        public string Category { get; set; }
        public QuestionType Type { get; set; }
        public string ChosenAnswer { get; set; }
    }

    public enum QuestionType
    {
        All = 0,
        Resolved,
        Open,
        Undecided
    }
}
