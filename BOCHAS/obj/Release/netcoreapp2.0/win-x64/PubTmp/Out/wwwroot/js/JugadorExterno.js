$(document).ready(function () {
    $("#altaJugador").after(function () {
        MostrarTipoDocumento(); MostrarLocalidades(); 
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




function MostrarTipoDocumento() {
    $.ajax({
        type: "GET",
        contentType: "application/json; charset=utf-8",
        dataType: "json",
        url: "/Home/MostrarTipoDocumento",
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
        url: "/Home/MostrarBarrios",
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
        url: "/Home/MostrarLocalidades",
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
        url: "/Home/ValidarUsuario",
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
        var dpto = $("#Dpto").val();
        var piso = $("#Piso").val();
        window.alert(nombre + apellido + tipodoc + numero + mail + telefono + barrio + usuario + contra + calle + ncalle + dpto + piso );
        $.ajax({
            type: "POST",
            url: "/Home/NewJugador",
            data: { nombre, apellido, tipodoc, numero, mail, telefono, localidad, barrio, usuario, contra, calle, ncalle, dpto, piso },
            success: function (response) {
                if (response === "OK") {
                  

                    alertify.alert('Alerta', 'Cuenta creada con exito').set('onok', function (closeEvent) { window.location = "/Usuarios/Index"; });
                }
                if (response === "ERROR") {
                    alertify.alert('Alerta', 'Ocurrio un error en la operacion..').set('onok', function (closeEvent) { window.location = "/Usuarios/Index"; });
                   
                }
                if (response === "EXISTE") {
                    alertify.alert('Alerta', 'Ocurrio un error en la operacion..').set('onok', function (closeEvent) { window.location = "/Usuarios/Index"; });
                   
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
