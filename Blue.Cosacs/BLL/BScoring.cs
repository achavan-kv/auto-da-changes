using System;
using STL.Common;
using System.Data;
using STL.DAL;
using System.Xml;
using STL.Common.Constants.ColumnNames;
using System.Data.SqlClient;
using STL.Common.Constants.TableNames;
using STL.Common.Constants.Tags;

namespace STL.BLL
{
    /// <summary>
    /// Summary description for BScoring.
    /// </summary>
    public class BScoring : CommonObject
    {
        public DataSet GetOperands()
        {
            DScoring score = new DScoring();
            DataSet ds = score.GetOperands();
            return ds;
        }


        public XmlNode GetRules(string country, char scoretype, string region)
         {
            XmlDocument doc = new XmlDocument();
            DScoring score = new DScoring();
            score.Country = country;
            score.Region = region;
            score.scoretype = scoretype; // Pass new Scoretype parameter. SC CR1034 Behavioural Scoring 15/02/2010
            
              score.GetRules();
            foreach (DataRow r in score.RulesTable.Rows)
            {
                doc.LoadXml((string)r[CN.RulesXML]);
            }
            return doc.DocumentElement;
        }

        public DataTable GetRulesTable(string country, string region, char scoretype)
        {
            DScoring score = new DScoring();
            score.Country = country;
            score.Region = region;
            score.scoretype = scoretype;
            score.GetRules();
            return score.RulesTable;
        }

        public void SaveRules(SqlConnection conn, SqlTransaction trans,
                                string country,
                                XmlNode rules,
                                string region)
        {
            DScoring score = new DScoring();
            score.Country = country;
            score.Region = region;
            score.RulesXML = rules.OuterXml;
            score.ImportedBy = this.User;
            score.DateImported = DateTime.Now;
            score.NewImport = true; // prevent deletion of incorrect scoring rules. 
            try
            {
                //    score.NewImport = Convert.ToBoolean(rules.Attributes[Tags.NewImport].Value);
                score.FileName = rules.Attributes[Tags.FileName].Value;
            }
            catch (NullReferenceException)
            {

            }

            int acceptScore = Convert.ToInt32(rules.Attributes[Tags.DeclineScore].Value);
            int referScore = Convert.ToInt32(rules.Attributes[Tags.ReferScore].Value);

            score.SaveRules(conn, trans);
            score.SaveAcceptReferScore(conn, trans, acceptScore, referScore);
        }

        public DataSet GetMatrix(string countryCode, char scoretype, string region)
 
        {
            DataSet ds = new DataSet();
            DScoring score = new DScoring();
            score.Country = countryCode;
            score.Region = region;
            score.scoretype = scoretype; //CR1034 SC 16/12/10
            score.GetMatrix();
            ds.Tables.Add(score.Matrix);
            return ds;
        }



        public DataSet GetTermsTypeMatrix(string countryCode, char scoretype)
           {
            DataSet ds = new DataSet();
            DScoring score = new DScoring();
            score.Country = countryCode;
            score.scoretype = scoretype;// SC CR1034 17-02-10
            score.GetTermsTypeMatrix();
            ds.Tables.Add(score.Matrix);
            return ds;
        }


        public void SaveMatrix(SqlConnection conn, SqlTransaction trans, string fileName, string countryCode, char scoretype, string region, DataSet matrix, bool newImport)
        {
            DateTime dateImported = DateTime.Now;
            DScoring sm = new DScoring();

            if (!newImport)
            {
                sm.DeleteMatrix(conn, trans, countryCode, scoretype, region);
            }

            matrix.AcceptChanges();
            foreach (DataRow r in matrix.Tables[TN.ScoringMatrix].Rows)
            {
                int score = (int)r[CN.Score];
                decimal fLimit = (decimal)r[CN.FurnitureLimit];
                decimal eLimit = (decimal)r[CN.ElectricalLimit];
                decimal income = (decimal)r[CN.Income];
                sm.SaveMatrixRow(conn, trans, fileName, countryCode, scoretype, region, score, fLimit, eLimit, income, User, dateImported);
            }
        }


        public void SaveTermsTypeMatrix(SqlConnection conn, SqlTransaction trans, string fileName, string countryCode, char scoretype, DataSet matrix, bool newImport)
        {
            DScoring sm = new DScoring();
            DateTime dateImported = DateTime.Now;

            if (!newImport)
            {
                sm.DeleteTermsTypeMatrix(conn, trans, countryCode,scoretype);
            }

            matrix.AcceptChanges();
            foreach (DataRow r in matrix.Tables[TN.TermsTypeMatrix].Rows)
            {
                string band = Convert.ToString(r[CN.Band]);
                int pointsFrom = Convert.ToInt32(r[CN.PointsFrom]);
                int pointsTo = Convert.ToInt32(r[CN.PointsTo]);
                decimal serviceCharge = Convert.ToDecimal(r[CN.ServiceChargePC]);

                sm.SaveTermsTypeMatrixRow(conn, trans,
                    countryCode,
                    scoretype,
                    band,
                    pointsFrom,
                    pointsTo,
                    serviceCharge,
                    dateImported,
                    User,
                    fileName);
            }
        }


        public void ApplyTermsTypeMatrix(SqlConnection conn, SqlTransaction trans, DateTime startDate, char scorecard)
        {
            DScoring sm = new DScoring();
            sm.ApplyTermsTypeMatrix(conn, trans, startDate, User, scorecard);
        }

        public BScoring()
        {

        }
    }
}


