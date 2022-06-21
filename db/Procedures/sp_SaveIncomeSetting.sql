create or alter procedure sp_SaveIncomeSetting(
	@VALUE_FROM	float
	,@VALUE_TO	float
	,@SCORE		int
)
AS
	insert into INCOME_SETTING(VALUE_FROM, VALUE_TO, SCORE)
	values (@VALUE_FROM, @VALUE_TO, @SCORE)
GO
