function handleOnLoad() {
    // Load player drop down
    // All players in alphabetically sorted order
    var players = Object.keys(resultsHistoryData);
    players.sort();

    var selectPlayer = document.getElementById("player");

    // Blank first line
    let opt = document.createElement("option");
    selectPlayer.appendChild(opt);

    // Add all names after blank line
    players.forEach(function (player) {
        let opt = document.createElement("option");
        opt.textContent = player;
        opt.value = player;
        selectPlayer.appendChild(opt);
    })
}

function handlePlayerChange() {

    // Clear any previous results
    const resultsHolder = document.getElementById("results-holder");
    resultsHolder.innerHTML = "";

    var playerName = document.getElementById("player").value;
    var playerResults = resultsHistoryData[playerName];

    var isDinking = document.getElementById("title").innerText.substring(0, 7) == "Dinking";

    var stats = {
        matchesPlayed: 0,
        matchesWonByScore: 0,
        matchesLostByScore: 0,
        matchesTiedByScore: 0,
        matchesWonByGames: 0,
        matchesLostByGames: 0,
        matchesTiedByGames: 0,
        pointsFor: 0,
        pointsAgainst: 0,
        gamesFor: 0,
        gamesAgainst: 0,
        game1For: 0,
        game1Against: 0,
        game2For: 0,
        game2Against: 0,
        game3For: 0,
        game3Against: 0
    };

    var statValues = []

    const statNames = [];
    if (isDinking)
        statNames.push("Average Game 3 Points Against:", "Average Game 3 Points For:");
    statNames.push("Average Game 2 Points Against:", "Average Game 2 Points For:");
    statNames.push("Average Game 1 Points Against:", "Average Game 1 Points For:");
   statNames.push("Average Match Points Against:", "Average Match Points For:");
    statNames.push("Total Points Against:", "Total Points For:");
    if (!isDinking)
        statNames.push("Matches Tied (by games):");
    statNames.push("Matches Lost (by games):", "Matches Won (by games):");
    statNames.push("Matches Tied (by score):", "Matches Lost (by score):", "Matches Won (by score):");
    statNames.push("Total Matches Played:");


    playerResults.forEach(function (weekResult) {
        weekResult.Results.forEach(function (result) {
            let match = {
                pointsFor: 0, pointsAgainst: 0,
                gamesFor: 0, gamesAgainst: 0,
                gameNPointsFor: [0,0,0], gameNPointsAgainst: [0,0,0]
            };

            for (let i = 0; i < result.MyScores.length; i++) {
                match.pointsFor += result.MyScores[i];
                match.pointsAgainst += result.OppScores[i];

                match.gamesFor += result.MyScores[i] == 11 ? 1 : 0;
                match.gamesAgainst += result.OppScores[i] == 11 ? 1 : 0;

                match.gameNPointsFor[i] += result.MyScores[i];
                match.gameNPointsAgainst[i] += result.OppScores[i];
            }

            stats.matchesPlayed++;

            stats.matchesWonByScore += match.pointsFor > match.pointsAgainst ? 1 : 0;
            stats.matchesLostByScore += match.pointsFor < match.pointsAgainst ? 1 : 0;
            stats.matchesTiedByScore += match.pointsFor == match.pointsAgainst ? 1 : 0;

            stats.matchesWonByGames += match.gamesFor > match.gamesAgainst ? 1 : 0;
            stats.matchesLostByGames += match.gamesFor < match.gamesAgainst ? 1 : 0;
            stats.matchesTiedByGames += match.gamesFor == match.gamesAgainst ? 1 : 0;

            stats.pointsFor += match.pointsFor;
            stats.pointsAgainst += match.pointsAgainst;

            stats.game1For += match.gameNPointsFor[0];
            stats.game1Against += match.gameNPointsAgainst[0];
            stats.game2For += match.gameNPointsFor[1];
            stats.game2Against += match.gameNPointsAgainst[1];
            stats.game3For += match.gameNPointsFor[2];
            stats.game3Against += match.gameNPointsAgainst[2];
       })
    })
    statValues.push((stats.game3Against / stats.matchesPlayed).toFixed(2), (stats.game3For / stats.matchesPlayed).toFixed(2));
    statValues.push((stats.game2Against / stats.matchesPlayed).toFixed(2), (stats.game2For / stats.matchesPlayed).toFixed(2));
    statValues.push((stats.game1Against / stats.matchesPlayed).toFixed(2), (stats.game1For / stats.matchesPlayed).toFixed(2));

    statValues.push((stats.pointsAgainst / stats.matchesPlayed).toFixed(2), (stats.pointsFor / stats.matchesPlayed).toFixed(2));
    statValues.push(stats.pointsAgainst, stats.pointsFor);

    if (!isDinking)
        statValues.push(stats.matchesTiedByGames);
    statValues.push(stats.matchesLostByGames, stats.matchesWonByGames);

    statValues.push(stats.matchesTiedByScore, stats.matchesLostByScore, stats.matchesWonByScore);

    statValues.push(stats.matchesPlayed);

    let statsTitle = document.createElement("div");
    statsTitle.className = "stats-title";
    statsTitle.innerText = "Stats";
    resultsHolder.appendChild(statsTitle);

    let statsRow = document.createElement("div");
    statsRow.className = "stats-row";

    const loopCount = statNames.length;
    for (let i = 0; i < loopCount; i++) {

        let statNameDiv = document.createElement("div");
        statNameDiv.className = "stat-name" + (i == 0 ? "" : " stat-name-not-first");
        statNameDiv.innerText = statNames.pop();
        statsRow.appendChild(statNameDiv);

        let statValueDiv = document.createElement("div");
        statValueDiv.className = "stat-value" + (i == 0 ? "" : " stat-value-not-first");
        statValueDiv.innerText = statValues.pop();
        statsRow.appendChild(statValueDiv);
    }
    resultsHolder.appendChild(statsRow);

    playerResults.forEach(function (weekResult) {

        let resultWeek = document.createElement("div");
        resultWeek.className = "week-number";
        resultWeek.innerText = "Week: " + weekResult.Week;
        resultsHolder.appendChild(resultWeek);

        weekResult.Results.forEach(function (result) {
            let location = document.createElement("div");
            location.className = "location";
            location.innerText = result.Location;
            resultsHolder.appendChild(location);

            let scoreRow = document.createElement("div");
            scoreRow.className = "score-row";

            let playerNameDiv = document.createElement("div");
            playerNameDiv.className = "player-name";
            playerNameDiv.innerText = playerName;
            scoreRow.appendChild(playerNameDiv);

            for (let i = 0; i < 3; i++) {
                let scoreDiv = document.createElement("div");
                if (isDinking || i < 2) {
                    scoreDiv.className = "score";
                    scoreDiv.innerText = result.MyScores[i];
                }
                scoreRow.appendChild(scoreDiv);
            }

            playerNameDiv = document.createElement("div");
            playerNameDiv.className = "player-name player2-name";
            playerNameDiv.innerText = result.OppName;
            scoreRow.appendChild(playerNameDiv);

            for (let i = 0; i < 3; i++) {
                let scoreDiv = document.createElement("div");
                if (isDinking || i < 2) {
                    scoreDiv.className = "score score2";
                    scoreDiv.innerText = result.OppScores[i];
                }
                scoreRow.appendChild(scoreDiv);
            }

            resultsHolder.appendChild(scoreRow);
        })
    });
}
