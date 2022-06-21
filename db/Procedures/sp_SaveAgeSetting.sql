create or alter procedure sp_SaveAgeSetting(
	@AGE 	tinyint
	,@SCORE	int
)
AS
	insert into AGE_SETTING(AGE, SCORE)
	values (@AGE, @SCORE)
GO
