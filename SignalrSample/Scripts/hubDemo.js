$(function () {
    var myClientName = $('#Placeholder').val();
    var chat = $.connection.chat; 
    // Declare a function on the chat hub so the server can invoke it
    chat.client.addSomeMessage = function (clientName, message) {        
        writeEvent('<b>' + clientName + '</b> 对大家说: ' + message, 'event-message');
    };

    $("#broadcast").click(function () {
        var a = chat;      
        // Call the chat method on the server
        chat.server.send(myClientName, $('#msg').val())
                            .done(function () {
                                console.log('Sent message success!');
                            })
                            .fail(function (e) {
                                console.warn(e);
                            });
    });

    // Start the connection
    $.connection.hub.start();

    //A function to write events to the page
    function writeEvent(eventLog, logClass) {       
        var now = new Date();
        var nowStr = now.getHours() + ':' + now.getMinutes() + ':' + now.getSeconds();
        $('#messages').prepend('<li class="' + logClass + '"><b>' + nowStr + '</b> ' + eventLog + '.</li>');
    }
});