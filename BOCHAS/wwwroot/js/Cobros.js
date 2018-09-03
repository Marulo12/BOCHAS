
$(document).ready(function () {
    
    ArmarListadoAdicionales();   
    $("#BtnBuscarCobro").click(function () {
        MostrarCobros();
    });

    $("#CerrarModalReporte").click(function () {
        setTimeout('nada()', 2000); 
        window.location = "/Cobro/CobroManual";
    });

    $("#MedioPago").change(function () {        
        
        if ($("#MedioPago option:selected").val() === "2") {
            $("#DivTarjeta").show();
        }
        if ($("#MedioPago option:selected").val() === "1") {
            $("#DivTarjeta").hide();
        }
    
    });
    $(document).on('click', '.borrar', function (event) {
        event.preventDefault();
        $(this).closest('tr').remove();
    });
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
    var NroCupon = null;
    var IdTarjeta = null;
    if (MedioPago === "2") {
        NroCupon = $("#Ncupon").val();
        IdTarjeta=$("#IdTarjeta option:selected").val();
    }
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
function RegistrarCobroClase() {
    // todo para clase cobro 
    var Nclase = $("#NClaseFinalizada").val();
    var Fecha = $("#FechaCobro").val();
    var MedioPago = $("#MedioPago option:selected").val();
    var MontoTotal = $("#InputTotalR").val();
    var NroCupon = null;
    var IdTarjeta = null;
    if (MedioPago === "2") {
        NroCupon = $("#Ncupon").val();
        IdTarjeta = $("#IdTarjeta option:selected").val();
    }
    // todo para detalle cobro
    // objeto servicio
    var cantidadCanchas = $("#CantC").val();
    var MontoServicio = $("#Stotal").val();
    var Servicio = { IdServicio: 2, Monto: MontoServicio, Id_NumeroCobro: 0, Cantidad: cantidadCanchas, IdServiciosAdicionales: null };
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
        data: { Nclase, Fecha, MedioPago, MontoTotal, NroCupon, IdTarjeta, MontoServicio, Servicio, ServiciosAdicionales },
        url: "/Cobro/RegistrarCobroClase",
        success: function (response) {

            $("#ModalCobro").modal("hide");
            alertify.success("Cobro realizado con exito");
            //  window.open("/Reportes/ReporteCobroReserva?NCobro=" + response);
            ReporteCobroClase(response);
        },

        failure: function (response) {
            alert(response);
        }

    });

}
function MostrarCobros() {
    var fecD = $("#fechadesde").val();
    var fecH = $("#fechahasta").val();
    $("#ImgLoad").css("display", "inline-block");
    $("#Bodicobro").empty();
    $.ajax({
        type: "GET",
        data: { fecD, fecH },
        url: "/Cobro/ConsultarCobros",
        success: function (response) {
            $("#Bodicobro").html(response);
            
            $("#ImgLoad").css("display", "none");
            $("#TablaCobros").DataTable({
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
                        title: 'BOCHAS PADEL - Cobros Alquiler de Canchas'

                    }, {
                        extend: 'excel',
                        text: 'Excel',
                        title: 'BOCHAS PADEL - Cobros Alquiler de Canchas'
                    },
                    {
                        extend: 'pdfHtml5',
                        text: 'PDF',
                        title: 'BOCHAS PADEL - Cobros Alquiler de Canchas'

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

    });


}
function CalcularXReserva() {   
    var IdReserva = $("#Reservas option:selected").val();
    $.ajax({
        type: "GET",
        data: { IdReserva },
        url: "/Cobro/TraerPorReserva",
        success: function (response) {
            var existe = false;
            var tr = "";

            tr = '<tr><td class="Nres">' + IdReserva + '</td><td>' + response.servicio + '</td><td>' + response.precio + '</td><td>' + response.canchas + '</td><td>' + Number.parseFloat(response.horas) + '</td><td class="Stotal">' + response.total + '</td><td><button class="btn btn-sm btn-danger borrar"><i class="fas fa-backspace"></i></button></td></tr>';
            $(".Nres").each(function () {
                var tr = $(this).closest('tr');
                var tot = $(tr).find('td:nth-child(1)').text();

                if (tot === IdReserva) {
                    
                    existe = true;
                }
               
            });
            if (existe) { alertify.error("Esa reserva ya esta incorporada para el cobro"); } else { $("#TDetalleR").append(tr); }
            
        }
    });
}
function CalcularTotalReservas() {
    $("#InputTotalR").val("");
    var TotalServicio = 0;
    var ServiciosAdicionales = 0;
    $(".Stotal ").each(function () {        
        var tr = $(this).closest('tr');
        var tot = $(tr).find('td:nth-child(6)').text();
        TotalServicio = parseInt(TotalServicio) + parseInt(tot);        
    });

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

function RegistrarCobroReservaManual() {
    // todo para clase cobro 
    var Nreserva = [];
    $(".Nres").each(function () {
        var tr = $(this).closest('tr');
        var tot = $(tr).find('td:nth-child(1)').text();
        Nreserva.push ( tot );      
    });
    var Fecha = $("#FechaCobro").val();
    var MedioPago = $("#MedioPago option:selected").val();
    var MontoTotal = $("#InputTotalR").val();
    var NroCupon = null;
    var IdTarjeta = null;
    if (MedioPago === "2") {
        NroCupon = $("#Ncupon").val();
        IdTarjeta = $("#IdTarjeta option:selected").val();
    }
    // todo para detalle cobro
    // objeto servicio    
    var MontoServicio = $("#Stotal").val();
    var Servicio = []; 
    $(".Nres").each(function () {
        var tr = $(this).closest('tr');
        var canchas = $(tr).find('td:nth-child(4)').text();
        var monto = $(tr).find('td:nth-child(6)').text();        
        servicio = { IdServicio: 1, Monto: monto, Id_NumeroCobro: 0, Cantidad: canchas, IdServiciosAdicionales: null };
        Servicio.push(servicio);
    });
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
        url: "/Cobro/RegistrarCobroReservaManual",
        success: function (response) {          
           
            alertify.success("Cobro realizado con exito");
            //  window.open("/Reportes/ReporteCobroReserva?NCobro=" + response);
            ReporteCobroReserva(response);
        },
        failure: function (response) {
            alert(response);
        }

    });

}

function Limpiar() {
    window.location = "/Cobro/CobroManual";
}