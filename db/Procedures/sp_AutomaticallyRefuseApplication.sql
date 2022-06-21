if exists (select * from sys.objects where name='sp_AutomaticallyRefuseApplication' and type='P')
	drop procedure dbo.sp_AutomaticallyRefuseApplication
GO

create procedure dbo.sp_AutomaticallyRefuseApplication(@ID				uniqueidentifier,
													   @REFUSAL_REASON	nvarchar(100))
AS
	declare @STATUS_ID tinyint
	if @REFUSAL_REASON=N'Սխալ փաստաթղթի տվյալներ'
		set @STATUS_ID=0
	else
		set @STATUS_ID=6
	update dbo.APPLICATION
	set REFUSAL_REASON=@REFUSAL_REASON
		,STATUS_ID=@STATUS_ID
		,TO_BE_SYNCHRONIZED=1
	where ID=@ID
GO
