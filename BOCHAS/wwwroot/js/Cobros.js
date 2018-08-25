$(document).ready(function() {
    ArmarListadoAdicionales();   
});

function ArmarListadoAdicionales(){    
$.ajax({
            type: "GET",            
            url: "/Cobro/ListadoServiciosAdicionales",
            success: function (response) {
                var tr = '';
                for (var i = 0; i < response.length; i++){
                    tr += '<tr><td>' + response[i].nombre + '</td><td>' + response[i].precio + '</td><td><input type="text" class="form-control SAcant" /></td><td><input type="text" class="form-control SAtot" readonly/></td><td><input type="checkbox" class="checkSA" onclick="mostrarServicio(this)"/></td><td style="display:none;">' + response[i].id + '</td></tr>';
                }
                
                $("#TserviciosA tbody").html(tr);               
            },

            failure: function (response) {
                alert(response);
            }

        });
}

function mostrarServicio(check) {
    var tr = $(check).closest('tr');
    var nombre = $(tr).find('td:nth-child(1)').text();
    var precio = $(tr).find('td:nth-child(2)').text();
    var cantidad = $(tr).find('td:nth-child(3) .SAcant').val();
    var tot = $(tr).find('td:nth-child(4) .SAtot');
    if ($(check).is(':checked')) {
        if (cantidad !== '') {
            tot.val(precio * cantidad);
        }
        else {
            alertify.error("Ingrese una cantidad");
            $(check).prop('checked', false);
        }
        
    } else {
        $(tr).find('td:nth-child(3) .SAcant').val("");
        tot.val("");
        $(check).prop('checked', false);
    }
    
    
}

function CalcularTotal() {
    $("#InputTotalR").val("");
    var TotalServicio = $("#Stotal").val();
var ServiciosAdicionales = 0;
    $(".checkSA ").each(function () {
        if ($(this).is(':checked')) {
            var tr = $(this).closest('tr');
            var tot = $(tr).find('td:nth-child(4) .SAtot').val();
            ServiciosAdicionales = parseInt(ServiciosAdicionales) + parseInt(tot);
        }
         
    });
    var Total = parseInt(TotalServicio) + parseInt(ServiciosAdicionales);
$("#InputTotalR").val(Total);    
}

function RegistrarCobroReserva() {
    // todo para clase cobro 
    var Nreserva = $("#NReservaFinalizada").val();
    var Fecha = $("#FechaCobro").val();
    var MedioPago = $("#MedioPago option:selected").val();
    var MontoTotal = $("#InputTotalR").val();
    var NroCupon = 0;
    var IdTarjeta = 0;
   
    // todo para detalle cobro
    // objeto servicio
    var cantidadCanchas = $("#CantC").val();
    var MontoServicio = $("#Stotal").val();
    var Servicio = { IdServicio: 1, Monto: MontoServicio, Id_NumeroCobro: 0, Cantidad: cantidadCanchas, IdServiciosAdicionales: null };
    //array de servicios adicionales
    var ServiciosAdicionales = [];
    $(".checkSA ").each(function () {
        if ($(this).is(':checked')) {
            var tr = $(this).closest('tr');
            var tot = $(tr).find('td:nth-child(4) .SAtot').val();
            var idservicio = $(tr).find('td:nth-child(6)').text();
            var cantidad = $(tr).find('td:nth-child(3) .SAcant').val();
            var servicioadicional = { IdServicio: null, Monto: tot, Id_NumeroCobro: 0, Cantidad: cantidad, IdServiciosAdicionales: idservicio };
            ServiciosAdicionales.push(servicioadicional);
        }

    });
    $.ajax({
        type: "POST",
        data: { Nreserva, Fecha, MedioPago, MontoTotal, NroCupon, IdTarjeta, MontoServicio, Servicio, ServiciosAdicionales },
        url: "/Cobro/RegistrarCobroReserva",
        success: function (response) {
          
            $("#ModalCobro").modal("hide");
            alertify.success("Cobro realizado con exito");
          //  window.open("/Reportes/ReporteCobroReserva?NCobro=" + response);
            ReporteCobroReserva(response);
            },

    failure: function (response) {
        alert(response);
    }

});

}


