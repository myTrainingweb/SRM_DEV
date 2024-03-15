

var minDate = '-90d';
var maxDate = '+0d';

//Date Picker Function
function DatePicker(mind, maxd) {
    if (mind == null) {
        $("#dtpTo, #dtpFrom").datepicker({
            format: 'yyyy.mm.dd',
            autoclose: true,
            startDate: '-30d',
            endDate: '+0d'
        });
    }
    else {
        $("#dtpTo, #dtpFrom").datepicker({
            format: 'yyyy.mm.dd',
            autoclose: true,
            startDate: '-90d',
            endDate: '+0d'
        });
    }
}

function DatePicker1(mind, maxd) {
    $("#dtpTo, #dtpFrom").datepicker({
        format: 'yyyy.mm.dd',
        autoclose: true,
        startDate: mind,
        endDate: maxd
    });
}

$(document).ajaxStart(function () {
    $('#wait').show();
}).ajaxStop(function () {
    $('#wait').hide();
});

//For Encrypt Link Information  

//function EncLinkInf(){  //Add By Kamlesh
//    $(document).ready(function () {
//        setTimeout(function () {

//            $('a[href]#no-link').each(function () {
//                var href = this.href;

//                $(this).removeAttr('href').css('cursor', 'pointer').click(function () {
//                    if (href.toLowerCase().indexOf("#") >= 0) {

//                    } else {
//                        window.open(href, '_blank');
//                    }
//                });
//            });

//        }, 500);
//    });
//}

function ShowOption(menuTitle, menuUrl, designedBy, fromIndex) {
    $('#modalFooter > #designedby').empty();
    $("#renderBody").html("");
    $('#modalHeader').empty();
    $('#modalHeader').append(menuTitle);
    $('#modalFooter > #designedby').append(designedBy);
    if (fromIndex == undefined) {
        $('#modalBody').load(menuUrl, function () {
            $('#option').modal({ show: true, backdrop: false });
        });
    }
    else {
        $('#option').modal({ show: true, backdrop: false });
    }
}

function ToggleDataList(val) {
    if (val == true) {
        $('#dataList').show();
        MenuHide();
        $("#wait").css("display", "none");
    }
    else {
        $('#dataList').hide();
        MenuHide();
    }
}

function ToggleDataListPartShrtge(id, val) {
    if (val == true) {
        $('#' + id).show();
        MenuHide();
        $("#wait").css("display", "none");
    }
    else {
        $('#' + id).hide();
        MenuHide();
    }
}

function MenuHide() {
    $('#main').addClass("skin-blue sidebar-mini lte-fixed sidebar-collapse");
}

function DisabledPaste(inputId) {
    $('#' + inputId).on("cut copy paste", function (e) {
        e.preventDefault();
    });
}
function MakeDataGrid(tableId, data, tableHeight, columns, search, isScrollX, footerFunction, detailed, reportTitleId, rowSelection) {
    var isFooter = footerFunction === undefined || footerFunction === false ? false : true;
    var isDetailed = detailed === undefined ? false : true;
    var reportTitle = '';

    if (reportTitleId === undefined)
        reportTitle = $('.box-title').text();
    else
        reportTitle = reportTitleId;

    var gridHeight = 0;

    isScrollX = isScrollX === undefined ? false : true;
    if (search === undefined) { search = true; }

    var gridButtonsClass = '';
    var gridSearchBoxClass = '';
    if (isDetailed === true) {
        gridButtonsClass = 'btn-header btn-box-tool';
        gridSearchBoxClass = 'bg-white  txtSearch';
        gridButtonSize = 20;
    }
    else {
        gridButtonsClass = 'btn-header btn-box-tool';
        gridSearchBoxClass = 'btn-box-tool bg-white txtSearch';
        gridButtonSize = 20;
    }



    if (search === true) {
        var boxtool = '<input class="' + gridSearchBoxClass + '" id="txtSearch" type="text" placeholder="Search.." style="padding-left: 10px !important;" >';
        $('#buttons').append(boxtool);
    }

    var table = $('#' + tableId).DataTable({
        order: [],
        createdRow: rowSelection,         // add Parameter for row selection in PO Detail // Added By Tahir
        data: data,
        columns: columns,
        paging: true,
        ordering: true,
        searching: search,
        deferRender: true,
        destroy: true,
        pagingType: 'full',
        pageLength: 100,
        scrollCollapse: false,
        scrollX: isScrollX,
        processing: true,
        //serverSide: true,
        filter: true,
        orderMulti: false,
        //scrollY: gridHeight,

        footerCallback: footerFunction,
        language: {
            emptyTable: "<p style=\"text-align:left;\">No record(s) found.</p>",
            zeroRecords: "<p style=\"text-align:left;\">No matching record(s) found</p>",
            info: "Showing _START_ to _END_ of _TOTAL_ entries",
            oPaginate: {
                sNext: '<i class="fa fa-forward"></i>',
                sPrevious: '<i class="fa fa-backward"></i>',
                sFirst: '<i class="fa fa-step-backward"></i>',
                sLast: '<i class="fa fa-step-forward"></i>'
            }
        },
        "dom": "<'row'<'col-sm-12'tr>>" +
            "<'row'<'col-sm-3'i><'#designedBy.col-sm-7'><'col-sm-2'p>>",
        buttons: [
            {
                extend: 'copyHtml5',
                footer: true,
                text: '<span><span class="fa-stack"><i class="fa fa-circle fa-stack-2x" style="color: rgb(244, 164, 37);"></i><i class="fa fa-copy fa-stack-1x" style="color: white;"></i></span></span>',
                titleAttr: 'Copy',
                title: reportTitle,
                filename: 'DataExport',
                exportOptions: {
                    //columns: ':visible'
                    columns: 'th:not(.notexport)' // Excluding columns while exporting
                }
            },
            {
                extend: 'excelHtml5',
                footer: true,
                text: '<span><span class="fa-stack"><i class="fa fa-circle fa-stack-2x" style="color: rgb(0, 179, 98);"></i><i class="fa fa-file-excel fa-stack-1x" style="color: white;"></i></span></span>',
                titleAttr: 'Export to excel',
                title: reportTitle,
                filename: 'DataExport', //Added By Tahir
                exportOptions: {
                    //columns: ':visible'
                    columns: 'th:not(.notexport)'
                }
            },
            {
                extend: 'print',
                footer: true,
                text: '<span><span class="fa-stack"><i class="fa fa-circle fa-stack-2x" style="color: rgb(0, 204, 255);"></i><i class="fa fa-print fa-stack-1x" style="color: white;"></i></span></span>',
                titleAttr: 'Print',
                title: '',
                messageTop: function () { return '<h4>' + reportTitle + '</h4>'; },
                filename: 'DataExport',
                exportOptions: {
                    // columns: ':visible'
                    columns: 'th:not(.notexport)'
                }
            }
        ],
        select: true
    });
    table.buttons().container()
        .appendTo('#buttons');

    table.button(0).nodes().removeClass('btn btn-default buttons-copy buttons-html5');
    table.button(0).nodes().addClass(gridButtonsClass);

    table.button(1).nodes().removeClass('btn btn-default buttons-excel buttons-html5');
    table.button(1).nodes().addClass(gridButtonsClass);

    table.button(2).nodes().removeClass('btn btn-default buttons-excel buttons-html5');
    table.button(2).nodes().addClass(gridButtonsClass);

    var dsgndby = $('.modal-footer > #designedby ').html();

    if (dsgndby === "") {
        $('#designedBy').html($('.desgndBy').html());
    }

    var layoutHeader = $('nav').innerHeight();
    var cardHeader = $('.box-header').innerHeight();
    var tableHeader = $('.dataTables_scrollHead').innerHeight();
    var tableFooter = 0;
    var cardFooterH = 30;
    var topHeader = $('#topHeader').innerHeight();

    if (isFooter === true)
        tableFooter = $('.dataTables_scrollFoot').innerHeight();

    var browserHeight = window.innerHeight;
    if (tableHeight === 0) {
        if (layoutHeader === undefined)
            gridHeight = browserHeight - (cardHeader + tableHeader + tableFooter + cardFooterH);
        else
            gridHeight = browserHeight - (layoutHeader + cardHeader + tableHeader + tableFooter + cardFooterH);
    }
    else { gridHeight = tableHeight; }

    if (topHeader > 0)
        gridHeight = gridHeight - topHeader;

    $('.dataTables_scrollBody').css('height', gridHeight + 'px');
    table.columns.adjust().draw();

    $('.dataTables_filter').hide();
    $('#txtSearch').on('keyup change', function () {
        table.search(this.value).draw();
    });

    return table;
}



function MakeMeGrid(tableId, data, tableHeight, columnWidth, search, isScrollX, footerFunction, isDetailed, reportTitleId, rowSelection) {

    var isFooter = (footerFunction == undefined ? false : true);
    var isDetailed = (isDetailed == undefined ? false : true);
    var reportTitle = '';

    if (reportTitleId == undefined)
        reportTitle = $('.box-title').text();
    else
        reportTitle = reportTitleId;

    var gridHeight = 0;

    isScrollX = (isScrollX == undefined ? false : true);
    if (search == undefined) { search = true; }

    var gridButtonsClass = '';
    var gridSearchBoxClass = '';
    if (isDetailed == true) {
        gridButtonsClass = 'btn-header btn-box-tool';
        gridSearchBoxClass = 'bg-white  txtSearch';
        gridButtonSize = 20;
    }
    else {
        gridButtonsClass = 'btn-header btn-box-tool';
        gridSearchBoxClass = 'btn-box-tool bg-white txtSearch'
        gridButtonSize = 20;
    }

    if (search == true) {
        var boxtool = '<input class="' + gridSearchBoxClass + '" id="txtSearch" type="text" placeholder="Search.." style="padding-left: 10px !important;" >';
        $('#buttons').append(boxtool);
    }

    var table = $('#' + tableId).DataTable({
        dom: 'Bfrtip',
        data: data,
        deferRender: true,
        //scrollY: gridHeight,
        paging: true,
        ordering: true,
        scrollCollapse: false,
        pagingType: 'full',
        destroy: true,
        searching: search,
        order: [],
        createdRow: rowSelection, // add Parameter for row selection in PO Detail // Added By Tahir
        pageLength: 100,
        columnDefs: columnWidth,
        drawCallback: footerFunction,
        scrollX: isScrollX,
        language: {
            emptyTable: "<p style=\"text-align:left;\">No record(s) found.</p>",
            zeroRecords: "<p style=\"text-align:left;\">No matching record(s) found</p>",
            info: "Showing _START_ to _END_ of _TOTAL_ entries",
            oPaginate: {
                sNext: '<i class="fa fa-forward"></i>',
                sPrevious: '<i class="fa fa-backward"></i>',
                sFirst: '<i class="fa fa-step-backward"></i>',
                sLast: '<i class="fa fa-step-forward"></i>'
            }
        },

        "dom": "<'row'<'col-sm-12'tr>>" +
            "<'row'<'col-sm-5'i><'#designedBy.col-sm-3'><'col-sm-4'p>>",
        buttons: [
            {
                extend: 'copyHtml5',
                footer: true,
                text: '<span><span class="fa-stack"><i class="fa fa-circle fa-stack-2x" style="color: rgb(244, 164, 37);"></i><i class="fa fa-copy fa-stack-1x" style="color: white;"></i></span></span>',
                titleAttr: 'Copy',
                title: reportTitle,
                filename: 'DataExport',
                exportOptions: {
                    //columns: ':visible'
                    columns: 'th:not(.notexport)' // Excluding columns while exporting
                }
            },
            {
                extend: 'excelHtml5',
                footer: true,
                text: '<span><span class="fa-stack"><i class="fa fa-circle fa-stack-2x" style="color: rgba(0, 115, 62, 1);"></i><i class="fa fa-file-excel fa-stack-1x" style="color: white;"></i></span></span>',
                titleAttr: 'Export to excel',
                title: reportTitle,
                filename: 'DataExport', //Added By Tahir
                exportOptions: {
                    //columns: ':visible'
                    columns: 'th:not(.notexport)'
                }
            },
            {
                extend: 'print',
                footer: true,
                text: '<span><span class="fa-stack"><i class="fa fa-circle fa-stack-2x" style="color: rgb(0, 204, 255);"></i><i class="fa fa-print fa-stack-1x" style="color: white;"></i></span></span>',
                titleAttr: 'Print',
                title: reportTitle,
                messageTop: function () { return '<h4>' + reportTitle + '</h4>' },
                filename: 'DataExport',
                exportOptions: {
                    // columns: ':visible'
                    columns: 'th:not(.notexport)'
                }
            }
        ],
        select: true
    });

    table.buttons().container()
        .appendTo('#buttons');

    table.button(0).nodes().removeClass('btn btn-default buttons-copy buttons-html5');
    table.button(0).nodes().addClass(gridButtonsClass);

    table.button(1).nodes().removeClass('btn btn-default buttons-excel buttons-html5');
    table.button(1).nodes().addClass(gridButtonsClass);

    table.button(2).nodes().removeClass('btn btn-default buttons-excel buttons-html5');
    table.button(2).nodes().addClass(gridButtonsClass);

    var dsgndby = $('.modal-footer > #designedby ').html();

    if (dsgndby == "") {
        $('#designedBy').html($('.desgndBy').html());
    }

    var layoutHeader = $('nav').innerHeight();
    var cardHeader = $('.box-header').innerHeight();
    var tableHeader = $('.dataTables_scrollHead').innerHeight();
    var tableFooter = 0;
    var cardFooterH = 30;
    var topHeader = $('#topHeader').innerHeight();

    if (isFooter == true)
        tableFooter = $('.dataTables_scrollFoot').innerHeight();

    var browserHeight = window.innerHeight;
    if (tableHeight == 0) {
        if (layoutHeader == undefined)
            gridHeight = browserHeight - (cardHeader + tableHeader + tableFooter + cardFooterH);
        else
            gridHeight = browserHeight - (layoutHeader + cardHeader + tableHeader + tableFooter + cardFooterH);
    }
    else { gridHeight = tableHeight; }

    if (topHeader > 0)
        gridHeight = gridHeight - topHeader;

    $('.dataTables_scrollBody').css('height', gridHeight + 'px');
    table.columns.adjust().draw();

    $('.dataTables_filter').hide();
    $('#txtSearch').on('keyup change', function () {
        table.search(this.value).draw();
    });
    return table;
}

function MakeMeBabyGridCopy(tableId, data, tableHeight, columnWidth, buttonid, isScrollX, reportTitle, footerFunction) {
    isScrollX = (isScrollX == undefined ? false : true);
    var isFooter = (footerFunction == undefined ? false : true);
    var drFlag;
    var order = [];
    if (columnWidth == undefined || columnWidth == null) {
        drFlag = false;
        columnWidth = null;
    }
    else {
        drFlag = true;
    }

    var table = $('#' + tableId).DataTable({
        data: data,
        deferRender: true,
        paging: false,
        searching: false,
        info: false,
        scrollY: tableHeight,
        columnDefs: columnWidth,
        scrollX: isScrollX,
        scrollCollapse: false,
        fixedHeader: false,
        ordering: true,
        order: [],
        footer: isFooter,
        drawCallback: footerFunction,
        language: {
            emptyTable: "<p style=\"text-align:left;\">No record(s) found.</p>"
        },
        buttons: [
            {
                extend: 'copyHtml5',
                footer: true,
                text: '<span><span class="fa-stack"><i class="fa fa-circle fa-stack-2x" style="color: rgb(244, 164, 37);"></i><i class="fa fa-copy fa-stack-1x" style="color: white;"></i></span></span>',
                titleAttr: 'Copy',
                title: reportTitle,
                filename: 'DataExport',
                exportOptions: {
                    columns: 'th:not(.notexport)' // Excluding columns while exporting
                }
            },
            {
                extend: 'excelHtml5',
                footer: true,
                text: '<span><span class="fa-stack"><i class="fa fa-circle fa-stack-2x" style="color: rgba(0, 115, 62, 1);"></i><i class="fa fa-file-excel fa-stack-1x" style="color: white;"></i></span></span>',
                titleAttr: 'Export to excel',
                title: reportTitle,
                filename: 'DataExport',
                exportOptions: {
                    columns: 'th:not(.notexport)' // Excluding columns while exporting
                }
            },
            {
                extend: 'print',
                footer: true,
                text: '<span><span class="fa-stack "><i class="fa fa-circle fa-stack-2x" style="color: rgb(0, 204, 255);"></i><i class="fa fa-print fa-stack-1x" style="color: white;"></i></span></span>',
                titleAttr: 'Print',
                title: '',
                messageTop: function () { return '<h4>' + reportTitle + '</h4>' },
                filename: 'DataExport',
                exportOptions: {
                    columns: 'th:not(.notexport)' // Excluding columns while exporting
                }
            }
        ],
        select: true
    });

    table.buttons().container()
        .appendTo('#' + buttonid);

    table.button(0).nodes().removeClass('btn btn-default buttons-copy buttons-html5');
    table.button(0).nodes().addClass('btn-baby btn-box-tool-baby ');

    table.button(1).nodes().removeClass('btn btn-default buttons-excel buttons-html5');
    table.button(1).nodes().addClass('btn-baby btn-box-tool-baby ');

    table.button(2).nodes().removeClass('btn btn-default buttons-excel buttons-html5');
    table.button(2).nodes().addClass('btn-baby btn-box-tool-baby ');

    $('.dataTables_filter').hide();
}

function MakeMeFrameGrid(tableId, data, tableHeight, columnWidth, buttonId, search, isScrollX, footerFunction, isFramed, reportTitle, isSqlDoc, showPaging) {
    var isFooter = (footerFunction == undefined || footerFunction == false ? false : true);
    isFramed = (isFramed == undefined || isFramed == false ? false : true);
    isScrollX = (isScrollX == undefined ? false : true);
    if (search == undefined) { search = true; }
    reportTitle = (reportTitle == undefined ? $('.box-title').text() : reportTitle);
    showPaging = (showPaging == undefined) ? true : false;

    var gridHeight = 0;
    var gridButtonsClass = '';
    var gridButtonSize = 0;
    gridButtonsClass = 'btn-header btn-box-tool';
    gridSearchBoxClass = 'bg-white';
    gridButtonSize = 20;

    var cardHeader = $('.box-header').innerHeight();
    var tableHeader = $('#' + tableId + ' > thead').innerHeight();
    var tableFooter = 0;
    var cardFooterH = 0;

    if (showPaging)
        cardFooterH = 30;

    if (isFooter == true)
        tableFooter = $('#' + tableId + ' > tfoot').innerHeight();

    var browserHeight = tableHeight;
    tableFooter = (tableFooter == undefined ? 0 : tableFooter);
    gridHeight = browserHeight - (cardHeader + tableHeader + tableFooter + cardFooterH);

    //For SQL Documentation
    if (search == true) {
        if (isFramed) {
            $('#trHeadSearch').empty();
            var boxtool = '<input class="txtSearch-frame" id="txtFrameSearch" type="text" placeholder="Search.." style="padding-left: 10px !important;" >';
            $('#trHeadSearch').append(boxtool);
        }
        else {
            $('#' + buttonId).empty();
            if (isSqlDoc == true)
                AddSqlDoc(buttonId, gridButtonSize);
            var boxtool = '<input class="txtSearch-frame" id="txtFrameSearch' + tableId + '" type="text" placeholder="Search.." style="padding-left: 10px !important;" >';
            $('#' + buttonId).append(boxtool);
        }
    }
    else {
        if (isFramed) {
            $('#trHeadSearch').empty();
            var boxtool = '<input class="txtSearch-frame" id="txtFrameSearch" type="text" placeholder="Search.." style="padding-left: 10px !important;" >';
            $('#trHeadSearch').append(boxtool);
        }
        else {
            $('#' + buttonId).empty();
            if (isSqlDoc == true)
                AddSqlDoc(buttonId, gridButtonSize);
        }
    }

    var table = $('#' + tableId).DataTable({
        dom: 'Bfrtip',
        dom: 'lBfrtip',
        data: data,
        deferRender: true,
        scrollY: gridHeight,
        paging: showPaging,
        bInfo: showPaging,
        ordering: true,
        scrollCollapse: false,
        pagingType: 'full',
        searching: search,
        order: [],
        pageLength: 100,
        columnDefs: columnWidth,
        drawCallback: footerFunction,
        scrollX: isScrollX,
        language: {
            emptyTable: "<p style=\"text-align:left;\">No record(s) found.</p>",
            zeroRecords: "<p style=\"text-align:left;\">No matching record(s) found</p>",
            info: "Showing _START_ to _END_ of _TOTAL_ entries",
            oPaginate: {
                sNext: '<i class="fa fa-forward"></i>',
                sPrevious: '<i class="fa fa-backward"></i>',
                sFirst: '<i class="fa fa-step-backward"></i>',
                sLast: '<i class="fa fa-step-forward"></i>'
            }
        },

        "dom": "<'row'<'col-sm-12'tr>>" +
            "<'row'<'col-sm-3'i><'#designedBy.col-sm-7'><'col-sm-2'p>>", // Added By Tahir
        buttons: [
            {
                extend: 'copyHtml5',
                footer: true,
                text: '<span><span class="fa-stack"><i class="fa fa-circle fa-stack-2x" style="color: rgb(244, 164, 37);"></i><i class="fa fa-copy fa-stack-1x" style="color: white;"></i></span></span>',
                titleAttr: 'Copy',
                title: reportTitle,
                filename: 'DataExport',
                exportOptions: {
                    columns: 'th:not(.notexport)'
                }
            },
            {
                extend: 'excelHtml5',
                footer: true,
                text: '<span><span class="fa-stack"><i class="fa fa-circle fa-stack-2x" style="color: rgb(0, 179, 98);"></i><i class="fa fa-file-excel fa-stack-1x" style="color: white;"></i></span></span>',
                titleAttr: 'Export to excel',
                title: reportTitle,
                filename: 'DataExport',
                exportOptions: {
                    columns: 'th:not(.notexport)'
                }
            },
            {
                extend: 'print',
                footer: true,
                text: '<span><span class="fa-stack"><i class="fa fa-circle fa-stack-2x" style="color: rgb(0, 204, 255);"></i><i class="fa fa-print fa-stack-1x" style="color: white;"></i></span></span>',
                titleAttr: 'Print',
                title: '',
                messageTop: function () { return '<h4>' + reportTitle + '</h4>' },
                filename: 'DataExport',
                exportOptions: {
                    columns: 'th:not(.notexport)'
                }
            },
        ],
        select: true
    });

    table.buttons().container()
        .appendTo('#' + buttonId);

    table.button(0).nodes().removeClass('btn btn-default buttons-copy buttons-html5');
    table.button(0).nodes().addClass(gridButtonsClass);

    table.button(1).nodes().removeClass('btn btn-default buttons-excel buttons-html5');
    table.button(1).nodes().addClass(gridButtonsClass);

    table.button(2).nodes().removeClass('btn btn-default buttons-excel buttons-html5');
    table.button(2).nodes().addClass(gridButtonsClass);

    table.columns.adjust().draw();
    $('.dataTables_filter').hide();
    $('#txtFrameSearch' + tableId).on('keyup change', function () {
        table.search(this.value).draw();
    });
    return table;
}


function MakeMeDMLGrid(tableId, data, tableHeight, columnWidth, search, isScrollX, reportTitle, buttonId) {
    var rptCode = $('#rptCode').val(); // for sql docs.
    var rptTitle = $('#rptTitle').val(); // for sql docs.
    isScrollX = (isScrollX == undefined ? false : true);
    if (search == undefined) { search = true; }

    reportTitle = (reportTitle == undefined ? $('.box-title').text() : reportTitle);
    buttonId = (buttonId == undefined ? 'buttons' : buttonId);

    var gridButtonsClass = '';
    var gridSearchBoxClass = '';
    var gridButtonSize = 0;

    gridButtonsClass = 'btn-header btn-box-tool';
    gridSearchBoxClass = 'btn-box-tool bg-white txtSearch'
    gridButtonSize = 20;

    //AddSqlDoc("buttons");
    if (search == true) {
        $('#buttons').empty();
        AddSqlDoc(buttonId, gridButtonSize);
        var boxtool = '<input class="txtSearch-frame" id="txtSearch" type="text" placeholder="Search.." style="padding-left: 10px !important;" >';
        $('#buttons').append(boxtool);
    }

    var table = $('#' + tableId).DataTable({
        dom: 'Bfrtip',
        data: data,
        //scrollY: gridHeight,
        paging: true,
        pagingType: 'full',
        destroy: true,
        searching: search,
        order: [],
        pageLength: 100,
        columnDefs: columnWidth,
        scrollX: isScrollX,
        select: true,
        language: {
            emptyTable: "<p style=\"text-align:left;\">No record(s) found.</p>",
            zeroRecords: "<p style=\"text-align:left;\">No matching record(s) found</p>",
            info: "Showing _START_ to _END_ of _TOTAL_ entries",
            oPaginate: {
                sNext: '<i class="fa fa-forward"></i>',
                sPrevious: '<i class="fa fa-backward"></i>',
                sFirst: '<i class="fa fa-step-backward"></i>',
                sLast: '<i class="fa fa-step-forward"></i>'
            }
        },
        "dom": "<'row'<'col-sm-12'tr>>" +
            "<'row'<'col-sm-5'i><'#designedBy.col-sm-3'><'col-sm-4'p>>",
        buttons: [
            {
                extend: 'excelHtml5',
                text: '<span><span class="fa-stack"><i class="fa fa-circle fa-stack-2x" style="color: rgba(0, 115, 62, 1);"></i><i class="fa fa-file-excel fa-stack-1x" style="color: white;"></i></span></span>',
                titleAttr: 'Export to excel',
                title: '',
                filename: 'DataExport', //Added By Tahir
                exportOptions: {
                    columns: 'th:not(.notexport)'
                }
            },
            {
                extend: 'print',
                footer: true,
                text: '<span><span class="fa-stack"><i class="fa fa-circle fa-stack-2x" style="color: rgb(0, 204, 255);"></i><i class="fa fa-print fa-stack-1x" style="color: white;"></i></span></span>',
                title: '',
                messageTop: function () { return '<h4>' + reportTitle + '</h4>' },
                titleAttr: 'Print',
                filename: 'DataExport',
                exportOptions: {
                    columns: 'th:not(.notexport)'
                }
            },
        ],
    });

    table.buttons().container().appendTo('#buttons');

    table.button(0).nodes().removeClass('btn btn-default buttons-copy buttons-html5');
    table.button(0).nodes().addClass(gridButtonsClass);

    table.button(1).nodes().removeClass('btn btn-default buttons-excel buttons-html5');
    table.button(1).nodes().addClass(gridButtonsClass);

    var dsgndby = $('.modal-footer > #designedby ').html();
    if (dsgndby == "") { $('#designedBy').html($('.desgndBy').html()) }//.css({ "padding": "8px" }); }

    var layoutHeader = $('nav').innerHeight();
    var cardHeader = $('.box-header').innerHeight();
    var tableHeader = $('.dataTables_scrollHead').innerHeight();
    var tableFooter = 0;
    var cardFooterH = 28;

    var browserHeight = window.innerHeight;
    if (tableHeight == 0) {
        if (layoutHeader == undefined)
            gridHeight = browserHeight - (cardHeader + tableHeader + tableFooter + cardFooterH + 4);
        else
            gridHeight = browserHeight - (layoutHeader + cardHeader + tableHeader + tableFooter + cardFooterH + 4);
    }
    else
        gridHeight = (window.innerHeight * tableHeight) / 100;

    $('.dataTables_scrollBody').css('height', gridHeight + 'px');
    table.columns.adjust().draw();

    $('.dataTables_filter').hide();
    $('#txtSearch').on('keyup change', function () {
        table.search(this.value).draw();
    });
    return table;
}

function AddSqlDoc(buttonId, gridButtonSize, isBaby) {
   
    //For SQL Documentation
    var rptCode = $('#rptCode').val(); // for sql docs.
    var rptTitle = $('#rptTitle').val(); // for sql docs.

    var className = 'btn-header btn-box-tool';
    if (isBaby)
        className = 'btn-baby btn-box-tool-baby';

    if (rptCode == undefined) {
        var rptDtlCode = $('#rptcode').val();
        var rptDtlTitle = reportTitle.substring(0, reportTitle.indexOf('>'));
        //$('#' + buttonId).empty();
        var qrytool = '<a  class="' + className + '" id="showQuery"  href="\\SqlDocs\\Index?rptCode=' + rptDtlCode + '&rptTitle=' + rptDtlTitle + '" title="How they do that" target="_blank">';
        qrytool += '<span><span><span class="fa-stack sqlDocFrameGrid" ><i class="fa fa-circle fa-stack-2x" style="color: rgb(221, 75, 57);"></i><i class="fa fa-database fa-stack-1x" style="color: white;"></i></span></span></span>';
        qrytool += '</a>';
        //$('#' + buttonId).append(qrytool);
    }

    else {
        //$('#' + buttonId).empty();
        var qrytool = '<a class="' + className + '" id="showQuery" href="\\SqlDocs\\Index?rptCode=' + rptCode + '&rptTitle=' + rptTitle + '" title="How they do that" target="_blank">';
        qrytool += '<span><span><span class="fa-stack sqlDocFrameGrid" ><i class="fa fa-circle fa-stack-2x" style="color: rgb(221, 75, 57);"></i><i class="fa fa-database fa-stack-1x" style="color: white;"></i></span></span></span>';
        qrytool += '</a>';
        //$('#' + buttonId).append(qrytool);
    }
    if (isAdmin === "True") {
        $('#' + buttonId).empty();
        $('#' + buttonId).append(qrytool);
    }
}

function ToggleFullScreen(srcId, targetId) {
    var imgControl = $('#' + srcId).children('i');
    var targetControl = document.getElementById(targetId);

    if ((document.fullScreenElement !== undefined && document.fullScreenElement === null) || (document.msFullscreenElement !== undefined && document.msFullscreenElement === null) || (document.mozFullScreen !== undefined && !document.mozFullScreen) || (document.webkitIsFullScreen !== undefined && !document.webkitIsFullScreen)) {
        if (targetControl.reqduestFullScreen) {
            targetControl.requestFullScreen();
        } else if (targetControl.mozRequestFullScreen) {
            targetControl.mozRequestFullScreen();
        } else if (targetControl.webkitRequestFullScreen) {
            targetControl.webkitRequestFullScreen(Element.ALLOW_KEYBOARD_INPUT);
        } else if (targetControl.msRequestFullscreen) {
            targetControl.msRequestFullscreen();
        }
        imgControl.removeClass('fa fa-expand');
        imgControl.addClass('fa fa-compress');

    } else {
        if (document.cancelFullScreen) {
            document.cancelFullScreen();
        } else if (document.mozCancelFullScreen) {
            document.mozCancelFullScreen();
        } else if (document.webkitCancelFullScreen) {
            document.webkitCancelFullScreen();
        } else if (document.msExitFullscreen) {
            document.msExitFullscreen();
        }

        imgControl.removeClass('fa fa-compress');
        imgControl.addClass('fa fa-expand');
    }
}

function PrintMe() {
    window.print();
}

function OpenPage(_url, designedBy) {
    
    //if (designedBy != undefined) {
    //    $('.modal-footer > #designedby').empty();
    //    $('.desgndBy').html('<div class="dataTables_info" id="lstMenus_info" role="status" aria-live="polite">Designed By : ' + designedBy + '</div>');
    //} 
    $.ajax({
        url: _url,
        success: function (data) {
            $("#renderBody").empty()
            $("#renderBody").html(data);
        }
    });
}
function OpenPageNewTab(_url, designedBy) {

    //if (designedBy != undefined) {
    //    $('.modal-footer > #designedby').empty();
    //    $('.desgndBy').html('<div class="dataTables_info" id="lstMenus_info" role="status" aria-live="polite">Designed By : ' + designedBy + '</div>');
    //} 
    $.ajax({
        url: _url,
        success: function (data) {debugger
            //$("#renderBody").empty()
            //$("#renderBody").html(data);
            window.open(_url);
        }
    });
}
function Navigate(report_target, report_URL, report_code, report_title, report_designed_by) {
    report_URL += '?rptCode=' + report_code;
    if (report_target == "option") {
        ShowOption(report_title, report_URL, report_designed_by);
    }
    else {
        report_URL += "&menuTitle=" + report_title;
        OpenPage(report_URL, report_designed_by);
    }
    ActiveReport(report_code, report_title);
    MenuHide();
}

function ActiveReport(rptCode, rptTitle) {
    $('#rptCode').val(rptCode);
    $('#rptTitle').val(rptTitle);
}

function OpenPageforDDL(_url, param, controlId) {
    $.ajax({
        url: _url,
        success: function (data) {
            if (param == undefined) {
                $("#renderBody").html(data);
            }
            else {
                $("#renderBody").html(data);
                $("#" + controlId + " option:selected").attr('value', param);
                $("#" + controlId + " option[value=" + param + "]").attr('selected', null);
            }
        }
    });
}

function OpenView(_url, target) {
    $.ajax({
        url: _url,
        success: function (data) {
            $('#' + target).hide();
            $("#wait").css("display", "none");
            $("#" + target).html(data);
            $('#' + target).show();
        }
    });
}

function validateEntry(string) {
    var str = string.replace(/[#'%!";~.*+?^${}()|[\]\\]/, "");
    return str.trim();
}

function thousands_separators(num) {
    if (num == 0) {
        return "";
    }
       else {

    var num_parts = num.toString().split(".");
    num_parts[0] = num_parts[0].replace(/\B(?=(\d{3})+(?!\d))/g, ",");
    return num_parts.join(".");
      }
}

function AllowNumeric(inputId) {
    $("#" + inputId).on("keypress keyup blur", function (event) {
        $(this).val($(this).val().replace(/[^\d].+/, ""));
        if ((event.which < 48 || event.which > 57)) {
            event.preventDefault();
        }
    });
}

function AllowDecimal(inputId) {
    $("#" + inputId).on("input", function (evt) {
        var self = $(this);
        self.val(self.val().replace(/[^0-9\.]/g, ''));
        if ((evt.which != 46 || self.val().indexOf('.') != -1) && (evt.which < 48 || evt.which > 57)) {
            evt.preventDefault();
        }
    });
}

// For Two Decimal Palces
function decimalPlaces(num, digits) {
    digits = digits == undefined ? "4" : digits;
    var n = num.toFixed(digits);
    return n;
}

function showComments(header, str) {
    $('#commentsheader').html(header);
    $('#commentsbody').html(str);
    $('#comments').modal('show');
}

function showAlert(msgType, bodyMsg) {
    $('#MsgBody').empty();
    $('#MsgType').empty();
    $('#MsgType').html(msgType);
    $('#MsgBody').html(bodyMsg);
    $('#MsgAlert').modal({ show: true });

}

function calcHeight(tableId, heightInPercentage, isDetail, isFooter) {
    isDetail = ((isDetail == undefined || isDetail == false) ? false : true);
    isFooter = ((isFooter == undefined || isFooter == false) ? false : true);
    var gridheader = $('.box-header').innerHeight();
    var tableHeader = $('#' + tableId + ' > thead').innerHeight();
    var _globHeight = 0;

    if (isDetail)
        _globHeight = (window.innerHeight * heightInPercentage) / 100;
    else
        _globHeight = getHeight(heightInPercentage);

    if (isFooter)
        _globHeight = _globHeight - ($('#' + tableId + ' > tfoot').innerHeight());

    return _globHeight - (gridheader + tableHeader + 28);
}

function getHeight(heightInPercentage) {
    var contentPageHeight = $('#renderBody').innerHeight();
    return (contentPageHeight * heightInPercentage) / 100;
}

function MakeMeBabyGridIP(tableId, data, tableHeight, columnWidth, buttonid, isScrollX, reportTitle, footerFunction, lstHeight) {
    isScrollX = (isScrollX == undefined ? false : true);
    var isFooter = (footerFunction == undefined ? false : true);
    var drFlag;
    var order = [];
    if (columnWidth == undefined || columnWidth == null) {
        drFlag = false;
        columnWidth = null;
    }
    else {
        drFlag = true;
    }

    var table = $('#' + tableId).DataTable({
        data: data,
        deferRender: true,
        paging: false,
        searching: false,
        info: false,
        //scrollY: tableHeight,
        columns: columnWidth,
        scrollX: isScrollX,
        scrollCollapse: false,
        fixedHeader: false,
        ordering: true,
        order: [],
        footer: isFooter,
        drawCallback: footerFunction,
        language: {
            emptyTable: "<p style=\"text-align:left;\">No record(s) found.</p>"
        },
        buttons: [
            {
                extend: 'copyHtml5',
                footer: true,
                text: '<span><span class="fa-stack"><i class="fa fa-circle fa-stack-2x" style="color: rgb(244, 164, 37);"></i><i class="fa fa-copy fa-stack-1x" style="color: white;"></i></span></span>',
                titleAttr: 'Copy',
                title: reportTitle,
                filename: 'DataExport',
                exportOptions: {
                    columns: 'th:not(.notexport)' // Excluding columns while exporting
                }
            },
            {
                extend: 'excelHtml5',
                footer: true,
                text: '<span><span class="fa-stack"><i class="fa fa-circle fa-stack-2x" style="color: rgb(0, 179, 98);"></i><i class="fa fa-file-excel fa-stack-1x" style="color: white;"></i></span></span>',
                titleAttr: 'Export to excel',
                title: reportTitle,
                filename: 'DataExport',
                exportOptions: {
                    columns: 'th:not(.notexport)' // Excluding columns while exporting
                }
            },
            {
                extend: 'print',
                footer: true,
                text: '<span><span class="fa-stack "><i class="fa fa-circle fa-stack-2x" style="color: rgb(0, 204, 255);"></i><i class="fa fa-print fa-stack-1x" style="color: white;"></i></span></span>',
                titleAttr: 'Print',
                title: '',
                messageTop: function () { return '<h4>' + reportTitle + '</h4>' },
                filename: 'DataExport',
                exportOptions: {
                    columns: 'th:not(.notexport)' // Excluding columns while exporting
                }
            }
        ],
        select: true
    });

    table.buttons().container()
        .appendTo('#' + buttonid);

    table.button(0).nodes().removeClass('btn btn-default buttons-copy buttons-html5');
    table.button(0).nodes().addClass('btn-baby btn-box-tool-baby ');

    table.button(1).nodes().removeClass('btn btn-default buttons-excel buttons-html5');
    table.button(1).nodes().addClass('btn-baby btn-box-tool-baby ');

    table.button(2).nodes().removeClass('btn btn-default buttons-excel buttons-html5');
    table.button(2).nodes().addClass('btn-baby btn-box-tool-baby ');

    var layoutHeader = $('nav').innerHeight();
    var cardHeader = $('.box-header').innerHeight();
    var tableHeader = $('.dataTables_scrollHead').innerHeight();
    var tableFooter = 0;
    var cardFooterH = 30;

    if (isFooter === true)
        tableFooter = $('.dataTables_scrollFoot').innerHeight();


    var parentHeight = $('#' + lstHeight).innerHeight();
    if (parentHeight == undefined)
        parentHeight = window.innerHeight;
    else
        parentHeight = $('#' + lstHeight).innerHeight();


    var browserHeight = parentHeight;
    if (tableHeight === 0) {
        if (layoutHeader === undefined)
            gridHeight = browserHeight - (tableHeader);
        else
            gridHeight = browserHeight - (layoutHeader + cardHeader + tableHeader + tableFooter + cardFooterH);
    }
    else { gridHeight = tableHeight; }

    //if (topHeader > 0)
    //    gridHeight = gridHeight - topHeader;

    $('.dataTables_scrollBody').css('height', gridHeight + 'px');
    table.columns.adjust().draw();

    $('.dataTables_filter').hide();
}



//function DrawGraph(chartId, chartType, xValue, yValue, bgColor) {

//    var chartData = {
//        labels: xValue,
//        datasets: [
//          {
//              data: yValue,
//              backgroundColor: bgColor,
//          }
//        ]
//    }
//    var chartOptions;

//    if (chartType == 'doughnut') {
//        chartOptions = {
//            segmentShowStroke: true,
//            segmentStrokeColor: "#fff",
//            segmentStrokeWidth: 5,
//            percentageInnerCutout: 50,
//            animationSteps: 100,
//            animationEasing: "easeOutBounce",
//            animateRotate: true,
//            animateScale: false,
//            responsive: true,
//            maintainAspectRatio: false,
//            showScale: true,
//            animateScale: true,
//            legend: {
//                display: true,
//                position: 'right'
//            },
//            plugins: {
//                datalabels: {
//                    anchor: 'end',
//                    align: 'start',
//                    offset: 10,
//                    color: 'black',
//                    fontSize: 20
//                }
//            }
//        }
//    }
//    else {
//        chartOptions = {
//            segmentShowStroke: true,
//            segmentStrokeColor: "#fff",
//            segmentStrokeWidth: 5,
//            percentageInnerCutout: 50,
//            animationSteps: 100,
//            animationEasing: "easeOutBounce",
//            animateRotate: true,
//            animateScale: false,
//            responsive: true,
//            maintainAspectRatio: false,
//            showScale: true,
//            animateScale: true,
//            scales: {
//                xAxes: [{
//                    ticks: {
//                        beginAtZero: true,
//                        autoSkip: false
//                    }
//                }]
//            },
//            legend: {
//                display: false,
//            },
//            plugins: {
//                datalabels: {
//                    anchor: 'end',
//                    align: 'end',
//                    offset: 1,
//                    color: 'black'
//                }
//            }
//        }
//    }

//    var ctx = document.getElementById(chartId);
//    var myChart = new Chart(ctx, {
//        type: chartType,
//        data: chartData,
//        options: chartOptions
//    });


//    ctx.onclick = function (evt) {
//        var activePoints = myChart.getElementsAtEvent(evt);
//        var firstPoint = activePoints[0];
//        if (firstPoint != undefined) {
//            var label = myChart.data.labels[firstPoint._index];
//            var value = myChart.data.datasets[firstPoint._datasetIndex].data[firstPoint._index];

//            if (chartType == 'doughnut') {
//                window.open('/Home/DefectLst?defect=' + label, '_blank');
//            }
//            else if (chartType == 'bar') {
//                window.open('/Home/WIPStatus?station=' + label, '_blank');
//            }
//            else if (chartType == 'horizontalBar') {
//                window.open('/Home/EmpTraining?Emp=' + label, '_blank');
//            }
//        }
//    };

//}

// For Showing Placeholder

function SetHeight(ctrlId, height) {
    var val = (window.innerHeight * height) / 100;
    $('#' + ctrlId).height(val);
}
