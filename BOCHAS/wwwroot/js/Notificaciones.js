const connection = new signalR.HubConnectionBuilder()
    .withUrl("/chatHub")
    .build();

connection.on("ReceiveMessage", (user, message) => {
    const msg = message.replace(/&/g, "&amp;").replace(/</g, "&lt;").replace(/>/g, "&gt;");
    const encodedMsg = user + " says " + msg;
    const li = document.createElement("li");
    li.textContent = encodedMsg;
    document.getElementById("Notificaciones").appendChild(li);
});
Object.defineProperty(WebSocket, 'OPEN', { value: 1, });
connection.start(WebSocket.OPEN).catch(err => console.error(err.toString()));

/*document.getElementById("btnNoti").addEventListener("click", event => {
    const user ="mario boscatto";
    const message = "Ingreso al sistema";
    connection.invoke("SendMessage", user, message).catch(err => console.error(err.toString()));
    event.preventDefault();
});*/