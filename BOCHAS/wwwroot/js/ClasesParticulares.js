$(document).ready(function () {

    $('[data-toggle="tooltip"]').tooltip();
    $("#BtnValida").click(function () {

        TraerCanchas();
    });
    $("#BtnOcupadas").click(function () {

        MostrarHorariosOcupados();
    });
    
    
    

});



function MostrarHorariosOcupados() {
    var fecR = $("#FecR").val();
    if (fecR !== "") {
        $("#ModalHorarios").modal();
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
               

            },
            failure: function (response) {
                alert(response);
            }

        });
    } else { alertify.error("Para consultar horarios tiene que incorporar una fecha de reserva"); }
}

function ComprobarCamposDates() {

    if ($("#FecR").val() === "") {
        alertify.error("Complete el campo fecha de Reserva");
        return false;
    }
    if ($("#FecR").val() < $("#FecP").val()) {
        alertify.error("La fecha de reserva no puede ser menor que la fecha ACTUAL");
        return false;
    }

    if ($("#HD").val() === "") {
        alertify.error("Complete el campo Hora Desde");
        return false;
    }
    if ($("#HH").val() === "") {
        alertify.error("Complete el campo Hora Hasta");
        return false;
    }

    if ($("#HH").val() <= $("#HD").val()) {
        alertify.error("La Hora hasta no puede ser menor o igual que la hora desde");
        return false;
    }

    if ($("#FecR").val() === $("#FecP").val()) {
        if ($("#HD").val() < $("#HR").val()) {
            alertify.error("La hora desde no puede ser menor a la hora actual");
            return false;
        }
    }
    return true;
}