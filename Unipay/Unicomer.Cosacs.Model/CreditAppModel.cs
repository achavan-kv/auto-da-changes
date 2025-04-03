using System;
using System.Collections.Generic;

namespace Unicomer.Cosacs.Model
{
    public class CreditAppQuestions
    {
        public List<CrAppQuestions> questionsAndAnswers { get; set; }
        //public List<MasterList> TitleList { get; set; }
        //public List<MasterList> AddressTypeList { get; set; }
        //public List<MasterList> DeliveryAreaList { get; set; }
        //public List<MasterList> OccupationList { get; set; }
        //public List<MasterList> PayFrequencyList { get; set; }
        //public List<MasterList> EmployeeStatusList { get; set; }
        //public List<MasterList> MaritalStatusList { get; set; }
        //public List<MasterList> AccountTypeList { get; set; }
        //public List<MasterList> NationalityList { get; set; }
        //public int NumCorrectRequired { get; set; }
    }

    public class CreditAppQuestion
    {
        public string Message { get; set; }
        public CreditAppQuestions CreditAppQuestions { get; set; }
    }

    public class CrAppQuestions
    {
        public string questionTitle { get; set; }
        public int qId { get; set; }
        public string question { get; set; }
        public string inputType { get; set; }
        public string inputCategory { get; set; }
        public OptionsList options { get; set; }
        public string[] answer { get; set; }
        public bool mandatory { get; set; }
        public Constraints constraints { get; set; }
        public DependsOn dependsOn { get; set; }
    }

    public class ConstraintObj
    {
        public int qId { get; set; }
        public Constraints constraintObj { get; set; }
    }


    public class Constraints
    {
        public Int64 max { get; set; }
        public Int64 min { get; set; }
        public string regex { get; set; }
        public CustomErrorMessage customErrorMessages { get; set; }
    }

    public class DependsOnObj
    {
        public int dId { get; set; }
        public int qId { get; set; }
        public DependsOn dependsOn { get; set; }
    }

    public class DependsOn
    {
        public string catalog { get; set; }
        public List<Question> questions { get; set; }

    }

    public class QuestionObj
    {
        public int dId { get; set; }
        public Question question { get; set; }
    }

    public class Question
    {
        public string qId { get; set; }
        public string filterName { get; set; }
    }

    public class CustomErrorMessage
    {
        public string max { get; set; }
        public string min { get; set; }
        public string regex { get; set; }
    }

    public class OptionsList
    {
        public List<OptionModel> option { get; set; }
    }

    public class OptionModel
    {
        public string id { get; set; }
        public string text { get; set; }
    }

    public class CodeList
    {
        public string Code { get; set; }
        public string Script { get; set; }
        public string Category { get; set; }
    }
    
    //public class EmployeeStatus
    //{
    //    public string Code { get; set; }
    //    public string Script { get; set; }
    //}

    //public class PayFrequency
    //{
    //    public string Code { get; set; }
    //    public string Script { get; set; }
    //}

    //public class Occupation
    //{
    //    public string Code { get; set; }
    //    public string Script { get; set; }
    //}

    //public class DeliveryArea
    //{
    //    public string Code { get; set; }
    //    public string Script { get; set; }
    //}

    //public class Title
    //{
    //    public string Code { get; set; }
    //    public string Script { get; set; }
    //}
    //public class AddressType
    //{
    //    public string Code { get; set; }
    //    public string Script { get; set; }

    //}
}
