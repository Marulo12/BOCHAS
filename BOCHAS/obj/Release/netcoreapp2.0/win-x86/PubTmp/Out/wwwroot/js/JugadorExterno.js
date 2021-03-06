﻿$(document).ready(function () {
    $("#altaJugador").after(function () {
        MostrarTipoDocumento(); MostrarLocalidades(); MostrarTipoJugador();
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

});
   


function MostrarTipoJugador() {
    $.ajax({
        type: "GET",
        contentType: "application/json; charset=utf-8",
        dataType: "json",
        url: "/InscripcionCuentaJugador/MostrarTipoJugador",
        success: function (response) {
            var dvItems = $("#TipoJugador");
            var rows = '<div class="checkbox">';
            for (var i = 0; i < response.length; i++) {

                if (response[i].id === 2) {
                    rows += '<label style="display:none;"><input type="checkbox" name="TipoJugador" value="' + response[i].id + '"  checked>' + response[i].nombre + '</label>';
                }
                else {
                    rows += '<label><input type="checkbox" name="TipoJugador" value="' + response[i].id + '"> Anotarme para Clases particulares (opcional)</label>';
                }
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
        url: "/InscripcionCuentaJugador/MostrarTipoDocumento",
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
        url: "/InscripcionCuentaJugador/MostrarBarrios",
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
        url: "/InscripcionCuentaJugador/MostrarLocalidades",
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

var usuarioExiste = false;
function ComprobarUsuario() {
    var usuario = $("#Usuario").val();

    $.ajax({
        type: "GET",
        url: "/InscripcionCuentaJugador/ValidarUsuario",
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
        var tipojugador = new Array();
        $("input:checkbox:checked").each(function () {
            tipojugador.push($(this).val());
        });
        var ncalle = $("#NCalle").val();       
        var dpto = $("#Dpto").val();
        var piso = $("#Piso").val();
       
        $.ajax({
            type: "POST",
            url: "/InscripcionCuentaJugador/NewJugador",
            data: { nombre, apellido, tipodoc, numero, mail, telefono, localidad, barrio, usuario, contra, calle, tipojugador, ncalle, dpto, piso },
            success: function (response) {
                if (response === "OK") {
                  

                    alertify.alert('Alerta', 'Cuenta creada con exito').set('onok', function (closeEvent) { window.location = "/Usuarios/Index"; });
                }
                if (response === "ERROR") {
                    alertify.alert('Alerta', 'Ocurrio un error en la operacion..').set('onok', function (closeEvent) { window.location = "/Usuarios/Index"; });
                   
                }
                if (response === "EXISTE") {
                    alertify.alert('Alerta', 'Lo siento esa persona ya tiene una cuenta activa').set('onok', function (closeEvent) { window.location = "/Usuarios/Index"; });
                   
                }

            },
            failure: function (response) {

                
                alertify.alert('Alerta', 'Ocurrio un error en la operacion..').set('onok', function (closeEvent) { window.location = "/Usuarios/Index"; });
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
    
    if ($("#Mail").val() === "") {
        alertify.error('No cargo el Mail');
        return false;
    }
    const tipos = document.querySelectorAll('input[type=checkbox]:checked');
    if (tipos.length <= 0) {
        alertify.error("Marque una opcion tipo de jugador");
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
    if ($("#Contra").val().length < 8) {
        alertify.error('La Contraeña tiene que tener mas de 8 caracteres');
        return false;
    }
    if ($("#Contra").val() === "") {
        alertify.error('No cargo la Contraeña');
        return false;
    }
    if ($("#Calle").val() === "") {
        alertify.error('No cargo la Calle');
        return false;
    }
    if ($("#NCalle").val() === "") {
        alertify.error('No cargo el Numero de Calle');
        return false;
    }

    return true;
    }

function LimpiarCampos() {
    window.location = "/Usuarios/Index";
    }
