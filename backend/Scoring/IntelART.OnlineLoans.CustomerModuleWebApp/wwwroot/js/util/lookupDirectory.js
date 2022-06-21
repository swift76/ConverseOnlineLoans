define(['jquery'], function ($) {
    var countries = null;
    var bankBranches = null;
    var identificationDocumentTypes = null;
    var loanTypes = null;
    var industries = null;
    var currencies = [];
    var states = [];
    var cities = [];
    var loanParameters = [];
    var maritalStatuses = null;
    var monthlyIncomeRanges = null;
    var workExperienceDurations = null;
    var addressCountries = null;
    var generalLoanSettings = null;
    var statementDeliveryMethods = null;

    LookupDirectory = function () {
    }

    LookupDirectory.prototype.getLoanParameters = function (loanTypeCode, callback) {
        if (loanParameters[loanTypeCode]) {
            callback(loanParameters[loanTypeCode]);
        } else {
            $.get({
                url: "/api/loan/Parameters?loanTypeCode=" + loanTypeCode,
                context: this,
                success: function (data) {
                    loanParameters[loanTypeCode] = data;
                    callback(loanParameters[loanTypeCode]);
                },
                dataType: 'json'
            });
        }
    };

    LookupDirectory.prototype.getGeneralLoanSettings = function (callback) {
        if (generalLoanSettings) {
            callback(generalLoanSettings);
        } else {
            $.get({
                url: "/api/loan/Parameters/GeneralLoanSettings",
                context: this,
                success: function (data) {
                    generalLoanSettings = data;
                    callback(generalLoanSettings);
                },
                dataType: 'json'
            });
        }
    };

    LookupDirectory.prototype.getCountries = function (callback) {
        if (countries != null) {
            callback(countries);
        }
        else {
            $.get({
                url: "/api/loan/Directory/Countries",
                context: this,
                success: function (data) {
                    countries = data;
                    callback(countries);
                },
                dataType: 'json'
            });
        }
    };

    LookupDirectory.prototype.getBankBranches = function (callback) {
        if (bankBranches != null) {
            callback(bankBranches);
        }
        else {
            $.get({
                url: "/api/loan/Directory/BankBranches",
                context: this,
                success: function (data) {
                    bankBranches = data;
                    callback(bankBranches);
                },
                dataType: 'json'
            });
        }
    };

    LookupDirectory.prototype.getLoanTypes = function (callback) {
        if (loanTypes != null) {
            callback(loanTypes);
        }
        else {
            $.get({
                url: "/api/loan/Directory/LoanTypes",
                context: this,
                success: function (data) {
                    loanTypes = data;
                    callback(loanTypes);
                },
                dataType: 'json'
            });
        }
    };

    LookupDirectory.prototype.getIdentificationDocumentTypes = function (callback) {
        if (identificationDocumentTypes != null) {
            callback(identificationDocumentTypes);
        }
        else {
            $.get({
                url: "/api/loan/Directory/IdDocumentTypes",
                context: this,
                success: function (data) {
                    identificationDocumentTypes = data;
                    callback(identificationDocumentTypes);
                },
                dataType: 'json'
            });
        }
    };

    LookupDirectory.prototype.getStates = function (country, callback) {
        //var countryCode = country;
        var countryCode = '';
        if (states[countryCode]) {
            callback(states[countryCode]);
        }
        else {
            $.get({
                url: "/api/loan/Directory/States",
                context: this,
                success: function (data) {
                    states[countryCode] = data;
                    callback(states[countryCode]);
                },
                dataType: 'json'
            });
        }
    };

    LookupDirectory.prototype.getCities = function (state, callback) {
        if (cities[state]) {
            callback(cities[state]);
        }
        else {
            $.get({
                url: "/api/loan/Directory/States/" + state + "/Cities",
                context: this,
                success: function (data) {
                    cities[state] = data;
                    callback(cities[state]);
                },
                dataType: 'json'
            });
        }
    };

    LookupDirectory.prototype.getIndustries = function (callback) {
        if (industries) {
            callback(industries);
        }
        else {
            $.get({
                url: "/api/loan/Directory/Industries",
                context: this,
                success: function (data) {
                    industries = data;
                    callback(industries);
                },
                dataType: 'json'
            });
        }
    };

    LookupDirectory.prototype.getCurrencies = function (loanType, callback) {
        if (loanType) {
            if (currencies[loanType]) {
                callback(currencies[loanType]);
            }
            else {
                $.get({
                    url: "/api/loan/Directory/Currencies/" + loanType,
                    context: this,
                    success: function (data) {
                        currencies[loanType] = data;
                        callback(currencies[loanType]);
                    },
                    dataType: 'json'
                });
            }
        }
        else {
            return null;
        }
    };

    LookupDirectory.prototype.getMaritalStatuses = function (callback) {
        if (maritalStatuses) {
            callback(maritalStatuses);
        }
        else {
            $.get({
                url: "/api/loan/Directory/MaritalStatuses",
                context: this,
                success: function (data) {
                    maritalStatuses = data;
                    callback(maritalStatuses);
                },
                dataType: 'json'
            });
        }
    };

    LookupDirectory.prototype.getMonthlyIncomeRanges = function (callback) {
        if (monthlyIncomeRanges) {
            callback(monthlyIncomeRanges);
        }
        else {
            $.get({
                url: "/api/loan/Directory/MonthlyNetIncomeRanges",
                context: this,
                success: function (data) {
                    monthlyIncomeRanges = data;
                    callback(monthlyIncomeRanges);
                },
                dataType: 'json'
            });
        }
    };

    LookupDirectory.prototype.getWorkExperienceDurations = function (callback) {
        if (workExperienceDurations) {
            callback(workExperienceDurations);
        }
        else {
            $.get({
                url: "/api/loan/Directory/WorkExperienceDurations",
                context: this,
                success: function (data) {
                    workExperienceDurations = data;
                    callback(workExperienceDurations);
                },
                dataType: 'json'
            });
        }
    };

    LookupDirectory.prototype.getAddressCountries = function (callback) {
        if (addressCountries != null) {
            callback(addressCountries);
        }
        else {
            $.get({
                url: "/api/loan/Directory/AddressCountries",
                context: this,
                success: function (data) {
                    addressCountries = data;
                    callback(addressCountries);
                },
                dataType: 'json'
            });
        }
    };

    LookupDirectory.prototype.getBankStatementDeliveryMethods = function (callback) {
        if (statementDeliveryMethods != null) {
            callback(statementDeliveryMethods);
        }
        else {
            $.get({
                url: "/api/loan/Directory/CommunicationTypes",
                context: this,
                success: function (data) {
                    statementDeliveryMethods = data;
                    callback(statementDeliveryMethods);
                },
                dataType: 'json'
            });
        }
    };

    return LookupDirectory;
});