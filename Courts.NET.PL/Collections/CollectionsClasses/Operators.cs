using System;
using System.Collections.Generic;
using System.Collections;
using System.Text;
using System.Reflection;

namespace STL.PL.Collections.CollectionsClasses
{
   class Operators : System.Attribute
   {
      private string _value;
      private static Hashtable _stringValues;
      private string _sms;

      public string SMS
      {
         get { return _sms; }
         set { _sms = value; }
      }

      public Operators()
      {
        
         SMS = GetStringValue(Action.SendSMS);
      }

      public Operators(string value)
      {
         _value = value;
         
      }

      public string Value
      {
         get { return _value; }
      }

      private enum Action
      {
         [Operators("Send SMS")]
         SendSMS = 1,
         [Operators("Send Letter")]
         SendLetter = 2,
         [Operators("Send to Worklist")]
         SendToWorklist = 3,
         [Operators("Send to Strategy")]
         SendToStrategy = 4
      }

      private static string GetStringValue(Enum value)
      {
         string output = null;
         Type type = value.GetType();

         //Check first in cached results
         if (_stringValues.ContainsKey(value))
         {
            output = (_stringValues[value] as Operators).Value;
         }
         else
         {
            //Look for 'Operators' 
            //in the field's custom attributes
            FieldInfo fi = type.GetField(value.ToString());
            Operators[] attrs = fi.GetCustomAttributes(typeof(Operators),false) as Operators[];
            if (attrs.Length > 0)
            {
               _stringValues.Add(value, attrs[0]);
               output = attrs[0].Value;
            }
         }

         return output;

      }

   }
}
