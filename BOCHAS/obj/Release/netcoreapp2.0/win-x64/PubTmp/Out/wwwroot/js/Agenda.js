$(document).ready(function(){

    $('#calendar').fullCalendar({
        allDaySlot: false,
        slotEventOverlap:false,
        aspectRatio: 2,
        theme: true,
        themeSystem: 'bootstrap4',
        height: "parent",
        defaultView: 'agendaWeek',
   events: '/Agenda/ArmarAgenda'
       
  });
});