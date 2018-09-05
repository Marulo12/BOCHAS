

function visor(nombre) {
    $("#servicio").val(nombre);
alertify.genericDialog || alertify.dialog('genericDialog', function () {
    return {
        main:function(content){
            this.setContent(content);
        },
        setup:function(){
            return {
                focus:{
                    element:function(){
                        return this.elements.body.querySelector(this.get('selector'));
                    },
                    select:true
                },
                options:{
                    basic:true,
                    maximizable:false,
                    resizable:false,
                    padding:false
                }
            };
        },
        settings:{
            selector:undefined
        }
    };
});
//force focusing password box
    alertify.genericDialog($('#loginForm')[0]);
}

function Generar() {
    var FecD = $("#FecD").val();
    var FecH = $("#FecH").val();
    
    if ($("#FecD").val() !== "" && $("#FecH").val() !== "") {
        if (ComprobarFechas()) {
            window.open("/Reportes/" + $("#servicio").val() + "?FecD=" + FecD + "&FecH=" + FecH);
            alertify.genericDialog().close();
        }

    }
     else { alertify.error("Complete los campos de fechas"); }
    
}

function ComprobarFechas() {
    if ($("#FecD").val() > $("#FecH").val()) {
        alertify.error("La fecha desde no puede ser mayor que el hasta");
        return false;
    }
    return true;
}

function ReporteServicio() {
    window.open("/Reportes/ReporteServicios");
}