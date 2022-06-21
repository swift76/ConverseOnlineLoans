create or alter procedure sp_SaveFicoSetting(
	@VALUE_FROM	int
	,@VALUE_TO	int
	,@SCORE		int
)
AS
	insert into FICO_SETTING(VALUE_FROM, VALUE_TO, SCORE)
	values (@VALUE_FROM, @VALUE_TO, @SCORE)
GO
