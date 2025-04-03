using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Forms;
using STL.Common.Collections;

namespace STL.PL.Collections.CollectionsClasses
{
   class Validation : CommonForm
   {
      internal ErrorProvider Validator()
      {
         ErrorProvider er = null;
         return er;
      }

      internal bool Valid(string text)
      {
         bool valid = true;
         if (IsNumeric(text) && text.Trim() != "")
         {
            valid = true;
         }
         else
         {
            valid = false;
         }

         return valid;
      }

      internal bool ValidateOperators(string operators)
      {
         bool valid = true;
         if (operators == DropDownValues.Operator || operators == "")
         {
            valid = false;
         }
         else
         {
            valid = true;
         }

         return valid;
      }

      internal bool ValidateExitStrategies(string exit)
      {
         bool valid = true;
         if (exit == DropDownValues.ExitStrategy || exit == "")
         {
            valid = false;
         }
         else
         {
            valid = true;
         }

         return valid;
      }

      internal bool ValidatePreviousStrategies(string exit)
      {
         bool valid = true;
         if (exit == DropDownValues.PreviousStrategy || exit == "")
         {
            valid = false;
         }
         else
         {
            valid = true;
         }

         return valid;
      }
   }
}
