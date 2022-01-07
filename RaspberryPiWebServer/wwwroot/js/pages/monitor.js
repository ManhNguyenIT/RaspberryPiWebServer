"use strict";

var connection = new signalR.HubConnectionBuilder()
    .withUrl("/hubs/monitor")
    .withAutomaticReconnect()
    .build();

//Disable the send button until connection is established.
//document.getElementById("sendButton").disabled = true;

connection.on("Monitor", function (data) {
    console.log(data);
});

connection.start().then(function () {
    console.log('started')
}).catch(function (err) {
    return console.error(err.toString());
});

document.getElementById("blink").addEventListener("click", function (event) {
    connection.invoke("ClickAsync", true).catch(function (err) {
        return console.error(err.toString());
    });
    event.preventDefault();
});