define([
    'knockout',
    'jquery',
    './applicationProcessStage',
    './loanTypeList.vm',
    './loanPreApprovalApplication.vm',
    './loanMainApplicationWrapper.vm',
    './cardIdentification.vm',
    './contractSummary.vm'
],

    function (ko, $,
        ApplicationStage,
        LoanTypeList,
        LoanPreApprovalApplication,
        LoanMainApplication,
        CardIdentification,
        ContractDetails) {

        LoanApplication = function (id, navigationHelper) {
            this.id = id;
            var self = this;

            this.selectedStage = ko.observable(null);

            var preApprovalStage = new ApplicationStage("VIEW.APPSTAGE.PRE_APPROVAL", self.id, function (id) { return new LoanPreApprovalApplication(id, navigationHelper, preApprovalStage.isEditable); });
            var mainApplicationStage = new ApplicationStage("VIEW.APPSTAGE.MAIN", self.id, function (id) { return new LoanMainApplication(id, function () { return preApprovalStage.view(); }, mainApplicationStage.isEditable); });
            var cardIdentificationStage = new ApplicationStage("VIEW.APPSTAGE.CARD_IDENTIFICATION", self.id, function (id) { return new CardIdentification(id, function () { return preApprovalStage.view(); }, function () { preApprovalStage.view.refresh(); }, cardIdentificationStage.isEditable); });
            var finalizationStage = new ApplicationStage("VIEW.APPSTAGE.CONTRACT_DETAILS", self.id, function (id) { return new ContractDetails(id, function () { return preApprovalStage.view(); }, navigationHelper, finalizationStage.isEditable); });

            preApprovalStage.isAccessible(true);

            var self = this;

            this.children = ko.pureComputed(
                function () {
                    var result = [preApprovalStage];
                    if (preApprovalStage.view.loaded()) {
                        var view = preApprovalStage.view();
                        if (view) {
                            var loanType = view.loanType();
                            if (loanType) {
                                result = [
                                    preApprovalStage,
                                    mainApplicationStage,
                                    finalizationStage
                                ];
                            }
                            applyApplicationState(view);
                        }
                    }

                    return result;
                });

            function applyApplicationState(preApprovalApplicationView) {
                if (preApprovalApplicationView.id && !self.id) {
                    self.id = preApprovalApplicationView.id;
                    mainApplicationStage.applicationId = self.id;
                    cardIdentificationStage.applicationId = self.id;
                    finalizationStage.applicationId = self.id;
                }
                var applicationState = preApprovalApplicationView.state();
                if (applicationState) {
                    switch (applicationState) {
                        case "PENDING_PRE_APPROVAL":
                            preApprovalStage.isEditable(false);
                            break;
                        case "PRE_APPROVAL_SUCCESS":
                            mainApplicationStage.isAccessible(true);
                            preApprovalStage.isEditable(false);
                            self.setCurrentView(mainApplicationStage);
                            break;
                        case "PENDING_APPROVAL":
                        case "PHONE_VERIFICATION_PENDING":
                            cardIdentificationStage.isAccessible(true);
                            mainApplicationStage.isAccessible(true);
                            preApprovalStage.isEditable(false);
                            mainApplicationStage.isEditable(false);
                            self.setCurrentView(cardIdentificationStage);
                            break;
                        case "APPROVAL_SUCCESS":
                            // load 3rd page of the application in editable mode
                            finalizationStage.isAccessible(true);
                            cardIdentificationStage.isAccessible(true);
                            mainApplicationStage.isAccessible(true);
                            preApprovalStage.isEditable(false);
                            mainApplicationStage.isEditable(false);
                            cardIdentificationStage.isEditable(false);
                            self.setCurrentView(finalizationStage);
                            break;
                        case "AGREED":
                        case "COMPLETED":
                        case "APPROVAL_REVIEW":
                            // load 3rd page. It is submitted and not editable
                            finalizationStage.isAccessible(true);
                            cardIdentificationStage.isAccessible(true);
                            mainApplicationStage.isAccessible(true);
                            preApprovalStage.isEditable(false);
                            mainApplicationStage.isEditable(false);
                            cardIdentificationStage.isEditable(false);
                            finalizationStage.isEditable(false);
                            self.setCurrentView(finalizationStage);
                            break;
                        case "CANCELLED":
                        case "PRE_APPROVAL_FAIL":
                        case "PRE_APPROVAL_REVIEW":
                        case "EXPIRED":
                            // load 1st page of the application, the rest pages should be disabled
                            preApprovalStage.isEditable(false);
                            break;
                    }
                }
            }

            this.viewTemplate = function (vm) {
                if (vm instanceof LoanPreApprovalApplication) {
                    return 'preApprovalApplication';
                } else if (vm instanceof LoanMainApplication) {
                    return 'mainApplication';
                } else if (vm instanceof CardIdentification) {
                    return 'cardIdentification';
                } else if (vm instanceof ContractDetails) {
                    return 'contractSummary';
                }
                return '__empty__';
            }

            this.setCurrentView = function (vmDescriptor) {
                if (vmDescriptor
                    && vmDescriptor.isAccessible
                    && vmDescriptor.isAccessible()) {
                    this.selectedStage(vmDescriptor);
                }
            }

            this.setCurrentView(this.children()[0]);

            this.isCurrentView = function (vm) {
                return vm && vm === this.selectedStage();
            }
        }

        return LoanApplication;
    });