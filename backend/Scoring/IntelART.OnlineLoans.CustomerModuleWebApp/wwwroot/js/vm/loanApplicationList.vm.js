define(['knockout', 'jquery', '../util/notificationDialog'], function (ko, $, NotificationDialog) {
     $(document).ajaxComplete(function (event, xhr, settings) {
                if (xhr.status == 401) {
                    var dialog = new NotificationDialog({
                        message: localization.strings["MESSAGE.END_SESSION"],
                        title: "Աշխատանքի ավարտ",
                        messageClass: "dialogNotify",
                        confirmButtonText: "Ավարտել",
                        visibleButton: "true",
                        visibleFirstButton: "false",
                        close: function () { window.location.href = "/account/logout"; },
                        confirm: function () { window.location.href = "/account/logout"; }
                    });
                    dialog.show();
                    dialog.notifyComplete();
                }
            });
            
    LoanApplicationList = function () {
        this.items = ko.onDemandObservable(this.getItems, this);
        this.selectStatusClass = function (state) {
            
            if (state == "NEW" || state == 'PENDING_PRE_APPROVAL' || state == "PENDING_APPROVAL" || state == "PHONE_VERIFICATION_PENDING" || state == "APPROVAL_REVIEW") {
                return 'pending';
            } else if (state == "PRE_APPROVAL_SUCCESS" || state == "APPROVAL_SUCCESS" || state == "AGREED" || state == "DELIVERING" || state == "COMPLETED") {
                return 'approved';
            } else {
				return 'rejected';
            }
        }
        this.deleteApplication = function(id) {
                $.ajax({
                    type: "DELETE",
                    url: "/api/loan/Applications/"+id,
                    context: this,
                    dataType: 'json'
                });
        }

        this.cancelApplication = function(id) {
           
                $.ajax({
                    type: "POST",
                    url: "/api/loan/Applications/Cancelled/"+id,
                    context: this,
                    data: JSON.stringify({}),
                    dataType: 'json',
                });
        }
        this.printApplication = function(id) {
            var xhr = new XMLHttpRequest();
            xhr.open("GET", "/api/loan/Applications/" + id + "/Documents/DOC_LOAN_CONTRACT_FINAL");
            xhr.onload = function () {
                if (xhr.status == 500) {
                    alert("Պայմանագրի տպելու ձևը դեռ պատրաստ չէ");
                } else {
                    var win = window.open("/api/loan/Applications/" + id + "/Documents/DOC_LOAN_CONTRACT_FINAL");
                    //win.print();
                }
            };
            xhr.send();
        }
        this.visitOffice = function() {
            var dialog = new NotificationDialog({ message: localization.strings["MESSAGE.VISIT_BANK_BRANCH"], title: "Մոտեցեք մասնաճյուղ",  messageClass: "dialogSuccess", visibleButton: "false", confirm: function () { navigationHelper.navigateToApplicationList(); } });
            dialog.show();
        }
    }
    
    LoanApplicationList.prototype.getItems = function () {
        var refreshInterval = 10000;
        var self = this;      
                    
        $.get({
            url: "/api/loan/Applications",
            context: self,
            success: function (data) {
                this.items(data);
                this.items.loaded(true);
                setTimeout(this.items.refresh, refreshInterval);
            },
            error: function (err) {
                setTimeout(this.items.refresh, refreshInterval);
            },
            dataType: 'json'
        });
    };

    return LoanApplicationList;
});