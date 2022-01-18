"use strict";

$(() => {
    let start = true;
    let connection = new signalR.HubConnectionBuilder()
        .configureLogging(signalR.LogLevel.Information)
        .withUrl("/hubs/monitor")
        .withAutomaticReconnect()
        .build();

    //Disable the send button until connection is established.
    //document.getElementById("sendButton").disabled = true;
    connection.start().then(function () {
        let store = [];
        let dataSource = new DevExpress.data.ArrayStore({
            key: 'Pin',
            data: store,
        });
        connection.on("Monitor", function (data) {
            data = JSON.parse(data);
            if (start) {
                start = false;
                [...data.Inputs.map(function (i) {
                    return { ...i, ...{ Type: "INPUT" } }
                }), ...data.Outputs.map(function (i) {
                    return { ...i, ...{ Type: "OUTPUT" } }
                })].map(e => {
                    store.push(e)
                });

                $("#gridContainer").dxDataGrid({
                    dataSource: dataSource,
                    reshapeOnPush: true,
                    visible: true
                });
            } else {
                [...data.Inputs.map(function (i) {
                    return { ...i, ...{ Type: "INPUT" } }
                }), ...data.Outputs.map(function (i) {
                    return { ...i, ...{ Type: "OUTPUT" } }
                })].map(e => {
                    dataSource.push([{ type: 'update', key: e.Pin, data: e }])
                });
            }
        });

    }).catch(function (err) {
        return console.error(err.toString());
    });
});