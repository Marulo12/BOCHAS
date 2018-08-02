$(document).ready(function(){

    $('#calendar').fullCalendar({
        theme: true,
        themeSystem: 'bootstrap3',
        height: "parent",
        defaultView: 'agendaWeek',
   events: '/Agenda/ArmarAgenda'
       
  });
});