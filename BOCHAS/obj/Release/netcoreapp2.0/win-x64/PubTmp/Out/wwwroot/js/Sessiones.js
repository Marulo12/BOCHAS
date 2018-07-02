$(document).ready(function () {

    $("#BtnSessionMostrar").click(function () {
        MostrarSessiones();
    });

});

function MostrarSessiones() {

    if (ComprobarCampos()) {
        var fechadesde = $("#fechadesde").val();
        var fechahasta = $("#fechahasta").val();

        $("#DivSessiones").html('<img id="ImgLoad" src="../images/extras/dots-2.gif" alt="" width="200" />');

        $.ajax({
            type: "GET",
            url: "/Sessions/MostrarSessiones",
            data: { fechadesde, fechahasta },
            success: function (response) {

                $("#DivSessiones").html(response);
                $("#TablaSessiones").DataTable({
                    searching: true,                   
                    responsive: true,
                    search: "Filtro&nbsp;:",                   
                    dom: 'Bfrtip',
                    buttons: [
                        {
                            extend: 'excel',
                            text: 'Excel',
                            title: 'BOCHAS PADEL - Sesiones'
                        },
                        {
                            extend: 'pdfHtml5',
                            text: 'PDF',
                            title: 'BOCHAS PADEL - Sesiones'

                        },
                        {
                            extend: 'print',
                            text: 'Imprimir',
                            title: 'BOCHAS PADEL - Sesiones'

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
                $("#TablaSessiones_filter").append($("div .btn-group"));
                
                $("div .btn-group").css("float", "left");
            }
        });
    }
}

function ComprobarCampos() {
    if ($("#fechadesde").val() === "") {

        alertify.error('No cargo la fecha desde');
        return false;
    }
    if ($("#fechahasta").val() === "") {
        alertify.error('No cargo la fecha hasta');
        return false;
    }
   

    return true;
}