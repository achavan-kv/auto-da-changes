namespace Blue.Cosacs.Merchandising.Infrastructure
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Dynamic;
    using System.Linq;
    using System.Net;
    using System.Reflection;
    using Blue.Collections.Generic;
    using Blue.Cosacs.Merchandising.Helpers;
    using Blue.Solr;
    using Newtonsoft.Json;

    public class Lunr
    {
        private string FormatQuery(string q)
        {
            if (!q.EndsWith("*"))
            {
                q = q + "*";
            }
            return string.Format("{{\"query\":\"{0}\"}}", q);
        }
        private ResponseDocument[] DeserializeSolrResult(string result)
        {
            var solrResult = JsonConvert.DeserializeObject<Result>(result);
            return solrResult.Response.Docs;
        }

        private ResponseDocument[] SolrSearch(string q, string fields, string filter)
        {
            string result;
            try
            {
                result = new Query().SelectAllRows(FormatQuery(q), fields, filter);
            }
            catch (WebException ex)
            {
                // swallow bad solr queries
                if (ex.Status == WebExceptionStatus.ProtocolError)
                {
                    var response = ex.Response as HttpWebResponse;
                    if (response != null && response.StatusCode != HttpStatusCode.BadRequest)
                    {
                        throw;
                    }
                }
                return new ResponseDocument[0];
            }
            return DeserializeSolrResult(result);
        }
        private static object DeserializeSolrObject(object obj, PropertyInfo prop)
        {
            var str = obj.ToString();
            if (str.Length > 6 && str[6] == '{')
            {
                str = str.Replace("\"{", "{").Replace("}\"", "}").Replace(@"\", string.Empty);
            }
            return JsonConvert.DeserializeObject(str, prop.PropertyType);
        }

        public List<dynamic> Search(string q, string filter, string fieldsCsv)
        {
            var responseDocuments = SolrSearch(q, fieldsCsv, filter);
            return responseDocuments.Select(d =>
            {
                dynamic obj = new ExpandoObject();
                var dict = (IDictionary<string, object>)obj;
                d.Keys.ForEach(k => dict[k] = d[k]);
                return obj;
            }).ToList();
        }

        public List<TResult> Search<TResult>(string q, string filter) where TResult : class, new()
        {
            var type = typeof(TResult);
            var props = type.GetModelProperties();
            var propNames = props.Select(p => p.Name).ToList();
            var fields = string.Join(",", propNames);
            var responseDocuments = SolrSearch(q, fields, filter);

            var results = responseDocuments.Select(r => new TResult()).ToList();

            foreach (var prop in props)
            {
                for (int i = 0; i < responseDocuments.Length; i++)
                {
                    if (!responseDocuments[i].ContainsKey(prop.Name))
                    {
                        if (typeof(IEnumerable).IsAssignableFrom(prop.PropertyType))
                        {
                            prop.SetValue(results[i], Activator.CreateInstance(prop.PropertyType), null);
                        }
                        continue;
                    }

                    var obj = responseDocuments[i][prop.Name];
                    if (typeof(IConvertible).IsAssignableFrom(prop.PropertyType))
                    {
                        obj = Convert.ChangeType(obj, prop.PropertyType);
                    }
                    else
                    {
                        obj = DeserializeSolrObject(obj, prop);
                    }
                    prop.SetValue(results[i], obj, null);
                }
            }
            return results;
        }
    }
}
