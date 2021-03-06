﻿var OltDescriptionListController = function (oltDescriptionListService) {
    "use strict";

    var onuListTableTbody = $(".onu-list > tbody");
    var onuListTableRefreshButton = $("#refresh-onu-list");
    var onuListTableAlert = $("#onu-list-alert");
    var onuListTableAlertDescription = $("#onu-list-alert-description");
    var searchForm = $("#search-form");

    var init = function (container) {
        $(container).on("click", "li:not(.disabled) > .js-get-onu-list", refreshOnuList);
        $(document).on("click", "#refresh-onu-list:not(:disabled)", refreshOnuList);
    };

    var initializeOnuList = function (oltId) {
        markActiveNavbarLink(oltId);
        onuListTableRefreshButton.addClass("disabled");
        onuListTableRefreshButton.addClass("gly-spin");
        oltDescriptionListService.getOltDescriptionList(oltId, done, fail);
    };

    var refreshOnuList = function (e) {
        var oltId = $(e.target).attr("data-olt-id");

        markActiveNavbarLink(oltId);
        onuListTableRefreshButton.addClass("disabled");
        onuListTableRefreshButton.addClass("gly-spin");
        onuListTableTbody.empty();
        searchForm.val("");
        oltDescriptionListService.getOltDescriptionList(oltId, done, fail);
    };

    var done = function (oltId, result) {
        onuListTableAlert.addClass("hidden");
        onuListTableAlertDescription.empty();
        onuListTableTbody.attr("data-olt-id", oltId);
        onuListTableRefreshButton.attr("data-olt-id", oltId);

        for (var i in result) {
            var oltPortId = result[i].oltPortId;
            var onuId = result[i].onuId;
            var onuGponSerialNumber = result[i].onuGponSerialNumber;
            var onuDescription;
            if (result[i].onuDescription !== "")
                onuDescription = result[i].onuDescription;
            else
                onuDescription = "(" + result[i].onuGponSerialNumber + ")";

            var onuOpticalConnectionStateStyle;
            if (result[i].onuOpticalConnectionState == "up")
                onuOpticalConnectionStateStyle = "text-default";
            else
                onuOpticalConnectionStateStyle = "text-danger";

            onuListTableTbody.append(
            "<tr class='clickable-row' data-olt-port-id='" + oltPortId + "' data-onu-id='" + onuId + "' data-href='#'>" +
            "<td class='onu-list-id'>" + oltPortId + "." + onuId + "</td>" +
                "<td class='onu-list-sn'><i class='glyphicon glyphicon-barcode " + onuOpticalConnectionStateStyle + "' title='" + onuGponSerialNumber + "'></i></td>" +
            "<td class='onu-list-item'>" + onuDescription + "</td>" +
            "</tr>");
        }
        onuListTableRefreshButton.removeClass("gly-spin");
        onuListTableRefreshButton.removeClass("disabled");
        removeDisabledPropertyFromNavbarLink();
    };

    var fail = function (oltId, result) {
        onuListTableAlert.removeClass("hidden");
        onuListTableAlertDescription.text(result.responseText);
        onuListTableTbody.attr("data-olt-id", oltId);
        onuListTableRefreshButton.attr("data-olt-id", oltId);
        onuListTableRefreshButton.removeClass("gly-spin");
        onuListTableRefreshButton.removeClass("disabled");
        removeDisabledPropertyFromNavbarLink();
    };

    var markActiveNavbarLink = function (oltId) {
        $("a.js-get-onu-list").parent().removeClass("active");
        $("a.js-get-onu-list").parent().removeClass("disabled");
        $(`a.js-get-onu-list[data-olt-id="${oltId}"]`).parent().addClass("active");
        $(`a.js-get-onu-list[data-olt-id!="${oltId}"]`).parent().addClass("disabled");
    };

    var removeDisabledPropertyFromNavbarLink = function () {
        $("a.js-get-onu-list").parent().removeClass("disabled");
    };

    return {
        init: init,
        initializeOnuList: initializeOnuList
    };
}(OltDescriptionListService);