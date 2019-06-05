//form dropdown button - elements to control (certificate create)
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

//on DOM ready
$(function () {
    //format DataTables
    $('#main-table-staff').DataTable({
        "columnDefs": [{
            "targets": 6,
            "orderable": false
        }]
    });
    $("#main-table-cert").DataTable();
    $('#main-table-customer').DataTable({
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
    //InputMask
    $(":input").inputmask();
    //Select2
    $('.select2').select2({
        width: '100%'
    });
    //Bootstrap tooltip
    $('[data-toggle="tooltip"]').tooltip();
    //reload Bootstrap tooltip on table interaction
    $(".dataTables_wrapper").click(() => {
        $('[data-toggle="tooltip"]').tooltip();
    });
    $('[type="search"]').keyup(() => {
        $('[data-toggle="tooltip"]').tooltip();
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
        document.getElementById("status-messages").innerHTML =
            "<div class=\"alert alert-success alert-dismissible\" role=\"alert\" style=\"margin-top: -2px; box-shadow: rgba(0,0,0,.5) 0 1px 3px\">" +
            "    <button type=\"button\" class=\"close\" data-dismiss=\"alert\" aria-label=\"Close\">" +
            "        <span aria-hidden=\"true\">&times;</span>" +
            "    </button>" +
            "    <strong>" +
            "       <i class=\"icon fa fa-check\"></i>" +
            "       Success" +
            "    </strong>" +
            "    <span class=\"pull-right\">" + sessionStorage.getItem("_alert.result") + "</span>" +
            "</div>";
        sessionStorage.setItem("_refresh.location", false);
    }
    //click listeners
    $("#modal-btn-yes").on("click", function () {
        $("#my-modal").modal('hide');
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
    $(':button, .btn').click(function () {
        var certId, staffId, url;
        $(this.id).data('clicked', false);
        switch (this.id) {
            case 'modal-btn-yes':
                console.log(this.id + " clicked");
                var eId = $('#post-data').data('id');
                getEl(eId);
                if (certId)
                    ajaxCall(certId, url);
                else if (staffId)
                    ajaxCall(staffId, url);
                break;
            case 'modal-btn-no':
                console.log(this.id + " clicked");
                break;
            default:
                console.log(this.id + " clicked");
                $("#post-data").data("id", this.id);
                break;
        }
        //get element & assign cert data to it
        function getEl(id) {
            var el = document.getElementById(id);
            certId = el.dataset.certId;
            staffId = el.dataset.staffId;
            url = el.dataset.url;
        }
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