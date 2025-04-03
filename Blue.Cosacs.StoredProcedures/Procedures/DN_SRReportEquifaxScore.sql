SET QUOTED_IDENTIFIER OFF 
GO
SET ANSI_NULLS ON 
GO

if exists (select * from dbo.sysobjects where id = object_id('[dbo].[DN_SRReportEquifaxScore]') and OBJECTPROPERTY(id, 'IsProcedure') = 1)
drop procedure [dbo].[DN_SRReportEquifaxScore]
GO

CREATE PROCEDURE [dbo].[DN_SRReportEquifaxScore]
 
	@Scorecard nvarchar(1) ='A', 
	@Datefrom datetime =null,
	@DateTo datetime =null 
	
AS

SET NOCOUNT OFF
declare @Country varchar(500)
if(@Datefrom =null and @DateTo=null )
begin
set @Datefrom =getdate()-30
set @DateTo= getdate()
 end


set @Country= (select CountryName from Country )

select 
P.ScoreCard as [Applied Score Card]
,p.Points as [Score]
, Case when (E.Scorecard ='C') then 'EquifaxApplicant'Else 'EquifaxBehavioural'  end as  [Equifax Score Card]	
,Cast(E.Score	as Float) as  [Equifax Score Value] 
,E.Custid as  [Customer ID] 	
,C.Name + ' ' + C.FirstName  as  [Customer Name] 
,E.Accountno  as  [Account Number]
,CONCAT(AU.FIRSTNAME,' ',AU.Lastname) as CSR
,E.Scoredate  as  [Date Scored] 
,case (P.propresult)
When 'X' Then 'Reject'
When 'R' Then 'Referral'
When 'D' Then 'Delete'
When 'A' Then 'Approve'
ELSE ''
End  as [Score Decision]
,lag(CH.points) over (partition by CH.custid,CH.acctno order by CH.datechange ) as [Previous Score]
,lag(CH.Datechange) over (partition by CH.custid,CH.acctno order by CH.datechange ) as [Previous Score Date]
-------------------------------------------------------------------------------------------------------------
,Cast(E.rl_Age  as Float) as  [Age]--- TT A,TT B---BB A,BB B,  doene
,Cast(E.Age  as Float) as  [Age Result]--- TT A,TT B---BB A,BB B, done
,cd2.Codedescript AS [EmploymentStatus_Woe]	---BB A, ,BZ B done
,Cast(E.EmploymentStatus_WOE as Float) as  [EmploymentStatus_WOE Result]---BB A, ,BZ B done
,CASE E.Scorecard
 WHEN 'C' THEN 'New'
 WHEN 'D' THEN 'Existing'
 ELSE ''
 End  AS [Flag_Customerstatus_His_Woe]  ---- Done to All Country
,Cast(E.Flag_CustomerStatus_His_WOE   as Float) as  [Flag_CustomerStatus_His_WOE Result]--- TT A---BB A,-3---BZ,A done
,CASE C.SEX 
 WHEN 'M' THEN 'Male'
 ELSE 'Female'
 End  AS [Gender_Woe] 	---BB A, done 
,Cast(E.Gender_WOE as Float) as  [Gender_WOE Result]---BB A, done  
,cd1.Codedescript as  [Maritalstatus_Woe]---TT A,TT B-2--BB A,,BB B,-3---BZ,A ,BZ B  done 
,Cast(E.MaritalStatus_WOE as Float) as  [MaritalStatus_WOE Result]---TT A,TT B-2--BB A,,BB B,-3---BZ,A ,BZ B done 
,Cast(E.rl_MobileNumber_WOE as Float) as  [MobileNumber_WOE]---TT A done 
,Cast(E.MobileNumber_WOE as Float) as  [MobileNumber_WOE Result]---TT A done 
,Cast(E.rl_NumberDependents as Float) as  [NumberDependents]---BB A,,BB B, done 
,Cast(E.NumberDependents as Float) as  [NumberDependents Result]---BB A,,BB B, done 
,cd.Codedescript AS [Occupation_Woe]	---TT A,TT B---BB A,,BB B,-3---BZ,A ,BZ B  done 
,Cast(E.Occupation_WOE as Float) as  [Occupation_WOE Result]---TT A,TT B---BB A,BB B,-3---BZ,A ,BZ B done 
,Cast(E.PostCode_WOE  as Float) as  [PostCode_WOE]---BB A,,BB B, done 
,Cast(E.rl_PostCode_WOE as Float) as  [PostCode_WOE Result]---BB A,,BB B, done 
,Cast(E.rl_Ratio_Ndependent_to_age as Float) as  [Ratio_Ndependent_to_age]---TT A done 
,Cast(E.Ratio_Ndependent_to_age as Float) as  [Ratio_Ndependent_to_age Result]---TT A done 
,Cast(E.rl_Ratio_TCurrentEmploy_to_age as Float) as  [Ratio_TCurrentEmploy_to_age]---TT A---BB A, done 
,Cast(E.Ratio_TCurrentEmploy_to_age  as Float) as  [Ratio_TCurrentEmploy_to_age Result]---TT A---BB A, done 
,Cast(E.rl_ResidentialStatus_WOE as Float) as  [ResidentialStatus_WOE]---TT A,TT B-----BZ,A ,BZ B done 
,Cast(E.ResidentialStatus_WOE as Float) as  [ResidentialStatus_WOE Result]---TT A,TT B -----BZ,A ,BZ B done 
,Cast(E.rl_TimeCurrentAddress as Float) as  [TimeCurrentAddress]------BB A,  done 
,Cast(E.TimeCurrentAddress as Float) as  [TimeCurrentAddress Result]------BB A, done 
,Cast(E.rl_TimeCurrentEmployment as Float) as  [TimeCurrentEmployment]--- ,BZ B  done
,Cast(E.TimeCurrentEmployment as Float) as  [TimeCurrentEmployment Result]--- ,BZ B done 
---------
,Cast(E.rl_Avg_Agreement_Total_1m_sq as Float) as  [Avg_Agreement_Total_1m_sq]--- ,BZ B done 
,Cast(E.Avg_Agreement_Total_1m_sq as Float) as  [Avg_Agreement_Total_1m_sq Result]--- ,BZ B done 
,Cast(E.rl_Avg_Balance_Arrears_1m_ln as Float) as  [Avg_Balance_Arrears_1m_ln]--- ,BZ B done 
,Cast(E.Avg_Balance_Arrears_1m_ln as Float) as  [Avg_Balance_Arrears_1m_ln Result]--- ,BZ B done 
,Cast(E.rl_Avg_Balance_Arrears_12m_ln as Float) as  [Avg_Balance_Arrears_12m_ln]--- ,BZ B done 
,Cast(E.Avg_Balance_Arrears_12m_ln as Float) as  [Avg_Balance_Arrears_12m_ln Result]--- ,BZ B done 
,Cast(E.rl_BalanceArrears_Pound_6M as Float) as  [BalanceArrears_Pound_6M]---,BB B, done 
,Cast(E.BalanceArrears_Pound_6M as Float) as  [BalanceArrears_Pound_6M Result]---,BB B, done 
,Cast(E.rl_BalanceArrears_Pound_6M_ln as Float) as  [BalanceArrears_Pound_6M_ln]---,TT B  done 
,Cast(E.BalanceArrears_Pound_6M_ln as Float) as  [BalanceArrears_Pound_6M_ln Result]---,TT B done 
,Cast(E.rl_Count_DaysArrear_30more_17m_ln as Float) as  [Count_DaysArrear_30more_17m_ln]---,BB B,  done 
,Cast(E.Count_DaysArrear_30more_17m_ln as Float) as  [Count_DaysArrear_30more_17m_ln Result]---,BB B, done 
,Cast(E.rl_Count_DaysArrear_60more_17m_ln as Float) as  [Count_DaysArrear_60more_17m_ln]---,TT B done 
,Cast(E.Count_DaysArrear_60more_17m_ln as Float) as  [Count_DaysArrear_60more_17m_ln Result]---,TT B done 
,Cast(E.rl_DaysArrears_Pound_6M as Float) as  [DaysArrears_Pound_6M]---,BZ B   done 
,Cast(E.DaysArrears_Pound_6M as Float) as  [DaysArrears_Pound_6M Result]---,BZ B done 
,Cast(E.rl_Max_Perc_Outs_3m_sq as Float) as  [Max_Perc_Outs_3m_sq]---,TT B, BZ B  done 
,Cast(E.Max_Perc_Outs_3m_sq as Float) as  [Max_Perc_Outs_3m_sq Result]---,TT B, BZ B done 
,Cast(E.rl_Max_Perc_OutsArrears_6m_ln as Float) as  [Max_Perc_OutsArrears_6m_ln]---,BB B,  done 
,Cast(E.Max_Perc_OutsArrears_6m_ln as Float) as  [Max_Perc_OutsArrears_6m_ln Result]---,BB B, done 
,Cast(E.rl_Newest_Credit_sq as Float) as  [Newest_Credit_sq]---,TT B ,BZ B  done 
,Cast(E.Newest_Credit_sq as Float) as  [Newest_Credit_sq Result]---,TT B ,BZ B done 
,Cast(E.rl_Number_Account_17m as Float) as  [Number_Account_17m]--- ,BZ B done 
,Cast(E.Number_Account_17m as Float) as  [Number_Account_17m Result]--- ,BZ B done 
,Cast(E.rl_Number_Account_Opened_3m  as Float) as  [Number_Account_Opened_3m]---,TT B done 
,Cast(E.Number_Account_Opened_3m as Float) as  [Number_Account_Opened_3m Result]---,TT B done 
,Cast(E.rl_Number_Account_Opened_3m_cr  as Float) as  [Number_Account_Opened_3m_cr]--- ,BZ B done 
,Cast(E.Number_Account_Opened_3m_cr as Float) as  [Number_Account_Opened_3m_cr Result]--- ,BZ B done 
,Cast(E.rl_NumberDependents_cr as Float) as  [NumberDependents_cr]  --- ,BZ B   done 
,Cast(E.NumberDependents_cr as Float) as  [NumberDependents_cr Result]  --- ,BZ B done 
,Cast(E.rl_NumberDependents_sq as Float) as  [NumberDependents_sq] ---,TT B done 
,Cast(E.NumberDependents_sq as Float) as  [NumberDependents_sq Result] ---,TT B done 

,Cast(E.rl_Oldest_Credit_ln  as Float) as  [Oldest_Credit_ln]---,TT B,--BB B, ,BZ B done 
,Cast(E.Oldest_Credit_ln as Float) as  [Oldest_Credit_ln Result]---,TT B,--BB B, ,BZ B done 
,Cast(E.rl_TimeCurrentAddress_ln  as Float) as  [TimeCurrentAddress_ln]---,BB B,----BZ,A ,BZ B  Not Done
,Cast(E.TimeCurrentAddress_ln as Float) as  [TimeCurrentAddress_ln Result]---,BB B,----BZ,A ,BZ B Not Done 
,Cast(E.rl_TimeCurrentEmployment_ln  as Float) as  [TimeCurrentEmployment_ln]---,BB B,  done 
,Cast(E.TimeCurrentEmployment_ln as Float) as  [TimeCurrentEmployment_ln Result]---,BB B, done 
,Cast(E.rl_TimeCurrentEmployment_sr  as Float) as  [TimeCurrentEmployment_sr]---,TT B-----BZ,A done 
,Cast(E.TimeCurrentEmployment_sr as Float) as  [TimeCurrentEmployment_sr Result]---,TT B-----BZ,A done 
,@Country as CountryName
from equifaxscore E
inner join Proposal P
on E.Custid=P.Custid and E.AccountNo=P.Acctno
inner join Customer C
on E.Custid=C.Custid
inner join CustomerScoreHist CH
ON P.Custid=CH.Custid and P.acctno=ch.acctno
INNER JOIN [ADMIN].[USER] AU
ON CH.empeeno =AU.ID
LEFT join  code cd on p.OCCUPATION=CD.code and cd.Category in ('WT1')
LEFT join  code cd1 on c.Maritalstat=Cd1.code and cd1.Category in ('MS1')
Left join custaddress CA ON C.Custid=ca.custid and ca.Addtype='H'
left join  code cd2 on p.EmploymentStatus =cd2.code and cd2.Category in ('ES1')
where CH.datechange>@Datefrom AND CH.datechange <@DateTo and E.Scorecard= @Scorecard
 