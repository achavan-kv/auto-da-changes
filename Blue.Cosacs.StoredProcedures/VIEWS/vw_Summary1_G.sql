


if exists(select * FROM dbo.sysobjects 
               WHERE ID = object_id('dbo.vw_Summary1_G') AND OBJECTPROPERTY(id, 'IsView') = 1)
	drop view dbo.vw_Summary1_G
go
	
create view dbo.vw_Summary1_G	--WITH SCHEMABINDING			-- removed jec 04/06/07
as
	Select 	s.acctno, --s.outstbalcorr, a.arrears, i.instalamount,
				baldue12 = case
					when isnull(i.instalamount,0)=0 then s.outstbalcorr
					when s.outstbalcorr=0 then 0
					when a.currstatus='S' then s.outstbalcorr		-- jec 13/02/08
					when ((isnull(i.instalamount,0) * 12) + a.arrears)>s.outstbalcorr then s.outstbalcorr
					when ((isnull(i.instalamount,0) * 12) + a.arrears)<0 then (i.instalamount * 12) + a.arrears	-- jec 13/02/08-- more than 12 months in adv
					else ((isnull(i.instalamount,0) * 12) + a.arrears)
				end,
				baldueafter12 = case
					when isnull(i.instalamount,0)=0 then 0
					when s.outstbalcorr=0 then 0
					when a.currstatus='S' then 0			-- jec 13/02/08
					when (s.outstbalcorr - ((isnull(i.instalamount,0) * 12) + a.arrears))<0 then 0
					when ((isnull(i.instalamount,0) * 12) + a.arrears) <0 then 
							s.outstbalcorr	+ ABS(((isnull(i.instalamount,0) * 12) + a.arrears))	-- jec 13/02/08-- more than 12 months in adv
					else (s.outstbalcorr - ((isnull(i.instalamount,0) * 12) + a.arrears))
				end
	From dbo.temp_Summary1_MR s left outer join dbo.instalplan i on s.acctno=i.acctno,dbo.acct a
	Where a.acctno=s.acctno
GO

