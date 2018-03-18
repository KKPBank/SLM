  alter table kkslm_ms_cardtype
  add slm_CIFSubscriptTypeId varchar(100)
  go



  update kkslm_ms_cardtype
  set slm_CIFSubscriptTypeId='001'
  where slm_CardTypeId=1;


  update kkslm_ms_cardtype
  set slm_CIFSubscriptTypeId='004'
  where slm_CardTypeId=2;


  update kkslm_ms_cardtype
  set slm_CIFSubscriptTypeId='002'
  where slm_CardTypeId=3;