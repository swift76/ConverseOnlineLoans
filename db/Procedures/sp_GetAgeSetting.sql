create or alter procedure sp_GetAgeSetting(
	@SCORECARD_CODE	varchar(3)
	,@VALUE 		tinyint
)
AS
	select SCORE
	from AGE_SETTING
	where SCORECARD_CODE=@SCORECARD_CODE
		and @VALUE between VALUE_FROM and VALUE_TO
GO
