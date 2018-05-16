



$(document).ready(function () {

   

    $("#altaEmpleado").after(function () {
        MostrarTipoDocumento(); MostrarLocalidades(); MostrarCargos();
    });
    $("#ConsultaEmpleado").after(function () {
        MostrarPersona("");
    });
    $("#BtnBuscarEmp").click(function () {
       
        MostrarPersona($("#FiltroEmp").val());

    });
    $("#Localidad").change(function () {
        MostrarBarrio();
    });
    $("#Registrar").click(function () {
        New();
    });
    $("#Limpiar").click(function () {
        LimpiarCampos();
    });
    $("#Continuar").click(function () {
        window.location("/Personas/RegistrarEmpleado");
    });
});
function MostrarPersona(filtro) {
    $.ajax({
        type: "GET",
        url: "/Personas/MostrarEmpleados",
        data: { filtro },
        contentType: "application/json; charset=utf-8",
        dataType: "json",
       

        success: function (response) {

           
            var dvItems = $("#CEmpleados");
            dvItems.empty();
            var Table = '<table class="table table-responsive table-hover"><thead  style="background-color: rgba(215, 40, 40, 0.9);color:white;font-size:17px;"><tr><td>Nombre</td><td>Apellido</td><td>Documento</td><td>Mail</td><td>Cargo</td><td></td></tr></thead>';
            for (var i = 0; i < response.length; i++) {
                Table += '<tr><td style="display:none">' + response[i].id + '</td><td>' + response[i].nombre + '</td>' + '<td>' + response[i].apellido + '</td>' + '<td>' + response[i].documento + '</td>' + '<td>' + response[i].mail + '</td>' + '<td>' + response[i].cargo + '</td> <td class="btn-group"> <button class=" btn btn-sm btn-success"><i class="far fa-address-card"></i></button><button class=" btn btn-sm btn-warning"><i class="fas fa-pencil-alt"></i></button><button class=" btn btn-sm btn-danger"><i class="fas fa-trash-alt"></i></button></td></tr>';
                

            }
            Table += "</table>";
            dvItems.append(Table);

        },
        failure: function (response) {
            alert(response);
        }

    });

}
function MostrarCargos() {
    $.ajax({
        type: "GET",
        contentType: "application/json; charset=utf-8",
        dataType: "json",
        url: "/Personas/MostrarCargos",

        success: function (response) {

            var rows;
            var dvItems = $("#Cargo");
            dvItems.empty();
            for (var i = 0; i < response.length; i++) {
                rows += '<option value="' + response[i].id + '">' + response[i].nombre + '</option>';

            }
            $('#Cargo').append(rows);

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
        var cargo = $("#Cargo option:selected").val();
        $("#PanelEmpleados").css("display", "none");
        $("#DivCarga").css("display", "inline");
        $.ajax({
            type: "POST",
            url: "/Personas/New",
            data: { nombre, apellido, tipodoc, numero, mail, telefono, localidad, barrio, usuario, contra, calle, cargo },
            success: function (response) {
                if (response === "OK") {
                    $("#DivCarga img").attr("src", "../images/ok.png");
                    $("#DivCarga img").attr("width", "250");
                    $("#DivCarga .ok").append('<p><input id="Continuar" type="button" onclick="LimpiarCampos()" class="btn btn-success"  style="margin-left:10%;" value="Continuar"/></p>');
                }
                if (response === "ERROR") {
                    $("#Error").html("Ocurrio un Error en la carga");
                    $("#DivCarga").css("display", "none");
                    $("#PanelEmpleados").css("display", "inline");
                }
                if (response === "EXISTE") {
                    $("#Error").html("Ese empleado ya existe!!");
                    $("#DivCarga").css("display", "none");
                    $("#PanelEmpleados").css("display", "inline");
                }

            },
            failure: function (response) {

                $("#DivCarga").css("display", "none");
                $("#PanelEmpleados").css("display", "inline");
                $("#Error").html("Ocurrio un Error en la carga");
            }
        });
    }


}
function ComprobarCampos() {
    if ($("#Nombre").val() === "") {
        $("#Error").html("No cargo el Nombre");

        return false;
    }
    if ($("#Apellido").val() === "") {
        $("#Error").html("No cargo el Apellido");
        return false;
    }
    if ($("#Numero").val() === "") {
        $("#Error").html("No cargo el Numero");
        return false;
    }
    if ($("#Mail").val() === "") {
        $("#Error").html("No cargo el Mail");
        return false;
    }
    if ($("#Telefono").val() === "") {
        $("#Error").html("No cargo el Telefono");
        return false;
    }
    if ($("#Usuario").val() === "") {
        $("#Error").html("No cargo el Usuario");
        return false;
    }
    if ($("#Contra").val() === "") {
        $("#Error").html("No cargo La Contraseña");
        return false;
    }
    if ($("#Calle").val() === "") {
        $("#Error").html("No cargo la Calle");
        return false;
    }

    return true;
}

function LimpiarCampos() {
    window.location("/Personas/RegistrarEmpleado");
}