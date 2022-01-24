"use strict";

$(() => {
    let history = { model: '', template: '' };

    let sensorPin = 8;
    let isRunning = false;
    let isSensor = false;
    let isSubmit = false;
    let running = $('#running')
    let setting = $('#setting')
    let sensorOn = $('#sensor-on')
    let sensorOff = $('#sensor-off')
    let error = $('#error')
    let model = $('#model')
    let template = $('#template')
    let code = $('#code')
    let totalQty = $('#total-qty')
    let okQty = $('#ok-qty')
    let okRate = $('#ok-rate')
    let ngQty = $('#ng-qty')
    let ngRate = $('#ng-rate')


    let password = $('#password').dxTextBox({
        mode: 'password',
        placeholder: 'Enter password',
        showClearButton: true,
    }).dxTextBox('instance');

    let popup = $('#popup').dxPopup({
        width: 300,
        height: 200,
        container: '.dx-viewport',
        showTitle: true,
        title: 'Mật khẩu cài đặt',
        visible: false,
        dragEnabled: false,
        closeOnOutsideClick: true,
        showCloseButton: false,
        onShown: function () {
            password.focus();
        },
        position: {
            at: 'center',
            my: 'center',
        },
        toolbarItems: [{
            widget: 'dxButton',
            toolbar: 'bottom',
            location: 'before',
            options: {
                icon: 'check',
                type: 'success',
                text: 'Xác nhận',
                stylingMode: 'outlined',
                onClick() {
                    if (password.option("value") === '') {
                        return
                    }
                    $.ajax({
                        url: 'api/localization/check-password',
                        type: 'POST',
                        contentType: 'application/x-www-form-urlencoded',
                        data: { password: password.option("value")},
                        success: function (res) {
                            if (res) {
                                password.option("value", '')
                                popup.callback();
                            } else {
                                DevExpress.ui.notify("Sai mật khẩu", 'error', 600);
                            }
                        },
                        error: function (xhr, ajaxOptions, thrownError) {
                            DevExpress.ui.notify(thrownError + "\r\n" + xhr.statusText + "\r\n" + xhr.responseText, 'error', 600);
                        }
                    });
                },
            },
        }, {
            widget: 'dxButton',
            toolbar: 'bottom',
            location: 'after',
            options: {
                icon: 'close',
                type: 'default',
                text: 'Close',
                stylingMode: 'outlined',
                onClick() {
                    popup.hide();
                },
            },
        }],
    }).dxPopup('instance');

    let connection = new signalR.HubConnectionBuilder()
        .configureLogging(signalR.LogLevel.Information)
        .withUrl("/hubs/monitor")
        .withAutomaticReconnect()
        .build();

    function loginCallback(c) {
        popup.callback = c;
        popup.show();
    }

    function onRunning() {
        isRunning = true;
        running.removeClass('btn-outline-success')
        running.addClass('btn-success')
        setting.removeClass('btn-warning')
        setting.addClass('btn-outline-warning')

        model.attr("readonly", "readonly")
        template.attr("readonly", "readonly")
        code.removeAttr("readonly")

        code.focus();
    }
    function onSetting() {
        popup.hide();
        isRunning = false;
        setting.removeClass('btn-outline-warning')
        setting.addClass('btn-warning')
        running.removeClass('btn-success')
        running.addClass('btn-outline-success')

        model.removeAttr("readonly")
        template.removeAttr("readonly")

        history.code = '';
        code.attr("readonly", "readonly")
        code.val(history.code)
        code.removeClass('bg-success text-white')
        code.removeClass('bg-danger text-white')
        model.focus();
    }
    function getCurrent() {
        if (history.model === '' || history.template === '') {
            return
        }
        $.ajax({
            url: 'api/history/current',
            type: 'POST',
            contentType: "application/json; charset=utf-8",
            dataType: "json",
            data: JSON.stringify(history),
            success: function (res) {
                //DevExpress.ui.notify('Done', 'success', 600);
                totalQty.val(res.Ok + res.Ng);
                okQty.val(res.Ok);
                ngQty.val(res.Ng);
                okRate.val(res.Ok + res.Ng === 0 ? 0 : (res.Ok * 100 / (res.Ok + res.Ng)).toFixed(2));
                ngRate.val(res.Ok + res.Ng === 0 ? 0 : (res.Ng * 100 / (res.Ok + res.Ng)).toFixed(2));
            },
            error: function (xhr, ajaxOptions, thrownError) {
                DevExpress.ui.notify(thrownError + "\r\n" + xhr.statusText + "\r\n" + xhr.responseText, 'error', 600);
            }
        });
    }

    error.on("click", function (e) {
        var result = DevExpress.ui.dialog.confirm("<i>Xác nhận?</i>", "Xác nhận lỗi");
        result.done(function (dialogResult) {
            if (dialogResult) {
                $.ajax({
                    url: 'api/history/cancel-lastest',
                    type: 'POST',
                    contentType: "application/json; charset=utf-8",
                    dataType: "json",
                    data: JSON.stringify(history),
                    success: function (res) {
                        DevExpress.ui.notify('Done', 'success', 600);
                        totalQty.val(res.Ok + res.Ng);
                        okQty.val(res.Ok);
                        ngQty.val(res.Ng);
                        okRate.val(res.Ok + res.Ng === 0 ? 0 : (res.Ok * 100 / (res.Ok + res.Ng)).toFixed(2));
                        ngRate.val(res.Ok + res.Ng === 0 ? 0 : (res.Ng * 100 / (res.Ok + res.Ng)).toFixed(2));
                    },
                    error: function (xhr, ajaxOptions, thrownError) {
                        DevExpress.ui.notify(thrownError + "\r\n" + xhr.statusText + "\r\n" + xhr.responseText, 'error', 600);
                    }
                });
            }
        });
    })

    template.on("change", function (e) {
        let value = e.target.value
        history.template = value;
        getCurrent();
    })

    model.on("change", function (e) {
        let value = e.target.value
        history.model = value;
        getCurrent();
    });

    totalQty.on('focus', function () {
        code.focus();
        code.select();
    });

    code.on("change", function (e) {
        let value = e.target.value
        history.code = value;
        code.removeClass('bg-success text-white')
        code.removeClass('bg-danger text-white')
        if (value === '') {
            return;
        }
        if (value === history.template) {
            code.addClass('bg-success text-white')
            console.log('bg-success text-white')
            history = { ...history, ...{ name: "OK", pin: 10 } }
        } else {
            code.addClass('bg-danger text-white')
            console.log('bg-danger text-white')
            history = { ...history, ...{ name: "NG", pin: 12 } }
        }

        if (isRunning && isSensor && !isSubmit && history.model !== '' && history.template !== '' && history.code !== '') {
            isSubmit = true

            $.ajax({
                url: 'api/history/post',
                type: 'POST',
                contentType: "application/json; charset=utf-8",
                dataType: "json",
                data: JSON.stringify(history),
                success: function (res) {
                    DevExpress.ui.notify('Done', 'success', 600);
                    totalQty.val(res.Ok + res.Ng);
                    okQty.val(res.Ok);
                    ngQty.val(res.Ng);
                    okRate.val(res.Ok + res.Ng === 0 ? 0 : (res.Ok * 100 / (res.Ok + res.Ng)).toFixed(2));
                    ngRate.val(res.Ok + res.Ng === 0 ? 0 : (res.Ng * 100 / (res.Ok + res.Ng)).toFixed(2));
                },
                error: function (xhr, ajaxOptions, thrownError) {
                    DevExpress.ui.notify(thrownError + "\r\n" + xhr.statusText + "\r\n" + xhr.responseText, 'error', 600);
                }
            });
        }
    })
    running.on('click', function () {
        onRunning();
    })
    setting.on('click', function () {
        loginCallback(onSetting);
    })

    connection.start().then(function () {
        connection.on("Monitor", function (data) {
            data = JSON.parse(data);
            let sensorData = data.find(i =>i.Type==="INPUT" && i.Pin === sensorPin);
            if (sensorData !== null) {
                if (sensorData.Value) {
                    isSensor = true;
                    sensorOn.removeClass('btn-outline-success')
                    sensorOn.addClass('btn-success')
                    sensorOff.removeClass('btn-danger')
                    sensorOff.addClass('btn-outline-danger')
                    console.log('started')
                } else {
                    isSubmit = false;
                    isSensor = false;
                    history.code = '';
                    code.val(history.code);
                    sensorOn.removeClass('btn-success')
                    sensorOn.addClass('btn-outline-success')
                    sensorOff.removeClass('btn-outline-danger')
                    sensorOff.addClass('btn-danger')
                    console.log('finished')
                }
            }
        });
    }).catch(function (err) {
        return console.error(err.toString());
    });
});