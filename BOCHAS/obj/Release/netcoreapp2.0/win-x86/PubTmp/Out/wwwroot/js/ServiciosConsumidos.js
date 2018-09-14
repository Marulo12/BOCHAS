
function NotaServiciosReservas() {
    var Numero = $("#NReservaFinalizada").val();
    var Tipo = 1;
    $.ajax({
        type: "GET",
        data: { Numero , Tipo },
        url: "/ServiciosAdicionales/TraerServiciosConsumidos",
        success: function (response) {
            var li = '';
            for (var i = 0; i < response.length; i++)
            {
                li += ' <li class="list-group-item linea">' + response[i].nombre + ': ' + response[i].cantidad + '</li>';
            }
            if (li === '') {
                li = '<li class="list-group-item" style="margin:0px;">No hay nota de Servicios prestados</li>';
            }
            $("#UlNotaServ").html(li); 
            

        }
    });

}
function NotaServiciosReservasManual(id) {
    var Numero = id;
    var Tipo = 1;
    $.ajax({
        type: "GET",
        data: { Numero, Tipo },
        url: "/ServiciosAdicionales/TraerServiciosConsumidos",
        success: function (response) {
            var li = '';
            for (var i = 0; i < response.length; i++) {
                li += ' <li class="list-group-item linea">' + response[i].nombre + ': ' + response[i].cantidad + '</li>';
            }
            if (li === '') {
                li = '<li class="list-group-item" style="margin:0px;">No hay nota de Servicios prestados</li>';
            }
            $("#UlNotaServ").html(li);
            $("#ModalNotaServicios").modal();

        }
    });

}
function NotaServiciosClasesManual(id) {
    var Numero = id;
    var Tipo = 2;
    $.ajax({
        type: "GET",
        data: { Numero, Tipo },
        url: "/ServiciosAdicionales/TraerServiciosConsumidos",
        success: function (response) {
            var li = '';
            for (var i = 0; i < response.length; i++) {
                li += ' <li class="list-group-item linea">' + response[i].nombre + ': ' + response[i].cantidad + '</li>';
            }
            if (li === '') {
                li = '<li class="list-group-item" style="margin:0px;">No hay nota de Servicios prestados</li>';
            }
            $("#UlNotaServ").html(li);
            $("#ModalNotaServicios").modal();

        }
    });

}

function NotaServiciosClases() {


    var Numero = $("#NClaseFinalizada").val();
    var Tipo = 2;
    $.ajax({
        type: "GET",
        data: { Numero, Tipo },
        url: "/ServiciosAdicionales/TraerServiciosConsumidos",
        success: function (response) {
            var li = '';
            for (var i = 0; i < response.length; i++) {
                li += ' <li class="list-group-item linea">' + response[i].nombre + ': ' + response[i].cantidad + '</li>';
            }
            if (li === '') {
                li = '<li class="list-group-item" style="margin:0px;">No hay nota de Servicios prestados</li>';
            }
            $("#UlNotaServ").html(li);


        }
    });


}