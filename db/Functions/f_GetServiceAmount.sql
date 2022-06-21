if exists (select * from sys.objects where name='f_GetServiceAmount' and type='FN')
	drop function dbo.f_GetServiceAmount
GO

create function dbo.f_GetServiceAmount(@APPLICATION_AMOUNT	money,
									   @SERVICE_AMOUNT		money,
									   @SERVICE_INTEREST	money)
RETURNS money
AS
BEGIN
	declare @Result money

	if isnull(@SERVICE_AMOUNT,0)>0
		set @Result=@SERVICE_AMOUNT
	else
		set @Result=@APPLICATION_AMOUNT*isnull(@SERVICE_INTEREST,0)/100

	return @Result
END
GO
