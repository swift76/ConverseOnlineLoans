create or alter procedure sp_SaveAgeSetting(
	@SCORECARD_CODE	varchar(3)
	,@VALUE_FROM	tinyint
	,@VALUE_TO		tinyint
	,@SCORE			int
)
AS
	insert into AGE_SETTING(SCORECARD_CODE, VALUE_FROM, VALUE_TO, SCORE)
	values (@SCORECARD_CODE, @VALUE_FROM, @VALUE_TO, @SCORE)
GO
