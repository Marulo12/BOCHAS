$(document).ready(function () {
    $("#NuevaReservaJugador").click(function () {
        
        connection.start().catch(err => console.error(err.toString()));
        connection.invoke("UsuariosConectados", $("#UsuarioConectado").val()).catch(err => console.error(err.toString()));

    });
});


const connection = new signalR.HubConnectionBuilder()
    .withUrl("/chatHub")
    .build();

connection.on("UsuariosConectado", (usuario) => {
    window.alert(JSON.stringify(usuario));
    for (var i = 0; i < usuario.length; i++) {

        const li = document.createElement("li");
        li.textContent = usuario[i].user + " - Esta conectado al sistema";
        document.getElementById("Notificaciones").appendChild(li);
    }
    $("#badgeNoti").html(usuario.length);
    $("#badgeNoti").css("display","block");
});
Object.defineProperty(WebSocket, 'OPEN', { value: 1, });
connection.start().catch(err => console.error(err.toString()));

/*document.getElementById("btnNoti").addEventListener("click", event => {
    const user ="mario boscatto";
    const message = "Ingreso al sistema";
    connection.invoke("SendMessage", user, message).catch(err => console.error(err.toString()));
    event.preventDefault();
});*/