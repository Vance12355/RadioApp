// Please see documentation at https://learn.microsoft.com/aspnet/core/client-side/bundling-and-minification
// for details on configuring this project to bundle and minify static web assets.

// Write your JavaScript code.
(function () {

    const connection = new signalR.HubConnectionBuilder()
        .withUrl("/radiohub")
        .build();

    connection.start().then(() => {
        console.log("Connected to SignalR hub");
    }).catch(err => {
        console.error("Error connecting to SignalR hub:", err);
    });

    $("#joinRoomBtn").click(() => {
        console.log("JOIN ROOM");
        connection.invoke("JoinRoom", "MusicRoom").catch(err => {
            console.error("Error joining room:", err);
        });
    });

    $("#leaveRoomBtn").click(() => {
        console.log("LEAVE ROOM");
        connection.invoke("LeaveRoom", "MusicRoom").catch(err => {
            console.error("Error leaving room:", err);
        });
    });

    connection.on("PlayAudio", (audioData) => {
        const audioPlayer = document.getElementById("audioPlayer");
        audioPlayer.src = "data:audio/mpeg;base64," + audioData;
    });

    connection.on("ReceiveAudio", (audioData) => {
        const audioPlayer = document.getElementById("audioPlayer");
        audioPlayer.src = "data:audio/mp3;base64," + audioData;
        //audioPlayer.src = "/audio/music.mp3"
    });

    $("#playAudioBtn").click(() => {
        const audioPlayer = document.getElementById("audioPlayer");
        audioPlayer.play();
    });
})();
