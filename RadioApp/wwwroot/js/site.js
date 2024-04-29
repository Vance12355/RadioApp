// Please see documentation at https://learn.microsoft.com/aspnet/core/client-side/bundling-and-minification
// for details on configuring this project to bundle and minify static web assets.

// Write your JavaScript code.


var connection = new signalR.HubConnectionBuilder().withUrl("/chathub").build();
var logged = false
//SPAM
var lastMessageTime = null;
var messageLimitInterval = 3000; // ограничение на одно сообщение в 3 секунды

var bannedWords = ["мат", "Кирилл"]; // список запрещенных слов

var isModerator = false;

connection.start().then(function () {
    console.log("connected");
}).catch(function (err) {
    return console.error(err.toString());
});

connection.on("ReceiveMessage", function (user, message) {
    var encodedUser = user;
    var encodedMsg = message;
    var messageDiv = document.createElement("div");
    messageDiv.innerHTML = encodedUser + ": " + encodedMsg;
    document.getElementById("messagesList").appendChild(messageDiv);
});
connection.on("NewMessage", function (messageId, messageContent, user) {
    var messageElement = `<li class="message" id="${messageId}">
                <span class="sender">${user}</span>
                <span class="separator"></span>
                <span class="content"> ${messageContent}</span>`;
    if (isModerator) {
        messageElement += `<button class="deleteButton" onclick="deleteMessage('${messageId}')">  Забыть</button>`;
    }
    messageElement += "</li>";
    $("#messagesList").append(messageElement);
});


connection.on("ModeratorStatus", function (moder) {
    isModerator = moder
});

function startChat() {
    if (!logged) {
        var username = document.getElementById("userInput").value.trim(); // Удаляем пробелы по краям
        if (username === "") {
            alert("Введите корректное имя пользователя."); // Выводим сообщение об ошибке, если имя пользователя пустое
            return;
        }
        logged = true
        $("#chatDiv").fadeIn();
        $("#participantsPanel").fadeIn();
        $("#welcome").hide();
        $("#messagesList").empty();
        connection.invoke("StartChat", username);
    }
}

connection.on("Join", function (username) {
    var kickedMessage = `<li>${username} присоединился в наше общество</li>`;
    $("#messagesList").append(kickedMessage);
});

function sendMessage() {
    var currentTime = new Date().getTime();
    if (lastMessageTime && currentTime - lastMessageTime < messageLimitInterval) {
        alert("Не спамь.");
        return;
    }
    lastMessageTime = currentTime;

    var messageInput = document.getElementById("messageInput").value.trim();
    if (containsBannedWord(messageInput)) {
        alert("Ведите себя культурно.");
        return;
    }
    var userInput = document.getElementById("userInput").value;
    connection.invoke("SendMessage", userInput, messageInput);
    connection.invoke("StartChat", userInput);
    document.getElementById("messageInput").value = "";
}
$(document).ready(function () {
    $("#messagesList").on("DOMNodeInserted", function () {
        $(this).scrollTop($(this).prop("scrollHeight"));
    });

    $("#sendMessageButton").click(function (e) {
        e.preventDefault();
        sendMessage();
    });

    // Отправка сообщения по нажатию Enter
    $("#messageInput").keypress(function (e) {
        if (e.which === 13) {
            e.preventDefault();
            sendMessage();
        }
    });

});

function containsBannedWord(message) {
    var lowerCaseMessage = message.toLowerCase();

    return bannedWords.some(word => lowerCaseMessage.includes(word.toLowerCase()));
}


connection.on("UpdateUserList", function (userList) {
    $("#participantsList").empty();
    userList.forEach(function (username) {
        var userElement = `<li class="participant" id="${username}">${username}  `;
        if (isModerator) {
            userElement += `<button class="kickButton" onclick="kickUser('${username}')">Дать по жопе</button>`;
        }
        userElement += "</li>";
        $("#participantsList").append(userElement);
    });
});


//MODERATION
connection.on("MessageDeleted", function (messageId) {
    $("#" + messageId).remove();
});

function deleteMessage(messageId) {
    connection.invoke("DeleteMessage", messageId).catch(function (err) {
        return console.error(err.toString());
    });
}
connection.on("UserKicked", function (username) {
    var kickedMessage = `<li>${username} был изгнан из чата</li>`;

    $("#messagesList").append(kickedMessage);
});

function kickUser(username) {
    connection.invoke("KickUser", username);
}

connection.on("YouHaveBeenKicked", function () {
    connection.stop();

    alert("Вы были изгнаны.");
    $("#chatDiv").fadeOut();
    $("#participantsPanel").fadeOut();
});
