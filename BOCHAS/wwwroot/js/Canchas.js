$(document).ready(function () {
    $("#TablaCanchas_wrapper").css("display","none");
    $("#TablaCanchas").ready(function () {
        $("#TablaCanchas").removeClass("ocultar");
        $("#ImgLoad").css("display", "none");
        $("#TablaCanchas").dataTable(

            {
                searching: true,
                responsive: true,
                search: "Filtro&nbsp;:",
                dom: 'Bfrtip',
                buttons: [
                    {
                        extend: 'excel',
                        text: 'Excel',
                        title: 'BOCHAS PADEL - Canchas'
                    },
                    {
                        extend: 'pdfHtml5',
                        text: 'PDF',
                        title: 'BOCHAS PADEL - Canchas'

                    },
                    {
                        extend: 'print',
                        text: 'Imprimir',
                        title: 'BOCHAS PADEL - Canchas'

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
            }
        );
        $("#TablaSessiones_filter").append($("div .btn-group"));
        $("div .btn-group").css("float", "left");
       
        $('[data-toggle="tooltip"]').tooltip();
    });
    $("#NuevaCancha").submit(function (event) {
        if (comprobarCampos()) {
            alertify.success("Cancha creada con exito");

        } else {
            event.preventDefault();
        }
      
    });
   
});
function EditarCancha(id) {
    $("#ModalEditarCancha .modal-body").empty();
    $.ajax({
        type: "GET",
        url: "/Canchas/EditarCancha",
        data: { id: id },
        success: function (response) {

            $("#ModalEditarCancha .modal-body").append(response);
            $("#ModalEditarCancha").modal();
        }

    });


}


function BajadeCancha(id,idEstado) {
    var ids = id;
    if (idEstado == 1) {
        alertify.alert("Alerta","No puede dar de baja una cancha que esta ocupada");
    }
    else {
        alertify.confirm('Baja de Cancha', 'Seguro desea dar de baja la cancha?', function () {
            $.ajax({
                type: "POST",
                url: "/Canchas/BajadeCancha",
                data: { id: ids },
                success: function (response) {


                    alertify.success("Cancha dada de baja");
                    setTimeout(function () { window.location = "/Canchas/Index"; }, 1000);

                },
                error: function (e) {
                    window.alert(e.responseText + e.responseJSON);
                }

            });

        }
            , function () { alertify.error('Baja Cancelada') });

    }

}

function  comprobarCampos() {
    if ($("#Numero").val() === "") {
        alertify.error('No cargo el Número');
        return false;
    }
    if ($("#Nombre").val() === "") {
        alertify.error('No cargo el Nombre');
        return false;
    }
    return true;
    
}