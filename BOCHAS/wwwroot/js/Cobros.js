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
                    tr += '<tr><td>' + response[i].nombre + '</td><td>' + response[i].precio + '</td><td><input type="text" class="form-control"/></td><td></td><td><input type="checkbox" class="checkSA"/></td></tr>';
                }
                
                $("#TserviciosA tbody").html(tr);               
            },

            failure: function (response) {
                alert(response);
            }

        });

}