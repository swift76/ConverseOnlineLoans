if exists (select * from sys.objects where name='sp_GetInitialApplication' and type='P')
	drop procedure dbo.sp_GetInitialApplication
GO

create procedure dbo.sp_GetInitialApplication(@ID uniqueidentifier)
AS
	select  a.CREATION_DATE,
		    a.SOURCE_ID,
		 	a.ID,
			a.LOAN_TYPE_ID,
			a.STATUS_ID,
			a.CUSTOMER_USER_ID,
			a.SHOP_USER_ID,
			a.PARTNER_COMPANY_CODE,
			a.INITIAL_AMOUNT,
			a.CURRENCY_CODE,
			a.REFUSAL_REASON,
			a.MANUAL_REASON,
			a.IS_NEW_CARD,
			a.EXISTING_CARD_CODE, -- TODO: check whether this is needed
			a.FIRST_NAME_AM,
			a.LAST_NAME_AM,
			a.PATRONYMIC_NAME_AM,
			a.BIRTH_DATE,
			a.SOCIAL_CARD_NUMBER,
			isnull(a.DOCUMENT_TYPE_CODE, c.DOCUMENT_TYPE_CODE) as DOCUMENT_TYPE_CODE,
			isnull(a.DOCUMENT_NUMBER, c.DOCUMENT_NUMBER) as DOCUMENT_NUMBER,
			a.DOCUMENT_GIVEN_BY,
			a.DOCUMENT_GIVEN_DATE,
			a.DOCUMENT_EXPIRY_DATE,
			a.ORGANIZATION_ACTIVITY_CODE,
			a.PRODUCT_TOTAL_AMOUNT,
			a.PREPAID_AMOUNT,
			a.CUSTOMER_STATUS_ID,
			a.IS_DATA_COMPLETE,
			a.CLIENT_CODE,
			a.ISN,
			a.HAS_BANK_CARD,
			dbo.f_GetApprovedAmount(a.ID, a.LOAN_TYPE_ID) as DISPLAY_AMOUNT,
			isnull(a.PRINT_EXISTS,0) as PRINT_EXISTS,
			isnull(a.AGREEMENT_NUMBER,'') as AGREEMENT_NUMBER
	from dbo.APPLICATION as a
	left join dbo.CUSTOMER_USER as c
		on a.CUSTOMER_USER_ID = c.APPLICATION_USER_ID
	where ID = @ID
GO
