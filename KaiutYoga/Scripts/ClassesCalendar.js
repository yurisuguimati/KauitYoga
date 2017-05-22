var weeklyClassesColor = 'green';
var weeklyClassesPresenceColor = '#003300';
var anyClassPresenceColor = 'gray';
//#D5AA00 - aquele amarelo
var anyClassColor = '#800000';
var spareClassColor = 'orange';
var spareClassPresenceColor = '#996300'

var spareEventsToChangeTitle = [];
var spareEvents = [];
var blockedSpareEvents = [];
var presentInClass = [];
var missedClass = [];

function PresentInClassOrNot(calendarName) {
    
    selectedPresentInClassEvents = $('#' + calendarName).fullCalendar('clientEvents', function (event) {
        if (presentInClass == null)
            return false;

        for (var i = 0; i < presentInClass.length; i++) {
            var dateEvent = new Date(event.start);
            if (presentInClass[i].getTime() == dateEvent.getTime()) {
                return true;
            }
        }
        return false;
    });

    for (var i = 0; i < selectedPresentInClassEvents.length; i++) {
        selectedPresentInClassEvents[i].backgroundColor = "white";
    }

    $('#'+calendarName).fullCalendar('updateEvent', calEvent);
    //$('#' + calendarName).fullCalendar('removeEventSource', createdEvents);
    //$('#' + calendarName).fullCalendar('refetchEvents');
    //$('#' + calendarName).fullCalendar('addEventSource', createdEvents);
}

function CorrectEventTitle(calendarName) {
    
    selectedEventsToChangeTitle = $('#' + calendarName).fullCalendar('clientEvents');
    var dataToSend = [];
    
    for (var i = 0; i < selectedEventsToChangeTitle.length; i++) {   
        dataToSend.push({ ClassId: selectedEventsToChangeTitle[i].classId, Start: new Date(selectedEventsToChangeTitle[i].start), Weekly: 0, Trial: 0, Replacement: 0 });
    }
    
    $.ajax({
        type: 'post',
        url: window.applicationBaseUrl + "Class/GetClasses",
        contentType: "application/json; charset=utf-8",
        dataType: "json",
        data: JSON.stringify(dataToSend),
        async: false,
        success: function (result) {
            for (var i = 0; i < result.length; i++) {
                selectedEventsToChangeTitle[i].title = "Inscritos: " + (result[i].Weekly + result[i].Trial + result[i].Replacement) + "/" + selectedEventsToChangeTitle[i].capacity;
            }
        },
    });
}

function GetAndPaintSelectedSpareEvents(calendarName) {
    selectedSpareEvents = $('#' + calendarName).fullCalendar('clientEvents', function (event) {
        if (spareEvents == null)
            return false;

        for (var i = 0; i < spareEvents.length; i++) {
            var dateEvent = new Date(event.start);
            if (spareEvents[i].getTime() == dateEvent.getTime()) {
                return true;
            }
        }
        return false;
    });
    for (var i = 0; i < selectedSpareEvents.length; i++) {
        selectedSpareEvents[i].selected = true;
        selectedSpareEvents[i].backgroundColor = spareClassColor;
    }
}

jQuery.expr.filters.offscreen = function (el) {
    return (
        (el.offsetLeft + el.offsetWidth) < 0
        || (el.offsetTop + el.offsetHeight) < 0
        || (el.offsetLeft > window.innerWidth || el.offsetTop > window.innerHeight)
    );
};

function RightClickMenuPosition(pageY, pageX) {
    var toolTipWidth = 20;
    var toolTipHeight = 20;
    
    var documentWidth = $(window).width();
    var documentHeight = $(window).height();

    var top = pageY;
    if (top + toolTipHeight > documentHeight) {
        top = pageY - toolTipHeight;
    }

    var left = pageX;

    if (left + toolTipWidth > documentWidth) {
        left = pageX - toolTipWidth;
    }
    $('.custom-menu').css({ position:'fixed', 'top': top, 'left': left });
}

function ToolTipPosition(pageY, pageX) {
    var offsetWidth = 10;
    var offsetHeight = 10;

    //Get the tool tip container width adn height
    var toolTipWidth = $(".calendarTooltip").width();
    var toolTipHeight = $(".calendarTooltip").height();

    var documentWidth = $(window).width();
    var documentHeight = $(window).height();

    //Set top and bottom position of the tool tip
    //var top = $(this).offset().top;
    var top = pageY;
    //alert(top+toolTipHeight +">"+documentHeight)
    if (top + toolTipHeight > documentHeight) {
        top = pageY - toolTipHeight - (2*offsetHeight);
    }

    var left = pageX + offsetWidth;

    if (left + toolTipWidth > documentWidth) {
        // shift the tool tip position to the left of the object 
        // so it won't go out of width of current HTML document width
        // and show up in the correct place
        left = pageX - toolTipWidth - (2 * offsetWidth);
    }
    $('.calendarTooltip').show();
    $('.calendarTooltip').css({ 'top': top, 'left': left});
    //return top + "|" + left;
}

function GetAndPaintEventsWithPresence(calendarName) {
    
    allEventsPresence = $('#' + calendarName).fullCalendar('clientEvents', function (event) {
        for (var i = 0; i < blockedSpareEvents.length; i++) {
            if (blockedSpareEvents[i] == event.start) {
                return true;
            }
        }
        return false;
    });
    
    selectedSpareEvents = $('#' + calendarName).fullCalendar('clientEvents', function (event) {
        if (spareEvents == null)
            return false;

        for (var i = 0; i < spareEvents.length; i++) {
            var dateEvent = new Date(event.start);
            if (spareEvents[i].getTime() == dateEvent.getTime()) {
                return true;
            }
        }
        return false;
    });


    selectedWeeklyEvents = $('#' + calendarName).fullCalendar('clientEvents', function (event) {
        if (event.weekly == true && event.selected)
            return true;
        return false;
    });

    for (var i = 0; i < allEventsPresence.length; i++) {
        allEventsPresence[i].backgroundColor = anyClassPresenceColor;
        for (var j = 0; j < selectedSpareEvents.length; j++) {
            if (allEventsPresence[i].start == selectedSpareEvents[j].start) {
                allEventsPresence[i].backgroundColor = spareClassPresenceColor;
                break;
            }
        }

        for (var j = 0; j < selectedWeeklyEvents.length; j++) {
            if (allEventsPresence[i].start == selectedWeeklyEvents[j].start) {
                allEventsPresence[i].backgroundColor = weeklyClassesPresenceColor;
                break;
            }
        }
    }
}

function AddClass(studentId, planId, amountSpare, calEvent, onlySpare) {
    $.ajax({
        type: 'post',
        url: window.applicationBaseUrl+"Student/AddClass",
        dataType: "json",
        data: "studentId=" + studentId + "&classId=" + calEvent.id + "&planId=" + planId + "&amountspare=" + amountSpare + "&date=" + calEvent.start + "&onlySpare=" + onlySpare,
        contentType: "application/x-www-form-urlencoded;charset=utf-8",
        async: true,
        success: function (result) {
            if (result.SpareAdded || result.WeeklyAdded){
                allEvents = $("#monthlyClasses").fullCalendar( 'clientEvents', function (event) {
                    var dateEvent = new Date(event.start);
                    if (registerDate.getTime() <= dateEvent.getTime() && calEvent.id == event.id) {
                        return true;
                    }
                    return false;
                });
                                    
                for (var i = 0; i < allEvents.length; i++) {
                    if (result.WeeklyAdded)
                    {
                        allEvents[i].selected = true;
                        allEvents[i].weekly = true;
                        allEvents[i].backgroundColor = weeklyClassesColor;
                        for (var j = 0; j < createdEvents.length; j++) {
                            if (createdEvents[j].id == allEvents[i].id)
                            {
                                lastTitle = createdEvents[j].title;
                                createdEvents[j]=allEvents[i];
                                createdEvents[j].title;
                            }
                        }
                    }
                }
                if (result.WeeklyAdded){
                    $('#monthlyClasses').fullCalendar('updateEvent',calEvent);
                }
                else if (result.SpareAdded == true){
                    for (var j = 0; j < createdEvents.length; j++) {
                        if (createdEvents[j].id == calEvent.id)
                        {
                            spareEvents.push(new Date(calEvent.start));
                            createdEvents[j]=calEvent;
                        }
                    }
                }
                $('#monthlyClasses').fullCalendar('next');
                $('#monthlyClasses').fullCalendar('prev');
                $('#amountnormal').html(result.WeeklyMsg);
                $('#amountspare').html(result.SpareMsg);
                changed=true;
            }
            else {
                if (result.HavePresenceList)
                    openErrorModal("A presença deste dia já foi feita, você não pode mais adicionar esta aula.")
                else
                    openErrorModal("Você já selecionou a quantidade máxima de aulas.")
            }
        },
        error: function (e){
            $('#amountnormal').html(e);
        }
    });
}

function RemoveClass(studentId, planId, amountSpare, calEvent) {
    $.ajax({
        type: 'post',
        url: window.applicationBaseUrl+"Student/RemoveClass",
        dataType: "json",
        data: "studentId="+studentId+"&classId=" + calEvent.id + "&planId="+planId+"&amountspare="+amountSpare+"&date=" + calEvent.start,
        contentType: "application/x-www-form-urlencoded;charset=utf-8",
        success: function (result) {
        if (result.Removed == true){
            allEvents = $("#monthlyClasses").fullCalendar( 'clientEvents', calEvent.id );
            for (var i = 0; i < allEvents.length; i++) {
                allEvents[i].selected = false;
                allEvents[i].backgroundColor = anyClassColor;

                for (var j = 0; j < createdEvents.length; j++) {
                    if (createdEvents[j].id == allEvents[i].id)
                    {
                        lastTitle = createdEvents[j].title;
                        createdEvents[j]=allEvents[i];
                        createdEvents[j].title = lastTitle;
                    }
                }
            }
            for (var k = 0; k < spareEvents.length; k++) {
                var dateEvent = new Date(calEvent.start);
                if (spareEvents[k].toUTCString() == dateEvent.toUTCString())
                {
                    spareEvents.splice(k,1);
                }
            }
            $('#monthlyClasses').fullCalendar('updateEvent',calEvent);
            GetAndPaintSelectedSpareEvents('monthlyClasses');
            GetAndPaintEventsWithPresence('monthlyClasses');

            changed=true;
        }
        else {
            openErrorModal("A presença deste dia ja foi feita, você não pode mais remover esta aula.")
        }
        $('#monthlyClasses').fullCalendar('next');
        $('#monthlyClasses').fullCalendar('prev');

        $('#amountnormal').html(result.WeeklyMsg);
        $('#amountspare').html(result.SpareMsg);
    },
    error: function (e){
        $('#amountnormal').html(e);
    }})
}

function openErrorModal(msg) {
    $("#errorMsgContent").html(msg);
    $('#errorMsgModal').modal('show');
}