namespace ReadWeeklyTPGD
{
    public class OneWeekResults
    {
        public int Week { get; set; }
        public List<Result> Results { get; set; }

        public OneWeekResults(int week, List<Result> results)
        {
            Week = week;
            Results = results;
        }
    }

    public class Result
    {
        public string? Location { get; set; }
        public string? OppName { get; set; }
        public List<int> MyScores { get; set; }
        public List<int> OppScores { get; set; }

        //public Result(string? location, string? oppName, int?[] myScores, int?[] oppScores)
        public Result(string? location, string? oppName, List<int> myScores, List<int> oppScores)
        {
            Location = location;
            OppName = oppName;
            MyScores = myScores;
            OppScores = oppScores;
        }
    }

    public class WeekRankings
    {
        public List<WeekRanking> WeekRankingList { get; set; }

        public WeekRankings(List<WeekRanking> weekRankingList)
        {
            WeekRankingList = weekRankingList;
        }
    }

    public class WeekRanking
    {
        public int x { get; set; }
        public int y { get; set; }

        public WeekRanking(int X, int Y)
        {
            x = X;
            y = Y;
        }
    }

}
