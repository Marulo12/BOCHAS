$(document).ready(function () {

    $("#BtnCambiaContra").click(function () {

        VerificarContraseña();

    });

});

function CambiaContraseña(contraactual) {
    
    var contranueva = $("#PassNuevo").val();
    var contraconfirma = $("#PassConfirma").val();

    if (ValidarContraseñas(contranueva, contraconfirma)) {
        $.ajax({
            type: "POST",
            url: "/Usuarios/CambiarContraseña",
            data: { contraactual, contranueva },
            success: function (response) {
                if (response == "OK") {

                    alertify.success("Clave Modificada con exito");
                    $("#PassActual").val("");
                    $("#PassNuevo").val("");
                    $("#PassConfirma").val("");
                }
                else {
                    alertify.alert("Se produjo un error en la operacion");
                 
                }

            }
        });

    }


}

function  VerificarContraseña() {

    if (ComprobarCampos()) {
    var contraactual = $("#PassActual").val();
    $.ajax({
        type: "GET",
        url: "/Usuarios/VerificarContraseña",
        data: { contraactual },
        success: function (response) {
            if (response === "OK") {
                CambiaContraseña(contraactual);
            }
            else {
                alertify.error("la contraseña original no es valida");
   
            }
        }
    });
    }
   
}

function ValidarContraseñas(contranueva, contraconfirma) {
    if (contranueva === contraconfirma) {
        return true;

    }
    else {
        alertify.error("Las contraseñas no coinciden");
        return false;
    }
}

function ComprobarCampos() {
    if ($("#PassActual").val() === "") {

        alertify.error('Ingrese la contraseña actual');
        return false;
    }
    if ($("#PassNuevo").val() === "") {
        alertify.error('Ingrese la nueva contraseña');
        return false;
    }
    if ($("#PassConfirma").val() === "") {
        alertify.error('Ingrese la confirmacion de la contraseña');
        return false;
    }
   

    return true;
}