﻿/// <reference path="../_intellisense/browserlink.intellisense.js" />
/// <reference path="../_intellisense/jquery-1.8.2.js" />

(function (browserLink, $) {
    /// <param name="browserLink" value="bl" />
    /// <param name="$" value="jQuery" />

    var file;

    function getFile(element) {
        file = browserLink.sourceMapping.getCompleteRange(element).sourcePath;
    }

    window.__weReportError = function (key, success) {
        browserLink.call("Error", key, success, file);
    };

    return {
        name: "BestPractices", // Has to match the BrowserLinkFactoryName attribute. Not needed in final version of VS2013

        onInit: function () { // Renamed to 'onConnected' in final version of VS2013

            setTimeout(function () {

                // HTML 5 microdata
                var microdata = $("[itemscope]").length > 0;
                window.__weReportError("microdata", microdata);


                // Description
                var description = $("meta[name=description]").length > 0;

                if (!description) {
                    var head = document.getElementsByTagName("head");

                    if (head.length > 0 && browserLink.sourceMapping.canMapToSource(head[0])) {
                        getFile(head[0]);
                    }
                }

                window.__weReportError("description", description);


                // Viewport
                var viewport = $("meta[name=viewport]").length > 0;
                if (file)
                    window.__weReportError("viewport", viewport);

            }, 500);

        }
    };
});