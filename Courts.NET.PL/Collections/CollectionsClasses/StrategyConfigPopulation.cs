using System;
using System.Collections.Generic;
using System.Text;
using System.Data;
using System.Data.SqlClient;
using System.Collections;
using System.Drawing;
using STL.Common.Constants.ColumnNames;

namespace STL.PL.Collections.CollectionsClasses
{
   class StrategyConfigPopulation : CommonForm
   {
      private string _error = "";

      /// <summary>
      /// Returns a DataTable containing all the possible conditions for the selected tab
      /// </summary>
      /// <param name="tabSelected"></param>
      /// <returns></returns>
      internal DataTable GetConditions(string tabSelected)
      {
         DataSet conditionsData = new DataSet();
         conditionsData = CollectionsManager.GetConditions(out _error);

         //Filter the data according to which tab has been selected
         
         DataTable dt = new DataTable();
         dt = FilterConditions(tabSelected, conditionsData);
         return dt;
      }

      /// <summary>
      /// Filters the conditions data according to their type
      /// </summary>
      /// <param name="tabSelected"></param>
      /// <param name="ds"></param>
      /// <returns></returns>
      private DataTable FilterConditions(string tabSelected,DataSet ds)
      {
         DataView conditionsView = new DataView();

         conditionsView = ds.Tables[0].DefaultView;

         switch (tabSelected)
         {
            case "tabPageEntryConditions":
               conditionsView.RowFilter = "Type = 'N' OR Type = ''";
               break;
            case "tabPageExitConditions":
               conditionsView.RowFilter = "Type = 'X' OR Type = ''";
               break;
            case "tabPageSteps":
               conditionsView.RowFilter = "Type = 'S' OR Type = '' OR Type IS NULL";
               break;
            default:
               break;
         }

         DataTable conditionsTable = new DataTable();
         conditionsTable = conditionsView.ToTable();

         return conditionsTable;

      }

      /// <summary>
      /// Returns a DataTable containing all the conditions associated with the selected strategy for the selected tab
      /// </summary>
      /// <param name="strategy"></param>
      /// <param name="tabSelected"></param>
      /// <returns></returns>
      internal DataTable GetStrategyConditions(string strategy, string tabSelected)
      {
         DataSet strategyConditionsData = new DataSet();
         strategyConditionsData = CollectionsManager.GetStrategyConditions(strategy, out _error);

         DataTable dt = new DataTable();
         dt = FilterSavedConditions(tabSelected, strategyConditionsData);

         return dt;
      }

      private DataTable FilterSavedConditions(string tabSelected, DataSet strategyConditionsData)
      {
         DataView conditionsView = new DataView();

         conditionsView = strategyConditionsData.Tables[0].DefaultView;

         switch (tabSelected)
         {
            case "tabPageEntryConditions":
               conditionsView.RowFilter = "SavedType = 'N'";
               break;
            case "tabPageExitConditions":
               conditionsView.RowFilter = "SavedType = 'X'";
               break;
            case "tabPageSteps":
               conditionsView.RowFilter = "SavedType = 'S'";
               break;
            default:
               break;
         }

         DataTable conditionsTable = new DataTable();
         conditionsTable = conditionsView.ToTable();

         return conditionsTable;
      }

      /// <summary>
      /// Returns a DataTable containing all the strategies
      /// </summary>
      /// <returns></returns>
      internal DataTable GetStrategies()
      {
         DataSet strategyData = new DataSet();
         strategyData = CollectionsManager.GetStrategies( out _error);

         DataTable dt = new DataTable();
         dt = strategyData.Tables[0];
         return dt;
      }

      //IP - 20/10/08 - UAT5.2 - UAT(551)
      //Return the 'Exit Condition Strategies' for the selected strategy.
      internal DataTable GetStrategiesToSendTo(string strategy)
      {
          DataSet dsStrategiesToSendTo = new DataSet();
          dsStrategiesToSendTo = CollectionsManager.GetStrategiesToSendTo(strategy, out _error);

          DataTable dtStrategiesToSendTo = dsStrategiesToSendTo.Tables[0];
          return dtStrategiesToSendTo;

      }

      /// <summary>
      /// Returns a DataTable containing all the strategies for the Bailiff Review screen
      /// </summary>
      /// <returns></returns>
      internal DataTable GetStrategiesForBailiff()
      {
         DataSet strategyData = new DataSet();
         strategyData = CollectionsManager.GetStrategiesForBailiff(out _error);

         DataTable dt = new DataTable();
         dt = strategyData.Tables[0];
         return dt;
      }

      /// <summary>
      /// Returns a DataTable containing all the actions
      /// </summary>
      /// <returns></returns>
      internal DataTable GetActions()
      {
         DataSet actionsData = new DataSet();
         actionsData = CollectionsManager.GetActions(out _error);

         DataTable dt = new DataTable();
         dt = actionsData.Tables[0];
         return dt;
      }

      /// <summary>
      /// Returns a DataTable containing all the actions associated with the selected strategy
      /// </summary>
      /// <param name="strategy"></param>
      /// <returns></returns>
      internal DataTable GetStrategyActions(string strategy)
      {
         DataSet strategyActions = new DataSet();
         strategyActions = CollectionsManager.GetStrategyActions(strategy, out _error);

         DataTable dt = new DataTable();
         dt = strategyActions.Tables[0];

         return dt;
      }

      /// <summary>
      /// Returns a DataTable containing all the letter codes
      /// </summary>
      /// <returns></returns>
      internal DataTable GetLetters()
      {
         DataSet letterData = new DataSet();
         letterData = CollectionsManager.GetLetters(out _error);

         DataTable dt = new DataTable();
         dt = letterData.Tables[0];
         return dt;
      }

      /// <summary>
      /// Returns a DataTable containing all the SMS codes
      /// </summary>
      /// <returns></returns>
      internal DataTable GetSMS()
      {
         DataSet smsData = new DataSet();
         smsData = CollectionsManager.GetSMS(out _error);

         DataTable dt = new DataTable();
         dt = smsData.Tables[0];
         return dt;
      }

      /// <summary>
      /// Returns a DataTable containing all the worklists
      /// </summary>
      /// <returns></returns>
      internal DataTable GetWorklists()
      {
         DataSet worklistData = new DataSet();
         worklistData = CollectionsManager.GetWorklistDataSet(out _error);

         DataTable dt = new DataTable();
         dt = worklistData.Tables[0];
         return dt;
      }

      /// <summary>
      /// Removes all the conditions associated with a particular strategy from the possible conditions DataTable
      /// </summary>
      /// <param name="ChosenConditions"></param>
      /// <param name="PossibleConditions"></param>
      /// <param name="column"></param>
      /// <returns></returns>
      internal DataTable FilterPossibleConditions(DataTable ChosenConditions, DataTable PossibleConditions,string column)
      {
         ArrayList strategyConditions = new ArrayList();
         int n = ChosenConditions.Rows.Count;

         for (int i = 0; i < n; i++)
         {
            strategyConditions.Add(ChosenConditions.Rows[i][column].ToString());
         }

         int m = PossibleConditions.Rows.Count;
         for (int i = 0; i < m; i++)
         {
            if (strategyConditions.Contains(PossibleConditions.Rows[i][column].ToString()) && (bool)PossibleConditions.Rows[i][CN.AllowReuse] != true)
            {
               DataRow r = PossibleConditions.Rows[i];
               PossibleConditions.Rows.Remove(r);
               m--;
               i--;
            }
         }
         return PossibleConditions;
      }
      
      /// <summary>
      /// Adds a new row to the chosen conditions DataTable and takes the Condition values from the Possible DataTable
      /// </summary>
      /// <param name="ChosenConditions"></param>
      /// <param name="PossibleConditions"></param>
      /// <param name="index"></param>
      /// <param name="n"></param>
      /// <param name="value1"></param>
      /// <param name="value2"></param>
      /// <param name="tab"></param>
      /// <returns>new chosen conditions DataRow or existing possible conditions DataRow</returns>
      internal DataRow AddDataRow(DataTable ChosenConditions, DataTable PossibleConditions, int index, int n, string value1, string value2, string operand, string tab, string exitStrategy,string previousStrategy)
      {
         DataRow row = PossibleConditions.Rows[index];
         DataRow rowChosenEntryConditions = ChosenConditions.NewRow();

         if ((bool)row[CN.AllowReuse] != true)
         {
            rowChosenEntryConditions[CN.Condition] = row[CN.Condition];
         }

         if (row[CN.Condition].ToString().Contains(" X"))
         {
            if ((bool)row[CN.AllowReuse] != true)
            {
               row[CN.Condition] = row[CN.Condition].ToString().Replace(" X", " " + value1);
               rowChosenEntryConditions[CN.Condition] = row[CN.Condition];
            }
            else
            {
               rowChosenEntryConditions[CN.Condition] = row[CN.Condition].ToString().Replace(" X", " " + value1);
            }
         }
         else
         {
            rowChosenEntryConditions[CN.Condition] = row[CN.Condition];
         }

         if (row[CN.Condition].ToString().Contains(" Y") && (bool)row[CN.AllowReuse] != true)
         {
            if ((bool)row[CN.AllowReuse] != true)
            {
               row[CN.Condition] = row[CN.Condition].ToString().Replace(" Y", " " + value2);
               rowChosenEntryConditions[CN.Condition] = row[CN.Condition];
            }
            else
            {
               rowChosenEntryConditions[CN.Condition] = row[CN.Condition].ToString().Replace(" Y", " " + value2);
            }
         }
         else if (!row[CN.Condition].ToString().Contains(" X"))
         {
            rowChosenEntryConditions[CN.Condition] = row[CN.Condition];
         }

         if(row[CN.OperandAllowable].ToString() == "P")
         {
            rowChosenEntryConditions[CN.Condition] = rowChosenEntryConditions[CN.Condition].ToString() + " " + previousStrategy;
         }

         rowChosenEntryConditions[CN.ConditionCode] = row[CN.ConditionCode];
         rowChosenEntryConditions[CN.Operand] = operand;
         rowChosenEntryConditions[CN.Operator1] = value1;
         rowChosenEntryConditions[CN.Operator2] = value2;
         rowChosenEntryConditions[CN.AllowReuse] = row[CN.AllowReuse];
         rowChosenEntryConditions[CN.ActionCode] = exitStrategy;

         //add a value for the step for the new row if this a steps condition
         if (tab == "step")
         {
            int step = ChosenConditions.Rows.Count;
            rowChosenEntryConditions[CN.Step] = step + 1;
            rowChosenEntryConditions[CN.FalseStep] = row[CN.FalseStep];
            rowChosenEntryConditions[CN.SavedType] = "S";
            if (previousStrategy != String.Empty)
            {
               rowChosenEntryConditions[CN.ActionCode] = previousStrategy;
               rowChosenEntryConditions[CN.StepActionType] = "P";
            }
         }
         else if (tab == "entry")
         {
            rowChosenEntryConditions[CN.SavedType] = "N";
             //UAT755
            if (previousStrategy != String.Empty)
            {
                rowChosenEntryConditions[CN.ActionCode] = previousStrategy;
                rowChosenEntryConditions[CN.StepActionType] = "P";
            }
         }
         else if (tab == "exit")
         {
            rowChosenEntryConditions[CN.SavedType]  = "X";
         }

         if (n == 0)
         {
            return rowChosenEntryConditions;
         }
         else
         {
            return row;
         }
      }

      /// <summary>
      /// Adds a new row to the chosen actions DataTable and takes the Action values from the Possible DataTable
      /// </summary>
      /// <param name="ChosenConditions"></param>
      /// <param name="PossibleConditions"></param>
      /// <param name="index"></param>
      /// <param name="n"></param>
      /// <returns>new chosen actions DataRow or existing possible actions DataRow</returns>
      internal DataRow AddDataRow(DataTable ChosenConditions, DataTable PossibleConditions, int index, int n)
      {
         DataRow row = PossibleConditions.Rows[index];
         DataRow rowChosenEntryConditions = ChosenConditions.NewRow();

         rowChosenEntryConditions[CN.Action] = row[CN.Action];
         rowChosenEntryConditions[CN.ActionCode] = row[CN.ActionCode];

         if (n == 0)
         {
            return rowChosenEntryConditions;
         }
         else
         {
            return row;
         }
      }

      /// <summary>
      /// returns new data row when moving step conditions up and down
      /// </summary>
      /// <param name="dt"></param>
      /// <returns></returns>
      internal DataRow GetNewRow(DataTable dtChosenStepConditions,int i)
      {
         DataRow row = dtChosenStepConditions.NewRow();
         row[CN.ConditionCode] = dtChosenStepConditions.Rows[i][CN.ConditionCode];
         row[CN.Condition] = dtChosenStepConditions.Rows[i][CN.Condition];
         row[CN.Operand] = dtChosenStepConditions.Rows[i][CN.Operand];
         row[CN.Operator1] = dtChosenStepConditions.Rows[i][CN.Operator1];
         row[CN.Operator2] = dtChosenStepConditions.Rows[i][CN.Operator2];
         row[CN.AllowReuse] = dtChosenStepConditions.Rows[i][CN.AllowReuse];
         row[CN.ActionCode] = dtChosenStepConditions.Rows[i][CN.ActionCode];
         row[CN.FalseStep] = dtChosenStepConditions.Rows[i][CN.FalseStep];
         row[CN.SavedType] = dtChosenStepConditions.Rows[i][CN.SavedType];
         row[CN.StepActionType] = dtChosenStepConditions.Rows[i][CN.StepActionType];
         row[CN.NextStepTrue] = dtChosenStepConditions.Rows[i][CN.NextStepTrue];
         row[CN.NextStepFalse] = dtChosenStepConditions.Rows[i][CN.NextStepFalse];

         return row;
      }

      /// <summary>
      /// Creates a random colour
      /// </summary>
      /// <returns></returns>
      internal Color RandomColourGenerator(int repeat)
      {
         int i = 0;
         int j = 0;
         int k = 0;
         if(repeat == 0)
         {
         i = new Random().Next(150,255);
         j = new Random().Next(1,150);
         k = new Random().Next(1,150);
         }
         else if (repeat < 20)
         {
            i = new Random().Next(1, repeat * 10 + 50);
            j = new Random().Next(repeat * 10 + 50, 255);
            k = new Random().Next(1, 100);
         }
         else
         {
            i = new Random().Next(1, 150);
            j = new Random().Next(170, 255);
            k = new Random().Next(1, 100);
         }
         Color color = Color.FromArgb(i, j, k);

         return color;
      }

      /// <summary>
      /// Creates a random letter from the alphabet
      /// </summary>
      /// <returns></returns>
      internal string RandomLetterGenerator()
      {
         string letter = "";
         int n = 0;
         n = new Random().Next(65,90);
         letter = Convert.ToChar(n).ToString();

         return letter;
      }

      /// <summary>
      /// Creates an ArrayList that holds all the letters being used for the OrClause
      /// </summary>
      /// <param name="dt"></param>
      /// <returns></returns>
      internal ArrayList CreateLettersArray(DataTable dt)
      {
         ArrayList letters = new ArrayList();
         int numberLetters = dt.Rows.Count;

         for (int i = 0; i < numberLetters; i++)
         {
            if (dt.Rows[i][CN.OrClause].ToString() != "" || dt.Rows[i][CN.OrClause] != DBNull.Value)
            {
               if (!letters.Contains(dt.Rows[i][CN.OrClause].ToString()))
               {
                  letters.Add(dt.Rows[i][CN.OrClause]);
               }
            }
         }

         return letters;
      }

     /// <summary>
      /// Saves all the required strategy condition parameters to the database
     /// </summary>
     /// <param name="strategy"></param>
     /// <param name="description"></param>
     /// <param name="dtStrategy"></param>
     /// <param name="type"></param>
      //IP - 02/06/09 - Credit Collection Walkthrough Changes - Allocations check
      internal void SaveStrategyCondition(string strategy, string description, DataSet dsStrategy, bool canBeAllocated, bool manual, int empeeno)   //UAT987
      {
         //DataSet dsStrategy = new DataSet();
         //dsStrategy.Tables.Add(dtStrategy);
         CollectionsManager.SaveStrategyCondition(strategy, description, dsStrategy, canBeAllocated, manual, empeeno, out _error);  //UAT987
      }

     
      /// <summary>
      /// Saves the active status of the strategy to the database
      /// </summary>
      /// <param name="strategy"></param>
      /// <param name="activeValue"></param>
      internal void SetStrategyActive(string strategy, int activeValue)
      {
         CollectionsManager.SetStrategyActive(strategy, activeValue, out _error);
      }

      // IP - UAT(514) - Delete an existing strategy
      internal void DeleteExistingStrategy(string strategy)
      {
          CollectionsManager.DeleteExistingStrategy(strategy, out _error);
      }
   
       
      
   }
}
