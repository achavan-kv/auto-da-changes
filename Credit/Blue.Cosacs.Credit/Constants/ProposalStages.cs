using System;
namespace Blue.Cosacs.Credit
{
    public enum ProposalStagesEnum
    {
        None = 0,
        BasicDetailsApp1Details = 1 << 0,
        BasicDetailsApp1Address = 1 << 1,
        BasicDetailsApp2 = 1 << 2,
        Sanction1App1Personal = 1 << 3,

        Sanction1App1Residential = 1 << 4,
        Sanction1App1Employment = 1 << 5,
        Sanction1App1Financial = 1 << 6,
        Sanction1App2Personal = 1 << 7,

        Sanction1App2Residential = 1 << 8,
        Sanction1App2Employment = 1 << 9,
        Sanction1App2Financial = 1 << 10,
        Scored = 1 << 11,

        Sanction2App1Previous = 1 << 12,
        Sanction2App1Landlord = 1 << 13,
        Sanction2App1Employer = 1 << 14,
        Sanction2App1References = 1 << 15,

        Sanction2App2References = 1 << 16,
        DocumentConfirmation = 1 << 17,
        UnderWriter = 1 << 18,
        Referred = 1 << 19,

        Declined = 1 << 20,
        Accepted = 1 << 21
    }

    public class ProposalStageScreen
    {
        // Fill all flags up till stage.
        private enum ProposalStagesCompletedEnum
        {
            None = 0,
            BasicDetailsApp1Details = 1,
            BasicDetailsApp1Address = 3,
            BasicDetailsApp2 = 7,
            Sanction1App1Personal = 15,
            Sanction1App1Residential = 31,
            Sanction1App1Employment = 63,
            Sanction1App1Financial = 127,
            Sanction1App2Personal = 255,
            Sanction1App2Residential = 511,
            Sanction1App2Employment = 1023,
            Sanction1App2Financial = 2047,
            Scored = 4095,
            Sanction2App1Previous = 8191,
            Sanction2App1Landlord = 16383,
            Sanction2App1Employer = 32767,
            Sanction2App1References = 65535,
            Sanction2App2References = 131071,
            DocumentConfirmation = 262143,
            Referred = 524287,
            Declined = 1048575,
            Accepted = 2097151
        }
        private static ProposalStagesEnum basicDetails = ProposalStagesEnum.BasicDetailsApp1Address | ProposalStagesEnum.BasicDetailsApp1Details;
        private static ProposalStagesEnum sanctionStage1App1 = ProposalStagesEnum.Sanction1App1Employment | ProposalStagesEnum.Sanction1App1Financial | ProposalStagesEnum.Sanction1App1Personal | ProposalStagesEnum.Sanction1App1Residential;
        private static ProposalStagesEnum sanctionStage1App2 = ProposalStagesEnum.Sanction1App2Personal | ProposalStagesEnum.Sanction1App2Residential | ProposalStagesEnum.Sanction1App2Employment | ProposalStagesEnum.Sanction1App2Financial;
        private static ProposalStagesEnum sanctionStage2App1 = ProposalStagesEnum.Sanction2App1Previous | ProposalStagesEnum.Sanction2App1Landlord | ProposalStagesEnum.Sanction2App1Employer | ProposalStagesEnum.Sanction2App1References;
        // Yes, I know this is ugly. No, I will not fix it.
        public static Tuple<int, int> GetStage(string search)
        {
            switch (search)
            {
                case "BasicDetails":
                    return new Tuple<int, int>(0, (int)ProposalStagesCompletedEnum.BasicDetailsApp1Address);
                case "BasicDetailsApplicant2":
                    return new Tuple<int, int>((int)ProposalStagesCompletedEnum.BasicDetailsApp1Address, (int)sanctionStage1App1);
                case "SanctionStage1":
                    return new Tuple<int, int>((int)ProposalStagesCompletedEnum.BasicDetailsApp2, (int)sanctionStage1App1);
                case "SanctionStage1Applicant2":
                    return new Tuple<int, int>((int)ProposalStagesCompletedEnum.Sanction1App2Personal, (int)ProposalStagesEnum.Scored);
                case "Score":
                    return new Tuple<int, int>((int)ProposalStagesCompletedEnum.Sanction1App2Financial, (int)sanctionStage2App1);
                case "SanctionStage2":
                    return new Tuple<int, int>((int)ProposalStagesCompletedEnum.Scored, (int)sanctionStage2App1);
                case "SanctionStage2Applicant2":
                    return new Tuple<int, int>((int)ProposalStagesCompletedEnum.Sanction2App1References, (int)ProposalStagesEnum.Sanction2App2References);
                case "DocumentConfirmation":
                    return new Tuple<int, int>((int)ProposalStagesCompletedEnum.Sanction2App2References, (int)ProposalStagesEnum.DocumentConfirmation);
                default:
                    return new Tuple<int, int>(0, 0);
            }
        }

        public static ProposalStagesEnum Sanction1App1
        {
            get
            {
                return sanctionStage1App1;
            }
        }
        public static ProposalStagesEnum Sanction1App2
        {
            get
            {
                return sanctionStage1App2;
            }
        }
        public static ProposalStagesEnum Sanction2App1
        {
            get
            {
                return sanctionStage2App1;
            }
        }
    }
}