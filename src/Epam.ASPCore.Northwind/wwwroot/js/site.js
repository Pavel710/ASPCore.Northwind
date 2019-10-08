// Please see documentation at https://docs.microsoft.com/aspnet/core/client-side/bundling-and-minification
// for details on configuring this project to bundle and minify static web assets.

// Write your JavaScript code.

function AddItem() {
    window.location = window.location.href + "\\Add";
};

function UpdateItem(btn) {
    var id = $(btn).parent().parent()[0].id;
    window.location = window.location.href + "\\Update?id=" + id;
};

function CancelAction() {
    window.history.back();
};