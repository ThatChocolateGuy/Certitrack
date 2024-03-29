﻿//form dropdown button - elements to control (certificate create)
var cusToggleSelect = document.getElementById("customer-toggle-select");
var cusToggleInput = document.getElementById("customer-toggle-input");
var cusEmailInput = document.getElementById("customer-email-input");
var newCustomer = document.getElementById("new-customer");
var existingCustomer = document.getElementById("existing-customer");
//placeholder nodes
var node1 = document.createElement("div"); node1.id = "node1";
var node2 = document.createElement("div"); node2.id = "node2";
//dropdown button selection method
var dropdownButton = function (text) {
    //replace dropdown button text
    document.getElementById("customer-toggle-text").innerText = text;

    //retain position in DOM with placeholder nodes
    if (cusToggleInput) {
        $("#customer-toggle-input").prop("required", false);
        $("#customer-toggle-input").attr("data-val", false);
        cusToggleInput.replaceWith(node1);
    }
    if (cusToggleSelect) {
        $("#customer-toggle-select").prop("required", false);
        $("#customer-toggle-select").attr("data-val", false);
        cusToggleSelect.replaceWith(node2);
    }

    if (text == "Existing") {
        //replace placeholder node with select element
        if (node2) {
            node2.replaceWith(cusToggleSelect);
            $("#customer-toggle-select").prop("required", true);
            $("#customer-toggle-select").attr("data-val", true);
        }

        //display select element if not shown
        if (!cusToggleSelect.style.display == "initial") {
            cusToggleSelect.style.display = "initial";
            cusEmailToggleSelect.style.display = "initial";
            console.log("display select Elements");
        }

        //add select2 classes to select elements
        if (!cusToggleSelect.classList.contains("select2")) {
            cusToggleSelect.classList.add("select2", "select2-name");
        }

        //set selected dropdown list item to active element
        if (!existingCustomer.classList.contains("active")) {
            newCustomer.classList.remove("active");
            existingCustomer.classList.add("active");
        }

        //enable select2 on newly displayed DOM element
        $(".select2-name").select2({
            placeholder: ' Full Name',
            width: 'resolve'
        });

        //prevents select2 span input from scaling abnormally
        $(".select2-container").css("display", "inline");
        $(".select2-selection__arrow").remove();
    }
    else if (text == "New") {
        $(".select2.select2-container").remove();

        //replace placeholder node with input element
        if (node1) {
            node1.replaceWith(cusToggleInput);
            $("#customer-toggle-input").prop("required", true);
            $("#customer-toggle-input").attr("data-val", true);
        }

        //set selected dropdown list item to active element
        if (!newCustomer.classList.contains("active")) {
            existingCustomer.classList.remove("active");
            newCustomer.classList.add("active");
        }
    }
};

//Bootstrap confirmation modal message
var modalConfirm = function (modalLabel) {
    $("#myModalLabel").html(modalLabel);
};

//ajax call
function ajaxCall(id, url) {
    var pData = $.extend({ id: id },
        { "CSRF-TOKEN-CERTITRACK-FORM": $('input[name="CSRF-TOKEN-CERTITRACK-FORM"]').val() });
    $.ajax({
        method: "POST",
        url: url,
        headers: {
            "CSRF-TOKEN-CERTITRACK-FORM": $('input[name="CSRF-TOKEN-CERTITRACK-FORM"]').val()
        },
        data: pData,
        error: function (result) {
            console.log("Result: " + result);
            alert("Something went wrong, Try agian!");
        },
        success: function (result) {
            if (result) {
                sessionStorage.setItem("_refresh.location", true);
                sessionStorage.setItem("_alert.result", result);
                window.scroll({
                    top: 0,
                    left: 0,
                    behavior: 'smooth'
                });
                window.location.reload();
            }
            else {
                console.log("Result: " + result);
                alert("Something went wrong, Try agian!\n");
            }
        }
    });
}

//delete fn
function deleteUser(name) {
    $("#my-modal").modal('show');
    modalConfirm("Are you sure you want to delete " + name + "?<hr>This action cannot be reversed.");
}
//redeem certificate fn
function redeem(certNo) {
    $("#my-modal").modal('show');
    modalConfirm("Redeem Certificate #" + certNo + " ?");
}
//btnGroup (redeem, edit, delete) fn
function btnGroup(_btn) {
    var certId, staffId, url;
    $(_btn.id).data('clicked', false);
    switch (_btn.id) {
        case 'modal-btn-yes':
            console.log(_btn.id + " clicked");
            var eId = $('#post-data').data('id');
            if (eId != null && (eId.includes("redeem") || eId.includes("delete"))) {
                getEl(eId);
                if (certId)
                    ajaxCall(certId, url);
                else if (staffId)
                    ajaxCall(staffId, url);
            }
            break;
        default:
            console.log(_btn.id + " clicked");
            $("#post-data").data("id", _btn.id);
            break;
    }
    //get element & assign cert data to it
    function getEl(id) {
        var el = document.getElementById(id);
        certId = el.dataset.certId;
        staffId = el.dataset.staffId;
        url = el.dataset.url;
    }
}

//on DOM ready
$(function () {
    //format DataTables
    $("#main-table-staff").DataTable({
        "columnDefs": [{
            "targets": 5,
            "orderable": false
        }]
    });
    $("#main-table-cert").DataTable();
    $("#main-table-customer").DataTable({
        "columnDefs": [{
            "targets": 4,
            "orderable": false
        }]
    });
    $("#main-table-customer-order").DataTable({
        order: [[0, 'desc']]
    });
    $("#main-table-customer-cert").DataTable({
        order: [[1, 'desc']]
    });
    $("#main-table-order").DataTable({
        "columnDefs": [{
            "targets": 3,
            "orderable": false
        }]
    });
    $("#main-table-channel").DataTable({
        "columnDefs": [{
            "targets": 1,
            "orderable": false
        }]
    });
    $("#main-table-promo").DataTable({
        "columnDefs": [{
            "targets": 1,
            "orderable": false
        }]
    });
    $("#main-table-role").DataTable({
        "columnDefs": [{
            "targets": 2,
            "orderable": false
        }]
    });
    //InputMask
    $(":input").inputmask();
    //Select2
    $(".select2").select2({
        width: "100%"
    });
    //Bootstrap tooltip
    $("[data-toggle='tooltip']").tooltip();
    //reload Bootstrap tooltip on table interaction
    $(".dataTables_wrapper").click(() => {
        $("[data-toggle='tooltip']").tooltip();
        var btn = this.activeElement;
        if (btn.id.includes("redeem") || btn.id.includes("delete"))
            btnGroup(btn);
    });
    $("[type='search']").keyup(() => {
        $("[data-toggle='tooltip']").tooltip();
    })
    //datepicker
    $.fn.datepicker.defaults = { //defaults for reference
        autoclose: true,
        beforeShowDay: $.noop,
        calendarWeeks: false,
        clearBtn: false,
        daysOfWeekDisabled: [],
        endDate: Infinity,
        forceParse: true,
        format: 'yyyy-mm-dd',
        keyboardNavigation: true,
        language: 'en',
        minViewMode: 0,
        orientation: "auto",
        rtl: false,
        startDate: -Infinity,
        startView: 2,
        todayBtn: false,
        todayHighlight: false,
        weekStart: 0
    };
    $('#set-expiry').datepicker({
        format: 'yyyy-mm-dd',
        startDate: "+1m",
        maxViewMode: 1,
        orientation: "bottom auto",
        daysOfWeekHighlighted: "1,2,3,4,5",
        todayBtn: true,
        todayHighlight: true
    });
    $('.datepicker').datepicker({
        format: 'yyyy-mm-dd',
        maxViewMode: 1,
        orientation: "bottom auto",
        daysOfWeekHighlighted: "1,2,3,4,5",
        todayBtn: true,
        todayHighlight: true
    });
    //Bootsrap alert modal
    if (sessionStorage.getItem("_refresh.location") == "true") {
        var alertType, alertLabel, icon;
        var alertResult = sessionStorage.getItem("_alert.result");
        if (alertResult.includes("success") || alertResult.includes("redeemed")) {
            alertType = "alert-success";
            alertLabel = "Success";
            icon = "<i class=\"icon fa fa-check\"></i>";
        } else {
            alertType = "alert-danger";
            alertLabel = "Something Went Wrong";
            icon = "<i class=\"icon fa fa-ban\"></i>";
        }
            
        document.getElementById("status-messages").innerHTML =
            "<div class=\"alert " + alertType + " alert-dismissible\" role=\"alert\" style=\"margin-top: -2px; box-shadow: rgba(0,0,0,.5) 0 1px 3px\">" +
            "    <button type=\"button\" class=\"close\" data-dismiss=\"alert\" aria-label=\"Close\">" +
            "        <span aria-hidden=\"true\">&times;</span>" +
            "    </button>" +
            "    <strong>" +
                    icon +
                    alertLabel +
            "    </strong>" +
            "    <span class=\"pull-right\">" + alertResult + "</span>" +
            "</div>";
        sessionStorage.setItem("_refresh.location", false);
    }
    //click listeners
    $("#modal-btn-yes").on("click", function () {
        $("#my-modal").modal('hide');
        btnGroup(this);
    });
    $("#modal-btn-no").on("click", function () {
        $("#my-modal").modal('hide');
    });
    $("#new-customer").on("click", function () {
        dropdownButton("New");
    });
    $("#existing-customer").on("click", function () {
        dropdownButton("Existing");
    });
    //select list on-change listener (certificate create)
    $("select#customer-toggle-select").change(function () {
        if ($(this).children("option:selected").val()) {
            //gets value of selected dropdown option
            var selectedCustomer = $(this).children("option:selected").val();
            //ajax call to retrieve customer email
            ajaxCallSelect(selectedCustomer, "/Certificates/GetCustomerEmail");
        }
        function ajaxCallSelect(selectedCustomer, url) {
            var pData = $.extend({ customerName: selectedCustomer },
                { "CSRF-TOKEN-CERTITRACK-FORM": $('input[name="CSRF-TOKEN-CERTITRACK-FORM"]').val() });
            $.ajax({
                method: "POST",
                url: url,
                headers: {
                    "CSRF-TOKEN-CERTITRACK-FORM": $('input[name="CSRF-TOKEN-CERTITRACK-FORM"]').val()
                },
                data: pData,
                error: function () {
                    alert("Something went wrong, Try agian!");
                },
                success: function (result) {
                    if (result) {
                        $("#customer-email-input").val(result[0]);
                        $("#customer-phone-input").val(result[1]);
                    }
                    else {
                        alert("Something went wrong, Try agian!");
                    }
                }
            });
        }
    });
});

