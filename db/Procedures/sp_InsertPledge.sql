if exists (select * from sys.objects where name='sp_InsertPledge' and type='P')
	drop procedure dbo.sp_InsertPledge
GO

create procedure sp_InsertPledge(@ApplicationID uniqueidentifier,
								 @ClientCode char(8) = NULL,
								 @Amount money,
								 @Currency char(3),
								 @Details nvarchar(100)	= NULL,
								 @Ratio	money = NULL,
								 @PledgeName nvarchar(100),
								 @Price money,
								 @PledgeSubject PledgeSubject readonly,
								 @ID int output)
AS
BEGIN TRANSACTION
BEGIN TRY
	insert into PLEDGE(APPLICATION_ID, CLIENT_CODE, AMOUNT, CURRENCY, DETAILS, RATIO, PLEDGE_NAME, PRICE)
	values(@ApplicationID, @ClientCode, @Amount, @Currency, @Details, @Ratio, @PledgeName, @Price)

	select @ID = SCOPE_IDENTITY()

	declare @PledgeSubjectName nvarchar(100),
			@Count money,
			@AmountPS money

	Select	@PledgeSubjectName = PledgeSubjectName,
			@Count = Count,
			@AmountPS = Amount
	from @PledgeSubject

	insert into PLEDGE_SUBJECT(PLEDGE_ID, PLEDGE_SUBJECT_NAME, COUNT, AMOUNT)
	values(@ID, @PledgeSubjectName, @Count, @AmountPS)

	COMMIT TRANSACTION
END TRY

BEGIN CATCH
	ROLLBACK TRANSACTION
END CATCH
GO
