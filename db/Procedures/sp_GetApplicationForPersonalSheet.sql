if exists (select * from sys.objects where name='sp_GetApplicationForPersonalSheet' and type='P')
	drop procedure dbo.sp_GetApplicationForPersonalSheet
GO

create procedure dbo.sp_GetApplicationForPersonalSheet(@ID	uniqueidentifier)
AS
	declare @CurrentDate date = getdate()

	select  a.ID,
			a.CREATION_DATE as DATE,
		 	t.NAME_AM as LOAN_TYPE,
		 	CONVERT(varchar, a.FINAL_AMOUNT, 1) + ' ' + a.CURRENCY_CODE as LOAN_AMOUNT,
		 	ltrim(rtrim(a.PERIOD_TYPE_CODE)) as LOAN_DURATION,
		 	a.INTEREST as LOAN_INTEREST,
		 	rtrim(n.FIRST_NAME) + ' ' + rtrim(n.LAST_NAME) as CUSTOMER_NAME,
		 	case st.CODE
		 		when '001' then st.NAME_AM
		 		else cm.NAME_AM
		 	end +
	 		', ' + a.REGISTRATION_STREET +
	 		', ' + a.REGISTRATION_BUILDNUM +
	 		case when isnull(a.REGISTRATION_APARTMENT,'')='' then '' else ', ' + a.REGISTRATION_APARTMENT end
		 	as CUSTOMER_ADDRESS,
		 	case stl.CODE
		 		when '001' then stl.NAME_AM
		 		else cml.NAME_AM
		 	end +
	 		', ' + a.CURRENT_STREET +
	 		', ' + a.CURRENT_BUILDNUM +
	 		case when isnull(a.CURRENT_APARTMENT,'')='' then '' else ', ' + a.CURRENT_APARTMENT end
		 	as CURRENT_ADDRESS,
		 	a.COMMUNICATION_TYPE_CODE,
		 	a.MOBILE_PHONE_1 as CUSTOMER_PHONE,
		 	a.EMAIL as CUSTOMER_EMAIL,
			dbo.f_GetServiceAmount(a.FINAL_AMOUNT,sc1.AMOUNT,sc1.INTEREST) as OTHER_PAYMENTS_LOAN_SERVICE_FEE,
			dbo.f_GetServiceAmount(a.FINAL_AMOUNT,sc2.AMOUNT,sc2.INTEREST) as OTHER_PAYMENTS_CARD_SERVICE_FEE,
			dbo.f_GetServiceAmount(a.FINAL_AMOUNT,sc3.AMOUNT,sc3.INTEREST) as OTHER_PAYMENTS_CASH_OUT_FEE,
			dbo.f_GetServiceAmount(a.FINAL_AMOUNT,sc9.AMOUNT,sc9.INTEREST) as OTHER_PAYMENTS_OTHER_FEE,
			@CurrentDate as SIGNATURE_DATE,
			@CurrentDate as SIGNATURE_DATE1,
			@CurrentDate as SIGNATURE_DATE2,

			t.IS_OVERDRAFT,
			case t.IS_OVERDRAFT
				when 1 then a.OVERDRAFT_TEMPLATE_CODE
				else a.LOAN_TEMPLATE_CODE
			end as TEMPLATE_CODE,
			a.FINAL_AMOUNT,
		 	a.CURRENCY_CODE,
			a.REPAY_DAY,
			a.LOAN_TYPE_ID,
			case t.IS_CARD_ACCOUNT
				when 0 then N'ՍՊԱՌՈՂԱԿԱՆ ՎԱՐԿԻ'
				else N'ՎԱՐԿԱՅԻՆ ԳԾԻ (ՕՎԵՐԴՐԱՖՏԻ)'
			end + N' ԷԱԿԱՆ ՊԱՅՄԱՆՆԵՐԻ ԱՆՀԱՏԱԿԱՆ ԹԵՐԹԻԿ' as FORM_CAPTION
	from dbo.APPLICATION as a
	join dbo.LOAN_TYPE t
		on t.CODE = a.LOAN_TYPE_ID
	join dbo.STATE st
		on st.CODE = a.REGISTRATION_STATE_CODE
	join dbo.CITY cm
		on cm.CODE = a.REGISTRATION_CITY_CODE
	join NORQ_QUERY_RESULT n
		on a.ID = n.APPLICATION_ID
	left join dbo.LOAN_SERVICE_CONDITION sc1
		on sc1.SERVICE_TYPE_CODE='1' and sc1.LOAN_TYPE_CODE = a.LOAN_TYPE_ID and sc1.CURRENCY = a.CURRENCY_CODE
			and isnull(sc1.CREDIT_CARD_TYPE_CODE,'') in (isnull(a.CREDIT_CARD_TYPE_CODE,''),'')
	left join dbo.LOAN_SERVICE_CONDITION sc2
		on sc2.SERVICE_TYPE_CODE='2' and sc2.LOAN_TYPE_CODE = a.LOAN_TYPE_ID and sc2.CURRENCY = a.CURRENCY_CODE
			and isnull(sc2.CREDIT_CARD_TYPE_CODE,'') in (isnull(a.CREDIT_CARD_TYPE_CODE,''),'')
	left join dbo.LOAN_SERVICE_CONDITION sc3
		on sc3.SERVICE_TYPE_CODE='3' and sc3.LOAN_TYPE_CODE = a.LOAN_TYPE_ID and sc3.CURRENCY = a.CURRENCY_CODE
			and isnull(sc3.CREDIT_CARD_TYPE_CODE,'') in (isnull(a.CREDIT_CARD_TYPE_CODE,''),'')
	left join dbo.LOAN_SERVICE_CONDITION sc9
		on sc9.SERVICE_TYPE_CODE='9' and sc9.LOAN_TYPE_CODE = a.LOAN_TYPE_ID and sc9.CURRENCY = a.CURRENCY_CODE
			and isnull(sc9.CREDIT_CARD_TYPE_CODE,'') in (isnull(a.CREDIT_CARD_TYPE_CODE,''),'')
	left join dbo.STATE stl
		on stl.CODE = a.CURRENT_STATE_CODE
	left join dbo.CITY cml
		on cml.CODE = a.CURRENT_CITY_CODE
	where ID = @ID
GO
