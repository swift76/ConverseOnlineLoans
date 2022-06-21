if exists (select * from sys.objects where name='sp_GetPledgeSubject' and type='P')
	drop procedure dbo.sp_GetPledgeSubject
GO

create procedure sp_GetPledgeSubject(@ID int)
AS
	select	PLEDGE_SUBJECT_NAME,
			COUNT,
			AMOUNT
	from PLEDGE_SUBJECT
	where PLEDGE_ID = @ID
GO
