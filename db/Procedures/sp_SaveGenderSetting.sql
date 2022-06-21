create or alter procedure sp_SaveGenderSetting(
	@GENDER	bit
	,@SCORE	int
)
AS
	insert into GENDER_SETTING(GENDER, SCORE)
	values (@GENDER, @SCORE)
GO
