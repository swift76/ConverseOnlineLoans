create or alter procedure sp_SaveFicoSetting(
	@SCORECARD_CODE	varchar(3)
	,@VALUE_FROM	int
	,@VALUE_TO		int
	,@SCORE			int
)
AS
	insert into FICO_SETTING(SCORECARD_CODE, VALUE_FROM, VALUE_TO, SCORE)
	values (@SCORECARD_CODE, @VALUE_FROM, @VALUE_TO, @SCORE)
GO
