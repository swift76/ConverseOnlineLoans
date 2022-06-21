create or alter procedure sp_SaveGenderSetting(
	@SCORECARD_CODE	varchar(3)
	,@GENDER		bit
	,@SCORE			int
)
AS
	insert into GENDER_SETTING(SCORECARD_CODE, GENDER, SCORE)
	values (@SCORECARD_CODE, @GENDER, @SCORE)
GO
