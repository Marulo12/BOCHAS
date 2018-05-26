$(document).ready(function () {
    $("#altaEmpleado").after(function () {
        MostrarTipoDocumento(); MostrarLocalidades(); MostrarCargos();
    });
    $("#ConsultaEmpleado").after(function () {
        MostrarEmpleado("");
    });
    $("#BtnBuscarEmp").click(function () {
        MostrarEmpleado($("#FiltroEmp").val());
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
        window.location = "/Personas/RegistrarEmpleado";
    });
    
    $("#TablaEmpladosBaja").after(function () {
        $("#TablaEmpladosBaja").DataTable({
            searching: true,
            responsive:true,
            colReorder: true,
            dom: 'Bfrtip',          
            buttons: [
               
                'excel',
                'pdf',
                {
                    extend: 'print',
                    text: 'Imprimir'
                   
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
});


function MostrarEmpleado(filtro) {
    $.ajax({
        type: "GET",
        url: "/Personas/MostrarEmpleados",
        data: { filtro },
        contentType: "application/json; charset=utf-8",
        dataType: "json",
        success: function (response) {
            var dvItems = $("#CEmpleados");
            dvItems.empty();
            var Table = '<table id="TablaEmpleados" class="table table-striped  display" style="width:100%;" ><thead style="background-color: rgba(158, 44, 44, 0.9);color:white"><tr><th>Nombre</th><th>Apellido</th><th>Documento</th><th>Telefono</th><th>Mail</th><th>Cargo</th><th ></th></tr></thead><tbody>';
            for (var i = 0; i < response.length; i++) {
                Table += '<tr ><td>' + response[i].nombre + '</td>' + '<td>' + response[i].apellido + '</td>' + '<td>' + response[i].documento + '</td>' + '<td>' + response[i].telefono + '</td>' + '<td>' + response[i].mail + '</td>' + '<td>' + response[i].cargo + '</td> <td><div class="btn-group" style="padding-left:17%;"> <button class=" btn btn-sm btn-primary " data-toggle="tooltip" title="Informacion adicional" data-placement="top"  onclick="ConocerDomicilio(' + response[i].id + ');"><i class="far fa-address-card"></i></button><button  class=" btn btn-sm BtnEditar" data-toggle="tooltip" title="Modificar"  onclick="EditarEmpleado(' + response[i].id + ');" data-placement="top" ><i class="fas fa-pencil-alt"></i></button><button class="btn btn-sm btn-danger" data-toggle="tooltip" title="Baja" data-placement="top" onclick="confirmarBaja(' + response[i].id + ');"><i class="fas fa-trash-alt"></i></button></div></td></tr>';
            }
            Table += "</tbody><tfoot></tfoot></table>";
            dvItems.append(Table);
            $("#TablaEmpleados").DataTable({
                searching: true,             
                "scrollX": true,
                responsive: true,
                search: "Filtro&nbsp;:",
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



function EditarEmpleado(id) {
    $.ajax({
        type: "GET",
        url: "/Personas/EditarEmpleado",
        data: { id: id },
        success: function (response) {
            $("#ModalBodyEdicion").html(response);
            $('#ModalEdicion').modal();
        }
    });
        


}

function ConocerDomicilio(id) {
   
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

function confirmarBaja(id) {   
    alertify.confirm("Confirmar baja de empleado", function (e) {
        if (e) {           
            alertify.success("Baja de empleado dada con exito");
                $.ajax({
                    type: "POST",
                    url: "/Personas/Baja",
                    data: { id: id },
                    success: function (response) {
                      
                        window.location = "/Personas/ConsultarEmpleado";
                       
                    }
                });                      
        } else {
            alertify.error("Baja Cancelada");
        }
    });
    
}
var usuarioExiste = false;
function ComprobarUsuario() {
    var usuario = $("#Usuario").val();
  
    $.ajax({
        type: "GET",
        url: "/Personas/ValidarUsuario",
        data: { usuario:usuario },
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
        var cargo = $("#Cargo option:selected").val();
        var dpto = $("#Dpto").val();
        var piso = $("#Piso").val();
        $("#PanelEmpleados").css("display", "none");
        $("#DivCarga").css("display", "inline");
        $.ajax({
            type: "POST",
            url: "/Personas/NewEmpleado",
            data: { nombre, apellido, tipodoc, numero, mail, telefono, localidad, barrio, usuario, contra, calle, cargo, ncalle , dpto, piso },
            success: function (response) {
                if (response === "OK") {
                    $("#DivCarga img").attr("src", "../images/ok.png");
                    $("#DivCarga img").attr("width", "250");
                    $("#DivCarga .ok").append('<p><input id="Continuar" type="button" onclick="LimpiarCampos()" class="btn btn-outline-success"  style="margin-left:10%;" value="Continuar"/></p>');
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
    window.location = "/Personas/RegistrarEmpleado";
}
