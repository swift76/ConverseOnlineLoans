create or alter procedure sp_GetScoringFicoValues
AS
	select FACTOR_ID, FROM_VALUE, TO_VALUE, SCORE
	from SCORING_FICO_VALUE
GO
