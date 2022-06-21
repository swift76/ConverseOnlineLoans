if exists (select * from sys.objects where name='f_GetApprovedAmount' and type='FN')
	drop function dbo.f_GetApprovedAmount
GO

create function dbo.f_GetApprovedAmount(@APPLICATION_ID	uniqueidentifier,
										@LOAN_TYPE_ID	char(2))
RETURNS varchar(1000)
AS
BEGIN
	declare @Result varchar(1000)

	if @LOAN_TYPE_ID='00'
		select @Result = CONVERT(varchar, AMOUNT, 1)
		from dbo.APPLICATION_SCORING_RESULT
		where APPLICATION_ID = @APPLICATION_ID
	else
		select @Result = CONVERT(varchar, max(AMOUNT), 1)
		from dbo.APPLICATION_SCORING_RESULT
		where APPLICATION_ID = @APPLICATION_ID

	return @Result
END
GO
