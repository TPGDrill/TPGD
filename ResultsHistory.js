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
