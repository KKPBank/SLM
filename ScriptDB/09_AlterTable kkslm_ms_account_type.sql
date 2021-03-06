
  alter table kkslm_ms_account_type
  add AccountType varchar(50)
  go

  alter table kkslm_ms_account_type
  add SystemCode varchar(50)
  go

  truncate table kkslm_ms_account_type
  go

  insert into kkslm_ms_account_type(SystemCode, AccountTypeCode,AccountType,AccountTypeName)
values ('DEP','CUR',	'D',	'CURRENT ACCOUNT'),
('DEP','ISA',	'X',	'SPECIAL FIXED'),
('DEP','SAV',	'S',	'SAVINGS ACCOUNT'),
('FDR','FDR',	'T',	'FIXED DEPOSIT'),
('GLS','GLS',	'G',	'GENERAL LEDGER'),
('LNS','LNS',	'L',	'LOAN'),
('SDB','SDB',	'B',	'SAFE DEPOSIT BOX'),
('TFS','GUA',	'F',	'GUARANTEE'),
('C3X','C3X',null,'Car3x'),
('HP','HP',null,'Hire Purchase'),
('INS','INS',null,'Insurance'),
('ISP','ISP',null,'TRADE FINANCE'),
('LES','LES',null,'Leasing'),
('MUX','MUX',null,'Murex Account'),
('ATM','CUR','D','ATM CURRENT ACCOUNT'),
('ATM','SAV','S','ATM SAVING ACCOUNT')
go

