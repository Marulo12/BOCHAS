$(document).ready(function() {

    $("#BtnValida").click(function () {
        TraerCanchas();
    });

});

function TraerCanchas() {


        $.ajax({
            type: "GET",
            url: "/AlquilerCanchas/MostrarCanchas",            
            success: function (response) {
                
                var table = $("#Canchas");
                var tr = "";
                for (var i = 0; i < response.length; i++) {

                    tr += '<tr><td style="display:none">' + response[i].id + '</td><td>' + response[i].numero + '</td><td>' + response[i].nombre + '</td><td>' + response[i].tipo + '</td><td><input type="checkbox"/></td></tr>';
                }

                table.html(tr);
                
            }

        });



}