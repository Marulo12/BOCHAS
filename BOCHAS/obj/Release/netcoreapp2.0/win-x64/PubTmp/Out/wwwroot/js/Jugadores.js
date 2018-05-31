$(document).ready(function () {
    $("#altaJugador").after(function () {
        MostrarTipoDocumento(); MostrarLocalidades(); MostrarTipoJugador();
    });
    $("#ConsultaJugador").after(function () {
        MostrarJugador("");
    });
    $("#BtnBuscarJu").click(function () {
        MostrarJugador($("#FiltroJu").val());
    });

    $("#Localidad").change(function () {
        MostrarBarrio();
    });
    $("#Registrar").click(function () {
        ComprobarUsuario();
    });
    $("#Limpiar").click(function () {
        LimpiarCampos();
    });
    $("#Continuar").click(function () {
        window.location = "/Personas/RegistrarJugador";
    });

    $("#TablaJugadoresBaja").after(function () {
        $(".col-lg-12 img").hide();
        $("#TablaJugadoresBaja").show();
        $("#TablaJugadoresBaja").DataTable({
            responsive: true,

            "scrollX": true,
            searching: true,

            dom: 'Bfrtip',
            buttons: [
                {
                    extend: 'excel',
                    text: 'Excel',
                    title: 'BOCHAS PADEL - Jugadores Activos'
                },
                {
                    extend: 'pdfHtml5',
                    text: 'PDF',
                    title: 'BOCHAS PADEL - Jugadores Activos'
                },
                {
                    extend: 'print',
                    text: 'Imprimir',
                    title: 'BOCHAS PADEL - Jugadores Activos'

                }
            ],
            language: {
                processing: "Procesando",
                search: "Filtro&nbsp;:",
                info: "",
                infoEmpty: "",
                infoFiltered: "(Filtrado de _MAX_ total de registros)",
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


    });
    $("#BtnTipoJugador").click(function () {
        AgregarTipoJugador();
    });
});


function MostrarJugador(filtro) {
    $.ajax({
        type: "GET",
        url: "/Personas/MostrarJugadores",
        data: { filtro },
        contentType: "application/json; charset=utf-8",
        dataType: "json",
        success: function (response) {
            var dvItems = $("#CJugadores");
            dvItems.empty();
            var Table = '<table id="TablaJugadores" class="table table-striped  display" style="width:100%;" ><thead style="background-color: rgba(158, 44, 44, 0.9);color:white"><tr><th>Nombre</th><th>Apellido</th><th>Documento</th><th>Telefono</th><th>Mail</th><th ></th></tr></thead><tbody>';
            for (var i = 0; i < response.length; i++) {
                Table += '<tr ><td>' + response[i].nombre + '</td>' + '<td>' + response[i].apellido + '</td>' + '<td>' + response[i].documento + '</td>' + '<td>' + response[i].telefono + '</td>' + '<td>' + response[i].mail + '</td>' + '</td> <td><div class="btn-group" style="padding-left:17%;"> <button class=" btn btn-sm btn-primary " data-toggle="tooltip" title="Informacion adicional" data-placement="top"  onclick="ConocerDomicilio(' + response[i].id + ');"><i class="far fa-address-card"></i></button><button  class=" btn btn-sm BtnEditar" data-toggle="tooltip" title="Modificar"  onclick="EditarJugador(' + response[i].id + ');" data-placement="top" ><i class="fas fa-pencil-alt"></i></button><button class="btn btn-sm btn-danger" data-toggle="tooltip" title="Baja" data-placement="top" onclick="confirmarBaja(' + response[i].id + ');"><i class="fas fa-trash-alt"></i></button><button class="btn btn-sm btn-warning" data-toggle="tooltip" title="Agregar tipo de jugador" data-placement="top" onclick="AbrirModalTipoJugador(' + response[i].id + ');"><i class="fas fa-user-tag"></i></button></div></td></tr>';
            }
            Table += "</tbody><tfoot></tfoot></table>";
            dvItems.append(Table);
            $("#TablaJugadores").DataTable({
                searching: true,
                "scrollX": true,
                responsive: true,
                search: "Filtro&nbsp;:",
                dom: 'Bfrtip',
                buttons: [
                    {
                        extend: 'excel',
                        text: 'Excel',
                        title: 'BOCHAS PADEL - Jugadores dados de baja'
                    },
                    {
                        extend: 'pdfHtml5',
                        text: 'PDF',
                        title: 'BOCHAS PADEL - Jugadores dados de baja'

                    },
                    {
                        extend: 'print',
                        text: 'Imprimir',
                        title: 'BOCHAS PADEL - Jugadores dados de baja'

                    }
                ],
                language: {
                    processing: "Procesando",
                    search: "Filtro&nbsp;:",
                    info: "",
                    infoEmpty: "",
                    zeroRecords: "Ningun registro coincide",
                    infoFiltered: "(Filtrado de _MAX_ total de registros)",
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


            $('[data-toggle="tooltip"]').tooltip();
        },
        failure: function (response) {
            alert(response);
        }

    });

}
function AbrirModalTipoJugador(id) {
    $.ajax({
        type: "GET",
        contentType: "application/json; charset=utf-8",
        dataType: "json",
        url: "/Personas/MostrarTipoJugador",
        success: function (response) {
            var rows;
            var dvItems = $("#CmbModalTipoJugador");
            dvItems.empty();
            for (var i = 0; i < response.length; i++) {
                rows += '<option value="' + response[i].id + '">' + response[i].nombre + '</option>';
            }
            $("#IdPersonaTipoJugador").val(id);
            dvItems.append(rows);
            $("#ModalTipoJugador").modal();
        },
        failure: function (response) {
            alert(response);
        }
    });
}
function AgregarTipoJugador() {
    var tipoJugador = $("#CmbModalTipoJugador option:selected").val();
    var IdPersona = $("#IdPersonaTipoJugador").val();

    $.ajax({
        type: "POST",
        url: "/Personas/AgregarTipoJugador",
        data: { IdPersona, tipoJugador },

        success: function (response) {
            if (response === "OK") {
                alertify.success("Se agrego el tipo de jugador con exito");

                $("#ModalTipoJugador").modal('hide');
            }
            else {
                alertify.error("Ya existe ese tipo para esa persona");
                $("#ModalTipoJugador").modal('hide');
            }
        },
        failure: function (response) {
            alertify.error(response);
        }
    });

}

function EditarJugador(id) {
    $.ajax({
        type: "GET",
        url: "/Personas/EditarJugador",
        data: { id: id },
        success: function (response) {
            $("#ModalBodyEdicion").html(response);
            $('#ModalEdicion').modal();
        }
    });



}

function ConocerDomicilio(id) {
    var idP = id;
    $.ajax({
        type: "GET",
        url: "/Personas/ConocerDomicilio",
        data: { IdPersona: id },
        contentType: "application/json; charset=utf-8",
        dataType: "json",
        success: function (response) {
            var rows;
            var dvItems = $("#ModalBodyDomicilio");
            $("#ModalCalle").val();
            $("#ModalNumero").val();
            $("#ModalLocalidad").val();
            $("#ModalBarrio").val();
            $("#ModalUsuario").val();
            $("#ModalContra").val();
            $("#ModalDpto").val();
            $("#ModalPiso").val();
            ConocerTipoJugador(idP);
            for (var i = 0; i < response.length; i++) {
                $("#ModalCalle").val(response[i].calle);
                $("#ModalNumero").val(response[i].numero);
                $("#ModalLocalidad").val(response[i].localidad);
                $("#ModalBarrio").val(response[i].barrio);
                $("#ModalUsuario").val(response[i].usuario);
                if (response[i].piso == "0") {
                    $("#ModalPiso").val("");
                }
                else {
                    $("#ModalPiso").val(response[i].piso);
                }

                $("#ModalDpto").val(response[i].dpto);



            }
            $('#ModalDomicilio').modal();
        },
        failure: function (response) {
            alert(response);
        }
    });
}

function ConocerTipoJugador(id) {

    $.ajax({

        type: "GET",
        url: "/Personas/ConocerTipoJugador",
        data: { id },
        contentType: "application/json; charset=utf-8",
        dataType: "json",
        success: function (response) {
            var li = "";
            for (var i = 0; i < response.length; i++) {
                li += '<li style="width:100%;"><i class="fas fa-angle-right"></i>' + response[i].nombre + '</li>';
            }
            $("#ListActividades").html(li);
        }

    });

}

function MostrarTipoJugador() {
    $.ajax({
        type: "GET",
        contentType: "application/json; charset=utf-8",
        dataType: "json",
        url: "/Personas/MostrarTipoJugador",
        success: function (response) {
            var dvItems = $("#TipoJugador");
            var rows = '<div class="checkbox">';
            for (var i = 0; i < response.length; i++) {
                rows += '<label><input type="checkbox" name="TipoJugador" value="' + response[i].id + '">' + response[i].nombre + '</label>&nbsp;';

            }
            rows += "</div>";
            dvItems.html(rows);
        },
        failure: function (response) {
            alert(response);
        }
    });
}
function MostrarTipoDocumento() {
    $.ajax({
        type: "GET",
        contentType: "application/json; charset=utf-8",
        dataType: "json",
        url: "/Personas/MostrarTipoDocumento",
        success: function (response) {
            var rows;
            var dvItems = $("#TipoDoc");
            dvItems.empty();
            for (var i = 0; i < response.length; i++) {
                rows += '<option value="' + response[i].id + '">' + response[i].nombre + '</option>';
            }
            $('#TipoDoc').append(rows);
        },
        failure: function (response) {
            alert(response);
        }
    });
}
function MostrarBarrio() {

    $.ajax({
        type: "GET",
        url: "/Personas/MostrarBarrios",
        data: { IdLocalidad: + $("#Localidad option:selected").val() },
        contentType: "application/json; charset=utf-8",
        dataType: "json",
        success: function (response) {
            var rows;
            var dvItems = $("#Barrio");
            dvItems.empty();
            for (var i = 0; i < response.length; i++) {
                rows += '<option value="' + response[i].id + '">' + response[i].nombre + '</option>';
            }
            $('#Barrio').append(rows);

        },
        failure: function (response) {
            alert(response);
        }
    });

}
function MostrarLocalidades() {

    $.ajax({
        type: "GET",
        contentType: "application/json; charset=utf-8",
        dataType: "json",
        url: "/Personas/MostrarLocalidades",
        success: function (response) {
            var rows;
            var dvItems = $("#Localidad");
            dvItems.empty();
            for (var i = 0; i < response.length; i++) {
                rows += '<option value="' + response[i].id + '">' + response[i].nombre + '</option>';
            }
            $('#Localidad').append(rows);
            MostrarBarrio();
        },
        failure: function (response) {
            alert(response);
        }
    });
}
function confirmarBaja(id) {
    alertify.confirm('Confirmar', 'Dar de baja al Jugador?', function () {
        alertify.success("Baja de Jugador dada con exito");
        $.ajax({
            type: "POST",
            url: "/Personas/BajaJugador",
            data: { id: id },
            success: function (response) {

                window.location = "/Personas/ConsultarJugador";

            }
        });
    }
        , function () { alertify.error('Baja Cancelada') });

}
var usuarioExiste = false;
function ComprobarUsuario() {
    var usuario = $("#Usuario").val();

    $.ajax({
        type: "GET",
        url: "/Personas/ValidarUsuario",
        data: { usuario: usuario },
        success: function (response) {
            if (response === "False") {


                alertify.error('El nombre de usuario ya esta en uso');

            }
            if (response === "OK") {
                New();

            }

        },
        failure: function (response) { window.alert(response); }
    });



}
function New() {

    if (ComprobarCampos()) {
        var nombre = $("#Nombre").val();
        var apellido = $("#Apellido").val();
        var tipodoc = $("#TipoDoc option:selected").val();
        var numero = $("#Numero").val();
        var mail = $("#Mail").val();
        var telefono = $("#Telefono").val();
        var localidad = $("#Localidad option:selected").val();
        var barrio = $("#Barrio option:selected").val();
        var usuario = $("#Usuario").val();
        var contra = $("#Contra").val();
        var calle = $("#Calle").val();
        var ncalle = $("#NCalle").val();
        var tipojugador = new Array();
        $("input:checkbox:checked").each(function () {
            tipojugador.push($(this).val());
        });

        var dpto = $("#Dpto").val();
        var piso = $("#Piso").val();
        $("#PanelJugadores").css("display", "none");
        $("#DivCarga").css("display", "inline");
        $.ajax({
            type: "POST",
            url: "/Personas/NewJugador",
            data: { nombre, apellido, tipodoc, numero, mail, telefono, localidad, barrio, usuario, contra, calle, tipojugador, ncalle, dpto, piso },
            success: function (response) {
                if (response === "OK") {
                    $("#DivCarga").css("display", "none");

                    alertify.alert('Alerta', 'Jugador cargado con exito').set('onok', function (closeEvent) { window.location = "/Personas/RegistrarJugador"; });
                }
                if (response === "ERROR") {
                    alertify.alert('Alerta', 'Ocurrio un error en la operacion..').set('onok', function (closeEvent) { window.location = "/Personas/RegistrarJugador"; });
                    $("#DivCarga").css("display", "none");
                    $("#PanelJugadores").css("display", "inline");
                }
                if (response === "EXISTE") {
                    alertify.alert('Alerta', 'Ocurrio un error en la operacion..').set('onok', function (closeEvent) { window.location = "/Personas/RegistrarJugador"; });
                    $("#DivCarga").css("display", "none");
                    $("#PanelJugadores").css("display", "inline");
                }

            },
            failure: function (response) {

                $("#DivCarga").css("display", "none");
                $("#PanelJugadores").css("display", "inline");
                alertify.alert('Alerta', 'Ocurrio un error en la operacion..').set('onok', function (closeEvent) { window.location = "/Personas/RegistrarJugador"; });
            }
        });
    }


}
function ComprobarCampos() {
    if ($("#Nombre").val() === "") {

        alertify.error('No cargo el Nombre');
        return false;
    }
    if ($("#Apellido").val() === "") {
        alertify.error('No cargo el Apellido');
        return false;
    }
    if ($("#Numero").val() === "") {
        alertify.error('No cargo el Numero de Documento');
        return false;
    }
    const tipos = document.querySelectorAll('input[type=checkbox]:checked');
    if (tipos.length <= 0) {
        alertify.error("Marque una opcion tipo de jugador");
        return false;
    }
    if ($("#Mail").val() === "") {
        alertify.error('No cargo el Mail');
        return false;
    }
    if ($("#Telefono").val() === "") {
        alertify.error('No cargo el Telefono');
        return false;
    }
    if ($("#Usuario").val() === "") {
        alertify.error('No cargo el Nombre de Usuario');
        return false;
    }
    if ($("#Contra").val() === "") {
        alertify.error('No cargo la Contraeña');
        return false;
    }
    if ($("#Calle").val() === "") {
        alertify.error('No cargo la Calle');;
        return false;
    }
    if ($("#NCalle").val() === "") {
        alertify.error('No cargo el Numero de Calle');
        return false;
    }

    return true;
}
function LimpiarCampos() {
    window.location = "/Personas/RegistrarJugador";
}
