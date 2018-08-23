$(document).ready(function() {

     ArmarListadoAdicionales();

});

function ArmarListadoAdicionales(){
    
$.ajax({
            type: "GET",            
            url: "/Cobro/ListadoServiciosAdicionales",
            success: function (response) {
                var tr = '';
                for(var i = 0; i < response.length; i++){
                    tr += '<tr><td>' + response[i].nombre + '</td><td>' + response[i].precio + '</td><td><input type="text" class="form-control SAcant"/></td><td><input type="text" class="form-control SAtot"/></td><td><input type="checkbox" class="checkSA" onclick="mostrarServicio(this)"/></td></tr>';
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
        cantidad.empty();
        tot.empty();
        $(check).prop('checked', false);
    }
    
    
}

function CalcularTotal() {

}