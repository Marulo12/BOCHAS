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

function ValidarContraseñas(contranueva, contraconfirma) {
    if (contranueva === contraconfirma) {
        return true;

    }
    else {
        alertify.error("Las contraseñas no coinciden");
        return false;
    }
}