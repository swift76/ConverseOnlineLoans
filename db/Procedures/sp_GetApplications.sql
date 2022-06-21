if exists (select * from sys.objects where name='sp_GetApplications' and type='P')
	drop procedure dbo.sp_GetApplications
GO

create procedure dbo.sp_GetApplications(@CUSTOMER_USER_ID	int)
AS
	select
		a.LOAN_TYPE_ID,
		a.CREATION_DATE,
		case isnull(a.FINAL_AMOUNT,a.INITIAL_AMOUNT)
			when 0
				then ''
			else
				CONVERT(varchar, isnull(a.FINAL_AMOUNT,a.INITIAL_AMOUNT), 1) +
					' ' + isnull(a.CURRENCY_CODE,'')
		end as DISPLAY_AMOUNT,
		a.STATUS_ID,
		a.ID,
		case isnull(s.UI_NAME_AM,'') when '' then s.NAME_AM else s.UI_NAME_AM end +
		case
		   when a.STATUS_ID in (5,8) then ' /' + dbo.f_GetApprovedAmount(a.ID,a.LOAN_TYPE_ID) + ' ' + a.CURRENCY_CODE + '/'
		   when isnull(a.REFUSAL_REASON,'')<>'' then ' /' + a.REFUSAL_REASON + '/'
		   else ''
		end as STATUS_AM,
		s.NAME_EN as STATUS_EN,
		t.NAME_AM as LOAN_TYPE_AM,
		t.NAME_EN as LOAN_TYPE_EN,
		isnull(a.PRINT_EXISTS,0) as PRINT_EXISTS
	from dbo.APPLICATION a with (NOLOCK)
	join dbo.APPLICATION_STATUS s with (NOLOCK)
		on a.STATUS_ID = convert(tinyint,s.CODE)
	join dbo.LOAN_TYPE t with (NOLOCK)
		on a.LOAN_TYPE_ID = t.CODE
	where CUSTOMER_USER_ID = @CUSTOMER_USER_ID
	order by a.CREATION_DATE desc
GO
