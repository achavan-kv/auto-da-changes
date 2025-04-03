using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Blue.Cosacs.Sales.Models;
using System.Text.RegularExpressions;

namespace Blue.Cosacs.Sales.Repositories
{
    public class ContractsRepository
    {

        public ContractsRepository(IClock clock)
        {
            this.clock = clock;
        }

        private readonly IClock clock;

        //Method to load contracts setup
        public List<ContractsSetupDto> GetContracts()
        {
            using (var scope = Context.Read())
            {
                List<ContractsSetupDto> contractSetup = new List<ContractsSetupDto>();
                var items = new StringBuilder();
                var categories = new StringBuilder();
                var currentContractName = string.Empty;

                //Load all contracts 
                var linkedContracts = scope.Context.LinkedContractsSetupView.OrderBy(l => l.Contract).ToList();

                //Loop through each contract (multiple lines originally saved for a contract for each item, category)
                //Need to combine the items and categories into a string seperated by coma for each contract
                for (var i = 0; i <= linkedContracts.Count(); i++)
                {
                    //If the contract does not have anything setup for it
                    if (i != linkedContracts.Count() && (linkedContracts[i].ItemNo == string.Empty && linkedContracts[i].Category == 0))
                    {
                        contractSetup.Add(new ContractsSetupDto
                        {
                            ContractName = linkedContracts[i].Contract,
                            Items = linkedContracts[i].ItemNo,
                            Categories = string.Empty
                        });

                    }
                    else
                    { 

                        if ( i == linkedContracts.Count() || currentContractName == string.Empty || currentContractName != linkedContracts[i].Contract)
                        {

                            if (items.Length > 0 || categories.Length > 0)
                            {
                                contractSetup.Add(new ContractsSetupDto{
                                    ContractName = currentContractName,
                                    Items = Convert.ToString(items),
                                    Categories = Convert.ToString(categories)
                                });

                                items.Clear();
                                categories.Clear();
                            }

                            if (i == linkedContracts.Count())
                            {
                                break;
                            }

                            currentContractName = linkedContracts[i].Contract;
                        }


                        if (currentContractName == linkedContracts[i].Contract)
                        {
                            if (linkedContracts[i].ItemNo != string.Empty)
                            {
                                if (items.Length > 0)
                                {
                                    items.Append(',');
                                }

                                items.Append(linkedContracts[i].ItemNo);
                            }

                            if (linkedContracts[i].Category != 0)
                            {
                                if (categories.Length > 0)
                                {
                                    categories.Append(',');
                                }

                                categories.Append(linkedContracts[i].Category);
                            }
                        }
                    }
                }

                return contractSetup;
                
            }
        }

        //Method to save contracts setup
        public void SaveContractsSetup(ContractsSetupDto[] contractsSetup)
        {
            using (var scope = Context.Write())
            {
                //Regular Expressions to check items and categories against
                var rItems = new Regex("[a-zA-Z0-9]");
                var rCategories = new Regex("[0-9]");

                var item = new StringBuilder();
                var category = new StringBuilder();
                var itemsToCheck = new List<string>();
                var categoriesToCheck = new List<short>();

                //Loop through each contract and save the items & categories
                for(var i = 0; i < contractsSetup.Count(); i++)
                {
                    itemsToCheck.Clear();
                    categoriesToCheck.Clear();

                    //Delete previous setup for a contract
                    var contractName = Convert.ToString(contractsSetup[i].ContractName);

                    var oldSetup = (from l in scope.Context.LinkedContracts select l).ToList();

                    oldSetup.ForEach(c =>
                    {
                        scope.Context.LinkedContracts.Remove(c);
                    });

                    //scope.Context.SaveChanges();

                        //Process items
                        var items = contractsSetup[i].Items;
                        if (items != null)
                        { 
                            for (var itemsIndex = 0; itemsIndex <= items.Count(); itemsIndex++)
                            {
                                if (itemsIndex < items.Count() && rItems.IsMatch(Convert.ToString(items[itemsIndex])))
                                {
                                    item.Append(items[itemsIndex]);
                                }
                                else
                                {
                                    if (item.Length > 0)
                                    {

                                        var linkedContracts = new LinkedContracts
                                        {
                                            Contract = contractName,
                                            ItemNo = item.ToString(),
                                            Category = 0
                                        };

                                        //Prevent duplicates from being saved.
                                        if (!itemsToCheck.Contains(item.ToString()))
                                        { 
                                            scope.Context.LinkedContracts.Add(linkedContracts);
                                        }

                                        itemsToCheck.Add(Convert.ToString(item));
                   
                                    }

                                    item.Clear();
                                }
                            }
                        }

                        //Process categories
                        var categories = contractsSetup[i].Categories;
                        if (categories != null)
                        { 
                            for (var categoriesIndex = 0; categoriesIndex <= categories.Count(); categoriesIndex++)
                            {
                                if (categoriesIndex < categories.Count() && rCategories.IsMatch(Convert.ToString(categories[categoriesIndex])))
                                {
                                    category.Append(categories[categoriesIndex]);
                                }
                                else
                                {
                                    if (category.Length > 0)
                                    {

                                        var linkedContracts = new LinkedContracts
                                        {
                                            Contract = contractName,
                                            ItemNo = string.Empty,
                                            Category = Convert.ToInt16(Convert.ToString(category))
                                        };

                                        //Prevent duplicates from being saved.
                                        if (!categoriesToCheck.Contains(Convert.ToInt16(Convert.ToString(category))))
                                        {
                                            scope.Context.LinkedContracts.Add(linkedContracts);
                                        }

                                        categoriesToCheck.Add(Convert.ToInt16(Convert.ToString(category)));
                                    }

                                    category.Clear();
                                }
                            }
                        }

                }
                scope.Context.SaveChanges();
                scope.Complete();
            }

        }

    }
}
