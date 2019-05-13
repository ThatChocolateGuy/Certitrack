//click fns
$("#modal-btn-yes").on("click", function () {
    $("#my-modal").modal('hide');
    return true;
});
$("#modal-btn-no").on("click", function () {
    $("#my-modal").modal('hide');
    return false;
});
$("#new-customer").on("click", function () {
    dropdownButton("New");
});
$("#existing-customer").on("click", function () {
    dropdownButton("Existing");
});
$(':button').click(function () {
    $(this.id).data('clicked', false);

    switch (this.id) {
        case 'modal-btn-yes':
            console.log(this.id + " clicked");

            var eId = $('#post-data').data('id');
            var el = document.getElementById(eId);
            var certId, staffId, url, message;

            console.log(eId);
            console.log(certId = el.dataset["certId"]);
            console.log(staffId = el.dataset["staffId"]);
            console.log(message = el.dataset["message"]);
            console.log(url = el.dataset["url"]);

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
            var el = document.getElementById(this.id);
            if (el.dataset["certId"] || el.dataset["staffId"]) {
                if (el.dataset["certId"])
                    console.log(el.dataset["certId"]);
                else if (el.dataset["staffId"])
                    console.log(el.dataset["staffId"]);
                console.log(el.dataset["message"]);
                console.log(el.dataset["url"]);
                console.log('<-------------->');
            }

            $("#post-data").data("id", this.id);
            break;
    }
});
//select list on-change fn (certificate create)
$("select#customer-toggle-select").change(function () {
    if ($(this).children("option:selected").val()) {
        //gets value of selected dropdown option
        var selectedCustomer = $(this).children("option:selected").val();
        //ajax call to retrieve customer email
        ajaxCallSelect(selectedCustomer, "/Certificates/CreateWithCustomer");
        //
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
                        $("#customer-email-input").val(result);
                    }
                    else {
                        alert("Something went wrong, Try agian!");
                    }
                }
            });
        }
    }
});

//form dropdown button - elements to control (certificate create)
var cusToggleSelect = document.getElementById("#customer-toggle-select");
var cusToggleInput = document.getElementById("#customer-toggle-input");
var cusEmailInput = document.getElementById("#customer-email-input");
var newCustomer = document.getElementById("#new-customer");
var existingCustomer = document.getElementById("#existing-customer");
//placeholder nodes
var node1 = document.createElement("div"); node1.id = "node1";
var node3 = document.createElement("div"); node3.id = "node3";
//dropdown button selection method
var dropdownButton = function (text) {
    //replace dropdown button text
    document.getElementById("customer-toggle-text").innerText = text;

    //retain position in DOM with placeholder nodes
    if (cusToggleInput) {
        cusToggleInput.replaceWith(node1);
    }
    if (cusToggleSelect) {
        cusToggleSelect.replaceWith(node3);
    }

    if (text == "Existing") {
        //replace placeholder node with select element
        if (node3) {
            node3.replaceWith(cusToggleSelect);
        }

        //display select element if not shown
        if (!cusToggleSelect.style.display == "initial") {
            cusToggleSelect.style.display = "initial";
            cusEmailToggleSelect.style.display = "initial";
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
        }

        //set selected dropdown list item to active element
        if (!newCustomer.classList.contains("active")) {
            existingCustomer.classList.remove("active");
            newCustomer.classList.add("active");
        }
    }
}

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
        error: function () {
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
                alert("Something went wrong, Try agian!\n");
            }
        }
    });
}

//delete staff
function deleteUser(id, name, url) {
    $("#my-modal").modal('show');
    modalConfirm("Are you sure you want to delete " + name + "?<hr>This action cannot be reversed.");
}
//redeem certificate
function redeem(id, certNo, url) {
    $("#my-modal").modal('show');
    modalConfirm("Redeem Certificate: #" + certNo + " ?");
}
//format staff index table
$(function () {
    $('#main-table-staff').DataTable({
        "columnDefs": [{
            "targets": 6,
            "orderable": false
        }]
    });
    $('#main-table-cert').DataTable({
        "columnDefs": [{
            "targets": 8,
            "orderable": false
        }]
    });
});
//datepicker
$(function () {
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
});

//onDocumentReady
$(document).ready(function () {
    //InputMask
    $(":input").inputmask();
    //Select2
    $('.select2').select2({
        width: '100%'
    });
    //Bootstrap tooltip
    $('[data-toggle="tooltip"]').tooltip();
    //Bootsrap alert modal
    if (sessionStorage.getItem("_refresh.location") == "true") {
        document.getElementById("status-messages").innerHTML =
            "<div class=\"alert alert-success alert-dismissible\" role=\"alert\" style=\"margin-top: -2px; box-shadow: rgba(0,0,0,.5) 0 1px 3px\">" +
            "    <button type=\"button\" class=\"close\" data-dismiss=\"alert\" aria-label=\"Close\">" +
            "        <span aria-hidden=\"true\">&times;</span>" +
            "    </button>" +
            "    <strong>Success</strong>" +
            "    <span class=\"pull-right\">" + sessionStorage.getItem("_alert.result") + "</span>" +
            "</div>";
        sessionStorage.setItem("_refresh.location", false);
    }
});