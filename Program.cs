//using Microsoft.Office.Interop.Excel;
using ExcelDataReader;
using System.Data;
using System.Text;
using System.Text.Json;

namespace ReadWeeklyTPGD
{
    internal class Program
    {
        static void Main()
        {
            // excel document
            string xlFolder = @"E:\Users\Mark\Documents\Personal\PB\Tournament Players Drill Group\Spreadsheets\";
            string xlFilePath = xlFolder + @"TPG Drillers.xlsm";

            Console.WriteLine("Current Dir: " + Environment.CurrentDirectory);

            DirectoryInfo? di = new FileInfo(System.Reflection.Assembly.GetExecutingAssembly().Location)?.Directory;
            Console.WriteLine("Code Dir: " + di?.FullName ?? "???");

            // Week number
            Console.Write("Week Number: ");
            int week = Convert.ToInt32(Console.ReadLine());
            Console.WriteLine();

            // Create the 4 html pages:
            // - Dinking Rankings
            // - Dinking Results
            // - Mini Singles Rankings
            // - Mini Singles Results

            // Create and save Dinking Rankings Latest page
            Console.Write("Creating Dinking latest rankings html...");
            WriteRankingsLatestHtmlFile("Dinking", BuildRankingsLatestPage("Dinking", xlFilePath, week).ToString());
            Console.WriteLine("done");

            // Create and save Mini Singles Rankings Latest page
            Console.Write("Creating Mini Singles latest rankings html...");
            WriteRankingsLatestHtmlFile("Mini Singles", BuildRankingsLatestPage("Mini Singles", xlFilePath, week).ToString());
            Console.WriteLine("done");

            // Create and save Dinking Results Latest page
            Console.Write("Creating Dinking latest results html...");
            WriteResultsLatestHtmlFile("Dinking", BuildResultsLatestPage("Dinking", xlFilePath, week).ToString());
            Console.WriteLine("done");

            // Create and save Mini Singles Results Latest page
            Console.Write("Creating Mini Singles latest results html...");
            WriteResultsLatestHtmlFile("Mini Singles", BuildResultsLatestPage("Mini Singles", xlFilePath, week).ToString());
            Console.WriteLine("done");


            // Append new data to the 4 json files:
            // - Dinking Rankings
            // - Dinking Results
            // - Mini Singles Rankings
            // - Mini Singles Results

            // Add latest dinking rankings to json data file
            Console.Write("Updating Dinking historic rankings json...");
            WriteRankingsHistoryJsonFile("Dinking", UpdateRankingsHistoryJsonFile("Dinking", xlFilePath, week).ToString());
            Console.WriteLine("done");

            // Add latest Mini Singles rankings to json data file
            Console.Write("Updating Mini Singles historic rankings json...");
            WriteRankingsHistoryJsonFile("Mini Singles", UpdateRankingsHistoryJsonFile("Mini Singles", xlFilePath, week).ToString());
            Console.WriteLine("done");

            // Create and save Dinking Results Latest page
            Console.Write("Updating Dinking historic results json...");
            WriteResultsHistoryJsonFile("Dinking", UpdateResultsHistoryJsonFile("Dinking", xlFilePath, week).ToString());
            Console.WriteLine("done");

            // Create and save Mini Singles Results Latest page
            Console.Write("Updating Mini Singles historic results json...");
            WriteResultsHistoryJsonFile("Mini Singles", UpdateResultsHistoryJsonFile("Mini Singles", xlFilePath, week).ToString());
            Console.WriteLine("done");

            Console.WriteLine();
            Console.Write("Press 'Enter' key to finish");
            Console.ReadLine();
        }

        private static void WriteRankingsLatestHtmlFile(string pageType, string pageContent)
        {
            var fullFilename = GetRootDir() + @"\" + pageType.Replace(" ", "") + "RankingsLatest.html";

            if (fullFilename != null)
            {
                File.WriteAllText(fullFilename, pageContent);
            }
        }

        private static StringBuilder BuildRankingsLatestPage(string pageType, string xlFilePath, int week)
        {
            var sb = new StringBuilder(RankingsHeading("(Week " + week.ToString() + ") " + pageType));

            Encoding.RegisterProvider(CodePagesEncodingProvider.Instance); //System.Text.
            using var stream = File.Open(xlFilePath, FileMode.Open, FileAccess.Read);
            using (var reader = ExcelReaderFactory.CreateReader(stream))
            {
                var result = reader.AsDataSet();

                // Get Dinking or Mini Singles table
                // DataTable table = pageType == "Dinking" ? result.Tables[0] : result.Tables[1];
                var table = result.Tables[pageType];

                string?[] results = new string?[13];

                DataRow row;
                for (int i = 1; i < table?.Rows.Count; i++)
                {
                    row = table.Rows[i];
                    results[0] = row[0]?.ToString();    // Rank
                    results[1] = row[1]?.ToString();    // Name
                    results[2] = Convert.ToInt32(row[3]).ToString();    // Total

                    for (int col = 3; col < 13; col++)
                    {
                        var score = row[col + 4];
                        results[col] = Convert.ToInt32(score).ToString();
                    }

                    sb.Append(RankingsDivs(results));

                }

                sb.Append(RankingsFooting());
            }
            //}
            return sb;
        }

        private static string RankingsHeading(string title)
        {
            string heading = """
            <!DOCTYPE html>
            <html lang="en">
            <head>
                <meta charset="UTF-8">
                <title>TPG Drillers - {title}</title>
                <meta name="viewport" content="width=device-width, initial-scale=1">
                <meta http-equiv="cache-control" content="no-cache, must-revalidate, post-check=0, pre-check=0, max-age=0" />
                <meta http-equiv="expires" content="Tue, 01 Jan 1980 1:00:00 GMT" />
                <meta http-equiv="pragma" content="no-cache" />

                <style>
                    .title {
                        display: flex;
                        justify-content: left;
                        margin: 8px 8px 8px 2px;
                        width: 100%;
                        font-size: 22px;
                        font-weight: bold;
                    }

                    .header-row {
                        display: grid;
                        grid-template-columns: 80px 210px 100px repeat(10,60px);
                        font-size: 22px;
                        font-weight: bolder;
                    }

                    .header-row > div {
                        text-align: center;
                        padding: 5px 0px 0px 2px; 
                        border: 1px black solid;
                    }

                    .results {
                        display: grid;
                        grid-template-columns: 80px 210px 100px repeat(10,60px);
                    }
                    .results > div {
                        text-align: center;
                        font-size: 18px;
                        border: 1px black solid;
                    }

                    .results > div:nth-child(26n+1), 
                    .results > div:nth-child(26n+2),
                    .results > div:nth-child(26n+3) /*,
                    .results > div:nth-child(26n+4), 
                    .results > div:nth-child(26n+5),
                    .results > div:nth-child(26n+6),
                    .results > div:nth-child(26n+7), 
                    .results > div:nth-child(26n+8),
                    .results > div:nth-child(26n+9),
                    .results > div:nth-child(26n+10), 
                    .results > div:nth-child(26n+11),
                    .results > div:nth-child(26n+12),
                    .results > div:nth-child(26n+13) */ {
                        background-color: rgb(221,235, 247);
                    }

                </style>
            </head>
            <body>
                <div class="title">Latest {title} Rankings</div>

                <div class="header-row">
                    <div>Rank</div><div>Name</div><div>Total</div><div>1</div><div>2</div><div>3</div><div>4</div><div>5</div><div>6</div><div>7</div><div>8</div><div>9</div><div>10</div>
                </div>

                <div class="results">

            """;

            return heading.Replace("{title}", title);

        }

        private static string RankingsFooting()
        {
            return """
                </div>
            </body>
            </html>
            """;
        }

        private static string RankingsDivs(string?[] r)
        {
            return $"\t\t<div>{r[0]}</div><div>{r[1]}</div><div>{r[2]}</div><div>{r[3]}</div><div>{r[4]}</div><div>{r[5]}</div><div>{r[6]}</div><div>{r[7]}</div><div>{r[8]}</div><div>{r[9]}</div><div>{r[10]}</div><div>{r[11]}</div><div>{r[12]}</div>\n";
        }

        private static void WriteResultsLatestHtmlFile(string pageType, string pageContent)
        {
            var fullFilename = GetRootDir() + @"\" + pageType.Replace(" ", "") + "ResultsLatest.html";

            if (fullFilename != null)
            {
                File.WriteAllText(fullFilename, pageContent);
            }
        }

        private static StringBuilder BuildResultsLatestPage(string pageType, string xlFilePath, int week)
        {
            const int dinkingFirstRow = 2;
            const int miniSinglesFirstRow = 6;
            const int hMatchStep = 7;
            const int vMatchStep = 10;

            DataRow? row;
            int rowIndex;
            string line;

            var sb = new StringBuilder(ResultsHeading("(Week " + week.ToString() + ") " + pageType));

            Encoding.RegisterProvider(CodePagesEncodingProvider.Instance); //System.Text.
            using var stream = File.Open(xlFilePath, FileMode.Open, FileAccess.Read);
            //using (var stream = File.Open(xlFilePath, FileMode.Open, FileAccess.Read))
            //{
            using (var reader = ExcelReaderFactory.CreateReader(stream))
            {
                var result = reader.AsDataSet();

                for (int sessionNumber = 1; sessionNumber < 7; sessionNumber++)
                {
                    var table = result.Tables["Session " + sessionNumber];

                    // Check if this session took place and continue to next if it didn't
                    row = table?.Rows[0];
                    if (row?[2] is DBNull || row?[2].ToString() == "") { continue; }

                    sb.AppendLine($"<div class='session'>Session {sessionNumber}: {row?[2]}</div>");

                    // Use pageType to determine where data is located
                    if (pageType == "Dinking")
                    {
                        // Matches 1, 2 and 3
                        for (int i = 0; i < 3; i++)
                        {
                            rowIndex = dinkingFirstRow + i * vMatchStep;
                            row = table?.Rows[rowIndex];

                            // Skip this match if first score is not there
                            if (row?[2] is DBNull) { continue; }

                            // First player
                            line = String.Format("<div class='score-row'><div class='player-name'>{0}</div><div class='score'>{1}</div><div class='score'>{2}</div><div class='score'>{3}</div></div>", RemoveRanking(row?[0].ToString()), row?[2], row?[3], row?[4]);
                            sb.AppendLine(line);

                            // Second player
                            row = table?.Rows[rowIndex + 1];
                            line = String.Format("<div class='score-row'><div class='player-name player-name2'>{0}</div><div class='score score2'>{1}</div><div class='score score2'>{2}</div><div class='score score2'>{3}</div></div><br>", RemoveRanking(row?[0].ToString()), row?[2], row?[3], row?[4]);
                            sb.AppendLine(line);
                        }

                        // Matches 4, 5 and 6
                        for (int i = 0; i < 3; i++)
                        {
                            rowIndex = dinkingFirstRow + i * vMatchStep;
                            row = table?.Rows[rowIndex];

                            // Skip this match if first score is not there
                            if (row?[2 + hMatchStep] is DBNull) { continue; }

                            // First player
                            line = String.Format("<div class='score-row'><div class='player-name'>{0}</div><div class='score'>{1}</div><div class='score'>{2}</div><div class='score'>{3}</div></div>", RemoveRanking(row?[0 + hMatchStep].ToString()), row?[2 + hMatchStep], row?[3 + hMatchStep], row?[4 + hMatchStep]);
                            sb.AppendLine(line);

                            // Second player
                            row = table?.Rows[rowIndex + 1];
                            line = String.Format("<div class='score-row'><div class='player-name player-name2'>{0}</div><div class='score score2'>{1}</div><div class='score score2'>{2}</div><div class='score score2'>{3}</div></div><br>", RemoveRanking(row?[0 + hMatchStep].ToString()), row?[2 + hMatchStep], row?[3 + hMatchStep], row?[4 + hMatchStep]);
                            sb.AppendLine(line);

                        }
                    }
                    else
                    {

                        // Matches 1, 2 and 3
                        for (int i = 0; i < 3; i++)
                        {
                            rowIndex = miniSinglesFirstRow + i * vMatchStep;
                            row = table?.Rows[rowIndex];

                            // Skip this match if first score is not there
                            if (row?[2] is DBNull) { continue; }

                            // First player
                            line = String.Format("<div class='score-row'><div class='player-name'>{0}</div><div class='score'>{1}</div><div class='score'>{2}</div></div>", RemoveRanking(row?[0].ToString()), row?[2], row?[3]);
                            sb.AppendLine(line);

                            // Second player
                            row = table?.Rows[rowIndex + 1];
                            line = String.Format("<div class='score-row'><div class='player-name player-name2'>{0}</div><div class='score score2'>{1}</div><div class='score score2'>{2}</div></div><br>", RemoveRanking(row?[0].ToString()), row?[2], row?[3]);
                            sb.AppendLine(line);
                        }

                        // Matches 4, 5 and 6
                        for (int i = 0; i < 3; i++)
                        {
                            rowIndex = miniSinglesFirstRow + i * vMatchStep;
                            row = table?.Rows[rowIndex];

                            // Skip this match if first score is not there
                            if (row?[2 + hMatchStep] is DBNull) { continue; }

                            // First player
                            line = String.Format("<div class='score-row'><div class='player-name'>{0}</div><div class='score'>{1}</div><div class='score'>{2}</div></div>", RemoveRanking(row?[0 + hMatchStep].ToString()), row?[2 + hMatchStep], row?[3 + hMatchStep]);
                            sb.AppendLine(line);

                            // Second player
                            row = table?.Rows[rowIndex + 1];
                            line = String.Format("<div class='score-row'><div class='player-name player-name2'>{0}</div><div class='score score2'>{1}</div><div class='score score2'>{2}</div></div><br>", RemoveRanking(row?[0 + hMatchStep].ToString()), row?[2 + hMatchStep], row?[3 + hMatchStep]);
                            sb.AppendLine(line);

                        }
                    }
                }
            }
            //}
            sb.Append(ResultsFooting());
            //var s = sb.ToString();
            //var ss = RemoveRanking("Hello (-1)");
            return sb;
        }

        private static string ResultsHeading(string title)
        {
            string heading = """
            <!DOCTYPE html>
            <html lang="en">
            <head>
                <meta charset="UTF-8">
                <title>TPG Drillers - {title}</title>
                <meta name="viewport" content="width=device-width, initial-scale=1">
                <meta http-equiv="cache-control" content="no-cache, must-revalidate, post-check=0, pre-check=0, max-age=0" />
                <meta http-equiv="expires" content="Tue, 01 Jan 1980 1:00:00 GMT" />
                <meta http-equiv="pragma" content="no-cache" />

                <style>
                    .title {
                        display: flex;
                        justify-content: left;
                        margin: 8px 8px 8px 2px;
                        width: 100%;
                        font-size: 22px;
                        font-weight: bold;
                    }

                    .session {
                        font-size: 22px;
                        padding-top: 4px;
                        padding-right: 10px;
                        padding-bottom: 8px;
                        text-decoration: underline;
                    }

                    .location {
                        font-size: 22px;
                        padding-bottom: 8px;
                        text-decoration: underline;
                    }

                    .score {
                        font-size: 14px;
                        text-align: center;
                        border: 1px solid black;
                        border-left: 0px solid white;
                    }
            
                    .score2 {
                        border-top: 0px solid white;
                    }

                    .player-name {
                        font-size: 14px;
                        text-align: left;
                        border: 1px solid black;
                    }

                    .player-name2 {
                        border-top: 0px solid white;
                    }
            
                    .score-row {
                        display: grid;
                        grid-template-columns: 170px 40px 40px 40px;
                        padding-left: 20px;
                    }

                </style>
            </head>
            <body>
                <div class="title">Latest {title} Results</div>

            """;

            return heading.Replace("{title}", title);

        }

        private static string ResultsFooting()
        {
            return """
            </body>
            </html>
            """;
        }

        private static void WriteRankingsHistoryJsonFile(string jsonType, string jsonContent)
        {
            var fullFilename = GetRootDir() + @"\" + jsonType.Replace(" ", "") + "RankingsHistory.json";

            if (fullFilename != null)
            {
                File.WriteAllText(fullFilename, "rankingsHistoryData = " + jsonContent);
            }
        }

        private static string UpdateRankingsHistoryJsonFile(string jsonType, string xlFilePath, int week)
        {
            // Read json rankings history file
            var history = File.ReadAllText(GetRootDir() + @"\" + jsonType.Replace(" ", "") + "RankingsHistory.json");
            var historyObj = history.Substring("rankingsHistoryData = ".Length, history.Length - 22);
            var scores = JsonSerializer.Deserialize<Dictionary<string, List<WeekRanking>>>(historyObj);

            scores ??= []; // If first week create blank object to add into

            Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);

            // Get Dinking or Mini Singles table
            var dataTable = GetRankingsDataTable(jsonType, xlFilePath);
            int numberRows = dataTable == null ? 0 : dataTable.Rows.Count;
            for (int i = 1; i < numberRows; i++)
            {
                string? playerName = RemoveRanking(dataTable?.Rows[i][1].ToString());

                if (playerName != null)
                {
                    var rank = new WeekRanking(week, i);
                    // Either create a new player entry or just add to existing list of week/rank values
                    if (scores.TryGetValue(playerName, out List<WeekRanking>? value))
                    {
                        var rankList = value;
                        rankList?.Add(rank);
                    }
                    else
                    {
                        scores.Add(playerName, [rank]);
                    }
                }
            }

            var scoresString = JsonSerializer.Serialize(scores);
            return scoresString;
        }

        private static void WriteResultsHistoryJsonFile(string jsonType, string jsonContent)
        {
            var fullFilename = GetRootDir() + @"\" + jsonType.Replace(" ", "") + "ResultsHistory.json";

            if (fullFilename != null)
            {
                File.WriteAllText(fullFilename, "resultsHistoryData = " + jsonContent);
            }
        }

        private static string UpdateResultsHistoryJsonFile(string jsonType, string xlFilePath, int week)
        {
            Dictionary<string, List<Result>> matchResults = [];

            // Read json rankings history file
            var history = File.ReadAllText(GetRootDir() + @"\" + jsonType.Replace(" ", "") + "ResultsHistory.json");
            var historyObj = history[("resultsHistoryData = ".Length - 1)..];

            var scores = JsonSerializer.Deserialize<Dictionary<string, List<OneWeekResults>>>(historyObj);

            ReadOneWeeksResults(jsonType, xlFilePath, matchResults);

            if (scores == null) return ("{}");

            // Add this weeks results into running total
            foreach (var name in matchResults.Keys)
            {
                var OneWeekResults = new OneWeekResults(week, matchResults[name]);

                if (scores.TryGetValue(name, out List<OneWeekResults>? value))
                {
                    // Add to existing players records
                    value.Add(OneWeekResults);
                }
                else
                {
                    // Create new player record
                    scores.Add(name, [OneWeekResults]);
                }
            }

            var scoresString = JsonSerializer.Serialize(scores);
            return scoresString;
        }

        static DataTable GetRankingsDataTable(string pageType, string xlFilePath)
        {
            DataTable table;

            Encoding.RegisterProvider(System.Text.CodePagesEncodingProvider.Instance);
            using var stream = File.Open(xlFilePath, FileMode.Open, FileAccess.Read);
            using (var reader = ExcelReaderFactory.CreateReader(stream))
            {
                var result = reader.AsDataSet();

                // Get Dinking table
                table = pageType == "Dinking" ? result.Tables[0] : result.Tables[1];
            }
            return table;
        }

        static DataTable? GetResultsDataTable(int sessionNumber, string xlFilePath)
        {
            DataTable? table = null;
            DataSet? result; // = null;

            Encoding.RegisterProvider(System.Text.CodePagesEncodingProvider.Instance);
            using var stream = File.Open(xlFilePath, FileMode.Open, FileAccess.Read);
            using (var reader = ExcelReaderFactory.CreateReader(stream))
            {
                result = reader.AsDataSet();

                // Get Session table
                table = result.Tables["Session " + sessionNumber];
            }
            return table;
        }

        static string? RemoveRanking(string? playerNameAndRank)
        {
            int? rankIndex = playerNameAndRank?.IndexOf('(');

            if (rankIndex == null || rankIndex < 0)
            {
                return playerNameAndRank;
            }
            else
            {
                return playerNameAndRank?[..Convert.ToInt32(rankIndex - 1)];
            }
        }

        static void ReadOneWeeksResults(string jsonType, string xlFullPath, Dictionary<string, List<Result>> matchResults)
        {
            bool isDinking = jsonType == "Dinking";

            const int dinkingFirstRow = 2;
            const int miniSinglesFirstRow = 6;
            const int hMatchStep = 7;
            const int vMatchStep = 10;

            DataRow? row0, row1, row2;
            int rowIndex;

            string? playername; // = "";
            string? location; // = null;
            string? oppName; // = null;
            //int?[] myScores;
            //int?[] oppScores;
            List<int> myScores;
            List<int> oppScores;
            Result result;

            for (int sessionNumber = 1; sessionNumber < 7; sessionNumber++)
            {
                DataTable? table = GetResultsDataTable(sessionNumber, xlFullPath);

                // Matches 1/4, 2/5 and 3/6
                for (int i = 0; i < 3; i++)
                {
                    rowIndex = i * vMatchStep + (isDinking ? dinkingFirstRow : miniSinglesFirstRow);
                    if (rowIndex >= table?.Rows.Count) { break; }
                    row0 = table?.Rows[0 + i * vMatchStep];
                    row1 = table?.Rows[rowIndex];
                    row2 = table?.Rows[rowIndex + 1];

                    // Two columns of results
                    for (int hOffsetj = 0; hOffsetj <= hMatchStep; hOffsetj += hMatchStep)
                    {
                        // Create 4 results from two matches (1/4 or 2/5 or 3/6)
                        // Skip this match if first score is not there
                        if (row1?[2 + hOffsetj] is DBNull) { continue; }

                        playername = RemoveRanking(row1?[0 + hOffsetj].ToString());
                        if (playername == null) { continue; }
                        location = row0?[2 + hOffsetj].ToString();
                        oppName = RemoveRanking(row2?[0 + hOffsetj].ToString());
                        if (isDinking)
                        {
                            myScores = [Convert.ToInt32(row1?[2 + hOffsetj]), Convert.ToInt32(row1?[3 + hOffsetj]), Convert.ToInt32(row1?[4 + hOffsetj])];
                            oppScores = [Convert.ToInt32(row2?[2 + hOffsetj]), Convert.ToInt32(row2?[3 + hOffsetj]), Convert.ToInt32(row2?[4 + hOffsetj])];
                        }
                        else
                        {
                            myScores = [Convert.ToInt32(row1?[2 + hOffsetj]), Convert.ToInt32(row1?[3 + hOffsetj])];
                            oppScores = [Convert.ToInt32(row2?[2 + hOffsetj]), Convert.ToInt32(row2?[3 + hOffsetj])];
                        }
                        result = new Result(location, oppName, myScores, oppScores);
                        if (matchResults.TryGetValue(playername, out List<Result>? value))
                        {
                            value.Add(result);
                        }
                        else
                        {
                            matchResults.Add(playername, [result]);
                        }

                        playername = RemoveRanking(row2?[0 + hOffsetj].ToString());
                        if (playername == null) { continue; }
                        location = row0?[2 + hOffsetj].ToString();
                        oppName = RemoveRanking(row1?[0 + hOffsetj].ToString());
                        if (isDinking)
                        {
                            myScores = [Convert.ToInt32(row2?[2 + hOffsetj]), Convert.ToInt32(row2?[3 + hOffsetj]), Convert.ToInt32(row2?[4 + hOffsetj])];
                            oppScores = [Convert.ToInt32(row1?[2 + hOffsetj]), Convert.ToInt32(row1?[3 + hOffsetj]), Convert.ToInt32(row1?[4 + hOffsetj])];
                        }
                        else
                        {
                            myScores = [Convert.ToInt32(row2?[2 + hOffsetj]), Convert.ToInt32(row2?[3 + hOffsetj])];
                            oppScores = [Convert.ToInt32(row1?[2 + hOffsetj]), Convert.ToInt32(row1?[3 + hOffsetj])];
                        }
                        result = new Result(location, oppName, myScores, oppScores);
                        if (matchResults.TryGetValue(playername, out value))
                        {
                            value.Add(result);
                        }
                        else
                        {
                            matchResults.Add(playername, [result]);
                        }
                    }
                }
            }
        }

        static string GetRootDir()
        {

            DirectoryInfo? di = new FileInfo(System.Reflection.Assembly.GetExecutingAssembly().Location)?.Directory;

            var codeDir = di?.FullName;

            if (codeDir == null) return "";

            var root = Directory.GetParent(codeDir)?.Parent?.Parent?.FullName;

            return root ?? "";
        }
    }
}

