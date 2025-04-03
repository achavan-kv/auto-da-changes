/* 
Author: Suresh -IGT
Date: Feb 1st
Description:JM BlueStart-Pre-Qualification API Exception Handling
 */
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Web;
using System.Web.Http.Filters;
using Unicomer.Cosacs.Model.Exceptions;

namespace Blue.Cosacs.Unipay.Web.Exceptions
{
    public class HandleExceptionAttribute : ExceptionFilterAttribute
    {
        private static readonly JsonSerializerSettings serializerOptions = new JsonSerializerSettings
        {
            ContractResolver = new DefaultContractResolver
            {
                NamingStrategy = new CamelCaseNamingStrategy()
            }
        };
        public override void OnException(HttpActionExecutedContext context)
        {
            if (context.Exception is ArgumentNullException)
            {
                HandleArgumentNullException(context);
            }
            else if (context.Exception is ValidationException)
            {
                HandleValidationException(context);
            }
            else if (context.Exception is EmmaApiException)
            {
                HandleEmmaApiException(context);
            }
            else if (context.Exception is MambuApiException)
            {
                HandleMambuApiException(context);
            }
            else if (context.Exception is WorkflowApiException)
            {
                HandleWorkflowApiException(context);
            }
            else if (context.Exception is DBException)
            {
                HandleDBException(context);
            }
            else
            {
                RaiseServerError(context);
            }
        }
        private static void HandleArgumentNullException(HttpActionExecutedContext context)
        {
            var apiException = context.Exception as ArgumentNullException;
            string content = "";

            string errorLogId = "";

            try
            {
                errorLogId = LogException(context, new { apiException.ParamName, apiException.Message });
            }
            catch
            { }

            var generalResponse = new GeneralResponse("Api Request ArgumentNull")
            {
                Errors = new List<Error> { }
            };

            generalResponse.ResponseDescription += " with errorLogId-" + errorLogId;

            var response = new ErrorResponse
            {
                GeneralResponse = new List<GeneralResponse> { generalResponse }
            };

            if (apiException.Message != null)
            {
                response.GeneralResponse.FirstOrDefault().Errors.Add(
                                    new Error
                                    {
                                        Detail = apiException.Message,
                                        Id = "",
                                        Title = "Api Request ArgumentNull Error"
                                    });
            }
            else
            {
                response.GeneralResponse.FirstOrDefault().Errors.Add(
                new Error
                {
                    Detail = "InternalServerError",
                    Id = context.Request.GetCorrelationId().ToString(),
                    Title = "Api Request ArgumentNull Error"
                });
            }

            content = JsonConvert.SerializeObject(response, serializerOptions);

            context.Response = new HttpResponseMessage(HttpStatusCode.BadRequest)
            {
                Content = new StringContent(content)
            };
        }

        private static void HandleValidationException(HttpActionExecutedContext context)
        {
            var apiException = context.Exception as ValidationException;
            string content = "";

            string errorLogId = "";

            try
            {
                errorLogId = LogException(context, new { apiException.RequestModel, apiException.ErrorMessages });
            }
            catch
            { }

            var generalResponse = new GeneralResponse("Api Validation")
            {
                Errors = new List<Error> { }
            };

            generalResponse.ResponseDescription += " with errorLogId-" + errorLogId;

            var response = new ErrorResponse
            {
                GeneralResponse = new List<GeneralResponse> { generalResponse }
            };

            if (apiException.ErrorMessages != null && apiException.ErrorMessages.Any())
            {
                foreach (var errorMessage in apiException.ErrorMessages)
                {
                    response.GeneralResponse.FirstOrDefault().Errors.Add(
                                    new Error
                                    {
                                        Detail = errorMessage.Detail,
                                        Id = errorMessage.Id,
                                        Title = "Api Validation Error"
                                    });
                }
            }
            else
            {
                response.GeneralResponse.FirstOrDefault().Errors.Add(
                new Error
                {
                    Detail = "InternalServerError",
                    Id = context.Request.GetCorrelationId().ToString(),
                    Title = "Validation Error"
                });
            }

            content = JsonConvert.SerializeObject(response, serializerOptions);

            context.Response = new HttpResponseMessage(HttpStatusCode.BadRequest)
            {
                Content = new StringContent(content)
            };
        }
        private static void HandleDBException(HttpActionExecutedContext context)
        {
            var dbException = context.Exception as DBException;

            string errorLogId = "";

            try
            {
                errorLogId = LogException(context, new { dbException.RequestModel });
            }
            catch
            { }

            var generalResponse = new GeneralResponse("DB Server")
            {
                Errors = new List<Error> { }
            };

            generalResponse.ResponseDescription += " with errorLogId-" + errorLogId;

            var response = new ErrorResponse
            {
                GeneralResponse = new List<GeneralResponse> { generalResponse }
            };

            response.GeneralResponse.FirstOrDefault().Errors.Add(
                new Error
                {
                    Code = "UnipayServerError",
                    Detail = "DB Server Error Occured",
                    Id = errorLogId,
                });

            var content = JsonConvert.SerializeObject(response, serializerOptions);

            context.Response = new HttpResponseMessage(HttpStatusCode.BadRequest)
            {
                Content = new StringContent(content)
            };
        }

        private static void HandleEmmaApiException(HttpActionExecutedContext context)
        {
            var apiException = context.Exception as EmmaApiException;

            string errorLogId = "";

            try
            {
                errorLogId = LogException(context, new { apiException.RequestModel, apiException.ErrorResponse });
            }
            catch
            { }

            var generalResponse = new GeneralResponse("Mambu Api")
            {
                Errors = new List<Error> { }
            };

            generalResponse.ResponseDescription += " with errorLogId-" + errorLogId;

            var response = new ErrorResponse
            {
                GeneralResponse = new List<GeneralResponse> { generalResponse }
            };

            if (apiException.ErrorResponse != null && apiException.ErrorResponse.Errors != null && apiException.ErrorResponse.Errors.Any())
            {
                foreach (var errorMessage in apiException.ErrorResponse.Errors)
                {
                    var error = new Error
                    {
                        Code = errorMessage.ErrorCode.ToString(),
                        Detail = errorMessage.ErrorReason,
                        Id = errorMessage.ErrorSource,
                        Title = "EmmaApiError"
                    };

                    if (!string.IsNullOrEmpty(errorMessage.ErrorSource))
                    {
                        error.Title = errorMessage.ErrorSource;
                    }
                    response.GeneralResponse.FirstOrDefault().Errors.Add(error);
                }
            }
            else
            {
                response.GeneralResponse.FirstOrDefault().Errors.Add(
                new Error
                {
                    Code = "EmmaAPIServerError",
                    Detail = JsonConvert.SerializeObject(apiException.ErrorResponse, serializerOptions),
                    Id = errorLogId,
                    Title = "EmmaApiError"
                });
            }

            var content = JsonConvert.SerializeObject(response, serializerOptions);

            context.Response = new HttpResponseMessage(HttpStatusCode.BadRequest)
            {
                Content = new StringContent(content)
            };
        }

        private static void HandleMambuApiException(HttpActionExecutedContext context)
        {
            var apiException = context.Exception as MambuApiException;

            string errorLogId = "";

            try
            {
                errorLogId = LogException(context, new { apiException.RequestModel, apiException.ErrorResponse });
            }
            catch
            { }

            var generalResponse = new GeneralResponse("Mambu Api")
            {
                Errors = new List<Error> { }
            };

            generalResponse.ResponseDescription += " with errorLogId-" + errorLogId;

            var response = new ErrorResponse
            {
                GeneralResponse = new List<GeneralResponse> { generalResponse }
            };

            if (apiException.ErrorResponse != null && apiException.ErrorResponse.Errors != null && apiException.ErrorResponse.Errors.Any())
            {
                foreach (var errorMessage in apiException.ErrorResponse.Errors)
                {
                    var error = new Error
                    {
                        Code = errorMessage.ErrorCode.ToString(),
                        Detail = errorMessage.ErrorReason,
                        Id = errorMessage.ErrorSource,
                        Title = "MambuApiError"
                    };

                    if (!string.IsNullOrEmpty(errorMessage.ErrorSource))
                    {
                        error.Title = errorMessage.ErrorSource;
                    }
                    response.GeneralResponse.FirstOrDefault().Errors.Add(error);
                }
            }
            else
            {
                response.GeneralResponse.FirstOrDefault().Errors.Add(
                new Error
                {
                    Code = "MambuAPIServerError",
                    Detail = JsonConvert.SerializeObject(apiException.ErrorResponse, serializerOptions),
                    Id = errorLogId,
                    Title = "MambuApiError"
                });
            }

            var content = JsonConvert.SerializeObject(response, serializerOptions);

            context.Response = new HttpResponseMessage(HttpStatusCode.BadRequest)
            {
                Content = new StringContent(content)
            };
        }

        private static void HandleWorkflowApiException(HttpActionExecutedContext context)
        {
            var apiException = context.Exception as WorkflowApiException;

            string errorLogId = "";

            try
            {
                errorLogId = LogException(context, new { apiException.RequestModel, apiException.ErrorResponse });
            }
            catch
            { }

            var generalResponse = new GeneralResponse("Workflow Api")
            {
                Errors = new List<Error> { }
            };

            generalResponse.ResponseDescription += " with errorLogId-" + errorLogId;

            var response = new ErrorResponse
            {
                GeneralResponse = new List<GeneralResponse> { generalResponse }
            };

            if (apiException.ErrorResponse != null &&
                apiException.ErrorResponse.GeneralResponse != null &&
                apiException.ErrorResponse.GeneralResponse.Errors != null)
            {
                var error = new Error
                {
                    Code = apiException.ErrorResponse.GeneralResponse.Errors.Code.ToString(),
                    Detail = apiException.ErrorResponse.GeneralResponse.Errors.Detail,
                    Id = apiException.ErrorResponse.GeneralResponse.Errors.Id,
                    Title = apiException.ErrorResponse.GeneralResponse.Errors.Title
                };

                response.GeneralResponse.FirstOrDefault().Errors.Add(error);
            }
            else
            {
                response.GeneralResponse.FirstOrDefault().Errors.Add(
                new Error
                {
                    Code = "WorkflowAPIServerError",
                    Detail = JsonConvert.SerializeObject(apiException.ErrorResponse, serializerOptions),
                    Id = errorLogId,
                    Title = "WorkflowApiError"
                });
            }

            var content = JsonConvert.SerializeObject(response, serializerOptions);

            context.Response = new HttpResponseMessage(HttpStatusCode.BadRequest)
            {
                Content = new StringContent(content)
            };
        }

        private static void RaiseServerError(HttpActionExecutedContext context)
        {
            string errorLogId = "";

            try
            {
                errorLogId = LogException(context, null);
            }
            catch
            { }

            var content = JsonConvert.SerializeObject(
                new ErrorResponse
                {
                    GeneralResponse = new List<GeneralResponse> {
                            new GeneralResponse ("Internal Server"){
                                Errors=new List<Error>{
                                    new Error {
                                            Code = "InternalServerError",
                                            Detail = "Server Error Occured",
                                            Id = errorLogId,
                                        }
                                }
                            }
                    }
                }, serializerOptions);

            context.Response = new HttpResponseMessage(HttpStatusCode.InternalServerError)
            {
                Content = new StringContent(content)
            };
        }

        private static string LogException(HttpActionExecutedContext httpActionExecutedContext, object executionModel)
        {
            string errorLogId = "";

            var context = HttpContext.Current;
            var log = global::Elmah.ErrorLog.GetDefault(HttpContext.Current);
            if (log != null)
            {
                try
                {

                    var error = new global::Elmah.Error(httpActionExecutedContext.Exception, HttpContext.Current);
                    string requestModel = "";
                    string requestBody = "";
                    string executionModelJosn = "";

                    if (executionModel != null)
                    {
                        try
                        {
                            executionModelJosn = JsonConvert.SerializeObject(executionModel, new JsonSerializerSettings { ReferenceLoopHandling = ReferenceLoopHandling.Ignore });
                        }
                        catch (Exception ex)
                        {
                            executionModelJosn = ex.Message;
                        }
                    }

                    error.ServerVariables.Set("executionModelJosn", executionModelJosn);

                    try
                    {
                        requestModel = JsonConvert.SerializeObject(httpActionExecutedContext.ActionContext.ActionArguments, new JsonSerializerSettings { ReferenceLoopHandling = ReferenceLoopHandling.Ignore });
                    }
                    catch (Exception ex)
                    {
                        requestModel = ex.Message;
                    }

                    error.ServerVariables.Set("requestModel", requestModel);

                    if (httpActionExecutedContext.Request.Content != null)
                    {
                        bool hasError = false;
                        ReadRequestBody(httpActionExecutedContext, context, ref requestBody, ref hasError);

                        if (hasError)
                        {
                            var jsonContentTask = httpActionExecutedContext.Request.Content.ReadAsStringAsync();
                            System.Threading.Tasks.Task.WaitAll(jsonContentTask);
                            string jsonContent = jsonContentTask.Result;
                            ReadRequestBody(httpActionExecutedContext, context, ref requestBody, ref hasError);
                        }
                    }

                    error.ServerVariables.Set("requestBody", requestBody);
                    errorLogId = log.Log(error);
                }
                catch (Exception ex)
                {
                    var error = new global::Elmah.Error(ex, HttpContext.Current);
                    errorLogId = log.Log(error);
                }
            }

            //global::Elmah.ErrorSignal.FromCurrentContext().Raise(exception);

            return errorLogId;
        }

        private static void ReadRequestBody(HttpActionExecutedContext httpActionExecutedContext, HttpContext context, ref string requestBody, ref bool hasError)
        {
            try
            {
                using (var stream = new MemoryStream())
                {
                    var httpContext = (HttpContextBase)httpActionExecutedContext.Request.Properties["MS_HttpContext"];
                    if (httpContext.Request.ContentLength > 0)
                    {
                        context.Request.InputStream.Seek(0, SeekOrigin.Begin);
                        context.Request.InputStream.CopyTo(stream);
                        requestBody = Encoding.UTF8.GetString(stream.ToArray());
                    }
                }
            }
            catch (Exception ex)
            {
                requestBody = ex.Message;
                hasError = true;
            }
        }
    }
}