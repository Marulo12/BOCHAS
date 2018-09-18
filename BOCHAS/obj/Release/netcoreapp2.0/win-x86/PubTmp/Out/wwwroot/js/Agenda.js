$(document).ready(function () {
  
    $('#calendar').fullCalendar({
        header: {
            left: 'prev,next today',
            center: 'title',
            right: 'agendaWeek,basicDay'
        },
        allDaySlot: false,
        slotEventOverlap:false,
        aspectRatio: 2,
        theme: true,
        themeSystem: 'bootstrap4',
        height: "parent",
        defaultView: 'agendaWeek',
       // events: '/Agenda/ArmarAgenda',
        eventClick: function (calEvent, jsEvent, view) {
            //var stime = calEvent.start.format('MM/DD/YYYY, h:mm a');
            //var etime = calEvent.end.format('MM/DD/YYYY, h:mm a');
            var evento = calEvent.id;
           // var xpos = jsEvent.pageX;
            //var ypos = jsEvent.pageY;
            
            $.ajax({
                type: "GET",
                url: "/Agenda/MostrarEvento",
                data: { evento },
               
                success: function (response) {
                    $("#EventoBody").html(response);
                    $("#ModalEvento").modal();
                },
                failure: function (response) {
                    alert(response);
                }

            });


            return false;
        }
       
    });
   
    MostrarAgendaPorCancha();

    $('#calendarProfe').fullCalendar({
        header: {
            left: 'prev,next today',
            center: 'title',
            right: 'agendaWeek,basicDay'
        },
        allDaySlot: false,
        slotEventOverlap: false,
        aspectRatio: 2,
        theme: true,
        themeSystem: 'bootstrap4',
        height: "parent",
        defaultView: 'agendaWeek',
         events: '/Agenda/ArmarAgenda',
        eventClick: function (calEvent, jsEvent, view) {
            //var stime = calEvent.start.format('MM/DD/YYYY, h:mm a');
            //var etime = calEvent.end.format('MM/DD/YYYY, h:mm a');
            var evento = calEvent.id;
            // var xpos = jsEvent.pageX;
            //var ypos = jsEvent.pageY;

            $.ajax({
                type: "GET",
                url: "/Agenda/MostrarEvento",
                data: { evento },

                success: function (response) {
                    $("#EventoBody").html(response);
                    $("#ModalEvento").modal();
                },
                failure: function (response) {
                    alert(response);
                }

            });


            return false;
        }

    });
  

});



function MostrarAgendaPorCancha() {
    $('#calendar').fullCalendar('removeEvents');    
    $('#calendar').fullCalendar('removeEvents');    
    $('#calendar').fullCalendar('addEventSource', '/Agenda/ArmarAgendaXCancha?cancha=' + $("#CmbCancha option:selected").val()); 
          }





