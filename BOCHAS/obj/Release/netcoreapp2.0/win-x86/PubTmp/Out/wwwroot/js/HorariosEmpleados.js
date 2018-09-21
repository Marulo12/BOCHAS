$(document).ready(function () {
    $("#Limpiar").click(function () {

        window.location = "/Personas/HorariosEmpleados";
    });

});

function RegistrarHorarios() {
var profesor = $("#IdProfesor option:selected").val();
    var turno = [];
    var selec = 0;
    var seguir = true;
       $(".H").each(function () {    
        var tr = $(this).closest('tr');            
        var turnos = $(tr).find('td:nth-child(1) label').text();
            var horaD = $(tr).find('td:nth-child(2) .H').val();
        var horaH = $(tr).find('td:nth-child(3) .H').val();
    if (validarHorarios(horaD, horaH)) {
        var horario = { IdProfesor: profesor, HoraDesde: horaD, HoraHasta: horaH, Turno: turnos };
        turno.push(horario);
        selec++;
    }
    else {
        seguir = false; return;
    }
       

    });

    if (seguir) {
        if (selec > 0) {
            $.ajax({
                type: "POST",
                data: { turno },
                url: "/Personas/RegistrarHorariosProfe",
                success: function (response) {
                    if (response === "OK") {
                        alertify.success("Horarios Registrados");
                        $(".H ").each(function () {
                            var tr = $(this).closest('tr');
                            $(tr).find('td:nth-child(3) .H').val("");
                            $(tr).find('td:nth-child(4) .H').val("");
                            $(tr).find('td:nth-child(1) .che').prop('checked', false);


                        });
                        $("#datosH").css("display", "none");
                    } else {
                        alertify.error("Ocurrio un error en la operacion");
                    }
                }

            });

        } else { alertify.error("Complete un turno por lo menos"); }
    }
    else {
        alertify.error("Error en la validacion de los campos, verifique");
    }

}

function validarHorarios(D, H) {
    if (D !== "" && H === "") {
        
        return false;
    }
    if (D === "" && H !== "") {
        
        return false;
    }
    if (D !== "" && H !== "") {
        if (D >= H) {
           
            return false;
        } }
    

    return true;
}


function BuscarHorarios() {

    var profesor = $("#IdProfesor option:selected").val();
    $(".H ").each(function () {
        var tr = $(this).closest('tr');
        $(tr).find('td:nth-child(2) .H').val("");
        $(tr).find('td:nth-child(3) .H').val("");
       
    });
    $.ajax({
        type: "GET",
        data: { profesor },
        url: "/Personas/BuscarHorariosProfe",
        success: function (response) {
            $("#datosH").css("display", "block");
            for (var i = 0; i < response.length; i++) {
                $(".H").each(function () {                   
                        var tr = $(this).closest('tr');
                        var turnos = $(tr).find('td:nth-child(1) label').text();                                            
                    if (turnos === response[i].turno) {
                        $(tr).find('td:nth-child(2) .H').val(response[i].horaDesde);
                        $(tr).find('td:nth-child(3) .H').val(response[i].horaHasta);                        
                    }

                });


            }
        }

    });
}