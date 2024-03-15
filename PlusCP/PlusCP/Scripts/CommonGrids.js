//function MakeDataGrid(tableId, data, tableHeight, columns, search, isScrollX, footerFunction, detailed, reportTitleId, rowSelection, lstHeight) {
//    var isFooter = footerFunction === undefined || footerFunction === false ? false : true;
//    var isDetailed = detailed === undefined ? false : true;
//    var reportTitle = '';

//    if (reportTitleId === undefined)
//        reportTitle = $('.box-title').text();
//    else
//        reportTitle = reportTitleId;

//    var gridHeight = 0;

//    isScrollX = isScrollX === undefined ? false : true;
//    if (search === undefined) { search = true; }

//    var gridButtonsClass = '';
//    var gridSearchBoxClass = '';
//    if (isDetailed === true) {
//        gridButtonsClass = 'btn-header btn-box-tool';
//        gridSearchBoxClass = 'bg-white  txtSearch';
//        gridButtonSize = 20;
//    }
//    else {
//        gridButtonsClass = 'btn-header btn-box-tool';
//        gridSearchBoxClass = 'btn-box-tool bg-white txtSearch';
//        gridButtonSize = 20;
//    }

//    //For SQL Documentation

//    var rptDtlCode, rptDtlTitle, qrytool;
//    //if (isDetailed === true) {
//    //    rptDtlCode = $('#rptcode').val();
//    //    rptDtlTitle = reportTitle.substring(0, reportTitle.indexOf('>'));

//    //    if (rptDtlTitle === "")
//    //        rptDtlTitle = reportTitle;

//    //    qrytool = '<a id="showQuery" class="btn-header btn-box-tool" href="\\SqlDocs\\Index?rptCode=' + rptDtlCode + '&rptTitle=' + rptDtlTitle + '&isDetail=Y' + '" title="How they do that" target="_blank">';
//    //    qrytool += '<span class="fa-stack sqlDoc" ><i class="fa fa-circle fa-stack-2x" style="color: rgb(221, 75, 57);"></i><i class="fa fa-database fa-stack-1x" style="color: white;"></i></span>';
//    //    qrytool += '</a>';

//    //    $('#buttons').empty();
//    //    $('#buttons').append(qrytool);
//    //}
//    //else {
//    //    var rptCode = $('#rptCode').val(); // for sql docs.
//    //    var rptTitle = $('#rptTitle').val(); // for sql docs.
//    //    if (rptCode === undefined) {
//    //        rptDtlCode = $('#rptcode').val();
//    //        rptDtlTitle = reportTitle.substring(0, reportTitle.indexOf('>'));

//    //        qrytool = '<a id="showQuery"  class="btn-header btn-box-tool" href="\\SqlDocs\\Index?rptCode=' + rptDtlCode + '&rptTitle=' + rptDtlTitle  + '&isDetail=N' + '" title="How they do that" target="_blank">';
//    //        qrytool += '<span class="fa-stack sqlDoc" ><i class="fa fa-circle fa-stack-2x" style="color: rgb(221, 75, 57);"></i><i class="fa fa-database fa-stack-1x" style="color: white;"></i></span>';
//    //        qrytool += '</a>';

//    //        $('#buttons').empty();
//    //        $('#buttons').append(qrytool);
//    //    }
//    //    else {
//    //        qrytool = '<a id="showQuery"  class="btn-header btn-box-tool" href="\\SqlDocs\\Index?rptCode=' + rptCode + '&rptTitle=' + rptTitle + '&isDetail=N' + '" title="How they do that" target="_blank">';
//    //        qrytool += '<span class="fa-stack sqlDoc"><i class="fa fa-circle fa-stack-2x" style="color: rgb(221, 75, 57);"></i><i class="fa fa-database fa-stack-1x" style="color: white;"></i></span>';
//    //        qrytool += '</a>';

//    //        $('#buttons').empty();
//    //        $('#buttons').append(qrytool);
//    //    }
//    //}

//    if (search === true) {
//        var boxtool = '<input class="' + gridSearchBoxClass + '" id="txtSearch" type="text" placeholder="Search.." style="padding-left: 10px !important;" >';
//        $('#buttons').append(boxtool);
//    }

//    var table = $('#' + tableId).DataTable({
//        order: [],
//        serverSide: true,
//        createdRow: rowSelection,         // add Parameter for row selection in PO Detail // Added By Tahir
//        data: data,
//        columns: columns,
//        paging: true,
//        ordering: true,
//        searching: search,
//        deferRender: true,
//        destroy: true,
//        pagingType: 'full',
//        pageLength: 100,
//        scrollCollapse: false,
//        scrollX: isScrollX,
//        //scrollY: gridHeight,

//        footerCallback: footerFunction,
//        language: {
//            emptyTable: "<p style=\"text-align:left;\">No record(s) found.</p>",
//            zeroRecords: "<p style=\"text-align:left;\">No matching record(s) found</p>",
//            info: "Showing _START_ to _END_ of _TOTAL_ entries",
//            oPaginate: {
//                sNext: '<i class="fa fa-forward"></i>',
//                sPrevious: '<i class="fa fa-backward"></i>',
//                sFirst: '<i class="fa fa-step-backward"></i>',
//                sLast: '<i class="fa fa-step-forward"></i>'
//            }
//        },
//        "dom": "<'row'<'col-sm-12'tr>>" +
//            "<'row'<'col-sm-3'i><'#designedBy.col-sm-7'><'col-sm-2'p>>",
//        buttons: [
//            {
//                extend: 'copyHtml5',
//                footer: true,
//                text: '<span><span class="fa-stack"><i class="fa fa-circle fa-stack-2x" style="color: rgb(244, 164, 37);"></i><i class="fa fa-copy fa-stack-1x" style="color: white;"></i></span></span>',
//                titleAttr: 'Copy',
//                title: reportTitle,
//                filename: 'DataExport',
//                exportOptions: {
//                    //columns: ':visible'
//                    columns: 'th:not(.notexport)' // Excluding columns while exporting
//                }
//            },
//            {
//                extend: 'excelHtml5',
//                footer: true,
//                text: '<span><span class="fa-stack"><i class="fa fa-circle fa-stack-2x" style="color: rgb(0, 179, 98);"></i><i class="fa fa-file-excel fa-stack-1x" style="color: white;"></i></span></span>',
//                titleAttr: 'Export to excel',
//                title: reportTitle,
//                filename: 'DataExport', //Added By Tahir
//                exportOptions: {
//                    //columns: ':visible'
//                    columns: 'th:not(.notexport)'
//                }
//            },
//            {
//                extend: 'print',
//                footer: true,
//                text: '<span><span class="fa-stack"><i class="fa fa-circle fa-stack-2x" style="color: rgb(0, 204, 255);"></i><i class="fa fa-print fa-stack-1x" style="color: white;"></i></span></span>',
//                titleAttr: 'Print',
//                title: '',
//                messageTop: function () { return '<h4>' + reportTitle + '</h4>'; },
//                filename: 'DataExport',
//                exportOptions: {
//                    // columns: ':visible'
//                    columns: 'th:not(.notexport)'
//                }
//            }
//        ],
//        select: true
//    });
//    table.buttons().container()
//        .appendTo('#buttons');

//    table.button(0).nodes().removeClass('btn btn-default buttons-copy buttons-html5');
//    table.button(0).nodes().addClass(gridButtonsClass);

//    table.button(1).nodes().removeClass('btn btn-default buttons-excel buttons-html5');
//    table.button(1).nodes().addClass(gridButtonsClass);

//    table.button(2).nodes().removeClass('btn btn-default buttons-excel buttons-html5');
//    table.button(2).nodes().addClass(gridButtonsClass);

//    var dsgndby = $('.modal-footer > #designedby ').html();

//    if (dsgndby === "") {
//        $('#designedBy').html($('.desgndBy').html());
//    }

//    var layoutHeader = $('nav').innerHeight();
//    var cardHeader = $('.box-header').innerHeight();
//    var tableHeader = $('.dataTables_scrollHead').innerHeight();
//    var tableFooter = 0;
//    var cardFooterH = 30;

//    if (isFooter === true)
//        tableFooter = $('.dataTables_scrollFoot').innerHeight();


//    var parentHeight = $('#' + lstHeight).innerHeight();
//    if (parentHeight == undefined)
//        parentHeight = window.innerHeight;
//    else
//        parentHeight = $('#' + lstHeight).innerHeight();

//    if (isFooter === true)
//        tableFooter = $('#' + tableId + ' > tfoot').innerHeight();

//    var browserHeight = parentHeight;
//    if (tableHeight === 0) {
//        if (layoutHeader === undefined)
//            gridHeight = browserHeight - (cardHeader + tableHeader + tableFooter + cardFooterH);
//        else
//            gridHeight = browserHeight - (layoutHeader + cardHeader + tableHeader + tableFooter + cardFooterH);
//    }
//    else { gridHeight = tableHeight; }

//    var topHeader = $('#topHeader').innerHeight();
//    if (topHeader > 0)
//        gridHeight = gridHeight - (topHeader + 1);

//    $('.dataTables_scrollBody').css('height', gridHeight + 'px');
//    table.columns.adjust().draw();

//    $('.dataTables_filter').hide();
//    $('#txtSearch').on('keyup change', function () {
//        table.search(this.value).draw();
//    });

//    return table;
//}

function MakeFrameGrid(tableId, data, tableHeight, columns, buttonId, search, isScrollX, footerFunction, isFramed, reportTitle, isSqlDoc, showPaging) {
    var isFooter = footerFunction === undefined || footerFunction === false ? false : true;
    isFramed = isFramed === undefined || isFramed === false ? false : true;
    isScrollX = isScrollX === undefined ? false : true;
    if (search === undefined) { search = true; }
    reportTitle = reportTitle === undefined ? $('.box-title').text() : reportTitle;
    showPaging = showPaging === undefined ? true : false;

    var gridHeight = 0;
    var gridButtonsClass = '';
    //var gridButtonSize = 0;
    gridButtonsClass = 'btn-header btn-box-tool';
    gridSearchBoxClass = 'bg-white';
    gridButtonSize = 20;

    var cardHeader = $('.box-header').innerHeight();
    var tableHeader = $('#' + tableId + ' > thead').innerHeight();
    var tableFooter = 0;
    var cardFooterH = 0;

    if (showPaging)
        cardFooterH = 30;

    if (isFooter === true)
        tableFooter = $('#' + tableId + ' > tfoot').innerHeight();

    var browserHeight = tableHeight;
    tableFooter = tableFooter === undefined ? 0 : tableFooter;
    gridHeight = browserHeight - (cardHeader + tableHeader + tableFooter + cardFooterH);




    //For SQL Documentation
    var boxtool;
    if (search === true) {
        if (isFramed) {
            $('#trHeadSearch').empty();
            boxtool = '<input class="txtSearch-frame" id="txtFrameSearch" type="text" placeholder="Search.." style="padding-left: 10px !important;" >';
            $('#trHeadSearch').append(boxtool);
        }
        else {
            $('#' + buttonId).empty();
            if (isSqlDoc === true)
                AddSqlDoc(buttonId);
            boxtool = '<input class="txtSearch-frame" id="txtFrameSearch' + tableId + '" type="text" placeholder="Search.." style="padding-left: 10px !important;" >';
            $('#' + buttonId).append(boxtool);
        }
    }
    else {
        if (isFramed) {
            $('#trHeadSearch').empty();
            boxtool = '<input class="txtSearch-frame" id="txtFrameSearch" type="text" placeholder="Search.." style="padding-left: 10px !important;" >';
            $('#trHeadSearch').append(boxtool);
        }
        else {
            $('#' + buttonId).empty();
            if (isSqlDoc === true)
                AddSqlDoc(buttonId);
        }
    }

    var table = $('#' + tableId).DataTable({
        order: [],
        data: data,
        columns: columns,
        bInfo: showPaging,
        paging: showPaging,
        ordering: true,
        searching: search,
        deferRender: true,
        pagingType: 'full',
        pageLength: 100,
        scrollCollapse: false,
        scrollX: isScrollX,
        scrollY: gridHeight,
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
            "<'row'<'col-sm-5'i><'#designedBy.col-sm-3'><'col-sm-4'p>>", // Added By Tahir
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
                messageTop: function () { return '<h4>' + reportTitle + '</h4>'; },
                filename: 'DataExport',
                exportOptions: {
                    columns: 'th:not(.notexport)'
                }
            }

        ],
        initComplete: function () {
            $('#txtSearch').focus();
        },
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

    $('#txtFrameSearch' + tableId).on('keyup change', function () {
        table.search(this.value).draw();
    });
    $('.dataTables_filter').hide();
    table.columns.adjust().draw();

}

function MakeMeBabyGrid(tableId, data, tableHeight, columnWidth, buttonid, isScrollX, reportTitle, footerFunction, lstHeight) {
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
    
    if (isFooter === true)
        tableFooter = $('#' + tableId + ' > tfoot').innerHeight();

    var browserHeight = parentHeight;
    if (tableHeight === 0) {
        if (layoutHeader === undefined)
            gridHeight = browserHeight - (tableHeader);
        else
            gridHeight = browserHeight - (layoutHeader + cardHeader + tableHeader + tableFooter + cardFooterH);
    }
    else { gridHeight = tableHeight; }

    var topHeader = $('#topHeader').innerHeight();
    if (topHeader > 0)
        gridHeight = gridHeight - (topHeader + 1);

    $('.dataTables_scrollBody').css('height', gridHeight + 'px');
    table.columns.adjust().draw();

    $('.dataTables_filter').hide();
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

function MakeDataGridIP(tableId, data, tableHeight, columns, search, isScrollX, footerFunction, detailed, reportTitleId, rowSelection, lstHeight) {

    var startTime = performance.now();
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


    //if (isDetailed === true) {
    //    rptDtlCode = $('#rptcode').val();
    //    rptDtlTitle = reportTitle.substring(0, reportTitle.indexOf('>'));

    //    if (rptDtlTitle === "")
    //        rptDtlTitle = reportTitle;

    //    qrytool = '<a id="showQuery" class="btn-header btn-box-tool" href="\\SqlDocs\\Index?rptCode=' + rptDtlCode + '&rptTitle=' + rptDtlTitle + '&isDetail=Y' + '" title="How they do that" target="_blank">';
    //    qrytool += '<span class="fa-stack sqlDoc" ><i class="fa fa-circle fa-stack-2x" style="color: rgb(221, 75, 57);"></i><i class="fa fa-database fa-stack-1x" style="color: white;"></i></span>';
    //    qrytool += '</a>';

    //    $('#buttons').empty();
    //    $('#buttons').append(qrytool);
    //}
    //else {
    //    var rptCode = $('#rptCode').val(); // for sql docs.
    //    var rptTitle = $('#rptTitle').val(); // for sql docs.
    //    if (rptCode === undefined) {
    //        rptDtlCode = $('#rptcode').val();
    //        rptDtlTitle = reportTitle.substring(0, reportTitle.indexOf('>'));

    //        qrytool = '<a id="showQuery"  class="btn-header btn-box-tool" href="\\SqlDocs\\Index?rptCode=' + rptDtlCode + '&rptTitle=' + rptDtlTitle + '&isDetail=N' + '" title="How they do that" target="_blank">';
    //        qrytool += '<span class="fa-stack sqlDoc" ><i class="fa fa-circle fa-stack-2x" style="color: rgb(221, 75, 57);"></i><i class="fa fa-database fa-stack-1x" style="color: white;"></i></span>';
    //        qrytool += '</a>';

    //        $('#buttons').empty();
    //        $('#buttons').append(qrytool);
    //    }
    //    else {
    //        qrytool = '<a id="showQuery"  class="btn-header btn-box-tool" href="\\SqlDocs\\Index?rptCode=' + rptCode + '&rptTitle=' + rptTitle + '&isDetail=N' + '" title="How they do that" target="_blank">';
    //        qrytool += '<span class="fa-stack sqlDoc"><i class="fa fa-circle fa-stack-2x" style="color: rgb(221, 75, 57);"></i><i class="fa fa-database fa-stack-1x" style="color: white;"></i></span>';
    //        qrytool += '</a>';

    //        $('#buttons').empty();
    //        $('#buttons').append(qrytool);
    //    }
    //}

    var rptDtlCode, rptDtlTitle, qrytool;
    if (isDetailed === true) {
        rptDtlCode = $('#rptcode').val();
        rptDtlTitle = reportTitle.substring(0, reportTitle.indexOf('>'));

        if (rptDtlTitle === "")
            rptDtlTitle = reportTitle;

        qrytool = '<a id="showQuery" class="btn-header btn-box-tool" href="\\SqlDocs\\Index?rptCode=' + rptDtlCode + '&rptTitle=' + rptDtlTitle + '&isDetail=Y' + '" title="How they do that" target="_blank">';
        qrytool += '<span class="fa-stack sqlDoc" ><i class="fa fa-circle fa-stack-2x" style="color: rgb(221, 75, 57);"></i><i class="fa fa-database fa-stack-1x" style="color: white;"></i></span>';
        qrytool += '</a>';

        //$('#buttons').empty();
        //$('#buttons').append(qrytool);
    }
    else {
        var rptCode = $('#rptCode').val(); // for sql docs.
        var rptTitle = $('#rptTitle').val(); // for sql docs.
        if (rptCode === undefined) {
            rptDtlCode = $('#rptcode').val();
            rptDtlTitle = reportTitle.substring(0, reportTitle.indexOf('>'));

            qrytool = '<a id="showQuery"  class="btn-header btn-box-tool" href="\\SqlDocs\\Index?rptCode=' + rptDtlCode + '&rptTitle=' + rptDtlTitle + '&isDetail=N' + '" title="How they do that" target="_blank">';
            qrytool += '<span class="fa-stack sqlDoc" ><i class="fa fa-circle fa-stack-2x" style="color: rgb(221, 75, 57);"></i><i class="fa fa-database fa-stack-1x" style="color: white;"></i></span>';
            qrytool += '</a>';

            //$('#buttons').empty();
            //$('#buttons').append(qrytool);
        }
        else {
            qrytool = '<a id="showQuery"  class="btn-header btn-box-tool" href="\\SqlDocs\\Index?rptCode=' + rptCode + '&rptTitle=' + rptTitle + '&isDetail=N' + '" title="How they do that" target="_blank">';
            qrytool += '<span class="fa-stack sqlDoc"><i class="fa fa-circle fa-stack-2x" style="color: rgb(221, 75, 57);"></i><i class="fa fa-database fa-stack-1x" style="color: white;"></i></span>';
            qrytool += '</a>';

            //$('#buttons').empty();
            //$('#buttons').append(qrytool);
        }
    }

    if (isAdmin === "True") {
        $('#buttons').empty();
        $('#buttons').append(qrytool);
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
        initComplete: function () {
            $('#txtSearch').focus();
        },
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
            gridHeight = browserHeight - (cardHeader + tableHeader + tableFooter + cardFooterH);
        else
            gridHeight = browserHeight - (layoutHeader + cardHeader + tableHeader + tableFooter + cardFooterH);
    }
    else { gridHeight = tableHeight; }

    var topHeader = $('#topHeader').innerHeight();
    if (topHeader > 0)
        gridHeight = gridHeight - (topHeader + 1);

    $('.dataTables_scrollBody').css('height', gridHeight + 'px');
    table.columns.adjust().draw();

    $('.dataTables_filter').hide();
    $('#txtSearch').on('keyup change', function () {
        table.search(this.value).draw();
    });

    var endTime = performance.now();
    var elapsedTimeInSeconds = (endTime - startTime) / 1000;
    
    $.ajax({
        cache: false,
        type: 'POST',
        url: '/Home/SaveLog',
        data: {
            gridExecutionTime: elapsedTimeInSeconds
        },
        success: function () {
            //console.log('Value sent to controller!');
        },
        error: function () {
            //console.log('Error sending value to controller!');
        }
    }); //Added by Huzaifa
    return table;
}



