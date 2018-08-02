


$(document).ready(function() {

    $("#BtnValida").click(function () {

        TraerCanchas();
    });
    $("#BtnOcupadas").click(function () {

        MostrarHorariosOcupados();
    });

});


function MostrarHorariosOcupados() {
    var fecR = $("#FecR").val();
    if (fecR != "") {
        $.ajax({
            type: "GET",
            data: { fecR },
            url: "/Agenda/MostrarHorariosOcupados",
            success: function (response) {
                $("#ModalHorariosBody").html(response);
                $("#TablaHorariosOcupados").DataTable({
                    searching: true,
                    pageLength: 50,
                    responsive: true,
                    search: "Filtro&nbsp;:",
                    language: {
                        processing: "Procesando",
                        search: "Filtro&nbsp;:",
                        info: "Pagina _PAGE_ de _PAGES_  / <b>Total de Registros: _MAX_</b> ",
                        infoEmpty: "",
                        infoFiltered: "",
                        zeroRecords: "Ningun registro coincide",
                        lengthMenu: "Mostrar _MENU_ registros",
                        infoPostFix: "",
                        loadingRecords: "Cargando...",
                        emptyTable: "No hay registros",
                        paginate: {
                            first: "Primero",
                            previous: "Anterior",
                            next: "Siguiente",
                            last: "Ultimo"
                        }
                    }
                });
                $("#ModalHorarios").modal();
              
            },
            failure: function (response) {
                alert(response);
            }

        });
    } else { alertify.error("Para consultar horarios tiene que incorporar una fecha de reserva"); }
}

function TraerCanchas() {
    if (ComprobarCamposDates()) {
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
                $("#ImgLoad").css("display", "none");
                if (response === "VACIO") {
                    alertify.alert("Alerta","No hay canchas disponibles para ese horario");
                } else {
                  
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
function ComprobarCamposDates() {
    
   
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

        if ($("#HH").val() <= $("#HD").val()) {
            alertify.error("La Hora hasta no puede ser menor o igual que la hora desde");
            return false;
        }
        return true;

    }


