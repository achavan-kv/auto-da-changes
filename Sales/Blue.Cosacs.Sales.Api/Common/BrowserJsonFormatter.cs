﻿using System;
using System.Net.Http.Formatting;
using System.Net.Http.Headers;

namespace Blue.Cosacs.Sales.Api.Common
{
  public class BrowserJsonFormatter : JsonMediaTypeFormatter
  {
    public BrowserJsonFormatter()
    {
      SupportedMediaTypes.Add(new MediaTypeHeaderValue("text/html"));
    }

    public override void SetDefaultContentHeaders(Type type, HttpContentHeaders headers, MediaTypeHeaderValue mediaType)
    {
      base.SetDefaultContentHeaders(type, headers, mediaType);
      headers.ContentType = new MediaTypeHeaderValue("application/json");
    }
  }
}