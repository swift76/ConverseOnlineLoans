define(['knockout', './loanApplication.vm', './loanApplicationList.vm', './userProfile.vm', './changePassword.vm', 'objectLoader'],
    function (ko, LoanApplication, LoanApplicationList, UserProfile, ChangePassword, ObjectLoader) {
        RootViewModel = function () {
            var loanApplicationList = new LoanApplicationList();
            var userProfile = new ObjectLoader(UserProfile);
            var self = this;

            var navigationHelper = new function () {
                this.navigateToApplicationList = function () {
                    self.selectApplicationList();
                }
            }

            var changePassword = new ChangePassword(navigationHelper);
            this.selectedView = ko.observable();
            this.selectedViewCode = ko.observable();

            var viewLoanApplicationList = { code: "VIEW.LOAN_APP_LIST", view: loanApplicationList, icon: "hayter" };
            var viewLoanApplication = { code: "VIEW.LOAN_APP", view: null };
            var viewUserProfile = { code: "VIEW.USER_PROFILE", view: userProfile };
            var viewChangePassword = { code: "VIEW.CHANGE_PASSWORD", view: changePassword, icon: "password" };

            this.children = [
                viewLoanApplicationList,
                viewChangePassword,
            ];

            this.viewTemplate = function (vm) {
                if (vm === loanApplicationList) {
                    return 'loanApplicationList';
                }
                else if (vm === userProfile) {
                    return 'userProfile';
                }
                else if (vm === changePassword) {
                    return 'changePassword';
                }
                else if (vm instanceof LoanApplication) {
                    return 'loanApplication'
                }
                return '__empty__';
            }

            this.setCurrentView = function (vmDescriptor) {
                if (vmDescriptor) {
                    var vm = vmDescriptor.view;
                    if (vmDescriptor.code == "VIEW.LOAN_APP") {
                        vm = new LoanApplication(null, navigationHelper);
                    }
                    if (vm) {
                        this.selectedView(vm);
                        this.selectedViewCode(vmDescriptor.code);
                        // scroll the window to top
                        $("html, body").animate({
                            scrollTop: 0
                        }, 0);
                    }
                }
            }

            this.isViewCurrent = function (vm) {
                return vm === this.selectedView;
            }

            this.openApplication = function (id) {
                var vm = new LoanApplication(id, navigationHelper);
                this.selectedView(vm);
                this.selectedViewCode("VIEW.LOAN_APP");
            }

            this.selectDefaultView = function () {
                this.setCurrentView(viewLoanApplicationList);
            }

            this.selectDefaultView();
            this.selectNewApplicationView = function () {
                this.setCurrentView(viewLoanApplication);
            }

            this.selectApplicationList = function () {
                this.setCurrentView(viewLoanApplicationList);
            }
        }

        return RootViewModel;
    }
);