var connection = $.hubConnection(), hub = connection.createHubProxy('chat');
var user = function (user) {
    if (user.length === 0 ) {
        $("#Sig").empty();
        $("#Sig").append('<li><a asp-action="Index" asp-controller="Sessions" class="notification-item"><span class="dot bg-danger"></span> No hay jugadores  Conectados</a></li>');
    }
    else {
        $("#Sig").empty();
        for (var i = 0; i < user.length; i++) {

            $("#Sig").append('<li><a asp-action="Index" asp-controller="Sessions" class="notification-item"><span class="dot bg-success"></span>' + user[i].us + ' Conectado</a></li>');
        }
    }   
}
var reservaJ = function (reservaJ) {
    $("#Listareserva").empty();
    if (reservaJ.length === 0) {
        $("#Lreserva").html("0");
        $("#Listareserva").html('<li><a asp-action="Index" asp-controller="Sessions" class="notification-item"><span class="dot bg-danger"></span> No hay reservas Confirmadas en el dia de hoy</a></li>');
    }
    else {
        $("#Lreserva").html(reservaJ.length);
        for (var i = 0; i < reservaJ.length; i++) {

            $("#Listareserva").append('<li><a asp-action="Index" asp-controller="Sessions" class="notification-item"><span class="dot bg-success"></span> Reserva Confirmada N°' + reservaJ[i].Numero + ', por ' + reservaJ[i].Nombre + " " + reservaJ[i].Apellido + '</a></li>');
        }
       
    }  
}



hub.on('join', user);
hub.on('ReservasJugador', reservaJ);
connection.start(
    function () {
        hub.invoke('join', '');
        hub.invoke('ReservasJugador');
    });


$.ajax({
    type: "GET",
    url: "/Sessions/MostrarTotalSesiones",    
    success: function (response) {

        $("#TotalSesiones").text(response);
    }

});