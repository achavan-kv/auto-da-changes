using System.Collections.Generic;
using System.Linq;

namespace Blue.Cosacs.Credit.Repositories
{
    public class CustomizeFieldsRepository : ICustomizeFieldsRepository
    {
        public List<Model.CustomizeFields> GetCustomizedFields()
        {
            var customizeFieldsList = new List<Model.CustomizeFields>();
            using (var scope = Context.Read())
            {
                var allFields = scope.Context.CustomizeFields.ToList();
                var custDistinctScreens = allFields.Select(p => new { p.ScreenId, p.ScreenLabel }).Distinct();

                foreach (var field in custDistinctScreens)
                {
                    var customizeField = new Model.CustomizeFields();
                    customizeField.ScreenLabel = field.ScreenLabel;
                    customizeField.ScreenId = field.ScreenId;

                    var distinctSections = allFields.Where(p => p.ScreenId == field.ScreenId).Select(p => p.Section).Distinct();
                    var sectionList = new List<Model.Section>();

                    foreach (var section in distinctSections)
                    {
                        var sectionFields = new Model.Section()
                        {
                            SectionName = section
                        };

                        var sectionFieldsList = allFields.Where(p => p.Section == section &&
                                                        p.ScreenId == field.ScreenId).Distinct();

                        var fieldsList = new List<Model.Field>();

                        foreach (var item in sectionFieldsList)
                        {
                            var f = new Model.Field()
                            {
                                FieldLookup = item.FieldLookup,
                                Id = item.FieldId,
                                Label = item.FieldName,
                                Length = item.Length,
                                Lookup = item.Lookup,
                                Readonly = item.Readonly,
                                Required = item.Required,
                                Type = item.Type,
                                Validation = item.Validation,
                                Values = item.Values,
                                Visible = item.Visible,
                                FieldOrder = item.FieldOrder,
                                RequiredValues = item.RequiredValues
                            };

                            fieldsList.Add(f);
                        }

                        sectionFields.Fields = fieldsList;

                        sectionList.Add(sectionFields);
                    }

                    customizeField.Sections = sectionList;
                    customizeFieldsList.Add(customizeField);
                }

                return customizeFieldsList;
            }
        }

        public void SaveCustomizedField(Model.Field field)
        {
            using (var scope = Context.Write())
            {
                var custField = scope.Context.CustomizeFields.Where(p => p.FieldId == field.Id && p.ScreenId == field.ScreenId).FirstOrDefault();

                if (custField != null)
                {
                    custField.Readonly = field.Readonly;
                    custField.Required = field.Required;
                    custField.Visible = field.Visible;
                    custField.RequiredValues = field.RequiredValues;
                }
                scope.Context.SaveChanges();
                scope.Complete();
            }
        }
    }
}
