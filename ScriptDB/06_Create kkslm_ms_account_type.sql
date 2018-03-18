

CREATE TABLE [dbo].[kkslm_ms_account_type](
	[AccountTypeId] [int] IDENTITY(1,1) NOT NULL,
	[AccountTypeCode] [varchar](50) NULL,
	[AccountTypeName] [varchar](100) NULL,
 CONSTRAINT [PK_kkslm_ms_account_type] PRIMARY KEY CLUSTERED 
(
	[AccountTypeId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO


grant select,insert,update,delete on  [dbo].[kkslm_ms_account_type] to SLMDB;




set identity_insert [kkslm_ms_account_type] on;
insert into [kkslm_ms_account_type]([AccountTypeId],[AccountTypeCode],[AccountTypeName]) values (1,'D','Current Account')
insert into [kkslm_ms_account_type]([AccountTypeId],[AccountTypeCode],[AccountTypeName]) values (2,'S','Savings Account')
insert into [kkslm_ms_account_type]([AccountTypeId],[AccountTypeCode],[AccountTypeName]) values (3,'T','Term Deposit Account')
insert into [kkslm_ms_account_type]([AccountTypeId],[AccountTypeCode],[AccountTypeName]) values (4,'L','Loan Account')
insert into [kkslm_ms_account_type]([AccountTypeId],[AccountTypeCode],[AccountTypeName]) values (5,'X','Special Account')

GO
set identity_insert [kkslm_ms_account_type] off;
