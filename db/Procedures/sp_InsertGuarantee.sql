if exists (select * from sys.objects where name='sp_InsertGuarantee' and type='P')
	drop procedure dbo.sp_InsertGuarantee
GO

create procedure sp_InsertGuarantee(@ApplicationID uniqueidentifier,
									@ClientCode char(8),
									@Amount money,
									@Currency char(3),
									@ID int output)
AS
	insert into GUARANTEE(APPLICATION_ID, CLIENT_CODE, AMOUNT, CURRENCY)
	values(@ApplicationID, @ClientCode, @Amount, @Currency)

	select @ID = SCOPE_IDENTITY()
GO
