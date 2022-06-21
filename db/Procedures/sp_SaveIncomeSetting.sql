create or alter procedure sp_SaveIncomeSetting(
	@SCORECARD_CODE	varchar(3)
	,@VALUE_FROM	money
	,@VALUE_TO		money
	,@SCORE			int
)
AS
	insert into INCOME_SETTING(SCORECARD_CODE, VALUE_FROM, VALUE_TO, SCORE)
	values (@SCORECARD_CODE, @VALUE_FROM, @VALUE_TO, @SCORE)
GO
