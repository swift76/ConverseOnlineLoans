define(["jquery", "knockout"], function ($,ko) {

    function FileUploader(targetUrl, callbacks, restrictions) {
        var self = this;
        this.fileUrl = ko.observable();
        this.loadedData = ko.observable(0);
        this.totalData = ko.observable(0);
        this.isUploadingStarted = ko.observable(false);
        this.isUploadingFinished = ko.observable(false);
        this.isUploadingFailed = ko.observable(false);

        this.upload = function () {
            self.isUploadingStarted(true);
            self.isUploadingFinished(false);
        }

        this.progress = function (loaded, total) {
            self.loadedData(loaded);
            self.totalData(total);
        }

        this.complete = function () {
            self.isUploadingFinished(true);
            self.isUploadingStarted(false);
        }

        this.uploadSuccess = function () {
            self.isUploadingStarted(false);
            self.isUploadingFinished(true);
            self.isUploadingFailed(false);
            if (callbacks && callbacks.uploadSuccess) {
                callbacks.uploadSuccess();
            }
        }

        this.uploadError = function (error) {
            self.isUploadingStarted(false);
            self.isUploadingFinished(true);
            self.isUploadingFailed(true);

            if (callbacks && callbacks.uploadError) {
                callbacks.uploadError(error);
            }
        }

        this.getRestrictionsViolations = function (file) {
            let violations = {
                hasViolation: false
            }

            if (!restrictions) {
                return violations
            }

            if (restrictions.fileMaxSize) {
                if (file.size > restrictions.fileMaxSize) {
                    violations.maxSizeExceeded = true;
                    violations.hasViolation = true;
                }
            }

            return violations;
        }

        this.fileUpload = function (data, e) {
            var file = e.target.files[0];
            var violations = self.getRestrictionsViolations(file);

            if (violations.hasViolation) {
                self.uploadError(violations);
                return;
            }

            var formData = new FormData();
            formData.append('file', file);

            var reader = new FileReader();

            reader.onloadend = function (onloadend_e) {
                var result = reader.result;

                $.ajax({
                    url: targetUrl,
                    type: 'PUT',
                    data: formData,
                    cache: false,
                    contentType: false,
                    processData: false,

                    xhr: function () {
                        var customXhr = $.ajaxSettings.xhr();
                        if (customXhr.upload) {
                            self.upload();

                            customXhr.upload.addEventListener('progress', function (e) {
                                if (e.lengthComputable) {
                                    self.progress(e.loaded, e.total);
                                }
                            }, false);
                        }
                        return customXhr;
                    },

                    complete: function () {
                        self.complete();
                    },

                    success: function () {
                        self.uploadSuccess();
                    },

                    error: function () {
                        self.uploadError();
                    }
                });


                self.fileUrl(result);
            };

            if (file) {
                reader.readAsDataURL(file);
            }
        };
    }

    function getFileMaxSize(callback) {
        if (FILE_MAX_SIZE) {
            callback();
            return;
        }
        $.get({
            url: "/api/loan/Settings/FileMaxSize",
            context: self,
            success: function (data) {
                FILE_MAX_SIZE = data;
                if (callback) {
                    callback();
                }
            },
            dataType: 'json'
        });
    }


    return {
        "FileUploader": FileUploader,
        "getFileMaxSize": getFileMaxSize
    }
});