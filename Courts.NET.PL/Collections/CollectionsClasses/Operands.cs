using System;
using System.Collections.Generic;
using System.Text;
using STL.Common.Collections;

namespace STL.PL.Collections.CollectionsClasses
{
   class Operands
   {
      /// <summary>
      /// Returns the string to be displayed in the operands ComboBox
      /// </summary>
      /// <param name="operand"></param>
      /// <returns>displayedOperand</returns>
      internal string GetDisplayedOperand(string operand)
      {
         string displayedOperand = String.Empty;

         switch (operand)
         {
            case (">"):
               displayedOperand = Operators.GreaterThan;
               break;
            case ("<"):
               displayedOperand = Operators.LessThan;
               break;
            case ("="):
               displayedOperand = Operators.Equal;
               break;
            case (">="):
               displayedOperand = Operators.GreaterThanEqualTo;
               break;
            case ("<="):
               displayedOperand = Operators.LessThanEqualTo;
               break;
            case ("<>"):
               displayedOperand = Operators.NotEqualTo;
               break;
            case ("Between"):
               displayedOperand = Operators.Between;
               break;
         }

         return displayedOperand;

      }

      internal string GetOperandSign(string operand)
      {
         string sign = String.Empty;

         switch(operand)
         {
            case(Operators.GreaterThan):
               sign = OperatorSigns.GreaterThan;
               break;
            case(Operators.LessThan):
               sign = OperatorSigns.LessThan;
               break;
            case (Operators.Equal):
               sign = OperatorSigns.Equal;
               break;
            case (Operators.GreaterThanEqualTo):
               sign = OperatorSigns.GreaterThanEqualTo;
               break;
            case (Operators.LessThanEqualTo):
               sign =OperatorSigns.LessThanEqualTo;
               break;
            case (Operators.NotEqualTo):
               sign = OperatorSigns.NotEqualTo;
               break;
            case (Operators.Between):
               sign = OperatorSigns.Between;
               break;
         }

         return sign;
      }
   }
}
