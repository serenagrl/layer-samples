$(function () {

    // Setup JQuery UI components.
    attachPicker('#StartDate');
    attachPicker('#EndDate');
    attachSpinner('#Duration');

    // SignalR
    wireUpSignalR();

    // Cancel action.
    $('.cancel-leave').on('click', function () {

        var tr = $(this).closest('tr');

        var leave =
        {
            LeaveID: tr.find('#LeaveID').val(),
            CorrelationID: tr.find('#CorrelationID').val(),
            StartDate: tr.find('.StartDate').text(),
            EndDate: tr.find('.EndDate').text(),
            Duration: tr.find('.Duration').text()
        };

        $.ajax({
            url: '/Home/Cancel/',
            data: JSON.stringify(leave),
            type: 'POST',
            context: tr,
            contentType: "application/json",
            success: function (data) {
                tr.find('.Status').text(data.Status);
                tr.find('.cancel-leave').hide();
            }
        });
    });
});

function attachPicker(controlId) {
    $(controlId).datepicker(
    {
        defaultDate: null,
        minDate: -7,
        maxDate: '6M',
        beforeShowDay: $.datepicker.noWeekends,
        onSelect: showDays,
        dateFormat: "dd/mm/yy"
    });
}

function attachSpinner(controlId) {
    $(controlId).spinner({ min: 1, max: 30 });
}

function showDays(date, sender) {
    var start = $('#StartDate').datepicker('getDate');
    var end = $('#EndDate').datepicker('getDate');

    if (!start || !end)
        return;

    var days = (new Date(end - start) / 1000 / 60 / 60 / 24) + 1;

    if (days < 0) days = 0;

    $('#Duration').val(days);
}

// SignalR
function wireUpSignalR() {
    // Connect to hub.
    var proxy = $.connection.leaveActionHub;

    // Update display when updated.
    proxy.client.leaveUpdated = function (leave) {
        $('.grid tr').each(function (i, row) {
            var tr = $(row);
            if (tr.find('#LeaveID').val() == leave.LeaveID) {
                tr.find('.Status').text(leave.Status);
                tr.find('.Remarks').text(leave.Remarks);
                tr.find('.cancel-leave').hide();
            }
        });
    };

    $.connection.hub.start().done(function () {
    });
}