create or alter procedure sp_GetFicoSetting(
	@SCORECARD_CODE	varchar(3)
	,@VALUE 		int
)
AS
	select SCORE
	from FICO_SETTING
	where SCORECARD_CODE=@SCORECARD_CODE
		and @VALUE between VALUE_FROM and VALUE_TO
GO
