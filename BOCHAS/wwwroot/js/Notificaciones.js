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
hub.on('join', user);
connection.start(
    function () {

        hub.invoke('join', '');

    });


$.ajax({
    type: "GET",
    url: "/Sessions/MostrarTotalSesiones",    
    success: function (response) {

        $("#TotalSesiones").text(response);
    }

});