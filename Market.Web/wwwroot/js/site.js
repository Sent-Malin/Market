// Объект подключения
const hubConnection = new signalR.HubConnectionBuilder().withUrl("/hub").build();

let connectionId = "3";

const elements = {
    createRoomBtn: document.getElementById("createRoomBtn"),
    rooms: document.getElementById("rooms"),
}
elements.createRoomBtn.disabled = true;
// Функции для работы лобби

// Обработчик кнопки создания комнаты
elements.createRoomBtn.addEventListener("click", function () {
    let name = document.getElementById("nameLobbyInput").value;
    
    if (name != "") {
        hubConnection.invoke("CreateRoom", name)
            .then(() => {
                document.getElementById("createRoomBtn").disabled = true;
            }).catch(function (err) {
                return console.error(err.toString());
            });
    }
    else
    {
        elements.createRoomBtn.focus();
    }
});

hubConnection.on("DrawRoom", function (name, player) {
    let div = document.createElement('div');
    let span = document.createElement("span");
    let button = document.createElement("button");

    span.innerText = `${name} - room. Owner = ${player.name}`;
    span.classList.add('gameLobbyRoomName');

    if (player.id == connectionId) {
        button.innerText = "Start";
        button.classList.add('gameLobbyRoomStartBtn');
    }
    else {
        button.innerText = "Join";
        button.classList.add('gameLobbyRoomJoinBtn');
    }
    
    div.append(span, button);
    div.classList.add(`${player.id}`, "roomItem");

    elements.rooms.appendChild(div);
})

hubConnection.on("ListRooms", function (rooms) {
    console.log("list worked");

    rooms.forEach(room => {
        let div = document.createElement('div');
        let span = document.createElement("span");
        let button = document.createElement("button");

        span.innerText = `${room.name} - room. Owner = ${room.player1.name}`;
        span.classList.add('gameLobbyRoomName');

        if (player.id == connectionId) {
            button.innerText = "Start";
            button.classList.add('gameLobbyRoomStartBtn');
        }
        else {
            button.innerText = "Join";
            button.classList.add('gameLobbyRoomJoinBtn');
        }

        div.append(span, button);
        div.classList.add(`${room.player1.id}`);

        elements.rooms.appendChild(div);
    });
})

// Устанавливает метод на стороне клиента, получающий данные с сервера
hubConnection.on("Recieve", function (message) {
    let messageElement = document.createElement("p");
    messageElement.textContent = message;
    document.getElementById("chat-text").appendChild(messageElement);
});

// Запуск соединения
hubConnection.start()
    .then(function () {
        elements.createRoomBtn.disabled = false;
    })
    .catch(function (err) {
        return console.error(err.toString());
    });