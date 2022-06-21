using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using IntelART.OnlineLoans.Entities;
using IntelART.OnlineLoans.Repositories;
using IntelART.Utilities.DocumentStore;
using IntelART.Utilities.PrintableFormGenerator;
using IntelART.Utilities;
using IntelART.OnlineLoans.DirectApplicationRestApi.Helper;

namespace IntelART.OnlineLoans.DirectApplicationRestApi.Controllers
{
    /// <summary>
    /// Controller class to implement the API methods required for viewing, creating, and 
    /// managing loan applications
    /// </summary>
    ////[Authorize(Roles ="ShopUser,ShopPowerUser")]
    [Route("/Applications/{id}/Documents")]
    public class LoanApplicationDocumentsController : RepositoryControllerBase<ApplicationRepository>
    {
        private IDocumentStore documentStore;
        private IPrintableFormGenerator printableFormGenerator;
        private IConfigurationSection printTemplateConfiguration;
        private IConfigurationSection individualSheetTemplateConfiguration;

        public LoanApplicationDocumentsController(IConfigurationRoot configuration, IDocumentStore documentStore, IPrintableFormGenerator printableFormGenerator)
            : base(configuration, (connectionString) => new ApplicationRepository(connectionString))
        {
            this.documentStore = documentStore;
            this.printableFormGenerator = printableFormGenerator;
            this.printTemplateConfiguration = configuration.GetSection("PrintTemplates");
            this.individualSheetTemplateConfiguration = configuration.GetSection("IndividualSheetPrintTemplates");
        }

        private string GetDocumentUniqueName(Guid applicationId, string documentType)
        {
            return string.Format("{0}_{1}", applicationId, documentType);
        }

        /// <summary>
        /// Implements GET /Applications/{id}/Documents
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpGet]
        ////public async Task<IEnumerable<LoanApplicationAttachmentMetadata>> Get(Guid id)
        public async Task<IEnumerable<string>> Get(Guid id)
        {
            IEnumerable<string> result = new string[0];
            IEnumerable<ApplicationScan> applicationDocuments = this.Repository.GetApplicationScan(id);
            if (applicationDocuments != null)
            {
                result = applicationDocuments.Select(doc => this.ResolveDocumentTypeIdCode(doc.APPLICATION_SCAN_TYPE_CODE));
            }
            return result;
        }

        private string NullString()
        {
            return "Չի պահանջվում";
        }

        /// <summary>
        /// Implements GET /Applications/{id}/Documents/{documentType}
        /// </summary>
        /// <param name="id"></param>
        /// <param name="documentType"></param>
        /// <returns></returns>
        [HttpGet("{documentType}")]
        public async Task<IActionResult> Get(Guid id, string documentType)
        {
            IActionResult result;
            Stream stream;
            string mimeType = "application/octet-stream";
            string originalFileName = null;
            if (documentType == "DOC_SCORING_REQUEST_AGREEMENT")
            {
                mimeType = "text/html; charset=utf-8";
                Dictionary<string, string> data = new Dictionary<string, string>();

                InitialApplication application = await this.Repository.GetInitialApplication(id);

                if (application == null)
                {
                    throw new ApplicationException("E-5001", string.Format("Unknown application (id {0})", id));
                }

                data["FIRST_NAME"] = application.FIRST_NAME_AM;
                data["LAST_NAME"] = application.LAST_NAME_AM;
                data["PATRONYMIC_NAME"] = application.PATRONYMIC_NAME_AM;
                ////data["FULL_ADDRESS"] = "";
                data["PASSPORT_NUMBER"] = application.DOCUMENT_NUMBER;
                data["DATE"] = DateTime.Now.ToString("dd/MM/yyyy");
                stream = await this.printableFormGenerator.GnerateFormAsync(documentType, data);
            }
            else if (documentType == "DOC_ARBITRAGE_AGREEMENT")
            {
                mimeType = "text/html; charset=utf-8";
                Dictionary<string, string> data = new Dictionary<string, string>();

                ApplicationContractDetails appContract = await this.Repository.GetApplicationContractDetails(id);

                if (appContract == null)
                {
                    throw new ApplicationException("E-5001", string.Format("Unknown application (id {0})", id));
                }

                string addressFormat1 = "{0} {1}, {2}, {3}, {4}";
                string addressFormat2 = "{0} {1}, բն․ {2}, {3}, {4}, {5}";

                string registrationAddress = "";

                if (string.IsNullOrWhiteSpace(appContract.REGISTRATION_APARTMENT))
                {
                    registrationAddress = string.Format(addressFormat1,
                        appContract.REGISTRATION_STREET,
                        appContract.REGISTRATION_BUILDNUM,
                        appContract.REGISTRATION_CITY_NAME,
                        appContract.REGISTRATION_STATE_NAME,
                        appContract.REGISTRATION_COUNTRY_NAME);
                }
                else
                {
                    registrationAddress = string.Format(addressFormat2,
                        appContract.REGISTRATION_STREET,
                        appContract.REGISTRATION_BUILDNUM,
                        appContract.REGISTRATION_APARTMENT,
                        appContract.REGISTRATION_CITY_NAME,
                        appContract.REGISTRATION_STATE_NAME,
                        appContract.REGISTRATION_COUNTRY_NAME);
                }

                data["DATE"] = appContract.CREATION_DATE.ToString("dd/MM/yyyy");
                data["FIRST_NAME"] = appContract.FIRST_NAME;
                data["LAST_NAME"] = appContract.LAST_NAME;
                data["PATRONYMIC_NAME"] = appContract.PATRONYMIC_NAME;
                data["ADDRESS"] = registrationAddress;
                data["PASSPORT_NUMBER"] = appContract.DOCUMENT_NUMBER;
                stream = await this.printableFormGenerator.GnerateFormAsync(documentType, data);
            }
            else if (documentType == "DOC_INDIVIDUAL_SHEET")
            {
                mimeType = "text/html; charset=utf-8";
                Dictionary<string, string> data = new Dictionary<string, string>();

                PersonalSheet personalSheet = this.Repository.GetApplicationForPersonalSheet(id);

                if (personalSheet == null)
                {
                    throw new ApplicationException("E-5001", string.Format("Unknown application (id {0})", id));
                }

                string template = null;
                if (this.individualSheetTemplateConfiguration != null)
                {
                    template = this.individualSheetTemplateConfiguration[personalSheet.LOAN_TYPE_ID];
                }
                if (template == null)
                {
                    template = documentType;
                }

                data["TITLE"] = personalSheet.FORM_CAPTION;
                data["ID"] = personalSheet.ID.ToString();
                data["DATE"] = personalSheet.DATE.ToString("dd/MM/yyyy");
                data["LOAN_TYPE"] = personalSheet.LOAN_TYPE;
                data["LOAN_AMOUNT"] = personalSheet.LOAN_AMOUNT;
                data["MONTH_DURATION"] = personalSheet.LOAN_DURATION + " ամիս";
                data["LOAN_INTEREST"] = personalSheet.LOAN_INTEREST.ToString("#0.##") + "%";
                data["LOAN_INTEREST_2"] = personalSheet.LOAN_INTEREST_2.ToString("#0.##") + "%";
                data["TOTAL_REPAYMENT"] = personalSheet.TOTAL_REPAYMENT.ToString("###,###,###0");
                data["TOTAL_REPAYMENT_INTEREST"] = personalSheet.TOTAL_INTEREST_AMOUNT.ToString("###,###,###0");
                data["OTHER_PAYMENTS"] = personalSheet.OTHER_PAYMENTS.ToString("###,###,###0");
                data["SECURING_MORTGAGE"] = this.NullString();
                data["SECURING_WARRANTY"] = this.NullString();
                data["REPAYMENT_FREQUENCY_BASE"] = "Ամսական";
                data["REPAYMENT_FREQUENCY_INTEREST"] = "Ամսական";
                data["REPAYMENT_FREQUENCY_OTHER"] = null;
                data["DOWNPAYMENT"] = null;
                data["PENALTY_1"] = "օրական ժամկ. Վարկի 0,15%-ի չափով տույժ, 5>= աշխ. օր ժամկետանցի դեպքում՝ տուգանք ժամկետանց վարկի 2%ի չափով";
                data["PENALTY_2"] = "օրական ժամկ. Տոկոսի 0,3%-ի չափով տույժ, 5>= աշխ. օր ժամկետանցի դեպքում՝ տուգանք ժամկետանց %ի 5%ի չափով";
                data["PENALTY_3"] = this.NullString();
                data["CREDITOR_NAME"] = "\"Կոնվերս Բանկ\" ՓԲԸ";
                data["CREDITOR_ADDRESS"] = "ք. Երևան, Հանրապետության Հրապարակ, Վազգեն Սարգսյան փողոց 26/1";
                data["CREDITOR_PHONE"] = "010-51-12-11";
                data["CREDITOR_EMAIL"] = "info@conversebank.am";
                data["CUSTOMER_NAME"] = personalSheet.CUSTOMER_NAME;
                data["CUSTOMER_ADDRESS"] = personalSheet.CUSTOMER_ADDRESS;
                data["CUSTOMER_PHONE"] = personalSheet.CUSTOMER_PHONE;
                data["CUSTOMER_EMAIL"] = personalSheet.CUSTOMER_EMAIL;
                switch (personalSheet.COMMUNICATION_TYPE_CODE)
                {
                    case "2":
                        data["CUSTOMER_PREFERRED_COMMUNICATION"] = personalSheet.CUSTOMER_ADDRESS;
                        break;
                    case "3":
                        data["CUSTOMER_PREFERRED_COMMUNICATION"] = personalSheet.CURRENT_ADDRESS;
                        break;
                    default:
                        data["CUSTOMER_PREFERRED_COMMUNICATION"] = personalSheet.CUSTOMER_EMAIL;
                        break;
                }
                data["OTHER_PAYMENTS_APPLICATION_FEE"] = this.NullString();
                data["OTHER_PAYMENTS_LOAN_SERVICE_FEE"] = personalSheet.OTHER_PAYMENTS_LOAN_SERVICE_FEE.ToString("###,###,###0");
                data["OTHER_PAYMENTS_MORTGAGE_EVALUATION_FEE"] = this.NullString();
                data["OTHER_PAYMENTS_NOTARY_FEE"] = this.NullString();
                data["OTHER_PAYMENTS_MORTGAGE_INSURANCE_FEE"] = this.NullString();
                data["OTHER_PAYMENTS_OTHER_FEES"] = personalSheet.OTHER_PAYMENTS_OTHER_FEE.ToString("###,###,###0");
                data["SIGNATURE_DATE"] = personalSheet.SIGNATURE_DATE.ToString("dd/MM/yyyy");
                data["SIGNATURE_DATE1"] = personalSheet.SIGNATURE_DATE1.ToString("dd/MM/yyyy");
                data["SIGNATURE_DATE2"] = personalSheet.SIGNATURE_DATE2.ToString("dd/MM/yyyy");

                data["OTHER_PAYMENTS_CARD_SERVICE_FEE"] = personalSheet.OTHER_PAYMENTS_CARD_SERVICE_FEE.ToString("###,###,###0");
                data["OTHER_PAYMENTS_CASH_OUT_FEE"] = personalSheet.OTHER_PAYMENTS_CASH_OUT_FEE.ToString("###,###,###0");

                stream = await this.printableFormGenerator.GnerateFormAsync(template, data);
            }
            else if (documentType == "DOC_LOAN_CONTRACT")
            {
                Application app = await this.Repository.GetInitialApplication(id);

                if (app == null)
                {
                    throw new ApplicationException("E-5001", string.Format("Unknown application (id {0})", id));
                }

                string template = null;
                if (this.printTemplateConfiguration != null)
                {
                    template = this.printTemplateConfiguration[app.LOAN_TYPE_ID];
                }

                if (template != null)
                {
                    mimeType = "text/html; charset=utf-8";
                    Dictionary<string, string> data = new Dictionary<string, string>();
                    ApplicationContractDetails appContract = await this.Repository.GetApplicationContractDetails(id);
                    PersonalSheet personalSheet = this.Repository.GetApplicationForPersonalSheet(id);
                    ClientDataForCardValidation clientData = await Repository.GetClientDataForCardValidation(id);
                    string branchName = "";
                    if (clientData != null)
                    {
                        Client client = await this.Repository.GetClientData(clientData.CLIENT_CODE);
                        if (client != null)
                        {
                            branchName = client.BranchName;
                        }
                    }

                    if (appContract == null)
                    {
                        throw new ApplicationException("E-5001", string.Format("Unknown application (id {0})", id));
                    }

                    string addressFormat1 = "{0} {1}, {2}, {3}, {4}";
                    string addressFormat2 = "{0} {1}, բն․ {2}, {3}, {4}, {5}";

                    string registrationAddress = "";
                    string currentAddress = "";

                    if (string.IsNullOrWhiteSpace(appContract.REGISTRATION_APARTMENT))
                    {
                        registrationAddress = string.Format(addressFormat1,
                            appContract.REGISTRATION_STREET,
                            appContract.REGISTRATION_BUILDNUM,
                            appContract.REGISTRATION_CITY_NAME,
                            appContract.REGISTRATION_STATE_NAME,
                            appContract.REGISTRATION_COUNTRY_NAME);
                    }
                    else
                    {
                        registrationAddress = string.Format(addressFormat2,
                            appContract.REGISTRATION_STREET,
                            appContract.REGISTRATION_BUILDNUM,
                            appContract.REGISTRATION_APARTMENT,
                            appContract.REGISTRATION_CITY_NAME,
                            appContract.REGISTRATION_STATE_NAME,
                            appContract.REGISTRATION_COUNTRY_NAME);
                    }

                    if (string.IsNullOrWhiteSpace(appContract.CURRENT_APARTMENT))
                    {
                        currentAddress = string.Format(addressFormat1,
                            appContract.CURRENT_STREET,
                            appContract.CURRENT_BUILDNUM,
                            appContract.CURRENT_CITY_NAME,
                            appContract.CURRENT_STATE_NAME,
                            appContract.CURRENT_COUNTRY_NAME);
                    }
                    else
                    {
                        currentAddress = string.Format(addressFormat2,
                            appContract.CURRENT_STREET,
                            appContract.CURRENT_BUILDNUM,
                            appContract.CURRENT_APARTMENT,
                            appContract.CURRENT_CITY_NAME,
                            appContract.CURRENT_STATE_NAME,
                            appContract.CURRENT_COUNTRY_NAME);
                    }

                    data["DATE"] = DateTime.Now.ToString("dd/MM/yyyy");
                    data["BRANCH_NAME"] = branchName;
                    data["CLIENT_CODE"] = appContract.CLIENT_CODE;
                    data["CLIENT_LOAN_CODE"] = string.Empty;
                    data["FIRST_NAME_AM"] = appContract.FIRST_NAME;
                    data["LAST_NAME_AM"] = appContract.LAST_NAME;
                    data["PATRONYMIC_NAME_AM"] = appContract.PATRONYMIC_NAME;
                    data["FIRST_NAME_EN"] = appContract.FIRST_NAME_EN;
                    data["LAST_NAME_EN"] = appContract.LAST_NAME_EN;
                    data["IDENTIFICATION_DOCUMENT_NUMBER"] = appContract.DOCUMENT_NUMBER;
                    data["IDENTIFICATION_DOCUMENT_ISSUED_BY"] = appContract.DOCUMENT_GIVEN_BY;
                    data["IDENTIFICATION_DOCUMENT_ISSUED_ON"] = appContract.DOCUMENT_GIVEN_DATE.ToString("dd/MM/yyyy");
                    data["IDENTIFICATION_DOCUMENT_VALID_UNTIL"] = appContract.DOCUMENT_EXPIRY_DATE.ToString("dd/MM/yyyy");
                    data["SSN"] = appContract.SOCIAL_CARD_NUMBER;
                    data["BIRTH_DATE"] = appContract.BIRTH_DATE.ToString("dd/MM/yyyy");
                    data["BIRTH_COUNTRY"] = appContract.BIRTH_PLACE_NAME;
                    data["NATIONALITY"] = appContract.CITIZENSHIP_COUNTRY_NAME;
                    data["MARITAL_STATUS"] = appContract.FAMILY_STATUS;
                    data["REGISTRATION_ADDRESS"] = registrationAddress;
                    data["ACTUAL_ADDRESS"] = currentAddress;
                    data["LANDLINE_PHONE_NUMBER"] = appContract.FIXED_PHONE;
                    data["CELL_PHONE_NUMBER"] = appContract.MOBILE_PHONE_1;
                    data["EMAIL"] = appContract.EMAIL;
                    data["EMPLOYER_NAME"] = appContract.COMPANY_NAME;
                    data["EMPLOYER_TYPE"] = appContract.ORGANIZATION_ACTIVITY_NAME;
                    data["EMPLOYER_PHONE_NUMBER"] = appContract.COMPANY_PHONE;
                    data["EMPLOYMENT_POSITION"] = appContract.POSITION;
                    data["WORK_EXPERIENCE"] = appContract.WORKING_EXPERIENCE_NAME;
                    data["NET_SALARY"] = appContract.MONTHLY_INCOME_NAME;
                    data["OTHER_INCOME"] = "";
                    data["OTHER_INCOME_SOURCE"] = "";
                    data["REQUESTED_AMOUNT"] = appContract.INITIAL_AMOUNT.ToString("#,###");
                    data["REQUESTED_REPAYMENT_DAY"] = appContract.REPAY_DAY.ToString();
                    data["FINE_AMOUNT_1"] = "____________";
                    data["APPROVED_AMOUNT"] = appContract.FINAL_AMOUNT.ToString("#,###");
                    data["APPROVED_AMOUNT_TEXT"] = appContract.FINAL_AMOUNT.ToWords();
                    data["APPROVED_REPAYMENT_PERIOD"] = personalSheet.REPAYMENT_END_DATE.ToString("dd/MM/yyyy");
                    data["LOAN_DURATION"] = personalSheet.LOAN_DURATION;
                    data["APPROVED_REPAYMENT_DAY"] = appContract.REPAY_DAY.ToString();
                    data["APPROVED_LOAN_INTEREST"] = appContract.INTEREST.ToString("#,###.##") + "%";
                    data["APPROVED_LOAN_ACTUAL_INTEREST"] = personalSheet.LOAN_INTEREST_2.ToString("#,###.##") + "%";
                    data["LOAN_PROVISION_FEE"] = "0 ";
                    data["TOTAL_EXPENSE"] = personalSheet.TOTAL_REPAYMENT_AMD.ToString("#,###");
                    data["APPLICATION_SUBMITION_DATE"] = appContract.CREATION_DATE.ToString("dd/MM/yyyy");
                    data["APPLICATION_ACCEPTANCE_DATE"] = data["DATE"];

                    data["CARD_BRANCH_PICKUP_CHECK"] = appContract.CARD_DELIVERY_BANK_BRANCH_NAME != null ? " checked " : null;
                    data["CARD_ADDRESS_DELIVERY_CHECK"] = appContract.CARD_DELIVERY_ADDRESS != null ? " checked " : null;

                    data["IS_NEW_CARD_CHECKED"] = appContract.IS_NEW_CARD == 1 ? " checked " : "";
                    data["IS_NEW_CARD_NOT_CHECKED"] = appContract.IS_NEW_CARD != 1 ? " checked " : "";
                    if (personalSheet.LOAN_DURATION == "0")
                    {
                        data["LOAN_DURATION"] = "Անժամկետ կամ մինչև Բանկի կողմից հետ կանչման պահը";
                    }
                    else
                    {
                        data["LOAN_DURATION"] = personalSheet.LOAN_DURATION.ToString() + " ամիս";
                    }
                    data["EXISTING_CARD_CODE"] = appContract.EXISTING_CARD_CODE;
                    data["CURRENCY_NAME"] = appContract.CURRENCY_NAME;
                    data["CARD_TYPE_NAME"] = appContract.NEW_CARD_TYPE_NAME;
                    data["CARD_SECRET_DELIVERY_PHONE"] = appContract.MOBILE_PHONE_1;
                    data["CARD_DELIVERY_BRANCH_NAME"] = appContract.CARD_DELIVERY_BANK_BRANCH_NAME;
                    data["CARD_DELIVERY_ADDRESS"] = appContract.CARD_DELIVERY_ADDRESS;
                    var loanPrintableSchedule = new List<object>();
                    if (personalSheet.SCHEDULE != null && personalSheet.SCHEDULE.Any())
                    {
                        LoanSchedule scheduleItem;
                        decimal plannedPaidLoanAmount = 0;
                        for (int i = 0; i < personalSheet.SCHEDULE.Count; i++)
                        {
                            scheduleItem = personalSheet.SCHEDULE[i];
                            plannedPaidLoanAmount += scheduleItem.MainAmount;

                            loanPrintableSchedule.Add(new {
                                scheduleItem.Date,
                                MainAmount = scheduleItem.MainAmount.ToString("#,###.##"),
                                InterestAmount = scheduleItem.InterestAmount.ToString("#,###.##"),
                                No = string.Format("{0}.", i + 1),
                                TotalAmount = (scheduleItem.MainAmount + scheduleItem.InterestAmount).ToString("#,###.##"),
                                LeftLoanAmount = (appContract.FINAL_AMOUNT - plannedPaidLoanAmount).ToString("#,###.##"),
                        });
                        }
                    }
                    data["LOAN_SCHEDULE"] = Newtonsoft.Json.JsonConvert.SerializeObject(loanPrintableSchedule, new CustomDateTimeConverter());

                    stream = await this.printableFormGenerator.GnerateFormAsync(template, data);
                }
                else
                {
                    mimeType = "application/pdf";
                    originalFileName = "Contract.pdf";
                    byte[] content = this.Repository.GetApplicationPrint(id, 1);

                    if (content == null)
                    {
                        throw new ApplicationException("E-5002", string.Format("The contract file does not exist for the application {0}", id));
                    }
                    stream = new MemoryStream(content);
                    stream.Position = 0;
                }
            }
            else if (documentType == "DOC_LOAN_CONTRACT_FINAL")
            {
                mimeType = "application/pdf";
                originalFileName = "Contract.pdf";
                byte[] content = this.Repository.GetApplicationPrint(id, 1);

                if (content == null)
                {
                    throw new ApplicationException("E-5002", string.Format("The contract file does not exist for the application {0}", id));
                }
                stream = new MemoryStream(content);
                stream.Position = 0;
            }
            else
            {
                ////stream = await this.documentStore.RetriveDocumentAsync(this.GetDocumentUniqueName(id, documentType));
                stream = null;
                IEnumerable<ApplicationScan> applicationDocuments = this.Repository.GetApplicationScan(id);
                string documentTypeCodeCode = this.ResolveDocumentTypeCodeCode(documentType);
                if (applicationDocuments != null)
                {
                    foreach (ApplicationScan documentMetadata in applicationDocuments)
                    {
                        if (documentMetadata.APPLICATION_SCAN_TYPE_CODE == documentTypeCodeCode)
                        {
                            if (documentMetadata.FILE_NAME != null)
                            {
                                int splitIndex = documentMetadata.FILE_NAME.LastIndexOf('|');
                                if (splitIndex >= 0)
                                {
                                    originalFileName = documentMetadata.FILE_NAME.Substring(0, splitIndex);
                                    mimeType = documentMetadata.FILE_NAME.Substring(splitIndex + 1);
                                    byte[] content = this.Repository.GetApplicationScanContent(id, documentTypeCodeCode);
                                    if (content == null)
                                    {
                                        throw new ApplicationException("E-5002", string.Format("The contract file does not exist for the application {0}", id));
                                    }
                                    stream = new MemoryStream(content);
                                    stream.Position = 0;
                                }
                            }
                            break;
                        }
                    }
                }
            }
            result = File(stream, mimeType, originalFileName);
            return result;
        }

        /// <summary>
        /// Implements PUT /Applications/{id}/Documents/{documentType}
        /// </summary>
        /// <param name="id"></param>
        /// <param name="documentType"></param>
        /// <returns></returns>
        [HttpPut("{documentType}")]
        public async Task<string> Put(Guid id, string documentType)
        {
            if (HttpContext.Request.HasFormContentType)
            {
                if (HttpContext.Request.Form != null
                    && HttpContext.Request.Form.Files != null)
                {
                    foreach (IFormFile file in HttpContext.Request.Form.Files)
                    {
                        string originalFileName = file.FileName;
                        string outputFileName = this.GetDocumentUniqueName(id, documentType);

                        try
                        {
                            ////await this.documentStore.StoreDocumentAsync(outputFileName, file.OpenReadStream());
                            using (MemoryStream ms = new MemoryStream())
                            {
                                file.OpenReadStream().CopyTo(ms);
                                ////this.Repository.SaveApplicationScan(id, ResolveDocumentTypeCodeId(documentType), string.Format("{0}|{1}", originalFileName, file.ContentType), new byte[0]);
                                this.Repository.SaveApplicationScan(id, ResolveDocumentTypeCodeCode(documentType), string.Format("{0}|{1}", originalFileName, file.ContentType), ms.ToArray());
                            }
                        }
                        catch (Exception ex)
                        {
                            throw new ApplicationException("E-0010", string.Format("Filed to save uploaded file {0}", originalFileName), ex);
                        }
                    }
                }
            }
            else
            {
                throw new ApplicationException("E-0310", string.Format("The request for saving document {0} for application {1} does not contain valid payload", documentType, id));
            }
            return "{Status: Success}";
        }

        private string ResolveDocumentTypeCodeCode(string typeCode)
        {
            string result = "0";
            switch(typeCode)
            {
                case "DOC_PASSPORT":
                    result = "1";
                    break;
                case "DOC_SOC_CARD":
                    result = "2";
                    break;
                case "DOC_INDIVIDUAL_SHEET_SIGNED":
                    result = "6";
                    break;
                case "DOC_SCORING_REQUEST_AGREEMENT_SIGNED":
                    result = "7";
                    break;
            }
            return result;
        }

        private string ResolveDocumentTypeIdCode(string typeId)
        {
            string result = "";
            switch (typeId)
            {
                case "1":
                    result = "DOC_PASSPORT";
                    break;
                case "2":
                    result = "DOC_SOC_CARD";
                    break;
                case "6":
                    result = "DOC_INDIVIDUAL_SHEET_SIGNED";
                    break;
                case "7":
                    result = "DOC_SCORING_REQUEST_AGREEMENT_SIGNED";
                    break;
            }
            return result;
        }

        /// <summary>
        /// Implements DELETE /Applications/{id}/Documents/{documentType}
        /// </summary>
        /// <param name="id"></param>
        /// <param name="documentType"></param>
        /// <returns></returns>
        [HttpDelete("{documentType}")]
        public async Task Delete(Guid id, string documentType)
        {
            ////await this.documentStore.DeleteDocumentAsync(this.GetDocumentUniqueName(id, documentType));
            this.Repository.DeleteApplicationScan(id, this.ResolveDocumentTypeCodeCode(documentType));
        }
    }
}
