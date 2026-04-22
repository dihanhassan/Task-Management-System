(function ($) {
    "use strict";

    var searchTimer = null;
    var SEARCH_DELAY_MS = 350;

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
            timer: 2200,
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
            timer: 2800,
            timerProgressBar: true
        });
    }

    function initSearch() {
        var $wrap = $(".tm-search");
        var $input = $("#searchInput");
        var $clear = $("#searchClear");
        if (!$input.length) return;

        $input.on("keyup input", function () {
            clearTimeout(searchTimer);
            var term = $(this).val();
            $wrap.toggleClass("has-value", term.length > 0);

            searchTimer = setTimeout(function () {
                performSearch(term);
            }, SEARCH_DELAY_MS);
        });

        $clear.on("click", function () {
            $input.val("").trigger("input").focus();
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
            $btn.addClass("is-checked").attr("title", "Mark as pending");
            $row.addClass("is-completed");
            $titleSpan.html("<del>" + titleText + "</del>");
            showSuccessToast("Task marked as complete");
        } else {
            $btn.removeClass("is-checked").attr("title", "Mark as complete");
            $row.removeClass("is-completed");
            $titleSpan.html(titleText);
            showSuccessToast("Task marked as pending");
        }
    }

    function initDeleteConfirm() {
        $(document).on("click", ".delete-btn", function () {
            var taskId = $(this).data("task-id");
            var taskTitle = $(this).data("task-title");

            Swal.fire({
                title: "Delete this task?",
                text: "\"" + taskTitle + "\" will be permanently removed.",
                icon: "warning",
                showCancelButton: true,
                confirmButtonColor: "#dc2626",
                cancelButtonColor: "#6b7280",
                confirmButtonText: "Yes, delete",
                cancelButtonText: "Cancel",
                buttonsStyling: true,
                reverseButtons: true
            }).then(function (result) {
                if (result.isConfirmed) {
                    $("#delete-form-" + taskId).submit();
                }
            });
        });
    }

    function initFilterAutoSubmit() {
        $(document).on("change", "#filterForm input[name='filter'], #sortSelect", function () {
            $("#filterForm").submit();
        });
    }

    function initPasswordToggle() {
        $(document).on("click", ".tm-password-toggle", function () {
            var $btn = $(this);
            var $input = $btn.closest(".tm-input-group").find("input");
            var isPwd = $input.attr("type") === "password";
            $input.attr("type", isPwd ? "text" : "password");
            $btn.find("i")
                .toggleClass("bi-eye", !isPwd)
                .toggleClass("bi-eye-slash", isPwd);
        });
    }

    $(document).ready(function () {
        initSearch();
        initToggleStatus();
        initDeleteConfirm();
        initFilterAutoSubmit();
        initPasswordToggle();
    });

}(jQuery));
