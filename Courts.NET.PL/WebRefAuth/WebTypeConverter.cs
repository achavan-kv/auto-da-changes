using System;
using System.Collections.Generic;
using System.Text;

namespace STL.PL
{
    public static class WebTypeConverter
    {
        //IP - 8/12/10 - Store Card
        //public static Blue.Cosacs.Shared.StoreCardBranchQualRules GetStoreCardBranchQualRulesBlue(STL.PL.WSStoreCard.StoreCardBranchQualRules rules)
        //{
        //    return new Blue.Cosacs.Shared.StoreCardBranchQualRules
        //    {
        //        BranchNo = rules.BranchNo,
        //        MinApplicationScore = rules.MinApplicationScore,
        //        MinBehaviouralScore = rules.MinBehaviouralScore,
        //        MinMthsAcctHist = rules.MinMthsAcctHist,
        //        MaxCurrMthsInArrs = rules.MaxCurrMthsInArrs,
        //        MaxPrevMthsInArrs = rules.MaxPrevMthsInArrs,
        //        MinAvailRFLimit = rules.MinAvailRFLimit,
        //        EmpeenoChange = rules.EmpeenoChange,
        //        ApplyTo = rules.
                

                
               
        //    };
        //}

        //IP - 8/12/10 - Store Card
        public static STL.PL.WS2.StoreCardBranchQualRules GetStoreCardBranchQualRulesWS2(Blue.Cosacs.Shared.StoreCardBranchQualRules rules)
        {
            if (rules == null) //IP - 10/03/11 - #3305 - Only convert if object is not null
                return null;
            else
            {
                return new STL.PL.WS2.StoreCardBranchQualRules
                {
                    BranchNo = rules.BranchNo,
                    MinApplicationScore = rules.MinApplicationScore,
                    MinBehaviouralScore = rules.MinBehaviouralScore,
                    MinMthsAcctHistX = rules.MinMthsAcctHistX,                  //IP - 21/04/11 - Feature 3000 - Renamed
                    MinMthsAcctHistY = rules.MinMthsAcctHistY,                  //IP - 21/04/11 - Feature 3000 
                    MaxCurrMthsInArrs = rules.MaxCurrMthsInArrs,
                    MaxPrevMthsInArrsX = rules.MaxPrevMthsInArrsX,              //IP - 21/04/11 - Feature 3000 - Renamed
                    MaxPrevMthsInArrsY = rules.MaxPrevMthsInArrsY,              //IP - 21/04/11 - Feature 3000
                    PcentInitRFLimit = rules.PcentInitRFLimit,                  //IP - 21/04/11 - Feature 3000 - Renamed
                    MaxNoCustForApproval = rules.MaxNoCustForApproval,          //IP - 10/05/11 - Feature 3593
                    EmpeenoChange = rules.EmpeenoChange,
                    ApplyTo = rules.ApplyTo
                };
            }
            
        }
    }
}
