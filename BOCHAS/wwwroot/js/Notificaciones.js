var connection = $.hubConnection(), hub = connection.createHubProxy('chat');
var user = function (user) {
    if (user.length === 0) {
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