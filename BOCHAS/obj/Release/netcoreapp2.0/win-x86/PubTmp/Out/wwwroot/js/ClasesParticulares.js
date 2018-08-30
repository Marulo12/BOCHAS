$(document).ready(function () {

    $('[data-toggle="tooltip"]').tooltip();
    $("#BtnValida").click(function () {

        TraerCanchas();
    });
    $("#BtnOcupadas").click(function () {

        MostrarHorariosOcupados();
    });
    $("#BtnConPart").click(function () {
        ConsultarClases();
    });

    $("#RegistrarClase").click(function () {
        RegistrarClase();
    });
    $(".BtnCancelaR").click(function (event) {
        $("#ModalMail").modal();
    });
    MensajesdeAcciones();
});

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
            url: "/ClaseParticulars/MostrarCanchas",
            success: function (response) {
                $("#FormCancha").hide();
                $("#ListCancha").empty();
                $("#ImgLoad").css("display", "none");          
                if (response === "VACIO") {
                    alertify.alert("Alerta", "No hay canchas disponibles para ese horario");
                } else {

                    var table = $("#ListCancha");
                    var tr = "";
                    for (var i = 0; i < response.length; i++) {

                        tr += '<option value="' + response[i].id  + '">' + 'Nro: ' + response[i].numero + ' Nombre: ' + response[i].nombre + ' Descripcion: ' + response[i].descripcion + '</option>';
                    }

                    table.html(tr);

                    $("#FormCancha").show('slow');
                }
            },
            failure: function (response) {
                alert(response);
            }

        });
    }
}

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

function RegistrarClase() {
    if ($("#IdCliente").val() === "") {
        alert.error("Seleccione un Jugador");
        return;
    }
       
    if (ComprobarCamposDates()) {
        var FechaReserva = $("#FecR").val();
        var HoraInicio = $("#HD").val();
        var HoraFin = $("#HH").val();
        var IdJugador = $("#IdCliente option:selected").val();
        var IdProfesor = $("#IdProfesor option:selected").val();
        var IdCancha = $("#ListCancha option:selected").val();
        var Obs = $("#Obs").val();
        $("#ImgLoad").css("display", "inline-block");
        $("#FormCancha").hide();
        $.ajax({
            type: "POST",
            data: { IdJugador, IdProfesor, FechaReserva, HoraInicio, HoraFin, IdCancha, Obs },
            url: "/ClaseParticulars/RegistrarClase",
            success: function (response) {

              //  $("#Canchas").empty();
                $("#ImgLoad").css("display", "none");
                
                if (response !== "ERROR") {

                    alertify
                        .alert("Notificacion","Clase particular registrada con exito", function () {
                            window.location = "/ClaseParticulars/Index";
                        });
                }
                else {

                    alertify.error("Error en la operacion");
                }
            },
            failure: function (response) {
                alertify.error(response);
            }

        });

    }


}

function ConsultarClases() {
    var IdJugador = $("#IdCliente option:selected").val();
    var fechaD = $("#fechaD").val();
    var fechaH = $("#fechaH").val();
    $("#TablaCobros").empty();
    $("#ImgLoad").css("display", "inline-block");
    $.ajax({
        type: "GET",
        data: { IdJugador, fechaD, fechaH },
        url: "/ClaseParticulars/MostrarClases",
        success: function (response) {
            $("#ImgLoad").css("display","none");
            $("#TablaCobros").empty();
            $("#TablaCobros").html(response);
            $("#TconsClase").DataTable({
                searching: true,
                lengthMenu: [5, 10, 20, 75, 100],
                responsive: true,
                search: "Filtro&nbsp;:",
                dom: 'Bfrtip',
                buttons: [

                    {
                        extend: 'print',
                        text: 'Imprimir',
                        title: 'BOCHAS PADEL - Mis Reservas'

                    }
                ],
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
        }

    });
}


function VerDetalleClase(id) {
    $(".modal-footer .progress").css("display", "block");
    $("#DetalleClaseBody").empty();
    $("#ModalDetalleClaseP").modal();
    $.ajax({
        type: "GET",
        data: { id },
        url: "/ClaseParticulars/VerDetalle",
        success: function (response) {
            $("#DetalleClaseBody").html(response);           
            $(".modal-footer .progress").css("display", "none");
        },
        failure: function (response) {
            alert(response);
        }

    });

}

function MensajesdeAcciones() {

    if ($("#Resultado").val() === "SI") {
        alertify.success("Clase Confirmada!!");
    }
    if ($("#Resultado").val() === "COMENZADO") {
        alertify.success("Clase Comenzada!!");
    }
    if ($("#Resultado").val() === "Cancelado") {
        alertify.success("Clase Cancelada");
    }
    if ($("#Resultado").val() === "FINALIZADO") {
        alertify.success("Clase Finalizada!!");
        var Numero = $("#NClaseFinalizada").val();
        alertify.confirm('Realizar Cobro', 'Desea realizar el cobro de la clase?', function () {
            $.ajax({
                type: "GET",
                data: { Numero },
                url: "/Cobro/CobroClase",
                success: function (response) {
                    $("#ModalCobro").modal();
                    $("#ModalCobro .mb").html(response);

                }
            });
        }
            , function () { alertify.error('Operacion cancelada'); });
    }
    if ($("#Resultado").val() === "NO") {
        alertify.error("Error en la operacion");
    }
    if ($("#Resultado").val() === "NoMail") {
        alertify.error("Se cancelo la reserva pero no se mando mensaje al jugador");
    }
    $("#Resultado").val("");
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

function ReporteCobroClase(NCobro) {

    $("#ModalPdf").modal();
    $("#GeneraPDF").css("display", "inline");
    $("#VisorPDF").attr("src", "");
    $("#VisorPDF").css("display", "none");
    setTimeout(function () {
        $("#GeneraPDF").css("display", "none");
        $("#VisorPDF").attr("data", "/Reportes/ReporteCobroClase?NCobro=" + NCobro);
        $("#VisorPDF").css("display", "inline-block");
        $("#VisorPDF").css("width", "100%");
        $("#VisorPDF").css("height", "500px");

    }, 2000);
}