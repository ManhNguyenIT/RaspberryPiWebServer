"use strict";

$(() => {
    let start = true;
    let connection = new signalR.HubConnectionBuilder()
        .withUrl("/hubs/monitor")
        .withAutomaticReconnect()
        .build();

    //Disable the send button until connection is established.
    //document.getElementById("sendButton").disabled = true;
    let store;
    connection.on("Monitor", function (data) {
        data = JSON.parse(data);
        if (start) {
            start = false;
            store = new DevExpress.data.CustomStore({
                load() {
                    return [...data.Inputs.map(function (i) {
                        return { ...i, ...{ Type: "INPUT" } };
                    }), ...data.Outputs.map(function (i) {
                        return { ...i, ...{ Type: "OUTPUT" } }
                    })];
                },
                key: 'Pin',
            });

            $("#gridContainer").dxDataGrid({
                dataSource: store,
                visible: true
            });
        } else {
            [...data.Inputs.map(function (i) {
                return { ...i, ...{ Type: "INPUT" } }
            }), ...data.Outputs.map(function (i) {
                return { ...i, ...{ Type: "OUTPUT" } }
            })].forEach((e, i) => store.push([{ type: 'update', key: e.Pin, e }]));
        }
    });

    connection.start().then(function () {
        console.log('started')
    }).catch(function (err) {
        return console.error(err.toString());
    });
});