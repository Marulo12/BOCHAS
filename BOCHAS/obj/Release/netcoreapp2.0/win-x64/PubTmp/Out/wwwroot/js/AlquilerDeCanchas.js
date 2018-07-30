
$(document).ready(function() {

    $("#BtnValida").click(function () {

        TraerCanchas();
    });

});

function TraerCanchas() {
    if (ComprobarCampos()) {
        $("#Canchas").empty();
        $("#ImgLoad").css("display", "inline-block");
        $.ajax({
            type: "GET",
            url: "/AlquilerCanchas/MostrarCanchas",
            success: function (response) {

                var table = $("#Canchas");
                var tr = "";
                for (var i = 0; i < response.length; i++) {
                    $("#ImgLoad").css("display", "none");
                    tr += '<tr><td style="display:none">' + response[i].id + '</td><td>' + response[i].numero + '</td><td>' + response[i].nombre + '</td><td>' + response[i].tipo + '</td><td><input type="checkbox"/></td></tr>';
                }

                table.html(tr);

            }

        });
    }
}
function ComprobarCampos() {
  
        if ($("#FecR").val() == "") {
            alertify.error("Complete el campo fecha de Reserva");
            return false;
        }
        if ($("#HD").val() == "") {
            alertify.error("Complete el campo Hora Desde");
            return false;
        }
        if ($("#HH").val() == "") {
            alertify.error("Complete el campo Hora Hasta");
            return false;
        }

        if ($("#HH").val() < $("#HD").val()) {
            alertify.error("La Hora hasta no puede ser menor que la hora desde");
            return false;
        }
        return true;

    }


