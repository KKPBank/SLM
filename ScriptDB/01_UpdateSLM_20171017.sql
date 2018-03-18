
CREATE TABLE [dbo].[kkslm_ext_sys_status_cbs_file](
	[kkslm_ext_sys_status_cbs_file_id] [int] IDENTITY(1,1) NOT NULL,
	[kkslm_filename] [varchar](1000) NOT NULL,
	[kkslm_file_created_date] [datetime] NOT NULL,
	[kkslm_file_process_time] [datetime] NOT NULL,
	[kkslm_process_status] [varchar](50) NOT NULL,
	[kkslm_process_error_step] [varchar](1000) NULL,
 CONSTRAINT [PK_kkslm_ext_sys_status_cbs_file] PRIMARY KEY CLUSTERED 
(
	[kkslm_ext_sys_status_cbs_file_id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO



CREATE UNIQUE NONCLUSTERED INDEX [NonClusteredIndex-20171020-093314] ON [dbo].[kkslm_ext_sys_status_cbs_file]
(
	[kkslm_filename] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, IGNORE_DUP_KEY = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON)

GO



CREATE TABLE [dbo].[kkslm_ext_sys_status_cbs_file_data](
	[kkslm_ext_sys_status_cbs_file_data_id] [int] IDENTITY(1,1) NOT NULL,
	[kkslm_ext_sys_status_cbs_file_id] [int] NOT NULL,
	[kkslm_reference_code] [varchar](50) NOT NULL,
	[kkslm_file_name] [varchar](100) NOT NULL,
	[kkslm_create_date] [varchar](50) NOT NULL,
	[kkslm_current_sequence] [int] NOT NULL,
	[kkslm_total_sequence] [int] NOT NULL,
	[kkslm_total_record] [int] NOT NULL,
	[kkslm_system_code] [varchar](100) NOT NULL,
	[kkslm_reference_no] [varchar](50) NOT NULL,
	[kkslm_channel_id] [varchar](50) NOT NULL,
	[kkslm_status_date_time] [varchar](50) NOT NULL,
	[kkslm_subscription_id] [varchar](100) NULL,
	[kkslm_subscription_cus_type] [varchar](50) NULL,
	[kkslm_subscription_card_type] [varchar](50) NULL,
	[kkslm_owner_system_id] [varchar](100) NOT NULL,
	[kkslm_owner_system_code] [varchar](100) NOT NULL,
	[kkslm_ref_system_id] [varchar](100) NULL,
	[kkslm_ref_system_code] [varchar](100) NULL,
	[kkslm_status] [varchar](50) NOT NULL,
	[kkslm_status_name] [varchar](100) NOT NULL,
 CONSTRAINT [PK_kkslm_ext_sys_status_cbs_file_data] PRIMARY KEY CLUSTERED 
(
	[kkslm_ext_sys_status_cbs_file_data_id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO

--ก่อนจะ Grant สิทธิ์ ให้ตรวจสอบชื่อ DB ก่อนนะ
grant select,insert,update,delete on kkslm_ext_sys_status_cbs_file to SLM;
grant select,insert,update,delete on kkslm_ext_sys_status_cbs_file_data to SLM;


insert into kkslm_ms_batch( slm_BatchCode,slm_BatchName, slm_Seq,
slm_BatchDescription, slm_type, is_Deleted, slm_system, slm_IsReRun)
values('SLM_PRO_08','Batch CAR Insert Status',8,
'Batch CAR Insert Status','EOD',0,'SLM',0);