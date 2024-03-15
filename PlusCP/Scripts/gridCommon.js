
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

function MakeMeGrid1(tableId, data, tableHeight, columnWidth, search, isScrollX, footerFunction, isDetailed, reportTitleId, rowSelection) {
    //gridCommon
    var isFooter = (footerFunction == undefined ? false : true);
    var isDetailed = (isDetailed == undefined ? false : true);
    var reportTitle = '';
    //  var reportTitle = 'DataExport';
    var padding = '';

    if (reportTitleId == undefined)
        reportTitle = $('.box-title').text();
    else
        reportTitle = reportTitleId;

    var gridHeight = 0;
    var topHeaderH = 30;
    var cardHeaderH = 40;
    var cardFooterH = 40;

    if (isFooter == true)
        padding = 120;

    else
        padding = 90;

    isScrollX = (isScrollX == undefined ? false : true);
    var browserHeight = window.innerHeight;
    if (tableHeight == 0)
        gridHeight = browserHeight - (topHeaderH + cardHeaderH + cardFooterH + padding);
    else
        gridHeight = (window.innerHeight * tableHeight) / 100;

    if (search == undefined) { search = true; }

    var gridButtonsClass = '';
    var gridSearchBoxClass = '';
    var gridButtonSize = 0;
    if (isDetailed == true) {
        gridButtonsClass = 'btn-baby btn-box-tool-baby';
        gridSearchBoxClass = 'bg-white  txtSearch-baby';
        gridButtonSize = 15;
    }
    else {
        gridButtonsClass = 'btn btn-box-tool';
        gridSearchBoxClass = 'btn-box-tool bg-white txtSearch'
        gridButtonSize = 20;
    }

    //For SQL Documentation
    //var rptCode = $('#rptCode').val(); // for sql docs.
    //var rptTitle = $('#rptTitle').val(); // for sql docs.
    //if (rptCode == undefined) {
    //    var rptDtlCode = $('#rptcode').val();
    //    var rptDtlTitle = reportTitle.substring(0, reportTitle.indexOf('>'));
    //    $('#buttons').empty();
    //    var qrytool = '<a id="showQuery"  class="btn btn-box-tool" href="\\SqlDocs\\Index?rptCode=' + rptDtlCode + '&rptTitle=' + rptDtlTitle + '" title="How they do that" target="_blank">';
    //    qrytool += '<img src="\\Content\\images\\sql.png" alt="NA" width="' + gridButtonSize + '" height="' + gridButtonSize + '" />';
    //    qrytool += '</a>';
    //    $('#buttons').append(qrytool);
    //}
    //else {
    //    $('#buttons').empty();
    //    var qrytool = '<a id="showQuery"  class="btn btn-box-tool" href="\\SqlDocs\\Index?rptCode=' + rptCode + '&rptTitle=' + rptTitle + '" title="How they do that" target="_blank">';
    //    qrytool += '<img src="\\Content\\images\\sql.png" alt="NA" width="' + gridButtonSize + '" height="' + gridButtonSize + '" />';
    //    qrytool += '</a>';
    //    $('#buttons').append(qrytool);
    //}

    if (search == true) {
        var boxtool = '<input class="' + gridSearchBoxClass + '" id="txtSearch" type="text" placeholder="Search">';
        $('#buttons').append(boxtool);
    }

    var table = $('#' + tableId).DataTable({
        dom: 'Bfrtip',
        data: data,
        deferRender: true,
        scrollY: gridHeight,
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
                text: '<img src="\\Content\\images\\copy.png" alt="NA" width="' + gridButtonSize + '" height="' + gridButtonSize + '" />',
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
                text: '<img src="\\Content\\images\\excel.png" alt="NA" width="' + gridButtonSize + '" height="' + gridButtonSize + '" />',
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
                text: '<img src="\\Content\\images\\print.png" alt="NA" width="' + gridButtonSize + '" height="' + gridButtonSize + '" />',
                titleAttr: 'Print',
                title: reportTitle,
                filename: 'DataExport',
                exportOptions: {
                    // columns: ':visible'
                    columns: 'th:not(.notexport)'
                }
            },
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

    //var dsgndby = $('.modal-footer > #designedby ').html();
    //if (dsgndby == "") { $('#designedBy').html($('.desgndBy').html()).css({ "padding": "8px" }); }


    $('.dataTables_filter').hide();
    $('#txtSearch').on('keyup change', function () {
        table.search(this.value).draw();
    });
}

function MakeMeBabyGrid1(tableId, data, tableHeight, columnWidth, buttonid, isScrollX, reportTitle, footerFunction) {

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
    // var reportTitle = ''
    //AddSqlDoc(buttonid, true);

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
            emptyTable: "<p style=\"text-align:left;padding:0px;\">No record(s) found.</p>"
        },
        buttons: [
            {
                extend: 'copyHtml5',
                footer: true,
                text: '<img src="\\Content\\images\\copy.png" alt="NA" width="15" height="15" />',
                titleAttr: 'Copy',
                title: reportTitle,
                filename: 'DataExport',
                exportOptions: {
                    //columns: ':visible' Comment By tahir
                    columns: 'th:not(.notexport)' // Excluding columns while exporting
                }
            },
            {
                extend: 'excelHtml5',
                text: '<img src="\\Content\\images\\excel.png" alt="NA" width="15" height="15" />',
                titleAttr: 'Export to excel',
                title: reportTitle,
                filename: 'DataExport',
                exportOptions: {
                    //columns: ':visible'
                    columns: 'th:not(.notexport)' // Excluding columns while exporting
                }
            },
            {
                extend: 'print',
                footer: true,
                text: '<img src="\\Content\\images\\print.png" alt="NA" width="15" height="15" />',
                titleAttr: 'Print',
                title: reportTitle,
                filename: 'DataExport',
                exportOptions: {
                    //columns: ':visible' 
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

function MakeMeFrameGrid1(tableId, data, tableHeight, columnWidth, buttonId, search, isScrollX, footerFunction, isFramed, reportTitleId, isSqlDoc) {
    var rptCode = $('#rptCode').val(); // for sql docs.
    var rptTitle = $('#rptTitle').val(); // for sql docs.

    var isFooter = (footerFunction == undefined || footerFunction == false ? false : true);
    isFramed = (isFramed == undefined || isFramed == false ? false : true);
    var reportTitle = '';
    var padding = '';
    if (reportTitleId == undefined)
        reportTitle = $('.box-title').text();
    else
        reportTitle = reportTitleId;


    var gridHeight = 0;
    var topHeaderH = 30;
    var cardHeaderH = 40;
    var cardFooterH = 40;

    if (isFooter == true)
        padding = 120;
    else
        padding = 90;

    isScrollX = (isScrollX == undefined ? false : true);
    var browserHeight = window.innerHeight;
    //if (tableHeight == 0)
    //    gridHeight = browserHeight - (topHeaderH + cardHeaderH + cardFooterH + padding);
    //else
    //    gridHeight = (window.innerHeight * tableHeight) / 100;

    gridHeight = tableHeight;

    if (search == undefined) { search = true; }

    var gridButtonsClass = '';
    var gridSearchBoxClass = '';
    var gridButtonSize = 0;

    gridButtonsClass = 'btn-baby btn-box-tool-baby';
    gridSearchBoxClass = 'bg-white txtSearch-frame';
    gridButtonSize = 15;


  
    if (search == true) {
        if (isFramed) {
            $('#trHeadSearch').empty();
            var boxtool = '<input class="txtSearch-frame" id="txtFrameSearch" type="text" placeholder="Search">';
            $('#trHeadSearch').append(boxtool);
        }
        else {
            $('#' + buttonId).empty();
            if (isSqlDoc == true)
                AddSqlDoc1(buttonId);

            var boxtool = '<input class="txtSearch-frame" id="txtFrameSearch' + tableId + '" type="text" placeholder="Search">';
            $('#' + buttonId).append(boxtool);
        }
    }
    else {
        if (isFramed) {
            $('#trHeadSearch').empty();
            var boxtool = '<input class="txtSearch-frame" id="txtFrameSearch" type="text" placeholder="Search">';
            $('#trHeadSearch').append(boxtool);
        }
        else {
            $('#' + buttonId).empty();
            if (isSqlDoc == true)
                AddSqlDoc1(buttonId);

            var boxtool = '<input class="txtSearch-frame" id="txtFrameSearch' + tableId + '" type="text" placeholder="Search">';
            $('#' + buttonId).append(boxtool);
        }

    }

    var table = $('#' + tableId).DataTable({
        dom: 'Bfrtip',
        data: data,
        deferRender: true,
        scrollY: gridHeight,
        paging: true,
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
        //"dom": "<'row'<'col-sm-12'tr>>" + // Comment By Tahir
        //    "<'row'<'col-sm-5'i><'col-sm-7'p>>",
        "dom": "<'row'<'col-sm-12'tr>>" +
            "<'row'<'col-sm-5'i><'#designedBy.col-sm-3'><'col-sm-4'p>>", // Added By Tahir
        buttons: [
            {
                extend: 'copyHtml5',
                footer: true,
                text: '<img src="\\Content\\images\\copy.png" alt="NA" width="' + gridButtonSize + '" height="' + gridButtonSize + '" />',
                titleAttr: 'Copy',
                title: reportTitle,
                filename: 'DataExport',
                exportOptions: {
                    //columns: ':visible'
                    columns: 'th:not(.notexport)'
                }
            },
            {
                extend: 'excelHtml5',
                text: '<img src="\\Content\\images\\excel.png" alt="NA" width="' + gridButtonSize + '" height="' + gridButtonSize + '" />',
                titleAttr: 'Export to excel',
                title: reportTitle,
                filename: 'DataExport',
                exportOptions: {
                    //columns: ':visible'
                    columns: 'th:not(.notexport)'
                }
            },
            {
                extend: 'print',
                footer: true,
                text: '<img src="\\Content\\images\\print.png" alt="NA" width="' + gridButtonSize + '" height="' + gridButtonSize + '" />',
                titleAttr: 'Print',
                title: reportTitle,
                filename: 'DataExport',
                exportOptions: {
                    //columns: ':visible'
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

   
    ////Added By Tahir
    //var dsgndby = $('.modal-footer > #designedby ').html();
    //if (dsgndby != "") { $('#designedBy').html($('.modal-footer').html()).css({ "padding": "8px" }); }
    //else { $('#designedBy').html($('.desgndBy').html()).css({ "padding": "8px" }); }


    $('.dataTables_filter').hide();
    $('#txtFrameSearch' + tableId).on('keyup change', function () {
        table.search(this.value).draw();
    });
}

function MakeMeDMLGrid(tableId, data, tableHeight, columnWidth, search, isScrollX, reportTitleId) {
    var rptCode = $('#rptCode').val(); // for sql docs.
    var rptTitle = $('#rptTitle').val(); // for sql docs.

    var reportTitle = '';
    var padding = '';
    if (reportTitleId == undefined)
        reportTitle = $('.box-title').text();
    else
        reportTitle = reportTitleId;

    var gridHeight = 0;
    var topHeaderH = 30;
    var cardHeaderH = 40;
    var cardFooterH = 40;
    padding = 90;

    isScrollX = (isScrollX == undefined ? false : true);
    var browserHeight = window.innerHeight;
    if (tableHeight == 0)
        gridHeight = browserHeight - (topHeaderH + cardHeaderH + cardFooterH + padding);
    else
        gridHeight = (window.innerHeight * tableHeight) / 100;

    if (search == undefined) { search = true; }

    var gridButtonsClass = '';
    var gridSearchBoxClass = '';
    var gridButtonSize = 0;

    gridButtonsClass = 'btn btn-box-tool';
    gridSearchBoxClass = 'btn-box-tool bg-white txtSearch'
    gridButtonSize = 20;

    //AddSqlDoc("buttons");
    if (search == true) {
        $('#buttons').empty();
        var boxtool = '<input class="' + gridSearchBoxClass + '" id="txtSearch" type="text" placeholder="Search">';
        $('#buttons').append(boxtool);
    }

    var table = $('#' + tableId).DataTable({
        dom: 'Bfrtip',
        data: data,
        scrollY: gridHeight,
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
                extend: 'csv',
                text: '<img src="\\Content\\images\\csv.png" alt="NA" width="' + gridButtonSize + '" height="' + gridButtonSize + '" />',
                titleAttr: 'Export to CSV',
                title: reportTitle,
                filename: 'DataExport',
                exportOptions: {
                    columns: 'th:not(.notexport)'
                }
            },
            {
                extend: 'print',
                footer: true,
                text: '<img src="\\Content\\images\\print.png" alt="NA" width="' + gridButtonSize + '" height="' + gridButtonSize + '" />',
                titleAttr: 'Print',
                title: reportTitle,
                filename: 'DataExport',
                exportOptions: {
                    columns: 'th:not(.notexport)'
                }
            },
        ],
    });

    table.buttons().container()
        .appendTo('#buttons');

    table.button(0).nodes().removeClass('btn btn-default buttons-copy buttons-html5');
    table.button(0).nodes().addClass(gridButtonsClass);

    table.button(1).nodes().removeClass('btn btn-default buttons-excel buttons-html5');
    table.button(1).nodes().addClass(gridButtonsClass);

    //var dsgndby = $('.modal-footer > #designedby ').html();
    //if (dsgndby == "") { $('#designedBy').html($('.desgndBy').html()).css({ "padding": "8px" }); }

    $('.dataTables_filter').hide();
    $('#txtSearch').on('keyup change', function () {
        table.search(this.value).draw();
    });

    return table;
}

function AddSqlDoc1(buttonId, gridButtonSize, isBaby) {
    
    //For SQL Documentation
    //var rptCode = $('#rptCode').val(); // for sql docs.
    //var rptTitle = $('#rptTitle').val(); // for sql docs.

    //var className = 'btn btn-box-tool';
    //if (isBaby)
    //    className = 'btn-baby btn-box-tool-baby';

    //$('#' + buttonId).empty();
    //var qrytool = '<a id="showQuery"  class="' + className + '" href="\\SqlDocs\\Index ? rptCode = ' + rptCode + ' & rptTitle=' + rptTitle + '" target="_blank">';
    //qrytool += '<img src="\\Content\\images\\sql.png" alt="NA" width="15" height="15" />';
    //qrytool += '</a>';
    //$('#' + buttonId).append(qrytool);

    //if (rptCode == undefined) {
    //    var rptDtlCode = $('#rptcode').val();
    //    var rptDtlTitle = reportTitle.substring(0, reportTitle.indexOf('>'));
    //    $('#' + buttonId).empty();
    //    var qrytool = '<a id="showQuery"   class="' + className + '" href="\\SqlDocs\\Index?rptCode=' + rptDtlCode + '&rptTitle=' + rptDtlTitle + '" title="How they do that" target="_blank">';
    //    qrytool += '<img src="\\Content\\images\\sql.png" alt="NA" width="' + gridButtonSize + '" height="' + gridButtonSize + '" />';
    //    qrytool += '</a>';
    //    $('#' + buttonId).append(qrytool);
    //}
    //else {
    //    $('#' + buttonId).empty();
    //    var qrytool = '<a id="showQuery"   class="' + className + '" href="\\SqlDocs\\Index?rptCode=' + rptCode + '&rptTitle=' + rptTitle + '" title="How they do that" target="_blank">';
    //    qrytool += '<img src="\\Content\\images\\sql.png" alt="NA" width="' + gridButtonSize + '" height="' + gridButtonSize + '" />';
    //    qrytool += '</a>';
    //    $('#' + buttonId).append(qrytool);
    //}
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

function OpenPage(_url, designedBy) {debugger
    //if (designedBy != undefined) {
    //    $('.modal-footer > #designedby').empty();
    //    $('.desgndBy').html("Designed By : " + designedBy);
    //}
    $.ajax({
        url: _url,
        success: function (data) {
            $("#renderBody").html(data);
        }
    });
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
    //if (num == 0) {
    //    return "";
    //}
    //   else {

    var num_parts = num.toString().split(".");
    num_parts[0] = num_parts[0].replace(/\B(?=(\d{3})+(?!\d))/g, ",");
    return num_parts.join(".");
    //  }
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

