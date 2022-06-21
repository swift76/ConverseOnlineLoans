if exists (select * from sys.objects where name='sp_GetApplicationsForACRARequest' and type='P')
	drop procedure dbo.sp_GetApplicationsForACRARequest
GO

create procedure dbo.sp_GetApplicationsForACRARequest
AS
	declare @ACRA_TRY_COUNT	tinyint
	select @ACRA_TRY_COUNT=convert(tinyint,VALUE)
		from dbo.SETTING
		where CODE='ACRA_TRY_COUNT'

	select a.ID,n.FIRST_NAME,n.LAST_NAME,n.BIRTH_DATE,
		case
			when rtrim(isnull(n.NON_BIOMETRIC_PASSPORT_NUMBER,''))<>'' then n.NON_BIOMETRIC_PASSPORT_NUMBER
			when rtrim(isnull(n.BIOMETRIC_PASSPORT_NUMBER,''))<>'' then n.BIOMETRIC_PASSPORT_NUMBER
			else n.ID_CARD_NUMBER
		end as PASSPORT_NUMBER
		,n.ID_CARD_NUMBER
		,n.SOCIAL_CARD_NUMBER
	from dbo.APPLICATION a
	join dbo.NORQ_QUERY_RESULT n
		on n.APPLICATION_ID=a.ID
	where a.STATUS_ID=2 and ACRA_TRY_COUNT<@ACRA_TRY_COUNT
	order by CREATION_DATE
GO
