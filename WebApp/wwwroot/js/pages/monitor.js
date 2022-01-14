"use strict";

$(() => {
    let connection = new signalR.HubConnectionBuilder()
        .withUrl("/hubs/monitor")
        .withAutomaticReconnect()
        .build();

    //Disable the send button until connection is established.
    //document.getElementById("sendButton").disabled = true;
    let store = new DevExpress.data.CustomStore({
        load() {
            return [];
        },
        key: 'Pin',
    });

    connection.on("Monitor", function (data) {
        let inputs = data.Inputs;
        for (var i = 0; i < inputs.length; i++) {
            store.push([{ type: 'update', key: inputs[i].Pin, inputs[i] }]);
        }
        let outputs = data.Outputs;
        for (var i = 0; i < outputs.length; i++) {
            store.push([{ type: 'update', key: outputs[i].Pin, outputs[i] }]);
        }
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
});