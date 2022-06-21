if exists (select * from sys.objects where name='sp_GetApplicationForNORQRequestByISN' and type='P')
	drop procedure dbo.sp_GetApplicationForNORQRequestByISN
GO

create procedure dbo.sp_GetApplicationForNORQRequestByISN(@ISN	int)
AS
	select a.ID
		,isnull(u.SOCIAL_CARD_NUMBER,a.SOCIAL_CARD_NUMBER) as SOCIAL_CARD_NUMBER
		,a.DOCUMENT_TYPE_CODE
		,a.DOCUMENT_NUMBER
		,isnull(u.FIRST_NAME_AM,a.FIRST_NAME_AM) as FIRST_NAME_AM
		,isnull(u.LAST_NAME_AM,a.LAST_NAME_AM) as LAST_NAME_AM
		,null as BIRTH_DATE
		,a.CUSTOMER_USER_ID
	from dbo.APPLICATION a
	left join dbo.CUSTOMER_USER u
		on a.CUSTOMER_USER_ID=u.APPLICATION_USER_ID
	where a.ISN=@ISN and a.STATUS_ID=1
GO
