using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using SsasHelper;
using Microsoft.AnalysisServices;
using System.IO;

namespace SsasHelperSampleCli
{
    class Program
    {
        /// <summary>
        /// This is a Console app to show how the SsasHelper library
        /// can be used.  Download the Adventure Works sample from
        /// http://msftdbprodsamples.codeplex.com/Release/ProjectReleases.aspx?ReleaseId=18407
        /// or use your own (you probably want to take a backup first, just in case).  I created
        /// a copy of the project for verification purposes (some functions will change the source).
        /// 
        /// I've tested this on my current project and on Adeventure Works, and everything works fine.
        /// This was tested on SQL Server Analysis Services 2008 CU3.  It should work on other versions,
        /// but was not tested on them.
        /// 
        /// USE AT YOUR OWN RISK.  IT WORKS ON MY MACHINE.
        /// 
        /// I used Beyond Compare (http://www.scootersoftware.com/) to compare
        /// the output of the library with the original to verify that no unintended
        /// results occur.
        /// </summary>
        /// <param name="args"></param>
        static void Main(string[] args)
        {
            string projectFile = string.Empty;
            string projectDirectory = string.Empty;
            string outputDirectory = string.Empty;
            string asDatabaseFile = string.Empty;
            string inputFile = string.Empty;
            string outputFile = string.Empty;
            int requiredArguements;

            // Clean a project directory
            if (args.Length == 0)
            {
                DisplayHelp();
                return;
            }

            try
            {
                // Clean a project
                if (args[0] == "/C")
                {
                    requiredArguements = 2;
                    ValidateArguementCount(args, requiredArguements);
                    projectDirectory = args[1];

                    // Clean a SSAS project directory to prepare for a merge
                    CleanSsasProject(projectDirectory);
                }
                // Build a .ASDatabase file
                else if (args[0] == "/B")
                {
                    requiredArguements = 3;
                    ValidateArguementCount(args, requiredArguements);
                    projectDirectory = args[1];
                    asDatabaseFile = args[2];

                    // Build a .ASDatabase file based on a project
                    BuildASDatabase(projectDirectory, asDatabaseFile);
                }
                // Serialize/De-serialize 
                else if (args[0] == "/S")
                {
                    requiredArguements = 3;
                    ValidateArguementCount(args, requiredArguements);
                    projectFile = args[1];
                    outputDirectory = args[2];

                    // Deserialize/Serialize a SSAS Project
                    CopyProject(projectFile, outputDirectory);
                }
                // Sort a file
                else if (args[0] == "/O")
                {
                    requiredArguements = 3;
                    ValidateArguementCount(args, requiredArguements);
                    inputFile = args[1];
                    outputFile = args[2];

                    SortFile(inputFile, outputFile);
                }
                // Validate a project
                else if (args[0] == "/V")
                {
                    requiredArguements = 2;
                    ValidateArguementCount(args, requiredArguements);
                    inputFile = args[1];

                    ValidateProject(inputFile);
                }
                // Otherwise, display help
                else
                {
                    DisplayHelp();
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("An error occurred");
                Console.WriteLine(ex.Message);
            }

            Console.ReadLine();
        }

        /// <summary>
        /// Verify the program was passed the right numbers of arguements for the function.
        /// </summary>
        /// <param name="args"></param>
        /// <param name="requiredArguements"></param>
        private static void ValidateArguementCount(string[] args, int requiredArguements)
        {
            if (args.Length != requiredArguements)
            {
                throw new ArgumentException(string.Format("This function requires {0} parameters.  Use the '/?' switch to get help.", requiredArguements-1));
            }
        }

        /// <summary>
        /// Display instructions on how to use the program.
        /// </summary>
        private static void DisplayHelp()
        {
            Console.WriteLine("NAME");
            Console.WriteLine("    SsasHelperSampleCli");
            Console.WriteLine("    ");
            Console.WriteLine("SYNOPSIS");
            Console.WriteLine("    Sample project to demonstrate SSAS Helper library functions.");
            Console.WriteLine("");
            Console.WriteLine("SYNTAX");
            Console.WriteLine("    SsasHelperSampleCli.exe [/C | /B | /S | /V | /?] [Project Directory] ");
            Console.WriteLine("      [Project File] [.ASDatabase File] [Target Directory] [SSAS File] ");
            Console.WriteLine("      [Target Filename]");
            Console.WriteLine("Project Directory   A directory that contains a SSAS project.");
            Console.WriteLine("Project File        A SSAS .dwproj file. ");
            Console.WriteLine(".ASDatabase File    .ASDatabase file to generate.");
            Console.WriteLine("Target Directory    Directory to write output to.    ");
            Console.WriteLine("SSAS File           A SSAS project file (.cube, .dsv, .dim, etc.).");
            Console.WriteLine("Target Filename     A filename to write output to.");
            Console.WriteLine("/C      Clean a SSAS Project Directory.  Requires [Project Directory]");
            Console.WriteLine("/B      Builds a .ASDatabase file.  Requires [Project File] and ");
            Console.WriteLine("        [.ASDatabase File] to create.");
            Console.WriteLine("/S      Serialize/De-serialize a project.  Requires [Project File]");
            Console.WriteLine("        and a [Target Directory] to output serialized files.");
            Console.WriteLine("/V      Validate a project.  Requires [Project File].");
            Console.WriteLine("/O      Sort a SSAS file.  Requires a [SSAS File] and a [Target Filename] ");
            Console.WriteLine("/?      Display help.");
            Console.WriteLine("");
            Console.WriteLine("EXAMPLE");
            Console.WriteLine("    Build a .ASDatabase file from an existing SSAS project");
            Console.WriteLine("    SsasHelperSampleCli.exe /B \"C:\\Test\\enterprise_Build\\Adventure Works ");
            Console.WriteLine("    DW 2008.dwproj\" \"C:\\Test\\Test.ASDatabase\"");
            Console.WriteLine("    ");
            Console.WriteLine("    Clean a SSAS Project");
            Console.WriteLine("    SsasHelperSampleCli.exe /C \"C:\\Test\\enterprise_Clean\" ");
            Console.WriteLine("    ");
            Console.WriteLine("    Serialize/De-Serialize a SSAS Project");
            Console.WriteLine("    SsasHelperSampleCli.exe /S \"C:\\Test\\enterprise_Serialize\\Adventure Works ");
            Console.WriteLine("    DW 2008.dwproj\" \"C:\\Test\\enterprise_Serialize_Target\" ");
            Console.WriteLine("    ");
            Console.WriteLine("    Sort a file");
            Console.WriteLine("    SsasHelperSampleCli.exe /O \"C:\\Test\\enterprise_Serialize\\Adventure ");
            Console.WriteLine("    Works.partitions\" \"C:\\Test\\enterprise_Serialize\\Adventure Works.partitions.Ordered\"");
            Console.WriteLine("    ");
            Console.WriteLine("DESCRIPTION");
            Console.WriteLine("    The SSAS Helper library contains functions to Serialize an AMO Database");
            Console.WriteLine("    into project files, de-searialize project files into an AMO Database,");
            Console.WriteLine("    generate an .ASDatabase file from an AMO Database, and clean ");
            Console.WriteLine("    project files of volatile fields.");
            Console.WriteLine("");
            Console.WriteLine("RELATED LINKS");
            Console.WriteLine("    http://sqlsrvanalysissrvcs.codeplex.com");
            Console.WriteLine("    http://agilebi.com/cs/blogs/ddarden");
        }

        /// <summary>
        /// Build a .ASDatabase file based on a SSAS Project
        /// </summary>
        private static void BuildASDatabase(string ssasProjectFile, string targetASDatabaseFile)
        {
            Console.WriteLine("Build a SSAS .ASDatabase file");
            Console.WriteLine("-----------------------------");
            Console.WriteLine(string.Format("Project File: {0}", ssasProjectFile));
            Console.WriteLine(string.Format("Output File : {0}", targetASDatabaseFile));

            ProjectHelper.GenerateASDatabaseFile(ssasProjectFile, targetASDatabaseFile);

            Console.WriteLine(".ASDatabase file created!");
        }

        /// <summary>
        /// Clean a SSAS Project
        /// </summary>
        private static void CleanSsasProject(string ssasProjectDirectory)
        {
            Console.WriteLine("Clean a SSAS Project");
            Console.WriteLine("--------------------");
            Console.WriteLine(string.Format("Project Directory: {0}", ssasProjectDirectory));

            int filesInspectedCount = 0;
            int filesAlteredCount = 0;
            int filesCleanedCount = 0;

            ProjectHelper.CleanSsasProjectDirectory(ssasProjectDirectory, "*.cube,*.dim,*.dsv,*.dmm,*.role"
                , SearchOption.AllDirectories, true, true, true
                , out filesInspectedCount, out filesCleanedCount, out filesAlteredCount);

            Console.WriteLine(string.Format("Files Inspected: {0}", filesInspectedCount));
            Console.WriteLine(string.Format("Files Altered  : {0}", filesAlteredCount));
            Console.WriteLine(string.Format("Files Cleaned  : {0}", filesCleanedCount));

            Console.WriteLine("Project cleaned!");
        }

        /// <summary>
        /// Deserialize a project into AMO objects, the serialize them back into objects
        /// </summary>
        static void CopyProject(string ssasProjectFile, string ssasNewProjectDirectory)
        {
            Console.WriteLine("Copy a SSAS Project");
            Console.WriteLine("-------------------");
            Console.WriteLine(string.Format("Project File    : {0}", ssasProjectFile));
            Console.WriteLine(string.Format("Output Directory: {0}", ssasNewProjectDirectory));

            Database database;

            // Load a SSAS database object based on a BIDS project
            database = ProjectHelper.DeserializeProject(ssasProjectFile);

            // ... Manipulate the database using AMO goes here ...

            ProjectHelper.SerializeProject(database, ssasNewProjectDirectory);

            Console.WriteLine("Project copied!");
        }

        /// <summary>
        /// Validate a SSAS project to ensure there are no errors.
        /// </summary>
        static void ValidateProject(string ssasProjectFile)
        {
            Console.WriteLine("Validate a SSAS Project");
            Console.WriteLine("-----------------------");
            Console.WriteLine(string.Format("Project File: {0}", ssasProjectFile));

            Database database;
            bool isValid = false;

            // Load a SSAS database object based on a BIDS project
            database = ProjectHelper.DeserializeProject(ssasProjectFile);

            // ... Verify our project doesn't have any errors ...
            ValidationResultCollection results;
            
            // Specify "Developer" edition for this validation test.
            isValid = ProjectHelper.ValidateDatabase(database, "Developer", out results);

            foreach (ValidationResult result in results)
            {
                Console.WriteLine(string.Format("{0}", result.Description));
            }

            Console.WriteLine(string.Format("Project is Valid?  {0}", isValid));

            Console.WriteLine("Project validated!");
        }

        /// <summary>
        /// Sort an Analysis Services project file
        /// </summary>
        /// <param name="inputFilename">File to sort</param>
        /// <param name="outputFilename">Output filename</param>
        static void SortFile(string inputFilename, string outputFilename)
        {
            Console.WriteLine("Sort an SSAS file");
            Console.WriteLine("-----------------");
            Console.WriteLine(string.Format("Input : {0}", inputFilename));
            Console.WriteLine(string.Format("Output: {0}", outputFilename));
            ProjectHelper.SortSsasFile(inputFilename, outputFilename);
            Console.WriteLine("File sorted!");
        }
    }
}
