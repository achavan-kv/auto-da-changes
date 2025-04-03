using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Blue.Events;
using System.Text;
using System.IO;
using Newtonsoft.Json.Linq;
using System.Reflection;
using System.Drawing;

public static class HtmlExtensions
{
    public static string Version(this HtmlHelper html)
    {
        var assembly = Assembly.GetExecutingAssembly();
        var assemblyName = assembly.GetName();
        var version = assemblyName.Version;
        return version.ToString();
    }

    public static void JsonToHtml(this HtmlHelper html, IEvent e)
    {
        var json = e.ToJson();
        var w = html.ViewContext.Writer;

        JsonToHtml(html, w, json);
    }

    private static void JsonToHtml(HtmlHelper html, TextWriter w, JToken o)
    {
        switch (o.Type)
        {
            case JTokenType.Array: JsonToHtml(html, w, (JArray)o); break;
            case JTokenType.Object: JsonToHtml(html, w, (JObject)o); break;
            case JTokenType.Property: JsonToHtml(html, w, (JProperty)o); break;

            case JTokenType.None: break;
            case JTokenType.Null: break;

            case JTokenType.Boolean: JsonToHtml(html, w, (JValue)o); break;
            case JTokenType.Bytes: JsonToHtml(html, w, (JValue)o); break;
            case JTokenType.Date: JsonToHtml(html, w, (JValue)o); break;
            case JTokenType.Float: JsonToHtml(html, w, (JValue)o); break;
            case JTokenType.Guid: JsonToHtml(html, w, (JValue)o); break;
            case JTokenType.Integer: JsonToHtml(html, w, (JValue)o); break;
            case JTokenType.String: JsonToHtml(html, w, (JValue)o); break;
            case JTokenType.TimeSpan: JsonToHtml(html, w, (JValue)o); break;
        }
    }

    private static void JsonToHtml(HtmlHelper html, TextWriter w, JObject o)
    {
        w.Write("<div class=fs>");
        foreach (var token in o.Children())
            JsonToHtml(html, w, token);
        w.Write("</div>");
    }

    private static void JsonToHtml(HtmlHelper html, TextWriter w, JProperty p)
    {
        // <div class=field><div class=name>${name}</div><div class=value>${value}</div></div>
        w.Write("<div class=f><div class=n>");
        w.Write(html.Encode(Humanize(p.Name)));
        w.Write("</div><div class=v>");
        JsonToHtml(html, w, p.Value);
        w.Write("</div></div>");
    }

    private static void JsonToHtml(HtmlHelper html, TextWriter w, JArray a)
    {
        if (IsTable(a))
        {
            w.Write("<table class=t><thead><tr>");
            var cols = TableColumns(a);
            foreach (var col in cols)
            {
                w.Write("<th>");
                w.Write(html.Encode(Humanize(col)));
                w.Write("</th>");
            }

            w.Write("</tr></thead><tbody>");
            foreach (var item in a)
            {
                w.Write("<tr>");
                foreach (var col in cols)
                {
                    var v = (JValue)item[col];
                    w.Write("<td>");
                    JsonToHtml(html, w, v);
                    w.Write("</td>");
                }
                w.Write("</tr>");
            }
            w.Write("</tbody></table>");
            return;
        }

        // if is not table
        w.Write("<div class=a>");
        var i = 0;
        foreach (var item in a)
        {
            w.Write("<div class=ai>");
            w.Write("<span>[");
            w.Write(++i);
            w.Write('/');
            w.Write(a.Count);
            w.Write("]</span>");
            JsonToHtml(html, w, item);
            w.Write("</div>");
        }
        w.Write("</div>");
    }

    /// <summary>
    /// Detect if array is a table (meaning it's not deep, only has scalars in the row objects)
    /// </summary>
    private static bool IsTable(JArray a)
    {
        foreach (var item in a)
            if (!IsShallow(item))
                return false;
        return true;
    }

    private static HashSet<string> TableColumns(JArray a)
    {
        var cols = new HashSet<string>();
        foreach (var item in a)
        {
            foreach (JProperty property in item.Children())
            {
                if (!cols.Contains(property.Name))
                    cols.Add(property.Name);
            }
        }
        return cols;
    }

    private static bool IsShallow(JToken o)
    {
        foreach (var token in o.Children())
        {
            if (token.Type != JTokenType.Property)
                return false;
            var property = (JProperty)token;
            if (!(property.Value is JValue))
                return false;
        }
        return true;
    }

    private static void JsonToHtml(HtmlHelper html, TextWriter w, JValue v)
    {
        if (v.Type == JTokenType.Date)
        {
            var d = (DateTime)v.Value;
            w.Write(d.ToString(d.Date == d ? "yyyy-MM-dd" : "yyyy-MM-dd HH:mm:ss"));
        }
        else w.Write(html.Encode(v.Value));

        /*
        var matchDate = /\/Date\((\d+)((\+|\-)\d{4})\)\//.exec(value);
        if (matchDate != null) {
            var ticks = parseInt(matchDate[1]);
            var timezone = matchDate[2];
            var date = new Date(ticks);
            if (isValidDate(date)) {
                value = date.format('yyyy-mm-dd HH:MM:ss');
                if (timezone != "+0000")
                    value += timezone
            }
        }
        */
    }

    private static string Humanize(string s)
    {
        var sb = new StringBuilder();
        var previous = ' ';
        foreach (var c in s)
        {
            if (sb.Length == 0)
                sb.Append(char.ToUpper(c));
            else if (char.IsUpper(c) && !(previous == 'I' && c == 'D'))
                sb.Append(' ').Append(c);
            else
                sb.Append(c);
            previous = c;
        }
        return sb.ToString();
    }

    public static StylesheetResult Stylesheet(this Controller controller, string viewName, object model)
    {
        if (string.IsNullOrEmpty(viewName))
            throw new ArgumentNullException("viewName");
        if (model != null)
            controller.ViewData.Model = model;
        return new StylesheetResult
        {
            ViewName = viewName,
            ViewData = controller.ViewData,
            TempData = controller.TempData,
        };
    }

}
