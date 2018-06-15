

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
    $("#Notificaciones").empty();
                 
    for (var i = 0; i < usuario.length; i++) {

        const li = document.createElement("li");
        li.textContent = usuario[i].user + " - Esta conectado";
        document.getElementById("Notificaciones").appendChild(li);
    }
   
    $("#badgeNoti").html(usuario.length);
    $("#btnNoti").css("display","inline");
});
Object.defineProperty(WebSocket, 'OPEN', { value: 1, });
connection.start().catch(err => console.error(err.toString()));

