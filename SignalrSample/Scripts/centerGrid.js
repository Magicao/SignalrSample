//var notification;
(function () {

    var notifi = $.connection.Notification;
    notifi.client.ShowMessage = function (level, message) {
        notification.show({
            title: "Message",
            message: message
        }, level);
        if (confirm("Data is update, refresh the data?"))
        {
            dataSource.read();
        }        
    };

   // $.connection.hub.start();
        var dataSource = new kendo.data.DataSource({
            transport: {
                read: {                    
                    url: "/Home/GetCenterList",
                    dataType: "json",
                    type: "post",
                    contentType: "application/json",
                },
                parameterMap: function (options, type) {
                    if (type == "read")
                    {
                        return JSON.stringify(options);
                       // return customJSONstringify(options);
                    }                    
                }
            },
            pageSize:10,
            serverPaging: true,
            serverSorting: true,
            serverFiltering: true,
            serverAggregates: true,
            serverGrouping:true,
            aggregate: [
                    { field: "Quota", aggregate: "max" },
                    { field: "Quota", aggregate: "sum" }
                ],
            error: function (e) {
                console.log(e.errors);
            },
            schema: {
                data: "DataList",
                total: "TotailRecords",
                aggregates: "Aggregates",
                model: {
                    id: "CenterId",
                    fields: {
                        CenterId:{type:"number"},
                        CenterName: { type: "string" },
                        CenterAddress: { type: "string" },
                        Quota: { type: "number" },
                        CreateOn: { type: "date" }
                    }
                }
            }
        });

        $("#Grid").kendoGrid({
            dataSource: dataSource,
            height: 500,
            //groupable: true,
            filterable: true,
            resizable: true,
           // columnMenu:true,
            pageable: {
                refresh: true,
                pageSizes: [10, 25, 50, 100]
            },
            sortable: {
                mode: "multiple",
                allowUnsort: true
            },
            columns: [
               // { field:"CenterId", title:"Center Id", footerTemplate:"Count:#: count #" },
               { field: "CenterId", title: "Center Id" },
                { field: "CenterName", title: "Center Name", groupable:true },
                { field: "CenterAddress", title: "Center Address" },
                //{ field: "Quota", title: "Center Quota", footerTemplate: "Sum: #: sum # Max: #: max #" },
                 { field: "Quota", title: "Center Quota"},
                { field: "CreateOn", title: "Create Time", format: "{0:dd/MMM/yyyy}" }
            ]
        });

        var notification = $("#notification").kendoNotification({
            position: {
                pinned: true,
                top: 30,
                right: 30
            },
            autoHideAfter: 3000,
            stacking: "down",
            templates: [{
                type: "info",
                template: $("#emailTemplate").html()
            }, {
                type: "error",
                template: $("#errorTemplate").html()
            }, {
                type: "upload-success",
                template: $("#successTemplate").html()
            }]

        }).data("kendoNotification");       

        $("#saveCenter").on("click", function () {
            var centerName = $.trim($("#centerName").val());
            var centerQuota = $.trim($("#centerQuota").val());
            var address = $.trim($("#centerAddress").val());
            if (centerName && centerQuota != "") {
                $.post("/Home/CreateCenter", { CenterName: centerName, Quota: centerQuota, CenterAddress: address }, function (res) {
                    if (res.IsSuccessful) {                        
                        //$('#AddCenterForm').modal('hide');           
                        debugger;
                        notifi.server.sendServiceMessage("upload-success", res.Message)
                            .done(function () {
                                console.log('Sent message success!');
                            })
                            .fail(function (e) {
                                console.warn(e);
                            });                        
                    } else {
                        notification.show({
                            title: "Error Message",
                            message: res.Message
                        }, "error");
                    }
                });
            }
        })

        $(".custom-close").on('click', function () {
            $('#myModal').modal('hide');
        });

        var myClientName = $('#Placeholder').val();
        var chat = $.connection.chat;
    // Declare a function on the chat hub so the server can invoke it
        chat.client.addSomeMessage = function (clientName, message) {
            writeEvent('<b>' + clientName + '</b> To Every Body: ' + message, 'event-message');
        };

        $("#broadcast").click(function () {
           // var a = chat;
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

        $("#btnPostValueTest").on("click", function () {
            debugger;
            var data = {
                list: dataSource.data(),
                model:{CenterName:"cccc"}
            }
        $.ajax({
            type: "post",
            url: "/Home/GetList",
            datatype: "json",
            data: JSON.stringify(data),
            contentType: "application/json; charset=utf-8",
        });
       
    });

})();

//This function sends json request asynchronously to server based on given parameteres
$.fn.postJsonData = function (actionUrl, postdata, onSuccess) {
    $(this).sendJsonRequest("post", actionUrl, postdata, onSuccess);
};

//This function sends json request asynchronously to server based on given parameteres
$.fn.sendJsonRequest = function (type, actionUrl, postdata, onSuccess) {
    $.ajax({
        url: actionUrl,
        type: type,
        data: postdata ? JSON.stringify(postdata) : null,
        datatype: "json",
        cache: false,
        contentType: "application/json; charset=utf-8",
        complete: $.OnAjaxComplete,
        success: function (model) {
            if (model && model.SysInfo) {
                onSuccess(model.Data);
            } else {
                if (model.Data) {
                    onSuccess(model);
                } else {
                    $.OnAjaxLogin();
                }
            }
        },
        error: $.OnAjaxError
    });
};


function customJSONstringify(obj) {
    return JSON.stringify(obj).replace(/\/Date/g, "\\\/Date").replace(/\)\//g, "\)\\\/");
}

function downLoadPdf(flag) {
    window.location.href = "/Home/ExportPdf?flag=" + flag;
}


function downLoadPdf2() {
    window.location.href = "/Home/ReportNetExport";
}