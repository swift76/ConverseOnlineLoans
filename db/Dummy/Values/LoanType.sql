﻿insert into dbo.LOAN_TYPE (CODE, NAME_AM, NAME_EN, REPAYMENT_DAY_FROM, REPAYMENT_DAY_TO, IS_OVERDRAFT, IS_REPAY_DAY_FIXED, IS_CARD_ACCOUNT, IS_REPAY_START_DAY, IS_REPAY_NEXT_MONTH, REPAY_TRANSITION_DAY)
	values ('01', N'Սպառողական', 'Consumer', 1, 20, 0, 0, 1, 0, 1, 15)
GO
insert into dbo.LOAN_TYPE (CODE, NAME_AM, NAME_EN, REPAYMENT_DAY_FROM, REPAYMENT_DAY_TO, IS_OVERDRAFT, IS_REPAY_DAY_FIXED, IS_CARD_ACCOUNT, IS_REPAY_START_DAY, IS_REPAY_NEXT_MONTH, REPAY_TRANSITION_DAY)
	values ('02', N'Օվերդրաֆտ', 'Overdraft', 1, 25, 1, 1, 1, 1, 1, 25)
GO