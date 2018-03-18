
CREATE TABLE [dbo].[kkslm_ms_bank](
	[BankId] [int] IDENTITY(1,1) NOT NULL,
	[BankNo] [varchar](50) NULL,
	[BankName] [varchar](200) NULL,
 CONSTRAINT [PK_kkslm_ms_bank] PRIMARY KEY CLUSTERED 
(
	[BankId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO


grant select,insert,update,delete on  [dbo].[kkslm_ms_bank] to SLMDB;



set identity_insert [kkslm_ms_bank] on;
insert into [kkslm_ms_bank]([BankId],[BankNo],[BankName]) values (1,'69','ธนาคารเกียรตินาคิน')

GO
set identity_insert [kkslm_ms_bank] off;
