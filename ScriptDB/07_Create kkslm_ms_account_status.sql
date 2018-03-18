


CREATE TABLE [dbo].[kkslm_ms_account_status](
	[AccountStatusId] [int] IDENTITY(1,1) NOT NULL,
	[AccountStatusCode] [varchar](50) NULL,
	[AccountStatusName] [varchar](100) NULL,
	[AccountTypeCode] [varchar](50) NULL,
 CONSTRAINT [PK_kkslm_ms_account_status] PRIMARY KEY CLUSTERED 
(
	[AccountStatusId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO


grant select,insert,update,delete on  [dbo].[kkslm_ms_account_status] to SLMDB
go




set identity_insert [kkslm_ms_account_status] on;
insert into [kkslm_ms_account_status]([AccountStatusId],[AccountStatusCode],[AccountStatusName],[AccountTypecode]) values (1,'1','ACTIVE','S')
insert into [kkslm_ms_account_status]([AccountStatusId],[AccountStatusCode],[AccountStatusName],[AccountTypecode]) values (2,'2','CLOSED','S')
insert into [kkslm_ms_account_status]([AccountStatusId],[AccountStatusCode],[AccountStatusName],[AccountTypecode]) values (3,'3','MATURED NOT REDEEM','S')
insert into [kkslm_ms_account_status]([AccountStatusId],[AccountStatusCode],[AccountStatusName],[AccountTypecode]) values (4,'4','NEW TODAY','S')
insert into [kkslm_ms_account_status]([AccountStatusId],[AccountStatusCode],[AccountStatusName],[AccountTypecode]) values (5,'5','DO NOT CLOSE ON ZERO','S')
insert into [kkslm_ms_account_status]([AccountStatusId],[AccountStatusCode],[AccountStatusName],[AccountTypecode]) values (6,'6','NO DEBIT ALLOWED','S')
insert into [kkslm_ms_account_status]([AccountStatusId],[AccountStatusCode],[AccountStatusName],[AccountTypecode]) values (7,'7','FROZEN','S')
insert into [kkslm_ms_account_status]([AccountStatusId],[AccountStatusCode],[AccountStatusName],[AccountTypecode]) values (8,'8','CHARGE OFF','S')
insert into [kkslm_ms_account_status]([AccountStatusId],[AccountStatusCode],[AccountStatusName],[AccountTypecode]) values (9,'9','DORMANT','S')
insert into [kkslm_ms_account_status]([AccountStatusId],[AccountStatusCode],[AccountStatusName],[AccountTypecode]) values (10,'1','ACTIVE','D')
insert into [kkslm_ms_account_status]([AccountStatusId],[AccountStatusCode],[AccountStatusName],[AccountTypecode]) values (11,'2','CLOSED','D')
insert into [kkslm_ms_account_status]([AccountStatusId],[AccountStatusCode],[AccountStatusName],[AccountTypecode]) values (12,'3','MATURED NOT REDEEM','D')
insert into [kkslm_ms_account_status]([AccountStatusId],[AccountStatusCode],[AccountStatusName],[AccountTypecode]) values (13,'4','NEW TODAY','D')
insert into [kkslm_ms_account_status]([AccountStatusId],[AccountStatusCode],[AccountStatusName],[AccountTypecode]) values (14,'5','DO NOT CLOSE ON ZERO','D')
insert into [kkslm_ms_account_status]([AccountStatusId],[AccountStatusCode],[AccountStatusName],[AccountTypecode]) values (15,'6','NO DEBIT ALLOWED','D')
insert into [kkslm_ms_account_status]([AccountStatusId],[AccountStatusCode],[AccountStatusName],[AccountTypecode]) values (16,'7','FROZEN','D')
insert into [kkslm_ms_account_status]([AccountStatusId],[AccountStatusCode],[AccountStatusName],[AccountTypecode]) values (17,'8','CHARGE OFF','D')
insert into [kkslm_ms_account_status]([AccountStatusId],[AccountStatusCode],[AccountStatusName],[AccountTypecode]) values (18,'9','DORMANT','D')
insert into [kkslm_ms_account_status]([AccountStatusId],[AccountStatusCode],[AccountStatusName],[AccountTypecode]) values (19,'1','ACTIVE','X')
insert into [kkslm_ms_account_status]([AccountStatusId],[AccountStatusCode],[AccountStatusName],[AccountTypecode]) values (20,'2','CLOSED','X')
insert into [kkslm_ms_account_status]([AccountStatusId],[AccountStatusCode],[AccountStatusName],[AccountTypecode]) values (21,'3','MATURED NOT REDEEM','X')
insert into [kkslm_ms_account_status]([AccountStatusId],[AccountStatusCode],[AccountStatusName],[AccountTypecode]) values (22,'4','NEW TODAY','X')
insert into [kkslm_ms_account_status]([AccountStatusId],[AccountStatusCode],[AccountStatusName],[AccountTypecode]) values (23,'5','DO NOT CLOSE ON ZERO','X')
insert into [kkslm_ms_account_status]([AccountStatusId],[AccountStatusCode],[AccountStatusName],[AccountTypecode]) values (24,'6','NO DEBIT ALLOWED','X')
insert into [kkslm_ms_account_status]([AccountStatusId],[AccountStatusCode],[AccountStatusName],[AccountTypecode]) values (25,'7','FROZEN','X')
insert into [kkslm_ms_account_status]([AccountStatusId],[AccountStatusCode],[AccountStatusName],[AccountTypecode]) values (26,'8','CHARGE OFF','X')
insert into [kkslm_ms_account_status]([AccountStatusId],[AccountStatusCode],[AccountStatusName],[AccountTypecode]) values (27,'9','DORMANT','X')
insert into [kkslm_ms_account_status]([AccountStatusId],[AccountStatusCode],[AccountStatusName],[AccountTypecode]) values (28,'1','ACTIVE','T')
insert into [kkslm_ms_account_status]([AccountStatusId],[AccountStatusCode],[AccountStatusName],[AccountTypecode]) values (29,'2','CLOSED','T')
insert into [kkslm_ms_account_status]([AccountStatusId],[AccountStatusCode],[AccountStatusName],[AccountTypecode]) values (30,'3','MATURED NOT REDEEM','T')
insert into [kkslm_ms_account_status]([AccountStatusId],[AccountStatusCode],[AccountStatusName],[AccountTypecode]) values (31,'4','NEW TODAY','T')
insert into [kkslm_ms_account_status]([AccountStatusId],[AccountStatusCode],[AccountStatusName],[AccountTypecode]) values (32,'5','DO NOT CLOSE ON ZERO','T')
insert into [kkslm_ms_account_status]([AccountStatusId],[AccountStatusCode],[AccountStatusName],[AccountTypecode]) values (33,'6','NO DEBIT ALLOWED','T')
insert into [kkslm_ms_account_status]([AccountStatusId],[AccountStatusCode],[AccountStatusName],[AccountTypecode]) values (34,'7','FROZEN','T')
insert into [kkslm_ms_account_status]([AccountStatusId],[AccountStatusCode],[AccountStatusName],[AccountTypecode]) values (35,'8','CHARGE OFF','T')
insert into [kkslm_ms_account_status]([AccountStatusId],[AccountStatusCode],[AccountStatusName],[AccountTypecode]) values (36,'9','DORMANT','T')
insert into [kkslm_ms_account_status]([AccountStatusId],[AccountStatusCode],[AccountStatusName],[AccountTypecode]) values (37,'0','In progress','L')
insert into [kkslm_ms_account_status]([AccountStatusId],[AccountStatusCode],[AccountStatusName],[AccountTypecode]) values (38,'1','Active','L')
insert into [kkslm_ms_account_status]([AccountStatusId],[AccountStatusCode],[AccountStatusName],[AccountTypecode]) values (39,'2','Paid-off / Closed','L')
insert into [kkslm_ms_account_status]([AccountStatusId],[AccountStatusCode],[AccountStatusName],[AccountTypecode]) values (40,'3','Matured','L')
insert into [kkslm_ms_account_status]([AccountStatusId],[AccountStatusCode],[AccountStatusName],[AccountTypecode]) values (41,'4','New','L')
insert into [kkslm_ms_account_status]([AccountStatusId],[AccountStatusCode],[AccountStatusName],[AccountTypecode]) values (42,'5','Zero accrual','L')
insert into [kkslm_ms_account_status]([AccountStatusId],[AccountStatusCode],[AccountStatusName],[AccountTypecode]) values (43,'6','Freeze with accrual','L')
insert into [kkslm_ms_account_status]([AccountStatusId],[AccountStatusCode],[AccountStatusName],[AccountTypecode]) values (44,'7','Freeze with zero accrual','L')

GO
set identity_insert [kkslm_ms_account_status] off;


