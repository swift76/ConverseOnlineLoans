create or alter procedure sp_GetIncomeSetting(
	@SCORECARD_CODE	varchar(3)
	,@VALUE			money
)
AS
	select SCORE
	from INCOME_SETTING
	where SCORECARD_CODE=@SCORECARD_CODE
		and @VALUE between VALUE_FROM and VALUE_TO
GO
