if exists (select * from sys.objects where name='sp_GetApplicationForACRARequestByISN' and type='P')
	drop procedure dbo.sp_GetApplicationForACRARequestByISN
GO

create procedure dbo.sp_GetApplicationForACRARequestByISN(@ISN	int)
AS
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
	where a.ISN=@ISN and a.STATUS_ID=2
GO
