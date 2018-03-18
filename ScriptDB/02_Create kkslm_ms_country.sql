

CREATE TABLE [dbo].[kkslm_ms_country](
	[slm_country_id] [int] IDENTITY(1,1) NOT NULL,
	[slm_CountryCode] [varchar](100) NULL,
	[slm_CountryDescriptionEN] [varchar](1000) NULL,
	[slm_CountryDescriptionTH] [varchar](1000) NULL,
	[is_delete] [numeric](1, 0) NOT NULL,
 CONSTRAINT [PK_kkslm_ms_country] PRIMARY KEY CLUSTERED 
(
	[slm_country_id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO

ALTER TABLE [dbo].[kkslm_ms_country] ADD  CONSTRAINT [DF_kkslm_ms_country_is_delete]  DEFAULT ((0)) FOR [is_delete]
GO


--SLMDB
grant select,insert,update,delete on  [dbo].[kkslm_ms_country] to SLMDB;