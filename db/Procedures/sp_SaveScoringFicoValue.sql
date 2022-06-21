create or alter procedure sp_SaveScoringFicoValue(
	@FACTOR_ID 	tinyint,
	@FROM_VALUE	money,
	@TO_VALUE	money,
	@SCORE		money
)
AS
	insert into SCORING_FICO_VALUE (FACTOR_ID,FROM_VALUE,TO_VALUE,SCORE)
		values (@FACTOR_ID,@FROM_VALUE,@TO_VALUE,@SCORE)
GO
