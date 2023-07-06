
let flagAuthWindow = true;

const elements = {
    gameAuthorization: document.getElementById("gameAuthorization"),
    printAuthButton: document.getElementById("printAuthButton"),
    printRegButton: document.getElementById("printRegButton"),
    authArticle: document.getElementById("authArticle"),
    authNameUserText: document.getElementById("authName"),
    authPassUserText: document.getElementById("authPass"),

    regArticle: document.getElementById("regArticle"),
    regNameUserText: document.getElementById("regName"),
    regPassUserText: document.getElementById("regPass"),
    regPassUserTextRepeat: document.getElementById("regPassRepeat"),
}

// Обработчики кнопок преключения интерфейса
elements.printAuthButton.addEventListener("click", function () {
    if (flagAuthWindow == false) {
        elements.authArticle.className = "column";
        elements.regArticle.className = "displayNone";
        flagAuthWindow = true;
    }
});

elements.printRegButton.addEventListener("click", function () {
    if (flagAuthWindow == true) {
        elements.authArticle.className = "displayNone";
        elements.regArticle.className = "column";
        flagAuthWindow = false;
    }
});