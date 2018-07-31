


$(document).ready(function() {

    $("#BtnValida").click(function () {

        TraerCanchas();
    });

});

function TraerCanchas() {
    if (ComprobarCampos()) {
        $("#Canchas").empty();
        $("#ImgLoad").css("display", "inline-block");
        var fecR = $("#FecR").val();
        var hd = $("#HD").val();
        var hh = $("#HH").val();
        $.ajax({
            type: "GET",
            data: { fecR, hd, hh },
            url: "/AlquilerCanchas/MostrarCanchas",
            success: function (response) {
                $("#Canchas").empty();
                if (response === "VACIO") {

                } else {
                    $("#ImgLoad").css("display", "none");
                    var table = $("#Canchas");
                    var tr = "";
                    for (var i = 0; i < response.length; i++) {

                        tr += '<tr><td style="display:none">' + response[i].id + '</td><td>' + response[i].numero + '</td><td>' + response[i].nombre + '</td><td>' + response[i].descripcion + '</td><td><input type="checkbox"/></td></tr>';
                    }

                    table.html(tr);}
            },
            failure: function (response) {
                alert(response);
            }

        });
    }
}
function ComprobarCampos() {
    
   
    if ($("#FecR").val() == "") {
            alertify.error("Complete el campo fecha de Reserva");
            return false;
    }
    if ($("#FecR").val() < $("#FecP").val()) {
        alertify.error("La fecha de reserva no puede ser menor que la fecha ACTUAL");
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


