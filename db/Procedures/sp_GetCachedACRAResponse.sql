if exists (select * from sys.objects where name='sp_GetCachedACRAResponse' and type='P')
	drop procedure dbo.sp_GetCachedACRAResponse
GO

create procedure dbo.sp_GetCachedACRAResponse(@SOCIAL_CARD_NUMBER	char(10))
AS
	declare @DayCount int
	select @DayCount=convert(int,VALUE) from SETTING where CODE='ACRA_CACHE_DAY'

	declare @DateTo date=getdate()
	declare @DateFrom date=dateadd(month,-@DayCount,@DateTo)

	select top 1 n.RESPONSE_XML
	from APPLICATION a
	join ACRA_QUERY_RESULT n
		on a.ID=n.APPLICATION_ID
	where a.SOCIAL_CARD_NUMBER=@SOCIAL_CARD_NUMBER
		and convert(date,n.QUERY_DATE) between @DateFrom and @DateTo
	order by n.QUERY_DATE desc
GO
