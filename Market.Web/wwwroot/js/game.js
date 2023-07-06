// Объект подключения
const hubConnection = new signalR.HubConnectionBuilder().withUrl("/hub").build();

let playerId;
let connectionId;
let gameId;
let playerName;
let playerCreator=false;
let time=null;
let lastmonth = 0;
let flagMaterialButton = false;
let flagFuelButton = false;
let flagTransportButton = false;
let flagOperationButton = false;
let flagDescriptionShow = false;
let flagSetPassword = false;
const numberSuppliers = 9;

//3 потому как по 3 поставщика каждого вида, для перехода от
//одной категории поставщиков к другой достаточно изменить индекс на 3
const shiftSuppliers = 3;

const elements = {
    userIdDiv: document.getElementById("userId"),
    gamePlayground: document.getElementById("gamePlayground"),
    gameLobby: document.getElementById("gameLobby"),
    namePlayer: document.getElementById("name-player"),
    createRoomBtn: document.getElementById("createRoomBtn"),
    nameLobbyInput: document.getElementById("nameLobbyInput"),
    passwordLobbyInput: document.getElementById("passwordLobbyInput"),
    setPasswordCheckbox: document.getElementById("setPasswordCheckbox"),
    namePlayerInput: document.getElementById("namePlayerInput"),
    rooms: document.getElementById("rooms"),
    //параметры компании
    personalData: document.getElementById("personalData"),
    bankData: document.getElementById("bankData"),
    profitData: document.getElementById("profitData"),
    saleData: document.getElementById("saleData"),
    reputationData: document.getElementById("reputationData"),
    //данные хода
    news: document.getElementById("news"),
    timer: document.getElementById("timer"),
    chatMessage: document.getElementById("chat-message"),
    chatText: document.getElementById("chat-text"),
    //боковые слайдеры
    materialBtn: document.getElementById("materialBtn"),
    fuelBtn: document.getElementById("fuelBtn"),
    transportBtn: document.getElementById("transportBtn"),
    searchBtn: document.getElementById("searchBtn"),
    //кнопки выбора поставщиков
    chooseFirstMaterial: document.getElementById("chooseFirstMaterial"),
    chooseSecondMaterial: document.getElementById("chooseSecondMaterial"),
    chooseThirdMaterial: document.getElementById("chooseThirdMaterial"),
    chooseFirstFuel: document.getElementById("chooseFirstFuel"),
    chooseSecondFuel: document.getElementById("chooseSecondFuel"),
    chooseThirdFuel: document.getElementById("chooseThirdFuel"),
    chooseFirstTransport: document.getElementById("chooseFirstTransport"),
    chooseSecondTransport: document.getElementById("chooseSecondTransport"),
    chooseThirdTransport: document.getElementById("chooseThirdTransport"),
    menuOperations: document.getElementById("menuOperations"),
    gameStatistic: document.getElementById("gameStatistic"),
    divForTabStat: document.getElementById("div-for-t-stat"),
}

elements.createRoomBtn.disabled = true;
elements.passwordLobbyInput.disabled = true;

// Функции для работы лобби
// Обработчик кнопки создания комнаты
elements.createRoomBtn.addEventListener("click", function () {
    let name = elements.nameLobbyInput.value;
    let password = elements.passwordLobbyInput.value;
    if (name != "") {
        if ((flagSetPassword == true) && (password != "")) {
            playerCreator = true;
            hubConnection.invoke("CreateRoom", name, playerId, playerName, password)
                .then(() => {
                    elements.createRoomBtn.disabled = true;
                    elements.setPasswordCheckbox.disabled = true;
                }).catch(function (err) {
                    return console.error(err.toString());
                });
        } else {
            playerCreator = true;
            hubConnection.invoke("CreateRoom", name, playerId, playerName, "")
                .then(() => {
                    elements.createRoomBtn.disabled = true;
                    elements.setPasswordCheckbox.disabled = true;
                }).catch(function (err) {
                    return console.error(err.toString());
                });
        }
    }
    else
    {
        elements.createRoomBtn.focus();
    }
});

function changePasswordCheckbox() {
    if (flagSetPassword == false) {
        flagSetPassword = true;
        elements.passwordLobbyInput.disabled = false;
    } else {
        flagSetPassword = false;
        elements.passwordLobbyInput.disabled = true;
    }
}

$(document).on('click', '.exitButton', function () {
    hubConnection.invoke("Exit", playerId)
        .then(() => { })
        .catch((err) => alert(err));
})

function drawRoom(room) {
    let divRoomElement = document.createElement('div');
    let divTop = document.createElement('div');
    let span = document.createElement("span");
    let button = document.createElement("button");

    if (room.password != false) {
        span.innerHTML = `${room.name} - комната.<br> Владелец - ${room.player1.name}`;
    } else {
        span.innerText = `${room.name} - комната. Владелец - ${room.player1.name}`;
    }
    span.classList.add('gameLobbyRoomName');

    console.log("connectionId: " + connectionId + " ; " + " room.player1.conId: " +
        room.player1.connectionId + " room.player1.userId: " + room.player1.userId +
        " playerId: " + playerId);
    if (room.player1.userId == playerId) {
        button.innerHTML = "Нач&shy;ать";
        button.classList.add('gameLobbyRoomStartBtn');
    }
    else {
        button.innerText = "Войти";
        button.classList.add('gameLobbyRoomJoinBtn');
        if ((room.player2.userId == playerId) || (room.player3.userId == playerId)
            || (room.player4.userId == playerId) || (playerCreator == true)) {
            button.disabled = true;
            elements.createRoomBtn.disabled = true;
        }
    }
    if (room.password != false) {
        divTop.append(span);
        if (room.player1.userId == playerId) {
            divTop.innerHTML += `<input type="text" id="${room.player1.userId}passinput" class="passwordInput" placeholder="Введите пароль лобби" disabled/>`;
        } else {
            divTop.innerHTML += `<input type="text" id="${room.player1.userId}passinput" class="passwordInput" placeholder="Введите пароль лобби" />`;
        }
        divTop.append(button);
    } else {
        divTop.append(span, button);
    }
    divTop.classList.add(`${room.player1.userId}`, "roomItem");

    //Создание таблицы игроков в комнате
    let divBottom = document.createElement('div');
    let table = document.createElement('table');
    let tbody = document.createElement('tbody');
    let strin;
    let td;

    strin = `<tr><td class="td-room">Игрок1</td><td class="td-room">Игрок2</td><td class="td-room">Игрок3</td>
        <td class="td-room">Игрок4</td></tr><tr><td class="td-room">${room.player1.name}</td>`;

    td = `<td class="td - room"></td>`;
    if (room.player2 != null) {
        td = `<td class="td-room">${room.player2.name}</td>`;
        console.log("if worked, player2 find");
    }
    strin += td;
    td = `<td class="td-room"></td>`;
    if (room.player3 != null) {
        td = `<td class="td-room">${room.player3.name}</td>`;
    }
    strin += td;
    td = `<td class="td - room"></td>`;
    if (room.player4 != null) {
        td = `<td class="td-room">${room.player4.name}</td>`;
    }
    strin += td + "</tr>";
    tbody.innerHTML = strin;
    table.append(tbody);
    table.classList.add("table-room");
    divBottom.append(table);
    divBottom.classList.add("roomItemBottom");
    divRoomElement.append(divTop, divBottom);
    divRoomElement.classList.add("room");

    return divRoomElement;
}

hubConnection.on("ListRooms", function (allServerRooms, conId = connectionId) {
    connectionId = conId;
    elements.rooms.innerHTML = "";
    allServerRooms.forEach(room => {
        elements.rooms.appendChild(drawRoom(room));
    });
    console.log("list worked");
})

$(document).on('click', '.gameLobbyRoomJoinBtn', function () {
    let classes = $(this).parent().attr('class');
    let idHost = classes.substring(0, classes.indexOf(" "));
    let password = "";
    let passinput = document.getElementById(`${idHost}passinput`);
    if (passinput != null)
        password = passinput.value;
    hubConnection.invoke("JoinRoom", playerName, playerId, idHost, password)
        .then(() => {})
        .catch((err) => alert(err));
})

hubConnection.on("WrongPassword", function (idHost) {
    let passInput = document.getElementById(`${idHost}passinput`);
    let roomDiv = passInput.parentElement.parentElement;
    if (roomDiv.dataset.er == null) {
        roomDiv.lastChild.innerHTML += (`<div class="errorPassword">Неверный пароль</div>`);
        roomDiv.dataset.er = "withError";
    }
})

$(document).on('click', '.gameLobbyRoomStartBtn', function () {
    hubConnection.invoke("StartGame", playerId, connectionId)
        .catch((err) => alert(err));
})

hubConnection.on("Start", function (game) {
    elements.gameLobby.style.display = "none";
    elements.gamePlayground.style.display = "flex";
    gameId = game.id;
})

function updateGame(game) {
    let player = null;
    if (game.player1.connectionId == connectionId) { player = game.player1; }
    else if (game.player2.connectionId == connectionId) { player = game.player2; }
    else if (game.player3.connectionId == connectionId) { player = game.player3; }
    else if (game.player4.connectionId == connectionId) { player = game.player4; }
    else { throw "No equality connection id and id players in target game"; }
    personalData.innerHTML = `<img src="../img/personal_icon.png" /> ${player.company.personal}`;
    bankData.innerHTML = `<img src="../img/bank_icon.png" /> ${player.company.bank}`;
    profitData.innerHTML = `<img src="../img/profit_icon.png" /> ${player.company.profit}`;
    saleData.innerHTML = `<img src="../img/sales_icon.png" /> ${player.company.sales}`;
    reputationData.innerHTML = `<img src="../img/reputation_icon.png" /> ${player.company.reputation}`;
    //обновление кнопок
    //перевод массива поставщиков в массив имен(строк) для поиска по индексу
    var listSuppliersNames = new Array(0);
    game.market.suppliers.forEach((suppl) => {
        listSuppliersNames.push(suppl.name)
    });
    //получение индексов поставщиков материала игроков
    let numberSupplierPlayer1 = listSuppliersNames.indexOf(game.player1.company.material.name);
    let numberSupplierPlayer2 = listSuppliersNames.indexOf(game.player2.company.material.name);
    let numberSupplierPlayer3 = listSuppliersNames.indexOf(game.player3.company.material.name);
    let numberSupplierPlayer4 = listSuppliersNames.indexOf(game.player4.company.material.name);
    //запись имен игроков в строки массива в соответствии с ноомерами поставщиков игроков
    var arrNames = ["", "", ""];
    arrNames[numberSupplierPlayer1] = `${game.player1.name}<br>`;
    arrNames[numberSupplierPlayer2] += `${game.player2.name}<br>`;
    arrNames[numberSupplierPlayer3] += `${game.player3.name}<br>`;
    arrNames[numberSupplierPlayer4] += `${game.player4.name}<br>`;

    elements.chooseFirstMaterial.innerHTML = `${game.market.suppliers[0].name}<br>
    Репутация:${game.market.suppliers[0].reputation}<br>Качество:${game.market.suppliers[0].quality}<br>
    ${arrNames[0]}<div class="update-show">Заключить контракт</div>`;
    elements.chooseSecondMaterial.innerHTML = `${game.market.suppliers[1].name}<br>
    Репутация:${game.market.suppliers[1].reputation}<br>Качество:${game.market.suppliers[1].quality}<br>
    ${arrNames[1]}<div class="update-show">Заключить контракт</div>`;
    elements.chooseThirdMaterial.innerHTML = `${game.market.suppliers[2].name}<br>
    Репутация:${game.market.suppliers[2].reputation}<br>Качество:${game.market.suppliers[2].quality}<br>
    ${arrNames[2]}<div class="update-show">Заключить контракт</div>`;

    //повторение для поставщиков топлива
    numberSupplierPlayer1 = listSuppliersNames.indexOf(game.player1.company.fueller.name);
    numberSupplierPlayer2 = listSuppliersNames.indexOf(game.player2.company.fueller.name);
    numberSupplierPlayer3 = listSuppliersNames.indexOf(game.player3.company.fueller.name);
    numberSupplierPlayer4 = listSuppliersNames.indexOf(game.player4.company.fueller.name);

    //вычитание смещения для перехода к другой категории поставщиков
    arrNames = ["", "", ""];
    arrNames[numberSupplierPlayer1 - shiftSuppliers] = `${game.player1.name}<br>`;
    arrNames[numberSupplierPlayer2 - shiftSuppliers] += `${game.player2.name}<br>`;
    arrNames[numberSupplierPlayer3 - shiftSuppliers] += `${game.player3.name}<br>`;
    arrNames[numberSupplierPlayer4 - shiftSuppliers] += `${game.player4.name}<br>`;

    elements.chooseFirstFuel.innerHTML = `${game.market.suppliers[3].name}<br>
    Репутация:${game.market.suppliers[3].reputation}<br>Качество:${game.market.suppliers[3].quality}<br>
    ${arrNames[0]}<div class="update-show">Заключить контракт</div>`;
    elements.chooseSecondFuel.innerHTML = `${game.market.suppliers[4].name}<br>
    Репутация:${game.market.suppliers[4].reputation}<br>Качество:${game.market.suppliers[4].quality}<br>
    ${arrNames[1]}<div class="update-show">Заключить контракт</div>`;
    elements.chooseThirdFuel.innerHTML = `${game.market.suppliers[5].name}<br>
    Репутация:${game.market.suppliers[5].reputation}<br>Качество:${game.market.suppliers[5].quality}<br>
    ${arrNames[2]}<div class="update-show">Заключить контракт</div>`;

    //повторение для поставщиков транспорта
    numberSupplierPlayer1 = listSuppliersNames.indexOf(game.player1.company.transport.name);
    numberSupplierPlayer2 = listSuppliersNames.indexOf(game.player2.company.transport.name);
    numberSupplierPlayer3 = listSuppliersNames.indexOf(game.player3.company.transport.name);
    numberSupplierPlayer4 = listSuppliersNames.indexOf(game.player4.company.transport.name);

    //вычитание смещения * 2 для перехода к третьей категории поставщиков
    arrNames = ["", "", ""];
    arrNames[numberSupplierPlayer1 - shiftSuppliers*2] = `${game.player1.name}<br>`;
    arrNames[numberSupplierPlayer2 - shiftSuppliers*2] += `${game.player2.name}<br>`;
    arrNames[numberSupplierPlayer3 - shiftSuppliers*2] += `${game.player3.name}<br>`;
    arrNames[numberSupplierPlayer4 - shiftSuppliers*2] += `${game.player4.name}<br>`;

    elements.chooseFirstTransport.innerHTML = `${game.market.suppliers[6].name}<br>
    Репутация:${game.market.suppliers[6].reputation}<br>Качество:${game.market.suppliers[6].quality}<br>
    ${arrNames[0]}<div class="update-show">Заключить контракт</div>`;
    elements.chooseSecondTransport.innerHTML = `${game.market.suppliers[7].name}<br>
    Репутация:${game.market.suppliers[7].reputation}<br>Качество:${game.market.suppliers[7].quality}<br>
    ${arrNames[1]}<div class="update-show">Заключить контракт</div>`;
    elements.chooseThirdTransport.innerHTML = `${game.market.suppliers[8].name}<br>
    Репутация:${game.market.suppliers[8].reputation}<br>Качество:${game.market.suppliers[8].quality}<br>
    ${arrNames[2]}<div class="update-show">Заключить контракт</div>`;
    console.log("update done");
}

function updateOperation(game) {
    elements.menuOperations.innerHTML = "";
    let player = null;
    if (game.player1.userId == playerId) { player = game.player1; }
    else if (game.player2.userId == playerId) { player = game.player2; }
    else if (game.player3.userId == playerId) { player = game.player3; }
    else if (game.player4.userId == playerId) { player = game.player4; }
    console.log(player);
    game.market.operations.forEach((operation) => {
        let hoverHolder = document.createElement("div");
        hoverHolder.classList.add("classHover");
        hoverHolder.setAttribute('id', operation.name);
        let button = document.createElement("button");
        button.classList.add('operationBtnOn');
        button.setAttribute('id', operation.name);
        let description = document.createElement("div");
        description.classList.add('descriptionWindow');
        description.setAttribute('id', `desc-${operation.name}`);
        description.innerHTML = `${operation.name}<br>${operation.description}<br>Стоимость: ${operation.cost}`;
        //Отключение сделанных операций
        if (player.company.doneOperations.indexOf(operation.name) >= 0) {
            button.disabled = true;
            button.style.opacity = 0.5;
            hoverHolder.style.backgroundColor = "white";
            hoverHolder.style.border = "2px solid black";
            hoverHolder.style.borderRadius = "4px";
        } else {
            hoverHolder.append(button);
        }
        elements.menuOperations.appendChild(hoverHolder);
        elements.menuOperations.appendChild(description);
    });
    $("div.classHover").hover(
        function () {
            document.getElementById(`desc-${(this).getAttribute("id")}`).setAttribute("style", "display:block");
        },
        function () {
            document.getElementById(`desc-${(this).getAttribute("id")}`).setAttribute("style", "display:none");
        }
    )
}

$(document).on('click', '.operationBtnOn', function () {
    console.log("oper");
    operationId = (this).getAttribute("id");
    hubConnection.invoke("DoOperation", playerId, operationId, gameId)
        .catch(function (err) {
            return console.error(err.toString());
        });
});

function timer(timeSeconds) {
    counter = timeSeconds / 1000;
    time=setInterval(function () {
        seconds = counter % 60;
        minutes = counter / 60;
        if (counter <= 0) {
            elements.timer.innerText = "00:00";
            clearInterval(time);
        } else {
            let strTimer = `${Math.trunc(minutes)}:${seconds}`;
            elements.timer.innerText = strTimer;
        }
        --counter;
    }, 1000);
    return time;
}

hubConnection.on("StartTurn", function (game, news, thisTurnTime) {
    if (lastmonth != 0) {
        monthName = document.getElementById(`${lastmonth}`);
        delTag = `<del>${monthName.innerText}</del>`;
        monthName.innerHTML = delTag;
    }
    lastmonth += 1;
    updateGame(game);
    updateOperation(game);
    elements.news.innerText = `${news.title}\n${news.description}`;
    if (time != null) { clearInterval(time); }
    timer(thisTurnTime);
})

document.getElementById("sendBtn").addEventListener("click", function () {
    let textMessage = elements.chatMessage.value;
    hubConnection.invoke("SendMessage", gameId, playerName, textMessage) // отправка данных серверу
        .catch(function (err) {
            return console.error(err.toString());
        });
});

// Устанавливает метод на стороне клиента, получающий данные с сервера
hubConnection.on("UpdateChat", function (playerName, message) {
    let messageElement = document.createElement("p");
    messageElement.textContent = playerName + ": " + message;
    elements.chatText.appendChild(messageElement);
});

//События кнопок-слайдеров
$("#materialBtn").click(function () {
    if (flagMaterialButton == false) {
        $("#element-slide-fi").animate({
            left: "-=900%",
        }, 1000);
        $("#material-slide").animate({
            opacity: "+=100%",
        }, 1000);
        flagMaterialButton = true;
    } else {
        $("#element-slide-fi").animate({
            left: "+=900%",
        }, 1000);
        $("#material-slide").animate({
            opacity: "-=100%",
        }, 1000);
        flagMaterialButton = false;
    }
})

$("#fuelBtn").click(function () {
    if (flagFuelButton == false) {
        $("#element-slide-s").animate({
            left: "-=900%",
        }, 1000);
        $("#fuel-slide").animate({
            opacity: "+=100%",
        }, 1000);
        flagFuelButton = true;
    } else {
        $("#element-slide-s").animate({
            left: "+=900%",
        }, 1000);
        $("#fuel-slide").animate({
            opacity: "-=100%",
        }, 1000);
        flagFuelButton = false;
    }
})

$("#transportBtn").click(function () {
    if (flagTransportButton == false) {
        $("#element-slide-t").animate({
            left: "-=900%",
        }, 1000);
        $("#transport-slide").animate({
            opacity: "+=100%",
        }, 1000);
        flagTransportButton = true;
    } else {
        $("#element-slide-t").animate({
            left: "+=900%",
        }, 1000);
        $("#transport-slide").animate({
            opacity: "-=100%",
        }, 1000);
        flagTransportButton = false;
    }
})

$("#operationBtn").click(function () {
    if (flagOperationButton == false) {
        $("#element-slide-f").animate({
            left: "-=900%",
        }, 1000);
        $("#operation-slide").animate({
            opacity: "+=100%",
        }, 1000);
        flagOperationButton = true;
    } else {
        $("#element-slide-f").animate({
            left: "+=900%",
        }, 1000);
        $("#operation-slide").animate({
            opacity: "-=100%",
        }, 1000);
        flagOperationButton = false;
    }
})

//События кнопок-смен поставщиков
$("#chooseFirstMaterial").click(function () {
    hubConnection.invoke("ChangeSupplier", gameId, connectionId, 0)
        .catch(function (err) {
            return console.error(err.toString());
        });
})

$("#chooseSecondMaterial").click(function () {
    hubConnection.invoke("ChangeSupplier", gameId, connectionId, 1)
        .catch(function (err) {
            return console.error(err.toString());
        });
})

$("#chooseThirdMaterial").click(function () {
    hubConnection.invoke("ChangeSupplier", gameId, connectionId, 2)
        .catch(function (err) {
            return console.error(err.toString());
        });
})

$("#chooseFirstFuel").click(function () {
    hubConnection.invoke("ChangeSupplier", gameId, connectionId, 3)
        .catch(function (err) {
            return console.error(err.toString());
        });
})

$("#chooseSecondFuel").click(function () {
    hubConnection.invoke("ChangeSupplier", gameId, connectionId, 4)
        .catch(function (err) {
            return console.error(err.toString());
        });
})

$("#chooseThirdFuel").click(function () {
    hubConnection.invoke("ChangeSupplier", gameId, connectionId, 5)
        .catch(function (err) {
            return console.error(err.toString());
        });
})

$("#chooseFirstTransport").click(function () {
    hubConnection.invoke("ChangeSupplier", gameId, connectionId, 6)
        .catch(function (err) {
            return console.error(err.toString());
        });
})

$("#chooseSecondTransport").click(function () {
    hubConnection.invoke("ChangeSupplier", gameId, connectionId, 7)
        .catch(function (err) {
            return console.error(err.toString());
        });
})

$("#chooseThirdTransport").click(function () {
    hubConnection.invoke("ChangeSupplier", gameId, connectionId, 8)
        .catch(function (err) {
            return console.error(err.toString());
        });
})

hubConnection.on("UpdateGame", function (game) {
    updateGame(game);
})

hubConnection.on("UpdateOperation", function (game) {
    updateOperation(game);
    updateGame(game);
})

hubConnection.on("EndGame", function (endedGame, points) {
    console.log("end");
    elements.gamePlayground.style.display = "none";
    elements.gameStatistic.style.display = "flex";

    let table = document.createElement('table');
    let tbody = document.createElement('tbody');
    let strin;
    let tr;
    console.log(points);
    strin = `<tr><td class="td-stat">Игрок</td><td class="td-stat">Компания</td><td class="td-stat">Персонал</td><td>Банк</td>
            <td class="td-stat">Прибыль</td><td class="td-stat">Репутация</td><td class="td-stat">Общий счет</td></tr>
            <tr><td class="td-stat">${endedGame.player1.name}</td><td class="td-stat">${endedGame.market.companies[0].name}</td>
            <td class="td-stat">${endedGame.market.companies[0].personal}</td><td class="td-stat">${endedGame.market.companies[0].bank}</td>
            <td class="td-stat">${endedGame.market.companies[0].profit}</td><td class="td-stat">${endedGame.market.companies[0].reputation}</td>
            <td class="td-stat">${points[0]}</td></tr>`;
    tr = `<tr></tr>`;
    if (endedGame.player2.connectionId != "") {
        tr = `<tr><td class="td-stat">${endedGame.player2.name}</td><td class="td-stat">${endedGame.market.companies[1].name}</td>
            <td class="td-stat">${endedGame.market.companies[1].personal}</td><td class="td-stat">${endedGame.market.companies[1].bank}</td>
            <td class="td-stat">${endedGame.market.companies[1].profit}</td><td class="td-stat">${endedGame.market.companies[1].reputation}</td>
            <td class="td-stat">${points[1]}</td></tr>`;
        console.log("if worked, player2 find");
    }
    strin += tr;
    tr = `<tr></tr>`;
    if (endedGame.player3.connectionId != "") {
        tr = `<tr><td class="td-stat">${endedGame.player3.name}</td><td class="td-stat">${endedGame.market.companies[2].name}</td>
            <td class="td-stat">${endedGame.market.companies[2].personal}</td><td class="td-stat">${endedGame.market.companies[2].bank}</td>
            <td class="td-stat">${endedGame.market.companies[2].profit}</td><td class="td-stat">${endedGame.market.companies[2].reputation}</td>
            <td class="td-stat">${points[2]}</td></tr>`;
    }
    strin += tr;
    tr = `<tr"></tr>`;
    if (endedGame.player4.connectionId != "") {
        tr = `<tr><td class="td-stat">${endedGame.player4.name}</td><td class="td-stat">${endedGame.market.companies[3].name}</td>
            <td class="td-stat">${endedGame.market.companies[3].personal}</td><td class="td-stat">${endedGame.market.companies[3].bank}</td>
            <td class="td-stat">${endedGame.market.companies[3].profit}</td><td class="td-stat">${endedGame.market.companies[3].reputation}</td>
            <td class="td-stat">${points[3]}</td></tr>`;
    }
    strin += tr;
    tbody.innerHTML = strin;
    table.append(tbody);
    table.classList.add("table-stat");
    elements.divForTabStat.append(table);

})

window.addEventListener("beforeunload", function () {
    hubConnection.invoke("Exit", playerId);
});

// Запуск соединения
hubConnection.start()
    .then(function () {
        elements.createRoomBtn.disabled = false;
        textName = elements.namePlayer.innerText;
        playerName = textName.substring(5, textName.length);
        playerId = elements.userIdDiv.className;
    })
    .catch(function (err) {
        return console.error(err.toString());
    });