if exists (select * from sys.types where name = 'PledgeSubject')
	drop type dbo.PledgeSubject
GO

CREATE TYPE PledgeSubject AS TABLE(
	PledgeSubjectName nvarchar(100),
	Amount money,
	Count money)
GO
