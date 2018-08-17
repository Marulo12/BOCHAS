

$(document).ready(function () {
   
    $('[data-toggle="tooltip"]').tooltip(); 
    $("#BtnValida").click(function () {

        TraerCanchas();
    });
    $("#BtnOcupadas").click(function () {

        MostrarHorariosOcupados();
    });
    $("#RegistrarReserva").click(function () {
        RegistrarReserva();

    });
    $("#RegistrarReservaJugador").click(function () {
        RegistrarReservaJugador();

    });
    $("#ImgLoad").css('display', 'none');
    $("#TablaReservasCons").css("display","table");
    $("#TablaReservasCons").DataTable({
            searching: true,
            pageLength: 10,
            lengthMenu: [5, 10, 20, 75, 100],
            responsive: true,
            search: "Filtro&nbsp;:",
            dom: 'Bfrtip',
            buttons: [

                {
                    extend: 'print',
                    text: 'Imprimir',
                    title: 'BOCHAS PADEL -  Alquiler de Canchas'

                }, {
                    extend: 'excel',
                    text: 'Excel',
                    title: 'BOCHAS PADEL - Alquiler de Canchas'
                },
                {
                    extend: 'pdfHtml5',
                    text: 'PDF',
                    title: 'BOCHAS PADEL - Alquiler de Canchas'

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
    $("#TablaMisReservas").DataTable({
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
    $(".BtnCancelaR").click(function (event) {
        $("#ModalMail").modal();
    });
    if ($("#Respuesta").val()==="SI") {
        alertify.success("Reserva Confirmada!!");
    }
    if ($("#Respuesta").val() === "COMENZADO") {
        alertify.success("Reserva Comenzada!!");
    }
    if ($("#Respuesta").val() === "Cancelado") {
        alertify.success("Reserva Cancelada");
    }
    if ($("#Respuesta").val() === "NO") {
        alertify.error("Error en la operacion");
    }
    if ($("#Respuesta").val() === "NoMail") {
        alertify.error("Se cancelo la reserva pero no se mando mensaje al jugador");
    }
    $("#BtnConPart").click(function () {
        ConsultaParticular();
    });
    
});


function NotificaReserva() {
    var connection = $.hubConnection(), hub = connection.createHubProxy('chat');

    connection.start(function () {

        hub.invoke('ReservasJugador');

    }).done(function () { }).fail(function (e) { alert(e); });
}

function RegistrarReserva() {
    if ($("#IdCliente").val() === "") {
        alert.error("Seleccione un Jugador");
        return;
    }

    var Canchas = new Array();
    $(".CanchasR:checked").each(function (index) {
        Canchas.push($(this).val());
    });
    if (ComprobarCamposDates() && Canchas.length > 0) {
        var fecR = $("#FecR").val();
        var hd = $("#HD").val();
        var hh = $("#HH").val();
        var Cliente = $("#IdCliente option:selected").val();
        $("#Canchas").empty();
        $("#ImgLoad").css("display", "inline-block");
        $.ajax({
            type: "POST",
            data: { fecR, hd, hh, Canchas, Cliente },
            url: "/AlquilerCanchas/RegistrarReserva",
            success: function (response) {

                $("#Canchas").empty();
                $("#ImgLoad").css("display", "none");
                if (response !== "ERROR") {

                    alertify.alert('Alerta', "Reserva Generada con exito con Numero: " + response, function () { window.location = "/AlquilerCanchas/NuevaReserva"; });
                }
                else {

                    alertify.error("Error en la operacion");
                }
            },
            failure: function (response) {
                alert(response);
            }

        });

    } else { alert.error("Seleccione una cancha"); }


}
function RegistrarReservaJugador() {
   
    var Canchas = new Array();
    $(".CanchasR:checked").each(function (index) {
        Canchas.push($(this).val());
    });
    if (ComprobarCamposDates() && Canchas.length > 0) {
        var fecR = $("#FecR").val();
        var hd = $("#HD").val();
        var hh = $("#HH").val();
        $("#Canchas").empty();
        $("#ImgLoad").css("display", "inline-block");
        $.ajax({
            type: "POST",
            data: { fecR, hd, hh, Canchas },
            url: "/AlquilerCanchas/RegistrarReservaJugador",
            success: function (response) {

                $("#Canchas").empty();
                $("#ImgLoad").css("display", "none");
                if (response !== "ERROR") {

                 
                    NotificaReserva();
                   

                    alertify.alert('Alerta', "Reserva Generada con exito con Numero de reserva: " + response + ", para ver si la reserva se confirmo verifique la misma en 'Mis Reservas'", function () { window.location = "/AlquilerCanchas/NuevaReservaJugador"; });
                }
                else {

                    alertify.error("Error en la operacion");
                }
            },
            failure: function (response) {
                alert(response);
            }

        });

    } else { alert.error("Seleccione una cancha"); }


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
                    alertify.alert("Alerta", "No hay canchas disponibles para ese horario");
                } else {

                    var table = $("#Canchas");
                    var tr = "";
                    for (var i = 0; i < response.length; i++) {

                        tr += '<tr><td style="display:none">' + response[i].id + '</td><td>' + response[i].numero + '</td><td>' + response[i].nombre + '</td><td>' + response[i].descripcion + '</td><td><input type="checkbox" class="CanchasR" value="' + response[i].id + '" /></td></tr>';
                    }

                    table.html(tr);
                }
            },
            failure: function (response) {
                alert(response);
            }

        });
    }
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

function ConsultaParticular() {
    var Jugador = $("#IdCliente option:selected").val();
    var fechaD = $("#fechaD").val();
    var fechaH = $("#fechaH").val();
    if (fechaD != "" && fechaH === "") {
        alertify.error("Complete el intervalo de fechas");
        return;
    } if (fechaD === "" && fechaH != "") {
        alertify.error("Complete el intervalo de fechas");
        return;
    }
    if (fechaD > fechaH ) {
        alertify.error("La fecha desde no puede ser mayor que la fecha hasta");
        return;
    }
        $("#TablaConPar").empty();
        $("#ImgLoad").css("display", "inline-block");
        $.ajax({
            type: "GET",
            data: {Jugador,fechaD,fechaH},
            url: "/AlquilerCanchas/ConsultaReservaParticular",
            success: function (response) {
                $("#ImgLoad").css("display", "none");
                $("#TablaConPar").html(response);
                $("#TablaReservasConsP").DataTable({
                    searching: true,
                    lengthMenu: [5, 10, 20, 75, 100],
                    responsive: true,
                    search: "Filtro&nbsp;:",
                    dom: 'Bfrtip',
                    buttons: [

                        {
                            extend: 'excel',
                            text: 'Excel',
                            title: 'BOCHAS PADEL - Reservas'
                        },
                        {
                            extend: 'pdfHtml5',
                            text: 'PDF',
                            title: 'BOCHAS PADEL - Reservas'
                        },
                        {
                            extend: 'print',
                            text: 'Imprimir',
                            title: 'BOCHAS PADEL - Reservas'

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
            },
            failure: function (response) {
                alert(response);
            }

        }); 
}
function VerDetalleMiReserva(numero) {
    $("#ModalDetalleReserva").modal();
    $.ajax({
        type: "GET",
        data: { numero },
        url: "/AlquilerCanchas/VerDetalleMiReserva",
        success: function (response) {
            $("#DetalleReservaBody").html(response);
            $("#TablaDetalleReserva").DataTable({
                searching: true,
                lengthMenu: [5, 10, 20, 75, 100],
                responsive: true,
                search: "Filtro&nbsp;:",
               
               
                language: {
                    processing: "Procesando",
                    search: "Filtro&nbsp;:",
                    info: "",
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
            }
            );
           
        },
        failure: function (response) {
            alert(response);
        }

    });

}
function VerDetalleReserva(numero) {
    $("#ModalDetalleReserva").modal();
    $.ajax({
        type: "GET",
        data: { numero },
        url: "/AlquilerCanchas/VerDetalle",
        success: function (response) {
            $("#DetalleReservaBody").html(response);
            $("#TablaDetalleReserva").DataTable({
                searching: true,
                lengthMenu: [5, 10, 20, 75, 100],
                responsive: true,
                search: "Filtro&nbsp;:",


                language: {
                    processing: "Procesando",
                    search: "Filtro&nbsp;:",
                    info: "",
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
            }
            );

        },
        failure: function (response) {
            alert(response);
        }

    });

}

function generaRepo(numero) {
    $("#ModalPdf").modal();
    $("#GeneraPDF").css("display", "inline");
    $("#VisorPDF").attr("src", "");
    $("#VisorPDF").css("display", "none");
    setTimeout(function () {
        $("#GeneraPDF").css("display", "none");
        $("#VisorPDF").attr("data", "/Reportes/ReporteReserva?Nreserva=" + numero);
        $("#VisorPDF").css("display", "inline-block");
        $("#VisorPDF").css("width", "100%");
        $("#VisorPDF").css("height", "500px");

    }, 2000);
    
   
   

}
