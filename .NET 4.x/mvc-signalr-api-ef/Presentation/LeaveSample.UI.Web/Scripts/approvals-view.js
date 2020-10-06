var isHover;

$(function () {

    wireUpSignalR();

    $('.edit-mode').hide();

    $('.edit-row, .cancel-edit').on('click', function () {

        var tr = $(this).closest('tr');
        tr.find('.edit-mode, .view-mode').toggle();

        var command = $(this).text();

        if (command == 'Cancel') {
            var remarks = tr.find("#remarksLabel").text();
            tr.find("#remarks").val(remarks);
        }
    });

    $('.approve-leave, .reject-leave').on('click', function () {

        var tr = $(this).closest('tr');
        tr.find('.edit-mode, .view-mode').toggle();

        var action = $(this).text();

        var leave =
        {
            LeaveID: tr.find('#LeaveID').val(),
            CorrelationID: tr.find('#CorrelationID').val(),
            StartDate: tr.find('.StartDate').text(),
            EndDate: tr.find('.EndDate').text(),
            Duration: tr.find('.Duration').text(),
            Remarks: tr.find("#remarks").val()
        };

        $.ajax({
            url: '/Home/' + action + '/',
            data: JSON.stringify(leave),
            type: 'POST',
            context: tr,
            contentType: "application/json",
            success: function (data) {
                $(this).hide();
            }
        });
    });

    var notification = $(".flyout");
    
    notification.mouseover(function () {
        isHover = true;
    });

    notification.mouseleave(function () {
        notification.fadeOut(500);
        isHover = false;
    });

    notification.hide();

    $('#closeNotification').click(function () {
        notification.hide();
    });
});

// SignalR
function wireUpSignalR() {

    // Connect to hub.
    var proxy = $.connection.leaveActionHub;

    // Update display when updated.
    proxy.client.leaveCancelled = function (leave) {
        $('.grid tr').each(function (i, row) {
            var tr = $(row);
            if (tr.find('#LeaveID').val() == leave.LeaveID) {
                tr.find('.Status').text(leave.Status);
                tr.find('.edit-mode').hide();
                tr.find('.view-mode').hide();
            }
        });
    };

    proxy.client.leaveApplied = function (leave) {
        $('#employeeName').text(leave.Employee);
        runEffect();
    };

    $.connection.hub.start().done(function () {

    });
}

function runEffect() {
    $(".flyout").fadeIn(500, callback);
};

function callback() {
    setTimeout(function () {
        if (!isHover) 
            $(".flyout").fadeOut(1000);
    }, 5000);
};