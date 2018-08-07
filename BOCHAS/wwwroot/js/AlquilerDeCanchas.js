
$(document).ready(function () {
    AutoCompletarCombo();
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

    $("#TablaReservasCons").ready(function () {
        $("#ImgLoad").css('display','none');
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
        $("#TablaReservasCons").removeAttr('style');
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

    if ($("#Respuesta").val()==="SI") {
        alertify.success("Reserva Confirmada!!");
    }
    if ($("#Respuesta").val() === "Cancelado") {
        alertify.success("Reserva Cancelada");
    }
    if ($("#Respuesta").val() === "NO") {
        alertify.error("Error en la operacion");
    }
    $("#BtnConPart").click(function () {
        ConsultaParticular();
    });
   
});

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
    return true;

}

function ConsultaParticular() {
    var nombreP = $("#nombreP").val();
    var apellidoP = $("#apellidoP").val();
    if (nombreP === "" || apellidoP === "" ) {
        alertify.error("Complete los campos de consulta");
       
    } else {
        $("#TablaConPar").empty();
        $("#ImgLoad").css("display", "inline-block");
        $.ajax({
            type: "GET",
            data: {nombreP,apellidoP},
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

        }); }
}
function VerDetalleReserva(numero) {

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
            $("#ModalDetalleReserva").modal();
        },
        failure: function (response) {
            alert(response);
        }

    });

}

function AutoCompletarCombo() {

    $.widget("custom.combobox", {
        _create: function () {
            this.wrapper = $("<span>")
                .addClass("custom-combobox")
                .insertAfter(this.element);

            this.element.hide();
            this._createAutocomplete();
            this._createShowAllButton();
        },

        _createAutocomplete: function () {
            var selected = this.element.children(":selected"),
                value = selected.val() ? selected.text() : "";

            this.input = $("<input>")
                .appendTo(this.wrapper)
                .val(value)
                .attr("title", "")
                .addClass("custom-combobox-input ui-widget ui-widget-content ui-state-default ui-corner-left")
                .autocomplete({
                    delay: 0,
                    minLength: 0,
                    source: $.proxy(this, "_source")
                })
                .tooltip({
                    classes: {
                        "ui-tooltip": "ui-state-highlight"
                    }
                });

            this._on(this.input, {
                autocompleteselect: function (event, ui) {
                    ui.item.option.selected = true;
                    this._trigger("select", event, {
                        item: ui.item.option
                    });
                },

                autocompletechange: "_removeIfInvalid"
            });
        },

        _createShowAllButton: function () {
            var input = this.input,
                wasOpen = false;

            $("<a>")
                .attr("tabIndex", -1)
                .attr("title", "Mostrar todos los Jugadores")
                .tooltip()
                .appendTo(this.wrapper)
                .button({
                    icons: {
                        primary: "ui-icon-triangle-1-s"
                    },
                    text: false
                })
                .removeClass("ui-corner-all")
                .addClass("custom-combobox-toggle ui-corner-right")
                .on("mousedown", function () {
                    wasOpen = input.autocomplete("widget").is(":visible");
                })
                .on("click", function () {
                    input.trigger("focus");

                    // Close if already visible
                    if (wasOpen) {
                        return;
                    }

                    // Pass empty string as value to search for, displaying all results
                    input.autocomplete("search", "");
                });
        },

        _source: function (request, response) {
            var matcher = new RegExp($.ui.autocomplete.escapeRegex(request.term), "i");
            response(this.element.children("option").map(function () {
                var text = $(this).text();
                if (this.value && (!request.term || matcher.test(text)))
                    return {
                        label: text,
                        value: text,
                        option: this
                    };
            }));
        },

        _removeIfInvalid: function (event, ui) {

            // Selected an item, nothing to do
            if (ui.item) {
                return;
            }

            // Search for a match (case-insensitive)
            var value = this.input.val(),
                valueLowerCase = value.toLowerCase(),
                valid = false;
            this.element.children("option").each(function () {
                if ($(this).text().toLowerCase() === valueLowerCase) {
                    this.selected = valid = true;
                    return false;
                }
            });

            // Found a match, nothing to do
            if (valid) {
                return;
            }

            // Remove invalid value
            this.input
                .val("")
                .attr("title", value + "no hay coincidencias")
                .tooltip("open");
            this.element.val("");
            this._delay(function () {
                this.input.tooltip("close").attr("title", "");
            }, 2500);
            this.input.autocomplete("instance").term = "";
        },

        _destroy: function () {
            this.wrapper.remove();
            this.element.show();
        }
    });

    $("#IdCliente").combobox();
    $("#toggle").on("click", function () {
        $("#IdCliente").toggle();
    });
    

}