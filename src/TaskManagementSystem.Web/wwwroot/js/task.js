(function ($) {
    "use strict";

    var searchTimer = null;
    var SEARCH_DELAY_MS = 400;

    function getAntiForgeryToken() {
        return $('input[name="__RequestVerificationToken"]').first().val();
    }

    function showSuccessToast(message) {
        Swal.fire({
            toast: true,
            position: "top-end",
            icon: "success",
            title: message,
            showConfirmButton: false,
            timer: 2500,
            timerProgressBar: true
        });
    }

    function showErrorToast(message) {
        Swal.fire({
            toast: true,
            position: "top-end",
            icon: "error",
            title: message,
            showConfirmButton: false,
            timer: 3000,
            timerProgressBar: true
        });
    }

    function initSearch() {
        var $input = $("#searchInput");
        if (!$input.length) return;

        $input.on("keyup input", function () {
            clearTimeout(searchTimer);
            var term = $(this).val();

            searchTimer = setTimeout(function () {
                performSearch(term);
            }, SEARCH_DELAY_MS);
        });
    }

    function performSearch(term) {
        $.ajax({
            url: "/Task/Search",
            type: "GET",
            data: { term: term },
            headers: { "X-Requested-With": "XMLHttpRequest" },
            success: function (html) {
                $("#taskTableBody").html(html);
            },
            error: function () {
                showErrorToast("Search failed. Please try again.");
            }
        });
    }

    function initToggleStatus() {
        $(document).on("click", ".toggle-status-btn", function () {
            var $btn = $(this);
            var taskId = $btn.data("task-id");

            $.ajax({
                url: "/Task/ToggleStatus/" + taskId,
                type: "POST",
                headers: {
                    "X-Requested-With": "XMLHttpRequest",
                    "RequestVerificationToken": getAntiForgeryToken()
                },
                contentType: "application/x-www-form-urlencoded",
                data: { __RequestVerificationToken: getAntiForgeryToken() },
                success: function (response) {
                    if (response.success) {
                        handleToggleSuccess($btn, response.isCompleted);
                    }
                },
                error: function () {
                    showErrorToast("Could not update task status.");
                }
            });
        });
    }

    function handleToggleSuccess($btn, isCompleted) {
        var $row = $btn.closest("tr");
        var $titleSpan = $row.find(".task-title");
        var titleText = $titleSpan.text().trim();

        if (isCompleted) {
            $btn.removeClass("btn-outline-success").addClass("btn-secondary");
            $btn.find("i").removeClass("bi-check-lg").addClass("bi-arrow-counterclockwise");
            $btn.attr("title", "Mark as Pending");
            $row.addClass("text-muted");
            $titleSpan.html("<del>" + titleText + "</del>");
            showSuccessToast("Task marked as complete!");
        } else {
            $btn.removeClass("btn-secondary").addClass("btn-outline-success");
            $btn.find("i").removeClass("bi-arrow-counterclockwise").addClass("bi-check-lg");
            $btn.attr("title", "Mark as Complete");
            $row.removeClass("text-muted");
            $titleSpan.html(titleText);
            showSuccessToast("Task marked as pending.");
        }
    }

    function initDeleteConfirm() {
        $(document).on("click", ".delete-btn", function () {
            var taskId = $(this).data("task-id");
            var taskTitle = $(this).data("task-title");

            Swal.fire({
                title: "Delete Task?",
                text: "\"" + taskTitle + "\" will be permanently removed.",
                icon: "warning",
                showCancelButton: true,
                confirmButtonColor: "#dc3545",
                cancelButtonColor: "#6c757d",
                confirmButtonText: "Yes, delete it",
                cancelButtonText: "Cancel"
            }).then(function (result) {
                if (result.isConfirmed) {
                    $("#delete-form-" + taskId).submit();
                }
            });
        });
    }

    function initFilterAutoSubmit() {
        $("#filterSelect, #sortSelect").on("change", function () {
            $("#filterForm").submit();
        });
    }

    $(document).ready(function () {
        initSearch();
        initToggleStatus();
        initDeleteConfirm();
        initFilterAutoSubmit();
    });

}(jQuery));
