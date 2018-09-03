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

$.ajax({
    type: "GET",
    url: "/Sessions/MostrarTotalSesiones",    
    success: function (response) {

        $("#TotalSesiones").text(response);
    }

});

$.ajax({
    type: "GET",
    url: "/AlquilerCanchas/TraerReservasPorDia",
    success: function (response) {

        $("#BodyTReservaPorDia").empty();
        var label = "";
        for (var i = 0; i < response.length; i++)
        {
            
            switch (response[i].ide) {
                case 2:
                    label = "label-warning";
                    break;
                case  3:
                    label = "label-info";
                    break;
                case 4:
                    label = "label-success";
                    break;
                case 5:
                    label = "label-danger";
                    break;
            }

            $("#BodyTReservaPorDia").append('<tr><td>' + response[i].numero + '</td><td>' + response[i].nombre + '</td><td><label class="label ' + label + ' " style="font-size:12px;">' + response[i].estado + '</label></td></tr>');
        }
    }

});

$.ajax({
    type: "GET",
    url: "/AlquilerCanchas/MostrarPorcentajeReservas",
    success: function (response) {
        $("#PorcentajeReservas").text(response + "%");
    }

});

$.ajax({
    type: "GET",
    url: "/ClaseParticulars/FinalizadasMensual",
    success: function (response) {
       
        $("#TotalClases").text(response);
    }

});