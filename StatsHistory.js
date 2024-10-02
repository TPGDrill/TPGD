// Globals
var chart;
const chartTitle = document.getElementById("title").innerText;

function handleOnLoad() {
    // Load player drop down
    // All players in alphabetically sorted order
    var players = Object.keys(resultsHistoryData);
    players.sort();

    var selectPlayer = document.getElementById("player0");

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

    // Create the chart datasets
    // var channelNames = ["Win % by score", "Lose % by score", "Tie % by score"];
    var channelNames = ["Points For", "Points Against"];
    var lineColors = ["rgb(255,0,0)", "rgb(0,255,0)"];

    var chartDatasets = new Array();

    for (let i = 0; i < channelNames.length; i++) {

        const chartDataset = {
            label: channelNames[i],
            fill: false,
            //showLine: true,
            //steppedLine: true,
            //lineTension: 0,
            pointRadius: 4,
            borderColor: lineColors[i],
            pointBackgroundColor: lineColors[i],
            data: []
        }

        var newDataset = Object.create(chartDataset);
        chartDatasets.push(newDataset);

    }

    var ctx = document.getElementById('myChart').getContext('2d');

    var ttcb = function toolTipCallback(tooltipItem, data) {
        return data.datasets[tooltipItem.datasetIndex].label + " Week " + tooltipItem.label + ": " + Math.abs(tooltipItem.value);
    }
    
    chart = new Chart(ctx, {
        type: "scatter",
        data: { datasets: chartDatasets },
        options: {
            //tooltips: { enabled: true, custom: function (tooltipModel) { var ttm = tooltipModel; } },
            title: { display: true, text: document.getElementById("title").innerText },
            tooltips: {
                enabled: true, callbacks: { label: ttcb } },
    
            animation: { duration: 0 },
            maintainAspectRatio: false,
            legend: { display: true, labels: { boxWidth: 12 } },
            scales: {
                yAxes: [{
                    ticks: { callback: function (value, index, values) { return Math.abs(value); } },
                    scaleLabel: {
                        display: true,
                        labelString: "Stats",
                    }
                }],
                xAxes: [{
                    scaleLabel: {
                        display: true,
                        labelString: "Week #"
                    }
                }]
    
            }
        }
    })
    
    //document.getElementById("myChart").onclick = handleCanvasClick;
    
    chart.update();
    
}
    

function handlePlayerChange() {

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

    const chartDataset0 = {
        label: "Points For",
        fill: false,
        showLine: true,
        //steppedLine: true,
        //lineTension: 0,
        pointRadius: 4,
        borderColor: "rgb(255,0,0)",
        pointBackgroundColor: "rgb(255,0,0)",
        data: []
    }

    const chartDataset1 = {
        label: "Points Against",
        fill: false,
        showLine: true,
        //steppedLine: true,
        //lineTension: 0,
        pointRadius: 4,
        borderColor: "rgb(0,255,0)",
        pointBackgroundColor: "rgb(0,255,0)",
        data: []
    }


    // Get each weeks results of selected player
    var playerName = document.getElementById("player0").value;
    var playerResults = resultsHistoryData[playerName];

    playerResults.forEach(function (weekResult) {

        let weekNumber = weekResult.Week;

        let matches = {
            played: 0,
            pointsFor: 0, pointsAgainst: 0,
            gamesFor: 0, gamesAgainst: 0,
            gameNPointsFor: [0,0,0], gameNPointsAgainst: [0,0,0]
        };
    weekResult.Results.forEach(function (result) {

            for (let i = 0; i < result.MyScores.length; i++) {
                matches.pointsFor += result.MyScores[i];
                matches.pointsAgainst += result.OppScores[i];

                matches.gamesFor += result.MyScores[i] == 11 ? 1 : 0;
                matches.gamesAgainst += result.OppScores[i] == 11 ? 1 : 0;

                matches.gameNPointsFor[i] += result.MyScores[i];
                matches.gameNPointsAgainst[i] += result.OppScores[i];
            }

            matches.played++

       })

       // Add this weeks stats to chart arrays


       // Update cummulative numbers and add to chart arrays
       stats.matchesPlayed += matches.played;

       stats.matchesWonByScore += matches.pointsFor > matches.pointsAgainst ? 1 : 0;
       stats.matchesLostByScore += matches.pointsFor < matches.pointsAgainst ? 1 : 0;
       stats.matchesTiedByScore += matches.pointsFor == matches.pointsAgainst ? 1 : 0;

       stats.matchesWonByGames += matches.gamesFor > matches.gamesAgainst ? 1 : 0;
       stats.matchesLostByGames += matches.gamesFor < matches.gamesAgainst ? 1 : 0;
       stats.matchesTiedByGames += matches.gamesFor == matches.gamesAgainst ? 1 : 0;

       stats.pointsFor += matches.pointsFor;
       stats.pointsAgainst += matches.pointsAgainst;

       stats.game1For += matches.gameNPointsFor[0];
       stats.game1Against += matches.gameNPointsAgainst[0];
       stats.game2For += matches.gameNPointsFor[1];
       stats.game2Against += matches.gameNPointsAgainst[1];
       stats.game3For += matches.gameNPointsFor[2];
       stats.game3Against += matches.gameNPointsAgainst[2];

       // Create the points for this week in the Datasets
       chartDataset0.data.push({x:weekNumber, y:stats.pointsFor});
       chartDataset1.data.push({x:weekNumber, y:stats.pointsAgainst});

    })

    // Put chart arrays into chart

    

    // statValues.push((stats.game3Against / stats.matchesPlayed).toFixed(2), (stats.game3For / stats.matchesPlayed).toFixed(2));
    // statValues.push((stats.game2Against / stats.matchesPlayed).toFixed(2), (stats.game2For / stats.matchesPlayed).toFixed(2));
    // statValues.push((stats.game1Against / stats.matchesPlayed).toFixed(2), (stats.game1For / stats.matchesPlayed).toFixed(2));

    // statValues.push((stats.pointsAgainst / stats.matchesPlayed).toFixed(2), (stats.pointsFor / stats.matchesPlayed).toFixed(2));
    // statValues.push(stats.pointsAgainst, stats.pointsFor);

    // if (!isDinking)
    //     statValues.push(stats.matchesTiedByGames);
    // statValues.push(stats.matchesLostByGames, stats.matchesWonByGames);

    // statValues.push(stats.matchesTiedByScore, stats.matchesLostByScore, stats.matchesWonByScore);

    // statValues.push(stats.matchesPlayed);

    //chart.data.datasets[0] = chartDataset;

    chart.data.datasets[0] = chartDataset0;
    chart.data.datasets[1] = chartDataset1;
    chart.update();
}

function toggleSettingsVisibility() {
    var settingsDisplay = document.getElementById("settings").style.display;
    document.getElementById("chartHolder").style.height = (settingsDisplay == "none") ? "80vh" : "88vh";
    document.getElementById("settings").style.display = (settingsDisplay == "none") ? "grid" : "none";
}
